using TheSingularityWorkshop.ProtocolAi;
using Xunit;

namespace ProtocolAi.Tests;

public sealed class PortableEventEnvelopeFixtureTests
{
    private static string Fixture(string name) =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "EventEnvelopeV1", name);

    [Fact]
    public void Portable_source_event_fixture_is_accepted_and_typed()
    {
        var envelope = ProtocolEventEnvelopeJson.Deserialize(
            File.ReadAllText(Fixture("valid-source-event.json")));

        Assert.Equal(ProtocolEventKind.SourceEvent, envelope.Kind);
        var payload = ProtocolTypedEventEnvelope.DeserializePayload<ProtocolSourceEvent>(envelope);
        Assert.Equal(ProtocolSourceEventOutcome.Succeeded, payload.Outcome);
    }

    [Fact]
    public void Portable_command_completed_fixture_preserves_idempotency()
    {
        var envelope = ProtocolEventEnvelopeJson.Deserialize(
            File.ReadAllText(Fixture("valid-command-completed.json")));

        Assert.Equal(ProtocolEventKind.CommandCompleted, envelope.Kind);
        Assert.Equal("fixture-idempotency-001", envelope.IdempotencyKey);
        var payload = ProtocolTypedEventEnvelope.DeserializePayload<ProtocolCommandOutcome>(envelope);
        Assert.Equal(ProtocolCommandOutcomeKind.Completed, payload.Outcome);
        Assert.Equal("command-001", payload.CommandId);
    }

    [Fact]
    public void Portable_unsupported_schema_version_fixture_is_rejected()
    {
        Assert.Throws<ArgumentException>(() => ProtocolEventEnvelopeJson.Deserialize(
            File.ReadAllText(Fixture("unsupported-schema-version.json"))));
    }

    [Fact]
    public void Portable_kind_payload_mismatch_fixture_is_rejected_by_typed_reader()
    {
        var envelope = ProtocolEventEnvelopeJson.Deserialize(
            File.ReadAllText(Fixture("kind-payload-mismatch.json")));

        Assert.Equal(ProtocolEventKind.ObserverEvaluation, envelope.Kind);
        Assert.Throws<ArgumentException>(() =>
            ProtocolTypedEventEnvelope.DeserializePayload<ProtocolObserverEvaluation>(envelope));
    }
}
