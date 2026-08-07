# Game Design Agent Continuity

Status: Living internal continuity note for game-design assistants

Last updated: 2026-08-02

## Audience Boundary

This file exists to preserve judgment and context between game-design assistant sessions. It is not implementation authority.

Implementation agents should receive a separate, explicitly approved, version-oriented brief. Do not treat an idea in this file as authorization to change the Unity project.

## Project At A Glance

- Company context: Pixoul.
- Delivery target: polished minimum lovable product by 2026-08-28.
- Engine: Unity 6000.5.4f1.
- Unity project: `UnityProject/InternshipGameSynced`.
- Main scene: `Assets/_Project/Scenes/P01_CoreLoop.unity`.
- Current local build identity: `v0.3.0.0`.
- Current published `master` commit: `v0.2.3.0` at `0636f5d` as of the 2026-08-02 audit.
- Repository state on 2026-08-02: local `v0.3.0.0` implementation and documentation are present in a substantial dirty working tree. Treat all changes as user-owned. Do not commit, push, discard, overwrite, or revert them without an explicit request.
- Target headset, venue runtime, physical play space, and available haptic/sensor devices remain unconfirmed.

## Current Product Direction

The player is a contractor performing dangerous paid work, not a soldier or helpless victim.

The full product direction is a roster of isolated 15-20 minute arcade contracts. Each curated contract combines:

- one site,
- one objective family,
- one primary intelligent entity,
- secondary hazards,
- a limited practical equipment set,
- extraction under escalating pressure.

The August proof contract is:

> Identify, scan, and recover three live fuel rods from a post-apocalyptic nuclear facility while surviving the Owl Thing, then endure the extraction cycle and leave.

The proof contract is not the permanent identity of the whole game.

## Current Build Reality

Prototype 2 began as the Readable Route Adaptation experiment and later received explicit user-directed desktop and presentation work. It is published at `v0.2.3.0` and feature-frozen.

`v0.2.3.0` includes:

- a ten-space authored facility with West, Central, and East route families,
- three live rods, two inert rods, and eight possible sockets,
- three-second held scanning and one-at-a-time transport,
- a three-rod extraction rack and 20-second exit cycle,
- seeded route, lighting/audio, rod, and Owl variation,
- run-local Owl route evidence with Uncertain, Suspicious, Adapted, and Counter-Adapted behavior,
- F1 telemetry, F2 Geiger interference comparison, and F3 adaptation comparison,
- desktop crouch, lean, momentum-based movement, pickup assistance, carrying, dropping, and throwing,
- fitted portal geometry and room-specific greybox dressing,
- multi-sample, cover-respecting Owl perception and last-credible-position search.

Latest recorded automated evidence for local `v0.3.0.0`:

- focused Edit Mode: 22/22 passed.
- full Edit Mode: 133/133 passed.
- Play Mode: 6/6 passed.
- Strict Windows build succeeded with zero errors and two rendering warnings.
- The exact player survived a deliberate 12-second smoke window.

This is strong implementation evidence, not complete game-design validation.

## Current Design And Delivery Gate

Prototype 2 feature expansion is closed. It still needs human evidence.

At least two uninformed desktop sessions are required, with three preferred. Validate whether players:

- notice changed Owl behavior,
- connect it to repeated route use,
- deliberately vary routes,
- perceive a temporary opening after varying routes,
- describe the Owl as observant rather than omniscient,
- retain at least one viable route,
- understand the contract quickly and want to replay.

Use `32_pt2_human_validation_sheet.md` to collect and classify this evidence.

Do not reopen PT2 as an indefinite polish bucket. Tune it only in response to observed evidence or a release-blocking defect.

`v0.3.0.0` is implemented and automated delivery-validated locally. It includes graded visual confirmation, movement noise, full-cover relocation, portable and fixed distractions, diegetic feedback, and operator telemetry. Its uninformed human comprehension evidence remains Not Tested. See `33_v0_3_0_0_readable_stealth_scope.md`, `34_v0_3_0_0_implementation_handoff.md`, and `35_v0_3_0_0_human_validation_sheet.md`.

The design protects full visual cover followed by quiet relocation. Confirmed sight outranks sound, and an accepted sound reveals only its own origin. The active feedback set is footsteps, accepted metal/machinery audio, and a restrained Owl investigation cue; breathing and visual stealth indicators are not in v0.3.

## Approved Next Design Direction

The approved next experiment is `v0.4.0.0` Spatial Readability and Pursuit Topology using the locked 7C macro layout.

Locked question:

> Can uninformed players form a usable mental map of a three-lobed nuclear facility and deliberately switch between its distinct connectors and cover grammars to complete the existing three-rod contract while the Owl’s sight, sound, search, and route adaptation remain explainable?

The facility is an asymmetric circuit around an inaccessible shielding/service void: Containment & Control, Coolant & Maintenance, and Fuel Handling & Waste, connected by North Pipe Gallery, Utility Spine, and South Maintenance Channel. The authority is documents 37–39. `31_pt3_design_workspace.md` is historical design provenance, not v0.4 scope.

