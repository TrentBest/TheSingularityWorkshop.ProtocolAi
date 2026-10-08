# ProtocolAI Theory\n\n> **The theory is not a claim that AI becomes deterministic. It is a study of where deterministic application ownership can begin.**\n\n## The semantic boundary between probability and identity

ProtocolAI starts with a deceptively small observation:

> A model may be probabilistic, while the application it is talking to already has deterministic knowledge.

If the application already knows that `Bob` is symbol `2001`, then an AI response does not need to become application meaning merely because a model produced a plausible string.

ProtocolAI introduces an explicit semantic boundary:

```text
probabilistic language
        |
        v
candidate value
        |
        v
application-owned vocabulary
        |
   +----+----+
   |         |
 known    unknown
   |         |
   v         v
identity   literal
   |         |
   +----+----+
        |
        v
deterministic application decision
```

The model proposes.

The protocol resolves representation.

The host decides what is allowed to happen.

That separation is the central idea.

---

## Deprobabilization

**Deprobabilization** is a useful name for the architectural pattern explored here:

> Move a piece of meaning from an open-ended probabilistic language space into an application-owned deterministic address space as early as the application can legitimately do so.

This is not a claim that ProtocolAI makes an LLM deterministic.

It does not.

It is also not a claim that integer identifiers inherently improve model reasoning.

Instead, ProtocolAI reduces the amount of semantic interpretation that must remain probabilistic **after the model has produced an answer**.

For a known value:

```text
"Bob"
  |
  v
known vocabulary entry
  |
  v
[2001]
```

For an unknown value:

```text
"Amelia"
  |
  v
not in vocabulary
  |
  v
literal
  |
  v
host decides:
create / register / reject / clarify
```

For an invalid reference:

```text
[9999]
  |
  v
not defined by this protocol
  |
  v
validation failure
```

Those are three different states. A conventional string-only interface can easily blur them together.

ProtocolAI makes them explicit.

---

## Why this may matter for hallucination

A hallucination is not a single technical failure mode. A model can invent a name, confuse two names, emit a stale identifier, or produce something syntactically valid but semantically inappropriate.

ProtocolAI does not prevent the model from generating any of those things.

What it can do is prevent **mere resemblance to a known value from becoming application identity automatically**.

The important boundary is:

```text
MODEL OUTPUT
    |
    v
candidate
    |
    +--> known, valid identity ----> reference
    |
    +--> unknown literal ----------> host decision
    |
    +--> invalid reference --------> reject
```

This changes the failure surface.

Instead of:

> “The model said Bob, therefore this is Bob.”

the architecture can say:

> “The model produced a value. Does this value correspond to an identity owned by this application?”

That is a much narrower question.

So the careful claim is:

> **ProtocolAI may reduce certain classes of semantic hallucination reaching deterministic application state by requiring application-owned identity to be resolved explicitly.**

That is an architectural hypothesis, not a measured hallucination-reduction result. It should be tested empirically with controlled model evaluations.

---

## The probabilistic funnel

The pattern can be viewed as a funnel:

```text
                 many possible strings
                         |
                         v
                 probabilistic model
                         |
                         v
                    candidate
                         |
                         v
              +---------------------+
              |     ProtocolAI      |
              | application-owned   |
              | semantic vocabulary |
              +----------+----------+
                         |
             +-----------+-----------+
             |                       |
          recognized              unrecognized
             |                       |
             v                       v
       deterministic             literal
         identity                   |
             |                       v
             |                host policy
             |                       |
             +-----------+-----------+
                         |
                         v
                  application state
```

The funnel does not run backward through the model.

It runs forward from probability toward deterministic software.

That is why the “ball falls up” metaphor is useful as intuition but dangerous as a literal description. ProtocolAI is not reverse inference. It is deterministic resolution after inference.

---

## Schema is not semantic identity

Structured output can make a response conform to a shape.

For example:

```json
{"person":"Bob","action":"inspect"}
```

A schema can establish that `person` is a string.

It does not establish which application object that string identifies.

ProtocolAI explores a different layer:

```text
shape
  |
  v
structured value
  |
  v
semantic identity
  |
  v
application state
```

This produces a useful division:

- **Schema** describes shape.
- **GrammarAI** describes structural relationships.
- **ProtocolAI** describes application-owned semantic identity.
- **Host policy** decides what an identity permits.
- **Execution** performs the operation.

The layers can cooperate without becoming the same system.

---

## A protocol is a self-describing vocabulary

A ProtocolAI definition has its own identity:

```text
[1001] People
```

and symbols within that identity:

```text
[2001] bobId  = "Bob"
[2002] janeId = "Jane"
```

The protocol provides the namespace.

The symbol provides the address.

The application provides the meaning.

This means ProtocolAI is not a global dictionary. Each application or capability can define the vocabulary it actually owns.

```text
Tool A -> People
Tool B -> Buildings
Tool C -> Materials
Tool D -> Commands
```

The infrastructure supplies the form.

