using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>Identifies the kind of event carried by a ProtocolAi event envelope.</summary>
[JsonConverter(typeof(JsonStringEnumConverter<ProtocolEventKind>))]
public enum ProtocolEventKind
{
    /// <summary>Identifies a request to register or change an observer.</summary>
    ObserverRegistrationRequested,
    /// <summary>Identifies a host acknowledgement of an observer registration.</summary>
    ObserverRegistrationAcknowledged,
    /// <summary>Identifies a completed event reported by an external source system.</summary>
    SourceEvent,
    /// <summary>Identifies an observer's explicit evaluation result.</summary>
    ObserverEvaluation,
    /// <summary>Identifies a versioned gate state transition.</summary>
    GateTransition,
    /// <summary>Identifies a notice asking a downstream consumer to reevaluate.</summary>
    DownstreamNotification,
    /// <summary>Identifies a host acceptance of a command proposal.</summary>
    CommandAccepted,
    /// <summary>Identifies a host rejection of a command proposal.</summary>
    CommandRejected,
    /// <summary>Identifies a reported command completion.</summary>
    CommandCompleted
}

/// <summary>Complete provenance for an event concerning one exact repository candidate.</summary>
/// <remarks>These fields are claims carried by a message, not proof of producer authenticity or evidence validity.</remarks>
public sealed class ProtocolCandidateProvenance
{
    /// <summary>Creates validated provenance claims for one repository candidate and its evidence.</summary>
    public ProtocolCandidateProvenance(string repository, string candidateRef, string candidateSha,
        string eventSha, string testedCheckoutSha, string workflowDefinitionRef, string workflowDefinitionSha,
        long workflowRunId, int workflowRunAttempt, string observedJobIdentity, string evidenceReference,
        string evidenceSha256)
    {
        Repository = ProtocolRepositoryIdentity.Validate(repository, nameof(repository));
        CandidateRef = Required(candidateRef, nameof(candidateRef), 512);
        CandidateSha = GitSha(candidateSha, nameof(candidateSha));
        EventSha = GitSha(eventSha, nameof(eventSha));
        TestedCheckoutSha = GitSha(testedCheckoutSha, nameof(testedCheckoutSha));
        WorkflowDefinitionRef = Required(workflowDefinitionRef, nameof(workflowDefinitionRef), 512);
        WorkflowDefinitionSha = GitSha(workflowDefinitionSha, nameof(workflowDefinitionSha));
        if (workflowRunId <= 0) throw new ArgumentOutOfRangeException(nameof(workflowRunId));
        if (workflowRunAttempt <= 0) throw new ArgumentOutOfRangeException(nameof(workflowRunAttempt));
        WorkflowRunId = workflowRunId;
        WorkflowRunAttempt = workflowRunAttempt;
        ObservedJobIdentity = Required(observedJobIdentity, nameof(observedJobIdentity), 512);
        EvidenceReference = Required(evidenceReference, nameof(evidenceReference), 2048);
        EvidenceSha256 = HexDigest(evidenceSha256, nameof(evidenceSha256));
    }

    /// <summary>Gets the canonical owner/repository identity to which the provenance claims apply.</summary>
    public string Repository { get; }
    /// <summary>Gets the candidate Git reference associated with the observed work.</summary>
    public string CandidateRef { get; }
    /// <summary>Gets the full Git object ID of the candidate revision.</summary>
    public string CandidateSha { get; }
    /// <summary>Gets the full Git object ID reported by the source event.</summary>
    public string EventSha { get; }
    /// <summary>Gets the full Git object ID of the checkout that was actually tested.</summary>
    public string TestedCheckoutSha { get; }
    /// <summary>Gets the reference containing the workflow definition.</summary>
    public string WorkflowDefinitionRef { get; }
    /// <summary>Gets the full Git object ID of the workflow definition revision.</summary>
    public string WorkflowDefinitionSha { get; }
    /// <summary>Gets the authoritative provider workflow-run identifier claimed by the producer.</summary>
    public long WorkflowRunId { get; }
    /// <summary>Gets the provider workflow-run attempt number.</summary>
    public int WorkflowRunAttempt { get; }
    /// <summary>Gets the identity of the job whose result is being reported.</summary>
    public string ObservedJobIdentity { get; }
    /// <summary>Gets the reference or URL for the associated evidence artifact.</summary>
    public string EvidenceReference { get; }
    /// <summary>Gets the SHA-256 digest claimed for the evidence artifact.</summary>
    public string EvidenceSha256 { get; }

