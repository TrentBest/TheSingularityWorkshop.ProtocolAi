using System.Text.Json;
using TheSingularityWorkshop.ProtocolAi;
using Xunit;

namespace ProtocolAi.Tests;

public sealed class ProtocolEventContractConformanceTests
{
    private static readonly DateTimeOffset OccurredAt =
        DateTimeOffset.Parse("2026-11-04T00:00:00Z");

    private static ProtocolCandidateProvenance Provenance() => new(
        "owner/repository", "refs/heads/development",
        new string('a', 40), new string('b', 40), new string('c', 40),
        "refs/heads/master", new string('d', 40), 123, 2,
        "build-and-test/test", "artifacts/evidence.json", new string('e', 64));

    [Fact]
    public void Every_typed_event_contract_round_trips_through_its_envelope()
    {
        var provenance = Provenance();

        var registrationRequest = new ProtocolObserverRegistrationRequest(
            "observer-1", "repository:owner/repository:development", "1.0");
        AssertPayloadRoundTrips(registrationRequest,
            ProtocolTypedEventEnvelope.Create("m1", "c1", "host", registrationRequest, OccurredAt));

        var registrationAcknowledgement = new ProtocolObserverRegistrationAcknowledgement(
            "observer-1", "repository:owner/repository:development", "1.0",
            ProtocolObserverRegistrationStatus.Rejected, "Scope is not permitted.");
        AssertPayloadRoundTrips(registrationAcknowledgement,
            ProtocolTypedEventEnvelope.Create("m2", "c1", "host", registrationAcknowledgement, OccurredAt));

        var sourceEvent = new ProtocolSourceEvent(
            "ci", "build.completed", "owner/repository@candidate",
            ProtocolSourceEventOutcome.Succeeded, "Build completed.");
        AssertPayloadRoundTrips(sourceEvent,
            ProtocolTypedEventEnvelope.Create("m3", "c1", "ci", sourceEvent, OccurredAt, provenance));

        var evaluation = new ProtocolObserverEvaluation(
            "observer-1", "required-checks", "2", "m3",
            ProtocolEvaluationState.Incomplete, "A required job is missing.");
        AssertPayloadRoundTrips(evaluation,
            ProtocolTypedEventEnvelope.Create("m4", "c1", "observer-host", evaluation, OccurredAt, provenance));

        var gateTransition = new ProtocolGateTransition(
            "release", "3", ProtocolGateState.Invalidated, 8,
            "m3", "required-checks", "Candidate changed.", "m7");
        AssertPayloadRoundTrips(gateTransition,
            ProtocolTypedEventEnvelope.Create("m8", "c1", "gate-host", gateTransition, OccurredAt, provenance));

        var notification = new ProtocolDownstreamNotification(
            "notice-1", "consumer/repository", "await-release", "release", "3",
            ProtocolGateState.Invalidated, 8, "m8", "Reevaluate local prerequisites.");
        AssertPayloadRoundTrips(notification,
            ProtocolTypedEventEnvelope.Create("m9", "c1", "gate-host", notification, OccurredAt, provenance));

        var accepted = new ProtocolCommandOutcome(
            ProtocolCommandOutcomeKind.Accepted, "proposal-1", "command-1");
        AssertPayloadRoundTrips(accepted,
            ProtocolTypedEventEnvelope.Create("m10", "c1", "command-host", accepted, OccurredAt, "idem-1"));

        var rejected = new ProtocolCommandOutcome(
            ProtocolCommandOutcomeKind.Rejected, "proposal-2", null, "Host policy rejected the proposal.");
        AssertPayloadRoundTrips(rejected,
            ProtocolTypedEventEnvelope.Create("m11", "c1", "command-host", rejected, OccurredAt, "idem-2"));

        var completed = new ProtocolCommandOutcome(
            ProtocolCommandOutcomeKind.Completed, "proposal-3", "command-3");
        AssertPayloadRoundTrips(completed,
            ProtocolTypedEventEnvelope.Create("m12", "c1", "command-host", completed, OccurredAt, "idem-3"));
    }

    [Theory]
    [InlineData(ProtocolCommandOutcomeKind.Accepted, ProtocolEventKind.CommandAccepted)]
    [InlineData(ProtocolCommandOutcomeKind.Rejected, ProtocolEventKind.CommandRejected)]
    [InlineData(ProtocolCommandOutcomeKind.Completed, ProtocolEventKind.CommandCompleted)]
    public void Command_payload_outcome_must_match_the_envelope_event_kind(
        ProtocolCommandOutcomeKind outcome, ProtocolEventKind expectedKind)
    {
        var payload = outcome switch
        {
            ProtocolCommandOutcomeKind.Accepted =>
                new ProtocolCommandOutcome(outcome, "proposal", "command"),
            ProtocolCommandOutcomeKind.Rejected =>
                new ProtocolCommandOutcome(outcome, "proposal", null, "Rejected."),
            _ => new ProtocolCommandOutcome(outcome, "proposal", "command")
        };

        var envelope = ProtocolTypedEventEnvelope.Create(
            "message", "correlation", "host", payload, OccurredAt, "idempotency");

        Assert.Equal(expectedKind, envelope.Kind);
        Assert.Equal(outcome,
            ProtocolTypedEventEnvelope.DeserializePayload<ProtocolCommandOutcome>(envelope).Outcome);

        var mismatchedKind = expectedKind == ProtocolEventKind.CommandAccepted
            ? ProtocolEventKind.CommandCompleted
            : ProtocolEventKind.CommandAccepted;
        var mismatched = new ProtocolEventEnvelope(
            "other-message", "correlation", "host", mismatchedKind, OccurredAt,
            envelope.PayloadJson, "idempotency");

        Assert.Throws<ArgumentException>(() =>
            ProtocolTypedEventEnvelope.DeserializePayload<ProtocolCommandOutcome>(mismatched));
    }

    [Fact]
    public void Typed_reader_rejects_downstream_revision_mismatch()
    {
        var payload = new ProtocolDownstreamNotification(
            "notice", "consumer/repository", "await", "gate", "1",
            ProtocolGateState.Satisfied, 4, "gate-message", "Reevaluate.");
        var envelope = new ProtocolEventEnvelope(
            "notice", "correlation", "host", ProtocolEventKind.DownstreamNotification,
            OccurredAt, ProtocolEventPayloadJson.Serialize(payload),
            provenance: Provenance(), causalRevision: 5);

        Assert.Throws<ArgumentException>(() =>
            ProtocolTypedEventEnvelope.DeserializePayload<ProtocolDownstreamNotification>(envelope));
    }

    private static void AssertPayloadRoundTrips<T>(T expected, ProtocolEventEnvelope envelope)
        where T : class
    {
        var restored = ProtocolTypedEventEnvelope.DeserializePayload<T>(envelope);
        Assert.Equal(
            JsonSerializer.Serialize(expected),
            JsonSerializer.Serialize(restored));
    }
}
