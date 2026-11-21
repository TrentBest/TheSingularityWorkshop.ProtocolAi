<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" />
  </a>
</p>

# Branch Reconciliation Ledger

**Review date:** 2026-11-21 UTC  
**Canonical branch inventory:** exactly `master` and `development` (both search pages checked; second page empty).  
**Development audit baseline:** `c3826982afe2bac6b064d70e137c21075586ee1a`.  
**Master audit baseline:** `657b3980f1f234cfcd71d65f3aff3af213e632b1`.  
**Compare at audit:** development 508 commits ahead and 26 behind master.  
**Disposition:** preserve both branches. No branch deletion, merge, reset, cherry-pick, release, deployment, or package publication was performed.

## Branch inventory and policy

The permanent Sith target is exactly two branches:

| Branch | Audit baseline | Meaning | Disposition |
|---|---|---|---|
| `development` | `c3826982afe2bac6b064d70e137c21075586ee1a` | Active source and next candidate | Preserve; continue work here |
| `master` | `657b3980f1f234cfcd71d65f3aff3af213e632b1` | Published-package history / release baseline | Preserve; do not move to current development |

These SHAs are the live branch tips verified on 2026-11-21 UTC. The exact branch inventory was fetched from GitHub and contains only these two refs.

The compare endpoint reports the histories are diverged: development is 508 commits ahead and 26 behind master. This divergence is not a cleanup defect. Do not merge, reset, force-push, or rewrite master to make the graph look simpler.

## Published package and source provenance

NuGet lists `TheSingularityWorkshop.ProtocolAi` version `0.1.0-alpha.2` as a published prerelease. The current development project declares `0.1.0-alpha.3`.

The repository contains release-named commit [`3a0bcc6f7b6c010d1e9598bd09f17ffd6c4e02de`](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/commit/3a0bcc6f7b6c010d1e9598bd09f17ffd6c4e02de), titled `release: ProtocolAI 0.1.0-alpha.2 with controlled publication`. Both master and development descend from that commit:
- master is 34 commits beyond it;
- development is 459 commits beyond it.

This proves that the release-named commit is retained in both histories. It does **not** prove that the exact published `.nupkg` was built from that SHA. The package's source SHA and artifact digest must be recovered from authoritative publish workflow/artifact provenance before any branch reconciliation is considered.

The previous ledger statement “No package was published” was incorrect and is superseded by the NuGet publication evidence. Do not repeat it.

## Sith and build evidence

The current candidate [`c3826982afe2bac6b064d70e137c21075586ee1a`](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/commit/c3826982afe2bac6b064d70e137c21075586ee1a) passed exact-head [build/test/package workflow #44845893139](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/44845893139) and [Sith policy workflow #44845893149](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/actions/runs/44845893149). The package contains `net8.0` and `net10.0`; 166 tests passed on each target, and the exact packed artifact installed and built in clean consumers for both. The previous candidate `2ea06ef...` was `net10.0`-only and is superseded. The current live inventory is exactly `master` and `development`; do not infer that passing policy means the histories should be reconciled.

The live Sith gate independently checks the remote branch inventory and fresh master-hosted evidence. Its evidence is time-bound. Re-run the gate on the exact candidate before any future CiAi release operation; this ledger is not a substitute for live verification.

The NuGet publication path remains disabled in the ProtocolAi workflow. Future publication is intended to be orchestrated through CiAi when explicitly directed by the owner. A successful build or Sith compliance result is not publication authorization.

## Event interoperability

ProtocolAi's typed v1 envelope and payload contracts are implemented and documented in [EVENT_ENVELOPES.md](EVENT_ENVELOPES.md). ProtocolAi does not own producer authentication, evidence verification, durable inbox/outbox storage, delivery, retries, deduplication, replay, authorization, or workflow execution.

The remaining host-side interoperability work is tracked in [Issue #13](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/issues/13) and [CROSS_REPOSITORY_EVENT_CONFORMANCE.md](CROSS_REPOSITORY_EVENT_CONFORMANCE.md). ProtocolAi may provide shared fixtures and contract guidance; implementation and operational proof belong in CommandAi, GrammarAi, CiAi, and the host repositories.

## Owner deletion readiness

- **READY FOR OWNER DELETION:** None.
- **Preserve:** `TrentBest/TheSingularityWorkshop.ProtocolAi:master` and `TrentBest/TheSingularityWorkshop.ProtocolAi:development`.
- No noncanonical branches were found in the complete two-page branch inventory.

No branch deletion is requested or required. This ledger is evidence and recommendation, not authorization to delete branches, merge histories, publish packages, or release.

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


## Superseding target-framework decision — 2026-11-21

The dual-target package evidence above is retained as historical evidence for the previous candidate. It is superseded for active development by the owner's ecosystem-wide .NET 10 baseline. The active ProtocolAi candidate is being aligned to `net10.0` only. Do not interpret the historical `net8.0` test/package results as validation of the new candidate; record fresh exact-head CI and package evidence after the target change. The published alpha.2 package and the `master` branch remain untouched.
