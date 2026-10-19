# ProtocolAi — Implementation and Documentation Roadmap

Purpose: living, evidence-based checklist for bringing implementation, tests, examples, and documentation to Workshop standard.

Status discipline: checked means verified in source/tests or by reproducible command. Design intention is not implementation evidence. Do not publish merely because a checklist section is complete.

## Release readiness
- **Current readiness:** 65% — CI passed build, tests, coverage, pack, documentation-path verification, clean-consumer installation, and artifact upload. The packed alpha package was inspected: 18 Markdown guides and 10 images are included under `docs/` without duplicated path prefixes, alongside README, license, assembly, and XML documentation. Remaining work includes complete public-contract and dependency-boundary coverage, validation-policy decisions, and final documentation/version consistency review.
- **Release-ready means:** coherent public contract, tested behavior, buildable examples, source-aligned docs, verified CI/package metadata, and explicit limitations.
- Publication authorization is separate from readiness.

## 00 — Definition of done
- [ ] Audit source tree, public API, tests, examples, project files, workflows, package metadata, and current NuGet artifact.
- [ ] Map every public type/member to purpose, invariants, validation, exceptions, and tests.
- [ ] Mark roadmap items implemented, missing, intentionally out of scope, or awaiting a decision.
- [ ] Ensure README claims and diagrams match source behavior.
- [ ] Label examples verified, source-shaped, or conceptual.
- [ ] Run restore, Release build, tests, coverage workflow, pack, and quickstart.
- [ ] Verify links, badges, package versions, license, and branch references.
- [ ] Keep master and development as the only maintained branches.
- [ ] Keep provider SDKs, credentials, network transport, and model execution outside this core.

## 01 — Product contract and conceptual model
- [ ] Define ProtocolAi as application-owned semantic identity and deterministic vocabulary resolution.
- [ ] Distinguish semantic identity from syntax/schema, grammar, authorization, and execution.
- [ ] Define protocol, symbol, identity, name, value, reference, literal, payload, encode, decode, resolve, validate, and describe.
- [ ] State ownership invariant: application owns domain meaning; ProtocolAi owns protocol identity and deterministic mapping.
- [ ] State the determinism boundary; do not claim model or host interaction is deterministic.
- [ ] Document unknown literals separately from malformed or foreign references.
- [ ] State that integer identity is not authorization or proof of semantic correctness.
- [ ] Maintain non-goals: inference, prompt management, provider SDKs, transport, execution, persistence, ID allocation policy, and grammar compilation unless separately decided.

## 02 — Protocol definition and symbol identity
- [ ] Audit ProtocolBuilder, ProtocolDefinition, ProtocolSymbol, constructors, and public methods against source.
- [ ] Define valid protocol IDs, symbol IDs, names, and values; decide zero/negative/duplicate/blank/null behavior.
- [ ] Verify duplicate ID and duplicate name/value behavior; prevent ambiguity or specify it explicitly.
- [ ] Verify built definitions are immutable and builder mutation cannot affect them.
- [ ] Verify lookup and equality semantics for protocol and symbol identities.
- [ ] Document whether identity is scoped by protocol ID, definition instance, version, or combination.
- [ ] Specify culture, case-sensitivity, Unicode, whitespace, and normalization rules.
- [ ] Decide whether aliases are supported; otherwise document their absence.
- [ ] Define canonical symbol ordering independent of dictionary enumeration.
- [ ] Ensure failures provide actionable exceptions or structured validation results.

## 03 — Encode, decode, resolve, and payload validation
- [ ] Inventory ProtocolValue, ProtocolReference, ProtocolPayload, Encode, Decode, Resolve, and Validate.
- [ ] Specify literal preservation, ordering, null handling, empty payloads, and repeated values.
- [ ] Test known values, unknown literals, duplicate inputs, empty strings, Unicode, and mixed sequences.
- [ ] Validate protocol identity before resolving references.
- [ ] Reject undefined symbols and references scoped to another protocol.
- [ ] Ensure validation never partially mutates state or silently rewrites payloads.
- [ ] Document fail-fast versus aggregate diagnostics.
- [ ] Define stable diagnostics for invalid IDs, missing symbols, malformed payloads, and cross-protocol references.
- [ ] Test promised round trips: encode → validate → resolve/decode.
- [ ] Ensure malformed external data cannot bypass validation via constructors or alternate paths.
- [ ] Define payload immutability and collection ownership.
- [ ] Test deterministic formatting and avoid hash-order dependence.

