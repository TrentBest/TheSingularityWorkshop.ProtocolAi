namespace TheSingularityWorkshop.ProtocolAi;

    /// <summary>Defines the reported outcome of a source-system event.</summary>
public enum ProtocolSourceEventOutcome
{
    /// <summary>The source reports successful completion.</summary>
    Succeeded,
    /// <summary>The source reports failed completion.</summary>
    Failed,
    /// <summary>The source reports that the operation was cancelled.</summary>
    Cancelled,
    /// <summary>The source cannot determine the operation outcome.</summary>
    Unknown
}

/// <summary>A completed source-system event; authoritative evidence remains in its provenance packet.</summary>
public sealed record ProtocolSourceEvent
{
    /// <summary>Creates a bounded source-event summary; authoritative evidence remains external.</summary>
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

    /// <summary>Gets the identity of the source system that emitted the event.</summary>
    public string SourceId { get; }
    /// <summary>Gets the source-specific event type.</summary>
    public string EventType { get; }
    /// <summary>Gets the subject or resource to which the source event applies.</summary>
    public string Subject { get; }
    /// <summary>Gets the reported outcome of the source event.</summary>
    public ProtocolSourceEventOutcome Outcome { get; }
    /// <summary>Gets a bounded human-readable summary of the source event.</summary>
    public string Summary { get; }
}

/// <summary>Routes a gate transition notice to a named downstream Await/consumer for local reevaluation.</summary>
public sealed record ProtocolDownstreamNotification
{
    /// <summary>Creates a notice for a named downstream consumer to reevaluate a gate transition.</summary>
    public ProtocolDownstreamNotification(string notificationId, string consumerRepository,
        string awaitId, string gateId, string gateVersion, ProtocolGateState state,
        long causalRevision, string gateTransitionMessageId, string reason)
    {
        NotificationId = ProtocolObserverRegistrationRequest.Required(notificationId, nameof(notificationId), 256);
        ConsumerRepository = ProtocolRepositoryIdentity.Validate(consumerRepository, nameof(consumerRepository));
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

    /// <summary>Gets the unique identity of the notification.</summary>
    public string NotificationId { get; }
    /// <summary>Gets the canonical owner/repository identity of the downstream consumer.</summary>
    public string ConsumerRepository { get; }
    /// <summary>Gets the identity of the downstream Await or consumer registration.</summary>
    public string AwaitId { get; }
    /// <summary>Gets the identity of the provider gate that changed.</summary>
    public string GateId { get; }
    /// <summary>Gets the version of the provider gate definition.</summary>
    public string GateVersion { get; }
    /// <summary>Gets the gate state reported by the notification.</summary>
    public ProtocolGateState State { get; }
    /// <summary>Gets the positive revision that orders the gate transition.</summary>
    public long CausalRevision { get; }
    /// <summary>Gets the identity of the gate-transition message that caused this notification.</summary>
    public string GateTransitionMessageId { get; }
    /// <summary>Gets the explanation for the reported notification or transition.</summary>
    public string Reason { get; }
}
