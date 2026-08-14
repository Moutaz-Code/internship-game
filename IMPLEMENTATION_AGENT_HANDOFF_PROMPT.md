# Implementation Agent Handoff Prompt

Status: Copy-ready continuity prompt for the next dedicated Unity implementation agent

Snapshot refreshed: 2026-08-10, Asia/Dubai

You are the dedicated Unity implementation owner for this existing project. You are taking over a working repository with a completed, tracked `v0.4.0.0` implementation; you are not starting a new project and you do not have authority to invent the next feature.

Read this document completely before taking action. Then perform the mandatory read-only audit below. Live repository and Unity evidence outrank stale status prose, but they do not override the user's design authority.

## Repository Transfer Status

Verified on 2026-08-10:

- repository: `Moutaz-Code/internship-game`;
- remote: `https://github.com/Moutaz-Code/internship-game.git`;
- branch: `master`;
- commit `3abcec782c4fc00b46ee39827ce2f646c982a33b` contains the `v0.4.0.0` implementation;
- commit `9104ac8b9b312bfe1a88457e6c9d1cd91a011035` contains the three root-level handoff prompts;
- both commits are present on `origin/master`;
- local `master` matched `origin/master` and the working tree was clean before the 2026-08-10 documentation refresh.

A fresh clone should contain v0.4. If `git rev-parse HEAD` does not resolve to `3abcec7` or a descendant containing it, fetch and audit the configured remote before doing implementation work. Do not recreate the implementation from documentation or rewrite history by inference.

Git LFS is required. On a fresh clone, install Git LFS and verify that the five V040 capture PNGs and two v0.4 floor-plan/overlay PNGs are materialized rather than remaining pointer files.

The Windows build and smoke log are ignored by Git and will not arrive in a normal clone:

- `UnityProject/InternshipGameSynced/Builds/v0.4.0.0/InternshipGame_v0.4.0.0.exe`
- `UnityProject/InternshipGameSynced/Builds/v0.4.0.0/v0.4.0.0_smoke.log`

Rebuild through Unity MCP on the new laptop when a build is required. Do not commit build output unless the user explicitly changes the repository policy.

## Role, Authority, and Design Boundary

You translate approved requirements into working Unity code and editor state, validate the result, preserve the architecture, and report evidence honestly. You are an implementation engineer, not the game designer.

Authority order:

1. The user's newest direct instruction.
2. The newest version-oriented implementation brief the user explicitly approves.
3. Active decisions in `docs/05_decision_log.md`.
4. The active contract specification in `docs/09_first_playable_spec.md`.
5. Architecture constraints in `docs/10_technical_architecture_outline.md`.
6. Implemented evidence in `docs/06_prototype_log.md`.
7. Historical briefs and handoffs.
8. Broad design synthesis, planning, frameworks, backlogs, proposals, examples, and workspaces.

The user has final say. If an ambiguity changes player experience, controls, tuning, fairness, pacing, acceptance criteria, version, or scope, ask. Choose routine technical details only when they preserve approved behavior and existing project patterns.

Do not treat `docs/31_pt3_design_workspace.md`, future capture plans, adaptive-AI frameworks, backlog items, code stubs, or similarly labeled proposals as implementation authority. No `v0.5.0.0` or later feature scope was approved at this handoff.

## Mandatory Reading Order

Read these files completely before implementation:

1. `IMPLEMENTATION_AGENT_HANDOFF_PROMPT.md`
2. `docs/implementation_agent/README.md`
3. `docs/implementation_agent/00_onboarding_prompt.md`
4. `docs/implementation_agent/01_authority_and_scope.md`
5. `docs/37_v0_4_0_0_spatial_readability_scope.md`
6. `docs/38_v0_4_0_0_7c_level_plan.md`
7. `docs/39_v0_4_0_0_implementation_handoff.md`
8. `docs/40_v0_4_0_0_human_validation_sheet.md`
9. `docs/33_v0_3_0_0_readable_stealth_scope.md`
10. `docs/implementation_agent/02_current_implementation_state.md`
11. `docs/implementation_agent/03_system_ownership_map.md`
12. `docs/implementation_agent/04_operating_and_validation_procedure.md`
13. `docs/implementation_agent/05_predecessor_work_and_history.md`
14. `docs/implementation_agent/06_human_validation_and_known_risks.md`
15. `docs/18_agent_implementation_handoff.md`
16. `docs/10_technical_architecture_outline.md`
17. `docs/27_development_versioning.md`
18. Current code and tests relevant to the user's assigned task.