    private static string Required(string value, string parameter, int max)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A non-empty value is required.", parameter);
        if (value.Length > max) throw new ArgumentException($"Value cannot exceed {max} characters.", parameter);
        return value;
    }

    private static string GitSha(string value, string parameter)
    {
        value = Required(value, parameter, 64);
        if ((value.Length != 40 && value.Length != 64) || !value.All(Uri.IsHexDigit))
            throw new ArgumentException("A full 40- or 64-character hexadecimal Git object ID is required.", parameter);
        return value.ToLowerInvariant();
    }

    private static string HexDigest(string value, string parameter)
    {
        value = Required(value, parameter, 64);
        if (value.Length != 64 || !value.All(Uri.IsHexDigit))
            throw new ArgumentException("A 64-character hexadecimal SHA-256 digest is required.", parameter);
        return value.ToLowerInvariant();
    }
}

/// <summary>A versioned, correlated message boundary for cross-component and cross-repository exchange.</summary>
/// <remarks>
/// The envelope establishes message identity and carries provenance. It does not authenticate the producer,
/// verify evidence, deduplicate delivery, evaluate CI, authorize a command, or execute a workflow.
/// Those responsibilities remain with the receiving host and authoritative evidence provider.
/// </remarks>
public sealed class ProtocolEventEnvelope
{
    /// <summary>Identifies the current event-envelope wire schema.</summary>
    public const string CurrentSchema = "TheSingularityWorkshop.ProtocolAi.EventEnvelope";
    /// <summary>Identifies the current version of the event-envelope wire schema.</summary>
    public const int CurrentSchemaVersion = 1;
    /// <summary>Defines the maximum serialized event-envelope character count.</summary>
    public const int MaxEnvelopeCharacters = 262_144;
    /// <summary>Defines the maximum embedded event-payload character count.</summary>
    public const int MaxPayloadCharacters = 196_608;

    /// <summary>Creates a versioned event envelope and validates its message, payload, and causal metadata.</summary>
    public ProtocolEventEnvelope(string messageId, string correlationId, string producerId, ProtocolEventKind kind,
        DateTimeOffset occurredAt, string payloadJson, string? idempotencyKey = null, string? causationId = null,
        ProtocolCandidateProvenance? provenance = null, long? causalRevision = null, string? supersedesMessageId = null)
    {
        MessageId = Required(messageId, nameof(messageId), 128);
        CorrelationId = Required(correlationId, nameof(correlationId), 128);
        ProducerId = Required(producerId, nameof(producerId), 256);
        if (!Enum.IsDefined(kind)) throw new ArgumentOutOfRangeException(nameof(kind));
        Kind = kind;
        if (occurredAt == default)
            throw new ArgumentOutOfRangeException(nameof(occurredAt), "A non-default occurrence timestamp is required.");
        OccurredAt = occurredAt.ToUniversalTime();
        if (string.IsNullOrWhiteSpace(payloadJson)) throw new ArgumentException("A JSON object payload is required.", nameof(payloadJson));
        if (payloadJson.Length > MaxPayloadCharacters) throw new ArgumentException($"Payload cannot exceed {MaxPayloadCharacters} characters.", nameof(payloadJson));
        try
        {
            using var document = JsonDocument.Parse(payloadJson);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                throw new ArgumentException("The event payload must be a JSON object.", nameof(payloadJson));
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("The event payload must be valid JSON.", nameof(payloadJson), ex);
        }
        PayloadJson = payloadJson;
        IdempotencyKey = Optional(idempotencyKey, nameof(idempotencyKey), 128);
        CausationId = Optional(causationId, nameof(causationId), 128);
        if (RequiresProvenance(kind) && provenance is null)
            throw new ArgumentException($"Event kind '{kind}' requires exact-candidate provenance.", nameof(provenance));
        Provenance = provenance;
        if (kind is ProtocolEventKind.GateTransition or ProtocolEventKind.DownstreamNotification)
        {
            if (!causalRevision.HasValue || causalRevision.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(causalRevision),
                    $"{kind} events require a positive causal revision.");
        }
        else if (causalRevision.HasValue)
        {
            throw new ArgumentException(
                $"Event kind '{kind}' does not define causal revision metadata.", nameof(causalRevision));
        }
        CausalRevision = causalRevision;
        if (supersedesMessageId is not null && kind != ProtocolEventKind.GateTransition)
            throw new ArgumentException(
                "Only gate transitions may identify a superseded message.", nameof(supersedesMessageId));
        SupersedesMessageId = Optional(supersedesMessageId, nameof(supersedesMessageId), 128);
        if (IsCommandOutcome(kind) && IdempotencyKey is null)
            throw new ArgumentException("Command outcome messages require an idempotency key.", nameof(idempotencyKey));
    }

