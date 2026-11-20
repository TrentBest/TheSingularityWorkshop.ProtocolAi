<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" />
  </a>
</p>

# ProtocolAi Release Readiness

> **Purpose:** evidence ledger for the next package candidate. This document does not authorize publication. Future package publication will be orchestrated through CiAi when the owner directs that workflow.

## Current package state

- NuGet lists `TheSingularityWorkshop.ProtocolAi` **0.1.0-alpha.2** as a published prerelease.
- The current `development` project declares **0.1.0-alpha.3**.
- The repository targets **.NET 10** (`net10.0`); the playground also targets `net10.0` and requires the .NET 10 SDK.
- The alpha.2 release-named commit `3a0bcc6f7b6c010d1e9598bd09f17ffd6c4e02de` is an ancestor of both canonical branches. This proves the release lineage is retained, but does **not** prove that this exact commit produced the published package. The package's source SHA remains to be established from authoritative package/build provenance.
- Preserve `master` as the published-package history until that exact source SHA is verified. Do not reconcile branches merely to reduce divergence.

## Latest verified candidate evidence

The current verified development candidate is [`f70a372e867b5ab445142f211453596cfffc5106`](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/commit/f70a372e867b5ab445142f211453596cfffc5106), with exact-head [build/test/package workflow #44678566065](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/44678566065) and [Sith policy workflow #44678566059](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/44678566059), both completed successfully on 2026-11-20 UTC.

The build workflow's `Verify live Sith compliance` and `build-and-test` jobs both passed. The latter completed .NET restore/build, tests with coverage, Codecov upload, package creation, packaged-documentation-path verification, clean-consumer installation/build, and artifact uploads. The separate `publish_nuget` job was skipped. The Sith workflow's `Evaluate permanent branch policy` job passed. The package artifact is `protocol-ai-nuget-package`, artifact ID `14603556235`, uploaded for this exact SHA; GitHub reports its ZIP artifact digest as `sha256:247d92c2b933f0b41d7bb6f68eecb985edf1dc4f34c98d0a97c69ea96025b3c2`. This is the Actions artifact archive digest, not the inner `.nupkg` digest. The package artifact has been produced and the clean-consumer check passed, but the current artifact contents have not yet been independently downloaded and inspected. Public XML documentation remains compiler-checked with the `CS1591` suppression removed.

## Packed artifact inspection

The CI artifact (artifact ID `14593068994`) from [workflow run #44656283163](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/44656283163) was downloaded and inspected directly.

- Package: `TheSingularityWorkshop.ProtocolAi.0.1.0-alpha.3.nupkg`
- NuGet repository metadata: `commit="cc005f69d2aec817267ea713bad184dc36a4283b"`
- Target framework: `net10.0`
- Dependencies: none declared
- Package contents: assembly, generated XML API docs, README, MIT license, Markdown guides, and intended SVG/raster documentation assets
- License entry: `LICENSE.txt`, with `Copyright (c) 2026 Trent Best`
- Package SHA-256: `8de4705891ac0e693f407cce6597e88b9cd723d195573c7ed8bac7fe542687aa`

The package contains the current Domain Integration Decisions guide, expanded ecosystem usage map, and fixed v1 event-kind scope guidance. The artifact is a CI build output only; it was not pushed to NuGet. A separate symbol/source package is not currently present and remains a package-quality decision.

## Repository integrity

The live branch inventory contains exactly two branches: `master` (`d63d0edd725219eb549ee8878f2a9a70af26de27`) and `development` (`f70a372e867b5ab445142f211453596cfffc5106`). The current compare endpoint reports development 496 commits ahead and 25 behind master, with diverged histories. The current branch ledger records the preservation rationale and release-lineage evidence.

No branch rewrite, merge, release, deployment, or package publication is part of this readiness work. The NuGet publication path remains disabled in CI. Future publication is intended to be driven by CiAi after the owner authorizes the specific release operation.

## Strong foundations already implemented

- Application-owned protocol and symbol identities with nonzero IDs.
- Immutable protocol definitions built from validated symbols.
- Known values encode as symbol references; unknown values remain explicit literals.
- Payload validation checks protocol scope and rejects undefined references.
- Resolution preserves ordering and literal values.
- Dependency-free JSON payload transport with explicit reference/literal representation.
- Typed event envelope and payload contracts with event-kind binding, correlation, idempotency, provenance, and causal revision fields.
- Duplicate JSON properties and unsupported schema versions are rejected by the relevant readers.
- Structural compatibility reporting distinguishes additive symbols from removed or redefined identities.
- Documentation describes the separation between identity, grammar, command semantics, provider communication, host policy, authorization, and execution.

These are implemented foundations, not proof that the public API is ready to leave alpha.

## Remaining verification and design work

### Public contract and correctness

- [ ] Map every public type/member to purpose, validation, exceptions, invariants, XML documentation, and tests.
- [x] Add XML summaries for public constructors, properties, methods, event kinds, and factories; remove the `CS1591` suppression so missing public-member documentation is no longer hidden.
- [x] Complete focused repository-name validation for provenance and downstream notifications. The shared validator rejects malformed owner/name shape, embedded whitespace, invalid characters, and invalid owner lengths while retaining valid punctuation in repository names; focused positive and negative tests cover both contracts.
- [ ] Verify immutability, builder isolation, culture-independent matching, ordinal/case-sensitive behavior, Unicode handling, and all documented resource limits.
- [ ] Decide permanent identity, ID reuse, semantic change, and definition-versioning policy before a stable contract.

### Serialization and event interoperability

- [ ] Keep the JSON wire contract, typed event-kind mapping, and compatibility rules aligned with tests and shared fixtures.
- [ ] Exercise real host-side producer/consumer paths in CommandAi, GrammarAi, and CiAi. ProtocolAi owns only the reusable contract and fixtures.
- [ ] Verify host-side authentication, durable inbox/outbox, delivery/retry, deduplication, replay, causal-gap handling, and authorization in the owning host repositories.
- [ ] Keep CiAi CODE RED / self-development priorities in front of generalized adoption work. ProtocolAi must not become a CI verdict engine or workflow runner.

### Samples, package, and documentation

- [ ] Browser-test the local playground's primary flow, invalid-reference path, reset/step behavior, and `/hello` route under .NET 10.
- [ ] Inspect the generated `.nupkg` and `.snupkg` contents, including README, license, XML docs, symbols, and intended documentation assets.
- [ ] Verify a clean consumer against the exact packed artifact and record package hash and test evidence.
- [ ] Verify all documentation links, badges, image/SVG paths, sample commands, package-version guidance, and cross-repository source references.
- [x] Replace the placeholder MIT copyright line with `Copyright (c) 2026 Trent Best`.
- [x] Align the playground README SDK requirement with its `net10.0` project target.

### Ecosystem fit

- [ ] Maintain a source-verified integration matrix distinguishing direct package use, source-pinned use, fixture-only conformance, indirect carrier boundaries, candidates, and deliberate non-applications.
- [ ] Confirm whether HeadlessAi needs ProtocolAi only at its host integration edge; do not add a mandatory dependency for simple HTTP request/response communication.
- [ ] Keep GrammarAi's qualified external reference seam lightweight; do not add a dependency unless it needs ProtocolAi behavior rather than the identity pair.
- [ ] Keep CommandAi's proposal/codec semantics in CommandAi; use ProtocolAi event contracts only where a real event boundary exists.
- [ ] Record explicit no-adoption decisions for repositories whose IDs are local implementation details or whose responsibilities are transport, rendering, physics, composition, or execution.

## Release decision

**Disposition: remain alpha; no publication action.** The next candidate must be judged from its exact source SHA, exact-head CI, package artifact, public API review, and cross-repository evidence. A completed checklist or green build is not publication authorization.

1. Finish source/API and provenance validation review.
2. Complete source-verified ecosystem mapping and the real host conformance work that is appropriate to this alpha.
3. Verify playground and package artifact on .NET 10.
4. Reconcile package provenance and branch preservation evidence.
5. Hand the verified candidate to the CiAi release workflow when the owner directs it.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>

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
