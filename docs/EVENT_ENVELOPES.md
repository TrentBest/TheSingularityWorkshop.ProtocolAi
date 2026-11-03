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

## Deliberate next steps

This first contract slice establishes envelope versioning, candidate provenance, correlation, idempotency metadata, and causal ordering. Follow-on work must define typed payload schemas for observer registration, observer evaluation, gate transitions, and command outcomes; explicit true/false/unknown/incomplete states; deterministic replay policy; and cross-repository integration tests with CommandAi, GrammarAi, and CiAi.