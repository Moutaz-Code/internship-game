# Implementation Agent Handoff

Status: Active onboarding and operating reference

Last verified: 2026-08-10

## Purpose

This section transfers implementation ownership without transferring game-design authority. It tells a replacement agent what it may change, how it must operate Unity, what already exists, what the predecessor implemented, and how to validate and report future work.

The section is descriptive and procedural. It does not authorize a new feature or version by itself.

## Required Reading Order

1. `00_onboarding_prompt.md` — copy-ready prompt for the new implementation agent.
2. `01_authority_and_scope.md` — authority order, design boundary, and allowed tools.
3. `../37_v0_4_0_0_spatial_readability_scope.md` — authoritative player-facing scope for the current implemented baseline.
4. `../38_v0_4_0_0_7c_level_plan.md` — exact greybox topology and builder data.
5. `../39_v0_4_0_0_implementation_handoff.md` — completed task order and delivery gate for v0.4.
6. `02_current_implementation_state.md` — repository, build, controls, and implemented behavior.
7. `03_system_ownership_map.md` — architecture, seams, files, and invariants.
8. `04_operating_and_validation_procedure.md` — mandatory Unity MCP workflow and delivery gates.
9. `05_predecessor_work_and_history.md` — work completed before the current version.
10. `06_human_validation_and_known_risks.md` — evidence still requiring a person or unavailable hardware.
11. `../18_agent_implementation_handoff.md` — live project change-control protocol.
12. `../27_development_versioning.md` — version rules.
13. `07_facility_kit_and_p02_authoring.md` — approved-facility P02 rebuild, modular-kit conventions, transform, and deviation register.

When no new version brief has been approved, onboarding, read-only auditing, defect diagnosis, and explicitly requested maintenance are allowed. Backlog items and exploratory design workspaces are not permission to start feature work.

## Handoff Snapshot

At the 2026-08-10 repository verification:

- Unity project: `UnityProject/InternshipGameSynced`
- Unity version: `6000.5.4f1`
- Main scene: `Assets/_Project/Scenes/P01_CoreLoop.unity`
- Implementation identity: `v0.4.0.0`
- Branch: `master`
- Remote: `https://github.com/Moutaz-Code/internship-game.git`
- `master` contains the v0.4 source, tests, scene, captures, and handoff material and matched `origin/master` before this documentation refresh.
- Recorded v0.4 evidence is focused Edit Mode 28/28, full Edit Mode 141/141, Play Mode 7/7, strict Windows build success, and smoke success; human evidence is Not Tested.
- Build outputs and smoke logs are ignored and device-local. Rebuild them through Unity MCP when required.
- No later feature version is authorized by this handoff.

This snapshot can become stale immediately. The replacement agent must inspect the live editor and repository before relying on it.

## Non-Negotiable Summary

- The user has final authority.
- The agent implements; it does not invent or revise game design.
- Ask when player-facing behavior, scope, values, or acceptance criteria are ambiguous.
- C# source, C# tests, and Markdown may be edited as files.
- Unity Editor state and Unity-owned assets must be created, changed, run, and verified through Unity MCP.
- If Unity MCP is unavailable, pause Unity work and ask the user to restore the connection. Do not bypass it.
- Preserve unrelated work and dirty files.
- Do not commit, push, publish, re-version, or make a release unless the user asks. In this project, an unqualified future request to “commit” means commit and push unless the user says otherwise.
