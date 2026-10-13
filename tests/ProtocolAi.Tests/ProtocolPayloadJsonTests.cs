using TheSingularityWorkshop.ProtocolAi;
using Xunit;

namespace ProtocolAi.Tests;

public sealed class ProtocolPayloadJsonTests
{
    private static ProtocolDefinition People() =>
        new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob")
            .Define(2002, "janeId", "Jane")
            .Build();

    [Fact]
    public void Serialize_preserves_protocol_identity_and_mixed_values()
    {
        var protocol = People();
        var payload = protocol.Encode(["Bob", "New Character", "Jane"]);

        var json = ProtocolPayloadJson.Serialize(payload);

        Assert.Equal(
            """{"protocolId":1001,"values":[{"symbolId":2001},{"literal":"New Character"},{"symbolId":2002}]}""",
            json);
    }

    [Fact]
    public void Deserialize_round_trips_mixed_payload()
    {
        var protocol = People();
        var original = protocol.Encode(["Bob", "New Character", "Jane"]);

        var restored = ProtocolPayloadJson.Deserialize(ProtocolPayloadJson.Serialize(original));

        Assert.Equal(original.ProtocolId, restored.ProtocolId);
        Assert.Equal(original.Values, restored.Values);
        Assert.Equal(["Bob", "New Character", "Jane"], protocol.Resolve(restored));
    }

    [Fact]
    public void Deserialize_rejects_value_with_both_reference_and_literal()
    {
        var error = Assert.Throws<ArgumentException>(() =>
            ProtocolPayloadJson.Deserialize(
                """{"protocolId":1001,"values":[{"symbolId":2001,"literal":"Bob"}]}"""));

        Assert.Contains("exactly one", error.Message);
    }

    [Fact]
    public void Deserialize_rejects_value_with_neither_reference_nor_literal()
    {
        var error = Assert.Throws<ArgumentException>(() =>
            ProtocolPayloadJson.Deserialize(
                """{"protocolId":1001,"values":[{}]}"""));

        Assert.Contains("exactly one", error.Message);
    }

    [Fact]
    public void Deserialize_does_not_resolve_unknown_symbols_by_itself()
    {
        var payload = ProtocolPayloadJson.Deserialize(
            """{"protocolId":1001,"values":[{"symbolId":9999}]}""");

        Assert.Equal(9999UL, payload.Values[0].SymbolId);
    }
}
