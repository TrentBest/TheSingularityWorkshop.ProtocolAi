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

<badge color="success">Implemented</badge>

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

<badge color="success">Source-pinned</badge>

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

<badge color="success">Implemented boundary</badge>

**Evidence:** the GrammarAi README and agent instructions define `GrammarProtocolReference(protocolId, symbolId)` as the external identity seam. GrammarAi's project owns grammar definitions, node kinds, roles/slots, productions, cardinality, composition, and structural validation. Its host-supplied resolver checks external membership and exposure against the request's exact catalog/capability snapshot.

**Decision:** GrammarAi should not need a ProtocolAi runtime dependency merely to store a pair of opaque IDs. It should consume ProtocolAi behavior only if a concrete integration needs more than that pair.

**Required proof:** tests must distinguish unknown protocol, unknown symbol, and not-exposed references. Grammar-definition identity/version and proposal-codec version remain separate from vocabulary identity/version.

### CommandAi — event boundary where needed; proposal semantics stay local

<badge color="info">Contract integration</badge>

**Evidence**
- CommandAi's project targets .NET 10 and its README explicitly says it has no runtime dependency on sibling packages.
- [CommandAi #1](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi/issues/1) defines the atomic proposal model, compatibility, and ownership split. It identifies ProtocolAi as the interoperable envelope/event boundary for observer registration, notifications, gate changes, and command outcomes.

**Decision**
- CommandAi owns compact/enriched atomic proposals, typed arguments, address-space exchange, proposal codecs, and command outcome semantics.
- ProtocolAi may carry a command accepted/rejected/completed event at a genuine cross-host boundary, with correlation and idempotency identity.
- The generic ProtocolAi envelope does not replace the CommandAi proposal codec or define command semantics.

**Do not:** add ProtocolAi as a dependency just to encode a local proposal; claim durable idempotency from an idempotency field; authorize or execute a command from envelope validity.

### HeadlessAi — optional semantic bridge, never a core dependency

<badge color="warning">Planned optional bridge</badge>

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

## Indirect carriers and composition boundaries

### FSM_UserIO — carry intent, do not own vocabulary

<badge color="success">Indirect carrier</badge>

FSM_UserIO's `SemanticIntent` carries a name and optional `ProtocolId`. This is a deliberately small primitive seam. It lets a host carry a ProtocolAi identity without forcing every UserIO consumer to take a dependency on the ProtocolAi implementation.

### FSM_COS — compose and carry, do not interpret

<badge color="success">Indirect carrier</badge>

FSM_COS may carry an intent through a RuntimeManifest into a RuntimeAssembly via FSM_UserIO. It should not depend on ProtocolAi or GrammarAi just because a manifest can contain an identity originating from either package. Composition is not semantic interpretation or authorization.

### WebPage — composition-level example

<badge color="success">Composition proof</badge>

The ProtocolAi usage map records WebPage's `AiExchangeCompositionBundle` and `WorkshopCompositionTests` as composition-level proof that ProtocolAi and GrammarAi can be assembled through FSM_COS. This is not a claim that the WebPage application project directly references the ProtocolAi NuGet package. Keep Renderer and Visualization as MicroBundles, not ProtocolAi dependencies.

## Candidate integrations and explicit non-applications

| Domain / repository | Decision | Where ProtocolAi could fit | Why it should not be a default dependency |
|---|---|---|---|
| Experiences / WebApp / MyVR | Candidate | Stable intent/capability identities shared across manifestations when the same semantic contract truly crosses hosts | Each Experience/host owns vocabulary and policy; local manifest IDs alone do not justify a dependency |
| FSM_Foundry / host | Event-contract consumer at the boundary | Consume typed gate/notification/outcome envelopes and preserve correlation/provenance | Foundry owns durable delivery, replay, authorization, lifecycle, and execution; ProtocolAi does not |
| GUI / GUI.SVG | Optional presentation adapter | Inspect/edit a host-owned vocabulary and display known/literal/invalid states | GUI is presentation; do not make GUI foundations depend on semantic identity |
| Gestures | Conditional candidate | Stable action labels across choreography, role routers, and simulation boundaries; grammar for sequences if needed | Do not replace FSM transitions, IK, timing, Physics, or contact/failure simulation |
| Physics | Conditional candidate | Event/material-operation identity only when serialized or exchanged across independent systems | Keep numeric simulation state strongly typed and deterministic; do not encode every scalar as a symbol |
| FSM_REST | No core adoption | Optional semantic operation identity at an AI-facing adapter or MicroBundle edge | REST core already owns HTTP capability shape and transport concerns |
| FSM_Serialization | Adapter only if demanded | Serialize a ProtocolAi definition/payload through a dedicated adapter | Serialization owns bytes; ProtocolAi owns semantic identity |
| MicroBundleDomain / MicroBundleRepository | No adoption by default | Associate a capability with a semantic identity at a higher composition boundary if needed | BundleId, version, and content hash identify artifacts, not domain meaning |
| Renderer / Visualize | No core adoption | Visualize real protocol resolution or grammar-validation state in a separate view | They own rendering/visualization; do not add GUI or renderer dependencies to ProtocolAi |
| The / Architect legacy AI stack | Legacy review | Compare the local ProtocolAi implementation and vocabulary against this package before any migration | It is a separate implementation; do not silently treat it as the NuGet contract or create two competing identity authorities |

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
