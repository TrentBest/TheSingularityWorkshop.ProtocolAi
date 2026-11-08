<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" />
  </a>
</p>

# ProtocolAi Event Envelope — Version 1 Contract

## Responsibility boundary

ProtocolAi defines a versioned message boundary and stable correlation/provenance fields. It does not authenticate a producer, verify an artifact, evaluate CI, deliver or retry events, deduplicate messages at rest, authorize commands, or execute workflows. The receiving host owns durable delivery and replay handling. CiAi remains authoritative for CI evidence and verdicts.

## Version 1 envelope

Every envelope carries a schema identifier and version; unique message ID; work correlation ID; optional causation and idempotency IDs; producer identity as an asserted value (not an authentication result); event kind; UTC occurrence time; and a JSON-object payload. Optional causal revision and superseded-message identity support ordered gate transitions. The occurrence timestamp must be non-default; readers reject a missing/default timestamp rather than treating it as a meaningful event time.

The initial event kinds are observer registration requested/acknowledged, source event, observer evaluation, gate transition, downstream notification, and command accepted/rejected/completed.

Evidence-bearing source events, evaluations, gate transitions, and downstream notifications require candidate provenance. Command outcome messages require an idempotency key. Gate transitions require a positive causal revision and may name the message they supersede.

## Schema compatibility rules

The envelope schema identifier is stable for this contract family; `schemaVersion` selects the wire contract. Version 1 readers must reject an unknown schema identifier or unsupported schema version rather than guessing. A producer must not send a v2 payload while labeling it v1.

The `kind` property is required on the wire. Readers reject an omitted `kind`; they must not allow the serializer's default enum value to silently select `ObserverRegistrationRequested`. Within v1, adding an optional property is compatible only when an older reader can safely ignore it without changing the meaning or safety of the message. Existing property meanings, requiredness, event-kind semantics, and enum values are immutable within v1. Changing any of those, adding an enum value that older readers must understand, or changing payload interpretation requires a new schema version and explicit consumer support. Consumers must treat missing required data, unknown event kinds, and unrecognized enum values as non-success and fail closed. Deployments should roll out readers that understand a new version before producers emit it.

## Exact-candidate provenance

The provenance object keeps these identities separate: canonical repository owner/name; candidate ref and full candidate SHA; event SHA; tested checkout SHA; workflow-definition ref and workflow-definition SHA; workflow run ID and attempt; observed job identity; evidence reference; and evidence SHA-256 digest.

The Git SHA fields are validated as full 40- or 64-character hexadecimal object IDs; the evidence digest is exactly 64 hexadecimal characters. This is structural validation only. A receiver must independently authenticate the producer, retrieve the evidence, verify its digest and contents, and confirm that the evidence refers to the exact candidate and permitted workflow.

## Replay and causal ordering

The envelope provides the identities a host needs to implement at-least-once delivery safely. The host should apply the following rules in its durable inbox/outbox transaction; timestamps are diagnostic and must not be used as the ordering authority:

1. **Message delivery is at least once.** Persist the message ID and a digest of the received canonical envelope before acknowledging delivery. A repeated message ID with the same digest is an already-processed delivery: acknowledge it without repeating the state transition or side effect. A repeated message ID with different content is an identity collision; quarantine it and do not apply either new content or a side effect.
2. **Command effects are idempotent.** Persist the idempotency key with the command/proposal identity and terminal outcome. A retry with the same key and same command identity returns the recorded outcome. Reuse of the key for a different command identity is a conflict and must be rejected. A new message ID does not make an already-applied command new.
3. **Scope is checked before evaluation.** Reject or quarantine an event whose repository, observer, candidate SHA, workflow run/attempt, workflow definition, or evidence provenance does not match the active registration and expected candidate. A matching SHA alone is not enough if the registered workflow or evidence identity differs.
4. **Gate revisions are monotonic within a gate version and candidate.** Apply a transition only when its causal revision is greater than the last committed revision and its causal predecessor/superseded-message relationship is consistent with the host's current gate record. An identical already-committed transition is a duplicate. A lower revision is stale and cannot replace newer state. Two different transitions claiming the same revision are a conflict and must fail closed.
5. **Gaps are not guessed across.** If a higher revision arrives before its required predecessor, retain it as pending or request/replay the missing causal history; do not expose it as the current gate state until continuity is established. A later `Invalidated`, `Expired`, or `Closed` transition therefore cannot be overwritten by a delayed older `Satisfied` transition.
6. **Version and candidate changes create a new evaluation boundary.** Do not carry a prior gate revision or satisfied result into a different gate version, observer registration, or candidate SHA unless the host's explicit registration policy says that state is reusable and independently revalidates the evidence.
7. **Notifications trigger local reevaluation only.** A downstream notification identifies the transition that prompted reevaluation; the receiving repository/host evaluates its own prerequisites against its own current candidate and evidence. The notification itself is never permission to proceed.

