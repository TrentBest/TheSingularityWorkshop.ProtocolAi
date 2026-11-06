<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Branch Reconciliation Ledger

**Review date:** 2026-11-06 UTC  
**Audited development head:** `b1355e0502fa7cc7210a0a922e27cf43a43e1e7c`  
**Disposition:** Audit and documentation refresh only. No branch deletion, PR closure, merge, reset, cherry-pick, release, deployment, or package publication has been performed.

## Branch inventory

The Sith policy requires exactly `master` and `development`. GitHub currently reports five branches:

| Branch | Head SHA | Ahead / behind development | Disposition |
|---|---|---:|---|
| `development` | `b1355e0502fa7cc7210a0a922e27cf43a43e1e7c` | Baseline | Active source branch. |
| `master` | `239b2ab4e1f8597cb4bf9d3b2ac4728b6e4d69a9` | 8 / 331 | Protected released baseline; preserve. |
| `docs/development-build-badge` | `0fc2ea425d939b393bf8ceb3995d5ac7b77b9f30` | 1 / 140 | PR #9 is open; its sole README badge change is already present in development. Retain until PR disposition. |
| `work/event-contract-conformance` | `e8dd3f2fa91c7a36799fea606aa397600a9cdd8e` | 2 / 22 | Its only changed test file is byte-identical to the file on development. **READY FOR OWNER DELETION**; hold for the single owner-approved deletion batch. |
| `work/protocol-compatibility-audit` | `14ea58c772ea1b728190e38184bd216f28e5a00e` | 9 / 140 | PR #8 is open. The compatibility API, report type, tests, README/consumer guidance, and theory material are already present in development. Retain until PR disposition. |

Comparison direction is commits unique to the named branch / commits by which it trails development. Merge bases:
- `master`: `860fed18a9028793a0709a8158f6c7549a88913c`
- `docs/development-build-badge`: `20ea8e6406dacb056edc9f3a9ff2a8c8352b30a2`
- `work/event-contract-conformance`: `6e1a46f29771649cc230247e882a71e2a7673a72`
- `work/protocol-compatibility-audit`: `20ea8e6406dacb056edc9f3a9ff2a8c8352b30a2`

## Pull request disposition

- [PR #8 — Add structural protocol compatibility checks](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/pull/8): open, not merged, GitHub reports not mergeable. Its original feature is now represented on development: `ProtocolCompatibilityReport.cs` has blob `9d78d4880311e279767061979c56f84543fcd61f`; `ProtocolDefinition.cs`, `ProtocolDefinitionTests.cs`, README, consuming guide, and theory documentation also contain the compatibility work. The branch is superseded by current development. Do not merge the stale PR. Owner must decide whether to close it before its branch enters the deletion batch.
- [PR #9 — docs: point ProtocolAi build badge at development](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/pull/9): open, not merged, GitHub reports not mergeable. Its one-line build-badge change is already present in current development. The branch is superseded. Owner must decide whether to close it before its branch enters the deletion batch.
- No PR has been closed and no branch has been deleted.

## Content verification

### Event-contract conformance branch
`tests/ProtocolAi.Tests/ProtocolEventContractConformanceTests.cs` is byte-identical on development and `work/event-contract-conformance` (blob `77cb24feedd1c01615cb14dd4646e1c4eba6a8e0`). The branch has no open PR. This is the sole surplus branch currently certified **READY FOR OWNER DELETION**.

### Compatibility audit branch
The PR's compatibility report/API and associated tests and documentation are present in development. The development implementation supersedes the old PR branch; retain the branch only until PR #8 is explicitly resolved.

### Development build badge branch
The PR's exact build badge line targeting `development` is already in the current development README. Retain the branch only until PR #9 is explicitly resolved.

## Master-only history and Sith policy evidence

The eight commits unique to master were reviewed:
- Three Workshop documentation commits add the animated moniker, standard identity heading, and link to the shared documentation standard. Those documentation changes are present in development.
- Five Sith-policy commits add the master-hosted policy badge/evidence, the initial policy workflow, and subsequent evidence refreshes. The live `assets/sith-policy.svg` and `assets/sith-policy.json` remain on master, which is the stable badge/evidence host referenced by the development README.
- Development contains a newer Sith workflow implementation. It checks out the triggering branch for normal branch pushes, evaluates the branch inventory, and restricts evidence publication to default-branch evaluations or delete events. The development README points its build badge at development and its Sith badge at the master-hosted SVG.
- The development copy of `assets/sith-policy.svg` is a placeholder; the live badge and JSON evidence intentionally remain on master. Do not copy the older master workflow wholesale over the newer development workflow.

Preserve master as the released baseline. The histories are diverged; do not merge, reset, force-push, or rewrite master as a cleanup shortcut.

## CI and release safeguards

- The last recorded full development build/test/package workflow, run [#42145038223](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/42145038223), passed at `7489463a52f7fa85018f7f810128431e78c8cd77`. The subsequent development commit `b1355e0` only refreshed this reconciliation ledger.
- The NuGet publish job was skipped. No package was published.
- Sith evaluation [#42145038190](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/42145038190) failed closed because five branches existed rather than exactly two. Its log confirmed development did not publish badge evidence to master.
- Keep NuGet publication disabled unless the repository owner explicitly authorizes it.

## Owner deletion readiness

- **READY FOR OWNER DELETION:** `work/event-contract-conformance`. It is content-identical to development and has no open PR. Do not delete until the owner performs the planned batch.
- **Pending PR disposition:** `docs/development-build-badge` (PR #9) and `work/protocol-compatibility-audit` (PR #8). Their changes are superseded, but their PRs remain open. Do not delete while those PRs remain unresolved.
- **Preserve:** `master` and `development`.

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
