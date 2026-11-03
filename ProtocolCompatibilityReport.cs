namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>Identifies a structural incompatibility between two protocol definitions.</summary>
public enum ProtocolCompatibilityIssueCode
{
    /// <summary>The candidate uses a different protocol namespace identity.</summary>
    ProtocolIdChanged,
    /// <summary>A symbol ID present in the baseline is absent from the candidate.</summary>
    SymbolRemoved,
    /// <summary>An existing symbol ID has a different name or literal value.</summary>
    SymbolDefinitionChanged
}

/// <summary>One deterministic structural compatibility finding.</summary>
/// <param name="Code">Machine-readable issue category.</param>
/// <param name="SymbolId">Affected symbol ID, or null when the protocol identity differs.</param>
/// <param name="Message">Human-readable explanation.</param>
public sealed record ProtocolCompatibilityIssue(
    ProtocolCompatibilityIssueCode Code,
    ulong? SymbolId,
    string Message);

/// <summary>Immutable result of comparing a published protocol definition with a candidate.</summary>
public sealed class ProtocolCompatibilityReport
{
    internal ProtocolCompatibilityReport(IEnumerable<ProtocolCompatibilityIssue> issues)
    {
        Issues = Array.AsReadOnly(issues.ToArray());
    }

    /// <summary>Gets the ordered compatibility findings.</summary>
    public IReadOnlyList<ProtocolCompatibilityIssue> Issues { get; }

    /// <summary>Gets whether the candidate preserves all checked structural contracts.</summary>
    public bool IsCompatible => Issues.Count == 0;
}
