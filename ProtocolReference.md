<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# ProtocolReference

## Purpose

Protocol-qualified symbol address that pairs protocol identity with symbol identity.

## Declaration and ownership

- **Declaration:** public readonly record struct ProtocolReference.
- **Source:** [ProtocolReference.cs](ProtocolReference.cs).
- **Related source types:** ProtocolDefinition; ProtocolSymbol; ProtocolValue.
- **Responsibilities:** Carries ProtocolId and SymbolId so a symbol address is not treated as globally unique without its owner.
- **Non-goals:** It does not contain the symbol's value or verify that the referenced definition currently exists.

## Contract and use

The protocol vocabulary is application-owned. IDs are unsigned 64-bit identities and a symbol address is qualified by its protocol ID. Known values can be encoded as references; unknown values remain explicit literals instead of being assigned invented IDs.

## Verification

Relevant tests: [tests/ProtocolAi.Tests/ProtocolResolutionTests.cs](tests/ProtocolAi.Tests/ProtocolResolutionTests.cs). This guide is source-authored and checked against the current public declaration; it is not generated from reflection or compiler AST.

## Related documentation

- [Problem domain](docs/PROBLEM_DOMAIN.md)
- [Consuming guide](docs/CONSUMING.md)
- [Theory](docs/THEORY.md)
- [Companion UML and use cases](./ProtocolReferenceUML.md)


---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
