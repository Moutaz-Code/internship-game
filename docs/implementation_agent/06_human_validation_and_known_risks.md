# Human Validation and Known Risks

Last verified: 2026-08-10

## Evidence Boundary

Automated tests establish deterministic logic, serialized scene expectations, route connectivity, collision clearances, and integration regressions. They do not establish whether the game feels fair, readable, frightening, comfortable, or well paced.

Do not convert the automated `v0.4.0.0` pass into a human-design claim.

## Immediate Desktop Human Validation

The next person-led v0.4 validation should use `../40_v0_4_0_0_human_validation_sheet.md` and check:

- whether a first-time player states the mission correctly within two minutes;
- whether scanning, Geiger guidance, live Heat confirmation, inert feedback, rack rejection, and extraction cues are understood;
- whether the full run trends toward 15–20 minutes without artificial waiting;
- whether Containment, Coolant, and Fuel are distinguishable while moving without F1;
- whether players can return to the Contractor Annex and deliberately choose among North Pipe Gallery, Utility Spine, and South Maintenance Channel;
- whether the Owl visibly appears suspicious after one credible observation and adapted after two;
- whether players respond by varying routes;
- whether that adaptation feels plausible rather than omniscient;
- whether any route becomes effectively unusable;
- whether F3 baseline versus adaptation produces an observable difference;
- whether F1 telemetry explains every evidence and state transition to the operator;
- whether the Owl can see and pressure a moving player without seeing through full cover;
- whether warning, chase speed, lost-sight grace, and search duration feel fair;
- whether crouch height/speed and ceiling blocking feel natural;
- whether lean distance, roll, collision limiting, and corner usefulness feel natural;
- whether pickup reach/assistance feels forgiving without selecting the wrong object;
- whether throw force and loose-prop distraction are legible;
- whether all seven portal approaches feel clear at native resolution;
- whether signs and mission/rack UI remain readable while moving, crouched, and leaned;
- whether lighting, inlays, and room silhouettes reduce navigation confusion;
- whether pause, cursor, fullscreen, and 144 Hz movement remain reliable in the standalone build.

Continue the separate route-adaptation and readable-stealth gates with `../32_pt2_human_validation_sheet.md` and `../35_v0_3_0_0_human_validation_sheet.md`. These sheets guide evidence collection; they do not authorize new gameplay.

## Hardware and XR Evidence Still Missing

No physical headset or venue hardware was available for the implementation passes. The following remain unverified:

- headset locomotion comfort;
- head/camera ownership under XR;
- physical crouch and lean behavior;
- XR grab pose, reach, drop, and throw;
- controller haptics;
- floor vibration hardware;
- heat-lamp behavior and safety;
- wind hardware;
- Bluetooth heart-rate reliability and consent/calibration behavior;
- venue runtime, headset model, boundary, and performance;
- actual hardware failure/recovery behavior.

Desktop simulation is not evidence for physical-device safety or headset comfort.

## Known Project Risks

### Working tree ownership

The repository matched `origin/master` before the 2026-08-10 documentation refresh, but that fact can become stale immediately. A replacement agent must run a fresh status audit and preserve any newer user-owned changes.

### Scope can outrun the remaining delivery window

PT2, v0.3, and v0.4 feature expansion are closed except for evidence-driven tuning or release-blocking fixes. No later feature family is authorized by the current documents. Preserve tests, fairness rules, reset behavior, human-validation time, and build reliability before adding content.

### Generated versus manual environment content

The facility is builder-generated. Rebuilding the wrong root or editing generated scene objects manually can cause later regeneration to erase work. Keep durable changes in the builder/specification and preserve manual dressing outside the generated root.

### Desktop and XR can diverge

Desktop bob, roll, crouch, lean, pickup assistance, and movement tuning are prototypes for keyboard/mouse. They must not be copied into XR without comfort and controller requirements.

### OpenXR desktop sanitation

Standalone no-headset builds intentionally avoid runtime startup noise and temporarily sanitize feature objects during build. Package or platform changes can regress either desktop startup or restoration of XR configuration. Validate both the build and post-build editor state.

### Frame-rate-sensitive movement

The 144 Hz WASD bug was caused by a movement threshold, not lost input. Preserve the zero minimum-move-distance safeguard and test at high refresh after controller changes.

### AI fairness and knowledge leakage

The Owl’s implementation must retain a distinction between visible observations, authored connector evidence, last credible observation, and hidden current player position. A convenient reference to the live player transform can silently reintroduce omniscience.

### Feedback semantics

Heat, Owl-proximity vibration, and extraction Wind have unique meanings. Reusing them for unrelated cues weakens device comprehension and invalidates design evidence.

### External hardware reliability

Heat lamps, blowers, floor vibration, heart-rate sensors, and Bluetooth devices can disconnect or fault. Future adapters must remain optional, normalized, independently recoverable, and unable to break the game loop.

## Honest Reporting Language

Use:

- “Automated tests passed the documented cases.”
- “The build launched and remained alive for the smoke window.”
- “Runtime screenshots show the tested viewpoints.”
- “Human feel/readability/comfort remains unverified.”

Do not use:

- “Players understand it” without observed players.
- “VR works” without a headset test.
- “Hardware is safe” without device requirements and safety validation.
- “The Owl is fair” based only on unit tests.
- “The build is fully tested” after only a launch smoke test.
