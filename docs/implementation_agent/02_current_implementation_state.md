# Current Implementation State

Last verified: 2026-08-10

## Repository and Project

| Item | Verified value |
|---|---|
| Repository root | Use the live checkout root; do not rely on a device-specific absolute path |
| Unity project | `UnityProject/InternshipGameSynced` |
| Unity version | `6000.5.4f1` |
| Render pipeline | Universal Render Pipeline |
| Main scene | `Assets/_Project/Scenes/P01_CoreLoop.unity` |
| Current implementation identity | `v0.4.0.0` |
| Current Unity bundle version | `0.4.0.0` |
| Branch at handoff | `master` |
| Repository state at verification | Local `master` matched `origin/master` and contained the v0.4 implementation before the documentation refresh |
| Remote | `https://github.com/Moutaz-Code/internship-game.git` |
| Windows build output | `UnityProject/InternshipGameSynced/Builds/v0.4.0.0/InternshipGame_v0.4.0.0.exe` (generated/ignored; rebuild locally when required) |

The tracked branch contains Prototype 2, the protected `v0.3.0.0` readable-stealth/counterplay baseline, and the complete `v0.4.0.0` 7C facility/topology pass. Historical notes describing v0.3 as dirty/unpublished or v0.4 as pending are stale status snapshots, not current authority.

Always re-run Git status before work. Build output and smoke logs are intentionally ignored and may be absent on a fresh checkout even when the matching source and recorded evidence are present.

## Protected v0.3.0.0 Systems Baseline

The systems baseline adds:

- Concealed/Noticeable/Exposed three-sample visual evidence and configurable confirmation;
- evaluated hearing threshold and explicit knowledge-source priority;
- protected full cover plus quiet relocation;
- crouch/walk/sprint movement noise and graded spatial footsteps;
- nine portable metal props with accepted-impact feedback;
- three cooldown-limited MAINTENANCE TEST panels;
- restrained Owl investigation feedback and expanded F1 telemetry;
- adaptation-disabled primary comparison and adaptation-enabled compatibility behavior.

Recorded evidence:

- focused Edit Mode 22/22 passed;
- full Edit Mode 133/133 passed;
- integrated Play Mode 6/6 passed;
- strict Windows build succeeded with zero errors and two rendering warnings;
- the exact executable completed a deliberate 12-second smoke window without a project exception;
- uninformed human evidence remains Not Tested.

These behaviors are protected regressions for v0.4. See `../06_prototype_log.md` and `../33_v0_3_0_0_readable_stealth_scope.md`.

## Implemented v0.4.0.0 Facility

The live scene uses the locked three-lobed 7C circuit from `../37_v0_4_0_0_spatial_readability_scope.md` and `../38_v0_4_0_0_7c_level_plan.md`. Durable generation is owned by `Assets/_Project/Scripts/Editor/FacilityGeneration/V040FacilityBuilder.cs` and orchestrated by `FacilityGeneration/PrototypeSceneBuilder.cs`. The superseded pre-7C builder was removed on 2026-08-11 and remains available in Git history.

## Current Playable Contract

The implemented desktop-first slice is a 15–20 minute extraction-horror run:

1. Enter through the Contractor Annex into an asymmetric three-lobed nuclear facility.
2. Search Containment & Control, Coolant & Maintenance, and Fuel Handling & Waste through North Pipe Gallery, Utility Spine, and South Maintenance Channel.
3. Find rod candidates selected from eight authored sockets.
4. Carry one rod at a time and hold it for a three-second scan.
5. Use Geiger guidance to locate candidates.
6. Distinguish three live rods from two inert decoys.
7. Deposit only scan-confirmed live rods in a visible three-slot intake rack.
8. Survive the 20-second extraction cycle.
9. Cross the opened exit.
10. Avoid or misdirect one Owl that patrols, investigates, hunts, searches, disengages, and applies bounded run-local route adaptation.

## Desktop Controls

- `WASD`: move.
- Mouse: look.
- `Shift`: sprint.
- `Space`: jump.
- Hold `Ctrl` or `C`: crouch.
- Hold `Z` or `X`: lean left or right.
- `E`: grab or safely drop.
- Left click: use a focused interactable; while holding a throwable object, perform the deliberate throw behavior.
- `R`: restart the current seed.
- `Shift+R`: generate a new seed.
- `F1`: operator/debug overlay.
- `F2`: bounded Owl/Geiger-interference comparison.
- `F3`: route-adaptation A/B toggle.
- `F11`: borderless fullscreen/windowed toggle.
- `Esc`: pause/resume and release/capture the cursor.

The build starts borderless fullscreen and retains a resizable 1280×720 windowed fallback.

## Facility and Run Variation

- Approximate footprint: 57 m × 40 m around an inaccessible shielding/service void.
- Playable macro areas: Contractor Annex, Containment & Control, Coolant & Maintenance, and Fuel Handling & Waste.
- Pairwise connectors: North Pipe Gallery, Utility Spine, and South Maintenance Channel.
- Seven authored portals P01-P07 use 2.76 m clear width and 2.52 m clear height.
- Eight sockets are lobe-owned: C1-C3, K1-K3, and F1-F2.
- Every seed selects exactly one live rod per lobe plus two inert decoys from unused sockets.
- Open Circuit, Coolant Service, and Fuel Inspection are the supported deterministic route profiles.
- A 20-node route graph preserves every socket and two graph-distinct approaches from the Containment side of P01 under every profile.
- Same-seed restart reproduces the authored configuration; new-seed restart changes eligible authored state without procedurally generating geometry.

