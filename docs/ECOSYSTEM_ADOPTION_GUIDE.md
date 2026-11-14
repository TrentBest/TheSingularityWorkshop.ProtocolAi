| Repository | Repository discussion |
|---|---|
| CiAi | [#58 — semantic stack composition](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/58#issuecomment-6885042967) |
| CommandAi | [#1 — durable command observers and evidence-bound reactions](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi/issues/1#issuecomment-6885039972) |
| Experiences | [#22 — multiplayer networking showcase](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/issues/22#issuecomment-6885041874) |
| FSM_Foundry | [#8 — governed CI-to-agent orchestration](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/issues/8#issuecomment-6885040762) |
| Hermit | [#5 — cross-host task lifecycle assessment](https://github.com/TrentBest/TheSingularityWorkshop.Hermit/issues/5#issuecomment-6885039060) |
| SingularityBot | [#15 — cross-boundary bot command and match lifecycle](https://github.com/TrentBest/TheSingularityWorkshop.SingularityBot/issues/15#issuecomment-6885037439) |
| AnyApp | [#8 — AnyApp ↔ WebApp local bridge and desktop deep-link contract](https://github.com/TrentBest/AnyApp/issues/8#issuecomment-6885194891) |

Other repository discussions already contain this guide's link and repository-specific boundaries, including ServerSC2, ServerApp, HeadlessAi, WebApp, MyVR, CreatorNetwork, MicroBundleRepository, DataWarehouse, FSM_REST, and FSM_COS. Their existing issues remain the place to update adoption notes; do not create duplicate tracking issues.

When a repository is notified, its LLM should:
1. Read the relevant repository row and global event-contract rules in this guide.
2. Compare them with the current `development` source and identify a real producer/consumer boundary.
3. Record one of: adopt fixture-only conformance, defer until a boundary exists, or no adoption.
4. If appropriate, update its own adoption note/issue during its normal development cycle, preserving domain ownership and exact source revisions.
5. Add a runtime dependency only after the required ProtocolAi API is explicitly released and exact-revision consumer tests justify it.

Notification does not authorize source changes, dependency additions, package publication, merges, deployment, or release.
