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
        Assert.Equal("github-actions", payload.SourceId);
        Assert.Equal("build-and-test.completed", payload.EventType);
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

    [Fact]
    public void Portable_fixture_manifest_pins_revision_and_unique_scenario_ids()
    {
        using var manifest = System.Text.Json.JsonDocument.Parse(
            File.ReadAllText(Fixture("manifest.json")));

        var root = manifest.RootElement;
        Assert.Equal("TheSingularityWorkshop.ProtocolAi.EventEnvelope",
            root.GetProperty("fixtureSet").GetString());
        Assert.Equal(1, root.GetProperty("schemaVersion").GetInt32());
        Assert.Equal("1.0.0", root.GetProperty("revision").GetString());

        var scenarios = root.GetProperty("scenarios").EnumerateArray().ToArray();
        Assert.Equal(4, scenarios.Length);
        Assert.Equal(4, scenarios.Select(x => x.GetProperty("id").GetString()).Distinct().Count());
        foreach (var scenario in scenarios)
            Assert.True(File.Exists(Fixture(scenario.GetProperty("file").GetString()!)));
    }
}
