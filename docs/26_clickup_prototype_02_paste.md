# ClickUp Prototype 2 Paste Pack

Status: Active PT2 task-creation prompt

Paste this into the ClickUp GPT app:

```text
Create a Prototype 2 task group for "VR Extraction Horror Prototype".

Prototype name: PT2 - Readable Route Adaptation
Target work window: 2026-07-22 through 2026-07-24
Tags: prototype-02, readable-adaptation, owl-ai, desktop-validation

Authoritative scope: docs/24_prototype_02_scope.md
Agent handoff: docs/25_prototype_02_agent_handoff.md

Do not delete existing Sprint 02 tasks. Mark unstarted Sprint 02 work that is outside PT2 as Deferred or move it to Backlog after user review. Do not mark existing work complete without evidence.

Create these tasks:

[Urgent] PT2-001 Baseline threat audit
List: 04 QA and Playtesting
Due: 2026-07-22
Description: Run existing focused ThreatPressureModel and relevant threat tests. Confirm current repeated-wing bias, reset behavior, and any baseline failures before editing.
Acceptance criteria: Baseline test results and current behavior are recorded; unrelated failures are separated.

[Urgent] PT2-002 Implement route evidence model
List: 02 Technical
Due: 2026-07-22
Depends on: PT2-001
Description: Extend the pure threat-pressure model with per-wing confidence, Direct/Inferred evidence, Uncertain/Suspicious/Adapted/Counter-Adapted state, configurable thresholds, inferred-evidence cooldown, decay, route-variety reduction, complete reset, and baseline-disable behavior. Use docs/24 and docs/25 exactly.
Acceptance criteria: Focused tests cover thresholds, evidence hierarchy, cooldown, counter-adaptation, decay, reset, deterministic streams, bias cap, and disabled mode.

[Urgent] PT2-003 Wire credible route evidence
List: 02 Technical
Due: 2026-07-23
Depends on: PT2-002
Description: Remove unconditional entity knowledge from zone entry. Record Direct evidence only with plausible perception. Convert sufficiently strong route-local stimuli into lower-weight Inferred evidence without exact hidden-location knowledge or duplicate counting.
Acceptance criteria: Silent unperceived entry adds no evidence; witnessed use adds Direct evidence; valid stimulus may add Inferred evidence; telemetry retains source and kind.

[High] PT2-004 Express suspicion, adaptation, and counter-adaptation
List: 02 Technical
Due: 2026-07-23
Depends on: PT2-003
Description: Give Suspicious mild route inspection, Adapted capped patrol bias, and varied routes a useful counter-adaptation opening. Prevent indefinite same-wing patrol. Use existing route/search/orientation and restrained presentation cues only.
Acceptance criteria: States differ behaviorally; no route is locked; route change reduces old attention; no final art or player UI notification is added.

[High] PT2-005 Add operator telemetry and F3 comparison toggle
List: 02 Technical
Due: 2026-07-23
Depends on: PT2-002
Description: Extend F1 operator information with state, West/Central/East confidence, dominant wing, last evidence source/kind, timing, and response. Add F3 adaptation enabled/disabled comparison. Disable clears memory and restores baseline weights.
Acceptance criteria: Observer can reconstruct transitions; F3 works reliably; normal player mission HUD is unchanged.

[Urgent] PT2-006 Run regression and desktop build gate
List: 04 QA and Playtesting
Due: 2026-07-24
Depends on: PT2-003, PT2-004, PT2-005
Description: Run focused and relevant regression tests, same-seed evidence replay, disabled-mode check, fresh desktop build, and launch smoke test where supported.
Acceptance criteria: Exact results are attached; no critical-path regression is hidden; limitations are explicit.

[High] PT2-007 Collect human adaptation evidence
List: 04 QA and Playtesting
Due: 2026-07-24
Depends on: PT2-006
Description: Run at least two observed sessions, target three, without explaining the adaptation rule. Capture perceived behavior change, inferred cause, route response, fairness, and enabled/disabled comparison where practical.
Acceptance criteria: Every human acceptance criterion is Pass, Fail, or Not Tested; observations are separated from interpretation; prototype log is updated.

Create a separate Deferred group containing these items. Do not create implementation subtasks for them:
- Capture recovery and second-capture penalty.
- Hiding-pattern and distraction/stun learning.
- Bluffing.
- Electrical probe and inventory.
- Flashlight stealth and graded stealth states.
- Heart-rate slider, simulator, sensor, and breathing stealth.
- Cart, ventilation/radiation controls, and movable shielding.
- Learned/neural AI or inference packages.
- VR headset and physical hardware integration.

After task creation, return a concise report of tasks created, dependencies, deferred items, and anything you could not create. Do not expand PT2 scope.
```

## Audit Prompt

```text
Audit PT2 tasks without editing them. Confirm PT2-001 precedes model work, PT2-002 precedes evidence wiring and telemetry, and PT2-006 blocks human testing. Flag any task that adds a gameplay family outside docs/24_prototype_02_scope.md. Treat tests, operator telemetry, baseline toggle, and human evidence as required work, not polish.
```
