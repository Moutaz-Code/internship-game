# Current Design Synthesis

Last updated: 2026-07-22

Sources: approved pitch/business case, Offline Game Design Workbook, Prototype 1 implementation, and follow-up design answers.

## Current Design Direction

The game is a short-session VR extraction horror framework about completing different dangerous paid contracts under pressure.

The player is not a lost civilian, a helpless victim, or an all-powerful soldier. They are a capable contractor who knows the job but cannot fully control the facility or its creature.

Full-game player promise:

> Take a dangerous contract, perform physical work in a hostile place, learn and counter-adapt to an intelligent entity that is learning you, and get out before the job closes around you.

Current proof contract:

> Identify and recover three live fuel rods from a post-apocalyptic nuclear facility while surviving the Owl Thing.

## Primary Experience

The objective is not identification alone or carrying alone. The fun target is surviving while doing both.

The intended emotional arc is:

- Minute 0-2: understand the contract, absorb the atmosphere, and form an initial route.
- Minute 3-7: discover and assay candidates while learning the creature's pressure language.
- Minute 8-15: adapt after close calls, disrupted routes, or recoverable setbacks.
- Extraction: hold together long enough to finish the job and leave.
- After the run: immediately understand how another seed or another strategy could go differently.

## Pillars

### Dangerous Contract Clarity

The player should know the job quickly. Uncertainty belongs in execution, routes, rod discovery, and creature behavior, not in vague mission wording.

### Adaptive Entity, Situational Counterplay

Every contract has an intelligent entity with no single solved answer. For the Owl, hiding, sight breaks, distraction, limited stunning, and rerouting each solve a particular moment and carry a tradeoff. The player learns its behavior while it develops bounded responses to theirs.

### Physical Work Under Pressure

Assaying, carrying, dropping, depositing, and recovering the objective should create the tension. Interaction should feel like work in the world, not button-spam.

### Haptics As Mechanical Language

Each physical effect has a narrow in-world meaning. Heat confirms a live rod, proximity vibration represents Owl pressure, and Wind represents extraction cycling unless hardware tests force a remap.

### Arcade-Fair Recovery

A paid 20-minute session should not end instantly because of one capture. Close calls and captures create escalating, legible setbacks while preserving a realistic recovery path.

## Confirmed Design Calls

- Player fantasy: contractor doing a dangerous paid recovery job.
- Engine: Unity.
- Objective: identify and transport fuel rods while surviving.
- Threat: one smart creature with mixed counterplay and no universal tactic.
- Product structure: a roster of curated contract scenarios; the Owl/fuel-rod/nuclear-facility combination is only the first proof contract.
- Every contract centers on an intelligent adaptive entity; disasters and hazards are secondary dangers.
- Entity memory lasts for one run. One use creates suspicion and two repeated uses permit adaptation.
- Direct observation matters more than inferred evidence.
- The first three adaptation signals are route repetition, hiding repetition, and distraction/stun reliance; Prototype 1 validates route repetition first.
- Tactical variety weakens entity confidence, while repetition strengthens it.
- Adaptation is communicated diegetically through animation, routes, and changed responses; no player-facing notification is used.
- Entities may bluff, and an invisible pacing director may create believable back-off windows.
- Entities cannot be killed in the internship scope; the player escapes, hides, distracts, delays, locks out, or temporarily stuns them.
- Hiding is primarily spatial, with a few explicit emergency hiding spots.
- Environmental debris, machinery, remote noise, doors, and close-range stun provide counterplay; bespoke anti-entity noise gadgets are not part of the current direction.
- Stun is approximately arm's-length with a starting cooldown near 20 seconds and no forceful real-world impact.
- The player carries no more than roughly two or three equipment items with little or no persistent UI.
- Sprinting and careless object handling create meaningful stealth noise.
- Equipment is divided between a small universal contractor kit and contract-specific tools such as the Geiger counter.
- Repeated counterplay primarily costs future effectiveness because the entity learns; additional costs are secondary and tactic-specific.
- Shared stealth uses graded concealed, noticeable, and exposed states interpreted through entity-specific senses.
- Physical crouching is primary with a controller-assisted accessibility option.
- Flashlight use is a visibility tradeoff: the player sees the environment and visually oriented entities may see the player.
- Repeated emergency hiding spots may be inspected only after plausible evidence and with fair warning/escape opportunity.
- Nuclear Recovery environmental candidates are ventilation/radiation systems and movable cover.
- The provisional loadout is flashlight, Geiger counter, and electrical probe.
- Future heart-rate stealth acts through avatar breathing or other world-readable behavior, never raw direct detection.
- The August MLP uses deterministic adaptive game AI; a pre-trained strategy model is future R&D behind deterministic validation and fallback.
- Player stealth judgment uses clear character breathing, optional subtle indicators, and a complete debug view.
- Biofeedback testing begins with a normalized slider and deterministic encounter-driven simulator.
- Deliberate calming remains emergent; the game does not require breath-holding.
- Electrical probe is the selected close-range stun-tool direction.
- The no-cart rod loop remains current; an optional staging cart is a separate prototype candidate.
- Shielding panels remain local/constrained rather than portable full-run protection.
- Stronger adaptation is rejected as the automatic second-capture penalty; a different recoverable setback is pending.
- Contracts are employer-assigned in fiction and randomly selected from eligible curated packages in gameplay.
- Scenarios are isolated arcade episodes connected by contractor fantasy, visual identity, and the enter/work/adapt/extract rhythm.
- Content combinations are curated and may use selective compatibility tags.
- Tone target: horror-adjacent for approximately age 12 and above, with implication instead of graphic harm.
- Owl capture fiction: unconsciousness and implied predation without direct graphic depiction.
- Owl capture escalation: two survivable captures, with the second more severe and the third currently terminal.
- Owl recovery: banked rods stay safe, carried items scatter, recovery uses one of roughly five authored zones, and the Owl retreats for about 60 seconds outside the final five minutes.
- Owl recovery escalation: first capture requires a short equipment reboot; second requires a short nest escape plus a heavier non-adaptation penalty still in design; third capture is terminal for the current test.
- Final-five-minute recovery protection: approximately 20-30 seconds.
- Encounter severity: near misses are sensory, physical contact disrupts an item or equipment, and capture triggers the full recovery loop.
- Rod mistakes: immediate gameplay consequence; score is secondary.
- Replay: restart immediately with seeded variation.
- Loadout/preparation choices: deferred as a later replayability layer.
- Persistent progression: not part of the current MLP.

