# Adaptive Entity Framework

Status: Core adaptation rules confirmed; per-entity behaviors and protected player agency remain in design

Last updated: 2026-07-22

## Core Promise

Every contract has an intelligent entity as its primary threat. The player learns the entity while the entity appears to learn the player.

Environmental hazards, disasters, time pressure, and facility failures may create secondary danger, but they should not replace the adversarial relationship at the center of a run.

Physical haptics are the other core differentiator. Entity adaptation and haptic communication should reinforce each other without allowing hidden hardware data to create unfair outcomes.

## Design Goal

The entity should feel intelligent because it:

- notices meaningful player habits,
- remembers selected evidence for a bounded period,
- changes priorities or tactics in response,
- communicates changed behavior through observable actions,
- remains fallible and gives the player room to counter-adapt.

The goal is perceived intelligence and mutual adaptation, not technical complexity for its own sake. Machine learning is not required.

Prototype 1 uses deterministic adaptive game AI. A pre-trained or learned strategy model remains future R&D governed by `22_ai_technology_strategy.md`; it is not part of the current implementation authority.

## Proposed Behavior Pipeline

```text
Perceive an event
    -> classify player behavior
    -> update bounded memory
    -> bias a strategy
    -> choose an entity-specific action
    -> reveal the changed behavior to the player
    -> decay or replace old assumptions
```

Shared architecture may classify behavior and hold memory. Each entity should interpret those signals differently so the roster does not feel like one AI wearing different models.

## Confirmed Memory And Learning Rules

- Memory lasts for one run only and resets fully when that run ends or restarts.
- One observed use creates suspicion.
- Two repeated uses permit a meaningful adaptation.
- Directly witnessed behavior carries more importance than inferred environmental evidence.
- Inferred evidence may increase suspicion, but should not produce omniscient certainty.
- Tactical variety spreads or weakens entity confidence, making the player harder to predict.
- Repeating one tactic strengthens the entity's confidence and response to that tactic.
- Capture escalation does not automatically strengthen adaptation. The second-capture penalty is designed separately from the entity's evidence-based learning state.
- An invisible pacing director may create believable back-off windows to protect the 15-20 minute intensity curve.

## Candidate Player Signals

The first three approved signal families are route repetition, hiding repetition, and distraction/stun reliance. Only route repetition is in the first Prototype 1 validation slice.

| Signal | Example Observation | Possible Adaptation | Fairness Requirement |
|---|---|---|---|
| Route repetition | Player repeatedly uses the west wing. | Bias patrol or ambush preparation toward that route. | Leave another route viable and telegraph increased attention. |
| Hiding repetition | Player returns to the same hiding category or location. | Inspect it more often or search nearby longer. | Do not magically know the player is inside without evidence. |
| Distraction reliance | Player repeatedly throws or activates distractions. | Delay commitment, check origin, or approach indirectly. | Distractions must still create some useful opening. |
| Sprint/noise reliance | Player frequently moves loudly. | Investigate faster or predict likely destination. | Quiet movement must remain a meaningful alternative. |
| Stun reliance | Player repeatedly uses a defensive tool. | Recover faster, bait use, or approach outside its ideal range. | Never make the tool silently stop functioning. |
| Objective order | Player follows the same terminal sequence. | Pressure the likely next area. | Prediction should be probabilistic and readable, not teleportation. |
| Long stationary play | Player waits repeatedly for safety. | Widen patrol or create indirect pressure. | Do not punish necessary accessibility pauses. |

Signals outside the first three remain candidates and do not authorize implementation.

Future stealth note:

- Sprinting and careless object handling are confirmed noise sources.
- Heart rate may later influence stealth flavor or pressure, but it is not an approved adaptation or detection input until device quality, calibration, consent, fallback, and fairness are tested.
- An entity may reduce a repeated tactic's reliability, but may not silently make carried equipment or defensive tools cease functioning.

## Evidence Hierarchy

Direct evidence includes behavior the entity plausibly sees or senses itself. Inferred evidence includes opened routes, displaced objects, noises, objective changes, or traces discovered later.

Confirmed ordering:

```text
Direct observation
    > corroborated inferred evidence
    > isolated ambiguous evidence
```

The exact numeric weights are tunable implementation values. A strong adaptation should normally require direct observation or multiple corroborating clues.

## Communication

There is no player-facing "the entity learned you" notification.

Players should notice adaptation through:

- changed search animations,
- changed patrol or interception routes,
- changed responses to a tactic that worked previously,
- entity-specific vocalization, posture, timing, or environmental traces.

Operator-only debug displays and telemetry remain allowed.

## Bluffing

Entities may test predictions, inspect an unexpected route, delay responding to a distraction, or appear to disengage before checking again.

A bluff must not:

- use the player's hidden exact location,
- remove every safe response,
- become indistinguishable from broken perception or random state changes.

## Fairness Laws

Every entity adaptation should obey these rules:

1. Observation before response: the entity cannot adapt to behavior it has not plausibly observed.
2. Pressure, not deletion: adaptation makes a tactic riskier; it does not permanently remove it.
3. Counter-adaptation: the player can change behavior and regain an advantage.
4. Bounded memory: old evidence decays or loses priority.
5. Legible change: animation, route choice, sound, posture, timing, or haptics reveal the new strategy.
6. No omniscience: adaptation does not grant exact location without valid perception.
7. No accessibility punishment: standing still, high heart rate, or slower physical movement cannot independently justify unavoidable aggression.
8. Intensity cap: the entity cannot stack adaptations until every route and tactic is invalid.

The exact player behavior that must remain permanently protected from adaptation is intentionally deferred until the game's core verbs are more stable. The laws above remain binding in the meantime.

## Entity Identity

Each entity needs an adaptation profile:

```text
Entity name:
Core fantasy:
Primary senses:
What it notices quickly:
What it learns slowly:
What it never understands:
Preferred adaptations:
Warning language:
Counterplay families:
Capture fiction:
Haptic meaning:
How memory decays:
What makes it feel different from the Owl:
```

## Owl Starting Hypothesis

Confirmed traits:

- sharp visual attention,
- route and wing awareness,
- interest in repeated player tactics,
- playful or predatory interaction with dropped objective items,
- increasing adaptation after a second capture.

Approved first adaptation experiment:

- Track repeated route-family use during the current run.
- After enough evidence, bias the Owl toward that route without camping it permanently.
- Reveal the change through an observable patrol shift or warning cue.
- Allow a different route, distraction, or sight break to counter the new pressure.

Implementation authority is limited to the route-family experiment in `20_owl_adaptation_v1_spec.md` and its current Sprint 02 task.

Approved sequencing:

1. Validate route-family learning in Prototype 1.
2. Add hiding-pattern learning only after route adaptation is perceived and understood.
3. Add distraction/stun reliance after both the counterplay and adaptation remain useful.
4. Add bluffing as an authored entity behavior after baseline perception is trustworthy.

## Playtest Questions

- Did the player believe the entity noticed something they did?
- Could the player identify what behavior caused the change?
- Did the response feel intelligent rather than random or unfair?
- Did the player change strategy afterward?
- Did that new strategy create a useful opening?
- Could an observer reconstruct the adaptation from telemetry?

## Open Decisions

1. Exact confidence values and decay rates.
2. Exact direct-versus-inferred evidence weights.
3. Which search animation and route changes make Owl suspicion readable.
4. Which parts of the learning system are shared across the roster.
5. Exact second-capture amplification of already learned behavior.
6. Which player behavior or critical affordance the entity must never counter.