    /// <summary>Gets the event-envelope schema identifier.</summary>
    public string Schema => CurrentSchema;
    /// <summary>Gets the event-envelope schema version.</summary>
    public int SchemaVersion => CurrentSchemaVersion;
    /// <summary>Gets the unique identity of this message.</summary>
    public string MessageId { get; }
    /// <summary>Gets the identity that groups messages belonging to the same logical operation.</summary>
    public string CorrelationId { get; }
    /// <summary>Gets the optional identity of the message that caused this message.</summary>
    public string? CausationId { get; }
    /// <summary>Gets the optional operation identity used by the host for its idempotency policy.</summary>
    public string? IdempotencyKey { get; }
    /// <summary>Gets the producer identity asserted by the message.</summary>
    public string ProducerId { get; }
    /// <summary>Gets the typed event kind that determines the payload contract.</summary>
    public ProtocolEventKind Kind { get; }
    /// <summary>Gets the UTC occurrence timestamp asserted by the producer.</summary>
    public DateTimeOffset OccurredAt { get; }
    /// <summary>Gets the optional exact-candidate provenance claims required by selected event kinds.</summary>
    public ProtocolCandidateProvenance? Provenance { get; }
    /// <summary>Gets the optional positive revision used by gate-transition and downstream-notification events.</summary>
    public long? CausalRevision { get; }
    /// <summary>Gets the optional prior message identity superseded by a gate transition.</summary>
    public string? SupersedesMessageId { get; }
    /// <summary>Gets the JSON object containing the event-kind-specific payload.</summary>
    public string PayloadJson { get; }

    private static bool RequiresProvenance(ProtocolEventKind kind) =>
        kind is ProtocolEventKind.SourceEvent or ProtocolEventKind.ObserverEvaluation
            or ProtocolEventKind.GateTransition or ProtocolEventKind.DownstreamNotification;
    private static bool IsCommandOutcome(ProtocolEventKind kind) =>
        kind is ProtocolEventKind.CommandAccepted or ProtocolEventKind.CommandRejected or ProtocolEventKind.CommandCompleted;
    private static string Required(string value, string parameter, int max)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A non-empty value is required.", parameter);
        if (value.Length > max) throw new ArgumentException($"Value cannot exceed {max} characters.", parameter);
        return value;
    }
    private static string? Optional(string? value, string parameter, int max) =>
        value is null ? null : Required(value, parameter, max);
}

