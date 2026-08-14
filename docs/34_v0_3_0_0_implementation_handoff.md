# v0.3.0.0 Implementation Handoff

Status: Active implementation handoff

Last updated: 2026-08-01

## One-Sentence Assignment

Deliver a substantial, testable stealth-and-counterplay pass in the existing fuel-rod contract: graded visual confirmation, movement noise, fair evidence priority, spatial hiding, portable metal distractions, three fixed machinery distractions, diegetic feedback, and complete operator explanation.

## Read Order

Before changing code or Unity state, read:

1. `33_v0_3_0_0_readable_stealth_scope.md`
2. this handoff
3. `implementation_agent/01_authority_and_scope.md`
4. `implementation_agent/02_current_implementation_state.md`
5. `implementation_agent/03_system_ownership_map.md`
6. `implementation_agent/04_operating_and_validation_procedure.md`
7. `10_technical_architecture_outline.md`
8. `27_development_versioning.md`
9. current code and tests in the affected systems

The scope brief overrides broader workspaces and future frameworks for `v0.3.0.0`.

## Verified Starting Point

Read-only audit on 2026-08-01 found:

- branch `master` at published `v0.2.3.0` commit `0636f5d`;
- Unity `6000.5.4f1`, project `UnityProject/InternshipGameSynced`;
- main scene `Assets/_Project/Scenes/P01_CoreLoop.unity` open and clean in Edit Mode;
- build identity still `v0.2.3.0`;
- 117 discoverable tests: 111 Edit Mode and 6 Play Mode;
- no current project-code console error; only prior MCP disconnect warnings;
- a dirty working tree containing documentation and an unrelated OpenXR settings change.

Re-audit all of this. Preserve every unrelated dirty or untracked file. Do not alter or publish the existing OpenXR settings change unless the user separately assigns it.

## Important Existing Behavior And Gaps

- The Owl has three cover-respecting player samples, but any one visible sample currently counts as line of sight.
- Sprint and landing emit stimuli; ordinary walk and crouch locomotion do not.
- Any nonzero evaluated sound currently updates the Owl's last-known target; there is no investigation threshold.
- A thrown prop can currently replace the target even while the Owl sees the player. This is contrary to the approved v0.3 priority rule.
- Route-evidence acceptance already uses a separate `0.30` strength minimum.
- Six throwable metal props already use `GrabbableObject` and `ThrowableThreatStimulus`.
- `IPlayerInteractable` and the desktop look-and-use path already exist.
- Crouch is publicly available on `DesktopFirstPersonController` but is not yet exposed through `IPlayerMotionSource`.
- Existing feedback provides Owl warning/detection/proximity, but no locomotion footstep family, accepted metal impact, or distinct investigation cue.
- The facility is generated. Durable content belongs in the builder and must be regenerated through Unity MCP.

## Technical Boundaries

- Extend the existing threat contracts and controller; do not replace navigation or the state machine.
- Keep classification, accumulation, audibility, and target-priority decisions in small pure policies where practical.
- A narrow posture property on the existing controller-independent motion source is acceptable. Do not introduce a generalized player-stat framework.
- Add the approved typed `Machinery` stimulus and route-evidence kinds through the existing threat contracts. Do not create a global event bus.
- Use the existing feedback hub or localized AudioSource patterns. Keep cue meanings distinct.
- Change C# and Markdown as files. Use Unity MCP for scene, generated-root rebuild, serialized asset/settings changes, compilation, tests, screenshots, and build.
- Do not hand-edit `.unity`, `.asset`, prefab, material, or Project Settings files.

## Task Order

### V030-001: Baseline And Task Record

Priority: Urgent

Actions:

- record the approved task, authority, version, observable behavior, acceptance criteria, affected systems, and no unresolved design alternatives;
- audit Git, bundle version, editor state, scene, console, relevant serialized values, and dirty files;
- run the narrow existing threat/perception/motion tests before editing.

Done when the baseline is reported and any pre-existing failure is separated from v0.3 work.

