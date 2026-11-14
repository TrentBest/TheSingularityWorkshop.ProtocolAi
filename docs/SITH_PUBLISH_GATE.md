<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" />
  </a>
</p>

# Sith Publish Gate — Fail-Closed Design

**Status:** Executable verification job implemented and live-CI-verified on `development`; publication remains hard-disabled.  
**Policy:** Exactly `master` and `development`; no exceptions.  
**Safety:** Package publication remains hard-disabled. This document does not authorize a release or a workflow change that enables publication.

## Objective

A successful build, test, or pack is not sufficient authorization to publish. Immediately before a package publication job can execute, the workflow must independently establish that the repository's current remote branch inventory is exactly `master` plus `development`, and that the master-hosted Sith evidence is recent, successful, and consistent with that inventory.

The gate is a prerequisite, not an authorization system. The owner-controlled release trigger remains a separate requirement.

## Required checks

The future gate job must fail closed unless every check below succeeds:

1. **Fetch the live inventory.** Use the GitHub API with pagination enabled; do not infer the branch list from the checkout, event payload, cached artifact, or a single page.
2. **Require exact set equality.** Sort and compare the complete remote branch-name set to exactly `development` and `master`. Missing, extra, duplicate/ambiguous, or unreadable inventory is a failure.
3. **Fetch master-hosted evidence.** Read `assets/sith-policy.json` from `master`, not from the candidate development checkout.
4. **Validate evidence structure.** Require the expected repository identity, policy identifier, required branch array, actual branch array, branch count, status, evaluation timestamp, and workflow-run URL.
5. **Validate evidence content.** Require status `COMPLIANT`, branch count `2`, and actual branches exactly `development` and `master`. Compare the evidence branch set with the independently fetched live set.
6. **Validate freshness.** Reject missing, unparsable, future-dated, or older-than-24-hours evidence. The maximum age is an explicit policy constant and should be tightened only with a corresponding schedule/operational decision.
7. **Validate the producing workflow run.** Parse the run ID from the evidence URL; require the URL to belong to this repository; query GitHub Actions and require the referenced Sith run to have completed with conclusion `success`. A successful HTTP response alone is not success evidence.
8. **Publish the gate result as a job output.** The publish job must depend on the gate job and consume its explicit success output. A skipped, failed, cancelled, timed-out, or missing gate job must never satisfy the dependency.

## Workflow shape

The intended dependency graph is:

```text
build-and-test ───────────────┐
                              ├── publish_nuget (still disabled)
verify-sith-compliance ───────┘
```

The compliance job can run independently of build/test/pack. The publication job requires both successful build artifacts and successful compliance verification.

The eventual publication condition must include all of the following:
- the compliance job completed successfully;
- the compliance output is explicitly `true`;
- the build-and-test job completed successfully;
- an owner-controlled release trigger is explicitly enabled.

Until Trent explicitly authorizes enabling the release path, retain the current unconditional hard stop on the publication job. Do not replace it with a condition that can publish merely because a workflow input was set.

## Failure semantics

| Condition | Gate result |
|---|---|
| API unavailable, permission denied, rate-limited, or pagination incomplete | Fail |
| Any branch other than exactly `master` and `development` | Fail |
| Sith JSON missing, malformed, or wrong repository/policy | Fail |
| Evidence says non-compliant or lists a different branch set | Fail |
| Evidence timestamp missing, future-dated, or older than 24 hours | Fail |
| Referenced workflow run missing, incomplete, or not successful | Fail |
| Any required output absent or ambiguous | Fail |
| Every check passes | Compliance prerequisite satisfied; owner authorization still required |

There is no warning-only mode, fallback to cached evidence, or default-success output.

## Security and implementation notes

- Give the verification job only `contents: read` and `actions: read`; it does not need contents write, package write, or NuGet credentials.
- Use the repository's GitHub token for API reads. Do not expose tokens in logs.
- Keep the package artifact produced by build/test/pack separate from the compliance evidence. The gate must verify remote state at publication time, not merely trust an artifact produced earlier in the workflow.
- Treat all fetched JSON and URLs as untrusted input. Validate repository identity and URL path before requesting a run.
- Use UTC timestamps and a single documented maximum-age constant.
- Emit a compact machine-readable result with status, observed branch set, evidence timestamp, evidence run ID, and failure reason. Never emit credentials or sensitive headers.
- Do not move branch enumeration into ProtocolAi library code; this is repository workflow policy.

## Verification plan before any enablement

1. Test exact two-branch success with fresh successful evidence.
2. Test an extra branch, missing branch, and API pagination failure.
3. Test missing/malformed JSON, wrong repository, wrong policy, contradictory branch arrays, and non-compliant status.
4. Test stale, future-dated, and unparsable timestamps.
5. Test missing run, in-progress run, failed run, and cancelled run.
6. Test missing gate output and skipped/failed dependency behavior.
7. Confirm build/test/pack still run when the compliance gate fails.
8. Confirm the publish job remains skipped in every test until explicit owner authorization.

## Current state

The current `.github/workflows/build.yml` contains an independent `verify-sith-compliance` job. It runs the standard-library unit tests, fetches the complete live branch inventory with `gh api --paginate`, reads `assets/sith-policy.json` from `master`, and validates the referenced Actions run before emitting `compliant=true`. Any failed command or validation exits nonzero; the job has read-only `contents` and `actions` permissions.

The verifier is implemented in `scripts/verify_sith_compliance.py`; focused failure-case tests are in `scripts/tests/test_verify_sith_compliance.py`. These tests cover exact-set enforcement, duplicate entries, contradictory evidence, malformed/stale/future timestamps, foreign run URLs, and unsuccessful or mismatched workflow runs. Live verification first succeeded in Actions run [#43532070642](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/43532070642), then passed again on the latest documentation commit `444b6112c2390e6e123fee280a45adadf6367a65` in run [#43532326421](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/43532326421). The latest run passed all 11 unit tests and the live `gh api --paginate` inventory/evidence/run validation. The verifier observed exactly `development` and `master`, with master-hosted evidence run `43422877852` evaluated at `2026-11-13T12:10:10Z`.

The latest Actions run also completed `build-and-test` successfully; restore, build, tests/coverage, Codecov, pack, documentation-path checks, clean-consumer build, and artifact uploads all passed. The `publish_nuget` job was skipped as intended. It depends on both `build-and-test` and `verify-sith-compliance`, but its unconditional `if: ${{ false }}` remains intact. The compliance output is diagnostic/prerequisite evidence only; it does not grant owner authorization. This implementation does not enable publication.

Related: [Operation Sith issue #11](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/issues/11), [Branch Reconciliation Ledger](BRANCH_RECONCILIATION.md).

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
