namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>
/// Ordered protocol data suitable for passing between an application and an AI model.
/// </summary>
public sealed class ProtocolPayload
{
    private readonly IReadOnlyList<ProtocolValue> _values;

    /// <summary>Creates an immutable, ordered payload scoped to one non-zero protocol identity.</summary>
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

        var totalLiteralCharacters = materialized.Where(value => value.IsLiteral).Sum(value => (long)value.Literal!.Length);
        if (totalLiteralCharacters > ProtocolLimits.MaxTotalPayloadLiteralCharacters)
            throw new ArgumentException($"A payload cannot contain more than {ProtocolLimits.MaxTotalPayloadLiteralCharacters} literal characters in total.", nameof(values));

        ProtocolId = protocolId;
        _values = Array.AsReadOnly(materialized);
    }

    /// <summary>
    /// Identifies the vocabulary that owns the symbol IDs in this payload.
    /// </summary>
    /// <summary>Gets the protocol namespace identity that owns the payload's symbol references.</summary>
    public ulong ProtocolId { get; }

    /// <summary>Gets the immutable ordered references and literals in the payload.</summary>
    public IReadOnlyList<ProtocolValue> Values => _values;
    /// <summary>Returns a compact textual representation of the payload values.</summary>
    public override string ToString() => string.Join(" ", _values);
}
