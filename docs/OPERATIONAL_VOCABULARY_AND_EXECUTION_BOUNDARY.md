<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" />
  </a>
</p>

# Operational Vocabulary and Execution Boundary

## Decision

The existing `ProtocolBuilder` and immutable `ProtocolDefinition` are sufficient for application-owned operational vocabularies such as button names, intent names, capability names, and command identities.

Do not add a second operational-domain, capability-registry, handler, or execution abstraction to ProtocolAi for this use case. A new abstraction is justified only if multiple independent consumers demonstrate a concrete contract that the existing vocabulary model cannot express.

## Minimal vocabulary

The issue #2 example:

```text
Buttons
  A
```

is represented directly:

```csharp
using TheSingularityWorkshop.ProtocolAi;

var buttons = new ProtocolBuilder(5000, "Buttons")
    .Define(5001, "A", "A")
    .Build();

var buttonA = buttons.Reference("A");
var resolvedName = buttons.Decode(buttonA); // "A"
var description = buttons.Describe();
```

The protocol ID and symbol ID are allocated and owned by the application. The name groups the vocabulary; each symbol provides a stable integer-backed identity and its application-defined value. IDs are not globally allocated by ProtocolAi.

The same shape works for other vocabularies:

- `Buttons` — `A`, `B`, `Start`;
- `WorkshopIntents` — `open.workshop`, `resume.experience`;
- `DeviceCapabilities` — `camera.capture`, `display.present`;
- `CommandKinds` — stable command identities, without defining command execution.

## Definition lifecycle

1. The application allocates a protocol ID and symbol IDs.
2. The application calls `ProtocolBuilder.Define` for each known name/value pair.
3. `Build` creates an immutable `ProtocolDefinition` snapshot.
4. Consumers carry `ProtocolReference` values or encode/decode through the owning definition.
5. A changed vocabulary is built as a candidate and compared with the accepted definition before the host adopts it.

A built definition is not a mutable global registry. Unknown values remain literals; ProtocolAi does not mint IDs or persist registrations. The host owns ID allocation, persistence, version selection, and the policy for proposing or accepting new vocabulary entries.

## Handler and execution boundary

A protocol symbol identifies meaning. It is not a delegate, permission, executable capability, or instruction to run.

When a host wants a symbol to select an operation, the host may maintain its own mapping from a validated `ProtocolReference` to a host-owned handler or adapter. That mapping, its lifetime, and all invocation rules remain outside ProtocolAi.

| Concern | Owner |
|---|---|
| Stable protocol and symbol identity | ProtocolAi |
| Vocabulary definition, immutable snapshot, description, resolution, structural compatibility | ProtocolAi |
| Handler registration and adapter lifetime | Host / FSM_Foundry |
| Run status, sequencing, cancellation, retries, and workflow outcomes | Host / FSM_Foundry |
| Authentication, authorization, approval, and execution policy | Host |
| Optional presentation of a workflow | GUI, only when the host chooses it |

ProtocolAi remains usable headlessly. It has no dependency on GUI, Foundry, provider SDKs, or application orchestration. It does not expose a handler registry or workflow lifecycle API.

## Relationship to event envelopes

The event envelope is also a contract, not an execution engine. ProtocolAi validates message structure, event-kind/payload pairing, correlation, and declared provenance shape. It does not authenticate producers, verify external evidence, persist inbox/outbox state, deliver or retry messages, deduplicate at rest, authorize transitions, or execute commands.

Hosts implement those behaviors against the semantics in [Event Envelopes](EVENT_ENVELOPES.md). Cross-repository proof belongs in the owning host repositories; a passing ProtocolAi test suite proves only ProtocolAi's local contract.

## Verification

The implementation is exercised by:

- `ProtocolDefinitionTests`: immutable definitions, stable references, encode/decode, deterministic description, duplicate rejection, and compatibility behavior.
- `ProtocolEventContractConformanceTests`: typed v1 event payload/envelope round-trips and event-kind pairing.
- `ProtocolEventEnvelopeTests` and `ProtocolEventPayloadTests`: malformed, incomplete, mismatched, unsupported, and unsafe wire-contract cases.

The `Buttons/A` example has a focused test so the operational-vocabulary decision remains executable rather than merely descriptive.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
