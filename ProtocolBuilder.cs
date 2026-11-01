namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>
/// Builder used by tools to define a protocol vocabulary before publishing its immutable definition.
/// </summary>
public sealed class ProtocolBuilder
{
    private readonly List<ProtocolSymbol> _symbols = [];

    public ProtocolBuilder(ulong id, string name)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A protocol name is required.", nameof(name));
        if (name.Length > ProtocolLimits.MaxNameCharacters) throw new ArgumentException($"A protocol name cannot exceed {ProtocolLimits.MaxNameCharacters} characters.", nameof(name));
        Id = id;
        Name = name;
    }

    public ulong Id { get; }
    public string Name { get; }

    public ProtocolBuilder Define(ulong symbolId, string name, string value)
    {
        if (_symbols.Count >= ProtocolLimits.MaxSymbols) throw new InvalidOperationException($"A protocol cannot define more than {ProtocolLimits.MaxSymbols} symbols.");
        _symbols.Add(new ProtocolSymbol(symbolId, name, value));
        return this;
    }

    public ProtocolDefinition Build() => new(Id, Name, _symbols);
}