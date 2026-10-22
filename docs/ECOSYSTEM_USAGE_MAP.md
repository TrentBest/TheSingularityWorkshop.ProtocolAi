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
5. Use this map to inspect the actual consumers.

## Confirmed usage map

For the domain questions and ordered remaining work, see [Domain Audit and Roadmap](DOMAIN_AUDIT_AND_ROADMAP.md).

### 1. AnyApp — implemented host-owned semantic vocabulary

**Repository:** [TrentBest/AnyApp](https://github.com/TrentBest/AnyApp)

**Source path:** [WorkshopSemanticExchange.cs](https://github.com/TrentBest/AnyApp/blob/master/WorkshopSemanticExchange.cs)  
**Package reference:** [AnyApp.csproj](https://github.com/TrentBest/AnyApp/blob/master/AnyApp.csproj)  
**Test coverage:** [AnyAppRuntimeTests.cs](https://github.com/TrentBest/AnyApp/blob/master/tests/TheSingularityWorkshop.AnyApp.Tests/AnyAppRuntimeTests.cs) includes assertions around resolved protocol/symbol IDs and manifest-carried SemanticIntent.  
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
| **P1** | **HeadlessAi and its AnyApp example** | Define a provider-neutral request/response envelope for semantic operations and structured results | Stable operation, field, result, and error identities; literals remain literals | Optional schema/sequence constraints for requests and responses | Provider adapter still owns HTTP, credentials, model-specific formatting, retries, and safety policy. Demonstrate round-trip and reject malformed structures without invoking a provider. |
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

- **Confirmed direct package references and API use:** AnyApp (`AnyApp.csproj`, `WorkshopSemanticExchange.cs`, runtime tests).
- **Confirmed composition-level use:** WebPage (`AiExchangeCompositionBundle` and `SingularityHub.Tests/WorkshopCompositionTests.cs`); verify exact source and test paths on the active branch before using this as a release claim.
- **Confirmed carrier boundary, not direct dependency:** FSM_UserIO (`SemanticIntent`); FSM_COS carries intent indirectly and should not acquire semantic-domain dependencies by default.
- **Legacy local implementation:** The (`Architect/Workshop/Ai/ProtocolAi.cs`, `GrammarAi.cs`, `CommandAi.cs`, `TerminalAi.cs`). This is a separate implementation and needs a deliberate migration decision.
- **Not yet confirmed as consumers:** HeadlessAi, Gestures, Physics, GUI/GUI.SVG, FSM_REST, FSM_Serialization, MicroBundleDomain/Repository, Renderer/Visualize. They remain candidates pending direct source and dependency inspection.
## Audit limitations

This map is based on source files and repository search inspected for this pass. GitHub code search indexes default branches and can be stale or incomplete. It is not proof that every private, unindexed, or locally unpushed repository has been examined. Re-run the search and inspect active source before promoting a candidate to confirmed usage.

---

<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://github.com/TrentBest.png?size=200" alt="The Singularity Workshop on GitHub" width="200">
  </a>
</p>

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
