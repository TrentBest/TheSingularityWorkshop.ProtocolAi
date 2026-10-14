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

### 1. AnyApp — implemented host-owned semantic vocabulary

**Repository:** [TrentBest/AnyApp](https://github.com/TrentBest/AnyApp)

**Source path:** [WorkshopSemanticExchange.cs](https://github.com/TrentBest/AnyApp/blob/master/WorkshopSemanticExchange.cs)  
**Package reference:** [AnyApp.csproj](https://github.com/TrentBest/AnyApp/blob/master/AnyApp.csproj)  
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
**Start here:** [FSM_UserIO README](https://github.com/TrentBest/FSM_UserIO)

FSM_UserIO's SemanticIntent carries a name and optional ProtocolId. The host can attach a ProtocolAi identity without making the lower-level interaction package depend on the ProtocolAi implementation.

~~~csharp
var intent = new SemanticIntent("open.forge", IntentProtocolId);
~~~

**Why this matters:** the interaction boundary carries intent; the domain owns its meaning; ProtocolAi owns the vocabulary identity. Keep the numeric seam until a demonstrated need justifies a shared contract.

### 4. FSM_COS — indirect carrier, not a ProtocolAi consumer

**Repository:** [TrentBest/FSM_COS](https://github.com/TrentBest/FSM_COS)

FSM_COS can carry SemanticIntent from a manifest into a runtime assembly through FSM_UserIO. That is an **indirect boundary**, not a reason to add ProtocolAi or GrammarAi as core dependencies. Composition should assemble; the application host should interpret.

### 5. WebPage — historical / verify before citing as current usage

**Repository:** [TrentBest/WebPage](https://github.com/TrentBest/WebPage)

Older search results referenced ProtocolAi in Workshop composition tests, but the currently inspected master test file did not directly import ProtocolAi. Treat that earlier reference as historical until the active source is rechecked. Do not present WebPage as a confirmed current direct consumer based on stale search results.

### 6. The — legacy local implementation, not the current NuGet API

**Repository:** [TrentBest/The](https://github.com/TrentBest/The)  
**Path:** [Architect/Workshop/Ai/ProtocolAi.cs](https://github.com/TrentBest/The/blob/master/Architect/Workshop/Ai/ProtocolAi.cs)

This older source contains a separate static ProtocolAi vocabulary implementation. Its API and design differ from TheSingularityWorkshop.ProtocolAi (ProtocolBuilder, ProtocolDefinition, ProtocolReference, and payload types).

**Action:** review whether this project is still active and whether it should remain a standalone legacy implementation or migrate deliberately. Do not mechanically replace it or imply it already consumes the NuGet package.

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

## Audit limitations

This map is based on source files and repository search inspected for this pass. GitHub code search indexes default branches and can be stale or incomplete. It is not proof that every private, unindexed, or locally unpushed repository has been examined. Re-run the search and inspect active source before promoting a candidate to confirmed usage.
