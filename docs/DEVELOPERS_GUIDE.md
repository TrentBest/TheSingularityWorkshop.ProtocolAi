# ProtocolAI Developer's Guide

ProtocolAI separates **meaning, identity, transport, and behavior**.

That separation is the architecture.

## The four boundaries

```
MEANING
ProtocolDefinition
    |
    v
IDENTITY
ProtocolReference / ProtocolValue
    |
    v
TRANSPORT
ProtocolPayload / ProtocolPayloadJson
    |
    v
BEHAVIOR
host/application policy
```

### Meaning

The application defines what its symbols mean.

### Identity

ProtocolAI gives those meanings stable, protocol-qualified integer addresses.

### Transport

A ProtocolPayload carries references and literals. Alpha 3 provides a provider-neutral JSON representation.

### Behavior

The host decides what a resolved identity actually permits or causes.

ProtocolAI stops before execution.

## Compatibility

A protocol ID identifies a vocabulary namespace.

A symbol ID identifies an entry inside that namespace.

Changing the meaning of an existing identity is a semantic breaking change even if every C# call still compiles.

Prefer adding identities rather than silently redefining them.

The current alpha does not provide protocol negotiation or dynamic registration. Those are future lifecycle concerns.

## Unknown values are useful

An unknown literal is not necessarily a failure.

A host may:

- retain it;
- ask for clarification;
- map it;
- propose registration;
- reject it.

This is particularly useful around probabilistic inputs because “not recognized” is not the same as “malformed.”

## Validation versus authorization

Validation answers:

> Is this payload structurally and semantically valid for the expected vocabulary?

Authorization answers:

> Is this actor allowed to use this valid identity here?

Those questions must remain separate.

## ProtocolAI and GrammarAI

ProtocolAI answers:

> **WHAT does this identity mean?**

GrammarAI answers:

> **HOW may identities be arranged?**

A host may use either package independently.

A higher-level system can combine them without forcing the two foundations into one dependency.

## ProtocolAI and FSM_UserIO

FSM_UserIO can carry semantic intent while remaining independent of the ProtocolAI implementation.

A host can associate an intent with a ProtocolAI protocol identity without making FSM_UserIO responsible for vocabulary resolution.

## ProtocolAI and AI models

An LLM is one possible participant.

A human, GUI, deterministic test, another process, or another agent can use the same semantic boundary.

That is a useful architectural test:

> **Can this protocol remain meaningful if the model disappears?**

It should.

## ProtocolAI and Python/provider ecosystems

ProtocolAI does not require moving the application domain into a provider-specific runtime.

A conventional C# application can retain its domain model and add an AI-facing semantic boundary around it.

The model integration remains an adapter.

## Design invariant

> **ProtocolAI defines what symbols mean without defining how an AI model reasons about them, and without granting those symbols operational authority.**

## Before adding a feature

Ask:

1. Does this feature define application-owned identity?
2. Does it preserve the distinction between known, unknown, and invalid?
3. Does it belong below host policy?
4. Does it remain useful without an AI provider?
5. Does it make the core package larger than necessary?

If the answer to the last question is yes, the feature may belong in a higher-level package.

## Next design frontier

The most important unanswered architectural problem is vocabulary lifecycle:

```
host authority
    |
    v
registration policy
    |
    v
immutable ProtocolDefinition snapshot
    |
    v
transport / exchange
    |
    v
receiving host
```

The alpha intentionally stops at the immutable snapshot.

That keeps the foundation deterministic while leaving composition room for a future registration/negotiation layer.