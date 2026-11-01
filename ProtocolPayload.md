<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# ProtocolPayload

## Purpose

Protocol-qualified collection of typed values to validate, resolve, or serialize.

## Declaration and ownership

- **Declaration:** public sealed class ProtocolPayload.
- **Source:** [ProtocolPayload.cs](ProtocolPayload.cs).
- **Related source types:** ProtocolDefinition; ProtocolValue; ProtocolPayloadJson.
- **Responsibilities:** Stores the owning protocol ID and a read-only sequence of ProtocolValue instances.
- **Non-goals:** It does not prove that the protocol ID exists or that every reference is defined; ProtocolDefinition.Validate performs definition-specific validation.

## Contract and use

A protocol definition is limited to 4,096 symbols and a payload to 65,536 values. Protocol and symbol names are limited to 256 UTF-16 characters; vocabulary values and payload literals are limited to 16,384 UTF-16 characters. The constructors enforce these bounds before retaining collections or literals.

The protocol vocabulary is application-owned. IDs are unsigned 64-bit identities and a symbol address is qualified by its protocol ID. Known values can be encoded as references; unknown values remain explicit literals instead of being assigned invented IDs.

## Verification

Relevant tests: [tests/ProtocolAi.Tests/ProtocolPayloadJsonTests.cs; tests/ProtocolAi.Tests/ProtocolResolutionTests.cs](tests/ProtocolAi.Tests/ProtocolPayloadJsonTests.cs). This guide is source-authored and checked against the current public declaration; it is not generated from reflection or compiler AST.

## Related documentation

- [Problem domain](docs/PROBLEM_DOMAIN.md)
- [Consuming guide](docs/CONSUMING.md)
- [Theory](docs/THEORY.md)
- [Companion UML and use cases](./ProtocolPayloadUML.md)


---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
