# TheSingularityWorkshop.ProtocolAi

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![NuGet version](https://img.shields.io/nuget/v/TheSingularityWorkshop.ProtocolAi?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/TheSingularityWorkshop.ProtocolAi)
[![NuGet downloads](https://img.shields.io/nuget/dt/TheSingularityWorkshop.ProtocolAi?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/TheSingularityWorkshop.ProtocolAi)
[![Build](https://img.shields.io/github/actions/workflow/status/TrentBest/TheSingularityWorkshop.ProtocolAi/build.yml?branch=master&style=flat-square&logo=github)](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/workflows/build.yml)
[![Code Coverage](https://img.shields.io/codecov/c/github/TrentBest/TheSingularityWorkshop.ProtocolAi?style=flat-square)](https://codecov.io/gh/TrentBest/TheSingularityWorkshop.ProtocolAi)

# ProtocolAI is **for AI**, not AI.

**TheSingularityWorkshop.ProtocolAi** is a small, provider-neutral .NET library for giving application-owned meaning a deterministic address space.

There is **no AI model inside this package**: no inference engine, chatbot, tokenizer, prompt runner, provider SDK, API-key handling, or network transport.

The **AI** in the name tells you what the infrastructure is engineered *for*: software that exchanges meaning with an LLM of your choice.

> **Your LLM supplies probabilities. Your application owns meaning. ProtocolAI helps move from the first to the second.**

## The mental model

Think of the classic ball-drop game: a ball enters at the top, hits pegs, and probability changes where it lands. AI interaction commonly works in that direction — possibilities fan out.

ProtocolAI is the opposite side of that boundary:

~~~text
             PROBABILISTIC
                LLM OUTPUT
                    |
                    v
              +-----------+
              | ProtocolAI|
              |   WHAT    |
              +-----+-----+
                    |
             deterministic
               resolution
                    |
                    v
          APPLICATION IDENTITY
                    |
                    v
             APPLICATION STATE
~~~

“**The ball falls up**” is the mental model, not a claim that ProtocolAI performs reverse inference. The actual operation is deterministic: given an application-owned vocabulary and an input value, known values resolve to their defined integer identity; unknown values remain literals.

## Why this boundary exists

Schemas and structured outputs solve **shape**. ProtocolAI addresses **semantic identity**.

For example:

~~~json
{"person":"Bob","action":"inspect"}
~~~

A schema can tell the host that person is a string. It does not tell the application which Bob that string identifies.

ProtocolAI lets the application define that vocabulary:

~~~text
[1001] People
  [2001] bobId  = "Bob"
  [2002] janeId = "Jane"
  [2003] saraId = "Sara"
~~~

Now the application has an address space:

~~~text
"Bob"    -> [2001]
"Jane"   -> [2002]
"Amelia" -> "Amelia"
~~~

Known values become references. Unknown values stay literal so the **host** can decide whether to create, register, reject, authorize, or otherwise handle them.

## 60-second example

Run the repository example:

~~~bash
dotnet run --project examples/ProtocolAi.QuickStart/ProtocolAi.QuickStart.csproj
~~~

Or install the package:

~~~bash
dotnet add package TheSingularityWorkshop.ProtocolAi --version 0.1.0-alpha.2
~~~

Then:

~~~csharp
using TheSingularityWorkshop.ProtocolAi;

var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Define(2003, "saraId", "Sara")
    .Build();

var payload = people.Encode(["Bob", "Jane", "New Character"]);

Console.WriteLine(payload);
// [2001] [2002] New Character
~~~

Inspect the vocabulary:

~~~csharp
Console.WriteLine(people.Describe());
~~~

Resolve a mixed payload:

~~~csharp
var resolved = people.Resolve(payload);
// ["Bob", "Jane", "New Character"]
~~~

Validate incoming data before consuming it:

~~~csharp
people.Validate(payload);
~~~

A payload is scoped to its protocol, and every reference must target a symbol owned by that protocol.

## What alpha.2 adds

Alpha.2 makes the boundary useful on the **receive** side of an eventual copy/paste or connected AI exchange:

- **Payload validation** — reject payloads from another protocol or references to undefined symbols.
- **Mixed-value resolution** — resolve references while preserving literals in their original order.
- **Deterministic host boundary** — validation happens before the application consumes model-facing data.
- **Provider neutrality remains intact** — API keys, HTTP, model selection, and provider SDKs remain outside ProtocolAI.

The package still does **not** implement an LLM client, exchange transport, grammar compiler, or command execution engine.

## Core API

| Type | Responsibility |
|---|---|
| ProtocolBuilder | Define a vocabulary before publishing it |
| ProtocolDefinition | Own the immutable vocabulary and resolution rules |
| ProtocolSymbol | Define one named domain value and integer identity |
| ProtocolReference | Carry protocol + symbol identity together |
| ProtocolValue | Represent either a reference or a literal |
| ProtocolPayload | Carry ordered values under one protocol ID |

The central flow is:

~~~text
ProtocolBuilder
      |
      v
ProtocolDefinition
      |
      +--> Encode(...)   --> ProtocolPayload
      +--> Validate(...)
      +--> Resolve(...)   --> domain values
      +--> Reference(...) --> ProtocolReference
      +--> Decode(...)    --> domain value
      +--> Describe(...)  --> self-description
~~~

## The ownership rule

ProtocolAI does **not** own your domain objects. It owns their **protocol identities**.

~~~text
application object
      |
      | application-owned meaning
      v
Research Laboratory
      |
      | ProtocolAI identity
      v
    [7202]
~~~

The application remains responsible for persistence, authorization, creation, execution, and whatever the identity ultimately controls.

## ProtocolAI + GrammarAI

ProtocolAI answers:

> **WHAT does this identifier mean?**

GrammarAI answers:

> **HOW may these identifiers be organized?**

The packages are deliberately separate:

~~~text
LLM / human interaction
          |
          v
       GrammarAI       HOW
          |
          v
      ProtocolAI       WHAT
          |
          v
 application-owned meaning
~~~

Higher layers can consume those identities later. Command execution, operating-system routing, application orchestration, GUI selection, and MicroBundle composition do not belong in this package.

## Clipboard and connected AI exchange

The intended host experience is one semantic artifact with two transports:

~~~text
                 semantic exchange
                       |
              +--------+--------+
              |                 |
          clipboard          provider
              |                 |
              v                 v
             LLM / human interaction
                       |
                       v
                 ProtocolAI
                       |
                 validate + resolve
                       |
                       v
                application state
~~~

Clipboard mode is first-class: a developer can copy a semantic snapshot into an LLM of their choice and paste the response back.

Connected mode can later add a provider adapter. The adapter owns credentials, endpoints, model selection, and transport. ProtocolAI does not.

See **[AI Exchange](docs/AI_EXCHANGE.md)** for the exchange boundary and future host contract.

## Deprobabilization in one sentence

**ProtocolAI does not make the model deterministic; it makes the application's semantic boundary deterministic.**

When a model produces a value, the application can distinguish a known identity from a genuinely new literal and from an invalid reference. That makes the transition from probabilistic language to application state explicit rather than implicit.

See **[ProtocolAI Theory](docs/THEORY.md)** for the deeper argument, including the deliberately cautious hypothesis about hallucination reduction.

## What ProtocolAI is not

ProtocolAI is not:

- an LLM;
- an AI provider;
- a prompt framework;
- a schema validator;
- a JSON replacement;
- a database;
- a grammar compiler;
- a command executor;
- a GUI framework;
- a MicroBundle host.

That restraint is intentional. The package is the deterministic semantic layer **for AI**.

## When to use it

ProtocolAI is useful when your application:

- already owns a domain vocabulary;
- needs AI to refer to that vocabulary;
- wants runtime-defined identities rather than only compile-time enums;
- needs known and genuinely new values to remain distinguishable;
- wants deterministic validation before acting on model-facing data;
- wants a provider-neutral semantic foundation.

It is probably unnecessary for a conventional DTO, enum, JSON serializer, schema validator, or one-off prompt.

## Release boundary

**Current target: 0.1.0-alpha.2**

Alpha.2 establishes:

- self-defining integer-backed vocabularies;
- protocol-qualified identity;
- known-value encoding;
- literal preservation;
- deterministic descriptions;
- ordered mixed payloads;
- payload validation;
- mixed reference/literal resolution.

Still outside the package:

- LLM inference;
- provider APIs and credentials;
- wire serialization;
- dynamic identity allocation;
- protocol negotiation;
- grammar compilation;
- command execution;
- GUI or platform integration.

Those belong to higher layers.

## Documentation

- **[Consuming ProtocolAI](docs/CONSUMING.md)** — practical installation and use
- **[ProtocolAI Examples](docs/EXAMPLES.md)** — patterns
- **[ProtocolAI Theory](docs/THEORY.md)** — architectural thesis
- **[ProtocolAI Reflection](docs/REFLECTION.md)** — limitations and open questions
- **[AI Exchange](docs/AI_EXCHANGE.md)** — clipboard/connected exchange boundary
- **[GrammarAI](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi)** — structural HOW layer

## Development

~~~bash
dotnet restore TheSingularityWorkshop.ProtocolAi.slnx
dotnet build TheSingularityWorkshop.ProtocolAi.slnx --configuration Release
dotnet test TheSingularityWorkshop.ProtocolAi.slnx --configuration Release
dotnet pack TheSingularityWorkshop.ProtocolAi.csproj --configuration Release --output ./artifacts
~~~

The public workflow builds, tests with coverage, packs the NuGet artifact, and publishes through NuGet Trusted Publishing on master or manual dispatch.

## License

MIT. See [LICENSE.txt](LICENSE.txt).

---

<p align="center">
  <a href="https://github.com/TrentBest/FSM_API">
    <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" height="200">
  </a>
</p>

<p align="center">
  <em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br>
  <strong>Because state shouldn't be a mess.</strong>
</p>