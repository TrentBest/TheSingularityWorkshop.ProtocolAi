namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>
/// One item in an AI protocol payload: either an integer-backed reference or a literal string.
/// </summary>
public readonly record struct ProtocolValue
{
    private ProtocolValue(ulong? symbolId, string? literal)
    {
        if (symbolId is null == (literal is null))
            throw new ArgumentException("A protocol value must contain exactly one of a symbol ID or a literal.");
        SymbolId = symbolId;
        Literal = literal;
    }

    public ulong? SymbolId { get; }
    public string? Literal { get; }
    public bool IsReference => SymbolId.HasValue;
    public bool IsLiteral => Literal is not null;

    public static ProtocolValue FromId(ulong symbolId)
    {
        if (symbolId == 0) throw new ArgumentOutOfRangeException(nameof(symbolId));
        return new ProtocolValue(symbolId, null);
    }

    public static ProtocolValue FromLiteral(string literal)
    {
        if (string.IsNullOrWhiteSpace(literal)) throw new ArgumentException("A literal value is required.", nameof(literal));
        return new ProtocolValue(null, literal);
    }

    public override string ToString() => IsReference ? $"[{SymbolId}]" : Literal!;
}