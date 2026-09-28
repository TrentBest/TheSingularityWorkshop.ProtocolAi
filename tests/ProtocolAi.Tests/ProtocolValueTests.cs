using Xunit;
using TheSingularityWorkshop.ProtocolAi;

namespace ProtocolAi.Tests;

public sealed class ProtocolValueTests
{
    [Fact]
    public void Reference_and_literal_are_distinct_forms()
    {
        var reference = ProtocolValue.FromId(42);
        var literal = ProtocolValue.FromLiteral("create this");

        Assert.True(reference.IsReference);
        Assert.False(reference.IsLiteral);
        Assert.Equal((ulong)42, reference.SymbolId);

        Assert.True(literal.IsLiteral);
        Assert.False(literal.IsReference);
        Assert.Equal("create this", literal.Literal);
    }
}