## Prototype 1 Reality

Prototype 1 has approximately 90% of its intended feature skeleton implemented. It includes rudimentary movement, a threat, and the fuel-rod objective, plus broader authored-facility, variation, feedback, and delivery infrastructure documented in the prototype log.

This is a strong technical starting point, but it is not design validation. The current build still needs human evidence for:

- objective comprehension,
- 15-20 minute pacing,
- route planning,
- threat fairness and intelligence,
- capture recovery,
- immediate mistake feedback,
- replay desire.

Desktop tests can answer those questions provisionally. They cannot prove VR comfort, XR interaction quality, or physical hardware value.

## Hardware And Platform Status

- Engine: Unity 6000.5.4f1.
- Target headset/runtime: unknown.
- Real haptic and sensor devices: unavailable during office renovation and remote work.
- Current response: continue with simulated feedback and device-independent interfaces.
- Heart-rate mechanics: deferred from core success, failure, and detection.

## Decisions Still Needed

These decisions are parked until the PT2 gate unless the user explicitly redirects scope.

1. Define the exact equipment-reboot and Owl-nest escape interactions.
2. Define the exact non-adaptation penalty added after the second capture.
3. Choose the authored carried-item scatter rule.
4. Define the exact immediate invalid-rod consequence.
5. Confirm target headset, runtime, play-space, and venue operator flow.
6. Inventory available haptic and sensor devices, SDKs, owners, and access dates.
7. Decide which Owl counterplays must ship in the MLP after observing baseline play.
8. Tune and validate the bounded route-family adaptation experiment in `20_owl_adaptation_v1_spec.md`.
9. Later, define the critical player behavior or affordance entities must never counter.
10. Define the physical two-to-three-item selection and holster model on target VR hardware.
11. Define diegetic player-noise feedback and exact graded-visibility thresholds.
12. Define the electrical probe's safe VR interaction, stun duration, and failure feedback.
13. Prototype ventilation/radiation control and movable cover separately before committing either.
14. Keep heart-rate stealth and trained-model AI as future R&D until their respective gates can be tested.
15. Select a non-adaptation second-capture penalty.
16. Decide whether optional staging-cart Variant B deserves a prototype.
17. Choose one primary and one secondary ventilation/radiation effect while preserving unique Heat, Wind, and Geiger meanings.

## Next Validation Target

Prototype 2 should answer:

> Does the Owl appear to notice repeated route-family use, become suspicious after one credible observation, adapt after two, and encourage the player to vary routes without feeling omniscient or making any route unusable?

Follow `24_prototype_02_scope.md`. Do not add capture recovery, new player tools, environmental systems, heart rate, hardware, or learned AI until this question has evidence.
