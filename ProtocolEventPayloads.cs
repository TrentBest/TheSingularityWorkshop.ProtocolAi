using System.Text.Json;

namespace TheSingularityWorkshop.ProtocolAi;

    /// <summary>Defines the lifecycle requested for a registered observer.</summary>
public enum ProtocolObserverLifecycle
{
    /// <summary>The observer is active.</summary>
    Active,
    /// <summary>The observer is suspended and should not evaluate new events.</summary>
    Suspended,
    /// <summary>The observer registration has been removed.</summary>
    Removed
}

    /// <summary>Defines the host's response state for an observer registration.</summary>
public enum ProtocolObserverRegistrationStatus
{
    /// <summary>The host accepted the registration or command proposal.</summary>
    Accepted,
    /// <summary>The host rejected the registration or command proposal.</summary>
    Rejected,
    /// <summary>The observer is active.</summary>
    Active,
    /// <summary>The observer is suspended and should not evaluate new events.</summary>
    Suspended,
    /// <summary>The observer registration has been removed.</summary>
    Removed
}

    /// <summary>Defines the four explicit outcomes of an observer evaluation.</summary>
public enum ProtocolEvaluationState
{
    /// <summary>The predicate evaluated to true.</summary>
    True,
    /// <summary>The predicate evaluated to false.</summary>
    False,
    /// <summary>The available information is insufficient to determine the result.</summary>
    Unknown,
    /// <summary>The evaluation could not be completed because required information is missing.</summary>
    Incomplete
}

    /// <summary>Defines the state reported for a versioned gate.</summary>
public enum ProtocolGateState
{
    /// <summary>The available information is insufficient to determine the result.</summary>
    Unknown,
    /// <summary>The gate is not satisfied.</summary>
    Closed,
    /// <summary>The gate's declared prerequisites are satisfied.</summary>
    Satisfied,
    /// <summary>A previously satisfied gate result is no longer valid.</summary>
    Invalidated,
    /// <summary>The gate result is outside its validity period.</summary>
    Expired
}

    /// <summary>Defines the outcome stage reported for a command proposal.</summary>
public enum ProtocolCommandOutcomeKind
{
    /// <summary>The host accepted the registration or command proposal.</summary>
    Accepted,
    /// <summary>The host rejected the registration or command proposal.</summary>
    Rejected,
    /// <summary>The command has completed according to the reporting host.</summary>
    Completed
}

/// <summary>Requests a durable observer registration within an explicitly bounded scope.</summary>
public sealed record ProtocolObserverRegistrationRequest
{
    /// <summary>Creates a bounded request to register or change an observer within a declared scope.</summary>
    public ProtocolObserverRegistrationRequest(string observerId, string scope, string protocolVersion,
        ProtocolObserverLifecycle requestedLifecycle = ProtocolObserverLifecycle.Active)
    {
        ObserverId = Required(observerId, nameof(observerId), 256);
        Scope = Required(scope, nameof(scope), 2048);
        ProtocolVersion = Required(protocolVersion, nameof(protocolVersion), 128);
        if (!Enum.IsDefined(requestedLifecycle)) throw new ArgumentOutOfRangeException(nameof(requestedLifecycle));
        RequestedLifecycle = requestedLifecycle;
    }

    /// <summary>Gets the stable identity of the observer.</summary>
    public string ObserverId { get; }
    /// <summary>Gets the scope in which the observer is requested to operate.</summary>
    public string Scope { get; }
    /// <summary>Gets the version of the protocol definition used by the observer.</summary>
    public string ProtocolVersion { get; }
    /// <summary>Gets the lifecycle state requested by the registration message.</summary>
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
    /// <summary>Creates the host's explicit acknowledgement or rejection of an observer registration.</summary>
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

    /// <summary>Gets the stable identity of the observer.</summary>
    public string ObserverId { get; }
    /// <summary>Gets the scope in which the observer is requested to operate.</summary>
    public string Scope { get; }
    /// <summary>Gets the version of the protocol definition used by the observer.</summary>
    public string ProtocolVersion { get; }
    /// <summary>Gets the registration state reported by the host.</summary>
    public ProtocolObserverRegistrationStatus Status { get; }
    /// <summary>Gets the optional explanation, required when registration is rejected.</summary>
    public string? Reason { get; }
}

/// <summary>A deterministic observer result. Unknown and incomplete are not successful evaluations.</summary>
public sealed record ProtocolObserverEvaluation
{
    /// <summary>Creates a result for one observer predicate and its exact input message.</summary>
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

    /// <summary>Gets the stable identity of the observer.</summary>
    public string ObserverId { get; }
    /// <summary>Gets the identity of the predicate that was evaluated.</summary>
    public string PredicateId { get; }
    /// <summary>Gets the version of the predicate contract used for evaluation.</summary>
    public string PredicateVersion { get; }
    /// <summary>Gets the identity of the source message evaluated by the observer.</summary>
    public string InputMessageId { get; }
    /// <summary>Gets the explicit true, false, unknown, or incomplete evaluation state.</summary>
    public ProtocolEvaluationState State { get; }
    /// <summary>Gets the explanation for the reported evaluation or transition.</summary>
    public string Reason { get; }
}

/// <summary>A versioned gate state transition tied to a causal event and exact source message.</summary>
public sealed record ProtocolGateTransition
{
    /// <summary>Creates a gate transition with a positive causal revision and source-message identity.</summary>
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

