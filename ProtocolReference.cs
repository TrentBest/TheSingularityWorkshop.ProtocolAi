namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>
/// Identifies a symbol inside a self-defining protocol.
/// </summary>
public readonly record struct ProtocolReference
{
    /// <summary>Creates a qualified reference to one non-zero symbol within a non-zero protocol namespace.</summary>
    public ProtocolReference(ulong protocolId, ulong symbolId)
    {
        if (protocolId == 0) throw new ArgumentOutOfRangeException(nameof(protocolId));
        if (symbolId == 0) throw new ArgumentOutOfRangeException(nameof(symbolId));
        ProtocolId = protocolId;
        SymbolId = symbolId;
    }

    /// <summary>Gets the protocol namespace that owns the symbol.</summary>
    public ulong ProtocolId { get; }
    /// <summary>Gets the symbol identity within the owning protocol namespace.</summary>
    public ulong SymbolId { get; }
}