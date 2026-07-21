# Player Tactics And Stealth Framework

Status: Confirmed product direction; detailed stealth and VR controls remain in design

Last updated: 2026-07-21

## Core Fantasy

The player is a prepared contractor, not a soldier or specialist monster hunter.

They expect danger and may carry practical equipment, but they do not enter with a perfect gadget designed to counter the exact entity. Survival comes from improvisation, site knowledge, varied tactics, and completing the job under pressure.

## Internship Combat Boundary

- Entities cannot be killed within the internship scope.
- The player may escape, hide, break line of sight, distract, delay, lock out, manipulate the environment, or temporarily stun.
- Meaningful maiming is a distant post-internship possibility, not an MLP requirement or current architecture target.
- Defensive actions create openings; they do not convert the entity into a health-bar encounter.

## Counterplay Families

| Family | Player Actions | Primary Cost | Entity Learning Opportunity |
|---|---|---|---|
| Evasion | Break sight, change route, use darkness or cover. | Time, distance, route uncertainty. | Predict repeated routes or hiding choices. |
| Spatial hiding | Remain outside sight and manage noise using the environment. | Reduced movement and objective progress. | Search repeated hiding categories or areas. |
| Emergency hiding | Use a small number of explicit safe-looking spots. | Predictability and possible entrapment. | Inspect repeatedly used spots more often. |
| Improvised distraction | Throw debris, activate machinery, create remote environmental noise. | Reveals intent, changes local pressure, may consume an object or opportunity. | Delay commitment or inspect likely throw origin. |
| Door/route control | Close, lock, power, or reroute through doors and site systems. | Setup time and temporary access restrictions. | Approach from another route, wait, or pressure the mechanism. |
| Temporary defense | Use an arm's-length stun to create a short escape window. | Close-range risk and approximately 20-second cooldown. | Recover or approach differently after repeated use. |
| Mission equipment | Use a contract-specific tool to perform or understand the job. | Attention, hand occupation, noise, or task time. | Pressure predictable task moments without invalidating the tool. |

## Spatial Hiding

Baseline hiding should emerge from:

- line of sight,
- occluding geometry,
- distance,
- movement and interaction noise,
- light and equipment use once defined,
- entity-specific senses.

A few explicit emergency hiding spots may provide readable recovery options. They should not become universal immunity buttons or the only viable stealth method.

Entities may learn and inspect repeated emergency spots after directly observing their use or gathering strong corroborated evidence. Repeated inspection still requires warning and an escape opportunity; the entity cannot silently know the player is inside.

## Shared Stealth Foundation

Use a graded foundation that entities interpret according to their senses:

| State | Meaning |
|---|---|
| Concealed | Current light, distance, movement, cover, and noise provide strong protection. |
| Noticeable | The entity has suspicious evidence and may orient, investigate, or search. |
| Exposed | The entity has enough valid perception to pursue or apply encounter pressure. |

The states are design language, not a requirement for a visible player-facing meter.

Entity-specific perception may weight vision, sound, environmental traces, or other senses differently while preserving the shared evidence and fairness rules.

## Crouching

- Physical crouching is the primary embodied interaction.
- Controller-assisted crouching is an opt-out alternative with equivalent gameplay effect.
- Support seated or limited-mobility calibration where the venue requires it.
- Do not require repeated deep crouching or long static crouches to remain viable.
- Player capsule, hand reach, camera height, and hiding geometry must remain consistent across both input methods.

## Flashlight Tradeoff

The flashlight improves navigation and objective readability but can make the player more noticeable to visually oriented entities.

Rules to preserve fairness:

- the player should understand that emitted light can reveal them,
- darkness must remain navigable enough that turning the light off is a real option,
- entities without relevant visual perception may respond differently,
- the flashlight must not become either always-on with no cost or unusably dangerous.

## Distraction Philosophy

Distractions primarily come from the site:

- loose debris and throwable objects,
- doors and movable equipment,
- machinery, alarms, power systems, or timed devices already present,
- deliberate movement or remote environmental activation.

Do not add a carried gadget whose obvious sole purpose is to emit a monster-specific distraction under the current direction. Contractor equipment should remain practical and believable outside a single entity encounter.

## Temporary Stun

Starting design target:

- range no farther than approximately arm's length,
- cooldown around 20 seconds,
- buys escape or repositioning time,
- does not damage or kill the entity,
- becomes less reliable or changes the entity's approach when repeated.

VR safety rules:

- no real impact is required,
- no forceful punch or swing,
- no required step toward the entity,
- conservative activation envelope inside the safe play area,
- clear successful and failed activation feedback,
- failed use must not encourage frantic repeated striking.

The exact device, gesture, stun duration, and recovery behavior are unresolved.

## Equipment Model

The player may carry multiple items without using a classic inventory screen.

Confirmed constraints:

- no more than roughly two or three equipment items,
- little or no persistent inventory UI,
- objective and world-object carrying remains distinct from active-tool use,
- equipment must be mentally trackable under threat.

Proposed interaction roles, not final controller bindings:

```text
Primary: use the active equipment item
Secondary: grab/carry the objective or a world object
Tertiary: select or cycle between the small equipment set
```

