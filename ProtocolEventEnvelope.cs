using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheSingularityWorkshop.ProtocolAi;

[JsonConverter(typeof(JsonStringEnumConverter<ProtocolEventKind>))]
public enum ProtocolEventKind
{
    ObserverRegistrationRequested,
    ObserverRegistrationAcknowledged,
    SourceEvent,
    ObserverEvaluation,
    GateTransition,
    DownstreamNotification,
    CommandAccepted,
    CommandRejected,
    CommandCompleted
}

/// <summary>Complete provenance for an event concerning one exact repository candidate.</summary>
/// <remarks>These fields are claims carried by a message, not proof of producer authenticity or evidence validity.</remarks>
public sealed class ProtocolCandidateProvenance
{
    public ProtocolCandidateProvenance(string repository, string candidateRef, string candidateSha,
        string eventSha, string testedCheckoutSha, string workflowDefinitionRef, string workflowDefinitionSha,
        long workflowRunId, int workflowRunAttempt, string observedJobIdentity, string evidenceReference,
        string evidenceSha256)
    {
        Repository = Required(repository, nameof(repository), 256);
        var parts = Repository.Split('/');
        if (parts.Length != 2 || parts.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Repository must use canonical owner/name form.", nameof(repository));
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

    public string Repository { get; }
    public string CandidateRef { get; }
    public string CandidateSha { get; }
    public string EventSha { get; }
    public string TestedCheckoutSha { get; }
    public string WorkflowDefinitionRef { get; }
    public string WorkflowDefinitionSha { get; }
    public long WorkflowRunId { get; }
    public int WorkflowRunAttempt { get; }
    public string ObservedJobIdentity { get; }
    public string EvidenceReference { get; }
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
    public const string CurrentSchema = "TheSingularityWorkshop.ProtocolAi.EventEnvelope";
    public const int CurrentSchemaVersion = 1;
    public const int MaxEnvelopeCharacters = 262_144;
    public const int MaxPayloadCharacters = 196_608;

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
        if (kind == ProtocolEventKind.GateTransition && (!causalRevision.HasValue || causalRevision.Value <= 0))
            throw new ArgumentOutOfRangeException(nameof(causalRevision), "Gate transitions require a positive causal revision.");
        if (causalRevision.HasValue && causalRevision.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(causalRevision));
        CausalRevision = causalRevision;
        SupersedesMessageId = Optional(supersedesMessageId, nameof(supersedesMessageId), 128);
        if (IsCommandOutcome(kind) && IdempotencyKey is null)
            throw new ArgumentException("Command outcome messages require an idempotency key.", nameof(idempotencyKey));
    }

    public string Schema => CurrentSchema;
    public int SchemaVersion => CurrentSchemaVersion;
    public string MessageId { get; }
    public string CorrelationId { get; }
    public string? CausationId { get; }
    public string? IdempotencyKey { get; }
    public string ProducerId { get; }
    public ProtocolEventKind Kind { get; }
    public DateTimeOffset OccurredAt { get; }
    public ProtocolCandidateProvenance? Provenance { get; }
    public long? CausalRevision { get; }
    public string? SupersedesMessageId { get; }
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
