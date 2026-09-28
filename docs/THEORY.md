# ProtocolAI Theory

## The semantic compression boundary

ProtocolAI starts with a simple observation:

A model may need to identify something that the application already knows.

If the application already knows that `Bob` is symbol `2001`, then repeatedly transmitting the entire human-readable value is not the only possible representation.

The architecture can separate:

```text
meaning
  |
  v
symbol
  |
  v
integer identity
```

The integer is not the meaning.

It is the address of the meaning.

---

## A protocol is a self-describing vocabulary

A ProtocolAI definition has its own identity:

```text
[1001] People
```

and symbols within that identity:

```text
[2001] bobId = "Bob"
[2002] janeId = "Jane"
```

The resulting address can be understood as:

```text
protocol [1001]
    |
    +-- symbol [2001] -> Bob
    +-- symbol [2002] -> Jane
```

This separation matters because a symbol ID without its protocol context is incomplete.

The protocol provides the namespace.

The symbol provides the address.

The value provides the meaning.

---

## Meaning, identity, representation

ProtocolAI deliberately separates three concerns:

1. **Meaning** — what the domain value represents.
2. **Identity** — the integer used to address it.
3. **Representation** — how that identity appears in a payload.

Example:

```text
Meaning:        Bob
Symbol name:    bobId
Symbol ID:      2001
Protocol ID:    1001
Payload form:   [2001]
```

This is an architectural pattern found in many systems that use compact identities to refer to richer data. ProtocolAI does not claim those systems are equivalent; it isolates the useful property:

> **A reference can be smaller than the thing it identifies.**

---

## The literal escape hatch

A closed vocabulary would make creation awkward.

Suppose the protocol knows:

```text
[2001] Bob
[2002] Jane
```

and the model needs to create:

```text
"Amelia"
```

ProtocolAI therefore permits a literal.

```text
[2001] [2002] "Amelia"
```

This establishes a semantic boundary:

```text
known
  |
  +--> integer reference

unknown
  |
  +--> literal
        |
        +--> creation
        |
        +--> registration
        |
        +--> future reference
```

The literal is therefore not intended to compete with the vocabulary.

It is how the vocabulary can encounter something new.

---

## Self-definition is the point

The package must not become a global dictionary.

Instead:

```text
Tool A -> defines People
Tool B -> defines Buildings
Tool C -> defines Materials
Tool D -> defines Commands
```

All use the same protocol form.

This follows a larger Workshop principle:

> **Infrastructure provides the form. Domain packages provide the meaning.**

The protocol definition is therefore data about a vocabulary, rather than a vocabulary imposed by the library.

---

## Relationship to structured model output

Modern model platforms increasingly support schema-constrained output. OpenAI's current documentation describes Structured Outputs and constrained generation, including grammar-based constraints in tool scenarios. [OpenAI Structured Outputs](https://developers.openai.com/api/docs/guides/structured-outputs) and [Function Calling](https://developers.openai.com/api/docs/guides/function-calling)

ProtocolAI does not replace those mechanisms.

It explores a semantic layer that can sit beneath them:

```text
schema / grammar
      |
      v
structured representation
      |
      v
ProtocolAI vocabulary
      |
      v
integer references
```

A schema can describe the shape of data.

A grammar can describe legal structure.

ProtocolAI describes **the vocabulary of the things being referenced**.

That gives the Workshop a clean conceptual split:

```text
WHAT
 |
 +-- ProtocolAI

HOW
 |
 +-- GrammarAI

WHAT TO DO
 |
 +-- host / tool / experience
```

---

## Identity can persist across interactions

A prompt is transient.

A protocol identity can be reused:

```text
Prompt A
"Bob"
   |
   v
[2001]
   |
   v
Response
[2001]
   |
   v
Prompt B
[2001]
```

The host retains the semantic mapping.

The model receives the compact reference.

This does not imply that integer references automatically improve model reasoning. It is an architectural hypothesis that should be evaluated empirically.

---

## Local protocols, not one universal vocabulary

There is no need for a universal dictionary.

A workshop tool might define:

```text
[7100] WorkshopCommands
[7101] inspect
[7102] move
[7103] create
[7104] delete
```

Another tool can use completely different IDs.

Protocol identity separates those namespaces.

That property becomes increasingly useful as independently authored capabilities are composed.

---

## Questions the alpha intentionally leaves open

### Symbol allocation
Who assigns a new symbol ID?

### Persistence
How does an identity survive process restarts?

### Registration
When does a literal become a permanent symbol?

### Versioning
How do definitions evolve without breaking consumers?

### Compatibility
How can two protocol definitions determine whether they can interoperate?

### Negotiation
How does one participant request the vocabulary it needs?

### Security
Who is allowed to define, reserve, or invoke a symbol?

### Model behavior
Do models reliably use a supplied integer vocabulary?

These are not implementation omissions to hide.

They are the next architectural questions.

---

## Architectural invariant

The strongest current invariant is:

> **ProtocolAI defines what symbols mean without defining how an AI model reasons about them.**

The package therefore remains a semantic substrate.

It does not own:

- the model;
- the prompt;
- the tokenizer;
- the transport;
- the grammar;
- the tool runtime.

It owns the lexicon boundary.

---

## The larger experiment

The long-term progression is:

```text
human/domain language
        |
        v
self-defining lexicon
        |
        v
integer identity
        |
        v
grammar / structure
        |
        v
protocol
        |
        v
tool execution
        |
        v
experience
```

ProtocolAI is the first semantic compression boundary.

GrammarAI is the next.

The goal is not to eliminate human language.

The goal is to give software a precise address space for meanings it already owns.