## Naming Boundary

Game-design conversations may use `PT3` and `PT3.x` as lightweight internal labels for design iterations.

Implementation agents should remain on the development version scheme in `27_development_versioning.md`:

- current weekly implementation begins at `v0.4.0.0` on 2026-08-03,
- major changes inside that weekly prototype increment `z`,
- minor changes increment `w`.

When preparing a later implementation handoff, translate approved design into the active four-part version line. Do not burden the implementation agent with internal design-iteration naming.

## Stable Design Principles

- Use one observable prototype question per week.
- Perceived intelligence matters more than technical novelty.
- The MLP uses deterministic, testable adaptive AI; learned-model AI is future R&D.
- Entity memory lasts for one run.
- Direct evidence matters more than inference.
- Repetition invites adaptation; varied tactics reduce confidence and create openings.
- Adaptation changes pressure but never silently disables a tactic or reveals hidden exact location.
- Entities are unkillable during the internship scope.
- Player counterplay emphasizes sight breaks, spatial hiding, distraction, environmental control, delay, and limited close-range defense.
- The intended audience is roughly age 12 and above. Capture may imply danger or feeding but should not depict graphic harm.
- VR comfort, accessibility, physical fatigue, play-space safety, and operator reliability outrank feature count.
- Haptic meanings remain narrow: Heat confirms live rods, vibration communicates Owl proximity, and Wind communicates extraction cycling unless hardware evidence forces a change.
- Hardware input must remain separated from gameplay decisions through simulated/disabled/live adapters.
- Human playtest evidence and automated correctness evidence must never be presented as interchangeable.

## Collaboration Notes

The user values momentum, concrete questions, practical scope cuts, and honest challenge. They are comfortable with assumptions when those assumptions are clearly labeled and reversible.

Avoid:

- freezing early ideas as final design,
- asking broad questions after enough information exists to run a focused experiment,
- producing a large feature list without one prototype question,
- letting implementation polish replace player evidence,
- interpreting "smart entity" as a requirement for machine learning,
- treating the fuel-rod/Owl contract as the entire future game,
- sending exploratory design material to the implementation agent as though it were approved scope.

The implementation agent is capable of moving quickly and may implement later explicit user requests beyond an original frozen scope. Future design assistants must distinguish:

- original experiment scope,
- later user-directed increments,
- current local implementation reality,
- validated player experience.

## Source Priority For Design Work

When sources disagree, use:

1. The newest explicit user instruction.
2. This continuity note and the current `00_project_hub.md`, after verifying their dates.
3. Active decisions in `05_decision_log.md`.
4. Evidence in `06_prototype_log.md`.
5. `14_current_design_synthesis.md`.
6. Current approved prototype or version scope.
7. Architecture and risk boundaries.
8. Framework documents and exploratory workbooks.

Never silently select the most expansive interpretation.

## High-Value Open Questions

- What is the target headset, runtime, physical bay, and operator flow?
- Which haptic and sensor devices, SDKs, owners, and access dates are real?
- Does PT2 route adaptation pass human observation?
- Do uninformed players understand the approved v0.3 sight, sound, hiding, and distraction model?
- Which threshold, cue, or placement changes are justified as bounded `v0.3.z.w` tuning?
- Do the fixed and portable distractions remain situational alternatives rather than guaranteed control?
- Do players recognize all three v0.4 lobes and return to the annex without an external map?
- Are North, Utility, and South perceived as meaningfully different connector risks?
- Does any v0.4 connector or Owl hold point become a mandatory choke?
- When should flashlight visibility, explicit emergency hiding, and the electrical probe receive separate experiments after baseline stealth evidence?
- What recoverable capture loop preserves a paid arcade session without creating a failure spiral?
- What minimum art/readability pass is necessary for the August stakeholder demo?

## Immediate Recommended Sequence

1. Re-audit and preserve the user-owned dirty tree and local v0.3 baseline.
2. Implement v0.4 data/graph and explicit connector semantics before generated geometry.
3. Build the complete three-lobed shell and prove player/Owl navigation before presentation detail.
4. Add the three lobe grammars, objective distribution, Owl graph, and telemetry.
5. Run focused/full regression, version/build/smoke gates, then use `40_v0_4_0_0_human_validation_sheet.md`.
6. Continue PT2 and v0.3 human validation separately; do not merge or overclaim evidence.

## How Future Agents Should Maintain This File

At every design-agent transfer:

- update the date,
- verify repository and published-build state,
- replace stale current-phase language,
- record the newest validated evidence,
- separate confirmed decisions from proposals,
- identify the next prototype question,
- preserve useful judgment from previous agents,
- remove resolved warnings only when evidence exists.

Keep this useful and compact. Historical implementation detail belongs in `06_prototype_log.md`, not here.
