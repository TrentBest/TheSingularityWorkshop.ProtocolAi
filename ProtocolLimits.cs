namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>Resource limits for protocol definitions and untrusted payload data.</summary>
public static class ProtocolLimits
{
    /// <summary>Maximum number of symbols in one protocol definition.</summary>
    public const int MaxSymbols = 4_096;
    /// <summary>Maximum number of values in one payload.</summary>
    public const int MaxPayloadValues = 65_536;
    /// <summary>Maximum protocol and symbol name length, in UTF-16 characters.</summary>
    public const int MaxNameCharacters = 256;
    /// <summary>Maximum vocabulary value or payload literal length, in UTF-16 characters.</summary>
    public const int MaxLiteralCharacters = 16_384;
}
