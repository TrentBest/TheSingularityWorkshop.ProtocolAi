namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>Creates envelopes from supported typed payloads while fixing the event-kind/payload pairing.</summary>
/// <remarks>
/// This API guarantees the local pairing for the typed contracts. It does not validate remote scope,
/// provenance authenticity, message ordering, replay, authorization, or execution.
/// </remarks>
public static class ProtocolTypedEventEnvelope
{
    public static ProtocolEventEnvelope Create(
        string messageId, string correlationId, string producerId,
        ProtocolObserverRegistrationRequest payload, DateTimeOffset occurredAt,
        string? causationId = null)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return CreateCore(messageId, correlationId, producerId,
            ProtocolEventKind.ObserverRegistrationRequested, payload, occurredAt, causationId);
    }

    public static ProtocolEventEnvelope Create(
        string messageId, string correlationId, string producerId,
        ProtocolObserverRegistrationAcknowledgement payload, DateTimeOffset occurredAt,
        string? causationId = null)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return CreateCore(messageId, correlationId, producerId,
            ProtocolEventKind.ObserverRegistrationAcknowledged, payload, occurredAt, causationId);
    }

    public static ProtocolEventEnvelope Create(
        string messageId, string correlationId, string producerId,
        ProtocolObserverEvaluation payload, DateTimeOffset occurredAt,
        ProtocolCandidateProvenance provenance, string? causationId = null)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(provenance);
        return CreateCore(messageId, correlationId, producerId,
            ProtocolEventKind.ObserverEvaluation, payload, occurredAt, causationId, provenance);
    }

    public static ProtocolEventEnvelope Create(
        string messageId, string correlationId, string producerId,
        ProtocolGateTransition payload, DateTimeOffset occurredAt,
        ProtocolCandidateProvenance provenance, string? causationId = null)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(provenance);
        return CreateCore(messageId, correlationId, producerId,
            ProtocolEventKind.GateTransition, payload, occurredAt, causationId, provenance,
            payload.CausalRevision, payload.SupersedesMessageId);
    }

    public static ProtocolEventEnvelope Create(
        string messageId, string correlationId, string producerId,
        ProtocolCommandOutcome payload, DateTimeOffset occurredAt,
        string idempotencyKey, string? causationId = null)
    {
        ArgumentNullException.ThrowIfNull(payload);
        var kind = payload.Outcome switch
        {
            ProtocolCommandOutcomeKind.Accepted => ProtocolEventKind.CommandAccepted,
            ProtocolCommandOutcomeKind.Rejected => ProtocolEventKind.CommandRejected,
            ProtocolCommandOutcomeKind.Completed => ProtocolEventKind.CommandCompleted,
            _ => throw new ArgumentOutOfRangeException(nameof(payload))
        };
        return CreateCore(messageId, correlationId, producerId, kind, payload, occurredAt,
            causationId, idempotencyKey: idempotencyKey);
    }

    private static ProtocolEventEnvelope CreateCore<T>(
        string messageId, string correlationId, string producerId,
        ProtocolEventKind kind, T payload, DateTimeOffset occurredAt,
        string? causationId = null, ProtocolCandidateProvenance? provenance = null,
        long? causalRevision = null, string? supersedesMessageId = null,
        string? idempotencyKey = null) where T : class =>
        new(messageId, correlationId, producerId, kind, occurredAt,
            ProtocolEventPayloadJson.SerializePayload(payload), idempotencyKey, causationId,
            provenance, causalRevision, supersedesMessageId);
}
