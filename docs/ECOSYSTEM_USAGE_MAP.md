<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# ProtocolAi Across the Workshop

> **Purpose:** a maintained map from the ProtocolAi API and theory to the real files that consume it, plus nearby integration opportunities that must not become accidental dependencies.

This is a source-navigation document, not a claim that every suggested integration already exists. Each entry is labelled **implemented**, **indirect boundary**, **candidate**, or **legacy review**.

## Reader route

1. Learn the abstraction in the [README](../README.md).
2. Run the [local playground](../README.md#-04-see-it-in-a-minute).
3. Read the [API and usage guides](GUIDES.md).
4. Follow the [mathematical contract](MATHEMATICAL_FOUNDATIONS.md).
5. Use this map to inspect actual consumers and indirect boundaries.
6. Read the evidence-backed [Domain Integration Decisions](DOMAIN_INTEGRATION_DECISIONS.md) for adoption and non-adoption rationale.

## Confirmed usage map

For the domain questions and ordered remaining work, see [Domain Audit and Roadmap](DOMAIN_AUDIT_AND_ROADMAP.md).

### 1. AnyApp — implemented host-owned semantic vocabulary

**Repository:** [TrentBest/AnyApp](https://github.com/TrentBest/AnyApp)

**Source path:** [WorkshopSemanticExchange.cs](https://github.com/TrentBest/AnyApp/blob/development/WorkshopSemanticExchange.cs)  
**Package reference:** [AnyApp.csproj](https://github.com/TrentBest/AnyApp/blob/development/AnyApp.csproj)  
**Test coverage:** [AnyAppRuntimeTests.cs](https://github.com/TrentBest/AnyApp/blob/development/tests/TheSingularityWorkshop.AnyApp.Tests/AnyAppRuntimeTests.cs) includes assertions around resolved protocol/symbol IDs and manifest-carried SemanticIntent.  
**Host overview:** [AnyApp README — Semantic intent exchange](https://github.com/TrentBest/AnyApp#semantic-intent-exchange)

The host defines its own protocol and symbols; GrammarAi defines which symbol references are admitted; FSM_UserIO carries the resulting intent. The foundation packages do not need to depend on the host or on each other.

~~~csharp
public static ProtocolDefinition IntentProtocol { get; } =
    new ProtocolBuilder(IntentProtocolId, "WorkshopExperienceIntents")
        .Define(OpenForgeSymbolId, "openForge", "open.forge")
        .Define(ExploreExperiencesSymbolId, "exploreExperiences", "explore.experiences")
        .Build();

public static SemanticIntent OpenForgeIntent { get; } =
    new("open.forge", IntentProtocolId);
~~~

**Why ProtocolAi belongs here:** open.forge is a host-owned concept with an explicit, stable identity. ProtocolAi defines that vocabulary; it does not decide whether the user is allowed to open Forge or execute the transition.

**Boundary reminder:** IsAllowed in this host sample means structurally admitted by its GrammarAi rules. It is not user authorization.

### 2. GrammarAi — implemented protocol-reference integration

**Repository:** [TrentBest/TheSingularityWorkshop.GrammarAi](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi)  
**Start here:** [GrammarAi README](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi)

GrammarAi references externally owned protocol symbols with GrammarProtocolReference(protocolId, symbolId). It does not need to import ProtocolAi just to carry that pair.

~~~csharp
GrammarSymbol.Terminal(
    new GrammarProtocolReference(IntentProtocolId, OpenForgeSymbolId))
~~~

**Why this matters:** ProtocolAi owns **WHAT** a symbol identifies; GrammarAi owns **HOW** identities can be arranged. The reference is the seam, not a reason to couple both packages.

### 3. FSM_UserIO — implemented primitive carrier boundary

**Repository:** [TrentBest/FSM_UserIO](https://github.com/TrentBest/FSM_UserIO)  
**Start here:** [FSM_UserIO README](https://github.com/TrentBest/FSM_UserIO) · [Domain definition and implementation roadmap](https://github.com/TrentBest/FSM_UserIO/blob/development/docs/IMPLEMENTATION_ROADMAP.md)

FSM_UserIO's SemanticIntent carries a name and optional ProtocolId. The host can attach a ProtocolAi identity without making the lower-level interaction package depend on the ProtocolAi implementation.

~~~csharp
var intent = new SemanticIntent("open.forge", IntentProtocolId);
~~~

**Why this matters:** the interaction boundary carries intent; the domain owns its meaning; ProtocolAi owns the vocabulary identity. Keep the numeric seam until a demonstrated need justifies a shared contract.

### 4. FSM_COS — indirect carrier, not a ProtocolAi consumer

**Repository:** [TrentBest/FSM_COS](https://github.com/TrentBest/FSM_COS)

FSM_COS can carry SemanticIntent from a manifest into a runtime assembly through FSM_UserIO. That is an **indirect boundary**, not a reason to add ProtocolAi or GrammarAi as core dependencies. Composition should assemble; the application host should interpret.

### 5. WebPage — implemented composition/test example

**Repository:** [TrentBest/WebPage](https://github.com/TrentBest/WebPage)  
**Test path:** [SingularityHub.Tests/WorkshopCompositionTests.cs](https://github.com/TrentBest/WebPage/blob/master/SingularityHub.Tests/WorkshopCompositionTests.cs)  
**Related test:** `ExplicitlyComposesAiExchange` asserts the composed AI exchange exposes both Protocol and Grammar, produces exchange text, and provides extract/submit command surfaces. `ExplicitAiCompositionIncludesClosure` checks the protocol and grammar bundles are included in the dependency closure.

WebPage is therefore a confirmed **composition-level example** of ProtocolAi and GrammarAi being assembled through FSM_COS. That does not by itself establish that WebPage directly references the ProtocolAi NuGet package in its application project; the example is a microbundle composition/test boundary, so describe it as composition-level use rather than a direct source dependency.

**Why this matters:** the host can compose the vocabulary and grammar as runtime capabilities without turning FSM_COS into the owner of semantic identity. Keep the ProtocolAi and GrammarAi definitions in their own bundles and preserve the host policy boundary.

### 6. CiAi — source-pinned semantic-stack composition

**Repository:** [TrentBest/TheSingularityWorkshop.CiAi](https://github.com/TrentBest/TheSingularityWorkshop.CiAi)  
**Source evidence:** [CiAi README](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/blob/development/README.md) records ProtocolAi source pin `13e86c23c8eab6a33f3f124342b54598748e8756` and GrammarAi source pin `028b1d1f707e3e0614c680d6fc4a556424e2e02a`.  
**Coordination:** [CiAi #58](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/58).

CiAi's README establishes source-pinned composition: ProtocolAi supplies stable category/member identity primitives; GrammarAi validates the typed workflow/evidence graph; CiAi owns CI vocabulary and workflow semantics. This is stronger than a mere candidate, but it is **not yet proof of a completed runtime event path**. Keep exact-revision conformance, evidence verification, and host delivery/replay proof separate.

### 7. CommandAi — event contract boundary, no sibling runtime dependency

**Repository:** [TrentBest/TheSingularityWorkshop.CommandAi](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi)  
**Source evidence:** [CommandAi project](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi/blob/development/TheSingularityWorkshop.CommandAi.csproj) has no ProtocolAi project/package reference. [CommandAi #1](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi/issues/1) explicitly assigns ProtocolAi the correlated event-envelope boundary.

CommandAi owns atomic proposals, proposal codecs, address-space exchanges, and command outcome meaning. ProtocolAi may carry accepted/rejected/completed outcomes across a real host boundary. Do not add a runtime dependency just to encode local proposals, and do not treat an idempotency field as durable idempotency.

### 8. HeadlessAi — planned optional semantic bridge

**Repository:** [TrentBest/TheSingularityWorkshop.HeadlessAi](https://github.com/TrentBest/TheSingularityWorkshop.HeadlessAi)  
**Coordination:** [HeadlessAi #2](https://github.com/TrentBest/TheSingularityWorkshop.HeadlessAi/issues/2) and [#9](https://github.com/TrentBest/TheSingularityWorkshop.HeadlessAi/issues/9).

HeadlessAi core remains provider/runtime-neutral. Its planned in-repository bridge may reference ProtocolAi and GrammarAi to provide a common semantic exchange path for manual/clipboard and connected-provider use. The bridge should reuse ProtocolAi's current JSON payload/identity contracts and GrammarAi's structural validation; it must not introduce provider transport, credentials, UI, host policy, or authorization into ProtocolAi. This is planned work, not an implemented consumer.

### 9. The / Architect — parallel in-repository implementation

**Repository:** [TrentBest/The](https://github.com/TrentBest/The)  
**Source:** [Architect/Workshop/Ai/ProtocolAi.cs](https://github.com/TrentBest/The/blob/master/Architect/Workshop/Ai/ProtocolAi.cs).

The Architect currently contains a separate local `ProtocolAi` implementation with its own keyword, action, parameter, and surface tables. Its README describes it as part of the terminal AI context stream. This is not the ProtocolAi NuGet package and is not API parity evidence. Treat it as a parallel implementation requiring an explicit migration/interop decision; do not silently assume the IDs or behavior are interchangeable.

## Integration candidates — not current direct dependencies

| Area | Candidate use | Boundary rule |
|---|---|---|
| Experience manifests | Stable identities for experience intents and declared capabilities | The Experience host owns vocabulary and policy |
| WebApp / AnyApp / future VR bridges | Same intent identity across manifestations | Transport the qualified identity; each host validates its own contract |
| GUI / GUI.SVG | Inspect and edit protocol definitions; visualize known, literal, and invalid values | GUI consumes the feature at the integration edge, not as a mandatory core dependency |
| AI Exchange / HeadlessAi | Put vocabulary and references around provider-neutral request/response data | Provider adapters own model, credentials, transport, and provider formatting |
| FSM_REST | Semantic operation IDs at an AI-facing adapter | Do not add ProtocolAi to the REST core without a real use case |
| FSM_Serialization | Serialize protocol definitions or payloads if an actual transport requires it | Serialization owns bytes; ProtocolAi owns semantics |
| MicroBundleDomain / Repository | Bundle or publish a vocabulary as an explicit artifact, if versioning and ownership are defined | A bundle's presence does not grant permission to register or execute symbols |
| Renderer / Visualize | Animate a real semantic state transition in docs or a tool UI | Visualization must distinguish live state from illustrative SVG |

These are **design opportunities**, not a claim that those repositories currently use ProtocolAi. Promote an entry only after checking the active branch, source path, API calls, tests, and dependency direction.

## What the cross-repository examples should consistently show

A good example should expose all four layers:

~~~text
Domain vocabulary
    ↓ define / encode
ProtocolAi identity
    ↓ qualified references
GrammarAi structure (when needed)
    ↓ validated representation
Host policy → authorization → interpreter / execution
~~~

For each real consumer, link to:
- repository and exact source path;
- the minimal relevant code excerpt;
- the test that exercises the behavior, where one exists;
- the ProtocolAi API or theory page that explains the contract;
- an image only where a visual clarifies a transformation or ownership boundary.

## Prioritized adoption opportunities

These candidates are based on repository architecture and concepts visible in inspected source. They are proposals, not a request to add ProtocolAi/GrammarAi references to every project. Add a dependency only where stable cross-boundary meaning or a compositional language is actually needed.

| Priority | Repository / boundary | Proposed use | ProtocolAi contribution | GrammarAi contribution | Guardrail / acceptance test |
|---|---|---|---|---|---|
| **P1** | **HeadlessAi optional bridge** | Compose provider-neutral semantic requests and validated responses for manual and connected-provider flows | Stable vocabulary identities and deterministic payload transport | Optional structural constraints and proposal validation | Keep core HeadlessAi independent; prove the same contract for clipboard and provider adapters without adding provider or host policy to ProtocolAi. |
| **P1** | **AnyApp / WebApp / Experience manifests** | Evolve the current host-owned intent example into a versioned, cross-manifest identity convention where multiple hosts genuinely share intents | Protocol and symbol identity across host boundaries | Grammar for allowed intent shapes or capability combinations | Host owns vocabulary and policy. Structural admission is not authorization. Add compatibility tests before standardizing shared IDs. |
| **P1** | **ProtocolAi + GrammarAi + FSM_COS living example** | Make the existing WebPage composition example a canonical executable demonstration of vocabulary → grammar → host policy | Demonstrate definitions, qualified references, literals, and resolution outcomes | Demonstrate rule construction, valid structure, and invalid structure | Keep FSM_COS a composition boundary; verify docs/tests exercise real APIs and never imply FSM_COS executes AI requests. |
| **P2** | **GUI / GUI.SVG** | Build a small inspector/editor for protocol definitions and grammar rules, with a live representation of valid, literal, and invalid references | Inspect symbol IDs, names, values, protocol ownership, and resolution | Inspect start symbols, rules, terminals, nonterminals, and validation failures | Visualization is a view over real definitions, not the source of truth. Keep GUI optional. |
| **P2** | **Gestures** | Give reusable gesture/action labels stable identities across choreography, role routers, and simulation boundaries | Name stable actions such as board, secure, connect hose, remove cover, signal launch, or abort without hard-coding UI text as identity | Express ordered or alternative action sequences for a choreography, when the current grammar model fits | Do not replace FSM state transitions, IK, physics, timing, or contact/failure simulation. Prototype one choreography and measure whether it reduces coupling. |
| **P2** | **FSM_Physics** | Give cross-system event and material-operation types stable identities when they must be serialized or exchanged between microbundles | Identify event/material concepts independent of display names | Describe a constrained event or command envelope only if the current grammar can represent it cleanly | Physics remains deterministic and strongly typed internally. Do not encode every numeric simulation value as a symbol or add AI dependencies to the physics core. |
| **P2** | **FSM_REST / external adapters** | Expose stable semantic operation IDs at an AI-facing or multi-client API edge | Stable operation and result identities | Optional request-shape constraints | REST core remains independent unless there is a real interoperability requirement. HTTP schema validation and authentication remain separate. |
| **P3** | **FSM_Serialization / MicroBundleDomain / MicroBundleRepository** | Persist or distribute protocol/grammar definitions as versioned artifacts | Identity and compatibility metadata for vocabulary artifacts | Versioned grammar artifact and referenced protocol IDs | First define ownership, version compatibility, migration, and trust rules. Transport layers should not silently register or authorize contents. |
| **P3** | **Renderer / Visualize / documentation** | Visualize semantic resolution and grammar validation as living diagrams | Show known-symbol, literal, and invalid outcomes from actual data | Show production expansion or a validation trace | Use only where it clarifies a real transformation; do not claim an illustrative SVG is live runtime state. |

### Recommended experiment order

1. **Strengthen the existing AnyApp contract**: test protocol resolution, grammar admission, host authorization, and execution as separate concepts. This is the clearest existing end-to-end seam.
2. **Promote WebPage's composition test into the canonical teaching example**: show actual protocol and grammar objects, bundle dependency closure, and host boundary in one living flow.
3. **Prototype HeadlessAi request/response semantics** without provider calls in ProtocolAi or GrammarAi. This is the most direct opportunity to make provider-independent exchange precise.
4. **Prototype one GUI.SVG inspector** against real definition objects before committing to a generalized editor.
5. **Run small, reversible Gestures and Physics spikes** only if stable cross-bundle identifiers solve a demonstrated problem. Do not add package references merely because a domain contains actions or events.

### Cross-repository audit status

- **Confirmed direct package/source-mode references and API use:** AnyApp (`AnyApp.csproj`, `WorkshopSemanticExchange.cs`, runtime tests); package mode pins ProtocolAi alpha.2 and repository-source mode uses a ProjectReference.
- **Confirmed source-pinned composition:** CiAi README pins ProtocolAi and GrammarAi immutable revisions; this is not yet proof of a completed runtime event path.
- **Confirmed composition-level use:** WebPage (`AiExchangeCompositionBundle` and `SingularityHub.Tests/WorkshopCompositionTests.cs`); this is composition proof, not a direct application package reference.
- **Confirmed identity seam, not direct dependency:** GrammarAi uses `GrammarProtocolReference(protocolId, symbolId)`; FSM_UserIO carries `SemanticIntent`; FSM_COS carries intent indirectly.
- **Contract boundary, no runtime dependency:** CommandAi's issue #1 assigns ProtocolAi event envelopes to cross-host event exchange while CommandAi owns proposal semantics.
- **Planned optional bridge:** HeadlessAi #2/#9; HeadlessAi core remains independent.
- **Parallel local implementation:** The/Architect has its own in-repo ProtocolAi implementation and requires an explicit migration/interop decision.
- **Candidates / deliberate non-adoptions:** Gestures, Physics, GUI/GUI.SVG, FSM_REST, FSM_Serialization, MicroBundleDomain/Repository, Renderer/Visualize. Adopt only at a real shared semantic boundary; keep local simulation, presentation, transport, artifact, and execution concerns in their owning repositories.
## Audit limitations

This map is based on source files and repository search inspected for this pass. GitHub code search indexes default branches and can be stale or incomplete. It is not proof that every private, unindexed, or locally unpushed repository has been examined. Re-run the search and inspect active source before promoting a candidate to confirmed usage.

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
