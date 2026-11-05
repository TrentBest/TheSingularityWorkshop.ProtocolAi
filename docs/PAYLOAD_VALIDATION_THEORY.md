<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Payload transport, validation, and failure semantics

## 1. The boundary problem

A sender and receiver can agree on the *shape* of a message while disagreeing about what its identifiers mean. They can also agree on the vocabulary while receiving malformed or resource-exhausting input.

These are different failure classes. ProtocolAi separates them into two stages:

1. **Transport parsing** reconstructs a bounded `ProtocolPayload` from JSON and checks the wire representation.
2. **Semantic validation** checks that the payload belongs to the expected `ProtocolDefinition` and that every referenced symbol exists.

Only after both stages should the application resolve values and apply its own policy. JSON parsing does not establish semantic ownership; semantic validation does not establish authorization.

## 2. The wire model

A payload contains one non-zero protocol ID and an ordered sequence of values. Each value is exactly one of:

- a non-zero `symbolId`, referring to a symbol in the payload's protocol namespace; or
- a non-empty `literal`, preserving text that has not been mapped to a known symbol.

Example:

```json
{"protocolId":1001,"values":[{"symbolId":2001},{"literal":"Amelia"},{"symbolId":2002}]}
```

The array order is significant and is preserved by serialization, deserialization, and resolution. The payload does not carry the vocabulary definition itself. The receiving application must already know which definition it expects.

## 3. Two validation stages

### Stage A — structural transport validation

`ProtocolPayloadJson.Deserialize(json)` checks the JSON representation and creates a payload. It rejects:

- null, empty, or whitespace-only input;
- input longer than the configured JSON character limit;
- malformed JSON and values outside the unsigned 64-bit integer range;
- duplicate property names, including nested objects and objects inside arrays;
- a missing or zero `protocolId`;
- a missing or null `values` array;
- a value containing both `symbolId` and `literal`, or neither;
- zero symbol IDs and empty/whitespace-only literals;
- a values array exceeding the payload value limit.

Unknown, non-duplicate properties are ignored by the current deserializer. This is a forward-tolerance behavior, not a promise that arbitrary extensions have defined meaning. A sender should emit the documented wire shape; applications that need stricter schema policy should enforce it at their own boundary.

A successfully deserialized payload means only that the data has an acceptable structural representation and satisfies the payload's local construction limits.

### Stage B — semantic protocol validation

The receiving definition performs the ownership check:

```csharp
var received = ProtocolPayloadJson.Deserialize(json);
people.Validate(received);
var values = people.Resolve(received);
```

Validation rejects a payload whose `ProtocolId` differs from the expected definition and rejects any symbol ID that is not defined by that definition. A literal does not need to be in the vocabulary; preserving unknown text as a literal is intentional.

The `Resolve(ProtocolPayload)` overload validates the payload before resolving its values. By contrast, `Resolve(ProtocolValue)` receives a single value without protocol provenance and cannot check which protocol originally supplied its symbol ID. Prefer the payload overload when ownership matters.

## 4. Failure classes and expected handling

| Input or condition | Stage | ProtocolAi behavior | Application responsibility |
|---|---|---|---|
| Invalid JSON syntax | Structural | Throws `ArgumentException` with JSON parse error as inner exception | Reject input; record safe diagnostics |
| Duplicate JSON property | Structural | Rejects the payload | Reject; do not rely on parser-specific duplicate handling |
| Both or neither value representation | Structural | Rejects the value | Correct the sender or reject |
| Zero ID or blank literal | Structural/domain shape | Rejects the payload | Correct the sender or reject |
| Too many values / too much text | Structural/resource | Rejects before retaining an oversized payload | Apply ingress limits and operational rate controls |
| Unknown non-duplicate property | Structural | Ignores the property | Decide whether your integration requires a stricter schema |
| Unknown symbol ID | Semantic | Deserialization preserves it; definition validation rejects it | Reject, migrate, or route to an explicit compatibility policy |
| Wrong protocol ID | Semantic | Definition validation rejects it | Select the correct definition or reject |
| Unknown literal | Semantic | Preserved and resolves as that literal | Decide whether to retain, clarify, register through an application workflow, or reject |
| Valid reference but unauthorized request | Host policy | Not evaluated by ProtocolAi | Authenticate, authorize, approve, and constrain execution |

The distinction between an unknown literal and an undefined reference is important. A literal explicitly says “this text has not been assigned an identity here.” An undefined reference claims an identity address that the receiving vocabulary does not own.

## 5. Resource bounds are part of the contract

The current public `ProtocolLimits` constants establish these bounds:

| Resource | Limit |
|---|---:|
| JSON input / serialized JSON | 8,388,608 UTF-16 characters |
| Values in one payload | 65,536 |
| One literal | 16,384 UTF-16 characters |
| Aggregate literal text in one payload | 1,048,576 UTF-16 characters |
| Symbols in one definition | 4,096 |
| Protocol or symbol name | 256 UTF-16 characters |

