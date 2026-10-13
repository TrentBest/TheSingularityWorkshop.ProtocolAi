# ProtocolAi Living Documentation

**There are two ways to experience this:**

1. **See the living diagrams right here on GitHub** — the animated SVGs below run directly in this README.
2. **Run the interactive C# playground locally** — this is the full experience with controls, a live FSM_API-driven transition, and inspectable payload/resolution.

## Watch the idea move

### The semantic flow

![Animated ProtocolAi flow: known values become references, unknown values remain literals, and the host decides.](../../docs/images/protocol-ai-living-flow.svg)

### Three distinct outcomes

![Animated comparison: a known value becomes a reference, an unknown value remains a literal, and an undefined reference is rejected.](../../docs/images/protocol-ai-three-outcomes.svg)

These diagrams are living documentation, not the interactive app. GitHub displays the SVG animations in the README; to change inputs, step the state machine, trigger the invalid-reference case, and inspect actual payload data, run the playground below.

## Start with SVG Hello World

For the smallest hands-on example, run the playground and open **[`/hello`](http://localhost:5000/hello)** using the local origin printed by `dotnet run` (the port may differ):

~~~powershell
dotnet run --project samples/ProtocolAi.Playground/ProtocolAi.Playground.csproj
~~~

The **SVG Hello World** page uses a separate, real FSM_API two-state flow. Advance between `Hello` and `Moving` to change the circle's position. The width and height sliders change the SVG's rendered viewport while its `viewBox` keeps the geometry's internal coordinate system stable. This is the first size-aware SVG GUI experiment—not a production renderer and not an SVG that executes FSM_API itself.

## Run the interactive playground

Requires the **.NET 8 SDK**. From the repository root:

~~~powershell
dotnet run --project samples/ProtocolAi.Playground/ProtocolAi.Playground.csproj
~~~

Then open the **local URL printed by `dotnet run`** in your browser. The playground is not currently published at a public hosted URL; the local run is where you can interact with it.

## What you can explore

- Compare multiple illustrative candidate responses to the same intent.
- Replay a second fixture batch and inspect wording, semantic, and shape variation.
- Release a concept through encoding, validation, and resolution.
- Compare a known value, an unknown literal, and an intentionally invalid reference.
- Step the actual FSM_API-driven flow one transition at a time.
- Inspect the live `ProtocolPayload` and resolved value.
- Explore the conceptual relationship between ProtocolAi and GrammarAi.

The candidate responses are fixtures—not live model output or measured probabilities. The demo uses the real ProtocolAi C# library; FSM_API is a sample-only dependency that drives the visible state progression.

## Architectural boundaries

The ProtocolAi core has no dependency on FSM_API, Visualize, a model provider, or the browser UI. The visual treatment borrows the inspectable SVG-experiment approach used in Visualize; it does not claim to execute Visualize's C# backend or a production GPU renderer.

The example does not call a live LLM, estimate hallucination rates, authorize commands, or claim that integer IDs make inference deterministic. It demonstrates the narrower property: the application can assign stable identities to known vocabulary values and keep unknown literals distinct.
