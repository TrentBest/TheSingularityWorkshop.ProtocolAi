<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" />
  </a>
</p>

# ProtocolAi — Domain Integration Decisions

**Audit date:** 2026-11-20 UTC  
**Purpose:** record where ProtocolAi belongs across the Workshop, how each repository should use it, and where a deliberate non-integration is the correct architecture.  
**Status language:** implemented, source-pinned, contract-only, indirect carrier, planned optional bridge, candidate, no adoption, or legacy review.

This is an evidence-based decision record, not a mandate to add a package reference to every repository. Each repository remains the owner of its domain semantics and implementation.

## The adoption rule

Use ProtocolAi when stable, application-owned meaning must cross a repository, process, host, or durable message boundary.

ProtocolAi contributes two related but distinct capabilities:

1. **Vocabulary identity:** application-owned protocol and symbol IDs, immutable definitions, known references, explicit unknown literals, validation, resolution, and structural compatibility reporting.
2. **Interoperable event contracts:** versioned event envelopes and typed payloads with message/correlation/causation/idempotency identity, provenance claims, and causal revision metadata.

A consumer should choose the smallest adoption level that solves its actual boundary:

| Level | Meaning | Runtime dependency? |
|---|---|---|
| Direct package | Uses released ProtocolAi API at a real runtime boundary | Yes |
| Source-pinned | Builds against an exact ProtocolAi source revision for coordinated development | Project/source reference |
| Contract-only | Tests portable JSON fixtures or event compatibility | No |
| Vocabulary mapping | Defines its own stable vocabulary using ProtocolAi | Yes, at the domain edge |
| Indirect carrier | Carries an opaque protocol ID/reference without interpreting it | No |
| Candidate | Plausible future use; no current implementation claim | No |
| No adoption | IDs and semantics are local implementation details | No |

Do not add ProtocolAi merely because a repository uses integers, enums, JSON, events, or AI. Do not make it a transitive prerequisite for unrelated MicroBundles.

## Confirmed and planned integrations

### AnyApp — direct package consumer and host-owned vocabulary

**Status: implemented**

