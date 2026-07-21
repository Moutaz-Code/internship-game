# Prototype Log

Use this file to track what each prototype is supposed to prove. A prototype without a question is usually just a feature pile.

## Prototype Entries

### P0: Planning Spine

Date range: 2026-07-06 to 2026-07-10

Question:

> Can we define a first playable VR extraction horror slice with clear scope, risks, fallback paths, and ClickUp tasks?

Build status: Complete

Primary deliverables:

- Living docs.
- ClickUp backlog seed.
- First playable scope.
- Risk register.
- Initial technical architecture outline.
- Playtest template.

Success criteria:

- Engine/hardware unknowns are clearly identified.
- First playable can be described in one paragraph.
- Every risky feature has a fallback.
- Sprint 02 work can begin without major ambiguity.

Notes:

- First playable should validate the loop before final art, complex AI, or real haptics.

### P1: First Playable Extraction Loop

Target date: TBD, recommended Sprint 02

Question:

> Is it tense and understandable to enter a small VR facility, locate a signal core, sense a threat, and extract?

Build status: Superseded by P2. Its core extraction-loop question remains relevant, but the one-core and compact-facility implementation assumptions no longer represent the prototype.

Must include:

- VR player baseline.
- Greybox facility.
- One signal core objective.
- Start and extraction state.
- Threat pressure cue.
- Simulated haptic or physical cue.

Success criteria:

- First-time player understands objective within 2 minutes.
- Player can complete a full run.
- At least one moment creates tension.
- No major comfort issue blocks testing.
- Session can restart cleanly.

What this prototype should not prove:

- Final art direction.
- Full procedural generation.
- Final enemy AI.
- Multiplayer.
- Persistent progression.

### P2: Multi-Path Fuel-Rod Vertical Slice

Date range: 2026-07-16 to first external playtest

Question:

> Can a first-time desktop player plan routes through a larger authored facility, reliably identify and transport three live fuel rods under fair adaptive Owl pressure, and survive a clearly communicated extraction holdout?

Build status: Feature skeleton approximately 90% implemented by developer estimate. Automated implementation and build checks pass; movement, threat behavior, and the fuel-rod loop are still rudimentary, and external pacing, fairness, comprehension, and replay-interest evidence are pending.

Maturity note:

- "90% implemented" describes Prototype 1's planned technical feature list, not MLP completion or design quality.
- Desktop evidence does not yet establish VR comfort or physical interaction quality.
- Real headset, haptic, and sensor behavior is untested because devices are not currently available.

Applicable design pillars:

- Mission clarity through one repeated objective type and a visible three-slot rack.
- Route planning through west, central, and east route families with authored seeded states.
- Fair Owl counterplay through warnings, sight breaks, bounded interference, searchable loops, and distraction stimuli.
- Physical interaction through exact one-at-a-time carrying, deterministic assay, rejection, and deposition.
- Meaningful room feedback with unique semantics: Heat confirms live rods, proximity vibration communicates the Owl, and Wind communicates extraction cycling.

Must include:

- A roughly 36 m x 55 m, 10-space authored facility with at least two non-jump routes to each terminal objective room.
- Three live rods and two inert rods selected from eight authored sockets by an immutable seed configuration.
- A three-second held assay, integrated Geiger guidance, and bounded optional Owl click interference.
- Desktop jump with optional shortcuts, coyote time, buffering, landing telemetry, and no required jumping.
- One Owl able to patrol, investigate, hunt, search, disengage, react to stimuli, and adapt patrol pressure by wing.
- A three-slot deposit rack, inert/unassayed rejection, 20-second extraction cycle, and final exit crossing.
- Same-seed restart, new-seed restart, operator overlay, player-facing mission feedback, clean failure, and complete reset.

Success criteria:

- First-time intended completion is 15-20 minutes; experienced/debug completion is approximately 9-12 minutes.
- The mission is understandable within two minutes and rod truth remains reliable even with Geiger interference enabled.
- All critical paths work without jumping under every route profile; the Maintenance shortcut works with jumping.
- Identical seeds reproduce placements and profiles, and at least three variables visibly change between suitable seeds.
- Carry error remains under 1 cm and 0.5 degrees during look, sprint, and jump; dropping restores the complete physics/interactable state.
- Partial delivery, invalid rods, duplicate entry, holdout, extraction, timeout, capture, and reset are deterministic.
- The Owl never crosses walls or creates an unavoidable spawn/catch during the extraction cycle.
- Automated tests, three-seed play checks, a repeated-seed check, a fresh Windows build, and a launch smoke test pass.

Scope note:

- This prototype intentionally supersedes P1's older one-objective-instance and 3-6-room cut line. It still respects the MLP boundary of one player, one fuel-rod objective type, one industrial facility style, and one Owl. It does not add procedural geometry, multiplayer, persistent progression, a second enemy, final art, required jumping, real heat hardware, or heart-rate-driven success/failure.

Pass evidence:

#### Pass 1 - Carrying and locomotion

- Prototype question: can desktop movement mimic the intended first-person physical loop closely enough to validate repeated object transport?
- Pillars supported: physical interaction, fair counterplay, and route planning.
- Evidence: desktop carrying now enforces the authored grip pose in `LateUpdate`, supports per-item offsets and mutually exclusive desktop/XR ownership, and restores parent, Rigidbody, collision, interpolation, and XR-interactable state on release. Jump implements the specified 0.9 m height, 0.12-second coyote time and input buffer, landing telemetry, and no double jump. XR jump is disabled by default.
- Validation: automated Play Mode coverage measures held-pose error below 1 cm and 0.5 degrees across moving/rotating frames and verifies full release-state restoration; Edit Mode coverage exercises jump, buffering, coyote time, landing, ceilings, and double-jump prevention.

