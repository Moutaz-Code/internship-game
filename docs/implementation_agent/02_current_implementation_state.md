# Current Implementation State

Last verified: 2026-08-02

## Repository and Project

| Item | Verified value |
|---|---|
| Repository root | `C:\Users\Motaz\Documents\Codex\Internship Game` |
| Unity project | `UnityProject/InternshipGameSynced` |
| Unity version | `6000.5.4f1` |
| Render pipeline | Universal Render Pipeline |
| Main scene | `Assets/_Project/Scenes/P01_CoreLoop.unity` |
| Current local build identity | `v0.3.0.0` |
| Current local Unity bundle version | `0.3.0.0` |
| Branch at handoff | `master` |
| Published commit at handoff | `0636f5d0664b05b317d6504974bc82427be1d91b` |
| Remote | `https://github.com/Moutaz-Code/internship-game.git` |
| Current local Windows build path | `UnityProject/InternshipGameSynced/Builds/v0.3.0.0/InternshipGame_v0.3.0.0.exe` |

The current `HEAD` contains the tracked `v0.2.3.0` implementation. Older July 26 notes saying v0.2.3.0 was uncommitted are stale. The newer local `v0.3.0.0` implementation is present in the user-owned dirty working tree and was not published at the 2026-08-02 audit.

The working tree was not clean at handoff. It included the local v0.3 scene, builders, threat/player/feedback code, tests, settings, evidence, and documents, plus other modified/untracked project files. The exact list must be obtained again with Git before work. Treat all of it as user-owned and do not assign ownership by inference.

## Local v0.3.0.0 Additions

The local systems baseline adds:

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

## Authorized v0.4.0.0 Replacement Map

The current generated ten-space West/Central/East facility remains the live scene until v0.4 implementation regenerates it. The authorized replacement is the locked three-lobed 7C circuit in `../37_v0_4_0_0_spatial_readability_scope.md` and `../38_v0_4_0_0_7c_level_plan.md`.

Do not implement from the older “Facility and Run Variation” description below. It documents the current pre-v0.4 runtime for audit and regression purposes only.

## Current Playable Contract

The implemented desktop-first slice is a 15–20 minute extraction-horror run:

1. Enter a ten-space authored nuclear facility.
2. Search West, Central, and East route families.
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

- Approximate footprint: 36 m × 55 m.
- Authored spaces: Entry/Extraction, Decontamination, Operations Hub, West Maintenance, Central Transfer, East Laboratory, North Cross Hall, West Rod Storage, Core Vault, East Reactor.
- Route families: West, Central, East, with cross-links.
- Thirteen fitted portal openings use one shared generated source of truth.
- Every terminal destination has a required non-jump path; jump is optional.
- The Maintenance shortcut has a non-jump alternative.
- Room-specific peripheral and overhead greybox profiles reduce symmetry without committing to final art.
- Same-seed restart reproduces authored configuration.
- New-seed restart selects rod sockets, one route profile, lighting/audio profile, and Owl profile without procedurally generating geometry.

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

## Automated Evidence at `v0.2.3.0`

Recorded in `../06_prototype_log.md`:

- Edit Mode: 111/111 passed.
- Integrated Play Mode: 6/6 passed.
- Play Mode validation covered all thirteen portals, seeded player/Owl connectivity, new pickup/Owl values, objective/extraction/reset determinism, and multi-sample Owl sight around partial versus full cover.
- Strict clean `StandaloneWindows64` development build succeeded in 136.20 seconds at 187,683,002 bytes with zero reported build errors or warnings.
- The exact player remained alive for a 20-second smoke window and was then deliberately closed.
- No project exception, missing script, OpenXR runtime failure, serialization failure, or invalid NavMesh-agent error was reported in that smoke log.

These results describe the recorded build, not the current working tree. Re-run tests appropriate to any later change.