These are representation and memory bounds, not semantic guarantees. They are measured in .NET UTF-16 `string.Length` characters, not UTF-8 bytes, Unicode scalar values, or grapheme clusters. A transport adapter should also impose its own request-body, timeout, concurrency, and rate limits appropriate to its environment; ProtocolAi's in-memory limits are not a complete network-denial-of-service defense.

The deserializer checks the JSON character limit before parsing and checks the values-array count before materializing wire records. The payload constructor independently bounds its values and aggregate literal text. Serialization also rejects output exceeding the JSON character limit.

## 6. Verified round-trip example

The following pattern is exercised by `Deserialize_round_trips_mixed_payload` and `Serialize_preserves_protocol_identity_and_mixed_values` in `tests/ProtocolAi.Tests/ProtocolPayloadJsonTests.cs`:

```csharp
var people = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Build();

var outgoing = people.Encode(["Bob", "Amelia", "Jane"]);
var json = ProtocolPayloadJson.Serialize(outgoing);

// A receiving component reconstructs data, not meaning.
var received = ProtocolPayloadJson.Deserialize(json);

// The expected definition establishes ownership and symbol existence.
people.Validate(received);
var resolved = people.Resolve(received);

// ["Bob", "Amelia", "Jane"]
// Bob and Jane resolve through IDs; Amelia remains a literal.
```

The exact serialized form for this example is:

```json
{"protocolId":1001,"values":[{"symbolId":2001},{"literal":"Amelia"},{"symbolId":2002}]}
```

<text>**Verification label:** Source-checked example, with its mixed-value serialization and round-trip behavior covered by the named unit tests. It is not a live network or multi-process interoperability test.</text>

## 7. Canonical form and compatibility limits

The serializer emits compact JSON with the documented property names and a stable field order. The current reader ignores unknown properties but rejects duplicate names. Consumers should depend on the documented field semantics, not incidental whitespace or formatting.

This representation has a `protocolId`, but it does not include a separate schema-version or negotiation field. It does not negotiate definitions, fetch vocabularies, migrate IDs, or guarantee that a sender and receiver deployed compatible definitions. The host must select the expected definition and decide its version policy before accepting a payload.

## 8. Security and responsibility boundary

The following statements are deliberately distinct:

- **Deserialized:** the input was accepted as a bounded payload representation.
- **Validated:** the payload belongs to the expected protocol and all references are defined there.
- **Resolved:** references were mapped through that definition; literals remained literals.
- **Authorized:** the host has decided that this actor may request the operation in this context.
- **Executed:** the host has performed an explicitly implemented operation.

ProtocolAi implements the first three steps. It does not authenticate the sender, authorize actions, detect replay, rate-limit a network endpoint, or execute application behavior. A valid symbol can still describe an inappropriate request; host policy remains mandatory.

## 9. Evidence map

| Claim | Source | Tests |
|---|---|---|
| Wire field names and compact serialization | `ProtocolPayloadJson.cs` | `Serialize_preserves_protocol_identity_and_mixed_values` |
| Structural validation and duplicate-property rejection | `ProtocolPayloadJson.cs` | malformed, duplicate, invalid-shape, and overflow cases in `ProtocolPayloadJsonTests.cs` |
| Unknown properties ignored | `ProtocolPayloadJson.cs` | `Deserialize_ignores_unknown_properties_for_forward_compatibility` |
| Value count and aggregate literal bounds | `ProtocolPayload.cs`, `ProtocolLimits.cs` | oversized values and literal aggregate tests |
| Wrong protocol and undefined symbol rejection | `ProtocolDefinition.Validate` | `ProtocolResolutionTests.cs` |
| Ordered mixed-value resolution | `ProtocolDefinition.Resolve(ProtocolPayload)` | `Resolve_decodes_references_and_preserves_literals_in_order` |

The evidence establishes local library behavior. It does not establish provider compliance, network reliability, end-to-end security, or cross-host delivery semantics.

## 10. Practice and assessment

1. Why is valid JSON insufficient to establish that a symbol ID has meaning to the receiver?
2. Give one example of a structurally invalid value and one structurally valid but semantically invalid payload.
3. Why does ProtocolAi preserve an unknown literal but reject an undefined symbol reference?
4. What provenance is missing from a standalone `ProtocolValue` reference, and how does the payload overload address that gap?
5. Why are UTF-16 character limits not interchangeable with HTTP byte limits?
6. Which additional controls would a public network endpoint need beyond ProtocolAi's local payload bounds?
7. Explain why a successful `Validate` call still cannot authorize execution.

## Further reading

- [Semantic Payload JSON](PROTOCOL_PAYLOAD_JSON.md) — concise wire contract and transport walkthrough.
- [ProtocolPayload type guide](../ProtocolPayload.md) — payload ownership and bounds.
- [ProtocolPayloadJson type guide](../ProtocolPayloadJson.md) — serializer API.
- [ProtocolLimits type guide](../ProtocolLimits.md) — public resource limits.
- [Identity lifecycle and evolution](IDENTITY_LIFECYCLE.md) — durable meaning across vocabulary changes.
- [Theory index](THEORY_INDEX.md) — the complete ProtocolAi learning path.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
