# v0.3.0.0 Scope: Readable Stealth And Contractor Counterplay

Status: Authoritative implementation scope

Approved: 2026-08-01

Target implementation window: 2026-08-01 through 2026-08-02

Next weekly line: `v0.4.0.0` begins 2026-08-03

## Authority And Naming

This brief is the player-facing authority for `v0.3.0.0`. It promotes the approved parts of `31_pt3_design_workspace.md` into implementation scope. Broader stealth, counterplay, capture, hardware, and AI documents remain background only.

Implementation agents must use the four-part version identity `v0.3.0.0`. `PT3` and `PT3.x` are internal game-design labels and should not appear in implementation task names, build labels, or reports.

Prototype 2 remains feature-frozen at `v0.2.3.0` except for evidence-driven tuning or release-blocking defects. Its human validation can continue against the frozen build without reopening its feature scope.

## Prototype Question

> Can uninformed players deliberately manage graded visibility, movement noise, spatial hiding, and improvised distractions, and can they correctly explain whether the Owl acted on current sight, an accepted sound, or last credible evidence without a stealth meter?

## Intended Player Loop

The player should be able to:

1. move quietly while selecting cover;
2. recognize that sprinting and hard physical impacts are loud;
3. break every visual sample with full cover;
4. relocate quietly instead of waiting at the last-seen point;
5. redirect an unseeing Owl with thrown metal or a fixed maintenance test panel;
6. understand from sound and Owl behavior whether the tactic was accepted;
7. continue the existing fuel-rod contract rather than enter a separate tutorial mode.

## Protected Affordance

Full visual cover followed by quiet relocation is the protected baseline response.

- With zero valid visual samples, the Owl must not receive the player's live hidden position from vision.
- A sound may reveal its own world origin when it passes the hearing threshold. It does not reveal the player's later position.
- Route adaptation may alter patrol and search pressure, but it must not invalidate full cover, teleport the Owl, or secretly track through walls.

This is the v0.3 resolution of the protected-agency question. It may be revisited only after observed play.

## Must Have

### 1. Graded Visual Evidence

Reuse the Owl's three existing player samples. Expose one pure, testable classification and confirmation policy:

| Visible samples | Player state | Default confirmation behavior |
|---:|---|---|
| 0 | Concealed | Visual confirmation decays at `1.0` per second. |
| 1 | Noticeable | Confirmation gains `0.45` per second while crouched or nearly still and `0.65` per second otherwise. |
| 2-3 | Exposed | Confirmation gains `1.35` per second. |

Prototype defaults:

- confirmation range: `0.0` to `1.0`;
- confirmed visual contact threshold: `1.0`;
- sprinting multiplies positive confirmation gain by `1.35`;
- crouch affects the Noticeable rate and the player's physical silhouette, but it does not make visible samples invisible;
- once the Owl is already Hunting or Searching, any visible sample may maintain or reacquire live visual contact;
- zero samples begins the existing lost-sight grace and last-seen search behavior;
- all values remain serialized/configurable and operator-visible.

The player receives no visibility meter, outline, vignette, eye icon, or detection bar. A restrained Owl noticing/investigation cue may occur when visual evidence becomes actionable, with rate limiting.

### 2. Normalized Movement And Impact Noise

Extend the existing stimulus path instead of creating a second hearing system. Evaluate hearing after distance falloff.

Default movement bands:

| Movement | Pulse interval | Intensity | Radius | Player-facing sound |
|---|---:|---:|---:|---|
| Crouch locomotion | `0.80 s` | `0.16` | `6 m` | Very restrained footstep. |
| Ordinary walk | `0.68 s` | `0.32` | `10 m` | Subdued footstep. |
| Sprint | `0.45 s` | `0.62` | `15 m` | Clearly audible heavy footstep. |

Additional rules:

- standing still emits no movement pulse;
- keep the existing landing range of approximately `0.35-0.75` intensity and `15 m` radius;
- keep thrown/dropped metal velocity-scaled, approximately `0.40-0.90` intensity and `10-22 m` radius;
- an evaluated strength below `0.15` is rejected for investigation and cannot replace the Owl's knowledge target;
- route evidence may retain its stricter existing `0.30` minimum;
- footstep timing, emitted movement pulses, and impact acceptance must be deterministic enough for focused tests;
- values are prototype defaults and may receive evidence-driven tuning inside the `v0.3.z.w` line.

### 3. Evidence Priority And Fair Target Selection

The Owl must distinguish these knowledge sources in code and telemetry:

1. confirmed current visual contact;
2. accepted sound origin;
3. last confirmed visual position;
4. local search probe;
5. patrol target.

Rules:

