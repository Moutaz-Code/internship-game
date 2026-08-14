# System Ownership and Architecture Map

Last verified: 2026-07-28

## Architecture Intent

The prototype is intentionally modular enough to replace desktop controls with XR, replace or extend hardware outputs, move the Owl to another compatible authored map, and tune or replace one objective implementation without turning the project into a generalized game framework.

The governing principle is narrow, typed seams around real variation points. Avoid speculative abstractions.

## Assemblies

| Assembly | Purpose |
|---|---|
| `InternshipGame.Prototype.Runtime` | Runtime gameplay code. References Input System, TextMeshPro, and XR Interaction Toolkit. |
| `InternshipGame.Prototype.Editor` | Facility/scene generation, build sanitation, and editor-only integration. |
| `InternshipGame.Prototype.Tests` | Edit Mode tests for deterministic logic and architecture seams. |
| `InternshipGame.Prototype.PlayModeTests` | Integrated scene and runtime validation. |

## System Map

| System | Primary files | Ownership boundary |
|---|---|---|
| Run/session | `Run/PrototypeRunController.cs`, `PrototypeRunStateMachine.cs`, `PrototypeRunInput.cs`, `RunConfiguration.cs` | Own run state, seeded configuration, reset, extraction cycle, escape/failure; do not make it know concrete controllers or devices. |
| Objectives | `Objectives/FuelRodItem.cs`, `FuelRodObjectiveSet.cs`, `FuelRodDepositRack.cs`, `FuelRodRunCoordinator.cs`, `RunProgressSnapshot.cs` | Own unique rod identity, hidden classification, scan, carry/deposit state, typed rack results, and aggregate progress. |
| Map/environment | `Environment/PrototypeMapDefinition.cs`, `FacilityRouteGraph.cs`, zone/socket/variation/lighting anchors | Expose typed authored anchors and connectivity. Do not rely on scene object-name searches for gameplay. |
| Facility generation | `Editor/FacilityGeneration/V040FacilityBuilder.cs`, `FacilityGeneration/PrototypeSceneBuilder.cs` | Generate the replaceable facility root, portal geometry, anchors, presentation, and navigation setup. Preserve manual content outside the generated root. Add a new approved version builder here rather than reviving the removed pre-7C builder. |
| Player | `Player/PlayerModeController.cs`, `DesktopFirstPersonController.cs`, `PlayerMotionSource.cs`, movement/posture helpers | Keep `IPlayerContext` as the controller swap seam and motion telemetry generic. Desktop embodiment must not silently define XR comfort behavior. |
| Interaction | `Interaction/DesktopGrabInteractor.cs`, `GrabbableObject.cs` | Own desktop targeting, ownership, exact holding, state restoration, drop, and throw. Keep XR ownership mutually exclusive. |
| Threat decision | `Threats/ThreatPressureModel.cs`, `PursuitThreatStateMachine.cs`, `PursuitThreatController.cs`, `ThreatContracts.cs` | Keep evidence, adaptation, state decisions, credible observations, and expression testable. Do not hide exact player knowledge inside navigation. |
| Threat navigation | `NavMeshThreatNavigator.cs`, `WaypointGraphThreatNavigator.cs`, `ThreatRouteNavigator.cs` | Implement movement behind `IThreatNavigator`; decision code must not depend on a concrete NavMeshAgent. |
| Threat perception/stimuli | `ThreatPerceptionMath.cs`, `PlayerThreatStimulusEmitter.cs`, `ThrowableThreatStimulus.cs`, `ThreatTargetPolicy.cs` | Convert visible observations and audible/credible stimuli into narrow threat inputs. Preserve occlusion and evidence rules. |
| Feedback | `Feedback/FeedbackHub.cs`, `FeedbackContracts.cs`, desktop/audio/light/XR/external outputs | Route typed normalized signals to isolated outputs. One failed device must not break gameplay. Preserve unique cue meanings. |
| Geiger | `Feedback/GeigerCounterController.cs`, `Threats/GeigerSignalModel.cs` | Guide discovery, support bounded comparison interference, never classify rods or emit Heat truth. |
| Biometrics | `Biometrics/BiometricContracts.cs`, `BiometricHub.cs`, `BluetoothHeartRateSensorStub.cs` | Compile-ready abstraction/stub only. It is not authorized gameplay input. |
| UI/display | `Run/PrototypeHudPresenter.cs`, `Core/DesktopPauseController.cs`, `DesktopDisplayController.cs` | Separate player-facing communication, pause/cursor state, display mode, and operator telemetry. |
| XR desktop-build hygiene | `Editor/BuildPipeline/DesktopPrototypeXrBuildSanitizer.cs` | Keep no-headset Windows builds clean while restoring XR feature configuration after the build. |
| Tests | `Assets/_Project/Tests/EditMode/**`, `PlayMode/PrototypePlayModeTests.cs` | Prove deterministic logic, boundaries, scene structure, integration, and regressions. |