## 04 — Interoperability and representation
- [ ] Specify canonical in-memory contracts separately from any wire representation.
- [ ] Label payload formatting implementation-specific unless stable wire compatibility is explicitly promised.
- [ ] Define versioning before promising long-term wire compatibility.
- [ ] Define serialization requirements: ordering, escaping, encoding, malformed input, and round trips.
- [ ] Decide whether serialization belongs here or in FSM_Serialization; avoid duplicate responsibilities without a decision.
- [ ] Define protocol comparison/fingerprinting if needed, without conflating ID equality and definition equality.
- [ ] Define compatibility rules; mark negotiation/version migration future work until implemented.
- [ ] Ensure GrammarAi can reference protocol identities without owning or copying domain meaning.
- [ ] Verify AnyApp, FSM_UserIO, and ecosystem integration links against source and branch paths.

## 05 — Robustness, security, and resource behavior
- [ ] Treat payloads and model output as untrusted input.
- [ ] Ensure references never grant authorization or trigger side effects.
- [ ] Test large payloads/vocabularies and document limits.
- [ ] Check integer overflow, pathological strings, duplicate-heavy input, and malformed references.
- [ ] Confirm no network access, provider secret handling, dynamic code execution, or hidden global mutable state.
- [ ] Document thread-safety only where source and tests establish it.
- [ ] Avoid performance claims without reproducible measurements.
- [ ] Add property/fuzz-style tests for arbitrary strings and reference combinations where practical.

## 06 — Testing strategy
- [ ] Map every public member and validation rule to tests.
- [ ] Add positive, negative, boundary, and regression tests for builders, definitions, symbols, references, payloads, and resolution.
- [ ] Test deterministic descriptions and output order.
- [ ] Test round trips and cross-protocol rejection.
- [ ] Test immutability and builder isolation.
- [ ] Test culture invariance where behavior should be culture-independent.
- [ ] Ensure tests do not depend on order or machine-local settings.
- [ ] Verify coverage includes production assembly and meaningful public behavior.
- [ ] Run Release tests and record actual results; badges are not proof.

## 07 — Examples and living demonstrations
- [ ] Verify QuickStart against current source and package version.
- [ ] Provide minimal first-minute demo with exact expected output.
- [ ] Demonstrate vocabulary definition, known-value encoding, literal preservation, decode/resolve, validation failures, and mixed payloads.
- [ ] Show multi-protocol use only if supported; otherwise label as future concept.
- [ ] Add a source-aligned protocol lifecycle diagram.
- [ ] Distinguish implemented paths from future paths in every visual.
- [ ] Keep assets version-controlled, accessible, legible in dark/light themes, and captioned.
- [ ] Add troubleshooting based on real errors.

## 08 — Documentation set
- [ ] Align README with Workshop sequence: ✳️ 00 identity, money-shot visual, 🟦 01 What, 🟣 02 Why, 🩵 03 How, 🟢 04 See it in a minute, 🟪 05 docs/theory.
- [ ] Keep README a concise map.
- [ ] Audit CONSUMING.md, EXAMPLES.md, THEORY.md, REFLECTION.md, AI_EXCHANGE.md, ECOSYSTEM_INTEGRATION.md, and ECOSYSTEM_USAGE_MAP.md for accuracy and overlap.
- [ ] Add/maintain architecture and API contract references mapping public members to guarantees and tests.
- [ ] Add development/verification guide and reader-oriented documentation index.
- [ ] Explain known identity vs unknown literal vs invalid reference for non-coders, including limits of the analogy.
- [ ] Separate current source, released package, intended architecture, and future work.
- [ ] Keep hallucination-reduction claims hypothetical absent controlled empirical evidence.
- [ ] Verify links, paths, commands, frameworks, badges, package versions, and footer.

