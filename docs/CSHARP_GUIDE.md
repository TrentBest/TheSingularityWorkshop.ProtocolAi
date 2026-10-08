# ProtocolAI C# Developer's Guide

This guide focuses on integrating ProtocolAI into a real .NET application.

## API map

| Type | Role |
|---|---|
| ProtocolBuilder | Constructs an application-owned vocabulary |
| ProtocolDefinition | Immutable vocabulary and resolution authority |
| ProtocolSymbol | Named symbol with stable integer identity |
| ProtocolReference | Protocol-qualified symbol identity |
| ProtocolValue | Exactly one reference or literal |
| ProtocolPayload | Ordered values owned by one protocol |
| ProtocolPayloadJson | Provider-neutral JSON transport |

The implementation is intentionally small. That makes its invariants worth understanding directly.

## Construct definitions deliberately

Protocol and symbol IDs are **identities**, not array positions.

Do not casually recycle an ID to mean something else after a consumer has learned its meaning.

Prefer:

```
old symbol -> remains meaningful
new concept -> receives a new identity
```

over silently changing the meaning of an existing identity.

## References and literals

A ProtocolValue represents exactly one of:

```
reference
    SymbolId

literal
    Literal
```

An unknown string is not automatically an error. Encoding preserves it as a literal.

## Validate at boundaries

Treat deserialized data as untrusted data.

```csharp
var received = ProtocolPayloadJson.Deserialize(json);
protocol.Validate(received);
var values = protocol.Resolve(received);
```

The order matters.

Structural JSON validation happens during deserialization.

Semantic reference validation happens against the expected ProtocolDefinition.

Authorization happens later in the host.

## JSON wire representation

Alpha 3 intentionally uses a boring representation:

```json
{"protocolId":1001,"values":[{"symbolId":2001},{"literal":"New Character"}]}
```

It is useful precisely because it does not contain provider-specific concepts.

The representation can be carried by HTTP, WebSocket framing, queues, files, databases, or clipboard exchange.

See [PROTOCOL_PAYLOAD_JSON.md](PROTOCOL_PAYLOAD_JSON.md) for the exact contract.

## Error thinking

Invalid construction and malformed external data should fail explicitly.

Do not build application logic around matching exception message text.

Instead, catch the appropriate exception type at the boundary where recovery is meaningful and preserve the underlying cause for diagnostics.

## Testing strategy

At minimum, test:

1. protocol construction;
2. duplicate identity rejection;
3. duplicate value/name rejection where applicable;
4. known-value encoding;
5. literal preservation;
6. protocol ownership validation;
7. undefined-reference rejection;
8. reference decoding;
9. deterministic description;
10. JSON serialization;
11. JSON round-trip;
12. malformed JSON;
13. both reference and literal rejected;
14. neither reference nor literal rejected;
15. unknown symbol preservation during deserialization.

A small package should have a very visible contract.

## Provider neutrality

The same ProtocolDefinition should work when the source is:

- an LLM;
- a human;
- a GUI;
- another process;
- deterministic code;
- a test fixture.

That is a useful design test:

> If removing the AI provider would make the vocabulary meaningless, the vocabulary is probably owned at the wrong layer.

## Performance mindset

Do not serialize merely to move data between two objects in the same process.

Use the JSON representation when a real representation boundary exists.

For performance-sensitive paths, measure the actual application workload rather than assuming that semantic identity, JSON, or provider interaction has a particular cost.

## C# integration pattern

A healthy host often looks like:

```csharp
var protocol = BuildApplicationVocabulary();

var outgoing = protocol.Encode(values);

var json = ProtocolPayloadJson.Serialize(outgoing);

// transport boundary

var incoming = ProtocolPayloadJson.Deserialize(json);

protocol.Validate(incoming);

var resolved = protocol.Resolve(incoming);

ApplyHostPolicy(resolved);
```

ProtocolAI stops before ApplyHostPolicy.

That boundary is deliberate.

## Related guides

- [Beginner's Guide](BEGINNERS_GUIDE.md) for the first working example.
- [Developer's Guide](DEVELOPERS_GUIDE.md) for compatibility and architecture.
- [AI Exchange](AI_EXCHANGE.md) for provider-neutral exchange.
- [Alpha 3 JSON specification](PROTOCOL_PAYLOAD_JSON.md) for wire details.