# Entity AI Technology Strategy

Status: Deterministic MLP boundary confirmed; trained-model path is future R&D

Last updated: 2026-07-22

## Outcome

The August MLP uses deterministic, testable game AI to create the experience of an entity that observes, remembers, predicts, adapts, bluffs, and backs off.

A pre-trained or learned model may be explored later, but it is not required to achieve the desired player experience and is not authorized for the current implementation scope.

## Important Distinction

The design target is an Alien-like relationship in which player and entity learn each other. That describes player experience, not a mandatory machine-learning technique.

Current adaptive game AI may use:

- authored perception rules,
- bounded within-run memory,
- confidence scores,
- state machines or utility selection,
- strategy biases,
- deterministic navigation,
- pacing-director constraints,
- seeded variation and telemetry.

This remains real game AI even without a neural model.

## Current MLP Architecture

```text
Valid perception and world events
    -> bounded run-only memory
    -> suspicion/adaptation rules
    -> deterministic strategy selection
    -> pacing and fairness validation
    -> entity-specific action
    -> validated navigation and presentation
```

The current route-family experiment in `20_owl_adaptation_v1_spec.md` is the baseline that any future model must outperform in perceived intelligence without reducing fairness, reliability, or explainability.

## Future Hybrid Model Boundary

If a learned model is explored, constrain it to a narrow strategy-policy role.

Candidate bounded outputs:

- route-attention bias,
- search-style recommendation,
- confidence adjustment,
- prediction of likely tactic category,
- proposal to bluff or inspect.

Deterministic code must validate, cap, reject, or replace every proposal.

The learned model must not directly control:

- locomotion or physics,
- collision or navigation safety,
- exact player detection,
- capture, damage, success, or failure,
- forced camera behavior,
- haptic intensity or physical-device safety,
- raw heart-rate interpretation,
- operator reset or session lifecycle.

## Required Fallback

The game must remain fully playable when learned inference is:

- disabled,
- unavailable,
- too slow,
- producing invalid output,
- incompatible with target hardware,
- rejected by the strategy validator.

Fallback is the deterministic adaptive policy, not a nonfunctional or passive entity.

## R&D Entry Gates

Do not start a trained-model implementation until all of these exist:

1. A narrow player-facing hypothesis that the deterministic baseline cannot test adequately.
2. A defined input and bounded output contract.
3. A lawful, documented dataset or simulation-generation plan with provenance.
4. Offline evaluation metrics for usefulness, invalid outputs, fairness, and behavior diversity.
5. Target headset/PC performance and latency budgets.
6. A deterministic validator, timeout, and complete fallback policy.
7. Telemetry that explains the evidence, model proposal, validation result, and final action.
8. Adversarial tests for impossible, unsafe, repetitive, or omniscient behavior.
9. An A/B playtest showing improved perceived intelligence over the deterministic baseline.

## Deployment Considerations

Venue reliability favors local inference if a model is eventually used. Remote inference adds network dependency, privacy questions, latency variance, and failure modes that conflict with unattended arcade operation.

The final choice depends on the actual venue PC, headset/runtime, model size, SDK constraints, and Pixoul policy. No deployment path is selected yet.

## Hardware And Biofeedback Boundary

Raw sensor values never enter a learned strategy model directly.

Future heart-rate flow remains:

```text
Sensor
    -> quality check and calibration
    -> bounded avatar breathing/noise behavior
    -> ordinary world stimulus
    -> entity perception
```

This preserves an observable causal chain and a non-sensor gameplay equivalent.

## Current Package Rule

Do not reinstall Unity AI Inference or another ML runtime for the current sprint. D-014 remains active. A future package decision requires a concrete approved R&D task that passes the entry gates above.

## Open Research Questions

1. What narrow strategy decision would benefit from learning rather than authored utility logic?
2. What training data could exist ethically and practically for that decision?
3. Can inference run locally within the unknown venue hardware budget?
4. How will a tester understand why model-driven behavior occurred?
5. What measurable improvement would justify the added failure surface?
