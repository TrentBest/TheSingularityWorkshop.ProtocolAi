namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>
/// Ordered protocol data suitable for passing between an application and an AI model.
/// </summary>
public sealed class ProtocolPayload
{
    private readonly IReadOnlyList<ProtocolValue> _values;

    public ProtocolPayload(IEnumerable<ProtocolValue> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        _values = Array.AsReadOnly(values.ToArray());
    }

    public IReadOnlyList<ProtocolValue> Values => _values;
    public override string ToString() => string.Join(" ", _values);
}