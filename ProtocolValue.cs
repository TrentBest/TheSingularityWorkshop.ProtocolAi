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

    /// <summary>Gets the symbol identity when this value is a reference; otherwise null.</summary>
    public ulong? SymbolId { get; }
    /// <summary>Gets the exact literal when this value is not a symbol reference; otherwise null.</summary>
    public string? Literal { get; }
    /// <summary>Gets whether this value contains a symbol reference.</summary>
    public bool IsReference => SymbolId.HasValue;
    /// <summary>Gets whether this value contains a literal string.</summary>
    public bool IsLiteral => Literal is not null;

    /// <summary>Creates a symbol-reference value from a non-zero symbol identity.</summary>
    public static ProtocolValue FromId(ulong symbolId)
    {
        if (symbolId == 0) throw new ArgumentOutOfRangeException(nameof(symbolId));
        return new ProtocolValue(symbolId, null);
    }

    /// <summary>Creates a literal value without inventing a symbol identity.</summary>
    public static ProtocolValue FromLiteral(string literal)
    {
        if (string.IsNullOrWhiteSpace(literal)) throw new ArgumentException("A literal value is required.", nameof(literal));
        if (literal.Length > ProtocolLimits.MaxLiteralCharacters) throw new ArgumentException($"A literal cannot exceed {ProtocolLimits.MaxLiteralCharacters} characters.", nameof(literal));
        return new ProtocolValue(null, literal);
    }

    /// <summary>Returns the symbol identity or literal value in a compact diagnostic representation.</summary>
    public override string ToString() => IsReference ? $"[{SymbolId}]" : Literal!;
}