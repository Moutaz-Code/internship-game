# Owl Adaptation V1 Spec

Status: Design basis; current implementation authority is `24_prototype_02_scope.md`

Last updated: 2026-07-22

## Prototype Question

> Does the Owl appear to notice repeated route-family use, change its behavior after enough evidence, and encourage the player to vary routes without making either route unusable?

Implementation note:

- Prototype 2 freezes this experiment into a concrete implementation and evidence gate.
- Follow `24_prototype_02_scope.md` and `25_prototype_02_agent_handoff.md` when details differ or are more specific.

## Authorized Scope

This experiment validates route-family learning only.

Do not add hiding-pattern learning, distraction/stun-reliance learning, persistent memory, generalized roster infrastructure, full bluffing, or heart-rate adaptation under this task.

Implement this experiment with the current deterministic game-AI architecture. Do not add a trained model, ML runtime, or inference package.

## Memory Rules

- Memory exists only for the current run.
- Restarting the same seed or starting a new seed clears all learned behavior.
- First credible use creates suspicion.
- Second repeated credible use permits adaptation.
- Directly witnessed route behavior contributes more evidence than inference.
- Inferred evidence may support suspicion but should require corroboration before strong adaptation.
- Using different route families weakens confidence in one dominant prediction.

Exact evidence weights and decay rates must be configurable and visible in operator telemetry. They are tuning values, not final design constants.

## Evidence

Direct route evidence may include:

- the Owl seeing the player enter, traverse, or leave a route family,
- the Owl maintaining valid pursuit knowledge as the player commits to that route.

Inferred route evidence may include:

- route-specific noise or stimulus,
- objective activity associated with that wing,
- a recently moved or dropped object discovered on the route.

Inference must never reveal the player's exact hidden position.

## Expression States

### Uncertain

- No route family has enough evidence.
- Owl uses its baseline authored profile.

### Suspicious

- One route family has one credible use or corroborated clues.
- Owl may inspect that route, orient toward it, or extend a nearby search.
- The response is subtle and does not block the route.

### Adapted

- The same route family receives a second credible use before confidence decays.
- Owl biases patrol, interception preparation, or search allocation toward that route.
- Bias is capped; at least one viable response or alternate route remains.

### Counter-Adapted

- The player varies routes or tactics.
- Confidence in the old prediction weakens.
- Owl redistributes attention instead of permanently camping the old route.

## Player Communication

No player-facing adaptation UI is allowed.

Use one or more of:

- changed patrol path,
- visible orientation or route inspection,
- different search animation,
- changed timing when checking the previously favored route,
- entity-specific vocalization or environmental trace.

Operator-only telemetry should show the underlying state during testing.

## Pacing Director

The director may delay or suppress the expression of an adaptation when current intensity is already too high. It should not erase evidence or fabricate perception.

Back-off must be expressed through believable Owl behavior such as disengaging, searching elsewhere, or returning to patrol.

## Required Telemetry

- run and seed,
- route family,
- evidence source: direct or inferred,
- evidence timestamp,
- current confidence per route family,
- transition into Suspicious, Adapted, or Counter-Adapted,
- selected patrol/search response,
- whether the pacing director delayed expression,
- reset confirmation at run restart.

## Functional Acceptance Criteria

- All adaptation memory resets on every run restart.
- One credible route use creates suspicion but not hard interception.
- A second repeated credible use can produce a capped route bias.
- Direct evidence has greater influence than a single inferred clue.
- Route variation reduces the old dominant confidence.
- The Owl never receives exact hidden player location from inference.
- Adaptation never makes all critical routes invalid.
- Same-seed runs remain deterministic when player evidence events are reproduced.
- The system can be disabled for baseline comparison.

## Human Playtest Acceptance Criteria

- A player notices that the Owl changed route/search behavior without UI explanation.
- The player can connect the change to their repeated route choice after the run.
- The player responds by varying their route or another tactic.
- The change feels intelligent rather than random, omniscient, or scripted.
- Changing behavior produces a useful opening.

## Cut Rule

If players cannot perceive route adaptation, improve behavioral expression before adding more tracked signals. If they perceive it as unfair, reduce evidence strength, route bias, or expression timing before expanding complexity.