Missing evidence and `Unknown`/`Incomplete` evaluation are non-success. Failure/cancellation diagnostics remain distinct from a success transition and cannot trigger a success action. ProtocolAi defines these interoperability semantics and carries the required identifiers, but does not persist a replay ledger, decide whether a producer is trusted, or enforce host-side delivery rules.

## CommandAi interoperability

The envelope is independent of the CommandAi proposal body. CommandAi/1 remains the proposal contract. A host can carry an encoded CommandAi/1 proposal or stable proposal identity in the payload without changing the command schema. Accepted, rejected, and completed outcomes are separate message kinds and share envelope correlation and idempotency fields. Rejection diagnostics must not be interpreted as successful completion.

## Typed payload contracts (v1)

`ProtocolEventPayloadJson` serializes the typed payloads as JSON objects using camel-case property names and string enum values. Readers require enum fields (including the optional requestedLifecycle when present and the envelope kind) to use JSON string tokens; numeric enum tokens are rejected even when the numeric value maps to a defined member. The envelope continues to carry the payload as JSON. `ProtocolTypedEventEnvelope.Create` binds each supported payload type to its event kind. `ProtocolTypedEventEnvelope.DeserializePayload<T>` validates the inbound event-kind/payload pairing and checks that gate/downstream causal metadata agrees between the envelope and typed payload before returning the payload. `ProtocolEventEnvelopeJson.Deserialize` validates the outer envelope structure and confirms that `payloadJson` contains a JSON object, but the payload is an opaque JSON string at that layer: it does not recursively inspect the nested payload object's property names or validate its typed contract. Recursive duplicate-property rejection (including inside nested extension objects) occurs when `ProtocolEventPayloadJson.Deserialize<T>` is called, including through `ProtocolTypedEventEnvelope.DeserializePayload<T>`. Consumers that need a validated typed event must use the typed reader; successfully parsing the outer envelope alone is not proof that its payload is valid for the event kind. Required enum-valued payload properties must be present on the wire; an omitted status, evaluation state, gate state, command outcome, or source outcome must not silently become the enum's zero value. `requestedLifecycle` is the exception: it is optional and defaults to `Active` by contract. SourceEvent and DownstreamNotification have typed payload contracts and factory overloads.

- `ProtocolSourceEvent` identifies the source system, event type, subject, outcome (Succeeded, Failed, Cancelled, Unknown), and summary. Exact candidate and evidence identity remain in envelope provenance; the payload outcome is a source claim, not verified CI evidence.\n- `ProtocolObserverRegistrationRequest` identifies the durable observer, scope, protocol version, and requested lifecycle.
- `ProtocolObserverRegistrationAcknowledgement` echoes observer identity/scope/version and reports Accepted, Rejected, Active, Suspended, or Removed. Rejection requires a reason.
- `ProtocolObserverEvaluation` identifies observer, predicate and predicate version, input message, and one of True, False, Unknown, or Incomplete. Every state includes an explanation; Unknown and Incomplete are explicitly not success.
- `ProtocolGateTransition` identifies gate/version, state, positive causal revision, source message, predicate, reason, and optional superseded message.
- `ProtocolDownstreamNotification` identifies the consumer repository, `awaitId`, gate/version/state/revision, and the gate-transition message that triggered local reevaluation. It is a notification, not permission to proceed.\n- `ProtocolCommandOutcome` identifies Accepted, Rejected, or Completed for a proposal, with the assigned command ID where applicable and a reason for rejection. The containing envelope still requires an idempotency key for all command outcomes.

The typed factory and reader prevent local construction and deserialization from silently pairing a supported payload with the wrong event kind; the reader also checks causal metadata consistency. This is not a trust boundary: a receiver still validates observer scope, message ordering, provenance authenticity, evidence contents, and host policy. The host retains responsibility for durable replay/deduplication.

## Deliberate next steps

The typed payload foundation and v1 schema-compatibility rules are implemented and covered by round-trip/validation tests. Remaining work: prove host-owned replay/deduplication, stale/out-of-order rejection, and durable delivery behavior without moving those responsibilities into ProtocolAi; and add end-to-end interoperability tests with CommandAi, GrammarAi, and CiAi.
---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