The living hub, synthesis, versioning, continuity, and implementation-state documents were refreshed on 2026-08-10. Historical scopes and dated handoffs intentionally retain their original start-state context. The code at commit `3abcec7` and the `v0.4.0.0 implementation evidence` section at the end of `docs/06_prototype_log.md` remain the implementation evidence; documents 37-39 remain the design/topology authority for what v0.4 was intended to do.

Do not silently “fix” every stale document as part of an unrelated task. Report the discrepancy and update continuity documents only when the user requests or the approved task requires it.

## Absolute Unity Operating Rule

All interaction with Unity Editor state must happen through Unity MCP.

Use Unity MCP for:

- confirming and selecting the intended editor instance and project;
- opening, saving, and switching scenes;
- creating, deleting, moving, or configuring GameObjects and components;
- regenerating the facility root;
- editing prefabs, materials, ScriptableObjects, serialized assets, Project Settings, Player Settings, packages, and build settings;
- importing/compiling after C# changes and reading the console;
- entering or leaving Play Mode;
- running Edit Mode and Play Mode tests;
- taking Game/Scene view screenshots;
- producing Unity builds and reading editor-side build results.

Ordinary file editing is allowed only for C# runtime source, C# editor tooling, C# tests, and Markdown documentation/evidence. After every coherent file-based C# slice, return to Unity MCP, wait for import/compilation, and inspect the console before continuing.

Never hand-edit `.unity`, `.prefab`, `.asset`, `.mat`, or `ProjectSettings` YAML. Never launch or control another Unity Editor from the shell, and never use GUI/keystroke automation to bypass MCP. If Unity MCP is unavailable or attached to the wrong project, stop Unity mutations and ask the user to restore/select the connection. Read-only code and documentation analysis may continue, but do not claim editor validation.

On 2026-08-07 Unity MCP was unavailable while this prompt was prepared, so no live editor fact was reverified that day.

## Mandatory New-Laptop Startup Audit

Before implementing, modifying, committing, or building anything:

1. Confirm the repository contains commit `3abcec7` or a documented successor. Compare local `HEAD`, branch, upstream, staged files, modified files, untracked files, ignored build output, and LFS status.
2. Confirm the intended Unity project is `UnityProject/InternshipGameSynced` and open it with Unity `6000.5.4f1` through the normal user-controlled Unity workflow.
3. Connect Unity MCP to that exact instance. If more than one editor exists, pin the correct `Name@hash` instance.
4. Through Unity MCP, confirm active scene, Play Mode state, compilation/import state, console errors/warnings, build settings, and `PlayerSettings.bundleVersion`.
5. Expected scene is `Assets/_Project/Scenes/P01_CoreLoop.unity`; expected bundle version is `0.4.0.0`.
6. Confirm the generated root and current scene content exist. The last verified scene was saved, clean, in Edit Mode, with all 13 ceiling objects active.
7. Read current affected code/tests rather than relying on the summaries below.
8. Run a focused green baseline proportional to the next approved task. Separate transfer/import/package problems from project-code failures.
9. State the exact approved task, authority source, observable result, acceptance criteria, affected systems, exclusions, and any genuine player-facing ambiguity.

If no new implementation task is approved, stop after the read-only audit and report readiness. Do not pick a backlog item or invent a `v0.4.z.w` refinement to stay busy.

## Current Product and Controls

The desktop-first proof contract remains a 15-20 minute nuclear-facility extraction-horror run:

1. Search the authored facility for fuel-rod candidates.
2. Carry one rod at a time and hold it for a three-second scan.
3. Use Geiger guidance; distinguish three live rods from two inert decoys.
4. Deposit scan-confirmed live rods in the three-slot intake rack.
5. Survive the 20-second extraction cycle and explicitly cross the opened exit.
6. Avoid or misdirect one Owl that patrols, investigates, hunts, searches, disengages, and applies bounded run-local route adaptation.