## Core Contracts

### Player

- `IPlayerContext` exposes active mode, player root, view, and interaction origin.
- `IPlayerMotionSource` exposes grounded state and generic movement telemetry.
- `PlayerModeController` selects desktop or XR control without downstream systems knowing the concrete controller.

Do not let an objective, threat, HUD, or interaction consumer reach directly into desktop-controller internals when the contract already exposes what it needs.

### Objective and run

- `IRunObjective` is aggregate progress, not a single concrete rod.
- `RunProgressSnapshot` reports counts without prematurely exposing live/decoy truth.
- `PrototypeRunState` uses `Ready`, `RetrieveFuelRods`, `ExtractionCycling`, `EscapeReady`, `Extracted`, and `Failed`.
- `FuelRodItem` owns identity, classification, scan, carry, and deposit state.
- Rack feedback uses typed deposit results.

Preserve idempotence: a rod cannot register twice, a slot cannot count twice, and reset must restore every rod and slot.

### Map

- `PrototypeMapDefinition` is the typed map boundary.
- Anchors expose spawn, rack, exit, Owl spawn/patrol, eight sockets, route variations, lighting variants, and ten facility zones.
- `FacilityRouteGraph` is the testable connectivity model.

New compatible maps should supply the same required anchors and traversability rather than depending on hidden scene names.

### Threat

- `IThreatNavigator` isolates movement.
- `IThreatRouteProfileReceiver` receives authored route-state changes.
- `IThreatStimulusSink` receives positional, typed stimuli.
- `RouteEvidence` separates Direct and Inferred knowledge.
- `ThreatPressureModel` owns run-local route confidence and comparison behavior.

Perception, evidence, adaptation, state decisions, and navigation are distinct concerns. Never make the navigator an omniscient sensor.

### Feedback and hardware

- `FeedbackHub` publishes typed `FeedbackSignal` values to `IFeedbackOutput` implementations.
- Outputs advertise availability and supported channels.
- `DeviceFaultGate` contains failures and retry behavior.
- Real hardware must be an adapter behind the same contracts and must fail safe.

Do not introduce a generic global event bus or service locator. Continue using narrow typed C# events and the existing `FeedbackHub`.

## Invariants to Protect

### Mission and reset

- Exactly three live rods are required.
- Rod truth is hidden until scan completes.
- Heat alone authoritatively confirms a live rod.
- The third accepted rod starts the 20-second extraction cycle.
- The exit requires an explicit final crossing.
- Same-seed restart reproduces the same authored configuration.
- Reset clears rods, slots, timers, threat state, route memory, route/lighting state, feedback, and UI.

### Fairness and route access

- Every required objective and extraction route remains accessible without jumping.
- Route variation changes authored state, not procedural geometry.
- The Owl may pressure a route but cannot make a route unusable.
- Silent unseen movement does not become Owl knowledge.
- Inferred evidence contains no exact hidden player position.
- Losing sight follows the last credible observation.
- Route variety must reduce confidence in stale predictions.
- Adaptation memory is run-only.

### Interaction

- Only one desktop-held item at a time.
- Desktop and XR ownership are mutually exclusive.
- Held pose remains exact.
- Release restores complete cached physical/interactable state before throw impulse.
- Aim assistance never permits a through-wall pickup.

### Platform and hardware

- Desktop keyboard/mouse is the active development path.
- XR jump and comfort behavior remain separate until headset testing.
- Hardware feedback is optional, normalized, isolated, and failure-tolerant.
- Biofeedback cannot secretly determine success/failure or reveal hidden player state.

## Dependency Discipline

Current important packages include:

- Unity Input System `1.19.0`
- Unity AI Navigation `2.0.14`
- URP `17.5.0`
- XR Interaction Toolkit `3.5.1`
- XR Management `4.5.4`
- OpenXR `1.17.1`
- Unity MCP package from `CoplayDev/unity-mcp`

Do not add or upgrade packages merely because a newer version exists. Package changes require task relevance, Unity MCP operation, compilation/regression checks, and explicit reporting.
