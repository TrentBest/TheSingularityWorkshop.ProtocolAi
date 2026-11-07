<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Branch Reconciliation Ledger

**Review date:** 2026-11-07 UTC  
**Audited development head:** `473f315b5790bab434dc8e32ae9170aff170d925`  
**Disposition:** Audit and documentation refresh only. No branch deletion, PR closure, merge, reset, cherry-pick, release, deployment, or package publication has been performed.

## Branch inventory

The Sith policy requires exactly `master` and `development`. GitHub currently reports five branches:

| Branch | Head SHA | Ahead / behind development | Disposition |
|---|---|---:|---|
| `development` | `473f315b5790bab434dc8e32ae9170aff170d925` | Baseline | Active source branch. |
| `master` | `5d563d7e5f4c8bfcbc9a86e3d16107219be515c3` | 10 / 336 | Released baseline; preserve. GitHub branch metadata currently reports protection disabled. |
| `docs/development-build-badge` | `0fc2ea425d939b393bf8ceb3995d5ac7b77b9f30` | 1 / 145 | PR #9 is open; its sole README badge change is already present in development. Pending PR disposition. |
| `work/event-contract-conformance` | `e8dd3f2fa91c7a36799fea606aa397600a9cdd8e` | 2 / 27 | Its only net changed file is byte-identical to development. Two unique commits reviewed; no open PR found. Candidate for owner deletion, but **not certified READY FOR OWNER DELETION** until workflow/consumer-reference checks are completed. |
| `work/protocol-compatibility-audit` | `14ea58c772ea1b728190e38184bd216f28e5a00e` | 9 / 145 | PR #8 is open. Compatibility API/report, tests, README/consumer guidance, and theory material are represented in development. Pending PR disposition. |

Comparison direction is commits unique to the named branch / commits by which it trails development. Merge bases:
- `master`: `860fed18a9028793a0709a8158f6c7549a88913c`
- `docs/development-build-badge`: `20ea8e6406dacb056edc9f3a9ff2a8c8352b30a2`
- `work/event-contract-conformance`: `6e1a46f29771649cc230247e882a71e2a7673a72`
- `work/protocol-compatibility-audit`: `20ea8e6406dacb056edc9f3a9ff2a8c8352b30a2`

## Pull request disposition

- [PR #8 — Add structural protocol compatibility checks](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/pull/8): open, not merged, GitHub reports not mergeable. Its original feature is represented on development: `ProtocolCompatibilityReport.cs` has blob `9d78d4880311e279767061979c56f84543fcd61f`; `ProtocolDefinition.cs`, `ProtocolDefinitionTests.cs`, README, consuming guide, and theory documentation also contain the compatibility work. The development versions evolved independently from the PR branch. Do not merge the stale PR. Owner must decide whether to close it before its branch enters the deletion batch.
- [PR #9 — docs: point ProtocolAi build badge at development](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/pull/9): open, not merged, GitHub reports not mergeable. Its one-line build-badge change is already present in current development. The branch is superseded. Owner must decide whether to close it before its branch enters the deletion batch.
- No PR has been closed and no branch has been deleted.

## Content verification

### Event-contract conformance branch

The branch has two unique commits:
- `bf4f90bdaa7c140fb6196fc63fa40064639c5746` — `test: cover every typed event envelope contract`
- `e8dd3f2fa91c7a36799fea606aa397600a9cdd8e` — `test: compare typed event payload round trips`

The compare endpoint reports one net changed file: `tests/ProtocolAi.Tests/ProtocolEventContractConformanceTests.cs`. Its blob is identical on development and the work branch (`77cb24feedd1c01615cb14dd4646e1c4eba6a8e0)). No open PR was found for this branch. The test is preserved in development, but branch-reference checks across workflows and consumers have not been conclusively completed. Therefore this branch remains a deletion candidate, **not yet certified READY FOR OWNER DELETION**.

### Compatibility audit branch

The PR's compatibility report/API and associated tests and documentation are present in development. The development implementation supersedes the old PR branch; retain the branch only until PR #8 is explicitly resolved.

### Development build badge branch

The PR's exact build badge line targeting `development` is already in the current development README. Retain the branch only until PR #9 is explicitly resolved.

## Master-only history and Sith policy evidence

The ten commits unique to master were reviewed at a high level. The master-only history contains the live Sith policy badge/evidence and related workflow history. The live `assets/sith-policy.svg` and `assets/sith-policy.json` remain on master, which is the stable badge/evidence host referenced by the development README.

Development contains a newer Sith workflow implementation. Preserve master as the released baseline. The histories are diverged; do not merge, reset, force-push, or rewrite master as a cleanup shortcut. GitHub currently reports the master branch as not protected; this ledger does not change branch protection.

## CI and release safeguards

- The latest recorded full development build/test/package workflow, [run #42342177808](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/42342177808), passed at `473f315b5790bab434dc8e32ae9170aff170d925` on 2026-11-07. Restore, build, tests/coverage, pack, package-documentation verification, clean-consumer verification, and artifact uploads succeeded.
- The `publish_nuget` job was skipped. No package was published.
- Sith evaluation [run #42366423518](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/42366423518) failed closed on 2026-11-07. The master-hosted `assets/sith-policy.json` records five actual branches against the required two and reports `NON-COMPLIANT`.
- Keep NuGet publication disabled unless the remote branch inventory is exactly `master` + `development`, the publish gate independently verifies current Sith evidence, and the repository owner explicitly authorizes publication.

## Owner deletion readiness

- **READY FOR OWNER DELETION:** None certified in this refresh.
- **Deletion candidate pending final reference audit:** `TrentBest/TheSingularityWorkshop.ProtocolAi:work/event-contract-conformance`. Its unique test content is present byte-for-byte in development, but workflow/consumer-reference checks remain incomplete. Do not delete until it is certified and included in the owner's planned batch.
- **Pending PR disposition:** `TrentBest/TheSingularityWorkshop.ProtocolAi:docs/development-build-badge` (PR #9) and `TrentBest/TheSingularityWorkshop.ProtocolAi:work/protocol-compatibility-audit` (PR #8). Their changes are superseded, but their PRs remain open. Do not delete while those PRs remain unresolved.
- **Preserve:** `TrentBest/TheSingularityWorkshop.ProtocolAi:master` and `TrentBest/TheSingularityWorkshop.ProtocolAi:development`.

This ledger is evidence and recommendation, not authorization to delete branches, close PRs, merge histories, publish packages, or release.

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
