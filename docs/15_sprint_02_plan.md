# Sprint 02 Plan

Status: Timebox record; remaining 2026-07-22 to 2026-07-24 implementation is frozen by Prototype 2 documents 24-26.

Scope-freeze note:

- Preserve completed Sprint 02 work.
- Pause unstarted capture, invalid-rod, onboarding, and broader counterplay implementation unless it is required by PT2 or the user explicitly redirects scope.
- Current authoritative goal is PT2 Readable Route Adaptation in `24_prototype_02_scope.md`.
- Tests, telemetry, baseline comparison, and human evidence are required PT2 work rather than optional polish.

Sprint dates: 2026-07-20 to 2026-07-24

Theme: Turn the Prototype 1 skeleton into credible human playtest evidence

## Sprint Goal

By Friday 2026-07-24, produce a coherent desktop playtest build in which a first-time tester can understand the contractor job, identify and transport live fuel rods, use more than one response to the Owl, recover from an initial capture, receive immediate feedback for a rod mistake, and reach a clear extraction or failure outcome.

## Prototype Question

> Can a new desktop tester understand the contract, assay and transport rods, respond to the Owl in more than one way, recover from a serious mistake, and finish or meaningfully fail within the target session length?

## Evidence Target

Run at least three observed first-time desktop playtests if tester access allows. Two is the minimum useful fallback. The developer should not explain controls or objectives after the scripted introduction unless the test is stopped.

Collect:

- time to state the objective correctly,
- time to first assay and first valid deposit,
- number and cause of captures,
- time needed to resume useful play after capture,
- counterplays attempted,
- invalid-rod attempts and player interpretation,
- total run time and outcome,
- one comfort/readability proxy note, clearly labeled desktop-only,
- replay intent and reason.

## Scope

Must have:

- Stable current desktop movement, interaction, objective, Owl, timer, extraction, and restart flow.
- Two-capture recovery loop with clear feedback, authored recovery zones, preserved deposits, scattered carried items, and bounded recovery cost.
- Immediate, understandable invalid-rod consequence.
- At least two readable Owl counterplay families in the build.
- Clear contract onboarding that can be understood within two minutes.
- Basic event/timestamp logging for playtest reconstruction.
- A distributable build and observer-ready test script.

Should have:

- Tuning controls for capture penalty and Owl pressure.
- Same-seed and new-seed restart verified.
- Three representative seeds selected for comparison.
- Simple operator summary at run end.

Could have:

- One limited stun implementation if the existing distraction, sight-break, and reroute options do not produce enough choice.
- Small presentation improvements that directly solve observed comprehension issues.

Will not have this sprint:

- Real headset validation.
- Real haptic, heat, wind, or heart-rate device integration.
- Loadout or preparation choices.
- Score as a primary system.
- New objective types, enemies, facility wings, or procedural geometry.
- Final art polish.

## Work Order

### Monday: Baseline And Rules

- Play the current build across three representative seeds.
- Record actual run time, broken flows, unclear cues, and tuning pain points.
- Specify the exact equipment-reboot and nest-escape actions, non-adaptation second-capture penalty, carried-item scatter rule, and invalid-rod test rule.
- Create the Sprint 02 ClickUp tasks and mark dependencies.
- Freeze additions that do not answer the sprint question.

Exit condition: the current baseline and two rule changes are documented well enough to implement and test.

### Tuesday: Recovery And Consequences

- Implement two survivable captures and terminal third capture as a tunable test rule.
- Preserve completed deposits, scatter carried items into valid authored positions, choose among roughly five recovery zones, and prevent recapture loops.
- Apply an approximately 60-second Owl retreat outside the final five minutes.
- Apply approximately 20-30 seconds of protection during the final five minutes.
- Use equipment reboot for first recovery and a short nest escape plus the approved non-adaptation penalty for second recovery.
- Implement immediate invalid-rod feedback and bounded Owl pressure.
- Add debug controls for penalty values.

Exit condition: capture and invalid-rod behavior work deterministically and reset cleanly.

### Wednesday: Readability And Counterplay

