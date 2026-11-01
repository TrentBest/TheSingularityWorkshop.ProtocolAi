namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>
/// Ordered protocol data suitable for passing between an application and an AI model.
/// </summary>
public sealed class ProtocolPayload
{
    private readonly IReadOnlyList<ProtocolValue> _values;

    public ProtocolPayload(ulong protocolId, IEnumerable<ProtocolValue> values)
    {
        if (protocolId == 0) throw new ArgumentOutOfRangeException(nameof(protocolId));
        ArgumentNullException.ThrowIfNull(values);

        var materialized = values.Take(ProtocolLimits.MaxPayloadValues + 1).ToArray();
        if (materialized.Length > ProtocolLimits.MaxPayloadValues)
            throw new ArgumentException($"A payload cannot contain more than {ProtocolLimits.MaxPayloadValues} values.", nameof(values));
        if (materialized.Any(value => !value.IsReference && !value.IsLiteral))
            throw new ArgumentException(
                "Each protocol payload value must contain exactly one of a symbol ID or a literal.",
                nameof(values));

        ProtocolId = protocolId;
        _values = Array.AsReadOnly(materialized);
    }

    /// <summary>
    /// Identifies the vocabulary that owns the symbol IDs in this payload.
    /// </summary>
    public ulong ProtocolId { get; }

    public IReadOnlyList<ProtocolValue> Values => _values;
    public override string ToString() => string.Join(" ", _values);
}