- confirmed current sight outranks sound;
- while the Owl has valid current sight, a thrown object or machinery sound may add pressure and route evidence but may not redirect the Owl away from the seen player;
- after sight is fully broken, an accepted sound may redirect investigation or search to the sound origin;
- a rejected sound changes neither state nor target;
- sound stores the sound origin, kind, evaluated strength, and time, not an inferred live player position;
- every target change must be explainable in F1 operator telemetry;
- preserve the existing two-second lost-sight grace and ten-second local search as initial comparison values unless a focused test proves they block the experiment.

### 4. Spatial Hiding And Quiet Relocation

- Use actual walls, columns, equipment, crouch, and lean; do not add universal hide volumes.
- Full cover means all three samples are occluded.
- Partial exposure remains Noticeable or Exposed according to sample count.
- Crouch remains hold-to-use on desktop and collision-limited lean remains `Z`/`X`.
- At least one authored comparison opportunity in each route family must combine a sight break, a nearby relocation option, and a distraction origin.
- The Owl searches the last credible position and bounded local probes; it does not receive a hidden transform update.

### 5. Portable And Fixed Distraction Content

Provide two contractor-readable counterplay forms.

Portable metal:

- retain the existing pick-up/drop/throw controls;
- provide nine loose metal props total, distributed across West, Central, and East route families;
- use at least two readable silhouettes, but keep one gameplay noise family for this experiment;
- play a spatial metal impact sound only when an armed impact is accepted;
- one accepted impact creates one stimulus; bounce spam inside the cooldown does not.

Fixed machinery:

- add three `MAINTENANCE TEST` panels, one per route family;
- use the existing `IPlayerInteractable` look-and-use path with prompt `TEST MACHINERY`;
- activation runs a localized mechanical alarm for about `2.5` seconds, emits one new typed `Machinery` stimulus at `0.75` intensity and `18 m` radius, and shows a local amber active state;
- add the corresponding typed `Machinery` route-evidence kind so adaptation-enabled integration remains explainable;
- each panel has a `25` second cooldown and resets with the run;
- the sound origin is the panel, never the player's current transform after activation;
- place each panel where the player can trigger it, break sight, and take a different nearby path; do not obstruct portals, NavMesh, rod sockets, or required carrying clearance.

The fixed panel is reliable but location-bound and cooldown-limited. Thrown metal is portable but requires acquiring, carrying, aiming, and spending a loose prop. Neither guarantees safety, distracts through confirmed sight, or disables the Owl.

### 6. Diegetic Feedback

Required player-facing feedback:

- graded footstep loudness, with sprint clearly readable;
- a localized metal impact for an accepted throwable stimulus;
- a localized machinery test sound and amber active state;
- one distinct, restrained, spatial Owl investigation cue when it accepts a sound or begins an actionable visual investigation;
- existing Owl detection, proximity, capture, objective, and extraction cues remain semantically distinct.

Do not add avatar breathing, heartbeat, a screen-space stealth effect, a visual stealth meter, or a normal-HUD debug label in `v0.3.0.0`.

### 7. Operator Telemetry And Comparison Procedure

Extend F1 operator information with:

- visible sample count and Concealed/Noticeable/Exposed state;
- visual confirmation value, gain/decay mode, and confirmed-contact status;
- motion band, latest movement pulse, and crouch/sprint state;
- last stimulus kind, raw intensity/radius, evaluated strength, `Accepted` or `Rejected`, and rejection reason;
- current knowledge source and current movement target;
- last seen and last heard positions and elapsed times;
- machinery-panel cooldowns or ready count;
- route-adaptation enabled state and existing route evidence summary.

F1 remains operator-only. Normal players must not see these values.

Primary v0.3 human sessions use route adaptation disabled through F3 so the stealth model can be interpreted in isolation. The delivered build keeps the existing normal default of adaptation enabled. Complete at least one automated or observed integration run with adaptation enabled before delivery.

### 8. Reset, Determinism, And Build Identity

- Same-seed and new-seed reset clear visual confirmation, last sound, transient knowledge state, movement pulse timing, prop armed state as applicable, panel active/cooldown state, and new feedback.
- Existing objective, extraction, Owl, F1/F2/F3, fullscreen, and run-reset behavior remains functional.
- Durable facility content is created in the builder/specification and regenerated through Unity MCP; do not hand-author changes only inside the disposable generated root.
- Set the implementation/build identity to `v0.3.0.0` through Unity MCP as part of the completed version.

## Should Have

- A compact operator event line for visual confirmation, sound acceptance/rejection, and target-source changes.
- Player-facing materials or labels that make the three machinery panels recognizable as the same interaction family.
- Same-seed playback of the same perception inputs yields equivalent classifications and target decisions.

