<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# ProtocolSymbol

## Purpose

One named vocabulary entry with stable identity and an application-owned value.

## Declaration and ownership

- **Declaration:** public readonly record struct ProtocolSymbol.
- **Source:** [ProtocolSymbol.cs](ProtocolSymbol.cs).
- **Related source types:** ProtocolBuilder; ProtocolDefinition; ProtocolReference.
- **Responsibilities:** Stores a non-zero symbol ID, name, and value as the atom of a protocol definition.
- **Non-goals:** It does not define application permissions or guarantee that a model will select the right symbol.

## Contract and use

Symbol names are limited to 256 UTF-16 characters and symbol values to 16,384 UTF-16 characters. A containing protocol can define at most 4,096 symbols. These bounds keep vocabulary descriptions and lookups finite; they do not constrain the application's interpretation of a symbol.

The protocol vocabulary is application-owned. IDs are unsigned 64-bit identities and a symbol address is qualified by its protocol ID. Known values can be encoded as references; unknown values remain explicit literals instead of being assigned invented IDs.

## Verification

Relevant tests: [tests/ProtocolAi.Tests/ProtocolDefinitionTests.cs](tests/ProtocolAi.Tests/ProtocolDefinitionTests.cs). This guide is source-authored and checked against the current public declaration; it is not generated from reflection or compiler AST.

## Related documentation

- [Problem domain](docs/PROBLEM_DOMAIN.md)
- [Consuming guide](docs/CONSUMING.md)
- [Theory](docs/THEORY.md)
- [Companion UML and use cases](./ProtocolSymbolUML.md)


---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
