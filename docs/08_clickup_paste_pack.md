# ClickUp Paste Pack

Status: Historical Sprint 01 setup pack. Use `16_clickup_sprint_02_paste.md` for current work.

Use this as source material for ClickUp tasks. Adjust names/statuses to match the team's ClickUp workflow.

## Suggested ClickUp Structure

Space:

- Internship Game Development

Folder:

- VR Extraction Horror Prototype

Lists:

- 00 Production and Planning
- 01 Design
- 02 Technical
- 03 Hardware and Haptics
- 04 QA and Playtesting
- 05 Stakeholder Updates
- 99 Parking Lot

## Statuses

Recommended statuses:

- Backlog
- Ready
- In Progress
- Blocked
- Review
- Done
- Cut or Deferred

## Priorities

Recommended priorities:

- Urgent: blocks playable prototype or demo reliability.
- High: directly supports first playable loop or major risk reduction.
- Normal: useful but not blocking.
- Low: polish or future-facing idea.

## Custom Fields

Useful custom fields:

- Sprint
- Epic
- Risk Level
- Prototype Question
- Acceptance Criteria
- Hardware Required
- VR Comfort Risk
- Fallback Plan
- Demo Critical

## Pasteable Tasks For ClickUp GPT

You can paste this instruction into the ClickUp GPT app:

```text
Create a ClickUp folder called "VR Extraction Horror Prototype" under the internship/game development space if it does not already exist.

Create these lists:
- 00 Production and Planning
- 01 Design
- 02 Technical
- 03 Hardware and Haptics
- 04 QA and Playtesting
- 05 Stakeholder Updates
- 99 Parking Lot

Use these statuses if possible:
- Backlog
- Ready
- In Progress
- Blocked
- Review
- Done
- Cut or Deferred

Create Sprint 01 tasks for 2026-07-06 to 2026-07-10 using the task list below. Add the tag "sprint-01" to all of them. Add due dates where provided. Use priority High for anything marked [High], Normal for [Normal], and Low for [Low].

Task list:

[High] Create project hub
List: 00 Production and Planning
Due: 2026-07-06
Description: Create the central index for project docs, links, current assumptions, open questions, and this week's goal.
Acceptance criteria: Project hub exists, links to key docs, and includes current phase, delivery target, and live questions.

[High] Create vision brief
List: 01 Design
Due: 2026-07-06
Description: Capture the approved business case and pitch direction in a living one-page-ish design brief.
Acceptance criteria: Includes executive summary, pillars, core loop, success metrics, non-goals, and quality attributes.

[High] Create risk and scope register
List: 00 Production and Planning
Due: 2026-07-06
Description: Create a risk table and scope guardrails for hardware, procedural generation, adaptive AI, VR comfort, demo stability, and deadline risk.
Acceptance criteria: Each major risk has an impact, probability, early warning, response, and fallback.

[High] Create decision log
List: 00 Production and Planning
Due: 2026-07-06
Description: Create a log for project decisions affecting scope, architecture, design, schedule, or stakeholder expectations.
Acceptance criteria: Includes initial decisions and pending decisions for engine, headset, hardware integration, locomotion, and Sprint 02 target.

[High] Define Sprint 01 plan
List: 00 Production and Planning
Due: 2026-07-06
Description: Define the planning-focused sprint for 2026-07-06 to 2026-07-10.
Acceptance criteria: Includes daily focus, sprint goal, first playable candidate scope, risks, and definition of done.

[High] Confirm engine and VR runtime
List: 02 Technical
Due: 2026-07-07
Description: Confirm whether the project will use Unity, Unreal, or another engine, and identify the target VR runtime/headset.
Acceptance criteria: Engine, headset, runtime, and any Pixoul constraints are recorded in the decision log.

[High] Confirm available Pixoul hardware integrations
List: 03 Hardware and Haptics
Due: 2026-07-08
Description: Identify which haptic suits, room effects, controllers, heart-rate sensors, or other devices are available for development and testing.
Acceptance criteria: Hardware list includes device name, SDK/API status, availability, owner/contact, and fallback plan.

[High] Define first playable prototype scope
List: 01 Design
Due: 2026-07-08
Description: Define the first playable prototype as a small VR extraction loop with greybox facility, one signal core, one threat pressure cue, and simulated haptics.
Acceptance criteria: Scope includes must/should/could/won't, success criteria, and test question.

[High] Draft technical architecture outline
List: 02 Technical
Due: 2026-07-09
Description: Define high-level systems for player, interaction, session/run state, objective, threat, hardware abstraction, events, and debugging.
Acceptance criteria: Architecture separates core gameplay from hardware-specific APIs and includes simulated hardware fallbacks.

[Normal] Draft first playtest and QA template
List: 04 QA and Playtesting
Due: 2026-07-09
Description: Create a template for comfort, objective clarity, threat tension, haptics, bugs, and post-test questions.
Acceptance criteria: Template can be used by an observer during the first internal test.

[Normal] Prepare weekly stakeholder report template
List: 05 Stakeholder Updates
Due: 2026-07-10
Description: Create a short weekly update format covering done, next, blockers, risks, decisions, and asks.
Acceptance criteria: Template can be filled in under 20 minutes.

[High] Decide Sprint 02 playable target
List: 00 Production and Planning
Due: 2026-07-10
Description: Choose the concrete playable target for the next sprint based on engine, hardware, and risk findings.
Acceptance criteria: Sprint 02 has one prototype question, must-have features, cut lines, and acceptance criteria.
```

## ClickUp GPT Follow-Up Prompt

Use this after the tasks are created:

```text
Review the Sprint 01 tasks for the VR Extraction Horror Prototype. Group them by list, flag missing dependencies, and suggest which tasks should be marked blocking or blocked. Keep the recommendation practical for a one-person intern-led project with a deadline of 2026-08-28.
```
