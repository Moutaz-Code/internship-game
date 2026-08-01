# Agent Implementation Handoff

Status: Active project protocol

Last updated: 2026-08-01

## Purpose

Another AI agent actively reads project documents and implements the game. This protocol prevents exploratory design notes from becoming accidental implementation scope.

## Source Priority

When documents disagree, use this order:

1. The newest explicit user instruction.
2. The newest explicitly approved version-oriented implementation brief.
3. Active entries in `05_decision_log.md`.
4. The current contract spec in `09_first_playable_spec.md`.
5. Current architecture constraints in `10_technical_architecture_outline.md`.
6. Prototype evidence in `06_prototype_log.md`.
7. Historical prototype scopes, including `24_prototype_02_scope.md` and `25_prototype_02_agent_handoff.md`.
8. Broader sprint plans, design synthesis, and framework documents.
9. Workbooks, examples, parking-lot ideas, and internal game-design workspaces.

Escalate a conflict instead of choosing the most expansive interpretation.

## Status Meanings

| Label | Implementation Meaning |
|---|---|
| Active / Confirmed | Approved design direction, but implement only when it is also inside current sprint scope. |
| Active for Prototype Testing | Implement as a tunable experiment; do not treat values as final. |
| Proposed | Do not implement without approval or a current task that explicitly resolves it. |
| Pending / Open Question | Do not choose silently in code. Surface the decision. |
| Future Example / Parking Lot | Product direction only; no implementation. |
| Historical / Superseded | Context only; do not use as current requirements. |

## Current Authorized Implementation Scope

`v0.3.0.0` Readable Stealth and Contractor Counterplay is authorized by `33_v0_3_0_0_readable_stealth_scope.md`. Use `34_v0_3_0_0_implementation_handoff.md` for task order and `35_v0_3_0_0_human_validation_sheet.md` for person-led evidence.

Current implementation baseline:

- `v0.2.3.0` contains the implemented Prototype 2 route-adaptation experiment plus later user-directed desktop embodiment, portal, pickup, presentation, and Owl-perception increments.
- The 2026-07-28 repository audit found that implementation published on `master` at commit `0636f5d`, with separate documentation/OpenXR-related working-tree changes still present.
- Preserve both the published baseline and unrelated local work. Do not revert, re-version, commit, or publish new changes without explicit user instruction.
- PT2 may receive evidence-driven tuning or release-blocking defect fixes only when the user explicitly requests them.

Dedicated replacement-agent onboarding and operating details are in `implementation_agent/README.md`.

Historical PT2 authority:

- `24_prototype_02_scope.md`,
- `25_prototype_02_agent_handoff.md`,
- `26_clickup_prototype_02_paste.md`.

These files explain the original experiment but no longer authorize new PT2 feature work.

Current implementation:

- Implement only the explicitly approved `v0.3.0.0` brief and newer direct user instructions.
- `31_pt3_design_workspace.md` remains an internal design record; it is not the implementation feature list.
- Use only the `v{x}.{y}.{z}.{w}` scheme in `27_development_versioning.md` for code, builds, commits, and implementation reports.

The proof-contract content remains:

The current implemented proof contract is:

- post-apocalyptic nuclear facility,
- identify and transport three live fuel rods,
- Owl Thing entity,
- seeded authored variation,
- extraction holdout and exit.

Do not implement a second mission, second entity, unrestricted mission generator, persistent progression, loadout economy, or generalized content framework without a future approved version brief.

## Before Implementing A Design Change

Record:

```text
Task ID:
Relevant decision IDs:
Relevant spec section:
Behavior being changed:
Acceptance criteria:
Fallback or feature flag:
Open question blocking implementation:
```

If an open question changes player-facing behavior, stop and request a decision. Technical details that preserve the documented behavior may be chosen using the existing codebase patterns.

## After Implementing

- Run tests proportional to the behavior changed.
- Report files and systems changed.
- Report what was verified automatically and what still needs human playtesting.
- Update `06_prototype_log.md` with evidence, not just completion claims.
- Add a decision-log entry only for a real design, scope, or architecture choice.
- Do not rewrite exploratory notes as though every idea was approved.

## Deferred Capture Handoff

The capture direction below remains valid design context but is not implementation authority for `v0.3.0.0`. Wait for a later explicitly approved version brief before implementing capture recovery.

Approved design direction for future prototype testing:

- Owl attack causes unconsciousness through a VR-safe fade/audio transition.
- No direct or graphic feeding depiction.
- First and second captures are recoverable; third is terminal for the current test.
- Completed deposits remain safe.
- Carried items scatter only into valid authored positions.
- Recovery selects from roughly five authored zones using safety and anti-fast-travel rules.
- Owl retreats for about 60 seconds outside the final five minutes.
- During the final five minutes, protection lasts approximately 20-30 seconds.
- First recovery uses a short equipment reboot, currently targeting 10-15 seconds.
- Second recovery uses a short Owl-nest escape, currently targeting 20-30 seconds, plus a heavier non-adaptation penalty that is not yet selected.
- Near misses are sensory, physical contact disrupts an item or equipment, and full capture triggers the recovery loop.

Still requires a design decision before final behavior is implemented:

- exact equipment-reboot and nest-escape actions,
- exact non-adaptation penalty added after the second capture,
- exact authored scatter-placement rule,
- exact equipment disruption selected for physical-contact severity.

Game-wide direction that does not authorize new systems in the current sprint:

- Every future contract centers on an adaptive intelligent entity.
- Contracts are narratively employer-assigned and gameplay-randomized from eligible curated packages.
- Scenarios are isolated arcade episodes tied together by contractor fantasy, visual identity, and the enter/work/adapt/extract loop.
- Curated compatibility tags are preferred over unrestricted content combinations.
- These are product rules, not authorization to implement Contract 02 or a generalized contract generator.

## Current Adaptive-AI Handoff

Approved game-wide behavior:

- Entity memory lasts for one run and resets on run end or restart.
- One use creates suspicion; two repeated uses permit adaptation.
- Direct observation matters more than inferred evidence.
- Route repetition, hiding repetition, and distraction/stun reliance are the first three signal families.
- Tactical variety weakens confidence; repeated tactics strengthen it.
- Adaptation is communicated through behavior, animation, routes, and responses, never player-facing UI text.
- Bluffing is allowed, but may not use hidden exact player location.
- A pacing director may create believable back-off windows.

Implemented PT2 baseline:

- Route-family adaptation was implemented and automatically validated. Human design validation remains.

Not automatically authorized for the next version:

- hiding-pattern learning,
- distraction/stun-reliance learning,
- a generalized cross-entity learning framework,
- persistent memory between runs,
- full bluffing behavior,
- heart-rate-driven adaptation,
- expanding post-capture adaptation beyond the approved route experiment.
- new stun mechanics,
- a multi-item inventory or holster system,
- generalized spatial-hiding mechanics,
- new door, light, or machinery manipulation systems,
- heart-rate-driven stealth.
- a trained entity model, ML inference runtime, or Unity AI Inference package.

Still unresolved:

- the player behavior or critical affordance entities must never counter.

The current implemented route-confidence and decay values are recorded in `06_prototype_log.md` and the PT2 historical scope. Any future adaptation family requires its own tuning and evidence gate.

## Player-Tactics Direction

Approved product direction, not current-sprint authority:

- Entities are unkillable during the internship scope.
- Baseline counterplay includes spatial hiding/sight breaks, environmental debris, remote environmental noise, temporary close-range stun, door control, and mission equipment.
- Use a few explicit emergency hiding spots alongside spatial stealth.
- Do not add a bespoke carried noise-distractor gadget for the current direction.
- Stun starts near a 20-second cooldown and no farther than arm's length; it may not require impact, lunging, or boundary violation.
- Equipment is capped around two or three items with little persistent UI.
- Sprinting and careless physical interaction create meaningful noise.
- Universal contractor equipment and contract-specific equipment remain separate content categories.
- Entity learning is the primary cost of repeating a counterplay; adaptation must not silently disable it.

See `21_player_tactics_and_stealth_framework.md` before proposing implementation.

Objective/environment ideas such as a staging cart, ventilation/radiation control, movable shielding, and flashlight stealth are design candidates only. The no-cart one-rod-at-a-time loop remains authoritative. See `23_nuclear_recovery_objective_environment.md`; do not implement these candidates without a separate approved task.

## Entity-AI Technology Boundary

- The MLP uses deterministic adaptive game AI.
- The desired Alien-like experience does not authorize a neural or pre-trained model.
- D-014 remains active: do not reinstall Unity AI Inference.
- Any future learned model is a separate R&D task and may only propose bounded high-level strategy behind deterministic validation and fallback.
- See `22_ai_technology_strategy.md` for entry gates and forbidden responsibilities.

## Change-Control Rule

When a compelling idea appears during implementation, add it to the parking lot or propose a decision. Do not bundle it into the active task unless it is required to satisfy that task's acceptance criteria.
