# ProtocolAI Ecosystem Integration

ProtocolAI is deliberately small: it owns deterministic semantic identity, while neighboring packages decide how those identities are carried, structured, rendered, transported, or executed.

This document records the integrations that currently make architectural sense in The Singularity Workshop and, just as importantly, the integrations that should not become direct dependencies merely because they can exchange an integer.

## 1. FSM_UserIO: semantic intent identity

TheSingularityWorkshop.FSM_UserIO is the first concrete consumer boundary to document.

FSM_UserIO currently defines:

~~~csharp
public sealed record SemanticIntent(
    string Name,
    ulong? ProtocolId = null);
~~~

The ProtocolId is an optional deterministic application identity. It can carry the identity of a ProtocolAI vocabulary without making FSM_UserIO responsible for the vocabulary itself.

~~~text
application-owned intent
        |
        | name
        v
FSM_UserIO
SemanticIntent
        |
        | optional ProtocolId
        v
ProtocolAI identity
        |
        v
application-owned meaning
~~~

A host can define a protocol vocabulary and then carry its protocol identity in a SemanticIntent:

~~~csharp
using TheSingularityWorkshop.ProtocolAi;
using TheSingularityWorkshop.FSM_UserIO;

var intents = new ProtocolBuilder(4200, "WorkshopIntents")
    .Define(4201, "openWorkshop", "open.workshop")
    .Define(4202, "resumeExperience", "resume.last.experience")
    .Build();

var intent = new SemanticIntent(
    "open.workshop",
    intents.ProtocolId);
~~~

The important point is ownership:

- ProtocolAI owns deterministic protocol and symbol identities.
- FSM_UserIO owns the semantic interaction boundary.
- The host decides what the intent means operationally.
- FSM_COS can carry the intent through runtime composition without interpreting it.

### Why FSM_UserIO should not depend on ProtocolAI yet

FSM_UserIO intentionally remains a very small, platform-neutral boundary. Making it reference the ProtocolAI package would make every UserIO consumer carry the full ProtocolAI dependency even when a host only needs an application-defined semantic name.

The current ulong? seam therefore has a useful property:

> FSM_UserIO can carry a ProtocolAI identity without owning ProtocolAI.

A future shared contract should be introduced only if repeated consumers need more than the numeric identity. Until then, the primitive boundary is the smaller and more independent contract.

See the FSM_UserIO repository: https://github.com/TrentBest/FSM_UserIO

## 2. GrammarAI: the natural structural companion

GrammarAI is the clearest second integration.

ProtocolAI answers:

> WHAT is this identity?

GrammarAI answers:

> HOW may identities be arranged?

GrammarAI already represents external protocol identities through GrammarProtocolReference:

~~~text
ProtocolAI
    |
    | owns
    v
[protocolId:symbolId]
    |
    | referenced by
    v
GrammarAI
    |
    | describes relationships
    v
structured representation
~~~

For example:

~~~csharp
var greeting = new GrammarBuilder(
        3001,
        "Greeting",
        4001)
    .Rule(
        5001,
        4001,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(4200, 4201)))
    .Build();
~~~

This is already the correct conceptual relationship.

### Do we need a direct GrammarAI -> ProtocolAI dependency?

Not necessarily. The current GrammarProtocolReference is deliberately small and carries only the identity needed by GrammarAI. That keeps GrammarAI capable of referencing any deterministic protocol identity without importing ProtocolAI's complete implementation.

A direct dependency becomes worthwhile only if GrammarAI needs ProtocolAI behavior rather than merely its identity contract.

## 3. FSM_COS: carry, do not interpret

FSM_COS currently consumes FSM_UserIO and carries SemanticIntent from RuntimeManifest into RuntimeAssembly.

That is the correct boundary.

~~~text
FSM_UserIO
    |
    | SemanticIntent
    v
RuntimeManifest
    |
    v
FSM_COS
    |
    | carries intent
    v
RuntimeAssembly
    |
    v
Host
~~~

FSM_COS should not become a direct ProtocolAI or GrammarAI dependency merely because a RuntimeManifest may contain an intent that originated from ProtocolAI.

The composition kernel assembles capabilities. It does not interpret application semantics.

## 4. AI Exchange: strong future consumer

ProtocolAI's AI Exchange boundary is an obvious higher-level consumer.

A complete exchange can carry protocol, grammar, context, request or intent, and response:

~~~text
[PROTOCOL]
    |
[GRAMMAR]
    |
[CONTEXT]
    |
[REQUEST / INTENT]
    |
[RESPONSE]
~~~

Here ProtocolAI supplies deterministic vocabulary identity, GrammarAI supplies structure, and FSM_UserIO can supply semantic interaction intent.

