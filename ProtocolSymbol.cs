namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>
/// Defines one named value in an integer-backed protocol vocabulary.
/// </summary>
public readonly record struct ProtocolSymbol
{
    /// <summary>Creates one non-zero symbol with a unique application-owned ID, name, and literal value.</summary>
    public ProtocolSymbol(ulong id, string name, string value)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id), "A protocol symbol ID must be non-zero.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A protocol symbol name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A protocol symbol value is required.", nameof(value));
        if (name.Length > ProtocolLimits.MaxNameCharacters) throw new ArgumentException($"A symbol name cannot exceed {ProtocolLimits.MaxNameCharacters} characters.", nameof(name));
        if (value.Length > ProtocolLimits.MaxLiteralCharacters) throw new ArgumentException($"A symbol value cannot exceed {ProtocolLimits.MaxLiteralCharacters} characters.", nameof(value));
        Id = id;
        Name = name;
        Value = value;
    }

    /// <summary>Gets the stable, non-zero identity of the symbol within its protocol.</summary>
    public ulong Id { get; }
    /// <summary>Gets the unique name used to refer to the symbol within its protocol.</summary>
    public string Name { get; }
    /// <summary>Gets the exact literal value represented by the symbol.</summary>
    public string Value { get; }
}