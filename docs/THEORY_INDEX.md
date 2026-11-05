<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# ProtocolAi — Theory and Learning Path

This index organizes ProtocolAi's domain theory as a modular learning path. Read it in order for a first introduction, or open any chapter independently. The theory explains why the boundary exists and how to reason about it; the API guides define exact member contracts, and runnable examples teach specific tasks.

## Learning path

| Order | Chapter | What it teaches | Evidence and limits |
|---|---|---|---|
| 00 | [The semantic identity boundary](THEORY.md) | The motivating problem, application-owned vocabularies, known identities versus unknown literals, local determinism, and the separation between identity, structure, policy, and execution. | Conceptual synthesis grounded in the current domain model. Claims about hallucination, token savings, latency, and model compliance remain hypotheses until measured. |
| 01 | [Mathematical foundations](MATHEMATICAL_FOUNDATIONS.md) | The qualified address space, deterministic encoding, reference validation, resolution, conditional execution boundary, and testable experimental claims. | Formal reasoning under stated assumptions; the host-side execution theorem is conditional on a correctly enforced gate. |
| 02 | [Problem domain](PROBLEM_DOMAIN.md) | The owned domain, vocabulary entities, responsibility boundaries, resource limits, and acceptance criteria. | Source-mapped to the core definition, symbol, reference, value, payload, and builder types. |
| 03 | [Protocol identity lifecycle and evolution](IDENTITY_LIFECYCLE.md) | Durable protocol-qualified addresses, immutable definition snapshots, exact matching, structural compatibility, and safe evolution practices. | Source-checked against `ProtocolDefinition`, `ProtocolBuilder`, and named compatibility/identity tests; semantic meaning and migration remain application responsibilities. |
| 04 | [Versioned event envelopes](EVENT_ENVELOPES.md) | Correlation, provenance, typed event payloads, schema compatibility, replay semantics, and causal gate transitions. | Typed payload construction/reading and contract conformance are implemented; durable inbox/outbox, replay, and cross-host interoperability remain host/integration responsibilities. |

## Implementation and verification map

| Concept | Implementation | Tests / contract |
|---|---|---|
| Vocabulary definition and symbol identity | `ProtocolDefinition.cs`, `ProtocolSymbol.cs`, `ProtocolBuilder.cs` | `ProtocolDefinitionTests.cs` |
| Qualified references and known/literal values | `ProtocolReference.cs`, `ProtocolValue.cs` | `ProtocolResolutionTests.cs`, `ProtocolValueTests.cs` |
| Payload representation and JSON | `ProtocolPayload.cs`, `ProtocolPayloadJson.cs` | `ProtocolPayloadJsonTests.cs` |
| Event envelope and typed payload mapping | `ProtocolEventEnvelope.cs`, `ProtocolEventPayloads.cs`, `ProtocolSourceEventPayloads.cs`, `ProtocolTypedEventEnvelope.cs` | `ProtocolEventEnvelopeTests.cs`, `ProtocolEventPayloadTests.cs`, `ProtocolEventContractConformanceTests.cs` |
| Structural compatibility | `ProtocolCompatibilityReport.cs`, `ProtocolDefinition.cs` | Compatibility tests in `ProtocolDefinitionTests.cs` |

The table names source files and test suites as navigation aids, not line-stable citations. Check the current `development` revision before relying on implementation details.

## How to use this path

1. Start with [The semantic identity boundary](THEORY.md) to understand the problem before the vocabulary.
2. Read [Mathematical foundations](MATHEMATICAL_FOUNDATIONS.md) to distinguish what follows from the model's assumptions from what still depends on a host.
3. Read [Problem domain](PROBLEM_DOMAIN.md) alongside the source and tests to see how the abstract model is represented.
4. Read [Protocol identity lifecycle and evolution](IDENTITY_LIFECYCLE.md) before changing or publishing vocabulary IDs.
5. Read [Versioned event envelopes](EVENT_ENVELOPES.md) for the separate event-contract domain; do not confuse event transport metadata with vocabulary identity.
6. Try the [beginner's guide](BEGINNERS_GUIDE.md) and [examples](EXAMPLES.md), then use the [C# guide](CSHARP_GUIDE.md) and [consuming guide](CONSUMING.md) for integration.

## Practice and assessment

- Why is a protocol-qualified symbol address safer to interpret than a bare integer?
- Give one example each of a known value, an unknown literal, and an undefined reference. Which one is invalid, and why?
- Construct a counterexample showing why a valid identity does not prove that the requested operation is authorized.
- What assumptions are required for deterministic encoding? What changes if two vocabulary entries match the same input?
- Which guarantees belong to ProtocolAi, and which require a host, provider adapter, grammar, or interpreter?
- Design an experiment that could test whether an integer vocabulary improves model accuracy or token use. Identify the baseline, controls, metrics, and evidence needed before making a performance claim.

## Scope and neighboring theory

ProtocolAi owns application-defined vocabulary semantics and stable protocol-qualified symbol identities. It does not own model/provider communication, grammar composition, command proposal semantics, authorization, or execution. See the [AI exchange guide](AI_EXCHANGE.md) for the provider-neutral exchange boundary and [ecosystem integration](ECOSYSTEM_INTEGRATION.md) for package relationships. Provider-specific transport theory belongs with the provider/host layer; grammar theory belongs with GrammarAi. This path links to those boundaries rather than duplicating their manuals.

## Editorial status

This is the initial modular index over the existing theory and contract documents. The two core theory chapters are independently readable; the domain and event-contract chapters provide implementation-specific extensions. Future chapters should be added as separate files with stable descriptive names, source/test mapping, evidence labels, limitations, practice prompts, and further reading. Do not expand this index into a second copy of the chapter content.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
