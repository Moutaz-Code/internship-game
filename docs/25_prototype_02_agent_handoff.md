# Prototype 2 Agent Handoff

Status: Active implementation handoff

Last updated: 2026-07-22

## Read Order

Before changing code, read:

1. `24_prototype_02_scope.md`
2. This handoff
3. `20_owl_adaptation_v1_spec.md` for design background
4. `18_agent_implementation_handoff.md` for project-wide authority rules
5. `10_technical_architecture_outline.md` for boundaries
6. Current threat code and tests

`24_prototype_02_scope.md` overrides broader unimplemented Sprint 02 ideas for PT2.

## One-Sentence Assignment

Turn the existing repeated-wing patrol bias into a run-only, evidence-based Suspicious/Adapted/Counter-Adapted system that players can perceive and counter, operators can explain, and tests can reproduce.

## Current Baseline

The code already provides:

- `ThreatPressureModel.RecordWingVisit`,
- adaptation after two repeated wing visits,
- `BiasedPatrolWeight = 1.75`,
- `PatrolBiasDurationSeconds = 60`,
- route-local `ThreatStimulus` events,
- `PursuitThreatController.OnZoneEntered`,
- authored West/Central/East mapping,
- F1 status-provider overlay,
- deterministic model tests.

Current gaps:

- all zone entry is treated as entity knowledge,
- no direct-versus-inferred evidence,
- no explicit suspicion state,
- no per-wing confidence telemetry,
- route variation does not deliberately weaken the old prediction,
- the player-facing behavior shift is not validated,
- no adaptation comparison toggle.

## Implementation Constraints

- Extend existing threat systems; do not replace navigation or the state machine.
- Keep the pure model testable without MonoBehaviour or scene dependencies.
- Keep memory run-only and clear it through existing reset paths.
- Preserve deterministic behavior for identical evidence streams.
- Use configurable prototype values rather than scattering constants.
- No player-facing adaptation UI.
- No ML package, trained model, hardware integration, or new gameplay family.
- Do not implement capture recovery under PT2.
- Preserve unrelated user/agent changes.

## Task Order

### PT2-001: Baseline Audit

Priority: Urgent

Actions:

- Run the focused existing `ThreatPressureModelTests` and relevant threat tests.
- Confirm current repeated-wing behavior and reset path.
- Record any failing baseline before editing.

Done when:

- baseline result is reported,
- current behavior is understood,
- no unrelated failure is misattributed to PT2.

### PT2-002: Route Evidence Model

Priority: Urgent

Primary file:

- `ThreatPressureModel.cs`

Actions:

- Add per-wing confidence.
- Add Direct and Inferred evidence sources.
- Add Uncertain, Suspicious, Adapted, and Counter-Adapted state reporting.
- Use the default weights and thresholds from the scope.
- Add inferred-evidence rate limiting.
- Add confidence decay and cross-wing confidence reduction.
- Preserve the existing maximum patrol weight and initial bias-duration comparison value.
- Reset every new field.

Focused tests:

- direct evidence reaches Suspicious after one event,
- direct evidence reaches Adapted after two repeated events,
- one inferred event remains below Suspicious,
- two inferred events can reach Suspicious,
- inferred spam inside the cooldown is ignored,
- different-wing evidence weakens old confidence,
- time decay changes state predictably,
- reset clears all memory,
- equal streams remain deterministic,
- disabled mode returns baseline weights.

### PT2-003: Evidence Wiring

Priority: Urgent

Primary files:

- `PursuitThreatController.cs`
- threat contracts only if a narrow typed event/API is needed

Actions:

- Replace unconditional route knowledge from `OnZoneEntered`.
- Record Direct evidence only when perception plausibly supports it.
- Convert sufficiently strong route-local stimuli into lower-weight Inferred evidence.
- Keep exact hidden player location unavailable to inferred adaptation.
- Avoid double-counting one event through multiple paths.

Done when:

- unperceived silent zone entry adds no evidence,
- witnessed route use adds Direct evidence,
- a valid strong route-local stimulus may add Inferred evidence,
- evidence source and kind are preserved for telemetry.

### PT2-004: Behavioral Expression And Counter-Adaptation

Priority: High

Primary files:

- `PursuitThreatController.cs`
- existing presentation/feedback code only where necessary

Actions:

- Give Suspicious a mild route-inspection preference below full bias.
- Give Adapted the capped existing patrol bias.
- Prevent indefinite same-wing patrol selection.
- Make credible route variation reduce old attention.
- Express state changes through patrol/search/orientation and existing restrained cues.
- Do not add final art or a player notification.

Done when:

- Suspicious and Adapted differ behaviorally,
- changed route creates an opening,
- adaptation cannot camp one wing indefinitely,
- baseline threat states and navigation remain valid.

### PT2-005: Operator Telemetry And Toggle

Priority: High

Primary files:

- `PursuitThreatController.cs`
- `PrototypeHudPresenter.cs` or the smallest established input/status surface

Actions:

- Extend `StatusLabel` or an established status provider with state and confidence.
- Show last evidence source/kind, dominant wing, and timing in F1 operator mode.
- Add the recommended F3 adaptation toggle.
- Disable mode clears route memory and restores baseline weights.
- Keep the mission HUD unchanged.

Done when:

- an observer can reconstruct why adaptation occurred,
- F3 comparison is reliable,
- no adaptation information leaks into normal player UI.

### PT2-006: Regression And Build Gate

Priority: Urgent

Actions:

- Run focused Edit Mode tests.
- Run the existing relevant threat, navigation, run-reset, objective, and integrated Play Mode tests.
- Test one same-seed repeated evidence sequence.
- Test adaptation disabled.
- Produce and smoke-test a desktop build if the local environment supports it.

Done when:

- results and counts are reported accurately,
- no critical-path regression is hidden,
- any environment limitation is stated explicitly.

### PT2-007: Human Evidence

Priority: High

Actions:

- Run at least two observed sessions, target three.
- Do not tell testers the exact adaptation rule beforehand.
- Ask what changed, why they think it changed, whether they changed route, and whether it felt fair.
- Compare with adaptation disabled when practical.
- Update `06_prototype_log.md` with evidence.

Done when:

- player observations are separated from developer interpretation,
- each acceptance criterion has Pass, Fail, or Not Tested,
- follow-ups are prioritized rather than silently added.

## Do Not Expand

Do not use spare time to add:

- hiding or tool-reliance learning,
- bluffing,
- capture recovery,
- electrical probe,
- inventory,
- flashlight stealth,
- graded stealth states,
- heart rate or simulator,
- cart,
- ventilation/radiation control,
- movable shielding,
- learned AI.

Finish tests, telemetry, and human evidence first.

## Required Agent Report

```text
Prototype: PT2 Readable Route Adaptation
Tasks completed:
Files changed:
Behavior implemented:
Prototype values used:
Tests run and exact results:
Build/smoke-test result:
Human evidence collected:
Acceptance criteria failed or not tested:
Known risks/regressions:
Docs updated:
Recommended next action:
```

Do not claim that PT2 proves VR comfort, physical haptics, long-term AI sophistication, or the full 15-20 minute product loop.