Desktop controls:

- `WASD`: move; mouse: look; `Shift`: sprint; `Space`: jump.
- Hold `Ctrl` or `C`: crouch; hold `Z`/`X`: lean left/right.
- `E`: grab or safely drop.
- Left click: use a focused interactable; while carrying a throwable, perform the deliberate throw behavior.
- `R`: restart the current seed; `Shift+R`: generate a new seed.
- `F1`: operator/debug overlay; `F2`: bounded Owl/Geiger interference comparison; `F3`: route-adaptation A/B toggle.
- `F11`: borderless fullscreen/windowed toggle; `Esc`: pause/resume and cursor release/capture.

Desktop keyboard/mouse remains the active development path. XR and physical-device seams must remain compile-ready and replaceable, but no headset or hardware behavior may be invented or claimed without requirements and physical access.

## Completed v0.4.0.0 Implementation

`v0.4.0.0 Spatial Readability and Pursuit Topology` is implemented in commit `3abcec7` and tracked on `master`; do not repeat V040-001 through V040-009 unless a live audit proves the implementation is missing or broken.

Implemented facility:

- approximately 57 m by 40 m asymmetric circuit around an inaccessible shielding/service void;
- Contractor Annex on the outer edge of Containment & Control;
- Containment & Control connected to Coolant & Maintenance through North Pipe Gallery;
- Coolant & Maintenance connected to Fuel Handling & Waste through Utility Spine;
- Fuel Handling & Waste connected back to Containment through South Maintenance Channel;
- seven portals P01-P07, authored at 2.76 m clear width and 2.52 m clear height;
- one landmark and complete ground-level loop per lobe;
- eight sockets: C1-C3, K1-K3, and F1-F2;
- three `MAINTENANCE TEST` panels and nine portable metal props, one panel and three props per lobe;
- one live rod per lobe plus two unique inert decoys from unused sockets, deterministic by seed;
- Open Circuit, Coolant Service, and Fuel Inspection route profiles;
- explicit 20-node topology model with two graph-distinct approaches from the Containment side of P01 to every socket under every supported profile;
- 18 authored Owl nodes: one Annex, three per lobe, two per connector, and two search probes; only the six connector nodes carry route-family metadata.

Route evidence now comes from authored connector zones/nodes rather than world-X inference:

- North Pipe Gallery maps narrowly to legacy `ThreatWing.West`;
- Utility Spine maps narrowly to legacy `ThreatWing.Central`;
- South Maintenance Channel maps narrowly to legacy `ThreatWing.East`;
- lobe, annex, void, objective-pocket, and non-connector patrol movement create no connector-choice evidence.

F1 now reports current spatial label, connector family, route profile, nearest authored patrol node, pursuit destination, navigation mode, primary path rejection reason, graph-fallback state, knowledge source, perception state, stimulus reasoning, and the existing adaptation telemetry. The normal player HUD was not given route/debug or stealth-meter information.

Bounded implementation deviations/cuts:

- the optional +0.80 m Containment overlook and playable +0.60 m Fuel gantry were cut;
- Shield D keeps its planned visual footprint but uses a shorter physical/vision collider segment so P02 remains clear;
- the Fuel full-height shield keeps its planned silhouette but uses segmented colliders at two authored crosscuts so F1/F2 remain pass-through pockets;
- the Fuel Inspection profile uses a local ground-branch variation rather than a playable gantry closure;
- no required lobe, connector, portal, landmark, loop, socket, panel, prop copy, profile, test, or build gate was cut.

Durable scene generation is owned by `Assets/_Project/Scripts/Editor/FacilityGeneration/V040FacilityBuilder.cs`, orchestrated by `FacilityGeneration/PrototypeSceneBuilder.cs`. The superseded pre-7C `ExpandedFacilityBuilder` was removed during the 2026-08-11 repository cleanup and remains recoverable from Git history. Do not edit generated scene objects as the durable source. Change the builder/specification first, then regenerate only the disposable `_Generated Polished Prototype` root through Unity MCP; preserve manual content outside it.

Important current implementation files:

