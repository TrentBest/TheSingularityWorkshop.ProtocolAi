# ProtocolAI AI Exchange

## Purpose

ProtocolAI already defines the **WHAT** side of an AI interaction: application-owned vocabulary, stable symbol identity, known-value resolution, and literal handling.

The next boundary is the **exchange** between a human, an LLM, and the application.

That exchange should work in two modes:

1. **Clipboard mode** — copy a complete request into an LLM and paste the response back into the application.
2. **Connected mode** — the host supplies an API key and provider adapter and performs the same semantic exchange programmatically.

The important rule is:

> **Clipboard and connected LLM interaction are two transports for the same semantic exchange.**

They should not become two different protocols.

---

## The round trip

The intended flow is:

```text
APPLICATION
    |
    | protocol + grammar + context + request
    v
AI EXCHANGE
    |
    +--------------------------+
    |                          |
    v                          v
CLIPBOARD / PASTE          PROVIDER ADAPTER
    |                          |
    v                          v
    LLM / HUMAN MODEL INTERACTION
    |                          |
    +-------------+------------+
                  |
                  v
          AI EXCHANGE RESPONSE
                  |
                  v
             APPLICATION
                  |
                  v
        ProtocolAI resolution
                  |
                  v
          deterministic state
```

The model is a participant in the exchange.

It is not the owner of the protocol.

---

## The exchange artifact

A future host should be able to render one complete, copyable artifact containing enough information for an LLM to understand the current semantic boundary.

Conceptually:

```text
=== THE SINGULARITY WORKSHOP / AI EXCHANGE ===
version: 1

[PROTOCOL]
<self-describing ProtocolAI vocabulary>

[GRAMMAR]
<self-describing GrammarAI structure>

[CONTEXT]
<host-selected context>

[REQUEST]
<task or interaction>

[RESPONSE]
<model response>

=== END AI EXCHANGE ===
```

The exact wire syntax is intentionally **not frozen by this document**. The important architectural property is that the artifact has explicit semantic sections and can survive a copy/paste round trip without requiring a live connection.

A future machine representation may be compact and serialized. The human-facing representation should remain inspectable.

---

## Context is not authority

An exchange can contain rich context:

- current Experience;
- selected objects;
- available commands;
- protocol descriptions;
- grammar descriptions;
- current state;
- previous turns;
- host capabilities.

That context does not grant the model authority to mutate the application.

The host remains responsible for:

```text
MODEL OUTPUT
    |
    v
parse / validate
    |
    v
ProtocolAI resolution
    |
    v
host policy
    |
    +--> accept
    +--> reject
    +--> ask for clarification
    +--> create / register
    +--> execute
```

The LLM proposes.

The host decides.

ProtocolAI makes owned meaning addressable.

---

## Clipboard mode

Clipboard mode is not a fallback hack. It is a first-class transport.

A developer can:

1. open the Workshop GUI;
2. select an Experience or context;
3. copy the generated exchange;
4. paste it into any compatible LLM;
5. copy the model response;
6. paste it back into the GUI;
7. let the host validate and resolve it.

This is useful when:

- no provider integration exists;
- the user wants to choose the LLM manually;
- an organization prohibits direct API access;
- a model is available only through a web interface;
- a developer is debugging the protocol itself.

The same artifact should be useful for all of those cases.

---

## Connected mode

Connected mode adds a provider adapter without changing the protocol.

Conceptually:

```text
                 AI EXCHANGE
                      |
              provider-neutral
                      |
          +-----------+-----------+
          |                       |
    Clipboard                LLM Adapter
                                  |
                  +---------------+---------------+
                  |               |               |
               Provider A      Provider B      Local Model
```

A provider adapter should own:

- endpoint selection;
- authentication;
- provider-specific request formatting;
- provider-specific response formatting;
- model selection;
- transport errors;
- rate-limit handling.

It should **not** own ProtocolAI vocabulary semantics.