The domain supplies the meaning.

---

## Meaning, identity, and representation

ProtocolAI deliberately separates three concerns:

1. **Meaning** — what a domain value represents.
2. **Identity** — the integer used to address it.
3. **Representation** — how that identity appears in a payload.

Example:

```text
Meaning:       Bob
Symbol name:   bobId
Symbol ID:     2001
Protocol ID:   1001
Payload form:  [2001]
```

The integer is not the meaning.

It is the address of the meaning.

That distinction allows the same semantic identity to participate in later serialization, storage, grammar, comparison, composition, or transport without forcing every layer to own the underlying domain object.

---

## The literal escape hatch

A completely closed vocabulary would make new information awkward.

Suppose the protocol knows:

```text
[2001] Bob
[2002] Jane
```

and the model produces:

```text
"Amelia"
```

ProtocolAI preserves the literal.

```text
[2001] [2002] "Amelia"
```

This is important because **unknown does not mean invalid**.

The host may decide to:

- create a new object;
- register a new symbol;
- reject the value;
- ask for clarification;
- retain it as transient data.

ProtocolAI does not silently choose.

That preserves the application's authority over its own vocabulary.

---

## Determinism is deliberately local

ProtocolAI's deterministic behavior is narrow.

Given:

- a protocol definition;
- an input value;
- the protocol's existing symbols;

the library can deterministically resolve known values and preserve unknown literals.

That does not make the surrounding interaction deterministic.

The model remains probabilistic.

The host remains responsible for policy.

The application remains responsible for execution.

The useful property is therefore not:

> “AI becomes deterministic.”

It is:

> **“The application can choose exactly where probabilistic output stops being allowed to define identity.”**

That is a much more general architectural idea.

---

## Identity can persist across interactions

A prompt is transient.

A protocol identity can persist:

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

The model receives a compact reference.

The protocol therefore creates a stable semantic address space around otherwise transient language interactions.

Whether this improves token usage, latency, model reliability, or reasoning quality is an empirical question.

---

## Relationship to GrammarAI

ProtocolAI answers:

> **WHAT does this identifier mean?**

GrammarAI answers:

> **HOW may these identifiers be organized?**

Together:

```text
probabilistic language
        |
        v
ProtocolAI
    WHAT / identity
        |
        v
GrammarAI
    HOW / structure
        |
        v
host validation
        |
        v
application execution
```

Neither package needs to become an AI provider or execution engine.

That separation is what makes the concepts composable.

---

## Relationship to structured model output

Modern model platforms provide structured outputs and constrained generation.

ProtocolAI does not replace those mechanisms.

It explores a semantic layer that can sit beneath them:

```text
provider constraint
      |
      v
structured representation
      |
      v
ProtocolAI vocabulary
      |
      v
application identity
```

A provider may constrain syntax.

ProtocolAI can constrain semantic identity according to a vocabulary the application owns.

That distinction is the interesting part.

---

## Local protocols, not universal dictionaries

There is no requirement for one universal vocabulary.

A workshop tool might define:

```text
[7100] WorkshopCommands
[7101] inspect
[7102] move
[7103] create
[7104] delete
```

Another capability can define completely different identities.

Protocol identity separates those namespaces.

This becomes increasingly useful as independently authored capabilities are composed.

---

## The security boundary

Integer identity is not authorization.

Knowing that `[7103]` means `create` does not grant permission to create anything.

The host must still apply policy:

```text
model output
    |
    v
ProtocolAI resolution
    |
    v
identity
    |
    v
authorization / policy
    |
 +--+--+
 |     |
allow deny
 |     |
 v     v
execute reject
```

This is another reason the protocol must remain separate from execution.

---

## The larger experiment

The Workshop's longer-term progression is:

```text
human/domain language
        |
        v
probabilistic generation
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
validated protocol
        |
        v
tool / application policy
        |
        v
execution
        |
        v
experience
```

ProtocolAI is the first semantic deprobabilization boundary.

GrammarAI is the structural boundary that follows it.

The goal is not to eliminate natural language.

The goal is to stop asking natural language to remain the authoritative representation after the application already knows something more precise.

---

## Questions the alpha intentionally leaves open

- Who allocates new symbol IDs?
- How are identities persisted across processes?
- When does a literal become a permanent symbol?
- How do definitions evolve without breaking consumers?
- How do two protocol definitions establish compatibility?
- How are vocabularies negotiated?
- Who is authorized to define or invoke symbols?
- Do models reliably use supplied integer vocabularies?
- Does semantic deprobabilization measurably reduce hallucination classes?
- What are the token, latency, and context-window effects?

These are not holes to hide.

They are the experiment.

---

## Architectural invariant

> **ProtocolAI defines what symbols mean without defining how an AI model reasons about them, and without granting those symbols operational authority.**

In one line:

```text
LLM          = probabilistic proposal
ProtocolAI   = deterministic semantic identity
GrammarAI    = deterministic structure
Host         = policy and execution
```

That boundary is the point.