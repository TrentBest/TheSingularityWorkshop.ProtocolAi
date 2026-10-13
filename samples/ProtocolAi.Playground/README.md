# ProtocolAi Living Documentation

This is an interactive C# / Blazor WebAssembly playground for the real TheSingularityWorkshop.ProtocolAi library.

## Run it

Requires the .NET 8 SDK.

    dotnet run --project samples/ProtocolAi.Playground/ProtocolAi.Playground.csproj

Open the local URL printed by dotnet run.

## What it demonstrates

- An illustrative batch of candidate LLM-like outputs. These are fixtures, not live inference or measured probabilities.
- A semantic ball-drop experiment whose payload comes from ProtocolDefinition.Encode, Validate, and Resolve.
- Visible state progression driven by TheSingularityWorkshop.FSM_API.
- A deliberate invalid-reference test, distinct from an unknown literal.
- A conceptual ProtocolAi → GrammarAi composition diagram.

The ProtocolAi core project has no dependency on FSM_API, Visualize, a model provider, or the browser UI. FSM_API is a sample-only dependency. The visual treatment borrows the inspectable SVG-experiment approach used in Visualize; it does not claim to execute Visualize's C# backend or a production GPU renderer.

## Boundaries

The example does not call a live LLM, estimate hallucination rates, authorize commands, or claim that integer IDs make inference deterministic. It demonstrates the narrower property: the application can assign stable identities to known vocabulary values and keep unknown literals distinct.
