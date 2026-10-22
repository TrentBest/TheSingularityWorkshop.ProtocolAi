# ProtocolAi Release Readiness

## Current package status

The project file on development declares version **0.1.0-alpha.3**. Treat the repository as pre-1.0 until a release decision is made. This document does not publish a package or change the version.

ProtocolAi is small, but a small codebase is not automatically a stable contract. Moving to 1.0.0 means consumers can reasonably depend on the public API and its semantics.

## Latest verified candidate — 2026-10-22 UTC

Candidate `development` commit: `751c4776689f11465b1a070df72ac2e7dd042257`. [Workflow run #39716862629](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/39716862629) passed for this exact commit, including restore, Release build, tests/coverage, package creation, documentation-path verification, and a clean consumer smoke test. The publication job was skipped; the workflow gate is deliberately hard-disabled. This status update is documentation-only and its resulting commit must also pass CI.

## Visual companion

![Animated ProtocolAi lifecycle and known, literal, invalid outcomes.](images/protocol-ai-domain-lifecycle.svg)

Release readiness is not just whether the happy path works: reviewers must verify each outcome, transport boundary, host-policy separation, and the claims made by the documentation.

## What is already a strong foundation

- Provider-neutral .NET library; no model SDK is required.
- Application-owned vocabulary with nonzero protocol and symbol identities.
- Duplicate symbol IDs, names, and values are rejected at definition build time.
- Immutable definition snapshot after build.
- Known values encode as symbol references; unknown values remain explicit literals.
- Payload validation checks protocol scope and rejects undefined symbol IDs.
- Resolution preserves ordering and literals.
- Deterministic self-description.
- JSON transport has explicit reference-versus-literal representation.
- Tests cover core mapping, duplicate definitions, invalid references, resolution, and JSON round trips.
- README, guides, theory, mathematical foundations, and animated SVGs explain the boundaries.

These are meaningful foundations, not proof of release readiness by themselves.

## Before removing alpha

### Correctness and API contract

- [x] Run full solution restore, Release build, and all tests in a clean environment (workflow run #39716469730).
- [ ] Verify the playground builds and run the visible flows in a browser.
- [ ] Review every public API for argument validation, exception types, XML docs, and naming consistency.
- [ ] Confirm malformed, cross-protocol, undefined, and empty-input behavior is intentional and tested.
- [ ] Review JSON compatibility: unknown fields, malformed shapes, numeric overflow, and compatibility expectations.
- [ ] Confirm no mutable collection or builder mutation can change a previously built definition.
- [x] The solution build includes the quick-start and playground projects; the clean consumer smoke test passed in workflow run #39716469730. Browser-level visual review remains outstanding.

### Package and consumer experience

- [ ] Review target frameworks against intended consumers; the current project declares .NET 8 only.
- [ ] Inspect the generated NuGet package, including README, license, symbols, and intended documentation assets.
- [ ] Verify package metadata and release notes describe current behavior rather than aspirations.
- [ ] Check every install snippet and package-version reference for staleness.
- [ ] Decide the compatibility/versioning policy for protocol IDs and symbol meanings.
- [x] Build, tests, package creation, packaged documentation-path verification, and clean consumer smoke test passed in workflow run #39716469730.
- [x] NuGet publication is explicitly disabled in the workflow pending the owner's authorization.

### Evidence and claims

- [ ] Keep mathematical guarantees aligned with implementation and tests.
- [ ] Do not claim fewer hallucinations, improved task success, or token savings without controlled measurements.
- [ ] State clearly that valid identity is not authorization and integer output alone is not safe execution.
- [ ] Keep animated diagrams labelled as illustrative unless driven by live sample state.

## Release decision

**Recommendation: not yet enough evidence to remove the alpha label.** The conceptual scope is focused and the implementation is approachable; the missing confidence is release verification and explicit API/compatibility review, not a need to turn ProtocolAi into a large framework.

1. Close the checklist, starting with clean CI and browser verification.
2. Make behavior-changing API corrections before stabilizing.
3. Decide supported target frameworks and the identity/versioning contract.
4. If the checks pass and the API is intentionally stable, prepare a reviewed 1.0.0 package.
5. Publish only after explicit authorization.

No item should be marked complete merely because a workflow exists or a document says it should work.
