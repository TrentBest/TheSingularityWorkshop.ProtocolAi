using Xunit;
using TheSingularityWorkshop.ProtocolAi;

namespace ProtocolAi.Tests;

public sealed class ProtocolDefinitionTests
{
    [Fact]
    public void Builder_creates_self_defining_vocabulary()
    {
        var protocol = new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob")
            .Define(2002, "ryanId", "Ryan")
            .Define(2003, "saraId", "Sara")
            .Build();

        Assert.Equal((ulong)1001, protocol.Id);
        Assert.Equal("Bob", protocol.Decode(2001));
        Assert.Equal((ulong)2002, protocol.Reference("ryanId").SymbolId);
        Assert.Equal((ulong)2003, protocol.ReferenceByValue("Sara").SymbolId);
        Assert.Equal("Bob", protocol.Decode(protocol.Reference("bobId")));
    }

    [Fact]
    public void Encode_uses_integer_for_known_value_and_literal_for_new_value()
    {
        var protocol = new ProtocolBuilder(1001, "People").Define(2001, "bobId", "Bob").Build();
        var payload = protocol.Encode(["Bob", "New Character"]);

        Assert.Equal((ulong)1001, payload.ProtocolId);
        Assert.True(payload.Values[0].IsReference);
        Assert.Equal((ulong)2001, payload.Values[0].SymbolId);
        Assert.True(payload.Values[1].IsLiteral);
        Assert.Equal("New Character", payload.Values[1].Literal);
    }

