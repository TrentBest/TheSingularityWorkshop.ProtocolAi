using TheSingularityWorkshop.ProtocolAi;
using Xunit;

namespace ProtocolAi.Tests;

public sealed class ProtocolEventEnvelopeTests
{
    private static ProtocolCandidateProvenance Provenance() => new(
        "TrentBest/ProtocolAi", "refs/heads/development",
        new string('a', 40), new string('b', 40), new string('c', 40),
        "refs/heads/master", new string('d', 40), 12345, 2,
        "build-and-test/test", "artifacts/evidence.json", new string('e', 64));

    private static ProtocolEventEnvelope SourceEvent() => new(
        "message-1", "work-7", "CiAi", ProtocolEventKind.SourceEvent,
        DateTimeOffset.Parse("2026-11-03T06:00:00Z"), "{\"result\":\"success\"}", provenance: Provenance());

    [Fact]
    public void Event_envelope_round_trips_correlation_and_provenance()
    {
        var original = SourceEvent();
        var restored = ProtocolEventEnvelopeJson.Deserialize(ProtocolEventEnvelopeJson.Serialize(original));
        Assert.Equal(original.MessageId, restored.MessageId);
        Assert.Equal(original.CorrelationId, restored.CorrelationId);
        Assert.Equal(original.ProducerId, restored.ProducerId);
        Assert.Equal(original.Kind, restored.Kind);
        Assert.Equal(original.OccurredAt, restored.OccurredAt);
        Assert.Equal(original.Provenance!.CandidateSha, restored.Provenance!.CandidateSha);
        Assert.Equal(original.Provenance.EventSha, restored.Provenance.EventSha);
        Assert.Equal(original.Provenance.TestedCheckoutSha, restored.Provenance.TestedCheckoutSha);
        Assert.Equal(original.Provenance.WorkflowDefinitionSha, restored.Provenance.WorkflowDefinitionSha);
        Assert.Equal(original.Provenance.EvidenceSha256, restored.Provenance.EvidenceSha256);
    }

    [Fact]
    public void Wire_schema_and_kind_are_explicit()
    {
        var json = ProtocolEventEnvelopeJson.Serialize(SourceEvent());
        Assert.Contains("\"schema\":\"TheSingularityWorkshop.ProtocolAi.EventEnvelope\"", json);
        Assert.Contains("\"schemaVersion\":1", json);
        Assert.Contains("\"kind\":\"SourceEvent\"", json);
    }

    [Fact]
    public void Envelope_rejects_a_default_occurrence_timestamp()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProtocolEventEnvelope(
            "m1", "c1", "producer", ProtocolEventKind.ObserverRegistrationRequested,
            default, "{}"));
    }

    [Fact]
    public void Evidence_events_require_exact_candidate_provenance()
    {
        Assert.Throws<ArgumentException>(() => new ProtocolEventEnvelope(
            "m1", "c1", "CiAi", ProtocolEventKind.SourceEvent,
            DateTimeOffset.UtcNow, "{\"result\":\"success\"}"));
    }

    [Fact]
    public void Command_outcomes_require_an_idempotency_key()
    {
        Assert.Throws<ArgumentException>(() => new ProtocolEventEnvelope(
            "m1", "c1", "CommandAi", ProtocolEventKind.CommandAccepted,
            DateTimeOffset.UtcNow, "{\"proposalId\":\"p1\"}"));
    }

    [Fact]
    public void Gate_transitions_require_a_positive_causal_revision()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProtocolEventEnvelope(
            "m1", "c1", "CiAi", ProtocolEventKind.GateTransition,
            DateTimeOffset.UtcNow, "{\"state\":\"Satisfied\"}", provenance: Provenance()));
    }

    [Fact]
    public void Gate_transitions_can_name_the_superseded_message()
    {
        var envelope = new ProtocolEventEnvelope("m2", "c1", "CiAi",
            ProtocolEventKind.GateTransition, DateTimeOffset.UtcNow,
            "{\"state\":\"Invalidated\"}", provenance: Provenance(),
            causalRevision: 2, supersedesMessageId: "m1");
        Assert.Equal(2, envelope.CausalRevision);
        Assert.Equal("m1", envelope.SupersedesMessageId);
    }

    [Theory]
    [InlineData("not-a-sha")]
    [InlineData("0000000000000000000000000000000000000000 ")]
    public void Provenance_rejects_noncanonical_git_object_ids(string sha)
    {
        Assert.Throws<ArgumentException>(() => new ProtocolCandidateProvenance(
            "owner/repo", "refs/heads/main", sha, new string('b', 40),
            new string('c', 40), "refs/heads/main", new string('d', 40),
            1, 1, "job", "evidence.json", new string('e', 64)));
    }

    [Fact]
    public void Provenance_requires_a_sha256_evidence_digest()
    {
        Assert.Throws<ArgumentException>(() => new ProtocolCandidateProvenance(
            "owner/repo", "refs/heads/main", new string('a', 40),
            new string('b', 40), new string('c', 40), "refs/heads/main",
            new string('d', 40), 1, 1, "job", "evidence.json", "abcd"));
    }

    [Fact]
    public void Envelope_rejects_non_object_or_invalid_payload_json()
    {
        Assert.Throws<ArgumentException>(() => new ProtocolEventEnvelope(
            "m1", "c1", "producer", ProtocolEventKind.ObserverRegistrationRequested,
            DateTimeOffset.UtcNow, "[]"));
        Assert.Throws<ArgumentException>(() => new ProtocolEventEnvelope(
            "m1", "c1", "producer", ProtocolEventKind.ObserverRegistrationRequested,
            DateTimeOffset.UtcNow, "{"));
    }

    [Fact]
    public void Deserialize_rejects_duplicate_properties_and_unknown_versions()
    {
        Assert.Throws<ArgumentException>(() => ProtocolEventEnvelopeJson.Deserialize(
            "{\"schema\":\"TheSingularityWorkshop.ProtocolAi.EventEnvelope\",\"schema\":\"x\"}"));
        var future = ProtocolEventEnvelopeJson.Serialize(SourceEvent()).Replace("\"schemaVersion\":1", "\"schemaVersion\":2");
        Assert.Throws<ArgumentException>(() => ProtocolEventEnvelopeJson.Deserialize(future));
    }

    [Fact]
    public void Provenance_keeps_event_checkout_and_workflow_hashes_distinct()
    {
        var p = Provenance();
        Assert.NotEqual(p.CandidateSha, p.EventSha);
        Assert.NotEqual(p.EventSha, p.TestedCheckoutSha);
        Assert.NotEqual(p.TestedCheckoutSha, p.WorkflowDefinitionSha);
    }
}
