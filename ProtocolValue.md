<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# ProtocolValue

## Purpose

A discriminated value that is either a symbol reference or an explicit literal.

## Declaration and ownership

- **Declaration:** public readonly record struct ProtocolValue.
- **Source:** [ProtocolValue.cs](ProtocolValue.cs).
- **Related source types:** ProtocolDefinition; ProtocolPayload; ProtocolPayloadJson.
- **Responsibilities:** Provides FromId and FromLiteral factories and exposes IsReference/IsLiteral so known identities and unknown text remain distinct.
- **Non-goals:** A reference does not carry its protocol ID itself; the enclosing payload and definition supply that context. A literal is not automatically trusted.

## Contract and use

The protocol vocabulary is application-owned. IDs are unsigned 64-bit identities and a symbol address is qualified by its protocol ID. Known values can be encoded as references; unknown values remain explicit literals instead of being assigned invented IDs.

## Verification

Relevant tests: [tests/ProtocolAi.Tests/ProtocolValueTests.cs; tests/ProtocolAi.Tests/ProtocolResolutionTests.cs](tests/ProtocolAi.Tests/ProtocolValueTests.cs). This guide is source-authored and checked against the current public declaration; it is not generated from reflection or compiler AST.

## Related documentation

- [Problem domain](docs/PROBLEM_DOMAIN.md)
- [Consuming guide](docs/CONSUMING.md)
- [Theory](docs/THEORY.md)
- [Companion UML and use cases](./ProtocolValueUML.md)


---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
