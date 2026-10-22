<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# ProtocolAi Gallery

ProtocolAi's visuals explain the semantic boundary: known values can become application-owned references, unknown values remain explicit literals, and invalid references are rejected by the protocol contract. The SVGs are explanatory artwork; their moving signals illustrate the concept rather than reporting live runtime state.

## Visual tour

### The money shot

![ProtocolAi: application-owned semantic addresses](images/protocol-ai-money-shot.svg)

This authored diagram introduces the application-owned vocabulary boundary; it is not a generated source graph.

### Follow the signal

![ProtocolAi lifecycle](images/protocol-ai-domain-lifecycle.svg)

![ProtocolAi living flow](images/protocol-ai-living-flow.svg)

These animated diagrams distinguish encoding, validation, resolution, and the host's decision. Animation is illustrative; executable behavior is in the library and its tests.

### Three outcomes and payload round-trip

![Known values, unknown literals, and invalid references](images/protocol-ai-three-outcomes.svg)

![ProtocolAi payload round trip](images/protocol-ai-payload-roundtrip.svg)

![ProtocolAi semantic boundary](assets/protocolai-semantic-boundary.svg)

The JSON representation transports protocol identity and values. It does not define application meaning, authorize actions, or contact an AI provider.

## Read the three outcomes

The three-outcomes visual is easiest to understand as a boundary decision, not as a confidence score:

| Outcome | What it means | What the host still decides |
|---|---|---|
| **Known reference** | The value maps to an identity defined by the application-owned protocol. | Whether that identity is relevant and permitted in the current task. |
| **Unknown literal** | The value is not in the known vocabulary but is preserved as explicit literal data where the contract allows it. | Whether to accept, transform, ask about, or reject that literal. |
| **Invalid reference** | The input violates the protocol's identity or payload rules and cannot be treated as a valid reference. | How to report or recover from the invalid input. |

These outcomes are deliberately different. An unknown value is not automatically an error, and a valid identity is not proof that the model understood the task or that the application should act on it. ProtocolAi makes identity handling explicit; it does not make model inference deterministic or grant authority.

## See actual behavior

From the repository root:

```bash
dotnet run --project samples/ProtocolAi.Playground/ProtocolAi.Playground.csproj
dotnet test tests/ProtocolAi.Tests/ProtocolAi.Tests.csproj --configuration Release
```

The playground calls the real library; tests cover encoding, validation, resolution, immutability, and JSON edge cases. These commands are behavioral evidence; the SVGs are visual explanations.

## Detailed recipe

Start with the [consumer guide](CONSUMING.md), then explore the [examples](EXAMPLES.md) and [JSON wire contract](PROTOCOL_PAYLOAD_JSON.md).

---

<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://github.com/TrentBest.png?size=200" alt="The Singularity Workshop on GitHub" width="200">
  </a>
</p>

<p align="center">
  <a href="https://github.com/TrentBest">GitHub</a> ·
  <a href="https://coderlegion.com/">Coder Legion</a> ·
  <a href="https://www.patreon.com/c/TheSingularityWorkshop">Patreon</a> ·
  <a href="https://www.paypal.com/donate/?hosted_button_id=3Z7263LCQMV9J">PayPal</a>
  <br>
  <a href="https://github.com/TrentBest/FSM_API">FSM_API</a> ·
  <a href="https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization">FSM_Serialization</a> ·
  <a href="https://github.com/TrentBest/WebPage">WebPage</a>
</p>

<p align="center"><em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br><strong>Because state shouldn't be a mess.</strong><br><em>And because static boundaries are invitations to cause trouble.</em></p>

## Phone-first release review

Start with [Consuming ProtocolAi](CONSUMING.md) to define a vocabulary, encode values, preserve unknown literals, and resolve known identities.
