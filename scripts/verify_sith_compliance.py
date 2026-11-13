#!/usr/bin/env python3
"""Fail-closed verification of the repository's live Sith branch policy."""
from __future__ import annotations

import argparse
import base64
import json
import re
import subprocess
import sys
from datetime import datetime, timedelta, timezone

REPOSITORY = "TrentBest/TheSingularityWorkshop.ProtocolAi"
REQUIRED_BRANCHES = {"development", "master"}
POLICY = "exactly-master-and-development"
MAX_AGE = timedelta(hours=24)
RUN_URL = re.compile(r"^https://github\.com/TrentBest/TheSingularityWorkshop\.ProtocolAi/actions/runs/([0-9]+)$")


class GateError(Exception):
    pass


def parse_utc(value: str) -> datetime:
    if not isinstance(value, str) or not value:
        raise GateError("missing evidence timestamp")
    try:
        parsed = datetime.fromisoformat(value.replace("Z", "+00:00"))
    except ValueError as exc:
        raise GateError("malformed evidence timestamp") from exc
    if parsed.tzinfo is None or parsed.utcoffset() is None:
        raise GateError("evidence timestamp must include a timezone")
    return parsed.astimezone(timezone.utc)


def validate_evidence(evidence: dict, live_branches: list[str], run: dict,
                      now: datetime | None = None) -> int:
    now = now or datetime.now(timezone.utc)
    if not isinstance(evidence, dict):
        raise GateError("evidence must be a JSON object")
    if evidence.get("repository") != REPOSITORY:
        raise GateError("wrong repository identity")
    if evidence.get("policy") != POLICY:
        raise GateError("wrong policy identifier")
    if evidence.get("required_branches") != ["development", "master"]:
        raise GateError("required branch declaration is contradictory")
    actual = evidence.get("actual_branches")
    if not isinstance(actual, list) or any(not isinstance(x, str) for x in actual):
        raise GateError("actual_branches must be a string array")
    if len(actual) != len(set(actual)) or set(actual) != REQUIRED_BRANCHES:
        raise GateError("evidence branch set is not exactly master + development")
    if evidence.get("branch_count") != 2 or type(evidence.get("branch_count")) is not int:
        raise GateError("evidence branch_count must be integer 2")
    if evidence.get("status") != "COMPLIANT":
        raise GateError("Sith evidence is not COMPLIANT")
    if not isinstance(live_branches, list) or any(not isinstance(x, str) for x in live_branches):
        raise GateError("live branch inventory is invalid")
    if len(live_branches) != len(set(live_branches)) or set(live_branches) != REQUIRED_BRANCHES:
        raise GateError("live branch inventory is not exactly master + development")
    if set(actual) != set(live_branches):
        raise GateError("evidence and live branch inventory disagree")

    evaluated = parse_utc(evidence.get("evaluated_at"))
    if evaluated > now:
        raise GateError("Sith evidence is future-dated")
    if now - evaluated > MAX_AGE:
        raise GateError("Sith evidence is older than 24 hours")

    match = RUN_URL.fullmatch(evidence.get("workflow_run", "") if isinstance(evidence.get("workflow_run"), str) else "")
    if not match:
        raise GateError("workflow_run URL is not an exact URL for this repository")
    run_id = int(match.group(1))
    if not isinstance(run, dict):
        raise GateError("referenced workflow run is unavailable")
    if str(run.get("id")) != str(run_id):
        raise GateError("workflow run ID does not match evidence URL")
    if run.get("name") != "Sith Policy":
        raise GateError("referenced run is not the Sith Policy workflow")
    workflow_path = run.get("path")
    if not isinstance(workflow_path, str) or workflow_path.split("@", 1)[0] != ".github/workflows/sith-policy.yml":
        raise GateError("referenced run uses an unexpected workflow path")
    if "@" in workflow_path and workflow_path.split("@", 1)[1] != "refs/heads/master":
        raise GateError("referenced Sith workflow run is not master-hosted")
    if run.get("head_branch") != "master":
        raise GateError("Sith evidence was not produced from master")
    if run.get("status") != "completed" or run.get("conclusion") != "success":
        raise GateError("referenced Sith workflow run did not complete successfully")
    triggering_commit = evidence.get("triggering_commit")
    if not isinstance(triggering_commit, str) or not re.fullmatch(r"[0-9a-fA-F]{40}", triggering_commit):
        raise GateError("missing or malformed triggering_commit")
    if run.get("head_sha") != triggering_commit:
        raise GateError("evidence triggering_commit does not match workflow run head SHA")
    return run_id


def gh_json(*args: str):
    completed = subprocess.run(["gh", "api", *args], check=True, text=True, capture_output=True)
    try:
        return json.loads(completed.stdout)
    except json.JSONDecodeError as exc:
        raise GateError("GitHub API returned malformed JSON") from exc


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--repository", default=REPOSITORY)
    args = parser.parse_args()
    try:
        if args.repository != REPOSITORY:
            raise GateError("unexpected repository argument")
        # --paginate is essential: this output includes every page. Preserve duplicates
        # so malformed/ambiguous inventories cannot be normalized into compliance.
        branches_result = subprocess.run(
            ["gh", "api", "--paginate", f"repos/{REPOSITORY}/branches?per_page=100", "--jq", ".[].name"],
            check=True, text=True, capture_output=True)
        branches = [line for line in branches_result.stdout.splitlines() if line]
        if len(branches) != len(set(branches)) or set(branches) != REQUIRED_BRANCHES:
            raise GateError(f"live branch inventory is not exactly master + development: {branches!r}")

        encoded = gh_json(f"repos/{REPOSITORY}/contents/assets/sith-policy.json?ref=master")
        if encoded.get("encoding") != "base64" or not isinstance(encoded.get("content"), str):
            raise GateError("master-hosted Sith evidence is missing or not base64 content")
        try:
            evidence = json.loads(base64.b64decode("".join(encoded["content"].split()), validate=True))
        except (ValueError, json.JSONDecodeError) as exc:
            raise GateError("master-hosted Sith evidence is malformed") from exc
        url = evidence.get("workflow_run") if isinstance(evidence, dict) else None
        match = RUN_URL.fullmatch(url if isinstance(url, str) else "")
        if not match:
            raise GateError("workflow_run URL is not an exact URL for this repository")
        run = gh_json(f"repos/{REPOSITORY}/actions/runs/{match.group(1)}")
        run_id = validate_evidence(evidence, branches, run)
        print(json.dumps({"status": "COMPLIANT", "repository": REPOSITORY,
                          "branches": sorted(branches), "evidence_run_id": run_id,
                          "evaluated_at": evidence["evaluated_at"]}, sort_keys=True))
        return 0
    except (GateError, subprocess.CalledProcessError) as exc:
        print(json.dumps({"status": "BLOCKED", "reason": str(exc)}, sort_keys=True), file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
