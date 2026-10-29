<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" />
  </a>
</p>

# ProtocolAi — Documentation

ProtocolAi gives application-owned vocabulary values stable, protocol-qualified identities. Known values can be resolved against their definitions; unknown values remain visible as literals for the host to handle.

## Start here

| Your goal | Read |
|---|---|
| Understand the problem and the value of stable identity | [README](README.md) |
| Add ProtocolAi to a .NET application | [Consuming ProtocolAi](docs/CONSUMING.md) |
| Define a vocabulary and encode values | [Examples](docs/EXAMPLES.md) |
| Understand the JSON payload contract | [ProtocolPayloadJson](docs/PROTOCOL_PAYLOAD_JSON.md) |
| See identity, literal, and resolution flows | [Gallery](docs/GALLERY.md) |
| Understand how the host and protocol definitions share responsibilities | [Theory](docs/THEORY.md) |
| Explore the formal model and its limits | [Mathematical foundations](docs/MATHEMATICAL_FOUNDATIONS.md) |


## Source-level type guides and UML

| Type | Contract and purpose | UML and use cases |
|---|---|---|
| ProtocolBuilder | [Type guide](ProtocolBuilder.md) | [UML](ProtocolBuilderUML.md) |
| ProtocolDefinition | [Type guide](ProtocolDefinition.md) | [UML](ProtocolDefinitionUML.md) |
| ProtocolPayload | [Type guide](ProtocolPayload.md) | [UML](ProtocolPayloadUML.md) |
| ProtocolPayloadJson | [Type guide](ProtocolPayloadJson.md) | [UML](ProtocolPayloadJsonUML.md) |
| ProtocolReference | [Type guide](ProtocolReference.md) | [UML](ProtocolReferenceUML.md) |
| ProtocolSymbol | [Type guide](ProtocolSymbol.md) | [UML](ProtocolSymbolUML.md) |
| ProtocolValue | [Type guide](ProtocolValue.md) | [UML](ProtocolValueUML.md) |

## Where it fits

- [GrammarAi](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi) defines structure using protocol-owned terminal identities.
- [HeadlessAi](https://github.com/TrentBest/TheSingularityWorkshop.HeadlessAi) communicates with configured AI-capable HTTP endpoints.
- [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) composes reusable MicroBundle capabilities into a runtime assembly.

ProtocolAi does not call a model, authorize an action, or execute application behavior. The consuming application remains responsible for those decisions.

**Explore the Workshop:** [Find packages on NuGet](https://www.nuget.org/packages?q=TheSingularityWorkshop) · [Browse the repository guide](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/docs/WORKSHOP_REPOSITORIES.md).

---

<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://github.com/TrentBest.png?size=200" alt="The Singularity Workshop on GitHub" width="200">
  </a>
</p>

<p align="center"><a href="https://github.com/TrentBest">GitHub</a> · <a href="https://coderlegion.com/">Coder Legion</a> · <a href="https://www.patreon.com/c/TheSingularityWorkshop">Patreon</a></p>
