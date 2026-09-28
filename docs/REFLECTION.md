# ProtocolAI Reflection

## Why the package is deliberately small

ProtocolAI is an alpha experiment.

It would be easy to turn it into an AI integration framework containing model clients, prompt templates, token accounting, tool calling, serialization, grammars, transport, memory, orchestration, and provider-specific behavior.

That would make the package larger while making its central idea harder to see.

The current implementation instead asks one question:

> **Can a tool define its own vocabulary and represent known values by stable integer identities?**

Everything else can compose around that boundary.

---

## What the current implementation demonstrates

The current tests establish that a tool can:

1. define a protocol;
2. define named symbols;
3. reject duplicate identities;
4. encode known values as integer references;
5. preserve unknown values as literals;
6. decode integer references;
7. emit a deterministic self-description.

That is enough to demonstrate the lexicon boundary.

It is not yet a complete AI protocol.

---

## The missing lifecycle

The likely semantic lifecycle is:

```text
DEFINE
  |
  v
DESCRIBE
  |
  v
PROMPT
  |
  v
REFERENCE
  |
  +---- known --> integer
  |
  +---- unknown -> literal
                    |
                    v
                  CREATE
                    |
                    v
                 REGISTER
                    |
                    v
              future reference
```

The missing pieces are therefore not arbitrary features.

They are the next steps in the lifecycle.

---

## Why LLM ownership stays outside

The model is a participant.

It should not become the package boundary.

That keeps ProtocolAI usable by:

- hosted models;
- local models;
- future model architectures;
- deterministic tools;
- non-LLM agents;
- test harnesses;
- generated interfaces.

A protocol definition can exist even when no model is present.

That is desirable because the vocabulary belongs to the application, not to the inference provider.

---

## Relationship to GrammarAI

ProtocolAI owns the lexicon.

GrammarAI owns structure.

```text
ProtocolAI
"What exists?"

GrammarAI
"How can it be arranged?"

Host
"What should happen?"
```

This separation gives each package a crisp architectural boundary.

---

## Alpha assessment

The current implementation is foundational rather than complete.

Its strongest property is the boundary.

Its largest unanswered question is lifecycle:

> **How does a self-defining vocabulary become a durable protocol between independently evolving participants?**

That should guide the next design work.

---

## Workshop principle

The package should continue to follow the Workshop pattern:

> **Keep the foundation small. Let composition create the complexity.**

ProtocolAI should provide the form.

Concrete AI applications should provide the meaning, policy, model, and execution around it.
