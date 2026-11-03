<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# ProtocolDefinition

## Purpose

Immutable vocabulary that resolves names and values through protocol-qualified symbol identities.

## Declaration and ownership

- **Declaration:** public sealed class ProtocolDefinition.
- **Source:** [ProtocolDefinition.cs](ProtocolDefinition.cs).
- **Related source types:** ProtocolBuilder; ProtocolSymbol; ProtocolReference; ProtocolValue; ProtocolPayload.
- **Responsibilities:** Owns a protocol ID/name and indexes symbols by ID, name, and value. It encodes known values, preserves unknown values as literals, validates payload ownership and references, emits a deterministic description, and compares a published baseline with a candidate definition through `CompareCompatibility`.
- **Non-goals:** It does not own model inference, transport, authorization, or execution.

## Contract and use

The protocol vocabulary is application-owned. IDs are unsigned 64-bit identities and a symbol address is qualified by its protocol ID. Known values can be encoded as references; unknown values remain explicit literals instead of being assigned invented IDs.

## Protocol evolution compatibility

Call `baseline.CompareCompatibility(candidate)` before adopting a changed vocabulary. The report allows additive symbol IDs and reports a changed protocol ID, a removed published symbol ID, or a changed name/value for an existing ID. Findings are deterministic and ordered by the baseline symbol order. This is a structural check only: it cannot detect a domain meaning change when the ID, name, and literal value remain unchanged, and it does not authorize adoption or perform migration.

## Verification

Relevant tests: [tests/ProtocolAi.Tests/ProtocolDefinitionTests.cs; tests/ProtocolAi.Tests/ProtocolResolutionTests.cs](tests/ProtocolAi.Tests/ProtocolDefinitionTests.cs). This guide is source-authored and checked against the current public declaration; it is not generated from reflection or compiler AST.

## Related documentation

- [Problem domain](docs/PROBLEM_DOMAIN.md)
- [Consuming guide](docs/CONSUMING.md)
- [Theory](docs/THEORY.md)
- [Companion UML and use cases](./ProtocolDefinitionUML.md)


---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
