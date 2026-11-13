# ProtocolAi — ecosystem adoption map

**Status:** implementation guidance, not a release or dependency mandate.  
**Authority:** ProtocolAi owns reusable protocol identity and versioned exchange contracts. Each consuming repository owns its domain semantics and decisions.

## 1. The rule

Use ProtocolAi when a repository must name, encode, validate, or exchange a stable meaning across a repository/process/host boundary.

ProtocolAi may provide:
- application-defined vocabularies and stable integer symbol identities through `ProtocolBuilder` / `ProtocolDefinition`;
- versioned event envelopes and typed payload contracts;
- correlation, causation, idempotency, producer identity, timestamps, provenance, and causal revision fields;
- deterministic serialization/deserialization and validation;
- portable conformance fixtures.

ProtocolAi does **not** provide:
- transport (HTTP, WebSocket, TCP, UDP, queues, GitHub dispatch);
- durable inbox/outbox, storage, delivery, retries, ordering, deduplication, replay, or exactly-once guarantees;
- authentication, authorization, trust decisions, human approval, policy, or capability grants;
- workflow orchestration, command execution, provider invocation, scheduling, or UI;
- domain-owned truth, evidence verification, or semantic acceptance.

An envelope's presence is not proof that its producer is trusted, its claim is true, or its requested effect is authorized.

## 2. Adoption levels

1. **Direct contract consumer/producer:** references the released ProtocolAi contract package (only when the required API is actually released and version-pinned).
2. **Fixture-only conformance:** consumes the portable JSON fixtures in tests without adding a runtime dependency. This is the preferred first step for independent repositories.
3. **Vocabulary mapping:** defines its own domain vocabulary with ProtocolBuilder when stable cross-boundary names/IDs are useful.
4. **No adoption:** keep the repository independent when it owns local domain logic and has no current exchange boundary.

Do not add a ProtocolAi package reference merely to use integer IDs internally. Do not make ProtocolAi a transitive prerequisite for unrelated MicroBundles.

## 3. Repository-by-repository instructions

### AI language and execution

| Repository | Required use | Explicitly do not |
|---|---|---|
| **GrammarAi** | Keep ProtocolAi references as external symbol identities. GrammarAi owns grammar IDs, nonterminals, productions, ordering, and structural validation. Add conformance tests proving referenced protocol/symbol IDs resolve against a supplied ProtocolDefinition; preserve unknown/foreign-reference rejection. Use fixture-only tests for envelope payloads only if GrammarAi itself consumes event contracts. | Do not own or duplicate the ProtocolAi vocabulary registry; do not make grammar structure evidence of truth, permission, or execution. |
| **CommandAi** | Use ProtocolAi vocabulary identities for stable command/event names where they cross a host boundary. Map command proposal and outcome to typed event kinds/payloads; include correlation and idempotency identity. CommandAi remains owner of command meaning, proposal validation, outcome semantics, and effect-specific idempotency rules. | Do not dispatch, authorize, execute, or claim durable idempotency based only on an envelope. Do not force Profiles or FSM_REST. |
| **HeadlessAi** | Keep provider request/response adapters provider-specific. At the host integration edge, optionally wrap a completed invocation, failure, or cancellation as a ProtocolAi event with correlation and provenance. Preserve raw provider response separately from validated application meaning. | Do not make ProtocolAi a required dependency for simple HTTP calls; do not treat model output or a valid envelope as trusted instructions. |
| **CiAi** | Adopt the event contract as the shared evidence vocabulary between coordinator, GitHub adapter, and Cogs. Use exact run/request/candidate identities, correlation/causation, idempotency, producer identity, and provenance. CiAi owns candidate identity, evidence collection, authoritative CI verification, and verdict. Require matching repository, full commit SHA, workflow/run identity, and evidence digest before accepting observations. | Do not let ProtocolAi decide whether a build passed, whether evidence is authoritative, or whether a repair may be dispatched. |
| **CiAi.CogA** | No runtime ProtocolAi dependency needed for the catalog's local capability records. If catalog changes are emitted, publish typed capability-declared/changed events with provider and pinned revision provenance. | Registration is not authorization or proof of provider behavior. |
| **CiAi.CogB** | Map bounded request creation/acceptance and request lifecycle notifications to typed events. Preserve request ID, idempotency key, pinned provider/source revision, contract version, and expiry. | Do not move request-book validation or durable idempotency into ProtocolAi. |
| **CiAi.CogC** | Consume result events and correlate them to the exact request, provider repository, pinned source SHA, and contract version. Preserve Published/Failed/Partial/Cancelled/Unknown distinctly. | A matching envelope or digest does not prove output correctness or publication truth. |
| **CiAi.CogD** | Keep default-deny policy and exact-match authorization entirely local. A policy decision may be reported as a typed event, carrying policy revision/provenance. | Never interpret protocol identity as a permission or let ProtocolAi own policy. |
| **CiAi.CogE** | Consume lifecycle observations as typed events and project evidence for humans/downstream systems. Preserve contradictory, missing, and out-of-order observations; use correlation/causation and producer provenance. | Do not infer missing success, reclaim, acceptance, or delivery from an envelope. |

