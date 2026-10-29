<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# ProtocolPayloadJson

## Purpose

Dependency-free JSON serializer/deserializer for ProtocolPayload.

## Declaration and ownership

- **Declaration:** public static class ProtocolPayloadJson.
- **Source:** [ProtocolPayloadJson.cs](ProtocolPayloadJson.cs).
- **Related source types:** ProtocolPayload; ProtocolValue.
- **Responsibilities:** Converts the payload contract to and from JSON without requiring a provider SDK or network transport.
- **Non-goals:** JSON decoding is representation handling, not protocol trust, semantic authorization, or command safety.

## Contract and use

The protocol vocabulary is application-owned. IDs are unsigned 64-bit identities and a symbol address is qualified by its protocol ID. Known values can be encoded as references; unknown values remain explicit literals instead of being assigned invented IDs.

## Verification

Relevant tests: [tests/ProtocolAi.Tests/ProtocolPayloadJsonTests.cs](tests/ProtocolAi.Tests/ProtocolPayloadJsonTests.cs). This guide is source-authored and checked against the current public declaration; it is not generated from reflection or compiler AST.

## Related documentation

- [Problem domain](docs/PROBLEM_DOMAIN.md)
- [Consuming guide](docs/CONSUMING.md)
- [Theory](docs/THEORY.md)
- [Companion UML and use cases](./ProtocolPayloadJsonUML.md)


---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
