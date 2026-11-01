using System.Text;

namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>
/// Immutable, self-describing vocabulary that maps strings to integer-backed protocol symbols.
/// </summary>
public sealed class ProtocolDefinition
{
    private readonly IReadOnlyList<ProtocolSymbol> _symbols;
    private readonly Dictionary<ulong, ProtocolSymbol> _byId;
    private readonly Dictionary<string, ProtocolSymbol> _byName;
    private readonly Dictionary<string, ProtocolSymbol> _byValue;

    internal ProtocolDefinition(ulong id, string name, IEnumerable<ProtocolSymbol> symbols)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A protocol name is required.", nameof(name));
        if (name.Length > ProtocolLimits.MaxNameCharacters) throw new ArgumentException($"A protocol name cannot exceed {ProtocolLimits.MaxNameCharacters} characters.", nameof(name));

        ArgumentNullException.ThrowIfNull(symbols);
        var values = symbols.Take(ProtocolLimits.MaxSymbols + 1).ToArray();
        if (values.Length > ProtocolLimits.MaxSymbols)
            throw new ArgumentException($"A protocol cannot define more than {ProtocolLimits.MaxSymbols} symbols.", nameof(symbols));
        var totalSymbolTextCharacters = values.Sum(symbol => (long)symbol.Name.Length + symbol.Value.Length);
        if (totalSymbolTextCharacters > ProtocolLimits.MaxTotalSymbolTextCharacters)
            throw new ArgumentException($"A protocol cannot contain more than {ProtocolLimits.MaxTotalSymbolTextCharacters} symbol-name/value characters in total.", nameof(symbols));
        if (values.GroupBy(x => x.Id).Any(g => g.Count() > 1))
            throw new ArgumentException("A protocol cannot define the same symbol ID more than once.", nameof(symbols));
        if (values.GroupBy(x => x.Name, StringComparer.Ordinal).Any(g => g.Count() > 1))
            throw new ArgumentException("A protocol cannot define the same symbol name more than once.", nameof(symbols));
        if (values.GroupBy(x => x.Value, StringComparer.Ordinal).Any(g => g.Count() > 1))
            throw new ArgumentException("A protocol cannot define the same symbol value more than once.", nameof(symbols));

        Id = id;
        Name = name;
        _symbols = Array.AsReadOnly(values);
        _byId = values.ToDictionary(x => x.Id);
        _byName = values.ToDictionary(x => x.Name, StringComparer.Ordinal);
        _byValue = values.ToDictionary(x => x.Value, StringComparer.Ordinal);
    }

    public ulong Id { get; }
    public string Name { get; }
    public IReadOnlyList<ProtocolSymbol> Symbols => _symbols;

    public ProtocolReference Reference(string name) =>
        _byName.TryGetValue(name, out var symbol) ? new ProtocolReference(Id, symbol.Id) :
        throw new KeyNotFoundException($"Protocol symbol '{name}' is not defined.");

    public ProtocolReference ReferenceByValue(string value) =>
        _byValue.TryGetValue(value, out var symbol) ? new ProtocolReference(Id, symbol.Id) :
        throw new KeyNotFoundException($"Protocol value '{value}' is not defined.");

    public string Decode(ulong symbolId) =>
        _byId.TryGetValue(symbolId, out var symbol) ? symbol.Value :
        throw new KeyNotFoundException($"Protocol symbol ID '{symbolId}' is not defined.");

    /// <summary>
    /// Decodes a protocol-qualified reference and verifies that this definition owns it.
    /// </summary>
    public string Decode(ProtocolReference reference)
    {
        if (reference.ProtocolId != Id)
            throw new ArgumentException($"Protocol reference belongs to protocol '{reference.ProtocolId}', not '{Id}'.", nameof(reference));

        return Decode(reference.SymbolId);
    }

    /// <summary>
    /// Resolves one unqualified value. References are interpreted in this definition's symbol namespace;
    /// literals pass through unchanged.
    /// </summary>
    /// <remarks>
    /// A <see cref="ProtocolValue"/> reference carries a symbol ID but no protocol ID, so this overload
    /// cannot verify where that reference originated. Use <see cref="Resolve(ProtocolPayload)"/> when
    /// protocol ownership must be validated before resolving a collection of values.
    /// </remarks>
    public string Resolve(ProtocolValue value)
    {
        if (value.IsReference)
            return Decode(value.SymbolId!.Value);

        if (value.IsLiteral)
            return value.Literal!;

        throw new ArgumentException("A protocol value must contain a symbol reference or a literal.", nameof(value));
    }

    /// <summary>
    /// Resolves every value in a payload after verifying that the payload belongs to this protocol.
    /// </summary>
    public IReadOnlyList<string> Resolve(ProtocolPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        Validate(payload);
        return payload.Values.Select(Resolve).ToArray();
    }

    /// <summary>
    /// Verifies that a payload belongs to this protocol and that every reference targets a defined symbol.
    /// </summary>
    public void Validate(ProtocolPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        if (payload.ProtocolId != Id)
            throw new ArgumentException($"Protocol payload belongs to protocol '{payload.ProtocolId}', not '{Id}'.", nameof(payload));

        foreach (var value in payload.Values)
        {
            if (!value.IsReference && !value.IsLiteral)
                throw new ArgumentException("Protocol payload contains an uninitialized protocol value.", nameof(payload));

            if (value.IsReference && !_byId.ContainsKey(value.SymbolId!.Value))
                throw new ArgumentException($"Protocol payload references undefined symbol ID '{value.SymbolId.Value}'.", nameof(payload));
        }
    }

    public ProtocolValue Encode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return _byValue.TryGetValue(value, out var symbol)
            ? ProtocolValue.FromId(symbol.Id)
            : ProtocolValue.FromLiteral(value);
    }

    public ProtocolPayload Encode(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new ProtocolPayload(Id, values.Select(Encode));
    }

    /// <summary>
    /// Emits the vocabulary in a deterministic, model-readable form.
    /// </summary>
    public string Describe()
    {
        var builder = new StringBuilder();
        builder.Append('[').Append(Id).Append("] ").Append(EscapeValue(Name));

        foreach (var symbol in _symbols)
        {
            builder.Append('\n');
            builder.Append("  [").Append(symbol.Id).Append("] ")
                .Append(EscapeValue(symbol.Name)).Append(" = ")
                .Append('"').Append(EscapeValue(symbol.Value)).Append('"');
        }

        return builder.ToString();
    }

    private static string EscapeValue(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            switch (character)
            {
                case '\\': builder.Append("\\\\"); break;
                case '"': builder.Append("\\\""); break;
                case '\r': builder.Append("\\r"); break;
                case '\n': builder.Append("\\n"); break;
                case '\t': builder.Append("\\t"); break;
                case '\b': builder.Append("\\b"); break;
                case '\f': builder.Append("\\f"); break;
                default:
                    if (character < ' ')
                        builder.Append("\\u").Append(((int)character).ToString("x4", System.Globalization.CultureInfo.InvariantCulture));
                    else
                        builder.Append(character);
                    break;
            }
        }

        return builder.ToString();
    }

    public override string ToString() => Describe();
}