**Evidence**
- [WorkshopSemanticExchange.cs](https://github.com/TrentBest/AnyApp/blob/development/WorkshopSemanticExchange.cs) imports ProtocolAi and GrammarAi, defines the `WorkshopExperienceIntents` vocabulary, resolves `SemanticIntent` values, and checks grammar admission.
- [AnyApp.csproj](https://github.com/TrentBest/AnyApp/blob/development/AnyApp.csproj) uses ProtocolAi and GrammarAi package references in package mode and sibling ProjectReferences in repository-source mode. The package pin observed is ProtocolAi `0.1.0-alpha.2`.
- The host's grammar deliberately knows the `inspect.workshop` symbol but does not admit it. This demonstrates the distinction between known vocabulary and structurally admitted intent.

**How to use the boundary**
- AnyApp owns the intent vocabulary and stable IDs.
- ProtocolAi owns the deterministic identity mapping and resolution.
- GrammarAi owns structural admission.
- FSM_UserIO carries the resulting intent.
- AnyApp/host owns live context, user authorization, and execution.

**Required proof:** keep tests separating protocol resolution, grammar admission, host authorization, and execution. A successful `IsAllowed` result is a structural grammar result, not user permission.

### CiAi — source-pinned semantic-stack integration and event-contract consumer

**Status: source-pinned**

**Evidence**
- The CiAi README records immutable ProtocolAi source pin `13e86c23c8eab6a33f3f124342b54598748e8756` alongside a GrammarAi source pin.
- The same README assigns ProtocolAi stable category/member identity primitives, GrammarAi typed workflow/evidence graph structure, and CiAi the CI vocabulary and workflow semantics.
- ProtocolAi [Issue #13](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/issues/13) tracks real host-side event conformance and replay/delivery proof.
- CiAi [#56](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/56) remains the priority self-development vertical slice; [#58](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/58) is supporting semantic-stack composition.

**How to use the boundary**
- ProtocolAi provides stable shared identities and typed envelope/payload shapes only where the self-observation and handoff path needs them.
- CiAi owns CI vocabulary meaning, exact-run observation, evidence retrieval and verification, diagnostic normalization, verdict, and autonomous-development policy.
- GrammarAi owns graph/grammar structure, not evidence truth.
- CommandAi owns generic proposal representation and outcome shape.
- Foundry/host owns delivery, durable state, approval, and execution.

**Priority guardrail:** only add or extend ProtocolAi contracts when a concrete CiAi #56 handoff gap requires them and an exact-revision conformance test demonstrates it. Do not let broad adoption or semantic-stack polish displace the working self-observation → evidence → proposal → authorized repair → rerun → verification proof.

### GrammarAi — qualified identity seam, not a required dependency

**Status: implemented boundary**

**Evidence:** the GrammarAi README and agent instructions define `GrammarProtocolReference(protocolId, symbolId)` as the external identity seam. GrammarAi's project owns grammar definitions, node kinds, roles/slots, productions, cardinality, composition, and structural validation. Its host-supplied resolver checks external membership and exposure against the request's exact catalog/capability snapshot.

**Decision:** GrammarAi should not need a ProtocolAi runtime dependency merely to store a pair of opaque IDs. It should consume ProtocolAi behavior only if a concrete integration needs more than that pair.

**Required proof:** tests must distinguish unknown protocol, unknown symbol, and not-exposed references. Grammar-definition identity/version and proposal-codec version remain separate from vocabulary identity/version.

### CommandAi — event boundary where needed; proposal semantics stay local

**Status: contract integration**

**Evidence**
- CommandAi's project targets .NET 10 and its README explicitly says it has no runtime dependency on sibling packages.
- [CommandAi #1](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi/issues/1) defines the atomic proposal model, compatibility, and ownership split. It identifies ProtocolAi as the interoperable envelope/event boundary for observer registration, notifications, gate changes, and command outcomes.

**Decision**
- CommandAi owns compact/enriched atomic proposals, typed arguments, address-space exchange, proposal codecs, and command outcome semantics.
- ProtocolAi may carry a command accepted/rejected/completed event at a genuine cross-host boundary, with correlation and idempotency identity.
- The generic ProtocolAi envelope does not replace the CommandAi proposal codec or define command semantics.

**Do not:** add ProtocolAi as a dependency just to encode a local proposal; claim durable idempotency from an idempotency field; authorize or execute a command from envelope validity.

### HeadlessAi — optional semantic bridge, never a core dependency

**Status: planned optional bridge**

**Evidence**
- HeadlessAi's README describes configured HTTP endpoint invocation and explicitly keeps host interpretation and authority outside HeadlessAi.
- [HeadlessAi #2](https://github.com/TrentBest/TheSingularityWorkshop.HeadlessAi/issues/2) specifies an optional in-repository bridge package, tentatively `TheSingularityWorkshop.HeadlessAi.ProtocolAi`, which may reference HeadlessAi, ProtocolAi, and GrammarAi.
- [HeadlessAi #9](https://github.com/TrentBest/TheSingularityWorkshop.HeadlessAi/issues/9) keeps ProtocolAi/GrammarAi optional at the bridge boundary and HeadlessAi core independent.

**Decision**
- Keep HeadlessAi core provider/runtime-neutral and independent of ProtocolAi and GrammarAi.
- Put the beginner-friendly semantic exchange API in the optional bridge project.
- Reuse ProtocolAi JSON payload transport where appropriate; do not invent a competing format.
- Preserve raw provider response separately from validated application meaning.

**Required proof:** manual/clipboard and connected-provider flows should share the same semantic request/response contract; test round trips, partial/invalid responses, unresolved references, and the rule that validation never authorizes execution. When the existing bridge issue is advanced, its old .NET 8 acceptance wording should be reconciled with the Workshop's .NET 10 baseline.

### Hermit — optional semantic identity; generic event use deferred

**Status: target architecture, not an implemented dependency.**

**Evidence:** [Hermit's README](https://github.com/TrentBest/TheSingularityWorkshop.Hermit/blob/development/README.md) lists ProtocolAi as deterministic semantic identity for resolving application-owned concepts. [Hermit #5](https://github.com/TrentBest/TheSingularityWorkshop.Hermit/issues/5) evaluates ProtocolAi for cross-host task lifecycle event contracts.

**Decision**
- ProtocolAi may resolve Hermit-owned capability/task vocabulary at an explicit semantic adapter boundary.
- Hermit owns task identity/meaning, planning, capability resolution, policy, approval, execution, evidence, memory, and audit.
- The deterministic in-process Hermit core must not require ProtocolAi.
- Defer runtime event integration until a real cross-process/durable consumer exists and the event-kind contract is suitable.

**Important v1 limitation:** ProtocolAi's current event envelope has a fixed event-kind set for observer registration, source events, observer evaluation, gate transitions, downstream notifications, and command outcomes. It is not a generic task-lifecycle event bus. Do not encode Hermit task-created, planning, approval, or execution events as unrelated `SourceEvent` or command-outcome kinds.

### Ontology — explicit no-dependency boundary

<badge color="success">Documented no-dependency</badge>

**Evidence:** [Ontology README](https://github.com/TrentBest/TheSingularityWorkshop.Ontology/blob/development/README.md) explicitly states there is no ProtocolAi package reference. Ontology owns concepts, semantic addresses, indexes, and relationships. Its `OntologyIndex` is a protocol-neutral integer primitive. [Ontology #8](https://github.com/TrentBest/TheSingularityWorkshop.Ontology/issues/8) coordinates this separation.

**Decision:** keep Ontology independent. A consuming application may map a ProtocolAi symbol ID into an `OntologyIndex`; Ontology must not need to know where the integer originated. ProtocolAi vocabulary identity and ontology semantic-address/relationship truth remain distinct.

### ServerApp — host boundary candidate, not a generic event consumer yet

**Status: candidate; no ProtocolAi runtime dependency confirmed.**

**Evidence:** [ServerApp README](https://github.com/TrentBest/TheSingularityWorkshop.ServerApp/blob/development/README.md) describes manifest-driven server roles and explicitly assigns server provisioning/hosting to ServerApp, composition to FSM_COS, and execution lifecycle to Foundry. [ServerApp #12](https://github.com/TrentBest/TheSingularityWorkshop.ServerApp/issues/12) asks for a concrete protocol/manifest boundary.

**Decision**
- ProtocolAi may provide stable identities for a genuinely shared server-facing message or operation vocabulary.
- GrammarAi may validate a structured Experience manifest or session request where structural constraints are useful.
- ServerApp retains server-role semantics, identity/context, persistence, trust, policy, authorization, network behavior, and execution.
- Do not make ProtocolAi a dependency of server provisioning or all manifest code by default.

**Important v1 limitation:** arbitrary session, hangout, world, and server-lifecycle messages do not fit ProtocolAi's current fixed event-kind set. Identify one concrete contract and consumer test first; otherwise defer.

## Indirect carriers and composition boundaries

### FSM_UserIO — carry intent, do not own vocabulary

**Status: indirect carrier**

FSM_UserIO's `SemanticIntent` carries a name and optional `ProtocolId`. This is a deliberately small primitive seam. It lets a host carry a ProtocolAi identity without forcing every UserIO consumer to take a dependency on the ProtocolAi implementation.

### FSM_COS — compose and carry, do not interpret

**Status: indirect carrier**

FSM_COS may carry an intent through a RuntimeManifest into a RuntimeAssembly via FSM_UserIO. It should not depend on ProtocolAi or GrammarAi just because a manifest can contain an identity originating from either package. Composition is not semantic interpretation or authorization.

### WebPage — composition-level example

**Status: composition proof**

The ProtocolAi usage map records WebPage's `AiExchangeCompositionBundle` and `WorkshopCompositionTests` as composition-level proof that ProtocolAi and GrammarAi can be assembled through FSM_COS. This is not a claim that the WebPage application project directly references the ProtocolAi NuGet package. Keep Renderer and Visualization as MicroBundles, not ProtocolAi dependencies.

## Candidate integrations and explicit non-applications

| Domain / repository | Decision | Where ProtocolAi could fit | Why it should not be a default dependency |
|---|---|---|---|
| Experiences / WebApp / MyVR | Candidate | Stable intent/capability identities shared across manifestations when the same semantic contract truly crosses hosts | Each Experience/host owns vocabulary and policy; local manifest IDs alone do not justify a dependency |
| FSM_Foundry / host | Event-contract consumer only for matching external boundaries | Consume typed gate/notification/outcome envelopes where the event kinds fit | Foundry owns durable delivery, replay, authorization, lifecycle, and execution; the current ProtocolAi v1 event-kind set is not a universal event bus |
| GUI / GUI.SVG | Optional presentation adapter | Inspect/edit a host-owned vocabulary and display known/literal/invalid states | GUI is presentation; do not make GUI foundations depend on semantic identity |
| Gestures | Conditional candidate | Stable action labels across choreography, role routers, and simulation boundaries; grammar for sequences if needed | Do not replace FSM transitions, IK, timing, Physics, or contact/failure simulation |
| Physics | Conditional candidate | Event/material-operation identity only when serialized or exchanged across independent systems | Keep numeric simulation state strongly typed and deterministic; do not encode every scalar as a symbol |
| FSM_REST | No core adoption | Optional semantic operation identity at an AI-facing adapter or MicroBundle edge | REST core already owns HTTP capability shape and transport concerns |
| FSM_API / FSM_Layer | No adoption | None in the state-machine primitive; a host may attach semantic intent above it | State progression and context lifecycle are lower-level execution primitives, not vocabulary ownership |
| FSM_NeuralNetworks | No model-core adoption | Optional mapping at a model-facing language/agent adapter if stable concepts cross that boundary | NeuralNetworks owns tensors, operations, graph/node/port identities, inference, and execution evidence; do not encode tensor/model internals as ProtocolAi symbols |
| DataWarehouse | No adoption | Store serialized definitions/payloads only through a higher-level adapter if a persistence need is demonstrated | DataWarehouse owns fixed-record memory layout, shelves, slots, and handles; keep storage generic and independent of semantic identity |
| Economy | No core adoption | Optional identities for shared operation vocabulary at an AI/host adapter | Economy owns money-like values, offers, transfer intent, fictional currency, and simulation ledger; do not turn currency values or local commerce models into ProtocolAi identities |
| Learning | Indirect carrier | Course/experience intent may pass through FSM_UserIO when a host needs a shared identity | Learning owns course and teaching semantics; do not assign ProtocolAi IDs to every lesson or learning concept by default |
| Profiles | No adoption by default | Optional mapping for shared entity categories at an application boundary | Profiles owns entity/profile, claims, relationships, disclosure, and observer representation semantics |
| MyVR | No adoption for artifact identity | Optional shared Experience intent at the host boundary | MyVR's BundleId + Version + ContentHash identifies artifacts; it is not a semantic protocol identity |
| Ontology | Explicit no dependency | Consumer-side mapping from ProtocolAi symbol IDs to OntologyIndex when useful | Ontology owns concepts, semantic addresses, indexes, and relationships; it intentionally has no ProtocolAi dependency |
| FSM_Serialization | Adapter only if demanded | Serialize a ProtocolAi definition/payload through a dedicated adapter | Serialization owns bytes; ProtocolAi owns semantic identity |
| MicroBundleDomain / MicroBundleRepository | No adoption by default | Associate a capability with a semantic identity at a higher composition boundary if needed | BundleId, version, and content hash identify artifacts, not domain meaning |
| Renderer / Visualize | No core adoption | Visualize real protocol resolution or grammar-validation state in a separate view | They own rendering/visualization; do not add GUI or renderer dependencies to ProtocolAi |
| The / Architect parallel AI stack | Migration/interop review | Compare the local ProtocolAi implementation and vocabulary against this package before any migration | It is a separate in-repository implementation used by the Architect terminal; do not silently treat it as the NuGet contract or assume identity/API parity |

## ProtocolAi usage pattern

For a domain that genuinely needs a shared vocabulary:

~~~csharp
using TheSingularityWorkshop.ProtocolAi;

var intents = new ProtocolBuilder(4200, "WorkshopExperienceIntents")
    .Define(4201, "openForge", "open.forge")
    .Define(4202, "exploreExperiences", "explore.experiences")
    .Build();

var payload = intents.Encode(["open.forge", "new.intent"]);
intents.Validate(payload);

var resolved = intents.Resolve(payload);
// Known identities resolve through this definition.
// Unknown values remain explicit literals.
~~~

Use a qualified `(ProtocolId, SymbolId)` reference when passing an identity to another contract. Keep the application-owned definition/version available at the receiving host. Do not ask an LLM to mint permanent IDs or infer that an integer is permission.

For actual cross-repository events, use the typed event envelope and payload contracts. Keep these distinctions intact:

- Candidate SHA, event SHA, tested checkout SHA, and workflow-definition SHA are different claims.
- Message identity, correlation, causation, and idempotency are different fields.
- A causal revision is not a delivery guarantee.
- A provenance object is a claim until the receiving host verifies the authoritative source.
- Structural compatibility is not proof that the domain meaning stayed the same.

## Event envelope v1: scope and extension gap

The current `ProtocolEventEnvelope` is a typed, versioned contract for the event kinds currently defined by ProtocolAi: observer registration, source event, observer evaluation, gate transition, downstream notification, and command accepted/rejected/completed outcomes.

It is **not yet a universal application event bus**:
- The event kind is a closed `ProtocolEventKind` enum in schema version 1.
- Several event kinds have specific provenance or causal-revision requirements.
- Typed payload factories enforce the current kind/payload pairing.
- An arbitrary application event must not be mislabeled as a source event, gate transition, or command outcome merely to reuse the envelope.

If Hermit, ServerApp, or another domain demonstrates a real cross-process need for application-owned event kinds, design that extension deliberately (including versioning, unknown-kind behavior, typed payload validation, and backward compatibility) before implementing it. Keep domain-specific task/session/world payload semantics in their owning repositories. Track the design question through ProtocolAi [Issue #13](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/issues/13); do not create duplicate work items.

## Cross-repository acceptance checklist

Before promoting an integration from candidate to confirmed:

- [ ] Name the owning domain and the exact semantic boundary.
- [ ] Identify the exact source file, package/project reference, and source revision.
- [ ] Identify the owning tests and the negative cases.
- [ ] Verify the intended dependency direction and target framework.
- [ ] Test known, unknown, undefined, wrong-protocol, stale-version, and malformed inputs as applicable.
- [ ] Verify the host still owns trust, policy, authorization, durable state, and execution.
- [ ] Update this decision record and the source-navigation usage map.
- [ ] Label examples as verified, source-checked, or conceptual based on actual evidence.

## Coordination queue

The existing repository work items already provide the right coordination points; do not create duplicate issues:

- ProtocolAi [#13](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/issues/13): real event interoperability and host-side delivery/replay proof.
- ProtocolAi [#16](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/issues/16): package/API release-readiness ledger.
- CommandAi [#1](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi/issues/1): atomic command and event boundary.
- HeadlessAi [#2](https://github.com/TrentBest/TheSingularityWorkshop.HeadlessAi/issues/2): optional ProtocolAi/GrammarAi bridge.
- HeadlessAi [#9](https://github.com/TrentBest/TheSingularityWorkshop.HeadlessAi/issues/9): resumable multi-provider exchange and optional semantic bridge.
- CiAi [#56](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/56) and [#58](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/58): preserve CODE RED priority while integrating shared semantic contracts.

This record is the ProtocolAi-side map. Implementations belong in their owning repositories and should be coordinated with those repositories' agent instructions and existing work.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>

<p align="center">
  <a href="https://github.com/TrentBest">GitHub</a> ·
  <a href="https://coderlegion.com/">Coder Legion</a> ·
  <a href="https://www.patreon.com/c/TheSingularityWorkshop">Patreon</a> ·
  <a href="https://www.paypal.com/donate/?hosted_button_id=3Z7263LCQMV9J">PayPal</a>
  <br>
  <a href="https://github.com/TrentBest/FSM_API">FSM_API</a> ·
  <a href="https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization">FSM_Serialization</a> ·
  <a href="https://github.com/TrentBest/WebPage">WebPage</a>
</p>

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>