## 09 — Package, CI, and release gate
- [ ] Verify target frameworks, package ID/version, dependencies, nullable settings, analyzers, XML docs, license, repository metadata, and packed README.
- [ ] Verify development dependency/artifact behavior and release dependency behavior.
- [ ] Verify branch triggers, permissions, coverage upload, trusted publishing, and publish gates.
- [ ] Prove ordinary development CI cannot accidentally publish.
- [ ] Run restore, Release build, tests, coverage, pack, and package-content inspection.
- [ ] Test packed package from a clean consumer project.
- [ ] Distinguish latest published version from source version.
- [ ] Verify release notes explain contract changes and breaking changes.
- [ ] Mark 100% only with evidence for every release criterion; publication is a separate explicit action.

## 10 — Decisions to resolve, not silently assume
- [ ] Are protocol and symbol IDs caller-assigned, or is a separate allocator warranted?
- [ ] Is symbol identity stable across protocol revisions, and what compatibility is promised?
- [ ] Does canonical serialization belong here or in a separate layer?
- [ ] Should validation aggregate errors or stop at first failure?
- [ ] Are aliases, namespaces, localized labels, or multiple textual forms required?
- [ ] What limits should apply to payload length and vocabulary size?
- [ ] What minimum contract is needed for GrammarAi and clipboard-based exchange?
- [ ] Which interoperability claims can be proved by a working consumer?

## Progress log
| Date | Change | Evidence | Readiness |
|---|---|---|---|
| 2026-10-18 | Initial roadmap created after reviewing README, theory, and shared documentation standard. Full source/test/CI audit remains outstanding. | [README](../README.md) · [Theory](THEORY.md) · [Documentation Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/master/DOCUMENTATION_STANDARD.md) | 35% |
| 2026-10-19 | Began source-level audit of `ProtocolDefinition`, payload JSON transport, project metadata, test suites, and CI. Confirmed existing unit coverage for encoding, ownership, duplicate definitions, round-trip JSON, and rejection paths. | [ProtocolDefinition.cs](../ProtocolDefinition.cs) · [JSON tests](../tests/ProtocolAi.Tests/ProtocolPayloadJsonTests.cs) · [CI workflow](../.github/workflows/build.yml) | 38% |
| 2026-10-19 | Fixed cross-line/control-character escaping in deterministic descriptions and rejected default/uninitialized `ProtocolValue` instances in direct resolution and payload validation. CI run 190 passed build, tests, coverage upload, pack, and artifact upload. Inspection of run 191 package exposed missing packaged Markdown guides and duplicated `docs/images` path prefixes; updated pack metadata to include docs and preserve asset paths. Rebuild and artifact inspection remain pending. | [ProtocolDefinition.cs](../ProtocolDefinition.cs) · [Protocol tests](../tests/ProtocolAi.Tests/ProtocolDefinitionTests.cs) · [Workflow run 191](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/39185503961) · [Project file](../TheSingularityWorkshop.ProtocolAi.csproj) | 38% |
| 2026-10-19 | Added a CI package-content assertion after pack: fail if no Markdown guide is present under `docs/` or if the duplicated `docs/images/docs/images/` prefix returns. The latest workflow edit is committed; successful execution and rebuilt artifact inspection are still required before readiness changes. | [Build workflow](../.github/workflows/build.yml) · [Project file](../TheSingularityWorkshop.ProtocolAi.csproj) | 38% |
| 2026-10-19 | Pinned all six external GitHub Actions references to upstream-verified immutable commit SHAs. The resulting development workflow run passed build, tests, coverage upload, pack, packaged-documentation-path checks, clean-consumer installation, and artifact upload; the NuGet publish job was skipped. Release authorization remains separate. | [Pinned workflow commit](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/commit/d63e022e5ead7099e2eb8562e76d320d02a093e6) · [CI run #39334899306](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/39334899306) · [Build workflow](../.github/workflows/build.yml) | 65% |
