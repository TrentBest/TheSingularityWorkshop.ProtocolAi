<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" />
  </a>
</p>

# Cross-Repository Event Conformance Plan

**Status:** Integration plan; host-side interoperability evidence is still outstanding.  
**Protocol baseline:** ProtocolAi event envelope schema v1 and typed payload contracts.

## Purpose and ownership

This plan turns the ProtocolAi v1 envelope into verifiable interoperability evidence across CommandAi, GrammarAi, and CiAi. It deliberately separates the wire contract from host behavior.

- **ProtocolAi:** versioned envelope, typed payload/event-kind mapping, correlation, idempotency fields, provenance shape, validation rules, and shared conformance fixtures.
- **CiAi:** authoritative CI run/evidence identity and verdict; exact candidate/workflow/evidence verification.
- **CommandAi:** command proposal/outcome semantics and idempotency of command effects.
- **GrammarAi:** structural representation and validation of observer/predicate/action proposals; it does not establish evidence truth.
- **Host / FSM_Foundry:** producer trust, durable inbox/outbox, delivery/retry, deduplication, replay, causal ordering, authorization, and execution.

ProtocolAi must not grow a workflow engine, CI evaluator, identity provider, durable message broker, or authorization layer to make these tests pass.

## Shared conformance fixture set

ProtocolAi should publish source-controlled, deterministic JSON fixtures (or equivalent test builders) for:

1. Valid v1 envelope for each supported event kind.
2. Valid typed payload paired with its matching event kind.
3. Kind/payload mismatch.
4. Unsupported schema identifier/version.
5. Missing or default timestamp; malformed IDs; oversized envelope/payload.
6. Duplicate JSON properties at envelope and nested payload levels.
7. Invalid/missing candidate provenance and invalid evidence digest.
8. Gate transition with missing/non-positive causal revision.
9. Downstream notification whose payload and envelope causal revisions disagree.
10. Command outcome without idempotency key.
11. Unknown/incomplete evaluation and failed/cancelled source outcome.
12. Duplicate message ID with same content and same ID with conflicting content.

Fixtures must be version-pinned, deterministic, and safe to reuse across repositories. Each host records the ProtocolAi contract version and fixture-set revision it tested.

## Host integration matrix

| Host | Producer/consumer path | Required proof | Owning responsibility |
|---|---|---|---|
| CiAi | CI completion/evidence → typed source event and evidence-bound evaluation/gate inputs | Event identifies repository, candidate ref/SHA, event SHA, tested checkout SHA, workflow-definition SHA, run/attempt, job, evidence reference/digest; host rejects evidence that does not match the exact candidate and workflow | CI truth, artifact retrieval/digest verification, gate predicate evaluation |
| GrammarAi | Observer registration/predicate/action proposal → validated structural representation | Version-pinned grammar accepts valid bounded structures; rejects unknown IDs, malformed roles/slots, scope/version mismatch, missing evidence fields, and unsafe/unbounded expressions; unknown remains distinct from false/true | Grammar structure/type/cardinality validation; no evidence verdict |
| CommandAi | Command proposal → accepted/rejected/completed outcome envelopes | Proposal identity is correlated; outcomes use stable idempotency key; same-key/same-command retry returns recorded outcome; same key with different command is rejected; rejection cannot be mistaken for completion | Command identity, outcome lifecycle, effect idempotency |
| FSM_Foundry / host | Registration, event delivery, gate transitions, downstream notification | Durable inbox/outbox; at-least-once delivery; same-ID/same-digest dedup; identity collision quarantine; monotonic revisions; causal-gap buffering/replay; stale transition rejection; notification triggers local reevaluation only | Trust, delivery, durable state, replay, causal ordering, authorization, execution |

## Minimum end-to-end scenarios

### A. Exact-candidate success

1. CiAi produces a successful build/test evidence record for candidate SHA A.
2. The host authenticates the producer, retrieves the evidence, verifies its digest and workflow identity, and checks that the evidence is for SHA A.
3. A typed source event and evaluation are correlated to the exact candidate and evidence.
4. A gate transition advances the gate revision only after the host's predicate succeeds.
5. A downstream notification identifies that exact gate transition and prompts the consumer to reevaluate its own prerequisites.

Expected: the host records a satisfied gate for SHA A only. ProtocolAi serialization success alone is never sufficient.

### B. Failure, cancellation, and incomplete evidence

Exercise failed and cancelled source outcomes, missing artifacts, digest mismatch, wrong checkout SHA, wrong workflow-definition SHA, and Unknown/Incomplete evaluation.

Expected: diagnostics remain observable, but none can produce a satisfied gate or authorize a command.

### C. Duplicate and conflicting delivery

Deliver the same message ID and canonical digest twice; then deliver the same ID with altered content. Repeat a command with the same idempotency key and same command identity, then reuse that key for a different command.

Expected: identical delivery is acknowledged without repeating effects; identity collision is quarantined; idempotent command retry returns the recorded outcome; conflicting key reuse is rejected.

### D. Delayed, stale, and out-of-order transitions

Deliver revision 3 before revision 2; deliver a delayed revision 1 after revision 3; deliver two different transitions claiming revision 3; then deliver a later invalidation followed by an older satisfied transition.

Expected: gaps remain pending until continuity is established; stale transitions cannot overwrite newer state; same-revision conflict fails closed; invalidation cannot be undone by delayed older success.

### E. Schema evolution

Test a reader against supported v1 fixtures and unknown future schema identifiers/versions. Confirm additive optional fields are ignored only where v1 compatibility rules permit. Confirm changed meanings or enum semantics require an explicit new schema version and reader rollout before producer rollout.

Expected: unsupported versions are rejected as non-success; no consumer guesses or silently upgrades payload meaning.

## Evidence required from each repository

Each host integration should provide:
- repository, commit SHA, and CI run URL;
- ProtocolAi schema version and fixture-set revision;
- exact fixture/scenario identifiers executed;
- test result and machine-readable report/artifact;
- explicit pass/fail for each host-owned obligation;
- any exclusions, known gaps, and follow-up issue links.

A passing ProtocolAi unit/conformance suite proves only the library's local contract behavior. It does not close the host integration criteria.

## Sequencing

1. Freeze the ProtocolAi v1 fixture inventory and make fixtures easy for hosts to consume without introducing unnecessary package dependencies.
2. Run the host-specific matrix in CiAi, GrammarAi, and CommandAi using their existing test frameworks and dependency boundaries.
3. Implement or verify durable delivery/replay behavior in the host/FSM_Foundry that actually owns it.
4. Collect exact commit/run evidence and review mismatches with the owning repository.
5. Update ProtocolAi issue #13 only when all required host evidence is available and reviewed.

## Current acceptance state

- ProtocolAi v1 schema, typed payload contracts, and local validation/conformance tests: implemented and documented.
- Real CommandAi/GrammarAi/CiAi interoperability: not yet evidenced.
- Host durable inbox/outbox, deduplication, replay, causal-gap buffering, and delivery guarantees: specified by ProtocolAi, but implementation evidence must come from the owning hosts.
- No release or package publication is implied or authorized.

Related: [ProtocolAi issue #13](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/issues/13), [Event Envelope v1](EVENT_ENVELOPES.md).

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