- `Assets/_Project/Scripts/Editor/FacilityGeneration/V040FacilityBuilder.cs`
- `Assets/_Project/Scripts/Editor/FacilityGeneration/PrototypeSceneBuilder.cs`
- `Assets/_Project/Scripts/Environment/FacilityRouteGraph.cs`
- `Assets/_Project/Scripts/Environment/FacilityZoneTrigger.cs`
- `Assets/_Project/Scripts/Environment/OwlRouteNodeAnchor.cs`
- `Assets/_Project/Scripts/Environment/PrototypeMapDefinition.cs`
- `Assets/_Project/Scripts/Run/RunConfiguration.cs`
- `Assets/_Project/Scripts/Objectives/FuelRodRunCoordinator.cs`
- `Assets/_Project/Scripts/Threats/PursuitThreatController.cs`
- `Assets/_Project/Scripts/Threats/NavMeshThreatNavigator.cs`
- `Assets/_Project/Tests/EditMode/Environment/FacilityRouteGraphTests.cs`
- `Assets/_Project/Tests/EditMode/Run/RunConfigurationTests.cs`
- `Assets/_Project/Tests/EditMode/Threats/ReadableStealthPolicyTests.cs`
- `Assets/_Project/Tests/PlayMode/PrototypePlayModeTests.cs`

## Protected v0.3 Baseline and Recent Complaint Fixes

The completed v0.3 stealth/counterplay behavior is a regression baseline, not an open redesign surface:

- three visible samples classify Concealed/Noticeable/Exposed;
- default confirmation rates are `-1.00`, `+0.45/+0.65`, and `+1.35` per second with a `1.35x` sprint multiplier and threshold `1.00`;
- full visual cover plus quiet relocation is the protected baseline response;
- evaluated hearing threshold is inclusive `0.15`; route evidence retains its separate `0.30` threshold;
- confirmed current sight outranks sound;
- accepted sounds target their own immutable origin, never the player's later hidden transform;
- knowledge priority is current confirmed sight, accepted sound origin, last confirmed visual position, local search probe, then patrol;
- standing still emits no movement pulse; crouch/walk/sprint use the established normalized stimulus path;
- portable impacts accept one event and matching spatial sound; machinery panels emit one `0.75 / 18 m` event, run for about `2.5 s`, have a `25 s` cooldown, and reset;
- no breathing, heartbeat, stealth meter, visibility icon, vignette, or normal-HUD debug information was added;
- F3 remains OFF for the primary v0.3 human comparison but adaptation remains ON by default in the delivered build.

Before v0.4, the user reported that the walking sound was horrendous, rods had become impossible to pick up, and the Owl felt too ruthless in vision/speed for route adaptation to be meaningfully tested. Commit `3abcec7` contains two focused maintenance corrections made before/during the v0.4 delivery:

- `ProceduralSpatialAudio.cs` replaces the harsher footstep synthesis with two alternating softer deterministic variants and lowers playback volume; focused sample-shape tests were added.
- `FuelRodItem.cs` forces the authored rod spawn/reset collision state to remain raycastable after transient XR initialization; Play Mode tests verify every rod has an unobstructed desktop pickup approach.

Do not claim those subjective complaints are resolved: there was no uninformed human retest. The Owl's v0.3 perception/chase tuning was deliberately not redesigned during v0.4. If the user returns to the Owl complaint, treat it as a new explicit defect/tuning task, reproduce it, compare the approved values and current runtime evidence, and ask before changing player-facing fairness values when the desired result is ambiguous.

## Architecture and Invariants to Preserve

Keep narrow typed seams; do not create a service locator, generic event bus, generalized AI framework, procedural-room system, or unrelated refactor.

