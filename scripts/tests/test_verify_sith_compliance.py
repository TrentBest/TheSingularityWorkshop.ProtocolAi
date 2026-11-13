import sys
import unittest
from datetime import datetime, timedelta, timezone
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from verify_sith_compliance import GateError, REPOSITORY, validate_evidence


NOW = datetime(2026, 11, 13, 23, 40, tzinfo=timezone.utc)
BASE_EVIDENCE = {
    "repository": REPOSITORY,
    "policy": "exactly-master-and-development",
    "required_branches": ["development", "master"],
    "actual_branches": ["development", "master"],
    "branch_count": 2,
    "status": "COMPLIANT",
    "evaluated_at": "2026-11-13T23:00:00Z",
    "workflow_run": "https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/12345",
    "triggering_commit": "a" * 40,
}
BASE_RUN = {
    "id": 12345,
    "name": "Sith Policy",
    "path": ".github/workflows/sith-policy.yml",
    "head_branch": "master",
    "status": "completed",
    "conclusion": "success",
    "head_sha": "a" * 40,
}


class SithGateTests(unittest.TestCase):
    def test_exact_fresh_success(self):
        self.assertEqual(validate_evidence(BASE_EVIDENCE, ["development", "master"], BASE_RUN, NOW), 12345)

    def assert_blocked(self, evidence=None, branches=None, run=None):
        with self.assertRaises(GateError):
            validate_evidence(evidence if evidence is not None else BASE_EVIDENCE,
                              branches if branches is not None else ["development", "master"],
                              run if run is not None else BASE_RUN, NOW)

    def test_extra_or_missing_live_branch_blocks(self):
        self.assert_blocked(branches=["development", "master", "feature"])
        self.assert_blocked(branches=["development"])

    def test_duplicate_live_or_evidence_branch_blocks(self):
        self.assert_blocked(branches=["development", "master", "master"])
        evidence = dict(BASE_EVIDENCE, actual_branches=["development", "master", "master"])
        self.assert_blocked(evidence=evidence)

    def test_contradictory_identity_policy_or_status_blocks(self):
        for field, value in (("repository", "attacker/repo"), ("policy", "other"),
                             ("status", "NON-COMPLIANT"),
                             ("required_branches", ["master"]),
                             ("branch_count", 3)):
            self.assert_blocked(evidence=dict(BASE_EVIDENCE, **{field: value}))

    def test_evidence_and_live_inventory_must_agree(self):
        self.assert_blocked(branches=["development", "master", "unexpected"])

    def test_stale_future_and_malformed_timestamps_block(self):
        self.assert_blocked(evidence=dict(BASE_EVIDENCE, evaluated_at=(NOW - timedelta(hours=25)).isoformat()))
        self.assert_blocked(evidence=dict(BASE_EVIDENCE, evaluated_at=(NOW + timedelta(minutes=1)).isoformat()))
        self.assert_blocked(evidence=dict(BASE_EVIDENCE, evaluated_at="not-a-date"))
        self.assert_blocked(evidence=dict(BASE_EVIDENCE, evaluated_at="2026-11-13T23:00:00"))

    def test_foreign_or_modified_run_url_blocks(self):
        self.assert_blocked(evidence=dict(BASE_EVIDENCE, workflow_run="https://evil.example/actions/runs/12345"))
        self.assert_blocked(evidence=dict(BASE_EVIDENCE, workflow_run=BASE_EVIDENCE["workflow_run"] + "?x=1"))

    def test_run_identity_and_success_are_required(self):
        for field, value in (("id", 9), ("name", "Other"), ("path", ".github/workflows/other.yml"),
                             ("head_branch", "development"), ("status", "in_progress"),
                             ("conclusion", "failure"), ("head_sha", "b" * 40)):
            self.assert_blocked(run=dict(BASE_RUN, **{field: value}))

    def test_missing_or_malformed_commit_blocks(self):
        self.assert_blocked(evidence=dict(BASE_EVIDENCE, triggering_commit="bad"))


if __name__ == "__main__":
    unittest.main()