- Improve contract onboarding and rod-truth communication.
- Verify at least two useful Owl responses are readable.
- Tune warning, search, disengage, and recovery windows.
- Add only the smallest missing counterplay needed for meaningful choice.
- Do not implement the broader stun, inventory, spatial-hiding, environmental-manipulation, or heart-rate concepts from the design framework during this sprint without a separate approved task.
- Do not add a trained entity model, ML inference runtime, or inference package; route adaptation v1 uses deterministic game AI.

Exit condition: a developer-free run is plausible from start through extraction.

### Thursday: QA And Playtest Build

- Run functional regression and three-seed checks.
- Verify timer, capture, deposits, holdout, outcomes, pause, and restart.
- Prepare a clean desktop build and playtest script.
- Conduct one pilot test and fix only blockers or high-severity clarity issues.

Exit condition: build can be handed to a tester without developer setup.

### Friday: Human Test And Decision Pass

- Run two or more additional first-time playtests.
- Separate bugs, comprehension issues, tuning issues, and feature requests.
- Update prototype log, risks, decisions, and stakeholder report.
- Decide Sprint 03 from evidence, not idea volume.

Exit condition: results support a clear keep, change, simplify, or cut decision for capture, rod mistakes, and Owl counterplay.

## Task Breakdown

| ID | Task | Priority | Depends On | Done When |
|---|---|---|---|---|
| S2-001 | Audit current three-seed baseline | Urgent | None | Timings, blockers, and known design gaps are recorded. |
| S2-002 | Finalize capture recovery v1 details | Urgent | S2-001 | Reboot/nest actions, five-zone selection, scatter range, grace timing, non-adaptation second-stage penalty, and anti-exploit rules are explicit. |
| S2-003 | Implement and test capture recovery v1 | Urgent | S2-002 | First reboot and second nest recovery work, third capture ends the run, carried items scatter safely, deposits persist, and no immediate recapture loop occurs. |
| S2-004 | Specify and implement invalid-rod consequence v1 | High | S2-001 | Rejection is immediate, understood, bounded, and not score-only. |
| S2-005 | Improve contract onboarding | High | S2-001 | A naive tester can state the job within two minutes. |
| S2-006 | Validate mixed Owl counterplay and route adaptation v1 | High | S2-001 | At least two responses remain useful, one use creates route suspicion, two repeated uses can create a readable capped bias, and route variation weakens that prediction. |
| S2-007 | Add playtest telemetry and run summary | High | S2-002, S2-004 | Key timestamps, capture recovery, counterplay, and outcome can be reconstructed. |
| S2-008 | Run regression and three-seed QA | Urgent | S2-003 through S2-007 | No blocker in start-to-end flow; same/new seed restart works. |
| S2-009 | Conduct first-time desktop playtests | Urgent | S2-008 | At least two, target three, observed sessions are documented. |
| S2-010 | Synthesize findings and choose Sprint 03 | High | S2-009 | Findings are prioritized and the next prototype question is written. |

## Acceptance Criteria

- The contract is understood within two minutes by most first-time testers.
- The first two captures produce clear escalating setbacks and realistic paths back into useful play; a third capture ends the current test run clearly.
- No observed player is trapped in an immediate recapture loop.
- Invalid-rod feedback is understood without relying on end-of-run score.
- Testers identify or attempt at least two Owl responses across the test set.
- At least one tester reaches extraction or a clearly understood terminal failure.
- Total duration trends toward 15-20 minutes or produces a specific pacing change.
- The build launches, ends, and restarts without developer-only intervention.

## Sprint Risks

- Too much time is spent polishing the already broad prototype instead of testing it.
- Capture recovery may be exploitable or may create a hidden unwinnable state.
- A smart-threat aspiration may produce unreadable behavior rather than perceived intelligence.
- Desktop findings may be mistaken for VR comfort evidence.
- First-time testers may be difficult to recruit while working remotely.

## Decision Rule For New Ideas

Add an idea to this sprint only if it is the smallest credible way to answer the prototype question or remove a blocker from the observed test. Otherwise, place it in the parking lot.