#### Pass 2 - Aggregate rods and deterministic run states

- Prototype question: does one repeated objective remain clear when identification, mistakes, delivery order, failure, and reset all matter?
- Pillars supported: mission clarity, physical interaction, and meaningful feedback.
- Evidence: five unique rods, a `FuelRodObjectiveSet`, complete run-state model, three-slot visible rack, force release and immutable accepted rods, typed rejection results, and full reset are implemented. Only confirmed-live rods are accepted; unassayed and inert rods produce distinct responses, and deposits cannot mutate a failed or completed run.
- Validation: automated coverage includes unique registration, partial delivery, rack capacity, unassayed/inert rejection, duplicate prevention, three valid deposits, holdout entry, escape, timeout, capture, and reset.

#### Pass 3 - Expanded authored facility

- Prototype question: can a substantially larger space create route decisions without obscuring the mission or allowing geometry exploits?
- Pillars supported: route planning, fair Owl counterplay, and mission clarity.
- Evidence: the generated facility now covers approximately 36 m x 56 m across 10 named spaces, three route families, cross-links, cover and landmarks, a no-jump critical path, and an optional Maintenance jump shortcut with a ramp alternative. Eight typed rod sockets, extraction anchors, route blockers, zone anchors, lighting variants, and Owl patrol anchors replace object-name lookups. Only the generated root is rebuilt.
- Validation: graph tests prove two edge-disjoint non-jump routes to each terminal under every profile; integrated Play Mode tests verify complete baked NavMesh paths from player spawn to all eight sockets. Scene validation reports no missing scripts or broken prefabs.

#### Pass 4 - Seeded authored variation

- Prototype question: can repeated runs feel meaningfully different while remaining reproducible and navigable?
- Pillars supported: route planning and fair counterplay.
- Evidence: immutable `RunConfiguration` selects exactly one live rod per terminal, two inert sockets, one authored route profile, one Owl profile, and one lighting/audio profile. R repeats the current seed, Shift+R creates a new seed, and F1 exposes the operator view.
- Validation: seeds 1337, 2027, and 9001 plus a repeated seed are covered; identical seeds reproduce the same configuration and all profiles preserve critical paths.

#### Pass 5 - Assay, Heat, and Geiger language

- Prototype question: can players discover rods under uncertainty but still establish rod truth without ambiguity?
- Pillars supported: mission clarity and meaningful feedback.
- Evidence: a continuous three-second held assay resets on an incomplete drop. Live confirmation alone drives conservative Heat and amber desktop simulation; inert rods use a cool pulse and distinct audio. Geiger guidance follows the nearest undeposited live rod. Optional Owl interference is limited to an active Owl within 8 m, at most 20 percent jitter, and one one-second false burst per 12 seconds; it cannot alter classification or trigger Heat. F2 enables it, while baseline play defaults it off.
- Validation: deterministic model tests prove the interference bounds and typed-feedback tests preserve the unique Heat, vibration, and Wind meanings.

#### Pass 6 - Owl navigation and adaptation

- Prototype question: can one readable threat pressure multiple routes, react to the player's physical actions, and recover fairly when sight is broken?
- Pillars supported: fair Owl counterplay, route planning, and physical interaction.
- Evidence: the Owl now separates perception/state decisions from `IThreatNavigator`; Unity AI Navigation supplies complete-path movement, with a collision-validated waypoint fallback. It patrols, warns, investigates stimuli, hunts, honors lost-sight grace, searches last-known space, disengages, and biases patrol after repeated wing use. Sprinting, landings, dropped metal, rod alarms, and objective progress use `IThreatStimulusSink`; scripted sightings reuse the same Owl presentation.
- Validation: automated and integrated tests cover state transitions, pressure inputs, wing bias, route-profile updates, complete NavMesh routes, floor-root conventions, and failure-closed fallback segments.

#### Pass 7 - Extraction climax and presentation

- Prototype question: does the final delivery create a readable escalation and explicit ending rather than silently completing the run?
- Pillars supported: mission clarity, fair counterplay, and meaningful feedback.
- Evidence: the third valid deposit starts a 20-second extraction cycle with emergency lighting, countdown audio, simulated Wind, two approach lanes, and bay cover. The exit opens only after cycling and the player must cross it. Player HUD and diegetic displays show deposits, assay status, countdown, outcomes, and restart guidance; operator data remains separate. Desktop pause manages cursor and simulation state.
- Validation: integrated Play Mode coverage performs three deposits, a shortened deterministic holdout, explicit escape, failure, and reset.

#### Pass 8 - Delivery gate

- Prototype question: is this exact package and scene state stable enough for the first external playtest?
- Pillars supported: all five pillars through end-to-end validation.
- Evidence: 79 Edit Mode tests and 3 integrated Play Mode tests pass; the active scene contains 506 objects with zero missing scripts and zero broken prefabs. The final strict clean Windows build completed in 20.96 seconds at 115.39 MB with zero errors and zero warnings. The exact player stayed alive through an eight-second D3D12/Input/physics/OpenXR initialization smoke test with no fatal log matches.
- Build hygiene: removing unused Unity AI Assistant and AI Inference reduced the player from 140.11 MB to 115.39 MB and removed 492 package-shader warnings. Restoring XR Interaction Toolkit's conventional tracked `Assets/Samples` folder removed its remaining sample-cache warning. Unity AI Navigation remains installed and validated.
- Remaining human evidence: the 15-20 minute first-time pacing target, two-minute mission comprehension, 9-12 minute experienced timing, and post-run cue recall require an external player and are not claimed by automation.

Supersession record:

- The expanded facility and multi-rod loop intentionally supersede the older one-rod/3-6-room cut line. The slice still contains one objective type, one Owl, one industrial facility style, authored rather than procedural geometry, and no required jumping.
