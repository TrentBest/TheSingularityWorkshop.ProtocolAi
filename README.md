<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>


# ✳️ 00 The Singularity Workshop — ProtocolAi

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![NuGet version](https://img.shields.io/nuget/v/TheSingularityWorkshop.ProtocolAi?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/TheSingularityWorkshop.ProtocolAi)
[![NuGet downloads](https://img.shields.io/nuget/dt/TheSingularityWorkshop.ProtocolAi?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/TheSingularityWorkshop.ProtocolAi)
[![Build](https://img.shields.io/github/actions/workflow/status/TrentBest/TheSingularityWorkshop.ProtocolAi/build.yml?branch=master&style=flat-square&logo=github)](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/workflows/build.yml)
[![Code Coverage](https://img.shields.io/codecov/c/github/TrentBest/TheSingularityWorkshop.ProtocolAi?style=flat-square)](https://codecov.io/gh/TrentBest/TheSingularityWorkshop.ProtocolAi)

> **An LLM can propose a program. Your application must own what its instructions mean.**

LLMs are powerful at proposing ideas and structures, but their output is not a stable application contract. A name can drift, an identifier can be invented, and a plausible-looking response can still ask for the wrong thing.

**ProtocolAi gives known concepts application-owned integer addresses.** Unknown values stay visible as literals; undefined references can be rejected. Compose those stable semantic primitives with [GrammarAi](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi), then let your host validate, authorize, and interpret the resulting program.

<p align="center">
  <img src="docs/images/protocol-ai-money-shot.svg" alt="ProtocolAi maps known vocabulary values to application-owned integer identities and keeps unknown values as literals for host policy." width="100%">
</p>

The idea in one line:

**The model proposes. ProtocolAi identifies. GrammarAi structures. Your host decides and executes.**

## 🟦 01 What

**TheSingularityWorkshop.ProtocolAi** is a small, provider-neutral .NET library for defining application-owned vocabularies.

- A recognized value maps to a stable, protocol-qualified integer reference.
- A reference is checked against the vocabulary that owns its meaning.
- An unknown value remains a literal instead of silently becoming a known identity.
- A reference to an undefined symbol—or to the wrong protocol—can be rejected.
- Valid references resolve through the application's definition.

The integer is an **address**, not the meaning itself. The application owns the mapping and must preserve the meaning of an identity once published.

ProtocolAi does not contain a model, provider SDK, prompt engine, grammar compiler, authorization system, or command executor.

## 🟣 02 Why

A language model may return different wording for the same intent. Sometimes it may even return an invented or incorrect identifier. We cannot assume what it will say next.

We *can* control the semantic contract on our side.

Give the model a vocabulary of application-owned identities. When a candidate comes back, resolve known references against the expected definition, keep unknown literals explicit, and reject undefined references. The model remains probabilistic; the application's interpretation boundary becomes deterministic.

This does **not** prove the model understood the user, guarantee it will choose the correct integer, or make an authorized command safe. Those require additional validation, host policy, and testing.

## 🔺 03 How

![Animated ProtocolAi flow: candidate language moves through the application-owned vocabulary; known values become references, unknown values remain literals, and the host decides what happens next.](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.ProtocolAi/development/docs/images/protocol-ai-living-flow.svg)

*Follow the moving signals from language to identity. The animation is illustrative; the encoding, validation, and resolution below are real library operations.*

### Three outcomes, not one vague “AI result”

![Animated comparison of a known value becoming a reference, an unknown value remaining a literal, and an undefined reference failing validation.](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.ProtocolAi/development/docs/images/protocol-ai-three-outcomes.svg)

| Candidate | ProtocolAi outcome | What happens next |
|---|---|---|
| Known value | Qualified integer reference | Validate and resolve using the expected definition |
| Unknown value | Literal | Host policy decides: retain, clarify, register, or reject |
| Undefined or wrong-protocol reference | Validation failure | Do not treat it as an owned identity |

A minimal example:

~~~csharp
using TheSingularityWorkshop.ProtocolAi;

var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Define(2003, "saraId", "Sara")
    .Build();

var payload = people.Encode(["Bob", "Jane", "Amelia"]);
people.Validate(payload);

var resolved = people.Resolve(payload);
// ["Bob", "Jane", "Amelia"]
// Bob and Jane are known references; Amelia remains a literal.
~~~

### From integer primitives to an executable program

ProtocolAi is the identity layer, not the whole execution engine. The larger composition is:

~~~text
LLM proposal
    ↓
parse the returned representation
    ↓
GrammarAi checks structure
    ↓
ProtocolAi checks semantic identities
    ↓
host policy checks permissions and context
    ↓
bounded interpreter performs approved operations
~~~

This is the design direction: the model can propose an integer-addressed program, while your ecosystem retains the definitions, validation rules, permissions, and code that determine what the program actually does. Integer indexing alone is not a safety proof; every stage needs its own contract.

## 🟢 04 See it in a minute

### Play the living documentation

The interactive playground embeds the behavior being explained. It uses the real ProtocolAi C# library, while FSM_API drives the visible state progression.

From the repository root, with the .NET 8 SDK installed:

~~~powershell
dotnet run --project samples/ProtocolAi.Playground/ProtocolAi.Playground.csproj
~~~

The playground lets you:

- compare illustrative candidate responses and see how wording can drift;
- release a candidate through encoding, validation, and resolution;
- compare a known value, an unknown literal, and an intentionally invalid reference;
- step the FSM_API-driven flow one transition at a time;
- inspect the live ProtocolPayload and resolved value;
- explore the conceptual relationship between ProtocolAi and GrammarAi;
- try the size-aware SVG Hello World at `/hello`.

The candidate responses are labelled fixtures—not live model output or measured probabilities. The SVGs are living documentation, not a claim that a production GPU renderer is executing.

## 🟪 05 The math and the next layer

The guarantees are deliberately narrow and useful: for a fixed, unambiguous vocabulary, encoding is deterministic; a reference passes validation only if its protocol and symbol are defined; and resolution uses the application-owned definition.

Read [Mathematical Foundations](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/MATHEMATICAL_FOUNDATIONS.md) for definitions, proofs, limits, and a test plan. Read [ProtocolAi Theory](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/THEORY.md) for the broader argument and [ProtocolAi Reflection](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/REFLECTION.md) for open questions.

- [Gallery](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/GALLERY.md) — the visual tour, artifact classifications, and links from each diagram to executable evidence.
- [Living Documentation](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/LIVING_DOCUMENTATION.md) — the animated SVGs, interactive playground, accessibility, and the distinction between illustration and executable behavior.
- [Release Readiness](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/RELEASE_READINESS.md) — current alpha status and evidence required before a stable 1.0 release.

- [Consuming ProtocolAi](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/CONSUMING.md) — installation and first integration.
- [ProtocolAi Examples](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/EXAMPLES.md) — practical domain examples.
- [AI Exchange](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/AI_EXCHANGE.md) — provider-neutral exchange boundaries.
- [Ecosystem Integration](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/ECOSYSTEM_INTEGRATION.md) — boundaries with FSM_UserIO, GrammarAi, FSM_COS, GUI, serialization, and ontology.
- [Semantic Payload JSON](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/PROTOCOL_PAYLOAD_JSON.md) — dependency-free wire representation and transport limits.
- [Guides for every starting point](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/GUIDES.md) — plain-language, beginner, C# integration, and developer guides.
- [NuGet package](https://www.nuget.org/packages/TheSingularityWorkshop.ProtocolAi) · [Source](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi) · [Issues](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/issues)

### Build, test, and pack

~~~powershell
dotnet restore TheSingularityWorkshop.ProtocolAi.slnx
dotnet build TheSingularityWorkshop.ProtocolAi.slnx --configuration Release
dotnet test TheSingularityWorkshop.ProtocolAi.slnx --configuration Release
dotnet pack TheSingularityWorkshop.ProtocolAi.csproj --configuration Release --output ./artifacts
~~~

NuGet publication remains disabled by default and requires an explicit Workshop decision.

---

<p align="center">
  <a href="https://github.com/TrentBest/FSM_API">
    <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" height="160">
  </a>
</p>

<p align="center">
  <a href="https://github.com/TrentBest/WebPage" aria-label="Visit The Singularity Workshop"><img src="https://raw.githubusercontent.com/TrentBest/WebPage/development/TheSingularityWorkshop/wwwroot/Images/TheSingularityWorkshopLivingAvatar.svg" alt="The Singularity Workshop living avatar — a digital vortex of information converging on its central eye" width="112" /></a><br />

  <em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br>
  <strong>Because state shouldn't be a mess.</strong>



## Find ProtocolAi in the Workshop


For the full domain gap analysis and the order of remaining work, see [Domain Audit and Roadmap](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/DOMAIN_AUDIT_AND_ROADMAP.md).
The [cross-repository usage map](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/blob/development/docs/ECOSYSTEM_USAGE_MAP.md) links to real consumer files, includes focused code excerpts, and distinguishes implemented integrations from candidates and legacy code. Start with [AnyApp's host-owned semantic exchange](https://github.com/TrentBest/AnyApp/blob/master/WorkshopSemanticExchange.cs) to see ProtocolAi paired with GrammarAi and FSM_UserIO without reversing dependency direction.

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
