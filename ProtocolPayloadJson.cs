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
        [property: JsonPropertyName("symbolId")]
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        ulong? SymbolId = null,
        [property: JsonPropertyName("literal")]
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string? Literal = null);

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

        var json = JsonSerializer.Serialize(new WirePayload(payload.ProtocolId, values), Options);
        if (json.Length > ProtocolLimits.MaxJsonCharacters)
            throw new ArgumentException($"Serialized payload cannot exceed {ProtocolLimits.MaxJsonCharacters} characters.", nameof(payload));
        return json;
    }

    public static ProtocolPayload Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON payload is required.", nameof(json));
        if (json.Length > ProtocolLimits.MaxJsonCharacters)
            throw new ArgumentException($"JSON payload cannot exceed {ProtocolLimits.MaxJsonCharacters} characters.", nameof(json));

        WirePayload? wire;
        try
        {
            using var document = JsonDocument.Parse(json);
            RejectDuplicateProperties(document.RootElement);
            if (document.RootElement.ValueKind == JsonValueKind.Object &&
                document.RootElement.TryGetProperty("values", out var valuesElement) &&
                valuesElement.ValueKind == JsonValueKind.Array &&
                valuesElement.GetArrayLength() > ProtocolLimits.MaxPayloadValues)
                throw new ArgumentException($"A payload cannot contain more than {ProtocolLimits.MaxPayloadValues} values.", nameof(json));
            wire = JsonSerializer.Deserialize<WirePayload>(document.RootElement, Options);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("The protocol payload is not valid JSON.", nameof(json), ex);
        }

        if (wire is null)
            throw new ArgumentException("The protocol payload is empty.", nameof(json));

        if (wire.ProtocolId == 0)
            throw new ArgumentException("The protocol payload must contain a non-zero protocol ID.", nameof(json));

        if (wire.Values is null)
            throw new ArgumentException("The protocol payload must contain a values array.", nameof(json));

        var values = wire.Values.Select((value, index) =>
        {
            var hasReference = value.SymbolId.HasValue;
            var hasLiteral = value.Literal is not null;

            if (hasReference == hasLiteral)
                throw new ArgumentException(
                    $"Protocol payload value at index {index} must contain exactly one of 'symbolId' or 'literal'.",
                    nameof(json));

            if (hasReference)
            {
                if (value.SymbolId == 0)
                    throw new ArgumentException(
                        $"Protocol payload value at index {index} must contain a non-zero symbol ID.",
                        nameof(json));

                return ProtocolValue.FromId(value.SymbolId!.Value);
            }

            if (string.IsNullOrWhiteSpace(value.Literal))
                throw new ArgumentException(
                    $"Protocol payload value at index {index} must contain a non-empty literal.",
                    nameof(json));

            return ProtocolValue.FromLiteral(value.Literal!);
        }).ToArray();

        return new ProtocolPayload(wire.ProtocolId, values);
    }

    private static void RejectDuplicateProperties(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var property in element.EnumerateObject())
            {
                if (!names.Add(property.Name))
                    throw new ArgumentException(
                        $"The protocol payload contains a duplicate JSON property '{property.Name}'.",
                        "json");

                RejectDuplicateProperties(property.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
                RejectDuplicateProperties(item);
        }
    }
}