# VR Extraction Horror Prototype

Pixoul internship project for a polished minimum lovable product by 2026-08-28.

## Current Working Summary

The current desktop-first vertical slice is a 15-20 minute sci-fi extraction-horror run through a 10-space authored facility. Find and hold rods long enough to scan them, use the Geiger counter to search, deliver three confirmed-live rods while one adaptive Owl patrols the route network, survive the 20-second extraction cycle, then cross the open exit. The player, threat navigation, feedback devices, and run configuration remain behind narrow seams so XR and physical hardware implementations can replace their desktop simulators later.

Current local build identity: `v0.2.3.0`. Prototype 2 now includes readable route adaptation plus the desktop embodiment/threat pass: fitted portal geometry, crouch, corner lean, forgiving occlusion-safe pickup targeting, and a more capable but still cover-respecting Owl. PT2 feature expansion is closed; human validation remains. The next proposed game-design experiment is readable stealth and contractor counterplay, but no `v0.3.0.0` implementation scope is authorized yet.

Repository note: the 2026-07-28 audit found published `master` at `v0.2.3.0` (`0636f5d`) with separate documentation/OpenXR-related working-tree changes still present. Verify branch, `HEAD`, and exact dirty paths before release or transfer work.

## Prototype Quick Start

- Unity project: `UnityProject/InternshipGameSynced`
- Main scene: `Assets/_Project/Scenes/P01_CoreLoop.unity`
- Windows player: `UnityProject/InternshipGameSynced/Builds/v0.2.3.0/InternshipGame_v0.2.3.0.exe`

Desktop controls:

- `WASD` move, mouse look, `Shift` sprint, `Space` jump.
- Hold `Ctrl` or `C` to crouch; hold `Z`/`X` to lean left/right around cover.
- `E` grab/drop; left click uses the focused door, exit, or other interactable.
- `R` restart the current seed; `Shift+R` generate a new seed.
- `F1` toggle the operator overlay; `F2` toggle bounded Owl/Geiger interference; `F3` toggle route adaptation for comparison.
- `F11` toggle borderless fullscreen/windowed mode. The Windows build starts in borderless fullscreen and preserves a resizable 1280x720 windowed fallback.
- `Esc` pause/resume and release/capture the cursor.

The baseline experience starts with Geiger interference disabled so first-time device comprehension can be measured before adding uncertainty.

## Core Workflow

Weekly prototype-heavy agile loop:

1. Monday: choose prototype goal, sprint tasks, risks, and success criteria.
2. Tuesday-Thursday: build, integrate, test locally, and keep notes.
3. Friday: playtest or demo, update docs, decide what changes next.

Docs are living control panels, not a waterfall phase. Each update should help make a build decision, communicate status, or reduce risk.

## Project Docs

- [Project Hub](docs/00_project_hub.md)
- [Current Design Synthesis](docs/14_current_design_synthesis.md)
- [Contract Scenario Framework](docs/17_contract_scenario_framework.md)
- [Agent Implementation Handoff](docs/18_agent_implementation_handoff.md)
- [Adaptive Entity Framework](docs/19_adaptive_entity_framework.md)
- [Owl Adaptation V1 Spec](docs/20_owl_adaptation_v1_spec.md)
- [Player Tactics And Stealth Framework](docs/21_player_tactics_and_stealth_framework.md)
- [Entity AI Technology Strategy](docs/22_ai_technology_strategy.md)
- [Nuclear Recovery Objective And Environment](docs/23_nuclear_recovery_objective_environment.md)
- [Prototype 2 Scope](docs/24_prototype_02_scope.md)
- [Prototype 2 Agent Handoff](docs/25_prototype_02_agent_handoff.md)
- [Prototype 2 ClickUp Paste Pack](docs/26_clickup_prototype_02_paste.md)
- [Development Versioning](docs/27_development_versioning.md)
- [Game Design Agent Continuity](docs/28_game_design_agent_continuity.md)
- [New Game Design Agent Onboarding Prompt](docs/29_game_design_agent_onboarding_prompt.md)
- [Game Design Agent Transfer Checklist](docs/30_game_design_agent_transfer_checklist.md)
- [PT3 Design Workspace](docs/31_pt3_design_workspace.md)
- [PT2 Human Validation Sheet](docs/32_pt2_human_validation_sheet.md)
- [Implementation Agent Handoff](docs/implementation_agent/README.md)
- [Implementation Agent Onboarding Prompt](docs/implementation_agent/00_onboarding_prompt.md)
- [First Playable Spec](docs/09_first_playable_spec.md)
- [Technical Architecture Outline](docs/10_technical_architecture_outline.md)
- [Vision Brief](docs/01_vision_brief.md)
- [Sprint 01 Plan](docs/02_sprint_01_plan.md)
- [Sprint 02 Plan](docs/15_sprint_02_plan.md)
- [Sprint 02 ClickUp Paste Pack](docs/16_clickup_sprint_02_paste.md)
- [Backlog Seed](docs/03_backlog_seed.md)
- [Risk and Scope Register](docs/04_risk_and_scope_register.md)
- [Decision Log](docs/05_decision_log.md)
- [Prototype Log](docs/06_prototype_log.md)
- [Playtest and QA Template](docs/07_playtest_qa_template.md)
- [ClickUp Paste Pack](docs/08_clickup_paste_pack.md)
- [Weekly Stakeholder Report Template](docs/templates/weekly_stakeholder_report.md)

## Obsidian Canvas Workflow

Open `docs` as an Obsidian vault, enable the Canvas core plugin, then generate
the visual docs overview from the project root:

```powershell
scripts\generate_canvas.bat
```

The generated board is `docs\_boards\all-docs-auto.canvas`. It is safe to
rerun and may be overwritten. Use `docs\_boards\working-map.canvas` for manual
planning because the generator will not overwrite it.

See [Obsidian Canvas Workflow](docs/12_obsidian_canvas_workflow.md).
