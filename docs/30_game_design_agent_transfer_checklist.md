# Game Design Agent Transfer Checklist

Status: Repeatable session-transfer procedure

Last updated: 2026-07-26

## Purpose

Use this checklist when moving game-design work to a fresh assistant. It prevents stale scope, naming drift, uncommitted work, and AI-written assumptions from becoming false project history.

## Before The Transfer

- Run `git status --short --branch`.
- Record the current local build version.
- Record the latest published or committed version separately.
- Do not describe a dirty worktree as committed.
- Confirm the current prototype question and whether it is Proposed, Locked, Implementing, or Awaiting Evidence.
- Separate automated, desktop-human, XR, and hardware evidence.
- Update `28_game_design_agent_continuity.md`.
- Update `00_project_hub.md` if its phase, date, or next goal is stale.
- Add only real design/scope decisions to `05_decision_log.md`.
- Add implementation or playtest evidence to `06_prototype_log.md`.
- Preserve unresolved questions instead of silently answering them.

## New Agent Read Order

1. `28_game_design_agent_continuity.md`
2. `00_project_hub.md`
3. `14_current_design_synthesis.md`
4. Latest entries in `06_prototype_log.md`
5. Latest active entries in `05_decision_log.md`
6. `27_development_versioning.md`
7. Current proposed/approved prototype workspace
8. Current focused human-validation sheet, when one exists
9. `04_risk_and_scope_register.md`
10. Relevant architecture, QA, and framework documents

## New Agent First Audit

The new agent should report:

```text
Repository state:
Current local build:
Latest committed/published build:
Current prototype status:
Current prototype question:
Verified automated evidence:
Missing human/XR/hardware evidence:
Stale or conflicting documents:
Top three risks:
Recommended next three actions:
```

## During Design

- Keep one observable player-behavior question at the center.
- Label material as Confirmed, Proposed, Open, Deferred, Historical, or Superseded.
- Record the reason behind decisions, not only the selected option.
- Keep tuning values provisional until measured.
- Maintain a cut list alongside the must-have list.
- Convert broad ideas into separate experiments instead of one oversized scope.
- Protect the distinction between full-game direction and the current proof contract.
- Keep the target audience, VR comfort, physical safety, and venue operation in every feature review.

## Before An Implementation Handoff

- Confirm the user explicitly locked the experiment.
- Translate internal PT design language into the approved `v0.y.z.w` development version.
- State one prototype question.
- State must-have behavior.
- State explicit non-goals.
- State relevant architecture boundaries.
- Identify current baseline code and tests.
- Define functional, human, VR, hardware, and demo acceptance separately.
- Include feature flags or fallbacks for risky integrations.
- Define task order and dependencies.
- Require the implementation agent to preserve unrelated work.
- Require an evidence-based completion report.
- Do not include unresolved alternatives as simultaneous requirements.

## After Implementation

- Compare delivered behavior with the approved brief.
- Record later user-directed increments separately from the original experiment.
- Require automated regression evidence proportional to the change.
- Schedule uninformed human observation before declaring the design question passed.
- Classify the result as Passed, Conditional, Failed, or Inconclusive.
- Move unrelated polish and new mechanics to the next scope or backlog.

## Canvas Refresh

From the project root:

```powershell
scripts\generate_canvas.bat
```

Verify:

- `_boards/all-docs-auto.canvas` includes every eligible Markdown file,
- `_boards/working-map.canvas` remains untouched,
- the generated board is not used for manual curation.

## Git Safety

- Never commit, push, reset, clean, checkout, discard, or rewrite history without the user's explicit request.
- A successful build does not imply a committed repository.
- Syncthing state does not imply Git state.
- Large temporary LFS or synchronization artifacts should be investigated before destructive cleanup.
- Preserve implementation-agent and user changes while editing planning documents.

## Closeout Message

End a transfer with:

- what was added or updated,
- the current authority chain,
- the current prototype status,
- the next concrete decision or test,
- repository caveats,
- the exact onboarding prompt location.
