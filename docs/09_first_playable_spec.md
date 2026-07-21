# Prototype 1 Feature Spec

Status: Implemented feature skeleton; design validation and recovery-loop revision pending

Last updated: 2026-07-22

## Prototype Question

> Can a first-time player act as a capable contractor, identify and transport three live fuel rods through an authored facility, survive a smart creature using situational counterplay, recover from a capture, and extract within 15-20 minutes without developer guidance?

Desktop testing answers loop, pacing, clarity, fairness, and reliability questions. It does not yet answer VR comfort or real-hardware questions.

Scope identity:

- This document specifies the Nuclear Recovery proof contract only.
- Fuel rods, the nuclear facility, and the Owl Thing are one scenario within a wider multi-contract game.
- Future missions and entities in `17_contract_scenario_framework.md` are direction examples, not Prototype 1 implementation scope.

## Player Fantasy

The player is a contractor doing a dangerous paid recovery job.

They should feel competent enough to understand and execute the contract, but never powerful enough to control the facility or reduce the creature to one solved tactic. Completing the job and surviving are the primary goals. Score can support comparison and replay later, but it is not the main objective.

## Design Commitments

- The core tension is surviving while both identifying and transporting fuel rods.
- The creature is intelligent and has no universal counterplay.
- The player can use a mix of hiding, breaking line of sight, distraction, limited stunning, and route changes as the situation allows.
- The first capture should normally cause a serious recoverable setback, not instant run loss.
- Rod mistakes should create immediate, understandable gameplay consequences.
- Pre-run loadout choice is deferred until the baseline loop is proven.
- Run variation comes from authored placements and states, not runtime-generated geometry.

## Current Run Shape

Target duration:

- First-time player: 15-20 minutes.
- Experienced or debug player: approximately 9-12 minutes.

Flow:

1. Receive the recovery contract and enter through a safe start zone.
2. Read the three-slot extraction rack and understand that three live rods are required.
3. Plan a route through the authored facility.
4. Use Geiger guidance to discover candidate rods.
5. Hold a candidate for a three-second assay; Heat uniquely confirms a live rod.
6. Carry one confirmed-live rod at a time back to the rack while managing the Owl.
7. Receive immediate rejection and pressure when attempting to deposit an invalid rod.
8. Recover from close calls or an initial capture with lost position, time, or objective control.
9. Deposit three live rods, survive the 20-second extraction cycle, and cross the exit.
10. Restart immediately with the same or a new seed.

## Threat Contract

The Owl should feel observant and adaptable, not omniscient.

Required behavior qualities:

- Warn before peak danger.
- Use line of sight, stimuli, objective pressure, and authored route knowledge.
- Search after losing the player, then disengage under readable rules.
- Bias future pressure based on repeated route use without hard-countering every player choice.
- Leave at least one viable recovery option after a mistake.
- Never spawn into an unavoidable capture or cross invalid geometry.

No-single-counter rule:

- Hiding fails if repeated predictably.
- Breaking line of sight creates space but does not erase investigation immediately.
- Distraction moves attention but can increase local pressure.
- Stunning, if retained, buys time rather than defeating the creature.
- Rerouting trades time and uncertainty for safety.

Prototype 1 does not need every counterplay at final fidelity. It does need at least two understandable options so a player can make a situational choice.

Prototype 1 adaptation scope:

- Memory lasts only for the current run and resets on restart.
- Route-family use is the only adaptation signal required for this validation slice.
- One use creates suspicion; two repeated uses may bias patrol or search behavior.
- Direct Owl observation carries more importance than inferred route evidence.
- Switching tactics or route families weakens confidence in the previous prediction.
- Communicate adaptation through patrol/search behavior rather than player-facing UI.
- The pacing director may create believable back-off windows.
- Hiding-pattern, distraction/stun-reliance, and full bluffing systems remain outside this sprint unless separately approved.

## Capture And Recovery

Confirmed Prototype 1 test rules:

