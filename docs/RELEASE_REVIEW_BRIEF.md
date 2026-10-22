# ProtocolAi — Release Review Brief

> Phone-first review | Snapshot: 2026-10-22 UTC

## The decision in one minute

ProtocolAi is the **WHAT / vocabulary-identity layer**. Applications define the symbols they own; values can refer to known identities or preserve unknown literal content. Definitions resolve references under explicit protocol identity rules. JSON payload transport is a wire representation—not execution, authorization, or proof that incoming data is safe.

**Current assessment: 75% ready.** This is a qualitative working estimate, not a measured score. The latest recorded source/documentation candidate passed its workflow. Package version remains 0.1.0-alpha.3; this is not release approval.

- Candidate commit: [8cb1ffa59c731d8cefd5b5af8cd98dd030589185](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/commit/8cb1ffa59c731d8cefd5b5af8cd98dd030589185)
- CI: [run #39726289235](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/39726289235)
- Target currently documented by the project: .NET 8.
- Publishing remains gated; a green build is not authorization.

## What is implemented

- Protocol-owned identity and symbol definitions.
- Integer-backed symbol identity and explicit literal values.
- Payloads carrying protocol identity and value collections.
- Definition lookup and resolution, including protocol-aware payload resolution.
- Dependency-free System.Text.Json serialization/deserialization.
- Tests for values, definitions, resolution, payload serialization, and malformed wire data.
- Consumer guide, examples, theory, ecosystem usage map, and visual gallery.

## Important resolution distinction

```text
Resolve(ProtocolValue)
  -> resolve symbol ID in this definition
  -> value has no protocol ID, so origin cannot be verified

Resolve(ProtocolPayload)
  -> check payload protocol identity
  -> resolve values against the matching definition
```

Use payload resolution when protocol ownership matters. Value-only resolution is a lower-level operation, not a cross-protocol security check.

## Wire-format contract

The JSON reader rejects duplicate properties recursively and explicitly rejects malformed symbol IDs and empty/whitespace literals at the JSON boundary. Unknown fields are ignored for forward compatibility. Consumers must still validate protocol identity, undefined references, payload size, and application semantics before acting.

Valid JSON is not a valid command; a resolved symbol is not an authorized operation. ProtocolAi describes application-owned meaning. The host decides whether and how that meaning is used.

## Release-critical review

1. **Public API:** argument validation, default struct values, exception types/parameters, XML docs, naming, and collection immutability.
2. **Cross-protocol behavior:** mismatched payload identity must be rejected; value-only resolution must remain clearly documented as unqualified.
3. **Wire compatibility:** malformed input, duplicate properties, unknown fields, numeric boundaries, payload-size/overflow policy, and stable round trips.
4. **Consumer proof:** compile examples, test a clean consumer project, inspect nupkg/snupkg contents, README, license, XML docs, metadata, and version.
5. **Distribution:** confirm target frameworks, release notes, package page/version, and Trusted Publishing configuration without changing it during audit.

## Five-minute review path

1. [README — mental model and first example](../README.md)
2. [Consuming ProtocolAi](CONSUMING.md)
3. [Protocol payload JSON contract](PROTOCOL_PAYLOAD_JSON.md)
4. [Examples](EXAMPLES.md)
5. [Ecosystem usage map](ECOSYSTEM_USAGE_MAP.md)
6. [Theory](THEORY.md)
7. [Release readiness](RELEASE_READINESS.md)
8. [Gallery and visual evidence](GALLERY.md)

## Recommendation

Keep ProtocolAi small and dependency-free. Resolve demonstrated contract or wire-format defects, verify the packed artifact and consumer experience, and keep publishing gated until explicit owner approval. GrammarAi can reference ProtocolAi identities but does not replace protocol validation.