This belongs above the individual foundation packages. It should not turn ProtocolAI itself into the exchange host.

## 5. Experiences and manifestations: strong future consumer

The Experience architecture has several places where deterministic semantic identity is useful:

- Experience-level intents;
- capability identities;
- commands;
- selected objects;
- cross-manifestation events;
- WebApp to AnyApp bridge messages;
- VR interaction semantics;
- session context.

The same Experience can then be encountered through WebApp, AnyApp, or a future VR manifestation while retaining application-owned semantic identities.

The host or Experience domain owns the vocabulary. ProtocolAI supplies the identity mechanism.

## 6. GUI: useful at the host boundary

GUI packages can benefit from ProtocolAI when a GUI is presenting or editing application-owned semantic vocabularies: symbolic identities, application commands, intents, protocol descriptions, or AI-facing semantic exchanges.

GUI should not make ProtocolAI a mandatory foundation dependency just because some controls can eventually display protocol identities. A GUI integration package can consume ProtocolAI where the feature actually needs it.

## 7. FSM_REST: possible, but not yet justified

FSM_REST contains many named operations such as listProducts, getProduct, and createOrder. ProtocolAI could eventually provide deterministic identities for those operations.

However, FSM_REST already has a clear responsibility: describe and execute REST capabilities without owning the remote domain.

There is no demonstrated requirement yet for ProtocolAI identities inside the core REST package.

Therefore: do not add ProtocolAI to FSM_REST yet. A REST provider or higher-level MicroBundle can introduce protocol identities when a real semantic exchange requires them.

## 8. FSM_Serialization: representation adapter, not semantic dependency

FSM_Serialization is a natural representation boundary for ProtocolAI artifacts. A future serialized ProtocolAI definition or AI exchange may need compact binary representation.

That does not mean FSM_Serialization should depend on ProtocolAI.

~~~text
ProtocolAI
    owns semantic meaning

FSM_Serialization
    owns byte representation

calling domain
    owns the mapping between them
~~~

A ProtocolAI-specific serialization adapter can therefore be added later without contaminating the low-level binary boundary.

## 9. MicroBundleDomain and Repository: identity already exists

MicroBundleDomain and MicroBundleRepository already have deterministic artifact identity: BundleId, Version, and ContentHash.

Those identify an artifact. ProtocolAI identifies meaning. They are related but not interchangeable.

Do not add ProtocolAI merely because MicroBundles contain IDs.

A useful future relationship is to associate a published capability with a ProtocolAI semantic identity at the composition or Experience layer when that capability actually participates in semantic exchange.

## Integration map

| Package / boundary | ProtocolAI | GrammarAI | Direct dependency now? |
|---|---|---|---|
| FSM_UserIO | semantic identity via ProtocolId | possible future structure | No |
| GrammarAI | external protocol references | owns structure | No |
| FSM_COS | carries intent | composes capabilities | No |
| AI Exchange | semantic vocabulary | structural envelope | Future host |
| Experiences | intent/capability/object identity | command/context structure | Future host/domain |
| GUI | semantic presentation/editing | structural presentation | Feature-specific |
| FSM_REST | possible operation identities | possible request structure | Not justified |
| FSM_Serialization | serialized representation | serialized representation | No |
| MicroBundleDomain/Repository | possible semantic capability identity | possible structure | Not justified |

## Architectural conclusion

The useful pattern is not that everything depends on ProtocolAI or GrammarAI.

~~~text
                 ProtocolAI
                    WHAT
                     |
          +----------+----------+
          |                     |
     FSM_UserIO             GrammarAI
     intent identity           HOW
          |                     |
          +----------+----------+
                     |
                higher host
                     |
              FSM_COS / Experience
                     |
                manifestation
~~~

ProtocolAI should become the deterministic identity vocabulary that higher layers can opt into.

GrammarAI should become the deterministic structural vocabulary that higher layers can opt into.

FSM_UserIO is already a good demonstration of why this works: it can carry a protocol identity while remaining independent of the package that defines the identity vocabulary.

## Current next steps

1. Keep FSM_UserIO's ProtocolId primitive and dependency-free.
2. Document the ProtocolAI to FSM_UserIO relationship here.
3. Treat GrammarAI as the primary structural companion, without forcing a package dependency yet.
4. Build the first real ProtocolAI + GrammarAI + FSM_UserIO exchange in a higher-level host.
5. Revisit FSM_REST, GUI, Experiences, and MicroBundles only when an actual semantic use case appears.
6. Keep FSM_COS neutral: it carries and composes; it does not become the AI semantics layer.