## Objective and Feedback Behavior

- Exactly three live rods and two inert rods are active.
- A rod must remain held for three seconds to complete its scan; an incomplete scan resets on release.
- Live confirmation uses Heat semantics. Desktop simulation presents a conservative warmth/amber response.
- Inert confirmation does not use Heat and has a distinct cool/audio response.
- The Geiger guides toward the nearest live rod.
- Optional Owl interference is bounded and cannot alter classification or fake Heat.
- The intake rack reports typed results for accepted, inert, unscanned, duplicate, or unregistered rods.
- Accepted rods force release, snap to a slot, become immutable, and cannot count twice.
- Rejected inert rods eject and create bounded noise/pressure.
- Heat means live-rod confirmation only.
- controller/floor vibration means Owl proximity only.
- Wind means extraction cycling only.

## Player and Interaction Behavior

- `IPlayerContext` is the player-controller replacement seam.
- `IPlayerMotionSource` exposes motion telemetry without binding consumers to the desktop controller.
- Desktop and XR modes are selected through `PlayerModeController`.
- Desktop carrying caches and restores complete Rigidbody, collider, parent, interpolation, and XR-interactable state.
- Held objects use the authored grip pose and exact late-frame following.
- Desktop movement uses acceleration, deceleration, limited air control, coyote time, jump buffering, landing telemetry, head bob, strafe roll, landing dip, and sprint FOV.
- `CharacterController.minMoveDistance` is forced to zero to avoid the high-refresh-rate WASD regression.
- Crouch is hold-based and prevents standing into a ceiling.
- Lean includes position and roll and is collision-limited.
- Pickup uses a 3.6 m reach and a 0.26 m aim-assist query. Direct hits win; assisted candidates are ranked and require their own clear line of sight, preventing through-wall grabs.

## Owl Behavior

- Navigation is behind `IThreatNavigator`.
- `NavMeshThreatNavigator` is primary; an authored waypoint graph remains a fallback.
- Eighteen authored Owl nodes cover the Annex, three lobes, three connectors, and two search probes; only the six connector nodes carry route-family metadata.
- Route-choice evidence comes from authored North/Utility/South connector zones and nodes, not world-X inference. Legacy West/Central/East values remain narrow internal compatibility IDs only.
- Route blockers carve/replan rather than allowing wall crossing.
- Perception uses head, upper-body, and lower-body samples at a bounded interval.
- Current documented values: 22 m main sight range, 120° main view, 170° near view only inside 2.5 m, 80 ms sampling.
- Warning: 1.25 seconds.
- Lost-sight behavior follows the last credible observation rather than the player’s hidden live position.
- Hunt speed: 3.85 m/s, above walk but below sprint.
- Search: 10 seconds with deterministic local probes.
- Credible stimuli include sprint, landing, dropped metal, rod rejection, and objective progress.
- Route knowledge is run-local and plausibly acquired.
- Direct route evidence is stronger than inferred evidence.
- Silent, unperceived zone entry does not become Owl knowledge.
- Adaptation states are Uncertain, Suspicious, Adapted, and Counter-Adapted.
- Route variety weakens the old prediction.
- Adaptation caps biased patrol weight at 1.75 and prevents more than two consecutive biased selections.
- F3 disables adaptation and clears its memory for a baseline comparison.
- No player-facing “the Owl adapted” notification exists.

## UI and Presentation

- Player-facing language uses “scan”; “assay” remains an internal term.
- The primary HUD shows live rods secured, held-rod scan state, next useful action, correction feedback, extraction state, and a timed controls strip.
- The operator overlay is separate from player guidance.
- Mission/rack signage is mounted at scale one and designed for player-distance readability.
- Room, route, portal, socket, shortcut, and closure labels use bounded layouts.
- In-world route color meanings remain cyan extraction, amber maintenance, green radiation/objective areas, and red closure/danger.
- Presentation remains intentionally greyboxed and replaceable.

## Recorded Automated Evidence at `v0.4.0.0`

Recorded in `../06_prototype_log.md`:

- Focused V040 Edit Mode: 28/28 passed.
- Full Edit Mode regression: 141/141 passed.
- Full Play Mode regression: 7/7 passed.
- Coverage includes topology/profile connectivity, graph-distinct socket routes, connector-only evidence, deterministic one-live-per-lobe distribution, portal dimensions, lobe grammar, authored Owl metadata/navigation, telemetry, objective/extraction/reset, and protected v0.3 perception/counterplay behavior.
- Strict clean `StandaloneWindows64` development build succeeded with zero errors and two rendering warnings.
- The exact player remained alive for a deliberate 12-second smoke window and logged no project exception or startup failure before it was closed.

These results describe the recorded v0.4 source/build, not every later checkout or machine. Re-run tests and produce a fresh ignored build artifact when required. Uninformed desktop gameplay, XR, and physical-hardware validation remain open.
