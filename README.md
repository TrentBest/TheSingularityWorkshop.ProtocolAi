# TheSingularityWorkshop.ProtocolAi

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![NuGet version](https://img.shields.io/nuget/v/TheSingularityWorkshop.ProtocolAi?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/TheSingularityWorkshop.ProtocolAi)
[![NuGet downloads](https://img.shields.io/nuget/dt/TheSingularityWorkshop.ProtocolAi?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/TheSingularityWorkshop.ProtocolAi)
[![Build](https://img.shields.io/github/actions/workflow/status/TrentBest/TheSingularityWorkshop.ProtocolAi/build.yml?branch=master&style=flat-square&logo=github)](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/workflows/build.yml)
[![Code Coverage](https://img.shields.io/codecov/c/github/TrentBest/TheSingularityWorkshop.ProtocolAi?style=flat-square)](https://codecov.io/gh/TrentBest/TheSingularityWorkshop.ProtocolAi)

# ProtocolAI is **for AI**, not AI.

**TheSingularityWorkshop.ProtocolAi** is a small, provider-neutral .NET library for giving application-owned meaning a deterministic address space.

It does **not** contain an AI model, prompt engine, provider SDK, API-key handling, HTTP client, grammar compiler, or command executor.

The boundary is simple:

> **The model proposes. ProtocolAI resolves application-owned identity. The host decides what happens.**

That makes ProtocolAI useful anywhere software needs to move from probabilistic language into deterministic application semantics.

---

## 🔺 03 — See It Work in 60 Seconds

### 1. Install it

Current alpha:

```bash
dotnet add package TheSingularityWorkshop.ProtocolAi --version 0.1.0-alpha.2
```

### 2. Define what your application already knows

Suppose your application owns a vocabulary of people:

```csharp
using TheSingularityWorkshop.ProtocolAi;

var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Define(2003, "saraId", "Sara")
    .Build();
```

Your application owns the meaning. ProtocolAI owns the deterministic identity structure.

### 3. Encode values

```csharp
var payload = people.Encode([
    "Bob",
    "Jane",
    "New Character"
]);
```

Conceptually:

```text
"Bob"           -> [2001]
"Jane"          -> [2002]
"New Character" -> literal
```

Known values become references. Unknown values remain visible instead of being silently assigned an identity.

### 4. Validate and resolve received data

```csharp
people.Validate(payload);

var resolved = people.Resolve(payload);
```

Validation makes sure the payload belongs to the expected protocol and that references identify symbols actually defined by it.

Resolution returns the application-facing values while preserving literals.

### 5. Inspect the vocabulary

```csharp
Console.WriteLine(people.Describe());
```

A definition can describe itself, which is useful for diagnostics, AI-facing context, documentation, and future exchange formats.

**That is the core of ProtocolAI.** You do not need to understand the theory, GrammarAI, FSM_COS, or an AI provider to use this part.

For a practical walkthrough, read **[Consuming ProtocolAI](docs/CONSUMING.md)**.

---

## Why this exists

AI systems are good at producing probable language. Applications often need something different: **authoritative identity**.

If your application already knows that:

```text
Bob = symbol 2001
```

then a model response containing `Bob` should not automatically become application identity merely because the string looks right.

ProtocolAI makes the boundary explicit:

```text
                 PROBABILISTIC
                   MODEL OUTPUT
                        |
                        v
                  candidate value
                        |
                        v
              +-------------------+
              |    ProtocolAI     |
              | deterministic WHAT|
              +---------+---------+
                        |
              +---------+---------+
              |                   |
           known               unknown
              |                   |
              v                   v
        application           literal
          identity               |
              |                   v
              |             host decision
              +---------+---------+
                        |
                        v
                application state
```

ProtocolAI does **not** make the model deterministic. It makes the application's semantic boundary deterministic.

---

## The important distinction: shape vs. meaning

Structured output and schemas are useful, but they solve a different problem.

For example:

```json
{"person":"Bob","action":"inspect"}
```

A schema can establish that `person` is a string.

It cannot, by itself, establish which application-owned identity that string represents.

ProtocolAI addresses **semantic identity**:

```text
shape
  |
  v
structured value
  |
  v
application-owned identity
  |
  v
host policy
  |
  v
application state
```

That is why ProtocolAI is deliberately narrower than an AI framework.

---

## The core API

| Type | What it is for |
|---|---|
| `ProtocolBuilder` | Define an application-owned vocabulary |
| `ProtocolDefinition` | Immutable vocabulary and resolution rules |
| `ProtocolSymbol` | One named value and its integer identity |
| `ProtocolReference` | Protocol + symbol identity together |
| `ProtocolValue` | Either a reference or a literal |
| `ProtocolPayload` | Ordered values belonging to one protocol |

The central flow is:

```text
ProtocolBuilder
      |
      v
ProtocolDefinition
      |
      +--> Encode(...)    -> ProtocolPayload
      +--> Validate(...)
      +--> Resolve(...)   -> application values
      +--> Reference(...) -> ProtocolReference
      +--> Decode(...)    -> application value
      +--> Describe(...)  -> self-description
```

The integer is **not** the meaning. It is the address of meaning owned by the application.

---

## Unknown does not mean invalid

This distinction is fundamental.

If the vocabulary contains:

```text
[2001] Bob
[2002] Jane
```

and a model produces:

```text
"Amelia"
```

ProtocolAI can preserve it as a literal.

The host can then choose to:

- create a new object;
- propose or allocate a new symbol;
- reject the value;
- ask for clarification;
- retain it as transient data.

The alpha intentionally does not decide which policy is correct.

Likewise, an invalid reference such as an undefined symbol ID is different from a legitimate unknown literal and can be rejected during validation.

This gives the host three explicit states:

```text
known + valid       -> deterministic identity
unknown + literal   -> host policy
invalid reference   -> validation failure
```

---

## ProtocolAI + GrammarAI

The packages have intentionally different jobs:

```text
ProtocolAI
    WHAT does this identity mean?

GrammarAI
    HOW may identities be organized?

Host
    WHAT should happen?

FSM_COS
    HOW are capabilities composed into a runtime?
```

ProtocolAI should not absorb GrammarAI, command execution, operating-system routing, GUI behavior, or MicroBundle composition.

See the [GrammarAI repository](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi) for the structural companion.

---

## AI exchange without provider lock-in

ProtocolAI is designed so the same semantic boundary can be used with:

- a human;
- a copied/pasted exchange;
- a hosted LLM;
- a local model;
- a future provider adapter;
- a deterministic test harness.

The provider is not the protocol.

```text
application vocabulary
        |
        v
    ProtocolAI
        |
        v
semantic exchange
        |
   +----+----+
   |         |
clipboard  provider
   |         |
   +----+----+
        |
        v
       LLM
        |
        v
ProtocolAI validation + resolution
        |
        v
host policy
        |
        v
application state
```

A provider adapter can own credentials, endpoints, model selection, and transport without taking ownership of application semantics.

See **[AI Exchange](docs/AI_EXCHANGE.md)** for the architectural boundary.

---

## When should you use it?

ProtocolAI is a good fit when your application:

- already owns a domain vocabulary;
- needs AI or another external participant to refer to that vocabulary;
- wants runtime-defined identities rather than only compile-time enums;
- needs known values and genuinely new values to remain distinguishable;
- needs deterministic validation before acting on model-facing data;
- wants a provider-neutral semantic foundation.

It is probably unnecessary for:

- an ordinary DTO;
- a normal enum;
- a one-off JSON document;
- a conventional schema validator;
- an application that has no stable vocabulary to expose.

Do not add ProtocolAI merely because the word “AI” appears somewhere in a feature.

---

## What ProtocolAI is **not**

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

That restraint is intentional.

**ProtocolAI provides the deterministic WHAT layer.**

---

## Alpha 2 boundary

Current target: **0.1.0-alpha.2**

Alpha.2 establishes:

- integer-backed vocabularies;
- protocol-qualified identity;
- known-value encoding;
- literal preservation;
- deterministic self-description;
- ordered mixed payloads;
- payload validation;
- mixed reference/literal resolution.

Still outside the package:

- LLM inference;
- provider APIs and credentials;
- wire transport;
- dynamic identity allocation;
- protocol negotiation;
- grammar compilation;
- command execution;
- GUI or platform integration.

Those belong to higher layers.

---

## Documentation

Start with the practical material:

- **[Consuming ProtocolAI](docs/CONSUMING.md)** — installation, first vocabulary, encoding, validation, resolution, and AI-host integration.
- **[ProtocolAI Examples](docs/EXAMPLES.md)** — practical domain examples.
- **[AI Exchange](docs/AI_EXCHANGE.md)** — clipboard and provider-neutral exchange design.
- **[ProtocolAI Theory](docs/THEORY.md)** — the deeper semantic and architectural argument.
- **[ProtocolAI Reflection](docs/REFLECTION.md)** — limitations, lifecycle questions, and future directions.
- **[Ecosystem Integration](docs/ECOSYSTEM_INTEGRATION.md)** — boundaries with FSM_UserIO, GrammarAI, FSM_COS, Experiences, GUI, REST, serialization, and Ontology.

**Read the first three if you want to use it. Read the last three if you want to understand where it is going.**

---

## Development

For contributors:

```bash
dotnet restore TheSingularityWorkshop.ProtocolAi.slnx
dotnet build TheSingularityWorkshop.ProtocolAi.slnx --configuration Release
dotnet test TheSingularityWorkshop.ProtocolAi.slnx --configuration Release
dotnet pack TheSingularityWorkshop.ProtocolAi.csproj --configuration Release --output ./artifacts
```

CI builds, tests with Cobertura coverage, reports to Codecov, and produces the NuGet artifact.

**NuGet publication is deliberately disabled by default.** A release requires an explicit Workshop decision to enable publication.

---

## The larger idea

A useful one-sentence description is:

> **ProtocolAI does not make AI deterministic; it makes application-owned meaning addressable and deterministic after AI interaction.**

That distinction is the reason the package can stay small while still being useful to much larger systems.

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
