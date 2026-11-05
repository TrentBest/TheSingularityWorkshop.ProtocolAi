<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Protocol identity lifecycle and evolution

## 1. The problem: an address outlives the conversation

An application may tell a model that `2001` means the person Bob. That mapping is useful only if the application continues to mean the same thing when it reads the reference tomorrow, after a deployment, or in a payload produced by another component.

This is an identity-lifecycle problem, not merely an integer-encoding problem.

A protocol symbol is an application-owned address. Once a symbol ID is published to another component, persisted, or included in a message, changing what that ID means can silently reinterpret old data. ProtocolAi can compare some structural changes, but the application remains responsible for preserving semantic meaning.

This chapter explains the current identity model, the compatibility check, and the limits of what the library can know.

## 2. Vocabulary and address model

A definition has a non-zero protocol ID and a name. Each symbol has a non-zero symbol ID, a name, and a value.

```text
ProtocolDefinition
  Protocol ID: 1001
  Name: People
  Symbols:
    Symbol ID: 2001
    Name: bobId
    Value: Bob
```

The portable identity is the pair:

```text
(protocol ID, symbol ID)
(1001, 2001)
```

The protocol ID qualifies the symbol ID. A bare integer is not a global identity: `(1001, 2001)` and `(9001, 2001)` belong to different protocol namespaces.

The implementation represents this qualified pair with `ProtocolReference`. A `ProtocolValue` reference carries the symbol ID within a payload whose `ProtocolId` supplies the namespace. Keep that distinction in mind when choosing an API: `Decode(ProtocolReference)` checks both IDs, while `Resolve(ProtocolValue)` resolves an unqualified value in the current definition. For a collection, `Resolve(ProtocolPayload)` validates the payload's protocol ownership and every referenced symbol before resolving it.

## 3. Author, build, and publish a definition

`ProtocolBuilder` is the mutable authoring surface. `Build()` creates a `ProtocolDefinition`, which is the immutable snapshot consumed by the rest of the API.

```csharp
var builder = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane");

var peopleV1 = builder.Build();

// A later builder change does not mutate the already-built definition.
builder.Define(2003, "saraId", "Sara");
```

The snapshot behavior is tested by `Definition_is_an_immutable_snapshot_of_the_builder` in `tests/ProtocolAi.Tests/ProtocolDefinitionTests.cs`.

The definition rejects duplicate symbol IDs, names, and values using ordinal string comparison. Matching is case-sensitive and does not normalize Unicode: `Bob` is a known value in the example, while `bob` remains a literal. The tests `Duplicate_ids_names_and_values_are_rejected`, `Encode_uses_ordinal_case_sensitive_matching`, and `Encode_rejects_empty_literals_and_preserves_unicode_without_normalization` make these rules executable.

These constraints matter because a lookup rule is part of the identity contract. If one version treats case or Unicode normalization differently from another, the same incoming text could resolve differently. ProtocolAi currently uses exact ordinal matching; applications should not assume culture-aware matching or implicit normalization.

## 4. Identity lifecycle rules

Treat a published protocol definition as a durable contract.

1. Allocate a protocol ID for one vocabulary namespace and keep it stable.
2. Allocate each symbol ID once within that namespace.
3. Never recycle a published symbol ID for a different concept.
4. Preserve the name and value associated with an existing symbol ID.
5. Add new symbols with new IDs.
6. If a concept must change meaning, define an explicit migration or a new identity/versioning strategy at the application boundary.

The library enforces non-zero IDs and rejects duplicate IDs within one definition. It does not provide a distributed ID allocator, registry, migration engine, or publication authority. Those are application and deployment responsibilities.

The protocol's display name is not the qualified address. The current compatibility comparison is keyed by protocol ID and existing symbol IDs; it does not report a protocol-name-only change. Applications that use names in user interfaces or external registries should manage that metadata separately.

## 5. Structural compatibility

Use the previously accepted definition as the baseline and compare it with the proposed candidate:

```csharp
var peopleV2 = new ProtocolBuilder(1001, "People")
    .Define(2001, "bobId", "Bob")
    .Define(2002, "janeId", "Jane")
    .Define(2003, "saraId", "Sara")
    .Build();

var report = peopleV1.CompareCompatibility(peopleV2);

if (!report.IsCompatible)
{
    // The application decides whether to reject, migrate, or otherwise handle it.
}
```

The current structural rules are:

| Candidate change | Result |
|---|---|
| Add a new symbol ID | Compatible |
| Change the protocol ID | Incompatible |
| Remove an existing symbol ID | Incompatible |
| Change the name or value of an existing symbol ID | Incompatible |
| Change only the protocol display name | Not reported by this comparison |

