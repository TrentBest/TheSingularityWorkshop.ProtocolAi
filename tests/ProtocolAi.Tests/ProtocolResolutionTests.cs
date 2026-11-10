using Xunit;
using TheSingularityWorkshop.ProtocolAi;

namespace ProtocolAi.Tests;

public sealed class ProtocolResolutionTests
{
    private static ProtocolDefinition People() =>
        new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob")
            .Define(2002, "janeId", "Jane")
            .Build();

    [Fact]
    public void Validate_accepts_own_mixed_payload()
    {
        var protocol = People();
        var payload = protocol.Encode(["Bob", "New Character"]);

        protocol.Validate(payload);
    }

    [Fact]
    public void Validate_rejects_payload_owned_by_another_protocol()
    {
        var protocol = People();
        var payload = new ProtocolPayload(9001, [ProtocolValue.FromId(2001)]);

        var error = Assert.Throws<ArgumentException>(() => protocol.Validate(payload));

        Assert.Contains("belongs to protocol", error.Message);
    }

    [Fact]
    public void Validate_rejects_reference_to_undefined_symbol()
    {
        var protocol = People();
        var payload = new ProtocolPayload(1001, [ProtocolValue.FromId(9999)]);

        var error = Assert.Throws<ArgumentException>(() => protocol.Validate(payload));

        Assert.Contains("undefined symbol ID", error.Message);
    }

    [Fact]
    public void Resolve_decodes_references_and_preserves_literals_in_order()
    {
        var protocol = People();
        var payload = protocol.Encode(["Bob", "New Character", "Jane"]);

        var resolved = protocol.Resolve(payload);

        Assert.Equal(["Bob", "New Character", "Jane"], resolved);
    }

    [Fact]
    public void Newer_additive_definition_can_resolve_payload_written_by_older_definition()
    {
        var older = People();
        var newer = new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob")
            .Define(2002, "janeId", "Jane")
            .Define(2003, "saraId", "Sara")
            .Build();
        var persistedJson = ProtocolPayloadJson.Serialize(older.Encode(["Bob", "Jane"]));
        var restored = ProtocolPayloadJson.Deserialize(persistedJson);

        Assert.True(older.CompareCompatibility(newer).IsCompatible);
        Assert.Equal(["Bob", "Jane"], newer.Resolve(restored));
    }

    [Fact]
    public void Older_definition_rejects_payload_using_symbol_added_by_newer_definition()
    {
        var older = People();
        var newer = new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob")
            .Define(2002, "janeId", "Jane")
            .Define(2003, "saraId", "Sara")
            .Build();
        var persistedJson = ProtocolPayloadJson.Serialize(newer.Encode(["Sara"]));
        var restored = ProtocolPayloadJson.Deserialize(persistedJson);

        var error = Assert.Throws<ArgumentException>(() => older.Resolve(restored));

        Assert.Contains("undefined symbol ID '2003'", error.Message);
    }
}