    [Fact]
    public void Duplicate_ids_names_and_values_are_rejected()
    {
        Assert.Throws<ArgumentException>(() => new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob").Define(2001, "otherId", "Other").Build());

        Assert.Throws<ArgumentException>(() => new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob").Define(2002, "bobId", "Other").Build());

        Assert.Throws<ArgumentException>(() => new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob").Define(2002, "otherId", "Bob").Build());
    }

    [Fact]
    public void Decode_rejects_a_reference_owned_by_another_protocol()
    {
        var protocol = new ProtocolBuilder(1001, "People").Define(2001, "bobId", "Bob").Build();
        var other = new ProtocolReference(9001, 2001);

        Assert.Throws<ArgumentException>(() => protocol.Decode(other));
    }

    [Fact]
    public void Definition_is_an_immutable_snapshot_of_the_builder()
    {
        var builder = new ProtocolBuilder(1001, "People").Define(2001, "bobId", "Bob");
        var first = builder.Build();

        builder.Define(2002, "janeId", "Jane");

        Assert.Single(first.Symbols);
        Assert.Throws<KeyNotFoundException>(() => first.Decode(2002));
    }

    [Fact]
    public void Description_escapes_embedded_quotes()
    {
        var protocol = new ProtocolBuilder(1001, "People").Define(2001, "quote", "Bob \"The Builder\"").Build();

        Assert.Contains("\\\"The Builder\\\"", protocol.Describe());
    }
    [Fact]
    public void Description_is_deterministic()
    {
        var protocol = new ProtocolBuilder(1001, "People").Define(2001, "bobId", "Bob").Build();
        Assert.Equal("[1001] People\n  [2001] bobId = \"Bob\"", protocol.Describe());
    }
    [Fact]
    public void Encode_uses_ordinal_case_sensitive_matching()
    {
        var protocol = new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob")
            .Build();

        Assert.True(protocol.Encode("Bob").IsReference);
        Assert.True(protocol.Encode("bob").IsLiteral);
        Assert.Equal("bob", protocol.Encode("bob").Literal);
    }

    [Fact]
    public void Resolve_rejects_default_uninitialized_protocol_value()
    {
        var protocol = new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob")
            .Build();

        Assert.Throws<ArgumentException>(() => protocol.Resolve(default(ProtocolValue)));
        Assert.Throws<ArgumentException>(() =>
            protocol.Validate(new ProtocolPayload(1001, [default(ProtocolValue)])));
    }

    [Fact]
    public void Description_escapes_control_characters_and_backslashes()
    {
        var protocol = new ProtocolBuilder(1001, "People")
            .Define(2001, "escaped", "first\\second\nthird\t\"quoted\"")
            .Build();

        Assert.Contains("first\\\\second\\nthird\\t\\\"quoted\\\"", protocol.Describe());
    }

    [Fact]
    public void Encode_rejects_null_instead_of_failing_inside_dictionary_lookup()
    {
        var protocol = new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob")
            .Build();

        Assert.Throws<ArgumentNullException>(() => protocol.Encode((string)null!));
    }


    [Fact]
    public void Encode_rejects_empty_literals_and_preserves_unicode_without_normalization()
    {
        var protocol = new ProtocolBuilder(1001, "People")
            .Define(2001, "bobId", "Bob")
            .Build();

        Assert.Throws<ArgumentException>(() => protocol.Encode(string.Empty));

        var unicode = protocol.Encode("東京 🛰️ e\u0301");
        Assert.True(unicode.IsLiteral);
        Assert.Equal("東京 🛰️ e\u0301", unicode.Literal);
    }

    [Fact]
    public void Protocol_definition_and_payload_inputs_are_bounded()
    {
        Assert.Throws<ArgumentException>(() => new ProtocolBuilder(1, "p")
            .Define(1, "symbol", new string('x', ProtocolLimits.MaxLiteralCharacters + 1)));
        Assert.Throws<ArgumentException>(() => new ProtocolBuilder(1, new string('p', ProtocolLimits.MaxNameCharacters + 1)));
        Assert.Throws<ArgumentException>(() => ProtocolValue.FromLiteral(new string('x', ProtocolLimits.MaxLiteralCharacters + 1)));
        Assert.Throws<ArgumentException>(() => new ProtocolPayload(1,
            Enumerable.Repeat(ProtocolValue.FromId(1), ProtocolLimits.MaxPayloadValues + 1)));
    }

    [Fact]
    public void Protocol_builder_stops_accepting_symbols_at_the_limit()
    {
        var builder = new ProtocolBuilder(1, "bounded");
        for (var i = 1; i <= ProtocolLimits.MaxSymbols; i++)
            builder.Define((ulong)i, $"s{i}", $"v{i}");
        Assert.Throws<InvalidOperationException>(() => builder.Define(ProtocolLimits.MaxSymbols + 1UL, "overflow", "overflow"));
    }

    [Fact]
    public void Protocol_builder_bounds_aggregate_symbol_text()
    {
        var builder = new ProtocolBuilder(1, "bounded");
        for (var i = 1; i <= 63; i++)
        {
            var value = new string('x', ProtocolLimits.MaxLiteralCharacters - 8) + i.ToString("D8");
            builder.Define((ulong)i, $"s{i}", value);
        }

        Assert.Throws<InvalidOperationException>(() =>
            builder.Define(9_999, "overflow", new string('y', ProtocolLimits.MaxLiteralCharacters)));
    }
    [Fact]
    public void Describe_escapes_all_json_control_characters_deterministically()
    {
        var protocol = new ProtocolBuilder(77, "controls")
            .Define(1, "control", "quote:\" slash:\\ cr:\r lf:\n tab:\t backspace:\b formfeed:\f nul:\0")
            .Build();

        var description = protocol.Describe();

        Assert.Contains("quote:\\\"", description);
        Assert.Contains("slash:\\\\", description);
        Assert.Contains("cr:\\r", description);
        Assert.Contains("lf:\\n", description);
        Assert.Contains("tab:\\t", description);
        Assert.Contains("backspace:\\b", description);
        Assert.Contains("formfeed:\\f", description);
        Assert.Contains("nul:\\u0000", description);
    }
    [Fact]
    public void Describe_escapes_protocol_and_symbol_names()
    {
        var protocol = new ProtocolBuilder(78, "protocol\\nname")
            .Define(1, "symbol\\tname", "value")
            .Build();

        var description = protocol.Describe();

        Assert.Contains("[78] protocol\\\\nname", description);
        Assert.Contains("[1] symbol\\\\tname", description);
    }
}