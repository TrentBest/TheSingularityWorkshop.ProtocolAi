<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# ProtocolAi problem domain

## The human problem

Software that exchanges structured information with an LLM often relies on words that can be misspelled, paraphrased, or interpreted inconsistently. A host needs a stable way to say which concept is meant without pretending the model itself is a source of authority.

ProtocolAi provides an application-owned vocabulary: stable numeric symbol identities, human-facing names, and associated values. It is a meaning/identity layer, not an inference engine.

## Domain model

- **Protocol definition** — an immutable, named vocabulary with a non-zero protocol identity.
- **Symbol** — a uniquely identified named entry with an application-owned value.
- **Reference** — a protocol-qualified symbol identity, so an integer is not mistaken for a globally unique name.
- **Value** — either a reference to a known symbol or an explicit literal.
- **Payload** — a protocol-qualified collection of values that can be validated and resolved against a definition.
- **Builder** — the authoring surface used to construct a definition before it is consumed.

## Responsibilities

ProtocolAi owns the stable identity and resolution of vocabulary symbols. A definition can describe itself deterministically, encode known values as references, preserve unknown values as literals, and validate that a payload belongs to the expected protocol and refers only to defined symbols.

ProtocolAi does not own model inference, prompts as an execution mechanism, endpoint transport, command syntax, authorization, application state, or side effects. A known symbol is a vocabulary fact, not a permission grant.

## Boundary with the other AI packages

- **GrammarAi** can reference protocol symbols when describing legal structure; it owns grammar rules, not vocabulary meaning.
- **CommandAi** can carry action/parameter identities in a proposal; it must not silently invent what those identities mean.
- **HeadlessAi** can exchange request/response content with a configured endpoint; transport does not establish semantic correctness.
- **The host** selects trusted definitions, versions, policies, and allowed operations.

These are responsibility boundaries, not a claim that every end-to-end adapter currently exists. Verify integration against the current source and tests before treating two packages as interoperable.

## Resource boundaries

Protocol definitions accept at most 4,096 symbols; payloads accept at most 65,536 values and 1,048,576 literal characters in aggregate. JSON payload input is capped at 8,388,608 UTF-16 characters and its array count is checked before wire records are materialized. Protocol and symbol names are limited to 256 UTF-16 characters, while vocabulary values and payload literals are limited to 16,384 UTF-16 characters. Builders and constructors enforce these limits before retaining the corresponding collections. These bounds protect representation and memory use; they do not establish semantic correctness or authorize a payload.

## Acceptance criteria

1. Protocol and symbol identities are explicit and non-zero.
2. A definition rejects duplicate symbol IDs, names, or values within its declared vocabulary.
3. A protocol reference retains both protocol identity and symbol identity.
4. Resolution rejects references to a different protocol or undefined symbols.
5. Unknown literal values can remain literal rather than being assigned invented identities.
6. Deterministic descriptions do not depend on model output or endpoint behavior.
7. The core package does not require a provider SDK, network client, GUI framework, or host execution API.

## Evidence and maturity

The statements above describe the current source model in ProtocolDefinition, ProtocolSymbol, ProtocolReference, ProtocolValue, ProtocolPayload, and ProtocolBuilder. Consult the [consuming guide](CONSUMING.md), [theory](THEORY.md), and [AI exchange guide](AI_EXCHANGE.md) for usage and integration context. This page does not claim that an LLM obeys the vocabulary or that a structurally valid payload is safe to act upon.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