### V030-002: Pure Perception Policies

Priority: Urgent

Likely files:

- `Assets/_Project/Scripts/Threats/ThreatContracts.cs`
- `Assets/_Project/Scripts/Threats/ThreatPerceptionMath.cs`
- one or two new focused threat policy files
- Edit Mode tests beside the existing threat tests

Implement and test:

- Concealed/Noticeable/Exposed classification from visible sample count;
- confirmation gain, sprint multiplier, crouch/slow Noticeable rate, decay, clamp, and reset;
- hearing acceptance at evaluated strength `>= 0.15`;
- knowledge-source and target-priority selection;
- confirmed sight outranking sound;
- rejected sound causing no target change.

Do not bury these rules entirely inside a MonoBehaviour.

### V030-003: Player Motion Noise And Feedback

Priority: Urgent

Likely files:

- `Assets/_Project/Scripts/Player/PlayerMotionSource.cs`
- `Assets/_Project/Scripts/Player/DesktopFirstPersonController.cs`
- `Assets/_Project/Scripts/Threats/PlayerThreatStimulusEmitter.cs`
- existing feedback contracts/output and builder wiring

Implement:

- narrow posture exposure through the current motion seam;
- still/crouch/walk/sprint band selection using actual planar speed;
- configured pulses and deterministic/rate-limited step timing;
- graded footstep audio with clearly readable sprint;
- clean enable/disable and run reset;
- focused tests for band choice, values, and no stationary emission.

Do not copy desktop camera motion or bindings into XR.

### V030-004: Owl Integration And Evidence Priority

Priority: Urgent

Primary file:

- `Assets/_Project/Scripts/Threats/PursuitThreatController.cs`

Actions:

- feed all three current sample results into the pure visual policy;
- distinguish actionable visual contact from partial unconfirmed exposure;
- preserve current tracking once Hunting/Searching according to the scope;
- accept or reject sound through the threshold policy;
- prevent sound from replacing a confirmed visual target;
- track separate last-seen and last-heard evidence;
- send the distinct, restrained investigation cue only on a meaningful transition, with rate limiting;
- ensure a sound-triggered transition calls the same feedback/state-change handling as other transitions;
- reset every new field;
- keep route adaptation compatible but subordinate to the perception laws.

Verify controlled cases: partial peek, full cover, quiet relocation, unseen throw, seen throw, and below-threshold sound.

### V030-005: Portable Distraction Completion

Priority: High

Likely files:

- `Assets/_Project/Scripts/Threats/ThrowableThreatStimulus.cs`
- feedback cue/profile code
- `Assets/_Project/Scripts/Editor/FacilityGeneration/PrototypeSceneBuilder.cs`

Actions:

- emit one accepted impact signal and one matching spatial metal sound;
- prevent bounce/cooldown duplicates;
- retain two readable prop silhouettes and distribute nine props across route families when the cut line permits;
- ensure restart and regenerated-scene behavior remain clean.

The accepted impact event, sound, and Owl telemetry must refer to the same stimulus.

### V030-006: Fixed Machinery Counterplay

Priority: High

Likely files:

- one new narrow `IPlayerInteractable` MonoBehaviour under Interaction or Threats
- feedback/audio support if required
- `Assets/_Project/Scripts/Editor/FacilityGeneration/PrototypeSceneBuilder.cs` or the appropriate facility builder
- focused Edit Mode and Play Mode tests

Build three matching `MAINTENANCE TEST` panels, one per route family:

- prompt `TEST MACHINERY`;
- one localized `0.75` intensity, `18 m` sound-origin stimulus per activation;
- about `2.5` seconds of localized machine audio and amber active state;
- `25` second cooldown;
- run reset restores ready state and stops feedback;
- placement supports sight break and alternate relocation without obstructing critical paths.

Validate the actual panel from the player's standing and crouched cameras. If three safe placements miss the cut line, implement one complete Central panel and report the other two as the first v0.3 increment; do not clone an untested component.

