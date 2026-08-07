# Implementation Agent Handoff

Status: Active onboarding and operating reference

Last verified: 2026-08-02

## Purpose

This section transfers implementation ownership without transferring game-design authority. It tells a replacement agent what it may change, how it must operate Unity, what already exists, what the predecessor implemented, and how to validate and report future work.

The section is descriptive and procedural. It does not authorize a new feature or version by itself.

## Required Reading Order

1. `00_onboarding_prompt.md` — copy-ready prompt for the new implementation agent.
2. `01_authority_and_scope.md` — authority order, design boundary, and allowed tools.
3. `../37_v0_4_0_0_spatial_readability_scope.md` — current authoritative player-facing version scope.
4. `../38_v0_4_0_0_7c_level_plan.md` — exact greybox topology and builder data.
5. `../39_v0_4_0_0_implementation_handoff.md` — current task order and delivery gate.
6. `02_current_implementation_state.md` — repository, build, controls, and implemented behavior.
7. `03_system_ownership_map.md` — architecture, seams, files, and invariants.
8. `04_operating_and_validation_procedure.md` — mandatory Unity MCP workflow and delivery gates.
9. `05_predecessor_work_and_history.md` — work completed before the current version.
10. `06_human_validation_and_known_risks.md` — evidence still requiring a person or unavailable hardware.
11. `../18_agent_implementation_handoff.md` — live project change-control protocol.
12. `../27_development_versioning.md` — version rules.

When no new version brief has been approved, onboarding, read-only auditing, defect diagnosis, and explicitly requested maintenance are allowed. Backlog items and exploratory design workspaces are not permission to start feature work.

## Handoff Snapshot

At the 2026-08-02 read-only audit:

- Unity project: `UnityProject/InternshipGameSynced`
- Unity version: `6000.5.4f1`
- Main scene: `Assets/_Project/Scenes/P01_CoreLoop.unity`
- Local build identity: `v0.3.0.0`
- Branch: `master`
- Published commit: `0636f5d0664b05b317d6504974bc82427be1d91b` (`v0.2.3.0`)
- Remote: `https://github.com/Moutaz-Code/internship-game.git`
- The tracked implementation at `HEAD` is published `v0.2.3.0`.
- The working tree contains user-owned local `v0.3.0.0` implementation and documentation plus other modified/untracked files. Their exact scope must be re-audited before any edit, commit, or push.
- Local v0.3 recorded evidence is focused 22/22, full Edit Mode 133/133, Play Mode 6/6, strict Windows build success, and smoke success; human evidence is Not Tested.
- `v0.4.0.0` is authorized by documents 37–39; implementation has not begun at this audit.

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