### Runtime, hosts, and Experience composition

| Repository | Required use | Explicitly do not |
|---|---|---|
| **FSM_COS** | Keep composition contracts and MicroBundle resolution independent. If runtime assembly/lifecycle is emitted to a remote observer, use ProtocolAi envelope and vocabulary at the host adapter boundary. The host supplies producer identity and provenance. | Do not add ProtocolAi as a mandatory dependency of the composition engine or put transport, authorization, or event persistence inside COS. |
| **FSM_Foundry** | Use typed events for execution lifecycle observations (accepted, started, yielded, completed, failed, cancelled) and command outcomes. Carry correlation to the originating request and causal sequence/revision where required. Foundry owns execution, state transitions, retry policy, cancellation, and effect idempotency. | Do not treat an event contract as an execution plan, authority token, durable queue, or scheduler. |
| **Experiences** | Define stable Experience/manifest/session/bridge event vocabulary using ProtocolBuilder where names cross WebApp/AnyApp/MyVR/ServerApp. Use versioned envelopes for hello/welcome, experience request/state, heartbeat, event, error, and synchronization observations. Keep immutable Experience/artifact identity and protocol version explicit. | Do not make browser input authoritative; no remote shell, arbitrary command execution, or executable code loading. |
| **ServerApp** | Own server-side protocol endpoints and durable messaging. Use ProtocolAi for typed event contracts between server roles and clients; authenticate producer/session before accepting, authorize each effect, and persist inbox/outbox plus dedup/replay outside ProtocolAi. | Do not make ProtocolAi the server, transport, database, identity provider, or policy engine. |
| **WebApp** | Use the Experiences/ServerApp contract at its host boundary. Validate envelope shape/version and session/correlation/sequence at the adapter; send only allowlisted intents. Prefer fixture-only conformance until the envelope API is released. | Do not trust client-side validation or give the browser authority from a protocol symbol. |
| **AnyApp** | Use the same Experience bridge vocabulary as WebApp and MyVR. Validate and correlate incoming requests, resolve immutable artifact identity, then hand off to host authorization and FSM_COS/Foundry. | Do not load or execute browser-supplied payloads; no protocol contract may bypass local trust checks. |
| **MyVR** | Produce/consume Experience session, observer, interaction, synchronization, and heartbeat events through the shared bridge contract. Keep headset input and presentation semantics local; map only stable cross-manifestation events. | Do not add a separate VR-specific protocol vocabulary where the shared Experience contract suffices. |
| **StoryBuilder** | Use ProtocolAi only for export/import or campaign collaboration events that cross a host boundary: timeline/character/place/event identity, revision, and provenance. Keep story schema and fictional continuity domain-owned. Separate implementation facts from fictional assertions in provenance/source metadata. | Do not force a runtime dependency for local authoring; do not treat narrative statements as verified facts. |

### Infrastructure and domain repositories

