using System.Text.Json;

namespace TheSingularityWorkshop.ProtocolAi;

public enum ProtocolObserverLifecycle
{
    Active,
    Suspended,
    Removed
}

public enum ProtocolObserverRegistrationStatus
{
    Accepted,
    Rejected,
    Active,
    Suspended,
    Removed
}

public enum ProtocolEvaluationState
{
    True,
    False,
    Unknown,
    Incomplete
}

public enum ProtocolGateState
{
    Unknown,
    Closed,
    Satisfied,
    Invalidated,
    Expired
}

public enum ProtocolCommandOutcomeKind
{
    Accepted,
    Rejected,
    Completed
}

/// <summary>Requests a durable observer registration within an explicitly bounded scope.</summary>
public sealed record ProtocolObserverRegistrationRequest
{
    public ProtocolObserverRegistrationRequest(string observerId, string scope, string protocolVersion,
        ProtocolObserverLifecycle requestedLifecycle = ProtocolObserverLifecycle.Active)
    {
        ObserverId = Required(observerId, nameof(observerId), 256);
        Scope = Required(scope, nameof(scope), 2048);
        ProtocolVersion = Required(protocolVersion, nameof(protocolVersion), 128);
        if (!Enum.IsDefined(requestedLifecycle)) throw new ArgumentOutOfRangeException(nameof(requestedLifecycle));
        RequestedLifecycle = requestedLifecycle;
    }

    public string ObserverId { get; }
    public string Scope { get; }
    public string ProtocolVersion { get; }
    public ProtocolObserverLifecycle RequestedLifecycle { get; }

    internal static string Required(string value, string parameter, int max)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A non-empty value is required.", parameter);
        if (value.Length > max) throw new ArgumentException($"Value cannot exceed {max} characters.", parameter);
        return value;
    }
}

/// <summary>A host's explicit response to an observer registration request.</summary>
public sealed record ProtocolObserverRegistrationAcknowledgement
{
    public ProtocolObserverRegistrationAcknowledgement(string observerId, string scope, string protocolVersion,
        ProtocolObserverRegistrationStatus status, string? reason = null)
    {
        ObserverId = ProtocolObserverRegistrationRequest.Required(observerId, nameof(observerId), 256);
        Scope = ProtocolObserverRegistrationRequest.Required(scope, nameof(scope), 2048);
        ProtocolVersion = ProtocolObserverRegistrationRequest.Required(protocolVersion, nameof(protocolVersion), 128);
        if (!Enum.IsDefined(status)) throw new ArgumentOutOfRangeException(nameof(status));
        Status = status;
        Reason = reason is null ? null : ProtocolObserverRegistrationRequest.Required(reason, nameof(reason), 2048);
        if (status == ProtocolObserverRegistrationStatus.Rejected && Reason is null)
            throw new ArgumentException("A rejected registration must include a reason.", nameof(reason));
    }

    public string ObserverId { get; }
    public string Scope { get; }
    public string ProtocolVersion { get; }
    public ProtocolObserverRegistrationStatus Status { get; }
    public string? Reason { get; }
}

/// <summary>A deterministic observer result. Unknown and incomplete are not successful evaluations.</summary>
public sealed record ProtocolObserverEvaluation
{
    public ProtocolObserverEvaluation(string observerId, string predicateId, string predicateVersion,
        string inputMessageId, ProtocolEvaluationState state, string reason)
    {
        ObserverId = ProtocolObserverRegistrationRequest.Required(observerId, nameof(observerId), 256);
        PredicateId = ProtocolObserverRegistrationRequest.Required(predicateId, nameof(predicateId), 256);
        PredicateVersion = ProtocolObserverRegistrationRequest.Required(predicateVersion, nameof(predicateVersion), 128);
        InputMessageId = ProtocolObserverRegistrationRequest.Required(inputMessageId, nameof(inputMessageId), 128);
        if (!Enum.IsDefined(state)) throw new ArgumentOutOfRangeException(nameof(state));
        State = state;
        Reason = ProtocolObserverRegistrationRequest.Required(reason, nameof(reason), 4096);
    }

    public string ObserverId { get; }
    public string PredicateId { get; }
    public string PredicateVersion { get; }
    public string InputMessageId { get; }
    public ProtocolEvaluationState State { get; }
    public string Reason { get; }
}

