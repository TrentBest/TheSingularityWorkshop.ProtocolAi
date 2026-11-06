<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Branch Reconciliation Ledger

**Review date:** 2026-11-06 UTC  
**Scope:** All repository branches, open pull requests, and current development CI  
**Disposition:** Audit only. No merge, cherry-pick, reset, branch deletion, PR closure, release, or publication has been performed.

## Current branch inventory

The Sith policy requires exactly `master` and `development`. GitHub currently reports five branches:

| Branch | Current head SHA | Relationship to `development` | Evidence / disposition |
|---|---|---:|---|
| `development` | `7489463a52f7fa85018f7f810128431e78c8cd77` | Baseline | Active source branch; latest build/test/package workflow passed. |
| `master` | `239b2ab4e1f8597cb4bf9d3b2ac4728b6e4d69a9` | 8 ahead / 330 behind | Released baseline; preserve. Histories are diverged. |
| `docs/development-build-badge` | `0fc2ea425d939b393bf8ceb3995d5ac7b77b9f30` | 1 ahead / 139 behind | PR #9 remains open; preserve pending review of its unique README change. |
| `work/event-contract-conformance` | `e8dd3f2fa91c7a36799fea606aa397600a9cdd8e` | 2 ahead / 21 behind | Contains a distinct 131-line event-contract conformance test file; no PR found in this review. Preserve pending integration/review. |
| `work/protocol-compatibility-audit` | `14ea58c772ea1b728190e38184bd216f28e5a00e` | 9 ahead / 139 behind | PR #8 remains open; preserve pending review of compatibility implementation, docs, and tests. |

Comparison direction above is expressed as commits unique to the named branch / commits by which it trails `development`. The comparison base for the three work branches is the current `development` head above. Their merge bases are:
- `master`: `860fed18a9028793a0709a8158f6c7549a88913c`
- `docs/development-build-badge`: `20ea8e6406dacb056edc9f3a9ff2a8c8352b30a2`
- `work/event-contract-conformance`: `6e1a46f29771649cc230247e882a71e2a7673a72`
- `work/protocol-compatibility-audit`: `20ea8e6406dacb056edc9f3a9ff2a8c8352b30a2`

## Open pull requests

| PR | Head → base | Head SHA | State | Current GitHub mergeability |
|---|---|---|---|---|
| [#8 — Add structural protocol compatibility checks](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/pull/8) | `work/protocol-compatibility-audit` → `development` | `14ea58c772ea1b728190e38184bd216f28e5a00e` | Open | `dirty`, not mergeable |
| [#9 — docs: point ProtocolAi build badge at development](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/pull/9) | `docs/development-build-badge` → `development` | `0fc2ea425d939b393bf8ceb3995d5ac7b77b9f30` | Open | `dirty`, not mergeable |

The PR base SHA reported by GitHub is `20ea8e6406dacb056edc9f3a9ff2a8c8352b30a2`, which is behind the current development head. Do not merge either PR based solely on its title or old review state. Reassess its complete diff against the current development source first.

## Verified development CI and policy result

- Latest development build workflow: [run #42145038223](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/42145038223) at `7489463a52f7fa85018f7f810128431e78c8cd77` — **passed**. Restore, build, tests with coverage, package, documentation-path verification, clean-consumer package verification, and artifact uploads all succeeded.
- The `publish_nuget` job was **skipped**. No NuGet publication occurred in this run.
- Latest development Sith evaluation: [run #42145038190](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/42145038190) — **failed closed as NON-COMPLIANT** because the five-branch inventory violates the exact-two-branch policy. The log confirms that development did not publish badge evidence to `master`.
- The workflow correction is therefore validated: development evaluations do not write the default-branch badge/evidence. The policy failure is expected until the surplus branches are reconciled.

## Reconciliation safeguards and next work

1. Review and harvest any still-useful unique commits from all three surplus work branches into `development` through deliberate, testable changes. In particular, review the compatibility report/definition/tests and the standalone event-contract conformance suite.
2. Reassess PR #8 and PR #9 against the current `development` head; both currently report `dirty` and must not be merged blindly.
3. Review the eight commits unique to `master` and the 330 commits unique to `development` before deciding how to reconcile the diverged released baseline. Never force-push or rewrite `master` as a shortcut.
4. Re-run build/test and Sith evaluation after integration. The Sith gate should pass only when exactly `master` and `development` remain.
5. Keep NuGet publication hard-disabled until the release gate is satisfied and the repository owner explicitly authorizes publication.

## Owner deletion readiness

**No branch is certified READY FOR OWNER DELETION.** All three surplus branches contain unique commits, and two are associated with open PRs. Preserve them until their unique work has been reviewed and safely integrated or explicitly rejected. This ledger is not authorization to delete branches, close PRs, merge histories, or publish packages.

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
