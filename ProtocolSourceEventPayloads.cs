namespace TheSingularityWorkshop.ProtocolAi;

public enum ProtocolSourceEventOutcome
{
    Succeeded,
    Failed,
    Cancelled,
    Unknown
}

/// <summary>A completed source-system event; authoritative evidence remains in its provenance packet.</summary>
public sealed record ProtocolSourceEvent
{
    public ProtocolSourceEvent(string sourceId, string eventType, string subject,
        ProtocolSourceEventOutcome outcome, string summary)
    {
        SourceId = ProtocolObserverRegistrationRequest.Required(sourceId, nameof(sourceId), 256);
        EventType = ProtocolObserverRegistrationRequest.Required(eventType, nameof(eventType), 256);
        Subject = ProtocolObserverRegistrationRequest.Required(subject, nameof(subject), 1024);
        if (!Enum.IsDefined(outcome)) throw new ArgumentOutOfRangeException(nameof(outcome));
        Outcome = outcome;
        Summary = ProtocolObserverRegistrationRequest.Required(summary, nameof(summary), 4096);
    }

    public string SourceId { get; }
    public string EventType { get; }
    public string Subject { get; }
    public ProtocolSourceEventOutcome Outcome { get; }
    public string Summary { get; }
}

/// <summary>Routes a gate transition notice to a named downstream Await/consumer for local reevaluation.</summary>
public sealed record ProtocolDownstreamNotification
{
    public ProtocolDownstreamNotification(string notificationId, string consumerRepository,
        string awaitId, string gateId, string gateVersion, ProtocolGateState state,
        long causalRevision, string gateTransitionMessageId, string reason)
    {
        NotificationId = ProtocolObserverRegistrationRequest.Required(notificationId, nameof(notificationId), 256);
        var repository = ProtocolObserverRegistrationRequest.Required(consumerRepository, nameof(consumerRepository), 256);
        var parts = repository.Split('/');
        if (parts.Length != 2 || parts.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Consumer repository must use canonical owner/name form.", nameof(consumerRepository));
        ConsumerRepository = repository;
        AwaitId = ProtocolObserverRegistrationRequest.Required(awaitId, nameof(awaitId), 256);
        GateId = ProtocolObserverRegistrationRequest.Required(gateId, nameof(gateId), 256);
        GateVersion = ProtocolObserverRegistrationRequest.Required(gateVersion, nameof(gateVersion), 128);
        if (!Enum.IsDefined(state)) throw new ArgumentOutOfRangeException(nameof(state));
        State = state;
        if (causalRevision <= 0) throw new ArgumentOutOfRangeException(nameof(causalRevision));
        CausalRevision = causalRevision;
        GateTransitionMessageId = ProtocolObserverRegistrationRequest.Required(
            gateTransitionMessageId, nameof(gateTransitionMessageId), 128);
        Reason = ProtocolObserverRegistrationRequest.Required(reason, nameof(reason), 4096);
    }

    public string NotificationId { get; }
    public string ConsumerRepository { get; }
    public string AwaitId { get; }
    public string GateId { get; }
    public string GateVersion { get; }
    public ProtocolGateState State { get; }
    public long CausalRevision { get; }
    public string GateTransitionMessageId { get; }
    public string Reason { get; }
}
