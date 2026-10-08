# ProtocolAI C# Developer's Guide

This guide focuses on integrating ProtocolAI into a real .NET application.

## Core types

- `ProtocolBuilder` creates a vocabulary.
- `ProtocolDefinition` is the immutable runtime vocabulary.
- `ProtocolSymbol` is one named value with a stable integer identity.
- `ProtocolReference` identifies a symbol within a protocol.
- `ProtocolValue` represents either a reference or a literal.
- `ProtocolPayload` is an ordered collection owned by one protocol.
- `ProtocolPayloadJson` provides a dependency-free JSON transport representation.

## Design IDs deliberately

Protocol and symbol IDs are application-owned identities. Treat them as persistent identifiers, not array indexes.

Do not recycle an ID to mean something else after consumers have learned its meaning.

## Validate at trust boundaries

A deserialized payload is data. It is not automatically trustworthy.

Use:

```csharp
protocol.Validate(payload);
```

before resolving references from an external source.

Validation catches:

- a payload owned by another protocol;
- references to undefined symbols.

JSON shape validation occurs during deserialization.

## References versus literals

A `ProtocolValue` has exactly one representation:

- reference: `SymbolId`;
- literal: `Literal`.

An unknown string is not an error merely because it is unknown. It becomes a literal.

## JSON is deliberately boring

The alpha.3 representation is intentionally simple:

```json
{"protocolId":1001,"values":[{"symbolId":2001},{"literal":"New Character"}]}
```

It is not an AI-specific format. It can travel through HTTP, WebSocket, a file, a queue, a database, or another application.

## Provider neutrality

ProtocolAI contains no provider SDK, credentials, network client, prompt engine, or model-selection policy.

The same vocabulary can be used when the source is an LLM, a human, a GUI, another program, or deterministic code.

## Exceptions are part of the contract

Invalid construction and invalid external data are rejected explicitly with standard .NET exceptions.

Do not use exception messages as machine-readable protocol codes.

## Testing strategy

At minimum test:

1. vocabulary construction;
2. duplicate ID/name/value rejection;
3. reference encoding;
4. literal preservation;
5. protocol ownership validation;
6. undefined-reference rejection;
7. JSON round-trip;
8. malformed JSON;
9. both/neither JSON value representations.

The package is intentionally small enough that these invariants can be tested directly.
