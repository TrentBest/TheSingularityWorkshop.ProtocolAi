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
    }

    [Fact]
    public void Encode_uses_integer_for_known_value_and_literal_for_new_value()
    {
        var protocol = new ProtocolBuilder(1001, "People").Define(2001, "bobId", "Bob").Build();
        var payload = protocol.Encode(["Bob", "New Character"]);

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
    public void Description_is_deterministic()
    {
        var protocol = new ProtocolBuilder(1001, "People").Define(2001, "bobId", "Bob").Build();
        Assert.Equal("[1001] People\n  [2001] bobId = \"Bob\"", protocol.Describe());
    }
}