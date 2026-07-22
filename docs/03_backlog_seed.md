# Backlog Seed

Last updated: 2026-07-06

This backlog is intentionally coarse. It should be mirrored into ClickUp, then broken down as implementation context becomes real.

## Epic: Production and Planning

### Feature: Living Planning System

- Create project hub.
- Create vision brief.
- Create risk and scope register.
- Create decision log.
- Create weekly stakeholder report template.
- Create prototype log.
- Create playtest/QA template.

Acceptance criteria:

- Docs are easy to update weekly.
- Each doc has a clear purpose.
- ClickUp hierarchy reflects current sprint priorities.

## Epic: Core VR Prototype

### Feature: VR Player Baseline

- Confirm target headset and runtime.
- Set up VR project.
- Implement comfortable locomotion baseline.
- Implement snap turn or comfort turn settings.
- Define play area safety assumptions.
- Add simple interaction ray or hand interaction.

Acceptance criteria:

- Player can move through greybox space comfortably.
- No required rapid turning.
- Player can interact with a test object.
- Session can reset cleanly.

### Feature: Greybox Facility Slice

- Create start area.
- Create 3-6 connected rooms.
- Create objective room.
- Create extraction zone.
- Add readable lighting pass.
- Add simple signage or guidance.

Acceptance criteria:

- Player can navigate from start to objective and back to extraction.
- Objective route is understandable without long explanation.
- Greybox supports at least 2 route or hazard variations.

### Feature: Extraction Objective

- Define signal core object.
- Implement core pickup, scan, or activation.
- Implement extraction validation.
- Add basic objective UI or diegetic guidance.
- Add success/failure state.

Acceptance criteria:

- Player understands the objective within 2 minutes.
- Objective can be completed from start to extraction.
- End state is clear.

## Epic: Threat and Tension

### Feature: Threat Pressure Prototype

- Define first threat model.
- Implement threat states: idle, investigate, hunt, disengage.
- Add triggers from time, objective progress, noise, or proximity.
- Add audio/visual warning cues.
- Add debug display for current threat state.

Acceptance criteria:

- Threat pressure increases during the run.
- Player receives warning before major danger.
- Behavior feels responsive enough for a first prototype.

### Feature: Adaptive Behavior Prototype

- Track simple player signals: noise, hiding repetition, objective progress, time in danger zone.
- Convert signals into a pressure score or behavior bias.
- Tune response thresholds.
- Log behavior changes for debugging.

Acceptance criteria:

- At least one player behavior changes threat behavior.
- Behavior is understandable in playtest notes.
- System can be disabled for baseline testing.

## Epic: Replayability and Run Variation

### Feature: Run Variable System

- Define run seed or run configuration.
- Randomize core location.
- Randomize hazard or locked route state.
- Randomize threat aggression profile or spawn pressure.
- Add operator/debug readout of selected variables.

Acceptance criteria:

- At least 3 variables can change between sessions.
- Variables do not break the critical path.
- Run can be restarted quickly.

## Epic: Hardware, Haptics, and Biofeedback

### Feature: Hardware Abstraction

- Define device interface for haptics and sensors.
- Add simulated haptic device.
- Add simulated heart-rate input.
- Add device available/unavailable states.
- Add fallback behavior.

Acceptance criteria:

- Gameplay can run with hardware enabled, simulated, or disabled.
- Core game does not depend directly on vendor-specific APIs.
- Device failure does not break the session.

### Feature: First Meaningful Haptic Cue

- Choose one haptic/physical effect.
- Map effect to an in-world event.
- Define intensity limits and comfort rules.
- Implement simulated version first.
- Integrate real device if available.

Acceptance criteria:

- Testers can identify what the cue represents.
- Cue improves tension or comprehension.
- Cue has safe intensity limits.

## Epic: QA and Playtesting

### Feature: Comfort and Safety Checklist

- Define comfort test criteria.
- Define play area safety assumptions.
- Define stop conditions.
- Define nausea/fatigue feedback questions.
- Define hardware safety checks.

Acceptance criteria:

- Every test session captures comfort notes.
- Any high discomfort issue becomes a blocking bug.

### Feature: First Playtest Plan

- Create test script.
- Create observer checklist.
- Create bug report format.
- Create post-run questions.
- Define pass/fail criteria.

Acceptance criteria:

- A tester can run the script without developer improvisation.
- Feedback converts into prioritized action items.

## Epic: Stakeholder Communication

### Feature: Weekly Update Rhythm

- Create weekly report format.
- Summarize done/next/blockers/risks/asks.
- Prepare demo explanation template.

Acceptance criteria:

- Weekly update can be written in under 20 minutes.
- Stakeholders can understand progress without opening the build.

## Epic: Prototype 2.3 Validation and Tuning

### Feature: Hands-On Route and Environment Readability Pass

- Walk West, Central, and East runs at native desktop resolution with the operator overlay off.
- Repeat at least one seed and compare route adaptation on/off with F3.
- Inspect every terminal socket approach, portal fit, cover clearance, floor seam, and route-closure presentation while walking, sprinting, and jumping.
- Check room identity, mission-board, intake-rack, scan, rejection, extraction, and Owl cues while moving rather than from static screenshots.
- Tune lighting, material intensity, movement feel, message timing, and minor greybox placement only from observed problems.
- Preserve P2.2 as a scrappable environment pass; do not convert tuning into final art or a new gameplay family.

Acceptance criteria:

- A first-time player can distinguish the three route families and describe the objective without the operator overlay.
- Every required route and socket remains usable without jumping under all route profiles.
- At least two observed route-adaptation sessions are recorded, target three.
- Each tuning change cites a witnessed comprehension, navigation, comfort, or fairness problem.
