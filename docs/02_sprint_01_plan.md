# Sprint 01 Plan

Status: Complete; historical planning record

Outcome updated: 2026-07-18

Sprint dates: 2026-07-06 to 2026-07-10

Theme: Planning spine and first playable slice definition

## Sprint Goal

By Friday 2026-07-10, produce a clear, scoped plan for the first playable prototype and a ClickUp-ready backlog that lets development begin without ambiguity.

The first playable should aim to answer:

> Is it tense and understandable to enter a small VR facility, locate a core, sense a threat, and extract?

## Monday 2026-07-06: Planning Pro Max

Primary outcomes:

- Create living project docs.
- Lock current assumptions and open questions.
- Define Week 1 deliverables.
- Seed ClickUp hierarchy.
- Identify highest-risk unknowns.
- Draft first playable acceptance criteria.

Deliverables:

- Project hub.
- Vision brief.
- Sprint 01 plan.
- Backlog seed.
- Risk and scope register.
- Decision log.
- Prototype log.
- Playtest/QA template.
- ClickUp paste pack.

## Tuesday 2026-07-07: Prototype Definition

Recommended focus:

- Confirm engine and VR target hardware.
- Define greybox map requirements.
- Define first objective flow.
- Define first threat behavior in simple states.
- Define fake/simulated haptic interface if real hardware is not ready.

Exit criteria:

- First playable slice has a feature list, acceptance criteria, and cut lines.
- Core systems have rough ownership or implementation order.

## Wednesday 2026-07-08: Technical Skeleton Planning

Recommended focus:

- Choose architecture boundaries.
- Define hardware abstraction layer.
- Define gameplay event flow.
- Define run/session state structure.
- Define prototype debugging tools.

Exit criteria:

- Technical design outline is ready.
- Hardware-dependent features have simulation fallbacks.

## Thursday 2026-07-09: Build Prep and QA Prep

Recommended focus:

- Convert feature list into concrete implementation tasks.
- Prepare first test plan.
- Prepare first demo script.
- Confirm asset and environment needs.
- Confirm stakeholder update format.

Exit criteria:

- Developer can start implementation with clear task order.
- QA checklist exists before major work begins.

## Friday 2026-07-10: Review and Sprint 02 Setup

Recommended focus:

- Review planning docs with supervisor or relevant stakeholder.
- Resolve engine/hardware blockers if possible.
- Decide Sprint 02 prototype build goal.
- Update risks, decisions, backlog, and weekly report.

Exit criteria:

- Sprint 02 has one concrete playable target.
- Any blocked items have explicit asks.

## First Playable Candidate Scope

Historical note: this candidate was later superseded by the multi-path, three-rod scope in D-006 and the current Prototype 1 spec.

Must have:

- VR player locomotion and comfort baseline.
- Small greybox facility with start, objective room, and extraction point.
- One signal core pickup and carry or interact action.
- One threat pressure system, even if the entity is initially represented by audio, lights, shadows, or a simple proxy.
- One simulated haptic or physical cue.
- End state: extracted, failed, or timed out.

Should have:

- 2-3 variable objective or hazard placements.
- Simple debug panel or log for run variables.
- Operator reset flow.

Could have:

- Basic entity navigation.
- Heart-rate simulation input.
- Multiple core locations.

Won't have this sprint:

- Multiplayer.
- Full procedural generation.
- Final art.
- Complex enemy AI.
- Real haptic integration unless hardware access is confirmed.
- Persistent progression.

## Sprint Risks

- At sprint planning time, engine choice was unresolved; Unity was later confirmed in D-015.
- Hardware access may not be available immediately.
- Prototype scope may drift toward final game design too early.
- Horror tuning cannot be validated only on paper.

## Sprint Definition Of Done

- Docs exist and are usable.
- ClickUp seed is ready to paste.
- First playable scope is clear.
- Risks have owners or next actions.
- No major feature is accepted without a fallback version.

## Actual Outcome

- The living planning and documentation spine was created.
- Unity was confirmed as the engine.
- Prototype 1 reached approximately 90% implementation of its intended feature skeleton using desktop controls and simulated feedback.
- Basic movement, one threat, and the fuel-rod objective are playable but remain rudimentary and need human design validation and tuning.
- The target headset/runtime remains unknown.
- Real haptic and sensor devices were unavailable because the office was under renovation and the team was working remotely.
- The next sprint should improve and test the existing loop rather than introduce a new major system.

Important interpretation:

> Approximately 90% implementation of Prototype 1 is not 90% completion of the MLP. Objective clarity, pacing, threat fairness, recoverable capture, VR comfort, hardware behavior, and replay desire still require evidence.