| Repository | Required use | Explicitly do not |
|---|---|---|
| **MicroBundleDomain** | Keep its manifest/capability contracts transport-neutral. If a MicroBundle declares externally exchanged event names, use an optional ProtocolDefinition reference or documented vocabulary mapping; GrammarAi remains optional too. | Do not add ProtocolAi as a base MicroBundle dependency or move bundle identity/version/dependency semantics into it. |
| **MicroBundleRepository** | Keep artifact identity (bundle ID, version, SHA-256), storage, retrieval, and integrity checks local. If remote repository operations emit events, wrap operation observations in ProtocolAi envelopes at the adapter boundary. | Do not let ProtocolAi own artifact identity, blob paths, persistence, or transport. |
| **MicroBundleIngestor** | Use stable ProtocolAi vocabulary for ingest lifecycle events only if they are consumed outside the process; include artifact hash and source provenance. | Do not use ProtocolAi to parse, inspect, approve, or store artifacts. |
| **FSM_REST** | Remain the HTTP capability/transport substrate. An adapter may map HTTP request/response observations to ProtocolAi event contracts; ProtocolAi is not a REST schema or transport dependency. | Do not merge REST operation descriptors with event envelope semantics. |
| **DataWarehouse** | Store protocol envelopes as opaque/versioned event records when persistence is needed; index correlation, producer, timestamp, and provenance without rewriting the original bytes. Own retention, query, access control, and replay policy. | Do not make the warehouse's internal data model depend on every domain vocabulary or treat storage as validation/authorization. |
| **Ontology** | Remain independent. A consumer may map a ProtocolAi symbol ID into an OntologyIndex, but Ontology owns ontology addresses and relationships. | Do not add a ProtocolAi package dependency; current integer-only boundary is correct. |
| **Profiles** | No direct dependency. A host may use protocol identities to refer to profile-related events, but Profiles owns entity identity, claims, disclosure, and observer-specific representations. | Do not leak private profile data into payloads without host disclosure policy. |
| **Economy** | No direct dependency for internal ledger/domain operations. If cross-service transactions or market events are later exchanged, define Economy-owned payloads and use envelopes for correlation/idempotency/provenance. | Do not make ProtocolAi the ledger, currency model, or transaction authority. |
| **Physics / Gestures / Learning** | No direct dependency for simulation, posture, gesture, or learning-domain models. At a remote session boundary, a host may map selected user intents or simulation observations to shared Experience events. | Do not use protocol vocabularies as a substitute for domain models or high-frequency simulation data paths. |
| **Renderer / Visualize / GUI** | No direct dependency for rendering, scene graphs, frames, or pixels. A host may exchange low-frequency scene/session control events through Experiences. | Do not route per-frame render data through event envelopes. |
| **Benchmarks and test-only repositories** | Use fixture JSON or contract test helpers only when explicitly validating a consumer boundary. | No production dependency. |

## 4. Event contract rules for every adopter

- Validate schema name and version before typed payload interpretation.
- Reject malformed required fields, duplicate JSON properties, invalid provenance, unsupported schema versions, and mismatched event-kind/payload pairs.
- Keep source producer identity distinct from authenticated principal identity. The host authenticates the producer.
- Correlation links related messages; causation identifies the immediate predecessor; idempotency keys identify a requested effect. They are not interchangeable.
- Use full immutable candidate/artifact revisions and content digests where provenance requires them.
- Event timestamps are claims from the producer; host receipt time is separate.
- Preserve unknown/partial/failed/cancelled states. Never upgrade them to success.
- Do not assume delivery, order, durability, exactly-once processing, or replay protection from the envelope.
- Keep domain payloads small and bounded. Large artifacts travel through artifact/repository channels and are referenced by immutable identity/digest.
- Share fixtures and tests before adding a runtime package dependency.

## 5. Adoption order

1. **First conformance wave:** CiAi + Cogs, CommandAi, HeadlessAi, FSM_Foundry. These have concrete request/result/lifecycle boundaries.
2. **Second wave:** Experiences, ServerApp, WebApp, AnyApp, MyVR. Agree one shared Experience bridge vocabulary and verify each host adapter.
3. **Third wave:** MicroBundleRepository, MicroBundleIngestor, StoryBuilder, FSM_REST, DataWarehouse. Add only specific event observations that have a real external consumer.
4. **Remain independent:** Ontology, Profiles, Economy, Physics, Gestures, Learning, Renderer, Visualize, GUI, FSM_COS core, MicroBundleDomain core.

## 6. Release and verification boundary

The current portable EventEnvelope v1 fixture set is an initial baseline, not exhaustive conformance. Each adopter should:
1. pin the fixture revision it consumes;
2. test valid source/command envelopes and negative schema/kind/payload cases;
3. add relevant provenance, duplicate-property, causal-metadata, size, and unknown-extension cases;
4. record repository commit SHA and test-run evidence;
5. avoid referencing an unreleased API or adding a package dependency until the required contract is present in a released package version.

No NuGet publication, merge, deployment, or release is implied by this guide.
