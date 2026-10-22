# Branch Reconciliation Ledger

**Review date:** 2026-10-22 UTC  
**Scope:** `master` versus `development`  
**Disposition:** Audit only. No merge, cherry-pick, reset, or branch deletion has been performed.

## Current branch evidence

- `development`: `7b705da7eea1a190680aaf65853c386731a66cd3`
- `master`: `d5f061b3b2e42e61ec391e2db98fc531f502ba1b`
- GitHub comparison: `development` is 161 commits ahead and 3 commits behind `master`.
- No open pull requests were reported during this review.
- Latest CI for the current development head: [run #39717085352](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/39717085352) — passed.

## Work found only on master

Master has three commits not present on development:

| Commit | Change | Reconciliation treatment |
|---|---|---|
| `6c510129926c` | Add shared Workshop moniker to README | Preserve the moniker and accessible image alternative when polishing the development README. |
| `334cfd7f8fdc` | Standardize README identity heading | Preserve the standard identity heading. |
| `d5f061b3b2e4` | Link shared Workshop documentation standard | Preserve the link, pointing at the canonical FSM_COS `master` documentation standard. |

The master README is a longer, 353-line historical version; development's README is 178 lines and links to separate detailed guides. Do not replace the development documentation with the older README wholesale. Compare its unique explanations and examples before any final consolidation.

## Release-critical differences

- Project version: development `0.1.0-alpha.3`; master `0.1.0-alpha.2`.
- Development has the dependency-free JSON payload transport and related guide/tests; keep that development implementation and its test coverage.
- Both workflows currently hard-disable NuGet publication. Development pins NuGet Trusted Publishing's action to an immutable commit SHA; master uses a mutable version tag. Preserve the safer development workflow.
- The project still suppresses CS1591 warnings. XML documentation and consumer-facing API coverage remain release-review items.

## Required before declaring branch reconciliation complete

- [ ] Compare master README's older examples and explanations against current source/tests and harvest any still-useful material into the appropriate guide.
- [ ] Confirm the moniker, identity heading, and canonical standard link are present and correct in the final development README.
- [ ] Review project metadata, package contents, CI workflow permissions, and publication gates on both branches.
- [ ] Re-run CI on the exact reconciled candidate.
- [ ] Obtain repository-owner approval before merging or deleting any branch.

**This ledger is not authorization to delete `master` or any other branch.**
