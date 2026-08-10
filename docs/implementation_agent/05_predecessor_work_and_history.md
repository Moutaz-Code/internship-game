# Predecessor Responsibilities and Implementation History

Last verified: 2026-07-28

## Responsibilities Held Before Transfer

The predecessor acted as the project’s implementation owner and was responsible for:

- auditing the existing Unity project and design documents before each pass;
- translating user-approved prototype goals into staged technical work;
- preserving a desktop-first path while maintaining replaceable XR and hardware seams;
- creating and editing C# runtime, editor, and test code;
- operating Unity scene state, generated geometry, Play Mode, Test Runner, screenshots, and builds through the connected Unity MCP workflow;
- validating compilation, console health, Edit Mode, Play Mode, Windows builds, and launch smoke behavior;
- maintaining deterministic run reset and seeded variation;
- guarding architecture boundaries and avoiding needless coupling;
- keeping player, objective, map, threat navigation, threat knowledge, feedback devices, and biometric stubs isolated;
- recording implementation evidence in the prototype log and durable choices in the decision log;
- maintaining version identity and release notes;
- initializing and publishing the GitHub repository when the user requested it;
- committing and pushing requested prototype milestones;
- producing supervisor-facing screenshots and gameplay-video artifacts where practical;
- diagnosing build-only regressions separately from editor behavior;
- reporting remaining human, headset, and hardware validation honestly.

The replacement inherits these implementation responsibilities, but not authority to invent design.

## Prototype 1: Multi-Path Fuel-Rod Slice

The original compact linear prototype was expanded into the current core proof:

- ten authored facility spaces over West, Central, and East route families;
- roughly 36 m × 55 m footprint;
- eight authored rod sockets;
- three live rods and two inert decoys;
- one-at-a-time physical carry;
- three-second held scan;
- Heat-confirmed live truth and distinct inert response;
- Geiger discovery guidance with optional bounded Owl interference;
- visible three-slot deposit rack with typed rejection/acceptance behavior;
- 20-second extraction holdout and final exit crossing;
- optional jump shortcut with no required jumping;
- deterministic same-seed/new-seed authored variation;
- one Owl with patrol, investigation, hunt, lost-sight search, disengage, stimuli, and route navigation;
- desktop, XR, haptic/environmental hardware, and biometric seams.

The monolithic scene setup was replaced with data-driven generated rooms, corridors, portals, landmarks, and typed anchors. The generated facility root is disposable; manual work outside it is preserved.

## Prototype 2.0: Readable Route Adaptation

The implementation added a testable run-local evidence model:

- West, Central, and East route confidence;
- Direct evidence weight `1.0`;
- Inferred evidence weight `0.5`;
- Suspicious threshold `1.0`;
- Adapted threshold `2.0`;
- inferred evidence rate limiting;
- Uncertain, Suspicious, Adapted, and Counter-Adapted states;
- confidence decay and route-variety weakening;
- suspicious inspections, capped adapted patrol/search bias, forced variety, and counter-adaptation openings;
- 1.75 maximum patrol bias;
- 60-second memory/decay comparison window;
- no more than two consecutive biased patrol selections;
- F1 evidence/transition telemetry;
- F3 baseline comparison toggle that resets adaptation memory;
- no player-facing adaptation notification.

The key fairness correction was that an unperceived zone entry does not automatically become Owl knowledge. Direct observation may carry a credible observed position; inferred evidence identifies a wing only and cannot reveal exact hidden location.

## Prototype 2.1: Desktop and Objective Polish

The pass improved support systems around the adaptation experiment:

- complete carry/drop Rigidbody, collider, parent, interpolation, and XR-interactable restoration;
- deliberate left-click throw after state restoration;
- loose metal props as distraction stimuli;
- actual rack collider-penetration acceptance;
- deterministic inert ejection and typed rack result transitions;
- player language changed from “assay” to “scan”;
- clearer mission progress, next-action, rack correction, extraction, and control messaging;
- acceleration/deceleration movement, air momentum, view bob/roll, landing dip, and sprint FOV;
- standalone NavMesh-agent startup fix;
- desktop OpenXR startup/build sanitation while preserving headset configuration.