/// <summary>Serializes and validates versioned ProtocolAi event envelopes.</summary>
public static class ProtocolEventEnvelopeJson
{
    private sealed record WireProvenance(string Repository, string CandidateRef, string CandidateSha,
        string EventSha, string TestedCheckoutSha, string WorkflowDefinitionRef, string WorkflowDefinitionSha,
        long WorkflowRunId, int WorkflowRunAttempt, string ObservedJobIdentity, string EvidenceReference,
        string EvidenceSha256);
    private sealed record WireEnvelope(string Schema, int SchemaVersion, string MessageId, string CorrelationId,
        string? CausationId, string? IdempotencyKey, string ProducerId, ProtocolEventKind Kind,
        DateTimeOffset OccurredAt, WireProvenance? Provenance, long? CausalRevision,
        string? SupersedesMessageId, string PayloadJson);

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>Serializes an event envelope using the current versioned JSON wire schema.</summary>
    public static string Serialize(ProtocolEventEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        var p = envelope.Provenance;
        var provenance = p is null ? null : new WireProvenance(p.Repository, p.CandidateRef, p.CandidateSha,
            p.EventSha, p.TestedCheckoutSha, p.WorkflowDefinitionRef, p.WorkflowDefinitionSha,
            p.WorkflowRunId, p.WorkflowRunAttempt, p.ObservedJobIdentity, p.EvidenceReference, p.EvidenceSha256);
        var wire = new WireEnvelope(envelope.Schema, envelope.SchemaVersion, envelope.MessageId,
            envelope.CorrelationId, envelope.CausationId, envelope.IdempotencyKey, envelope.ProducerId,
            envelope.Kind, envelope.OccurredAt, provenance, envelope.CausalRevision,
            envelope.SupersedesMessageId, envelope.PayloadJson);
        var json = JsonSerializer.Serialize(wire, Options);
        if (json.Length > ProtocolEventEnvelope.MaxEnvelopeCharacters)
            throw new ArgumentException($"Serialized envelope cannot exceed {ProtocolEventEnvelope.MaxEnvelopeCharacters} characters.", nameof(envelope));
        return json;
    }

    /// <summary>Deserializes and validates an event envelope from its versioned JSON wire representation.</summary>
    public static ProtocolEventEnvelope Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("Event envelope JSON is required.", nameof(json));
        if (json.Length > ProtocolEventEnvelope.MaxEnvelopeCharacters)
            throw new ArgumentException($"Event envelope cannot exceed {ProtocolEventEnvelope.MaxEnvelopeCharacters} characters.", nameof(json));
        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object
                || !document.RootElement.TryGetProperty("kind", out var kind))
                throw new ArgumentException("The event envelope must explicitly provide a kind.", nameof(json));
            if (kind.ValueKind != JsonValueKind.String)
                throw new ArgumentException("The event envelope kind must be a JSON string.", nameof(json));
            RejectDuplicateProperties(document.RootElement);
            var wire = JsonSerializer.Deserialize<WireEnvelope>(json, Options)
                ?? throw new ArgumentException("Event envelope JSON cannot be null.", nameof(json));
            if (!string.Equals(wire.Schema, ProtocolEventEnvelope.CurrentSchema, StringComparison.Ordinal))
                throw new ArgumentException($"Unsupported event envelope schema '{wire.Schema}'.", nameof(json));
            if (wire.SchemaVersion != ProtocolEventEnvelope.CurrentSchemaVersion)
                throw new ArgumentException($"Unsupported event envelope version '{wire.SchemaVersion}'.", nameof(json));
            var p = wire.Provenance;
            var provenance = p is null ? null : new ProtocolCandidateProvenance(p.Repository, p.CandidateRef,
                p.CandidateSha, p.EventSha, p.TestedCheckoutSha, p.WorkflowDefinitionRef,
                p.WorkflowDefinitionSha, p.WorkflowRunId, p.WorkflowRunAttempt, p.ObservedJobIdentity,
                p.EvidenceReference, p.EvidenceSha256);
            return new ProtocolEventEnvelope(wire.MessageId, wire.CorrelationId, wire.ProducerId,
                wire.Kind, wire.OccurredAt, wire.PayloadJson, wire.IdempotencyKey, wire.CausationId,
                provenance, wire.CausalRevision, wire.SupersedesMessageId);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("The event envelope is not valid JSON or does not match the wire schema.", nameof(json), ex);
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
            foreach (var item in element.EnumerateArray()) RejectDuplicateProperties(item);
    }
}
