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
        ProtocolId = protocolId;
        _values = Array.AsReadOnly(values.ToArray());
    }

    /// <summary>
    /// Identifies the vocabulary that owns the symbol IDs in this payload.
    /// </summary>
    public ulong ProtocolId { get; }

    public IReadOnlyList<ProtocolValue> Values => _values;
    public override string ToString() => string.Join(" ", _values);
}