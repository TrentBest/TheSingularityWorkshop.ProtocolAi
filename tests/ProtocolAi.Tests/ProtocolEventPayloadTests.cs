using TheSingularityWorkshop.ProtocolAi;
using Xunit;

namespace ProtocolAi.Tests;

public sealed class ProtocolEventPayloadTests
{
    [Fact]
    public void Observer_registration_request_round_trips_scope_and_version()
    {
        var payload = new ProtocolObserverRegistrationRequest(
            "observer-ci", "repository:TrentBest/ProtocolAi:development", "1.0", ProtocolObserverLifecycle.Active);

        var json = ProtocolEventPayloadJson.Serialize(payload);
        var restored = ProtocolEventPayloadJson.Deserialize<ProtocolObserverRegistrationRequest>(json);

        Assert.Equal(payload.ObserverId, restored.ObserverId);
        Assert.Equal(payload.Scope, restored.Scope);
        Assert.Equal(payload.ProtocolVersion, restored.ProtocolVersion);
        Assert.Equal(ProtocolObserverLifecycle.Active, restored.RequestedLifecycle);
        Assert.Contains("\"requestedLifecycle\":\"Active\"", json);
    }

    [Fact]
    public void Rejected_registration_requires_a_reason()
    {
        Assert.Throws<ArgumentException>(() => new ProtocolObserverRegistrationAcknowledgement(
            "observer-ci", "repository:owner/repo", "1.0", ProtocolObserverRegistrationStatus.Rejected));
    }

    [Theory]
    [InlineData(ProtocolEvaluationState.True)]
    [InlineData(ProtocolEvaluationState.False)]
    [InlineData(ProtocolEvaluationState.Unknown)]
    [InlineData(ProtocolEvaluationState.Incomplete)]
    public void Observer_evaluation_preserves_all_four_explicit_states(ProtocolEvaluationState state)
    {
        var payload = new ProtocolObserverEvaluation(
            "observer-ci", "build-and-test", "2", "source-event-17", state, "Evaluation detail.");

        var restored = ProtocolEventPayloadJson.Deserialize<ProtocolObserverEvaluation>(
            ProtocolEventPayloadJson.Serialize(payload));

        Assert.Equal(state, restored.State);
        Assert.Equal("source-event-17", restored.InputMessageId);
    }

    [Fact]
    public void Unknown_and_incomplete_are_distinct_from_true()
    {
        var unknown = new ProtocolObserverEvaluation("o", "p", "1", "m", ProtocolEvaluationState.Unknown, "No authoritative evidence.");
        var incomplete = new ProtocolObserverEvaluation("o", "p", "1", "m", ProtocolEvaluationState.Incomplete, "Required job is missing.");

        Assert.NotEqual(unknown.State, incomplete.State);
        Assert.NotEqual(ProtocolEvaluationState.True, unknown.State);
        Assert.NotEqual(ProtocolEvaluationState.True, incomplete.State);
    }

    [Fact]
    public void Gate_transition_round_trips_revision_and_superseded_message()
    {
        var payload = new ProtocolGateTransition(
            "release-gate", "3", ProtocolGateState.Invalidated, 12,
            "event-12", "required-checks", "A newer candidate invalidated the prior result.", "event-11");

        var restored = ProtocolEventPayloadJson.Deserialize<ProtocolGateTransition>(
            ProtocolEventPayloadJson.Serialize(payload));

        Assert.Equal(12, restored.CausalRevision);
        Assert.Equal(ProtocolGateState.Invalidated, restored.State);
        Assert.Equal("event-11", restored.SupersedesMessageId);
    }