Invisible cycling is risky in VR. Evaluate visible belt/holster slots or a hybrid selector so players can confirm equipment through position, silhouette, sound, and haptics rather than menu text.

## Equipment Categories

### Universal Contractor Equipment

Appears across many contracts and reinforces the shared contractor identity.

Current candidate:

- flashlight.

Other universal items remain undecided.

### Contract-Specific Equipment

Matches the site, objective, or hazard package.

Current proof-contract example:

- Geiger counter for Nuclear Recovery.

A radiation detector should not appear in unrelated contracts unless the environment provides a coherent reason.

Provisional Nuclear Recovery equipment set:

- universal flashlight,
- contract-specific Geiger counter,
- industrial electrical probe for close-range emergency stun.

The stun-tool form remains undecided between the probe and wrench. This equipment set is a design target, not current Sprint 02 implementation authority.

### Improvised World Objects

Picked up or activated inside the site rather than carried into the contract.

Examples:

- debris,
- bottles, cans, tools, or scrap,
- doors and switches,
- machinery and alarms.

## Noise And Stealth

Confirmed noise sources include:

- sprinting,
- dropped or thrown objects,
- careless collisions and physical interaction,
- machinery and doors,
- mission-equipment operation where appropriate.

Noise should be predictable enough that players can learn from it. Ordinary VR tracking noise or minor hand tremor must not create unavoidable detection.

Heart rate may later influence breathing, noise, or entity inference. It remains future research and cannot enter core stealth until sensor quality, calibration, consent, accessibility, failure handling, and non-hardware fallback are proven. Deliberate calming remains emergent; do not require breath-holding or a dedicated calming action.

If researched, the signal path should be:

```text
Heart-rate sensor
    -> signal quality and player calibration
    -> bounded avatar breathing or handling response
    -> audible/visible world stimulus
    -> ordinary entity perception
```

Raw heart rate never supplies the entity with exact player location and never directly decides detection, capture, success, or failure. A simulated or input-driven breathing equivalent must preserve gameplay for players without a sensor.

Before real hardware integration, provide a normalized debug slider and simulated input adapter. The simulator may spike after close encounters and decay during recovery, allowing repeatable positive-feedback and recovery tests. Caps and pacing limits must prevent elevated simulated stress from creating an unavoidable encounter loop.

## Stealth Feedback And Debugging

Player-facing feedback:

- clear avatar breathing,
- optional subtle indicators,
- entity reactions and world audio.

Any visual effect must be optional, comfort-tested, and restrained enough to avoid vection, tunneling, disorientation, or confusion with damage feedback.

Developer/operator debug view must expose:

- concealed, noticeable, or exposed state,
- visibility contributions,
- noise contributions and recent noise events,
- avatar breathing/stress value and source,
- heart-rate adapter mode: Disabled, Simulated, or Live,
- signal quality and calibrated baseline when Live,
- entity evidence, suspicion, and current perception result.

## Environmental Interaction Budget

The player may manipulate doors, lights, machinery, routes, and similar systems, but every contract should choose only a few meaningful interactions.

Approve an interaction only when it supports at least one of:

- objective work,
- entity counterplay,
- route choice,
- a unique haptic or physical effect,
- critical onboarding or feedback.

If an interaction is unlikely to be discovered and used during a 20-minute run, simplify, combine, or cut it.

Current Nuclear Recovery candidates:

- ventilation or radiation-control systems,
- movable cover.

Both require focused prototypes before becoming MLP commitments. Movable cover must not block critical routes, permanently trap the Owl, or encourage unsafe exertion.

## Adaptation As Counterplay Cost

The main long-term drawback of a tactic is that the entity learns from repetition.

This does not mean a tactic becomes useless:

- repeated hiding becomes more likely to be checked,
- repeated routes receive more attention,
- repeated distractions are approached more cautiously,
- repeated stun changes timing or approach,
- tactical variety reduces confidence and creates openings.

Individual tactics may also have clear intrinsic costs such as time, noise, cooldown, close range, or hand occupation. Avoid stacking arbitrary costs when learning already creates enough pressure.

## Current Sprint Boundary

This framework does not authorize new stun, inventory, holster, spatial-hiding, door, machinery, or heart-rate systems in Sprint 02.

The active AI experiment remains route-family adaptation in `20_owl_adaptation_v1_spec.md`. New player tactics require their own approved prototype question and task.

## Open Decisions

1. Exact universal contractor equipment beyond a flashlight.
2. Physical holster, hand, and equipment-selection model.
3. Electrical-probe activation, stun duration, cooldown tuning, and failure feedback.
4. Exact visibility thresholds and whether any optional accessibility indicator is needed.
5. How the player judges their own noise without a HUD meter.
6. Exact flashlight intensity, spill, and entity-visibility response.
7. Ventilation/radiation-system interaction and player value.
8. Movable-cover constraints, destinations, and Owl response.
9. Whether explicit hiding spots can be compromised, destroyed, or temporarily blocked.
10. Future heart-rate calibration and its accessibility-equivalent fallback.