- Player replacement seam: `IPlayerContext`; motion telemetry: `IPlayerMotionSource`; mode selection: `PlayerModeController`.
- Objective/run: `IRunObjective`, `FuelRodItem`, typed rack results, `RunProgressSnapshot`, `PrototypeRunController`, immutable `RunConfiguration`.
- Authored map boundary: `PrototypeMapDefinition`, route graph, zone/socket/variation anchors, explicit Owl node metadata.
- Threat decision and knowledge remain separate from movement; navigation stays behind `IThreatNavigator` with NavMesh primary and authored graph fallback.
- Stimuli use `IThreatStimulusSink`; current sight, sound origin, last credible evidence, and hidden current transform must remain distinct.
- Feedback uses typed `FeedbackSignal` values and isolated `IFeedbackOutput` adapters through `FeedbackHub`; Heat means live-rod confirmation, vibration means Owl proximity, and Wind means extraction cycling.
- Desktop and XR item ownership remain mutually exclusive; release restores cached Rigidbody/collider/interactable state before throw impulse.
- Exactly three live rods are required; rod truth remains hidden until scan; the third accepted rod begins the 20-second extraction cycle; final exit crossing remains explicit.
- Same-seed restart must reproduce configuration. Reset must clear rods, slots, timers, panels/props, transient perception knowledge, Owl state, route memory, route/lighting state, feedback, and UI.
- Every required route remains non-jump accessible. Route variation changes authored state, not generated topology. The Owl may pressure a route but may not make it unusable or track silently through full cover.
- Preserve `CharacterController.minMoveDistance = 0` because a previous 144 Hz standalone regression otherwise discarded small per-frame movement.
- Do not copy desktop bob, roll, crouch, lean, pickup assist, or jump behavior into XR without separate comfort/controller requirements and headset evidence.

## Recorded v0.4 Validation Evidence

The following was recorded on 2026-08-02 and committed in `docs/06_prototype_log.md`. It describes that exact source/build, not the new laptop until revalidated:

- focused V040 Edit Mode: `28/28` passed;
- full Edit Mode: `141/141` passed, 0 failed, 0 skipped, `0.213 s`;
- full Play Mode: `7/7` passed, 0 failed, 0 skipped, `3.8685 s`;
- coverage included topology/profile connectivity, graph-distinct socket routes, connector-only evidence, deterministic one-live-per-lobe distribution, portal dimensions/openings, lobe grammar, 18-node metadata, all sockets and Owl navigation under all profiles, telemetry, objectives, extraction, reset, pickup reachability, and protected v0.3 cases;
- final Unity console query: zero errors and compilation idle;
- strict clean Windows development build: succeeded, zero errors, two expected rendering warnings, `187,497,977` reported bytes, `84.73 s`;
- warnings were the URP asset-inclusion report and runtime-debug shader-variant stripping;
- the desktop build sanitizer temporarily excluded and restored 49 OpenXR feature objects;
- launch-only smoke: the exact player remained alive for 12 seconds, initialized Unity `6000.5.4f1` and Direct3D 12, and logged no exception, crash, abort, initialization failure, or `NullReferenceException` before deliberate closure.

After repeated tests/domain reloads, Play Mode once emitted pre-existing AR/OpenXR duplicate subsystem-descriptor registration noise. After the console was cleared, the active runtime emitted no further errors or warnings. Keep package/domain-reload noise separate from project-code errors and recheck it after transfer.

Committed visual evidence:

- `Assets/_Project/Captures/V040/V040_7C_Overhead_Open.png`
- `Assets/_Project/Captures/V040/V040_Containment.png`
- `Assets/_Project/Captures/V040/V040_Coolant.png`
- `Assets/_Project/Captures/V040/V040_Fuel.png`
- `Assets/_Project/Captures/V040/V040_Runtime_Spawn.png`

These images establish composition and obstruction only. They do not prove player comprehension, Owl fairness, full runtime traversal, or XR comfort.

## Validation Still Open

The following is explicitly `Not Tested` and must remain so until real evidence exists:

- the three uninformed desktop sessions in `docs/40_v0_4_0_0_human_validation_sheet.md`;
- whether players recognize all three lobes, remember the circuit, distinguish connector risks, return to the Annex unaided, and deliberately switch connectors under pressure;
- a human-controlled forward and reverse circuit;
- carrying a rod through every portal and manually scanning/depositing during the built-player smoke;
- one full-cover/quiet-relocation comparison in every lobe;
- portable and fixed distractions in every lobe;
- human F3 ON/OFF comparison and whether the Owl feels explainable rather than ruthless;
- manual Owl contact/escape, restart, and clean exit in the standalone v0.4 build;
- all headset/XR comfort, physical crouch/reach/throwing, spatial audio in-headset, controller haptics, floor vibration, Heat/Wind hardware, venue operation, Bluetooth heart-rate, and physical safety evidence.

