# ClickUp Sprint 02 Paste Pack

Paste the instruction below into the linked ClickUp GPT app. Review the proposed tasks before allowing it to alter existing Sprint 01 items.

```text
Create or update Sprint 02 for the project "VR Extraction Horror Prototype".

Sprint dates: 2026-07-20 through 2026-07-24.
Tag every created task with "sprint-02" and "prototype-1-validation".
Do not delete, rename, or reopen existing Sprint 01 tasks.

Sprint goal:
Produce a coherent desktop playtest build in which a first-time tester understands the contractor job, identifies and transports live fuel rods, uses more than one response to the Owl, recovers from an initial capture, receives immediate feedback for a rod mistake, and reaches a clear extraction or failure outcome.

Create the following tasks. Preserve each ID at the start of its task name.

[Urgent] S2-001 Audit current three-seed baseline
List: 04 QA and Playtesting
Due: 2026-07-20
Description: Run the current build with three representative seeds. Record objective timing, first assay/deposit timing, captures, run outcome, total duration, broken flows, unclear cues, and tuning pain points. Clearly label all evidence desktop-only.
Acceptance criteria: Baseline notes exist for three seeds; blockers and design gaps are separated from polish requests.

[Urgent] S2-002 Finalize capture recovery v1 details
List: 01 Design
Due: 2026-07-20
Depends on: S2-001
Description: Specify the concrete actions for the 10-15 second first-capture equipment reboot and 20-30 second second-capture Owl-nest escape. Define five authored-zone selection rules, carried-item scatter range, exact second-stage adaptation parameters, anti-fast-travel behavior, approximately 60-second normal retreat, and approximately 20-30 second final-five-minute protection. Deposited rods remain safe and third capture is terminal for this test.
Acceptance criteria: Every capture transition and player-facing cue is explicit; no behavior-changing choice remains hidden in implementation; the rule cannot silently make the remaining run unwinnable.

[Urgent] S2-003 Implement and test capture recovery v1
List: 02 Technical
Due: 2026-07-21
Depends on: S2-002
Description: Replace instant capture failure with the approved two-recovery escalation. The Owl knocks the player unconscious through a short fade/audio transition with no graphic feeding or forced-camera sequence. First recovery uses equipment reboot. Second recovery uses a short nest escape and stronger approved adaptation. Keep capture and recovery in explicit run state, preserve deposits, scatter carried items only into valid authored positions, select among roughly five recovery zones, apply approved protection timing, and restore cleanly on restart.
Acceptance criteria: Equipment reboot and nest escape work; first and second captures are recoverable; the third ends the test run; no immediate recapture loop; automated or repeatable checks cover zone safety, item scatter, capture, recovery, escalation, protection timing, and reset.

[High] S2-004 Implement invalid-rod consequence v1
List: 02 Technical
Due: 2026-07-21
Depends on: S2-001
Description: On an invalid rod deposit, provide immediate physical/audio rejection plus a bounded noise or Owl-pressure event. Do not use a silent score-only penalty and do not cause instant failure.
Acceptance criteria: Consequence is immediate, clear, bounded, event-driven, and resets correctly.

[High] S2-005 Improve contractor contract onboarding
List: 01 Design
Due: 2026-07-22
Depends on: S2-001
Description: Make the paid recovery job, three-live-rod requirement, assay truth cue, and extraction goal understandable through concise mission presentation and diegetic feedback.
Acceptance criteria: A first-time tester can correctly state the objective within two minutes without developer explanation.

[High] S2-006 Validate mixed Owl counterplay
List: 01 Design
Due: 2026-07-22
Depends on: S2-001
Description: Verify hiding/sight breaks, distraction, and rerouting create at least two useful situational responses. Tune warning, search, and disengage rules. Add a limited stun only if the existing responses do not produce meaningful choice.
Acceptance criteria: At least two counterplay families are readable and useful; no one tactic universally solves the Owl; unavoidable catches are treated as blockers.

[High] S2-007 Add playtest telemetry and run summary
List: 02 Technical
Due: 2026-07-22
Depends on: S2-002 and S2-004
Description: Record run start/end, objective-understanding proxy events, assays, deposits, invalid attempts, captures, recovery duration, major Owl state/counterplay events, extraction, failure, and seed.
Acceptance criteria: An observer can reconstruct the important run sequence without relying on memory.

[Urgent] S2-008 Run regression and three-seed QA
List: 04 QA and Playtesting
Due: 2026-07-23
Depends on: S2-003, S2-004, S2-005, S2-006, S2-007
Description: Test clean launch, movement, carry, assay, deposits, invalid rods, Owl states, capture/recovery, timeout, holdout, exit, pause, same-seed restart, and new-seed restart on three representative seeds.
Acceptance criteria: No blocker or unexplained critical-path failure; results are attached to the task; a clean desktop build is produced.

[Urgent] S2-009 Conduct first-time desktop playtests
List: 04 QA and Playtesting
Due: 2026-07-24
Depends on: S2-008
Description: Run at least two observed first-time tests, target three. Do not coach after the scripted introduction. Capture objective clarity, pacing, captures/recovery, counterplays, invalid-rod interpretation, outcome, memorable moment, and replay intent.
Acceptance criteria: Session notes and prioritized findings exist for every tester; desktop results are not presented as VR comfort evidence.

[High] S2-010 Synthesize findings and choose Sprint 03
List: 00 Production and Planning
Due: 2026-07-24
Depends on: S2-009
Description: Separate bugs, clarity issues, tuning issues, comfort risks, and feature requests. Update design decisions, prototype status, risk register, and stakeholder summary. Select one Sprint 03 prototype question.
Acceptance criteria: Each finding has Keep, Change, Simplify, Delay, or Cut disposition; Sprint 03 has one evidence-based goal.

Also create these blocked placeholders without adding implementation subtasks:

[Blocked] Confirm target headset and runtime
List: 02 Technical
Blocker: Pixoul venue hardware decision/access.

[Blocked] Inventory and test physical devices
List: 03 Hardware and Haptics
Blocker: Office renovation and remote work; actual devices unavailable.

Place these items in 99 Parking Lot with status Cut or Deferred for Sprint 02:
- Pre-run loadouts or preparation economy.
- Score-based progression.
- Heart-rate-driven detection or win/loss.
- Additional objective types.
- Additional enemies.
- Runtime procedural geometry.
- Final art pass.

After creating the tasks, return a concise report listing created tasks, dependencies, blocked items, and any task you could not create. Do not make additional scope recommendations yet.
```

## Follow-Up Audit Prompt

```text
Audit the Sprint 02 tasks for the VR Extraction Horror Prototype. Check that only S2-001 and genuinely independent work can start before the baseline audit, that S2-008 is blocked by all implementation and readability work, and that S2-009 is blocked by S2-008. Flag scope that does not directly support the Sprint 02 prototype question. Do not edit tasks during this audit; report recommendations only.
```
