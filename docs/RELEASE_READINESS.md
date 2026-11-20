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

The latest previously verified development candidate is [`fb4e008935ad6c8bac9301d3b92292aa8e49160d`](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/commit/fb4e008935ad6c8bac9301d3b92292aa8e49160d), with exact-head [workflow run #43799124521](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/43799124521).

That run passed the development build/test job and live Sith compliance verification. Restore, Release build, tests/coverage, Codecov upload, package creation, packaged-documentation-path verification, clean-consumer installation/build, and artifact uploads succeeded. The NuGet publication job was skipped. This evidence applies only to that exact SHA; every later commit requires a new exact-head run.

## Repository integrity

The live branch inventory contains exactly two branches: `master` and `development`. At the audit snapshot, the compare endpoint reported development 450 commits ahead and 25 behind master. The current branch ledger records the preservation rationale and release-lineage evidence.

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
- [ ] Resolve missing XML documentation on public constructors, properties, methods, and factories; do not suppress missing-member warnings as a substitute for API documentation.
- [ ] Complete the focused repository-name validation review for provenance and downstream notifications. Existing validation checks slash shape but does not reject embedded whitespace in both segments. Add narrow tests and validation without rejecting valid GitHub owner/repository names.
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