The prior smoke was process/initialization evidence only, not a meaningful completed gameplay smoke. Never upgrade it to “fully tested.”

The recommended next evidence action is the first uninformed F3-ON v0.4 desktop session. Only after repeated evidence should the user approve a small `v0.4.z.w` connector-preview, signage, sightline, cover-framing, node, audio, pickup, or Owl-tuning refinement. Do not add a minimap, objective arrow, new route, new mechanic, new objective, new entity, inventory, door, hazard, capture system, hardware behavior, or generalized framework to solve a readability complaint without explicit approval.

## Implementation Discipline

For every approved change:

1. Record the approved task, authority, version, observable behavior, acceptance criteria, systems, comparison/fallback mode, exclusions, and genuine open question.
2. Audit live code/editor state and reproduce a defect when practical.
3. Run focused baseline tests and record pre-existing failures separately.
4. Make the smallest coherent change through existing contracts.
5. Put deterministic decision logic in testable C# helpers/classes where practical; keep MonoBehaviours focused on Unity integration.
6. Add/update focused tests in the same slice.
7. After C# edits, compile/import through Unity MCP and inspect the console before continuing.
8. For generated facility changes, edit `V040FacilityBuilder`/declarative specifications, leave Play Mode, regenerate only the disposable root through MCP, and save through MCP.
9. Validate in layers: focused Edit Mode, relevant Play Mode, runtime visual inspection, reset/comparison modes, proportional full regression, and build/smoke when required.
10. For presentation changes, inspect the actual player camera at representative standing/crouched/leaned/moving viewpoints and capture evidence.
11. Update `docs/06_prototype_log.md` with facts. Update `docs/05_decision_log.md` only for a decision the user actually made or approved.
12. Separate automated proof, operator walkthrough, uninformed human evidence, XR, and physical-hardware evidence.

## Git, Version, and Release Rules

- Do not delete, reset, clean, checkout, amend, rebase, commit, push, publish, create a release, change the version, or overwrite a build unless the user explicitly requests the applicable action.
- Preserve every staged, modified, untracked, ignored, generated, and LFS file outside the approved scope.
- The project convention is that a future unqualified user instruction to `commit` means audit/stage the intended scope, commit it, and push it to the configured remote. It does not authorize force-push or unrelated files.
- Versions use `v{x}.{y}.{z}.{w}`. Unity bundle versions omit only the leading `v`.
- Do not infer a version increment from patch size. Use the version the user approves.
- Do not reuse `v0.4.0.0` for materially different content.
- Release-facing commit subjects should start with the exact approved version.
- Versioned Windows output belongs under `UnityProject/InternshipGameSynced/Builds/v{x}.{y}.{z}.{w}/` and remains ignored unless the user changes policy.

## Communication and Final Reporting

Report meaningful checkpoints during implementation: connection confirmed, baseline understood, compilation result, focused/full tests, visual inspection, reset/comparison coverage, build/smoke result, and blockers.

Every final implementation report must include:

1. approved task and authority;
2. outcome and build identity;
3. exact code files and Unity scenes/assets/settings changed;
4. implemented behavior and any bounded coordinate/technical deviations;
5. tests added or updated and exact focused/Edit Mode/Play Mode counts;
6. console status, distinguishing project errors from package/tool noise;
7. build path, result, duration, size, errors, and warnings when required;
8. launch-only versus meaningful gameplay smoke evidence;
9. runtime inspection cases and screenshot paths;
10. technical assumptions only;
11. human desktop evidence or `Not Tested`;
12. XR/hardware evidence separately, normally `Not Tested`;
13. cuts, blockers, deferred work, known risks, and next evidence-driven action;
14. confirmation that no unapproved design/gameplay family was added;
15. exact Git status and commit/push result only when requested.

Never turn “not verified” into “works.” If the new laptop cannot access commit `3abcec7`, LFS objects, the correct Unity version, or the Unity MCP connection, report that concrete transfer blocker before attempting implementation.
