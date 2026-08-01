# PT3 Design Workspace: Readable Stealth And Contractor Counterplay

Status: Locked internal design record; implementation authority is `33_v0_3_0_0_readable_stealth_scope.md`

Last updated: 2026-08-01

## Audience And Naming

`PT3` is an internal game-design label. The approved implementation uses `v0.3.0.0` and the development rules in `27_development_versioning.md`.

Do not send the exploratory sections of this workspace as a feature list. Send `33_v0_3_0_0_readable_stealth_scope.md` and `34_v0_3_0_0_implementation_handoff.md`.

## Locked Prototype Question

> Can uninformed players deliberately manage graded visibility, movement noise, spatial hiding, and improvised distractions, and correctly explain whether the Owl acted on current sight, an accepted sound, or last credible evidence without a stealth meter?

## Why This Is The Recommended Next Experiment

The current build already provides:

- capable, cover-respecting Owl vision,
- last-credible-position pursuit and local search,
- route adaptation,
- sprint and object-noise stimuli,
- crouch and corner lean,
- loose-object carrying and throwing,
- operator telemetry.

These systems create a useful technical baseline, but they do not yet prove a readable player stealth model. Readable stealth should be tested before recoverable capture, hiding-pattern adaptation, the electrical probe, heart-rate breathing, or a broader environmental-control package.

## Intended Player Learning

By the end of a run, a player should be able to form mostly correct beliefs such as:

- sprinting and careless physical work are loud,
- full cover breaks visual evidence,
- partial exposure can still be seen,
- the Owl searches the last credible place rather than tracking through walls,
- thrown debris can redirect attention without guaranteeing safety,
- changing position after breaking sight is stronger than waiting at the last-known point,
- repeating one response becomes less reliable over time only when the entity has plausible evidence.

The player should learn this through world behavior, sound, avatar feedback, and consequences. A mandatory stealth meter is not the target.

## Approved Scope Summary

The authoritative brief includes:

- one normalized player-noise model using existing movement and object events,
- one readable player-visibility model based on posture, cover, and movement,
- spatial hiding through sight breaks and relocation,
- deliberate debris distraction using existing loose objects,
- graded footsteps, accepted impact/machinery sound, and a restrained Owl investigation cue,
- three fixed maintenance-test panels in addition to portable metal distractions,
- complete operator telemetry explaining detection inputs and Owl conclusions,
- at least one repeatable comparison scenario,
- automated tests for evidence boundaries and full-cover fairness,
- uninformed human tests focused on detection comprehension.

## Locked Exclusions

Keep outside `v0.3.0.0` unless the user explicitly changes the approved scope:

- recoverable capture and nest escape,
- hiding-pattern learning,
- electrical probe or other stun tool,
- flashlight visibility,
- inventory and holsters,
- door locking,
- ventilation/radiation control,
- movable shielding or cart,
- heart-rate hardware or simulated stress,
- cross-run memory,
- learned-model AI,
- new mission, map, or entity,
- final art or animation.

Flashlight visibility is intentionally listed as a decision rather than an automatic feature. It may be valuable, but it adds a distinct perception channel and can obscure whether baseline movement, cover, and sound are readable.

## Resolved Design Choices

- Active inputs: three-sample graded visibility, crouch/walk/sprint noise, full-cover relocation, portable metal, and fixed machinery distraction.
- Crouch: smaller silhouette, slower movement, reduced Noticeable confirmation and quieter locomotion; it is not invisibility.
- Sound: ordinary movement is graded; only evaluated strength at or above the hearing threshold can change investigation or target selection.
- Feedback: footsteps, accepted metal/machinery audio, and a restrained Owl investigation cue; no breathing or visual meter.
- Protected affordance: full visual cover followed by quiet relocation.
- Knowledge law: confirmed sight outranks sound; sound reveals its own origin, not the player's later hidden transform.
- Explicit hide spots, flashlight, capture recovery, stun, inventory, biofeedback, and new environmental-system families remain excluded.
- Primary tests use route adaptation OFF; one integration run uses it ON.

## Locked Success Evidence

In at least three uninformed desktop sessions:

- most players correctly identify at least two major detection causes,
- players intentionally break sight and relocate at least once,
- players use or attempt an environmental distraction for a clear purpose,
- players distinguish active pursuit from last-known-position search,
- full cover is consistently perceived as fair,
- at least one viable response remains during every tested encounter,
- players do not require a mandatory stealth meter to explain the outcome,
- operator telemetry can explain every surprising detection.

Desktop evidence cannot close XR comfort, physical crouch, reach, fatigue, haptics, or play-space safety.

## Principal Risks

- Too many perception channels make failures impossible to understand.
- Stronger Owl perception overwhelms the new stealth affordances.
- Crouch becomes a dominant always-on state.
- Distractions become guaranteed remote control or feel useless.
- Diegetic feedback becomes either unreadable or distracting.
- PT3 quietly expands into capture, inventory, flashlight, and tool development.
- Testing polish replaces observation of player beliefs.

## Delivery Sequence

1. Preserve PT2 feature freeze and collect its human evidence in parallel.
2. Implement perception classification, hearing threshold, and evidence priority before multiplying content.
3. Add movement/impact feedback and one complete fixed machinery interaction.
4. Multiply safe authored content to three route families, subject to the documented cut order.
5. Validate route adaptation OFF, then run one integration sequence ON.
6. Build, smoke-test, and record evidence before `v0.4.0.0` begins on 2026-08-03.

## Decision Record

Use this section during design:

```text
PT3 status: Locked; translated to authoritative v0.3.0.0 scope.
Prototype question: Can uninformed players manage and explain graded sight, sound, hiding, and distractions without a stealth meter?
Included stealth inputs: Three-sample visibility, crouch/walk/sprint noise, full cover plus relocation, thrown metal, three maintenance-test panels.
Excluded systems: Capture recovery, explicit hide spots, flashlight, stun, inventory, environmental control, biofeedback, new mission/map/entity, learned AI.
Player feedback: Graded footsteps, accepted impact/machinery audio, restrained spatial Owl investigation cue; no breathing or visual meter.
Owl evidence rules: Confirmed sight outranks sound; hearing threshold 0.15 after falloff; sound target is the sound origin; hidden live position is never inferred.
Critical protected affordance: Full cover followed by quiet relocation.
Test scenario: Primary sessions with F3 OFF; one integration run with F3 ON.
Pass criteria: Majority comprehension and deliberate use in three uninformed desktop sessions, plus complete automated/build evidence.
Failure criteria: Causal confusion, wall tracking, sight overridden by sound, dominant universal tactic, or breadth preventing a trustworthy build.
Open blockers: Human tester access; all XR and physical-hardware validation.
```
