# Agent Implementation Handoff

Status: Active project protocol

Last updated: 2026-07-20

## Purpose

Another AI agent actively reads project documents and implements the game. This protocol prevents exploratory design notes from becoming accidental implementation scope.

## Source Priority

When documents disagree, use this order:

1. The newest explicit user instruction.
2. Active entries in `05_decision_log.md`.
3. The current contract spec in `09_first_playable_spec.md`.
4. Current-sprint tasks in `15_sprint_02_plan.md` and ClickUp.
5. Current architecture constraints in `10_technical_architecture_outline.md`.
6. Prototype evidence in `06_prototype_log.md`.
7. Design synthesis and framework documents.
8. Workbooks, examples, parking-lot ideas, and historical sprint documents.

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

## Current Authorized Content Scope

The current implementation target is one proof contract:

- post-apocalyptic nuclear facility,
- identify and transport three live fuel rods,
- Owl Thing entity,
- seeded authored variation,
- extraction holdout and exit.

Do not implement a second mission, second entity, unrestricted mission generator, persistent progression, loadout economy, or generalized content framework beyond the smallest interfaces needed by the current slice.

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

## Current Capture Handoff

Approved for Prototype 1 testing:

- Owl attack causes unconsciousness through a VR-safe fade/audio transition.
- No direct or graphic feeding depiction.
- First and second captures are recoverable; third is terminal for the current test.
- Completed deposits remain safe.
- Carried items scatter only into valid authored positions.
- Recovery selects from roughly five authored zones using safety and anti-fast-travel rules.
- Owl retreats for about 60 seconds outside the final five minutes.
- During the final five minutes, protection lasts approximately 20-30 seconds.
- First recovery uses a short equipment reboot, currently targeting 10-15 seconds.
- Second recovery uses a short Owl-nest escape, currently targeting 20-30 seconds, and stronger adaptation.
- Near misses are sensory, physical contact disrupts an item or equipment, and full capture triggers the recovery loop.

Still requires a design decision before final behavior is implemented:

- exact equipment-reboot and nest-escape actions,
- exact adaptation parameters added after the second capture,
- exact authored scatter-placement rule,
- exact equipment disruption selected for physical-contact severity.

Game-wide direction that does not authorize new systems in the current sprint:

- Every future contract centers on an adaptive intelligent entity.
- Contracts are narratively employer-assigned and gameplay-randomized from eligible curated packages.
- Scenarios are isolated arcade episodes tied together by contractor fantasy, visual identity, and the enter/work/adapt/extract loop.
- Curated compatibility tags are preferred over unrestricted content combinations.
- These are product rules, not authorization to implement Contract 02 or a generalized contract generator.

## Change-Control Rule

When a compelling idea appears during implementation, add it to the parking lot or propose a decision. Do not bundle it into the active task unless it is required to satisfy that task's acceptance criteria.
