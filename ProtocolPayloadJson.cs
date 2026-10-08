using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>
/// Provides a dependency-free JSON wire representation for protocol payloads.
/// </summary>
/// <remarks>
/// JSON is a transport representation only. Protocol identity and symbol ownership
/// remain defined by <see cref="ProtocolDefinition"/>; this type does not contact
/// an AI provider or interpret application meaning.
/// </remarks>
public static class ProtocolPayloadJson
{
    private sealed record WirePayload(
        [property: JsonPropertyName("protocolId")] ulong ProtocolId,
        [property: JsonPropertyName("values")] IReadOnlyList<WireValue> Values);

    private sealed record WireValue(
        [property: JsonPropertyName("symbolId")] ulong? SymbolId = null,
        [property: JsonPropertyName("literal")] string? Literal = null);

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = false
    };

    public static string Serialize(ProtocolPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        var values = payload.Values.Select(value =>
            value.IsReference
                ? new WireValue(SymbolId: value.SymbolId)
                : new WireValue(Literal: value.Literal))
            .ToArray();

        return JsonSerializer.Serialize(new WirePayload(payload.ProtocolId, values), Options);
    }

    public static ProtocolPayload Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON payload is required.", nameof(json));

        WirePayload? wire;
        try
        {
            wire = JsonSerializer.Deserialize<WirePayload>(json, Options);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("The protocol payload is not valid JSON.", nameof(json), ex);
        }

        if (wire is null)
            throw new ArgumentException("The protocol payload is empty.", nameof(json));

        if (wire.ProtocolId == 0)
            throw new ArgumentException("The protocol payload must contain a non-zero protocol ID.", nameof(json));

        var values = wire.Values.Select((value, index) =>
        {
            var hasReference = value.SymbolId.HasValue;
            var hasLiteral = value.Literal is not null;

            if (hasReference == hasLiteral)
                throw new ArgumentException(
                    $"Protocol payload value at index {index} must contain exactly one of 'symbolId' or 'literal'.",
                    nameof(json));

            return hasReference
                ? ProtocolValue.FromId(value.SymbolId!.Value)
                : ProtocolValue.FromLiteral(value.Literal!);
        }).ToArray();

        return new ProtocolPayload(wire.ProtocolId, values);
    }
}