This pass retained the controller-swap seam and did not copy desktop camera motion into XR.

## Prototype 2.2: Greybox Environment and Presentation

The environment remained scrappable but became less symmetric and more presentable:

- room-specific profiles for all ten spaces;
- asymmetric peripheral and overhead structures;
- preserved gameplay cover footprints and topology;
- fitted portal frames and consistent openings;
- raised route inlays replacing overlapping coplanar plates;
- reduced Z-fighting risk;
- corrected Hub wall/portal alignment;
- scale-one mission and rack assemblies;
- readable room, portal, socket, shortcut, and closure signs;
- distinct route and room identity without final-art commitment.

Full Play Mode validation was expanded to check room profiles, portals, surface separation, UI scale, and removal of legacy full-room plates.

## v0.2.2.1: Fullscreen and Versioning

- Added borderless fullscreen/windowed switching on `F11`.
- Preserved a resizable 1280×720 windowed fallback.
- Added the four-part early-development version scheme `v{x}.{y}.{z}.{w}`.
- Synchronized runtime/build identity and documentation.

## v0.2.2.2: High-Refresh Movement Regression

A standalone-build WASD failure was isolated to `CharacterController.minMoveDistance`: at 144 Hz, per-frame displacement could remain below the controller threshold, so movement input was accepted but discarded. The fix explicitly sets the threshold to zero, preserving high-refresh gameplay rather than capping frame rate.

This is an important diagnostic precedent: when editor and build input differ, inspect downstream motion thresholds and frame-rate-sensitive integration rather than assuming the Input System is at fault.

## v0.2.3.0: Embodiment, Portal Accuracy, Pickup, and Owl Capability

The last completed implementation pass added:

- wall strips generated from the same dimensions and centers as their portal openings;
- verified clearance for all thirteen portal/interior-wall combinations;
- hold crouch on `Ctrl`/`C` with smooth posture and ceiling-safe standing;
- collision-limited positional and roll lean on `Z`/`X`;
- 3.6 m pickup reach and 0.26 m occlusion-safe reticle assistance;
- stronger Owl perception using three body samples;
- 22 m/120° primary vision and bounded 2.5 m near awareness;
- bounded lead from observed movement;
- last-credible-position pursuit after sight loss;
- deterministic local search probes;
- adjusted warning, grace, search, and chase behavior while keeping sprint escape possible;
- updated HUD/pause controls and `v0.2.3.0` identity.

Recorded validation was 111/111 Edit Mode, 6/6 integrated Play Mode, a clean Windows build, and a 20-second launch smoke window.

## Architecture Work Established

The predecessor intentionally created or preserved:

- `IPlayerContext` and `IPlayerMotionSource` for controller replacement;
- aggregate `IRunObjective` and typed progress/deposit results;
- `PrototypeMapDefinition` typed authored anchors;
- immutable deterministic `RunConfiguration`;
- `IThreatNavigator`, `IThreatRouteProfileReceiver`, and `IThreatStimulusSink`;
- testable route-evidence and threat-state models outside the navigation adapter;
- `FeedbackHub` and typed output adapters for visual, audio, controller, floor, Heat, and Wind channels;
- device health/fault isolation;
- biometric sensor abstractions that remain inactive stubs;
- Edit Mode logic coverage and Play Mode scene/integration gates;
- desktop-build XR sanitation without deleting future XR configuration.

These seams exist for known replacement needs. They are not permission to build a generic framework.

## Delivery and Repository Work

- The Git repository and GitHub remote were established at `Moutaz-Code/internship-game`.
- Prototype 1, Prototype 2.0, environment/fullscreen increments, and `v0.2.3.0` were committed and pushed when requested.
- At the 2026-08-02 predecessor handoff, published `master` contained commit `0636f5d` (`v0.2.3.0`); later repository history supersedes that dated snapshot.
- Supervisor-facing captures and video artifacts were prepared in the repository workspace.
- Build output directories are versioned locally and normally excluded according to repository rules.

Always verify the current remote and history. This history is continuity context, not a request to perform another release.