### V030-007: Operator Explanation

Priority: Urgent

Likely files:

- `Assets/_Project/Scripts/Threats/PursuitThreatController.cs`
- the established F1 status-provider path

Expose the exact fields required by the scope. Keep normal HUD unchanged. Confirm telemetry explains:

- partial visual evidence that did not confirm;
- visual confirmation;
- rejected sound;
- accepted unseen distraction;
- distraction ignored for targeting during current sight;
- last-seen search after full cover;
- adaptation disabled and enabled.

### V030-008: Generated Content, Reset, And Runtime Inspection

Priority: Urgent

Actions:

- update builders/specifications first;
- leave Play Mode, regenerate only the disposable generated root through Unity MCP, and save through MCP;
- preserve manual content outside that root;
- inspect portal clearance, NavMesh, rod sockets, prop reachability, panel reachability, and Owl navigation;
- test same-seed reset, new-seed reset, failed run, and extracted run;
- inspect standing, crouched, leaned, walking, sprinting, full-cover, and distraction scenarios from the player camera;
- capture evidence screenshots where readability or placement changed.

### V030-009: Regression, Version, Build, And Evidence

Priority: Urgent

Actions:

- run focused new Edit Mode tests;
- run all Edit Mode and Play Mode tests;
- run primary comparison cases with route adaptation disabled;
- run at least one integration sequence with route adaptation enabled;
- inspect the console after compilation, tests, Play Mode, and build;
- set version `v0.3.0.0` through Unity MCP;
- produce a fresh Windows desktop build, launch it, and perform a meaningful smoke test;
- update `06_prototype_log.md` with implemented behavior, automated evidence, build evidence, human evidence still required, and exact cuts;
- do not commit or push unless separately requested.

## Two-Day Execution Plan

### Day 1: Rules Before Content

- V030-001 through V030-004.
- Compile and test after each coherent slice.
- End the day with visual bands, sound threshold, target priority, and movement feedback working in the current scene.

### Day 2: Content And Delivery Gate

- V030-005 through V030-009.
- Finish one complete fixed panel before multiplying it.
- Regenerate, inspect, test, version, build, smoke-test, and document.
- Preserve time for at least one short human session if a tester is available; do not trade the regression/build gate for presentation polish.

## Required Test Cases

At minimum, add focused coverage for:

- `0/1/2/3` visible samples;
- confirmation gain by exposure, crouch/slow movement, sprint multiplier, decay, and reset;
- below/equal/above hearing threshold after distance falloff;
- sight-over-sound priority;
- accepted unseen sound selection;
- no target replacement from rejected sound;
- stationary/crouch/walk/sprint movement bands and pulse intervals;
- one impact per armed throw and cooldown;
- panel ready, activate, active, cooldown, unavailable, ready, and run reset;
- F3-disabled route memory and enabled integration;
- scene wiring for all panels and portable props;
- full-cover integration and current critical-path regression.

## Out-Of-Scope Confirmation

Do not add flashlight stealth, explicit hide spots, capture recovery, stun, inventory, doors, ventilation, radiation controls, movable cover, carts, biofeedback, breathing, heartbeat, visual stealth UI, new mission/map/entity, learned AI, or generalized gameplay infrastructure.

## Required Final Report

```text
Approved task and authority:
Version delivered:
Tasks completed:
Behavior implemented:
Prototype values actually used:
Files changed:
Unity scenes/assets/settings changed through MCP:
Focused Edit Mode tests and exact result:
Full Edit Mode result:
Play Mode result:
Console result:
Route adaptation OFF comparison result:
Route adaptation ON integration result:
Build and smoke-test result:
Runtime viewpoints inspected:
Human evidence collected or Not Tested:
Cuts taken from the two-day cut order:
Known risks or regressions:
Docs updated:
Unrelated dirty files preserved:
Commit/push status:
Recommended first v0.3.z.w refinement:
```

Never report desktop evidence as XR comfort or physical-hardware validation.