    /// <summary>Gets the identity of the gate whose state changed.</summary>
    public string GateId { get; }
    /// <summary>Gets the version of the gate definition.</summary>
    public string GateVersion { get; }
    /// <summary>Gets the gate state reported by this transition.</summary>
    public ProtocolGateState State { get; }
    /// <summary>Gets the positive revision that orders this gate transition.</summary>
    public long CausalRevision { get; }
    /// <summary>Gets the source message that caused or justified this transition.</summary>
    public string SourceMessageId { get; }
    /// <summary>Gets the identity of the predicate that was evaluated.</summary>
    public string PredicateId { get; }
    /// <summary>Gets the explanation for the reported evaluation or transition.</summary>
    public string Reason { get; }
    /// <summary>Gets the optional earlier transition message superseded by this transition.</summary>
    public string? SupersedesMessageId { get; }
}

/// <summary>A CommandAi proposal outcome. This reports a host decision/result; it does not authorize or execute.</summary>
public sealed record ProtocolCommandOutcome
{
    /// <summary>Creates a correlated outcome for a command proposal; rejection requires a reason.</summary>
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

    /// <summary>Gets the command outcome stage.</summary>
    public ProtocolCommandOutcomeKind Outcome { get; }
    /// <summary>Gets the identity of the proposal associated with this outcome.</summary>
    public string ProposalId { get; }
    /// <summary>Gets the optional host-assigned command identity.</summary>
    public string? CommandId { get; }
    /// <summary>Gets the optional explanation, required when registration is rejected.</summary>
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

    /// <summary>Serializes an observer-registration request to its typed JSON payload.</summary>
    public static string Serialize(ProtocolObserverRegistrationRequest payload) => SerializePayload(payload);
    /// <summary>Serializes an observer-registration acknowledgement to its typed JSON payload.</summary>
    public static string Serialize(ProtocolObserverRegistrationAcknowledgement payload) => SerializePayload(payload);
    /// <summary>Serializes an observer evaluation to its typed JSON payload.</summary>
    public static string Serialize(ProtocolObserverEvaluation payload) => SerializePayload(payload);
    /// <summary>Serializes a source-system event to its typed JSON payload.</summary>
    public static string Serialize(ProtocolSourceEvent payload) => SerializePayload(payload);
    /// <summary>Serializes a downstream notification to its typed JSON payload.</summary>
    public static string Serialize(ProtocolDownstreamNotification payload) => SerializePayload(payload);
    /// <summary>Serializes a gate transition to its typed JSON payload.</summary>
    public static string Serialize(ProtocolGateTransition payload) => SerializePayload(payload);
    /// <summary>Serializes a command outcome to its typed JSON payload.</summary>
    public static string Serialize(ProtocolCommandOutcome payload) => SerializePayload(payload);

    /// <summary>Deserializes a typed event payload and rejects malformed or duplicate JSON properties.</summary>
    public static T Deserialize<T>(string json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Payload JSON is required.", nameof(json));
        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                throw new ArgumentException("The typed event payload must be a JSON object.", nameof(json));
            var requiredEnumProperty = RequiredEnumProperty(typeof(T));
            if (requiredEnumProperty is not null
                && !document.RootElement.TryGetProperty(requiredEnumProperty, out _))
                throw new ArgumentException($"The typed event payload must explicitly provide '{requiredEnumProperty}'.", nameof(json));
            foreach (var enumProperty in EnumProperties(typeof(T)))
            {
                if (document.RootElement.TryGetProperty(enumProperty, out var value)
                    && value.ValueKind != JsonValueKind.String)
                    throw new ArgumentException($"Typed event payload enum property '{enumProperty}' must be a JSON string.", nameof(json));
            }
            RejectDuplicateProperties(document.RootElement);
            var result = JsonSerializer.Deserialize<T>(json, Options);
            return result ?? throw new ArgumentException("The typed event payload cannot be null.", nameof(json));
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("The typed event payload is invalid.", nameof(json), ex);
        }
    }

    private static string[] EnumProperties(Type payloadType) => payloadType switch
    {
        var type when type == typeof(ProtocolObserverRegistrationRequest) => ["requestedLifecycle"],
        var type when type == typeof(ProtocolObserverRegistrationAcknowledgement) => ["status"],
        var type when type == typeof(ProtocolObserverEvaluation) => ["state"],
        var type when type == typeof(ProtocolGateTransition) => ["state"],
        var type when type == typeof(ProtocolCommandOutcome) => ["outcome"],
        var type when type == typeof(ProtocolSourceEvent) => ["outcome"],
        var type when type == typeof(ProtocolDownstreamNotification) => ["state"],
        _ => []
    };

    private static string? RequiredEnumProperty(Type payloadType) =>
        payloadType == typeof(ProtocolObserverRegistrationAcknowledgement) ? "status" :
        payloadType == typeof(ProtocolObserverEvaluation) ? "state" :
        payloadType == typeof(ProtocolGateTransition) ? "state" :
        payloadType == typeof(ProtocolCommandOutcome) ? "outcome" :
        payloadType == typeof(ProtocolSourceEvent) ? "outcome" :
        payloadType == typeof(ProtocolDownstreamNotification) ? "state" : null;

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
