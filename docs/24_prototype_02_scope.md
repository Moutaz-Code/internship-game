# Prototype 2 Scope: Readable Route Adaptation

Status: Authoritative implementation scope

Scope frozen: 2026-07-22

Target gate: 2026-07-24

## Naming

- Prototype 1: the current multi-path fuel-rod vertical slice. It was historically labeled `P2` in the prototype log; that label is retired.
- Prototype 2 / `PT2`: the Readable Route Adaptation experiment defined here.
- Sprint 02 and Prototype 2 are different labels. Sprint 02 is the production timebox; PT2 is the build experiment.

## Prototype Question

> Does the Owl appear to notice repeated route-family use, become suspicious after one credible observation, adapt after two, and encourage the player to vary routes without feeling omniscient or making any route unusable?

## Why This Prototype Now

- Adaptive intelligence is a core product pillar.
- The current build already has a deterministic `ThreatPressureModel`, repeated-wing tracking, temporary patrol bias, seeded runs, stimuli, navigation, operator status, and tests.
- Current behavior treats all zone entry as knowledge and exposes only the final biased wing. It does not yet distinguish direct observation from inference, express suspicion, or show counter-adaptation clearly.
- This experiment can produce meaningful evidence before the end of the week without adding a new content or hardware family.

## Existing Code Context

Primary implementation surfaces:

- `Assets/_Project/Scripts/Threats/ThreatPressureModel.cs`
- `Assets/_Project/Scripts/Threats/PursuitThreatController.cs`
- `Assets/_Project/Scripts/Threats/ThreatContracts.cs`
- `Assets/_Project/Scripts/Run/PrototypeHudPresenter.cs`
- `Assets/_Project/Tests/EditMode/Threats/ThreatPressureModelTests.cs`

Related systems that should remain stable:

- `IThreatNavigator` implementations,
- `PursuitThreatStateMachine`,
- `IThreatStimulusSink`,
- authored route profiles and patrol points,
- `RunConfiguration` and restart behavior,
- F1 operator overlay,
- F2 Geiger-interference comparison mode.

## Must Have

### 1. Run-Only Route Memory

- Track evidence independently for West, Central, and East route families.
- Reset all memory on same-seed restart and new-seed restart.
- Do not persist memory to disk, profiles, later sessions, or other contracts.

### 2. Evidence Hierarchy

- Direct evidence: the Owl plausibly sees the player enter, traverse, or leave a route family.
- Inferred evidence: the Owl receives a sufficiently strong route-local stimulus such as sprinting, landing, dropped metal, or objective activity.
- Global zone entry without perception is not knowledge and contributes no evidence.
- Direct evidence has twice the starting weight of inferred evidence.

Prototype defaults:

- direct evidence: `1.0`,
- inferred evidence: `0.5`,
- Suspicious threshold: `1.0`,
- Adapted threshold: `2.0`,
- repeated inferred evidence from the same wing is rate-limited so one noisy moment cannot instantly adapt the Owl.

Values must be configurable and covered by tests.

### 3. Adaptation States

Per dominant route family:

| State | Default Rule | Behavior |
|---|---|---|
| Uncertain | Confidence below `1.0`. | Existing baseline patrol. |
| Suspicious | Confidence `1.0` to below `2.0`. | Mild route inspection or patrol preference; no camping or interception certainty. |
| Adapted | Confidence `2.0` or above. | Capped patrol/search allocation toward the route using the existing maximum bias weight. |
| Counter-Adapted | Credible behavior shifts to other route families. | Old confidence weakens and attention redistributes. |

### 4. Tactical Variety

- Credible evidence in another wing reduces confidence in the previously dominant wing.
- Confidence also decays when not reinforced.
- A changed route must create a useful opening before the Owl develops a new prediction.
- Adaptation pressures a route; it never locks the route, teleports the Owl, or grants exact hidden location.

Prototype tuning targets:

- retain the existing adapted patrol weight cap of `1.75`,
- retain the existing approximate 60-second bias window as the first comparison value,
- after at most two consecutive biased patrol selections, force patrol consideration outside the dominant wing.

