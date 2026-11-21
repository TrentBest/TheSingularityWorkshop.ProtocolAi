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
- The published alpha.2 package remains historical .NET 8 evidence. The active development candidate is being aligned to the ecosystem baseline: **.NET 10 (`net10.0`) only**. The playground remains `net10.0` and requires the .NET 10 SDK.
- The alpha.2 release-named commit `3a0bcc6f7b6c010d1e9598bd09f17ffd6c4e02de` is an ancestor of both canonical branches. This proves the release lineage is retained, but does **not** prove that this exact commit produced the published package. The package's source SHA remains to be established from authoritative package/build provenance.
- Preserve `master` as the published-package history until that exact source SHA is verified. Do not reconcile branches merely to reduce divergence.

## Target-framework alignment — .NET 10 development baseline

The published alpha.2 package targeted .NET 8; that published artifact is not modified by this repository change. The previous alpha.3 candidate temporarily dual-targeted `net8.0` and `net10.0` as a conservative compatibility measure. The owner clarified that the ecosystem is moving to .NET 10, so the active candidate should target `net10.0` only rather than carrying forward .NET 8 solely for the old prerelease. The prior dual-target build/test/package evidence below remains historical and is superseded as candidate evidence by the .NET-10-only change. New exact-head CI and package inspection must pass before this candidate is considered verified.

## Latest verified candidate evidence

The latest exact-head package-verified candidate is [`c3826982afe2bac6b064d70e137c21075586ee1a`](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/commit/c3826982afe2bac6b064d70e137c21075586ee1a), with [build/test/package workflow #44845893139](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/44845893139) and [Sith policy workflow #44845893149](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/44845893149), both completed successfully on 2026-11-21 UTC.

The build workflow's `Verify live Sith compliance` and `build-and-test` jobs both passed. The latter completed restore/build for `net8.0` and `net10.0`, the .NET 8 library test run, the full .NET 10 solution test run with coverage, Codecov upload, pack, documentation-path and target-framework verification, clean-consumer install/build for both frameworks, and artifact uploads. The separate `publish_nuget` job remains skipped. The separate Sith policy workflow's `Evaluate permanent branch policy` job also passed. The package and test artifact hashes and results are recorded below. Public XML documentation remains compiler-checked with the `CS1591` suppression removed.

## Packed artifact inspection

The package artifact `protocol-ai-nuget-package` (artifact ID `14682408982`) from [exact-head workflow run #44845893139](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/44845893139) was downloaded and inspected directly. Its embedded NuGet repository metadata identifies the exact source candidate `c3826982afe2bac6b064d70e137c21075586ee1a`.

- Package: `TheSingularityWorkshop.ProtocolAi.0.1.0-alpha.3.nupkg`
- Repository metadata commit: `c3826982afe2bac6b064d70e137c21075586ee1a`
- Target frameworks: `net8.0` and `net10.0`
- Dependencies: none declared for either target
- Package size: 2,506,267 bytes
- Inner `.nupkg` SHA-256: `659f3d32079664725ae9df98809529aa58b5743f7b8dc316fe0e75b901c7fc34`
- Actions ZIP archive SHA-256: `2c018d7e19730533f649e1993c55b83ace466d12920d7888002c3a198d945225`
- Package contents: both target assemblies and generated XML API docs, README, MIT license, Markdown guides, and SVG/raster documentation assets
- License entry: `LICENSE.txt`, with `Copyright (c) 2026 Trent Best`
- No separate `.snupkg` was generated; symbol-package support remains an explicit package-quality decision.

The test artifact is `protocol-ai-test-results` (artifact ID `14682518648`), with Actions ZIP SHA-256 `3244087bbd1425296de9c97bc4da37e3b2bab47bd69672f8abf847ccb8f3b0f2`. Its TRX files report **166/166 passed on .NET 8** and **166/166 passed on .NET 10**, with zero failures. The Cobertura report records 698/721 lines covered (96.8%) and 371/431 branches covered (86.1%) for the .NET 10 run.

The three packaged raster documentation images account for approximately 2.3 MB of the package. They are intentional documentation assets, not runtime dependencies. The package target groups, documentation paths, and clean-consumer installs/builds for both frameworks passed in the same exact-head workflow. The artifact is a CI build output only; it was not pushed to NuGet.

## Repository integrity

At the branch-audit baseline `c3826982afe2bac6b064d70e137c21075586ee1a`, the live branch inventory contained exactly two branches: `master` (`657b3980f1f234cfcd71d65f3aff3af213e632b1`) and `development` (`c3826982afe2bac6b064d70e137c21075586ee1a`). The compare endpoint reported development 508 commits ahead and 26 behind master, with diverged histories. The two-branch inventory was verified across both search pages. The current branch ledger records the preservation rationale and release-lineage evidence.

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
- [ ] Verify a clean .NET 10 consumer against the exact packed artifact and record package hash and test evidence.
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
3. Verify the library package and clean consumer on .NET 10; verify the playground on .NET 10.
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


## Superseding candidate — .NET 10 only

The dual-target alpha.3 candidate documented above is historical and no longer represents the active target-framework policy. The current development candidate is being changed to `net10.0` only across the library, tests, CI package verification, clean-consumer verification, and consuming documentation. The prior .NET 8 test and package results must not be presented as evidence for this candidate. Record the exact new commit, workflow runs, test counts, package target group, package hash, and clean-consumer result here only after those checks pass. NuGet publication remains disabled and unauthorized.
