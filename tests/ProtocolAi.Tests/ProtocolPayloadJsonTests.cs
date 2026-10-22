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

    [Theory]
    [InlineData("""{"protocolId":1001,"protocolId":2002,"values":[]}""")]
    [InlineData("""{"protocolId":1001,"values":[{"symbolId":2001,"symbolId":2002}]}""")]
    [InlineData("""{"protocolId":1001,"values":[{"literal":"first","literal":"second"}]}""")]
    public void Deserialize_rejects_duplicate_json_properties(string json)
    {
        var error = Assert.Throws<ArgumentException>(() => ProtocolPayloadJson.Deserialize(json));

        Assert.Contains("duplicate JSON property", error.Message);
    }

    [Fact]
    public void Deserialize_rejects_malformed_json_with_a_clear_argument_error()
    {
        var error = Assert.Throws<ArgumentException>(() =>
            ProtocolPayloadJson.Deserialize("""{"protocolId":1001,"values":["""));

        Assert.Contains("not valid JSON", error.Message);
        Assert.IsAssignableFrom<System.Text.Json.JsonException>(error.InnerException);
    }

    [Theory]
    [InlineData("""{"protocolId":18446744073709551616,"values":[]}""")]
    [InlineData("""{"protocolId":1001,"values":[{"symbolId":18446744073709551616}]}""")]
    public void Deserialize_rejects_unsigned_integer_overflow(string json)
    {
        var error = Assert.Throws<ArgumentException>(() => ProtocolPayloadJson.Deserialize(json));

        Assert.Contains("not valid JSON", error.Message);
        Assert.IsType<System.Text.Json.JsonException>(error.InnerException);
    }

    [Fact]
    public void Deserialize_ignores_unknown_properties_for_forward_compatibility()
    {
        var payload = ProtocolPayloadJson.Deserialize(
            """{"protocolId":1001,"futureField":true,"values":[{"literal":"Bob","annotation":"future"}]}""");

        Assert.Equal((ulong)1001, payload.ProtocolId);
        Assert.Equal(ProtocolValue.FromLiteral("Bob"), Assert.Single(payload.Values));
    }

    [Theory]
    [InlineData("""{"protocolId":1001,"values":[{"symbolId":0}]}""")]
    [InlineData("""{"protocolId":1001,"values":[{"literal":""}]}""")]
    [InlineData("""{"protocolId":1001,"values":[{"literal":"   "}]}""")]
    public void Deserialize_rejects_invalid_domain_values_as_json_argument_errors(string json)
    {
        var error = Assert.Throws<ArgumentException>(() => ProtocolPayloadJson.Deserialize(json));

        Assert.Equal("json", error.ParamName);
    }


    [Theory]
    [InlineData("[]")]
    [InlineData("null")]
    [InlineData("{")]
    [InlineData("{\"protocolId\":1001}")]
    [InlineData("{\"protocolId\":1001,\"values\":null}")]
    [InlineData("{\"protocolId\":0,\"values\":[]}")]
    [InlineData("{\"protocolId\":1001,\"values\":[{\"symbolId\":0}]}")]
    [InlineData("{\"protocolId\":1001,\"values\":[{\"literal\":\" \"}]}")]
    public void Deserialize_rejects_invalid_root_shapes_and_domain_values(string json)
    {
        Assert.Throws<ArgumentException>(() => ProtocolPayloadJson.Deserialize(json));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Deserialize_rejects_empty_input(string json)
    {
        Assert.Throws<ArgumentException>(() => ProtocolPayloadJson.Deserialize(json));
    }

}