    [Fact]
    public void Gate_transition_rejects_nonpositive_revision()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProtocolGateTransition(
            "gate", "1", ProtocolGateState.Satisfied, 0, "event", "predicate", "reason"));
    }

    [Fact]
    public void Command_outcomes_require_identity_and_rejection_reason()
    {
        Assert.Throws<ArgumentException>(() => new ProtocolCommandOutcome(
            ProtocolCommandOutcomeKind.Accepted, "proposal-1", null));
        Assert.Throws<ArgumentException>(() => new ProtocolCommandOutcome(
            ProtocolCommandOutcomeKind.Rejected, "proposal-1", null));
    }

    [Theory]
    [InlineData(ProtocolCommandOutcomeKind.Accepted)]
    [InlineData(ProtocolCommandOutcomeKind.Rejected)]
    [InlineData(ProtocolCommandOutcomeKind.Completed)]
    public void Command_outcome_round_trips_proposal_and_command_identity(ProtocolCommandOutcomeKind outcome)
    {
        var payload = outcome switch
        {
            ProtocolCommandOutcomeKind.Accepted => new ProtocolCommandOutcome(outcome, "proposal-1", "command-1"),
            ProtocolCommandOutcomeKind.Rejected => new ProtocolCommandOutcome(outcome, "proposal-1", null, "Host policy rejected the proposal."),
            _ => new ProtocolCommandOutcome(outcome, "proposal-1", "command-1")
        };

        var restored = ProtocolEventPayloadJson.Deserialize<ProtocolCommandOutcome>(
            ProtocolEventPayloadJson.Serialize(payload));

        Assert.Equal(outcome, restored.Outcome);
        Assert.Equal("proposal-1", restored.ProposalId);
        Assert.Equal(payload.CommandId, restored.CommandId);
    }

    [Fact]
    public void Typed_payload_deserializer_rejects_non_object_json()
    {
        Assert.Throws<ArgumentException>(() =>
            ProtocolEventPayloadJson.Deserialize<ProtocolObserverEvaluation>("[]"));
    }

    [Fact]
    public void Typed_envelope_factory_fixes_registration_event_kind()
    {
        var payload = new ProtocolObserverRegistrationRequest("observer", "repo:owner/name", "1.0");

        var envelope = ProtocolTypedEventEnvelope.Create(
            "message-1", "correlation-1", "producer-1", payload, DateTimeOffset.UtcNow);

        Assert.Equal(ProtocolEventKind.ObserverRegistrationRequested, envelope.Kind);
        Assert.Equal(payload.ObserverId,
            ProtocolEventPayloadJson.Deserialize<ProtocolObserverRegistrationRequest>(envelope.PayloadJson).ObserverId);
    }

    [Fact]
    public void Typed_envelope_factory_copies_gate_causality_into_envelope_metadata()
    {
        var payload = new ProtocolGateTransition(
            "gate", "1", ProtocolGateState.Satisfied, 7, "source-7", "predicate", "All required evidence is present.",
            "message-6");
        var provenance = CreateProvenance();

        var envelope = ProtocolTypedEventEnvelope.Create(
            "message-7", "correlation-1", "producer-1", payload, DateTimeOffset.UtcNow, provenance);

        Assert.Equal(ProtocolEventKind.GateTransition, envelope.Kind);
        Assert.Equal(7, envelope.CausalRevision);
        Assert.Equal("message-6", envelope.SupersedesMessageId);
        Assert.Same(provenance, envelope.Provenance);
    }

    [Theory]
    [InlineData(ProtocolCommandOutcomeKind.Accepted, ProtocolEventKind.CommandAccepted)]
    [InlineData(ProtocolCommandOutcomeKind.Rejected, ProtocolEventKind.CommandRejected)]
    [InlineData(ProtocolCommandOutcomeKind.Completed, ProtocolEventKind.CommandCompleted)]
    public void Typed_envelope_factory_maps_command_outcome_to_matching_event_kind(
        ProtocolCommandOutcomeKind outcome, ProtocolEventKind expectedKind)
    {
        var payload = outcome switch
        {
            ProtocolCommandOutcomeKind.Accepted => new ProtocolCommandOutcome(outcome, "proposal", "command"),
            ProtocolCommandOutcomeKind.Rejected => new ProtocolCommandOutcome(outcome, "proposal", null, "Rejected by host."),
            _ => new ProtocolCommandOutcome(outcome, "proposal", "command")
        };

        var envelope = ProtocolTypedEventEnvelope.Create(
            "message", "correlation", "producer", payload, DateTimeOffset.UtcNow, "idempotency-1");

        Assert.Equal(expectedKind, envelope.Kind);
        Assert.Equal("idempotency-1", envelope.IdempotencyKey);
    }

    [Fact]
    public void Typed_envelope_factory_requires_provenance_for_evaluation_events()
    {
        var payload = new ProtocolObserverEvaluation("observer", "predicate", "1", "source",
            ProtocolEvaluationState.True, "Evidence-backed result.");

        Assert.Throws<ArgumentNullException>(() => ProtocolTypedEventEnvelope.Create(
            "message", "correlation", "producer", payload, DateTimeOffset.UtcNow, null!));
    }

    [Fact]
    public void Source_event_round_trips_and_factory_requires_candidate_provenance()
    {
        var payload = new ProtocolSourceEvent("github-actions", "build-and-test.completed",
            "owner/repo@candidate-sha", ProtocolSourceEventOutcome.Succeeded, "All required checks passed.");
        var restored = ProtocolEventPayloadJson.Deserialize<ProtocolSourceEvent>(
            ProtocolEventPayloadJson.Serialize(payload));
        Assert.Equal(ProtocolSourceEventOutcome.Succeeded, restored.Outcome);
        Assert.Equal(payload.Subject, restored.Subject);

        var envelope = ProtocolTypedEventEnvelope.Create("source-1", "corr-1", "ci-host",
            payload, DateTimeOffset.UtcNow, CreateProvenance());
        Assert.Equal(ProtocolEventKind.SourceEvent, envelope.Kind);
        Assert.NotNull(envelope.Provenance);
    }

    [Fact]
    public void Downstream_notification_identifies_consumer_await_and_gate_revision()
    {
        var payload = new ProtocolDownstreamNotification("notice-1", "consumer/repo", "await-1",
            "gate-1", "2", ProtocolGateState.Satisfied, 9, "gate-transition-9",
            "The provider gate changed; reevaluate local prerequisites.");
        var restored = ProtocolEventPayloadJson.Deserialize<ProtocolDownstreamNotification>(
            ProtocolEventPayloadJson.Serialize(payload));
        Assert.Equal("consumer/repo", restored.ConsumerRepository);
        Assert.Equal("await-1", restored.AwaitId);
        Assert.Equal(9, restored.CausalRevision);

        var envelope = ProtocolTypedEventEnvelope.Create("notice-1", "corr-1", "host",
            payload, DateTimeOffset.UtcNow, CreateProvenance());
        Assert.Equal(ProtocolEventKind.DownstreamNotification, envelope.Kind);
        Assert.Equal(9, envelope.CausalRevision);
    }

    [Fact]
    public void Downstream_notification_rejects_nonpositive_revision_and_noncanonical_repository()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProtocolDownstreamNotification(
            "n", "consumer/repo", "await", "gate", "1", ProtocolGateState.Satisfied, 0, "m", "reason"));
        Assert.Throws<ArgumentException>(() => new ProtocolDownstreamNotification(
            "n", "not-a-repository", "await", "gate", "1", ProtocolGateState.Satisfied, 1, "m", "reason"));
    }

    private static ProtocolCandidateProvenance CreateProvenance() => new(
        "owner/repo", "refs/heads/development", new string('a', 40), new string('b', 40),
        new string('c', 40), "refs/heads/master", new string('d', 40), 42, 1,
        "build-and-test", "https://example.invalid/evidence", new string('e', 64));

}
