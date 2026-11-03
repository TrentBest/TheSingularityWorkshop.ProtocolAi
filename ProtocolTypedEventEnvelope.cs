namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>Creates envelopes from supported typed payloads while fixing the event-kind/payload pairing.</summary>
/// <remarks>
/// This API guarantees the local pairing for the typed contracts. It does not validate remote scope,
/// provenance authenticity, message ordering, replay, authorization, or execution.
/// </remarks>
public static class ProtocolTypedEventEnvelope
{
    /// <summary>Deserializes a typed payload only when its event kind and envelope metadata agree.</summary>
    /// <exception cref="ArgumentException">The requested payload type does not match the envelope kind or metadata.</exception>
    public static T DeserializePayload<T>(ProtocolEventEnvelope envelope) where T : class
    {
        ArgumentNullException.ThrowIfNull(envelope);
        var payload = ProtocolEventPayloadJson.Deserialize<T>(envelope.PayloadJson);
        var expectedKind = typeof(T) switch
        {
            var type when type == typeof(ProtocolObserverRegistrationRequest) =>
                ProtocolEventKind.ObserverRegistrationRequested,
            var type when type == typeof(ProtocolObserverRegistrationAcknowledgement) =>
                ProtocolEventKind.ObserverRegistrationAcknowledged,
            var type when type == typeof(ProtocolSourceEvent) =>
                ProtocolEventKind.SourceEvent,
            var type when type == typeof(ProtocolObserverEvaluation) =>
                ProtocolEventKind.ObserverEvaluation,
            var type when type == typeof(ProtocolGateTransition) =>
                ProtocolEventKind.GateTransition,
            var type when type == typeof(ProtocolDownstreamNotification) =>
                ProtocolEventKind.DownstreamNotification,
            var type when type == typeof(ProtocolCommandOutcome) => CommandOutcomeKind((ProtocolCommandOutcome)(object)payload),
            _ => throw new NotSupportedException($"No typed event mapping is registered for payload type '{typeof(T).FullName}'.")
        };

        if (envelope.Kind != expectedKind)
            throw new ArgumentException($"Event kind '{envelope.Kind}' does not match payload type '{typeof(T).Name}'.", nameof(envelope));

        if (payload is ProtocolGateTransition gate &&
            (envelope.CausalRevision != gate.CausalRevision ||
             !string.Equals(envelope.SupersedesMessageId, gate.SupersedesMessageId, StringComparison.Ordinal)))
            throw new ArgumentException("Gate payload causality must match envelope causal metadata.", nameof(envelope));

        if (payload is ProtocolDownstreamNotification notification &&
            envelope.CausalRevision != notification.CausalRevision)
            throw new ArgumentException("Downstream notification revision must match envelope causal metadata.", nameof(envelope));

        return payload;
    }

    private static ProtocolEventKind CommandOutcomeKind(ProtocolCommandOutcome payload) =>
        payload.Outcome switch
        {
            ProtocolCommandOutcomeKind.Accepted => ProtocolEventKind.CommandAccepted,
            ProtocolCommandOutcomeKind.Rejected => ProtocolEventKind.CommandRejected,
            ProtocolCommandOutcomeKind.Completed => ProtocolEventKind.CommandCompleted,
            _ => throw new ArgumentOutOfRangeException(nameof(payload))
        };

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
        ProtocolSourceEvent payload, DateTimeOffset occurredAt,
        ProtocolCandidateProvenance provenance, string? causationId = null)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(provenance);
        return CreateCore(messageId, correlationId, producerId,
            ProtocolEventKind.SourceEvent, payload, occurredAt, causationId, provenance);
    }

    public static ProtocolEventEnvelope Create(
        string messageId, string correlationId, string producerId,
        ProtocolDownstreamNotification payload, DateTimeOffset occurredAt,
        ProtocolCandidateProvenance provenance, string? causationId = null)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(provenance);
        return CreateCore(messageId, correlationId, producerId,
            ProtocolEventKind.DownstreamNotification, payload, occurredAt, causationId, provenance,
            payload.CausalRevision);
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
