namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>
/// Defines one named value in an integer-backed protocol vocabulary.
/// </summary>
public readonly record struct ProtocolSymbol
{
    public ProtocolSymbol(ulong id, string name, string value)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id), "A protocol symbol ID must be non-zero.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A protocol symbol name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A protocol symbol value is required.", nameof(value));
        Id = id;
        Name = name;
        Value = value;
    }

    public ulong Id { get; }
    public string Name { get; }
    public string Value { get; }
}