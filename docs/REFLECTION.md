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

## Immutable definitions and future dynamic registration

There is an important distinction between **an immutable protocol definition** and **a vocabulary that can evolve**.

The alpha chooses immutability because it makes a published definition deterministic:

~~~text
ProtocolBuilder
      |
      v
ProtocolDefinition
      |
      +---- same definition + same value
      |              |
      |              v
      |          same identity
~~~

That is a useful property for reasoning, testing, caching, diagnostics, and later serialization.

It does not mean the surrounding domain is immutable.

A future lifecycle can instead separate the **mutable authority** from the **immutable snapshots** it publishes:

~~~text
                 domain / host authority
                          |
                          v
                 +-------------------+
                 | registration      |
                 | policy + allocator|
                 +---------+---------+
                           |
                    publish snapshot
                           |
                           v
                 +-------------------+
                 | ProtocolDefinition|
                 |    immutable      |
                 +-------------------+
                           |
                    Describe / Encode
                           |
                           v
                    protocol payload
~~~

That gives the architecture a clean place to answer the questions alpha intentionally leaves open:

- Who is allowed to register a new symbol?
- How is an integer identity allocated?
- How does the host reject a proposed registration?
- How is a new definition published?
- How do existing participants learn that the vocabulary changed?
- What does protocol version mean when a vocabulary evolves?
- How are old integer references kept meaningful?
- Can two participants merge compatible vocabulary updates?
- When should a literal become an identity at all?

The likely answer is **not** to make ProtocolDefinition itself mutable.

Instead, a future registration layer can own mutation and publish new immutable definitions or snapshots.

That preserves the strongest property of the alpha while giving the larger system somewhere to evolve.

### Describe() as a future protocol boundary

Describe() is currently a deterministic diagnostic representation.

Conceptually, however, it points toward something larger:

~~~text
immutable definition
       |
       v
   Describe()
       |
       v
self-description
       |
       v
future serialized protocol
       |
       v
host consumes definition
       |
       v
policy
       |
       +---- accept
       +---- reject
       +---- merge
       +---- publish new snapshot
~~~

The important design constraint is that **description is not authority**.

A description can tell a host what a vocabulary claims to contain.

It should not, by itself, grant permission to create identities or mutate the application's domain.

That policy belongs outside the immutable definition.

---

## The next useful boundary

The next implementation step should therefore be considered a lifecycle problem rather than simply "make the dictionary mutable."

A useful future decomposition is:

~~~text
ProtocolDefinition
    = immutable vocabulary snapshot

ProtocolRegistry
    = mutable host-owned authority

ProtocolRegistration
    = proposed identity change

ProtocolAllocator
    = identity allocation policy

ProtocolNegotiation
    = compatibility between snapshots
~~~

Those names are deliberately conceptual at this stage.

The alpha does not promise these types.

The architectural question comes first:

> **How can a changing domain publish deterministic, addressable vocabulary snapshots without allowing probabilistic model output to become authority over identity?**

That is the question the next generation of ProtocolAI should answer.

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