---

## API keys

A user/developer supplied API key belongs to the host or provider adapter layer, not ProtocolAI.

Recommended lifecycle:

```text
user enters key
      |
      v
host-owned credential boundary
      |
      v
provider adapter
      |
      v
LLM request
```

The core semantic packages should never require a particular secret mechanism.

A GUI may eventually offer:

- provider;
- model;
- endpoint;
- API key;
- connection test;
- context budget;
- connected/disconnected state.

The GUI should make the security boundary visible.

Keys should not be:

- embedded in ProtocolAI definitions;
- emitted into exchange artifacts;
- written into prompts;
- committed to source;
- logged as ordinary diagnostic data.

Persistence, if offered, should be an explicit host feature rather than a hidden behavior of the protocol package.

---

## Speed

The Workshop notion of **speed** should mean more than token latency.

A contextual interaction can feel fast when the application does not need to rediscover its world for every turn.

The architecture therefore aims for:

```text
stable context
    +
stable vocabulary
    +
stable grammar
    +
compact references
    +
provider connection
    =
low-friction contextual interaction
```

This is an architectural hypothesis, not a benchmark claim.

The actual latency and token effects should be measured later.

---

## The boundary of this package

ProtocolAI should remain responsible for:

- protocol identity;
- symbol identity;
- vocabulary definition;
- deterministic known-value resolution;
- literal preservation;
- self-description.

It should **not** become responsible for:

- API keys;
- HTTP;
- provider SDKs;
- model selection;
- prompt transport;
- inference;
- GUI;
- execution;
- orchestration.

Those capabilities compose above it.

---

## Relationship to GrammarAI

The exchange becomes substantially more useful when it carries both:

```text
ProtocolAI
    WHAT exists
       |
       v
GrammarAI
    HOW it may be arranged
       |
       v
AI Exchange
    WHAT + HOW + CONTEXT + REQUEST
       |
       v
LLM / human interaction
```

This is the semantic foundation that a later FSM_COS MicroBundle can consume.

---

## Future direction

The likely progression is:

1. define a human-readable exchange envelope;
2. define its machine representation;
3. validate pasted responses;
4. add provider-neutral adapter contracts;
5. add local credential handling in a host;
6. expose contextual sessions;
7. let FSM_COS assemble these capabilities as MicroBundles.

The first five steps should happen without making ProtocolAI an AI framework.



---

## Semantic layering above ProtocolAI

ProtocolAI is intentionally the foundation.

It does not decide what a button does, how commands are assembled, or how applications cooperate. It answers a narrower question:

> **What does this identifier mean?**

A useful mental model is:

```text
ProtocolAI
    WHAT exists
    integer ↔ semantic identity
    string ↔ semantic identity
```

The protocol is the strainer: application-owned meaning is admitted into the semantic system as stable, addressable identities; arbitrary model noise is not allowed to become meaning merely because it resembles a known word.

Higher layers may consume those identities:

```text
ProtocolAI
    ↓
GrammarAI
    ↓
CommandAI
    ↓
OperatingSystemAI
    ↓
AppAI
```

These are **not mandatory execution stages**. A tool only exposes the layers its engineering requires.

ProtocolAI therefore remains below all of them and remains unaware of their execution semantics.

---

## Operational-domain extraction

When a host offers an **Extract to Clipboard** operation, it should not merely copy a natural-language prompt.

It can extract a semantic snapshot of the current operational domain:

- protocol identities;
- grammar definitions;
- available command vocabulary;
- current context;
- host-selected capabilities;
- integer-backed references.

A tool can introduce itself by registering facts such as:

```text
Buttons
  A

Actions
  Click

Relationship
  Click → A
```

The exact command semantics belong above ProtocolAI. ProtocolAI's job is to provide the stable identities that make those semantics addressable.

This keeps the LLM from having to infer that two strings happen to refer to the same application-owned object.
