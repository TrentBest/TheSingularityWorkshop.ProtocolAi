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
}