/// <summary>A versioned gate state transition tied to a causal event and exact source message.</summary>
public sealed record ProtocolGateTransition
{
    public ProtocolGateTransition(string gateId, string gateVersion, ProtocolGateState state,
        long causalRevision, string sourceMessageId, string predicateId, string reason,
        string? supersedesMessageId = null)
    {
        GateId = ProtocolObserverRegistrationRequest.Required(gateId, nameof(gateId), 256);
        GateVersion = ProtocolObserverRegistrationRequest.Required(gateVersion, nameof(gateVersion), 128);
        if (!Enum.IsDefined(state)) throw new ArgumentOutOfRangeException(nameof(state));
        State = state;
        if (causalRevision <= 0) throw new ArgumentOutOfRangeException(nameof(causalRevision));
        CausalRevision = causalRevision;
        SourceMessageId = ProtocolObserverRegistrationRequest.Required(sourceMessageId, nameof(sourceMessageId), 128);
        PredicateId = ProtocolObserverRegistrationRequest.Required(predicateId, nameof(predicateId), 256);
        Reason = ProtocolObserverRegistrationRequest.Required(reason, nameof(reason), 4096);
        SupersedesMessageId = supersedesMessageId is null ? null :
            ProtocolObserverRegistrationRequest.Required(supersedesMessageId, nameof(supersedesMessageId), 128);
    }

    public string GateId { get; }
    public string GateVersion { get; }
    public ProtocolGateState State { get; }
    public long CausalRevision { get; }
    public string SourceMessageId { get; }
    public string PredicateId { get; }
    public string Reason { get; }
    public string? SupersedesMessageId { get; }
}

/// <summary>A CommandAi proposal outcome. This reports a host decision/result; it does not authorize or execute.</summary>
public sealed record ProtocolCommandOutcome
{
    public ProtocolCommandOutcome(ProtocolCommandOutcomeKind outcome, string proposalId, string? commandId,
        string? reason = null)
    {
        if (!Enum.IsDefined(outcome)) throw new ArgumentOutOfRangeException(nameof(outcome));
        Outcome = outcome;
        ProposalId = ProtocolObserverRegistrationRequest.Required(proposalId, nameof(proposalId), 256);
        CommandId = commandId is null ? null :
            ProtocolObserverRegistrationRequest.Required(commandId, nameof(commandId), 256);
        Reason = reason is null ? null :
            ProtocolObserverRegistrationRequest.Required(reason, nameof(reason), 4096);
        if (outcome == ProtocolCommandOutcomeKind.Accepted && CommandId is null)
            throw new ArgumentException("An accepted proposal must identify its assigned command.", nameof(commandId));
        if (outcome == ProtocolCommandOutcomeKind.Rejected && Reason is null)
            throw new ArgumentException("A rejected proposal must include a reason.", nameof(reason));
        if (outcome == ProtocolCommandOutcomeKind.Completed && CommandId is null)
            throw new ArgumentException("A completed command must identify its command.", nameof(commandId));
    }

    public ProtocolCommandOutcomeKind Outcome { get; }
    public string ProposalId { get; }
    public string? CommandId { get; }
    public string? Reason { get; }
}

/// <summary>JSON codec for the strongly typed event payload contracts.</summary>
public static class ProtocolEventPayloadJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public static string Serialize(ProtocolObserverRegistrationRequest payload) => SerializePayload(payload);
    public static string Serialize(ProtocolObserverRegistrationAcknowledgement payload) => SerializePayload(payload);
    public static string Serialize(ProtocolObserverEvaluation payload) => SerializePayload(payload);
    public static string Serialize(ProtocolSourceEvent payload) => SerializePayload(payload);
    public static string Serialize(ProtocolDownstreamNotification payload) => SerializePayload(payload);
    public static string Serialize(ProtocolGateTransition payload) => SerializePayload(payload);
    public static string Serialize(ProtocolCommandOutcome payload) => SerializePayload(payload);

    public static T Deserialize<T>(string json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Payload JSON is required.", nameof(json));
        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                throw new ArgumentException("The typed event payload must be a JSON object.", nameof(json));
            RejectDuplicateProperties(document.RootElement);
            var result = JsonSerializer.Deserialize<T>(json, Options);
            return result ?? throw new ArgumentException("The typed event payload cannot be null.", nameof(json));
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("The typed event payload is invalid.", nameof(json), ex);
        }
    }

    private static void RejectDuplicateProperties(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var property in element.EnumerateObject())
            {
                if (!names.Add(property.Name))
                    throw new ArgumentException($"Duplicate JSON property '{property.Name}' is not allowed.", "json");
                RejectDuplicateProperties(property.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
                RejectDuplicateProperties(item);
        }
    }

    internal static string SerializePayload<T>(T payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return JsonSerializer.Serialize(payload, Options);
    }
}