### 5. Diegetic Expression

Player-facing adaptation has no text notification, meter, or icon.

Use the smallest available combination of:

- changed patrol selection,
- visible orientation or inspection of the suspected route,
- longer search allocation near the route,
- an existing Owl audio/visual presentation cue at a restrained intensity.

Do not create final animation or art solely for PT2. A readable prototype proxy is acceptable.

### 6. Operator Telemetry

Extend F1 operator information to expose:

- adaptation enabled/disabled,
- current state: Uncertain, Suspicious, Adapted, or Counter-Adapted,
- confidence for West, Central, and East,
- dominant wing,
- last evidence source: Direct or Inferred,
- last evidence kind and position/zone,
- remaining bias or decay timing,
- selected patrol response,
- reset confirmation.

Operator information must never appear as player-facing adaptation feedback.

### 7. Baseline Toggle

- Provide a desktop comparison toggle, recommended `F3`, for adaptation enabled/disabled.
- Disabling adaptation clears route memory and returns all patrol weights to baseline.
- Toggle state appears in F1 operator information.
- The normal player-facing mission panel remains unchanged.

## Should Have

- A deterministic telemetry line or short run summary listing adaptation transitions.
- Same-seed evidence replay produces the same confidence and state transitions.
- Suspicion and adaptation can be tuned without scene reconstruction.

## Could Have

- One bounded fake-disengage or unexpected-route inspection using no hidden player location, but only after every Must Have item and test passes.

## Explicitly Out Of Scope

- Capture/recovery redesign, nest escape, or second-capture penalty.
- New stun, inventory, holster, flashlight-stealth, crouch, hiding-memory, door, ventilation, radiation, movable-cover, or cart systems.
- Heart-rate sensor, slider, simulator, breathing-stealth implementation, or other biofeedback work.
- Hiding-pattern or distraction/stun-reliance adaptation.
- Cross-run memory or customer profiles.
- Learned/neural AI, Unity AI Inference, or another ML runtime.
- New entity, objective, map wing, final art, or final animation.
- Real headset or physical-hardware integration.

If any excluded work already exists locally, preserve it and keep it disabled or separately testable. Do not expand it under PT2.

## Functional Acceptance Criteria

- Memory resets on every run restart.
- Unperceived zone entry creates no evidence.
- One direct observation creates Suspicious, not Adapted.
- Two repeated direct observations can create Adapted.
- Two corroborating inferred events can equal one direct observation under default weights.
- Inferred evidence is rate-limited and never supplies exact hidden location.
- Route variety weakens the previous dominant confidence.
- Adapted patrol weight never exceeds the configured cap.
- The Owl cannot remain indefinitely in one wing because of adaptation.
- Adaptation-disabled mode returns baseline patrol weights and clears memory.
- Equal evidence streams produce equal state transitions.
- Existing threat navigation, state-machine, run-reset, objective, assay, extraction, and Geiger tests remain green.

## Human Acceptance Criteria

Run at least two observed desktop sessions, target three.

- A tester notices changed Owl route/search behavior without an adaptation notification.
- After the run, the tester can connect the change to repeated route use.
- The tester attempts or describes route variation as a response.
- Changing route creates a believable temporary advantage.
- The Owl feels more observant, not omniscient or random.
- No route becomes functionally unusable.

## Failure Conditions

PT2 fails its question if:

- players cannot perceive the adaptation,
- players perceive ordinary random patrol as learning,
- the Owl acts on information it could not plausibly possess,
- changed routes do not reduce pressure,
- adaptation causes camping or unavoidable capture,
- debug telemetry cannot explain why a transition happened,
- implementation destabilizes the existing end-to-end loop.

## Definition Of Done

- Must Have behavior is implemented behind a comparison toggle.
- Focused Edit Mode tests pass.
- Existing relevant Edit Mode and Play Mode tests pass.
- A fresh desktop build launches.
- At least two human sessions are documented, or the missing tester access is explicitly recorded.
- `06_prototype_log.md` contains implementation and human evidence.
- Any failed acceptance criterion becomes a prioritized follow-up, not hidden polish work.
