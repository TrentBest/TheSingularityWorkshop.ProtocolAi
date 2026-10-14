# ProtocolAi Living Documentation

> **A diagram should teach a concept. A living diagram should let the reader watch the concept unfold.**

The README is the front door. Animated SVGs invite the reader in; the local playground lets the reader change inputs and inspect actual library results; the mathematical foundations explain exactly what is guaranteed and what is not.

## The reader's route

Notice the problem → watch language enter the boundary → see three distinct outcomes → run the C# implementation → inspect payload and resolution → read the proof and limits.

## Living images in this repository

### 1. The money shot — the whole idea at a glance

![Animated overview of language entering the application-owned ProtocolAi address space.](images/protocol-ai-money-shot.svg)

**Teaches:** the model may propose language; the application owns the vocabulary; known values become references and unknown values remain visible for host policy.

### 2. The living flow — follow the signal

![Animated ProtocolAi flow from candidate language through encoding, validation, resolution, and host decision.](images/protocol-ai-living-flow.svg)

**Teaches:** encoding, validation, and resolution are different operations. The moving signal is illustrative; the C# methods are the executable behavior.

### 3. Three outcomes — do not confuse the cases

![Animated comparison of a known value, unknown literal, and invalid reference.](images/protocol-ai-three-outcomes.svg)

- **Known value:** can map to an application-owned reference.
- **Unknown value:** remains literal data until the host decides what to do.
- **Invalid reference:** violates the protocol contract and should not be resolved as a valid identity.

### 4. The interactive playground — touch the actual behavior

From the repository root, run:

    dotnet run --project samples/ProtocolAi.Playground/ProtocolAi.Playground.csproj

Open the local URL printed by the .NET host. The playground uses the real ProtocolAi library. Its candidate responses are fixtures, not live LLM calls; FSM_API drives the sample's visible state progression. The /hello page is a separate size-aware SVG experiment.

## What makes an image “living”?

A living image can use SVG animation to make time, movement, branching, or changing relationships visible. Here, animation is a teaching aid, not a source of truth.

Every visual should have:
- a specific learning objective;
- meaningful labels and a text explanation;
- enough contrast to read at ordinary README scale;
- a reduced-motion path;
- an explicit distinction between illustrative animation and executed library behavior;
- a route to the relevant source, test, or mathematical claim.

An animation that merely glows is decoration. An animation that lets a reader follow a value through a boundary is documentation.

## SVG versus interactive UI

| Layer | What it does | What it does not claim |
|---|---|---|
| Animated SVG in GitHub | Shows a concept unfolding over time | Does not execute the C# library |
| Local playground | Calls ProtocolAi and exposes state/payload results | Does not call a live LLM or measure hallucination rates |
| ProtocolAi library | Defines, encodes, validates, and resolves vocabulary identities | Does not own authorization or command execution |
| Host / interpreter | Applies domain policy and performs approved effects | Must not treat a valid ID alone as permission |

## Next experiments

These are directions, not claims of current implementation:

1. **Inspectable geometry:** select a stage and see the matching explanation, formula, and C# method.
2. **One concept, several views:** keep the same semantic model visible in the README SVG, interactive playground, and mathematical document.
3. **Data-led animation:** animate actual sample state and payload in the playground rather than duplicating behavior as hard-coded “proof” inside an SVG.
4. **Reusable Workshop patterns:** compare GUI.SVG, Renderer, and Visualize, then adopt only patterns that improve clarity without adding dependencies to ProtocolAi.
5. **Accessible by default:** retain text alternatives, reduced-motion support, and static meaning when animation is unavailable.

## The principle

**Make the boundary visible. Make the implementation inspectable. Make the claim smaller than the evidence—not larger.**