- The Owl knocks the player unconscious. Feeding or predation may be implied through restrained audio and environment storytelling but is never directly or graphically shown.
- Use a short VR-safe fade/audio transition; never animate or force the player's camera through a dragging sequence.
- The first and second captures are survivable; the third capture is the current terminal-failure test rule.
- Completed rod deposits remain permanently safe for the run.
- Carried objective items are dropped or scattered into valid authored world positions.
- The player returns at one of roughly five authored recovery zones selected by safety, distance, and route rules rather than unconstrained randomness.
- Outside the final five minutes, the Owl retreats for approximately 60 seconds.
- During the final five minutes, post-recovery protection is shortened to approximately 20-30 seconds rather than removed.
- First recovery uses a short equipment-reboot interaction, currently targeting 10-15 seconds.
- Second recovery uses a short Owl-nest escape, currently targeting 20-30 seconds, plus a heavier recoverable penalty that is still pending design.
- Third capture ends the current test run.
- The player immediately understands what was lost and how to resume the contract.
- Recovery must not consume so much time that the run becomes secretly unwinnable.

Open implementation choices; do not silently decide these in code:

- Exact equipment-reboot and nest-escape actions.
- Exact non-adaptation penalty applied after the second capture.
- Exact authored scatter range. Items should appear playfully displaced by the entity, but must remain recoverable and must not imply a second creature implementation.

Reject this version if players deliberately exploit capture as fast travel, enter a recapture loop, or lose realistic hope of finishing.

## Close Calls

Use readable severity tiers:

| Tier | Consequence |
|---|---|
| Near miss | Sensory scare through animation, audio, lighting, and bounded haptics; no mechanical punishment required. |
| Physical contact | Drop a carried item or apply a short equipment disruption, plus clear sensory feedback. |
| Full capture | Unconscious transition, authored relocation, carried-item scatter, recovery interaction, and capture escalation. |

Do not fire every possible consequence on every contact. The player should be able to understand the severity, cause, effect, and recovery path.

## Immediate Rod-Mistake Feedback

The exact penalty remains a tuning decision. The first test should use:

- clear physical and audio rejection,
- a bounded noise or Owl-pressure event,
- no silent score-only punishment,
- no instant run loss.

The assay remains authoritative: only Heat confirms a live rod. Owl interference may affect discovery guidance but must never change rod truth.

## Required Features

- Unity desktop baseline with a clean start, pause, end, and restart flow.
- Authored multi-route facility with a no-jump critical path.
- Three live rods, inert candidates, deterministic assay, one-at-a-time carry, and a three-slot rack.
- One Owl with patrol, investigate, hunt, search, disengage, and stimulus response.
- At least three visible seeded run variables.
- 20-minute run timer and 20-second extraction cycle.
- Recoverable capture path.
- Two-capture escalation and authored recovery-zone selection.
- Immediate invalid-rod consequence.
- Player-facing mission feedback separated from operator/debug information.
- Simulated Heat, proximity vibration, and Wind cues with unique meanings.

## Acceptance Criteria

Prototype 1 is design-ready for the next stage when human testing shows:

- A first-time tester can state the contract within two minutes.
- A first-time run trends toward 15-20 minutes without forced waiting.
- The tester can explain live-versus-inert rod feedback correctly.
- The tester uses or identifies more than one viable response to the Owl.
- At least one capture is recoverable without developer intervention or a failure spiral.
- Immediate mistake feedback is understood and feels consequential but fair.
- At least three run variables are noticeable across suitable seeds.
- The tester reports at least one tense close call and a reason to replay.
- The build restarts cleanly and remains stable.

## Evidence Still Missing

- First-time objective comprehension.
- Human pacing and route-choice behavior.
- Threat fairness and perceived intelligence.
- Capture recovery tuning.
- Replay desire.
- Headset locomotion and interaction comfort.
- Physical haptic meaning, intensity, latency, and safety.

## Cut Lines

Cut or defer before weakening the core loop:

1. Pre-run loadouts and prep economy.
2. Score-driven progression.
3. Heart-rate-driven detection or win/loss.
4. Additional objective types.
5. Additional enemies.
6. Runtime procedural geometry.
7. Final art and nonessential lore.

Protect even when simplifying:

1. Clear contractor objective.
2. Reliable rod truth.
3. Situational Owl counterplay.
4. Recoverable close calls and captures.
5. Explicit extraction and restart.
6. Deterministic debug and fallback modes.
