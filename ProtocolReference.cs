namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>
/// Identifies a symbol inside a self-defining protocol.
/// </summary>
public readonly record struct ProtocolReference
{
    public ProtocolReference(ulong protocolId, ulong symbolId)
    {
        if (protocolId == 0) throw new ArgumentOutOfRangeException(nameof(protocolId));
        if (symbolId == 0) throw new ArgumentOutOfRangeException(nameof(symbolId));
        ProtocolId = protocolId;
        SymbolId = symbolId;
    }

    public ulong ProtocolId { get; }
    public ulong SymbolId { get; }
}