The report is deterministic and findings follow baseline symbol order. The implementation is in `ProtocolDefinition.CompareCompatibility`; the tests `Compatibility_allows_additive_symbols_without_reassigning_published_ids`, `Compatibility_reports_removed_and_redefined_symbols`, and `Compatibility_rejects_a_different_protocol_identity` cover the principal cases.

Compatibility is directional: call the method on the old definition and pass the proposed new definition. Reversing those roles changes which symbols count as baseline commitments.

## 6. What the comparison cannot prove

The comparison is intentionally structural. It can detect that a symbol with ID `2001` changed from the recorded name/value pair. It cannot detect that the application silently changed the real-world meaning of Bob while leaving the ID, name, and value unchanged.

For example, if an application reuses the label `Bob` and the value `Bob` for a different person, the two definitions are structurally identical. The library has no domain database or historical evidence with which to discover the semantic substitution.

Likewise, a compatible report does not prove that every consumer can safely upgrade. An older consumer may not know an additive symbol ID and should reject that undefined reference rather than inventing a meaning. Deployment order, supported-version policy, fallback behavior, and migration remain host concerns.

Therefore:

- **Structural compatibility** means the fields checked by ProtocolAi satisfy its comparison rules.
- **Semantic compatibility** means the application has preserved the domain meaning of every published identity.
- **Operational compatibility** means the deployed producers and consumers can exchange and handle the versions they actually encounter.

Only the first is evaluated by `CompareCompatibility`.

## 7. Worked evolution scenario

Suppose version 1 publishes:

```text
[1001] People
  [2001] bobId = "Bob"
  [2002] janeId = "Jane"
```

A later version adds Sara as ID 2003. This is structurally additive: old addresses retain their recorded definitions, and the new address is distinct.

Now consider three alternative edits:

- Reassign ID 2001 to a different person: the comparison reports a changed symbol definition if the recorded name or value changes. If the real-world meaning changes but those fields do not, only the application can detect it.
- Remove ID 2002: the comparison reports a removed symbol. Old payloads containing it cannot be resolved by the new definition.
- Change protocol ID 1001 to 9001: the comparison reports a protocol identity change. The qualified address space is different, even if all symbol IDs are copied.

Do not repair these cases by reusing old IDs for new meanings. Plan a migration or a new protocol identity, and define how old payloads will be handled.

## 8. Evidence and limits

Source mapping:

| Concern | Implementation | Tests |
|---|---|---|
| Non-zero protocol and symbol IDs | `ProtocolBuilder.cs`, `ProtocolSymbol.cs`, `ProtocolReference.cs` | `ProtocolDefinitionTests.cs` |
| Duplicate ID/name/value rejection | `ProtocolDefinition.cs` | `Duplicate_ids_names_and_values_are_rejected` |
| Immutable built snapshot | `ProtocolDefinition.cs` | `Definition_is_an_immutable_snapshot_of_the_builder` |
| Exact ordinal matching | `ProtocolDefinition.cs` | `Encode_uses_ordinal_case_sensitive_matching` |
| Structural compatibility | `ProtocolDefinition.CompareCompatibility` | Compatibility tests in `ProtocolDefinitionTests.cs` |
| Qualified payload validation | `ProtocolDefinition.Validate` | `ProtocolResolutionTests.cs` |

This chapter is source-checked against the current development implementation and named tests. It does not claim a distributed registry, automatic migration, semantic-diff engine, version negotiation, or multi-host interoperability.

## 9. Practice and assessment

1. Why is `(protocolId, symbolId)` a stronger address than a bare symbol ID?
2. What does the immutable snapshot protect against when a builder is changed after `Build()`?
3. Why does an additive symbol pass the current structural compatibility check?
4. Give an example of a semantic breaking change that the structural comparison cannot detect.
5. Why should an older consumer reject an unknown symbol ID rather than infer its meaning?
6. Design a migration plan for a symbol whose domain meaning must change. State how old payloads are recognized and resolved during the transition.

## Further reading

- [ProtocolDefinition type guide](../ProtocolDefinition.md) — public operations and compatibility contract.
- [ProtocolBuilder type guide](../ProtocolBuilder.md) — vocabulary authoring.
- [Problem domain](PROBLEM_DOMAIN.md) — ownership, boundaries, and resource limits.
- [Mathematical foundations](MATHEMATICAL_FOUNDATIONS.md) — qualified address space and validation properties.
- [Semantic payload JSON](PROTOCOL_PAYLOAD_JSON.md) — carrying identity across a transport boundary.
- [Theory index](THEORY_INDEX.md) — the complete learning path.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