## Could Have

- Minor placement or audio-level tuning after the first human session.
- A small panel-ready indicator distinct from its active amber state.

Could Have items may not displace tests, build validation, or human observation.

## Explicitly Out Of Scope

- recoverable capture, nest escape, or capture escalation;
- hiding-pattern, distraction-reliance, or crouch-reliance adaptation;
- flashlight visibility;
- electrical probe, stun, weapons, or killing the Owl;
- explicit lockers, closets, or emergency hide spots;
- inventory, holsters, equipment cycling, or a bespoke noise gadget;
- doors, ventilation, radiation control, movable cover, or staging cart;
- biofeedback, breathing, heartbeat, stress simulation, haptics, or physical effects;
- new mission, map, objective, entity, learned AI, cross-run memory, final art, or final animation;
- XR comfort claims or hardware validation.

## Functional Acceptance Criteria

- Zero, one, and two-or-more visible samples classify correctly.
- Confirmation gains and decays at the configured rate for posture and movement.
- Full cover never supplies a live visual position.
- Standing still emits no movement stimulus.
- Crouch, walk, and sprint produce distinct configured movement bands.
- Below-threshold sound is rejected without a state or target change.
- Accepted sound redirects an unseeing Owl to the sound origin.
- Confirmed current sight cannot be overridden by a distraction.
- Breaking sight and moving quietly causes search at last credible evidence rather than hidden tracking.
- An accepted metal impact emits one stimulus and one matching sound despite bounce noise.
- All three machinery panels activate, signal locally, cool down, and reset.
- Nine portable props remain reachable and reset safely.
- F1 explains every tested perception and target transition; normal HUD remains free of stealth values.
- Route-adaptation-disabled primary mode clears route memory and remains stable.
- Route adaptation enabled still records plausible direct/inferred evidence without changing the perception priority laws.
- Existing Edit Mode and Play Mode suites remain green, and focused new tests cover the rules above.
- A fresh Windows desktop build identifies as `v0.3.0.0`, launches, and produces no project-code exceptions in the smoke test.

## Human Acceptance Criteria

Use `35_v0_3_0_0_human_validation_sheet.md` for at least three uninformed desktop sessions when testers are available.

- At least two of three testers correctly name sprinting or hard impact as a major sound risk.
- At least two of three intentionally use full cover and quiet relocation.
- At least two of three deliberately use either thrown metal or a machinery panel to influence the Owl.
- At least two of three distinguish current pursuit from investigation/search at last evidence.
- At least two of three can explain one success and one failure without seeing F1 telemetry or a stealth meter.
- No tester reports a controlled full-cover trial as obvious wall tracking; any surprise is reconstructable from telemetry.
- At least one tester uses both fixed and portable counterplay and can describe their tradeoff.

Desktop evidence does not validate VR crouch, reach, throwing, comfort, fatigue, audio spatialization in-headset, haptics, or physical safety.

## Failure Conditions

`v0.3.0.0` fails its prototype question if:

- players cannot identify whether sight or sound caused the Owl's response;
- one visible sample causes effectively instant detection regardless of posture or movement;
- sound redirects the Owl away from confirmed sight;
- full cover still appears to leak a hidden live position;
- crouch is the dominant always-on answer;
- fixed panels or thrown props guarantee safety or feel functionally useless;
- normal play requires F1 or a stealth meter;
- the implementation destabilizes the existing contract loop or cannot reset reliably;
- breadth prevents focused tests, a build, or honest evidence collection.

## Two-Day Cut Order

If the implementation window slips, cut in this order:

1. extra panel-ready visual polish;
2. the increase from six to nine portable props;
3. more than one panel silhouette/material treatment;
4. nonessential operator log wording.

Do not cut perception priority, the hearing threshold, full-cover fairness, accepted-event feedback, focused tests, reset, or build validation. If the three fixed panels cannot be completed safely, deliver one fully tested Central panel and record the other two as the first `v0.3.z.w` content increment; do not ship three unreliable copies.

## Definition Of Done

- Every Must Have item is implemented or explicitly classified against the cut order.
- Focused Edit Mode and Play Mode tests pass.
- Existing regression suites pass.
- Runtime behavior is inspected from standing, crouched, leaned, walking, sprinting, and full-cover viewpoints.
- One route-adaptation-enabled integration run passes.
- A fresh `v0.3.0.0` desktop build launches and is smoke-tested.
- Human evidence is recorded, or lack of tester access is stated as `Not Tested` rather than implied success.
- `06_prototype_log.md`, implementation evidence, and outstanding risks are updated.
- No commit or push occurs unless the user separately requests it.
