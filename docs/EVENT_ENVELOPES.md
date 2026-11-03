<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" />
  </a>
</p>

# ProtocolAi Event Envelope — Initial Contract

## Responsibility boundary

ProtocolAi defines a versioned message boundary and stable correlation/provenance fields. It does not authenticate a producer, verify an artifact, evaluate CI, deliver or retry events, deduplicate messages at rest, authorize commands, or execute workflows. The receiving host owns durable delivery and replay handling. CiAi remains authoritative for CI evidence and verdicts.

## Version 1 envelope

Every envelope carries a schema identifier and version; unique message ID; work correlation ID; optional causation and idempotency IDs; producer identity as an asserted value (not an authentication result); event kind; UTC occurrence time; and a JSON-object payload. Optional causal revision and superseded-message identity support ordered gate transitions.

The initial event kinds are observer registration requested/acknowledged, source event, observer evaluation, gate transition, downstream notification, and command accepted/rejected/completed.

Evidence-bearing source events, evaluations, gate transitions, and downstream notifications require candidate provenance. Command outcome messages require an idempotency key. Gate transitions require a positive causal revision and may name the message they supersede.

## Exact-candidate provenance

The provenance object keeps these identities separate: canonical repository owner/name; candidate ref and full candidate SHA; event SHA; tested checkout SHA; workflow-definition ref and workflow-definition SHA; workflow run ID and attempt; observed job identity; evidence reference; and evidence SHA-256 digest.

The Git SHA fields are validated as full 40- or 64-character hexadecimal object IDs; the evidence digest is exactly 64 hexadecimal characters. This is structural validation only. A receiver must independently authenticate the producer, retrieve the evidence, verify its digest and contents, and confirm that the evidence refers to the exact candidate and permitted workflow.

## Replay and causal ordering

The envelope provides the identities a host needs to implement at-least-once delivery safely: deduplicate by message ID and command effects by idempotency key; correlate messages through the work correlation ID and causal chain; reject events whose repository, candidate SHA, workflow run/attempt, or evidence provenance does not match the registered observer scope; and compare causal revisions so a delayed older Satisfied event cannot overwrite a newer Invalidated event.

Missing evidence and unknown/incomplete evaluation are non-success. A success gate is never inferred from a bare Boolean detached from evidence. ProtocolAi does not persist a replay ledger or enforce host-side delivery rules.

## CommandAi interoperability

The envelope is independent of the CommandAi proposal body. CommandAi/1 remains the proposal contract. A host can carry an encoded CommandAi/1 proposal or stable proposal identity in the payload without changing the command schema. Accepted, rejected, and completed outcomes are separate message kinds and share envelope correlation and idempotency fields. Rejection diagnostics must not be interpreted as successful completion.

## Typed payload contracts (v1)

`ProtocolEventPayloadJson` serializes the typed payloads as JSON objects using camel-case property names and string enum values. The envelope continues to carry the payload as JSON. `ProtocolTypedEventEnvelope.Create` provides typed overloads that bind each supported payload type to its only valid event kind; consumers should still validate the wire payload against that kind when deserializing untrusted messages. SourceEvent and DownstreamNotification remain generic because their domain payload contracts are not yet defined.

- `ProtocolObserverRegistrationRequest` identifies the durable observer, scope, protocol version, and requested lifecycle.
- `ProtocolObserverRegistrationAcknowledgement` echoes observer identity/scope/version and reports Accepted, Rejected, Active, Suspended, or Removed. Rejection requires a reason.
- `ProtocolObserverEvaluation` identifies observer, predicate and predicate version, input message, and one of True, False, Unknown, or Incomplete. Every state includes an explanation; Unknown and Incomplete are explicitly not success.
- `ProtocolGateTransition` identifies gate/version, state, positive causal revision, source message, predicate, reason, and optional superseded message.
- `ProtocolCommandOutcome` identifies Accepted, Rejected, or Completed for a proposal, with the assigned command ID where applicable and a reason for rejection. The containing envelope still requires an idempotency key for all command outcomes.

The typed factory prevents local event-kind/payload mismatches for the five supported payload contracts and copies gate causal revision/superseded-message metadata from the gate payload into the envelope. This is not a trust boundary: a receiver still validates the wire payload, observer scope, message ordering, provenance, and host policy. The host retains responsibility for durable replay/deduplication.

## Deliberate next steps

The typed payload foundation is now implemented and covered by round-trip/validation tests. Remaining work: specify host replay/deduplication and stale-event acceptance tests without moving host responsibilities into ProtocolAi; and add end-to-end interoperability tests with CommandAi, GrammarAi, and CiAi.
---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
