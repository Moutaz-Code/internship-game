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

### Archived P1 Concept: First Playable Extraction Loop

Target date: TBD, recommended Sprint 02

Question:

> Is it tense and understandable to enter a small VR facility, locate a signal core, sense a threat, and extract?

Build status: Superseded by the implemented Multi-Path Fuel-Rod Vertical Slice, now named Prototype 1. Its core extraction-loop question remains relevant, but the one-core and compact-facility implementation assumptions no longer represent the prototype.

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

### Prototype 1: Multi-Path Fuel-Rod Vertical Slice (formerly P2)

Date range: 2026-07-16 to first external playtest

Question:

> Can a first-time desktop player plan routes through a larger authored facility, reliably identify and transport three live fuel rods under fair adaptive Owl pressure, and survive a clearly communicated extraction holdout?

Build status: Feature skeleton approximately 90% implemented by developer estimate. Automated implementation and build checks pass; movement, threat behavior, and the fuel-rod loop are still rudimentary, and external pacing, fairness, comprehension, and replay-interest evidence are pending.

Naming note:

- This build was historically labeled `P2` in this log because P0 and an early P1 concept preceded it.
- The project team refers to this implemented build as Prototype 1.
- The historical `P2` label is retired. Prototype 2 is the Readable Route Adaptation experiment below.

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

### Prototype 2: Readable Route Adaptation

Date range: 2026-07-22 to 2026-07-24 target gate

Authoritative scope: `24_prototype_02_scope.md`

Question:

> Does the Owl appear to notice repeated route-family use, become suspicious after one credible observation, adapt after two, and encourage the player to vary routes without feeling omniscient or making any route unusable?

Build status: PT2 implementation and automated verification are complete. The focused threat suite passes 28/28, the full Edit Mode suite passes 89/89, and integrated Play Mode passes 3/3. A strict clean Windows development build completed at 182.56 MB with zero build errors or warnings. The executable remained stable through representative desktop and headless startup windows; launch evidence is qualified by pre-existing OpenXR feature-serialization/runtime warnings and a standalone NavMesh-agent warning described below. Human route-readability validation remains pending.

Must prove:

- Run-only route memory resets completely.
- Direct observation matters more than inferred evidence.
- One credible direct observation creates suspicion; two repeated observations may create capped adaptation.
- Silent unperceived route use does not become entity knowledge.
- Route variety weakens the previous prediction and creates an opening.
- Players perceive the behavioral change without an adaptation notification.
- Operator telemetry explains every transition.
- Adaptation can be disabled for baseline comparison.

Explicitly does not prove:

- Capture recovery.
- Full stealth, hiding, inventory, stun, cart, or environmental systems.
- Heart-rate gameplay.
- Learned/neural AI.
- VR comfort or physical hardware.
- Final Owl art or animation.

Evidence required:

- Focused model and integration tests.
- Relevant regression tests.
- Same-seed deterministic evidence replay.
- Adaptation-disabled comparison.
- Fresh desktop build and smoke test where supported.
- At least two observed human sessions, target three.

Results:

- PT2-001 baseline audit: complete. The audit confirmed unconditional `OnZoneEntered` knowledge, no direct/inferred distinction, no suspicion/counter state, a patrol selector that did not consume the model's 1.75 weight, single-line telemetry, and no A/B toggle. The pre-change focused threat/navigation/state baseline passed 18/18.
- PT2-002 evidence model: complete. `ThreatPressureModel` now owns deterministic per-wing confidence, Direct/Inferred evidence, `Uncertain`, `Suspicious`, `Adapted`, and `CounterAdapted` states, inferred cooldown, delayed passive decay, cross-wing confidence reduction, run reset, and an adaptation-disabled baseline. Defaults remain Direct 1.0, Inferred 0.5, Suspicious 1.0, Adapted 2.0, adapted cap 1.75, and bias duration 60 seconds.
- PT2-003 evidence wiring: complete. Zone entry records Direct evidence only while the active Owl has current line of sight. Strong route-local sprint, landing, dropped-prop, rejection, and objective stimuli may record rate-limited Inferred evidence. Inferred records contain route family and event kind but no exact hidden position.
- PT2-004 behavior and counterplay: complete. Suspicious and Adapted states select authored route inspection/patrol/search responses; Adapted uses the capped weight, and the third consecutive biased patrol choice is forced to consider another wing. Search allocation extends toward an authored point in the suspected wing. Credible cross-wing evidence reduces the old prediction, temporarily returns patrol weights to baseline, and shortens search to create a counter-adaptation opening. Existing restrained Owl audio provides the only transition cue; no player-facing adaptation notice was added.
- PT2-005 telemetry and comparison: complete. F1 now exposes enabled state, W/C/E confidence, dominant wing, last source/kind/zone or observed position, transition time, decay/bias/counter timing, selected response, tuning, and reset confirmation. F3 clears route memory and switches between adaptation and baseline without conflicting with F1 or F2. The normal mission panel is unchanged.
- PT2-006 regression gate: complete for automation. Focused Edit Mode is 28/28; full Edit Mode is 89/89; integrated Play Mode is 3/3. Coverage includes no unperceived evidence, weighting, inferred rate limiting, thresholds, reset, decay, route variety, counter-adaptation, cap, maximum consecutive choices, deterministic replay, disabled baseline, search timing, existing perception/state transitions, all route profiles, objective completion, failure, and seed reset.
- Build evidence: strict clean `StandaloneWindows64` development build of the exact final source succeeded in 92.44 seconds at 182.56 MB with zero build errors and warnings. The built player stayed alive for 25 seconds on the normal graphics path and was closed deliberately after the smoke window; no project-code exception appeared.
- Qualified startup finding: the player log reports missing/changed serialized Meta OpenXR feature objects and `XR_ERROR_RUNTIME_UNAVAILABLE` because no headset runtime is present, plus `Failed to create agent because there is no valid NavMesh` in the standalone player. Editor Play Mode proves complete primary NavMesh paths under all three route profiles, and the authored collision-validated waypoint adapter remains available as the runtime fallback. XR cleanup and standalone primary-NavMesh diagnosis are preserved as separate follow-up work rather than being expanded under PT2.
- Prototype tuning assumptions: inferred cooldown 4 seconds; confidence decay starts after 60 seconds at 0.025 per second; cross-wing reduction equals the incoming evidence weight; Counter-Adapted opening 12 seconds; Suspicious search multiplier 1.15; Adapted search multiplier 1.35; Counter-Adapted search multiplier 0.8. These are visible comparison values, not final design constants.
- Human evidence: Not Tested. Two observed desktop sessions are still required, target three, to judge noticeability, attribution, route variety, temporary advantage, omniscience, camping, and route viability.
- Decision: automated implementation gate Pass; prototype-question verdict Pending human evidence.
- Scope confirmation: no capture/recovery, second-capture, stun, inventory, crouch/hiding, flashlight, doors, cart, ventilation/radiation, heart-rate, hardware/haptics, cross-run memory, learned AI, new mission, entity, environment, or final-art gameplay family was added.

### Prototype 2.1: Deep Desktop Polish

Date: 2026-07-22

Question:

> Is the Prototype 2 route-adaptation experiment supported by sufficiently reliable physics, objective feedback, readable language, and expressive desktop movement to collect valid human evidence?

Applicable design pillars:

- Physical interaction: carried objects remain exact, released objects recover their authored physics state, and deliberate throws are readable and testable.
- Mission clarity: the player always sees the secured-live-rod count and the next useful action in plain language.
- Meaningful feedback: scan truth, rack rejection, Geiger strength, and environmental feedback retain distinct meanings.
- Route planning and fair counterplay: acceleration-based movement adds commitment without reducing route access, while the Owl continues to use the existing swappable navigation seam.

Results:

- Physics and interaction: desktop carry/drop still restores the complete cached Rigidbody, collision, parent, interpolation, and XR-interactable state. Left mouse now performs an intentional forward throw after that restoration, including inherited player velocity and bounded spin; `E` remains the safe drop action. Rods and loose props receive light damping. The deposit rack now accepts actual collider overlap rather than relying only on the rod center, deterministically ejects inert rods, publishes typed deposit results, and throttles repeated overlap chatter without delaying a changed result.
- Objective communication: player-facing copy now uses `scan` rather than the implementation term `assay`. The HUD prioritizes `LIVE RODS SECURED X/3`, held-rod scan state, the next action, short rack correction messages, extraction state, and a timed controls strip. The Geiger presents qualitative strength without exposing its F2 developer comparison control. F1 threat telemetry remains operator-only and no player-facing adaptation notification was added.
- Desktop locomotion: planar acceleration/deceleration replaces instantaneous full-speed changes, with separate grounded and airborne tuning and collision-measured velocity feedback. Subtle head bob, strafe roll, landing dip, and sprint FOV make motion less linear while leaving jump timing, `IPlayerMotionSource`, `IPlayerContext`, and the XR controller path intact.
- Presentation: desktop camera FOV is 72 degrees, base/ambient lighting is more legible, environmental messages no longer overlap the mission panel, and the controls strip was visually checked at runtime. A live rack exercise confirmed that an unscanned rod returns `Unassayed`, does not deposit, and immediately produces the correction message.
- Runtime hygiene: the NavMesh navigator now samples a valid point before enabling its agent, eliminating the standalone startup warning while preserving the authored waypoint fallback. Standalone OpenXR auto-initialization is disabled for the current no-headset Windows target. A build-only sanitizer excludes OpenXR feature objects from non-XR Windows players and restores all 49 feature objects, including seven enabled choices, after the build. Android/headset settings and the controller abstractions remain present for the hardware pass.
- Automated validation: full Edit Mode passed 95/95. Full integrated Play Mode passed 3/3 after the final navigation/XR configuration, including complete route profiles, objective completion, carry/throw physics, and primary NavMesh behavior. Focused new coverage verifies acceleration, braking, air momentum, direction reversal, throw-state restoration and impulse, true rack collider overlap, separation rejection, and deposit-result transitions.
- Delivery validation: the final strict clean `StandaloneWindows64` development build succeeded in 89.09 seconds at 178.76 MB with zero build errors and zero build warnings. The exact executable remained alive for the 25-second smoke window, reached managed-domain, PhysX, and input initialization, and was then closed deliberately. Stderr was empty; the log contained no OpenXR runtime error, Meta serialization warning, missing script, project-code exception, or invalid-NavMesh-agent warning. The only matched low-level diagnostic was the graphics driver's benign D3D12 info-queue query.
- Supersession note: this resolves the qualified OpenXR and standalone NavMesh startup findings recorded for Prototype 2.0. It does not change the route-adaptation model or expand the PT2 prototype question.
- Human evidence still required: tune acceleration/braking, view motion, throw weight, lighting, message timing, and UI comprehension by feel; then run the two observed route-adaptation sessions required by PT2, target three. VR comfort and hardware behavior remain untested without the headset and devices.
- Scope confirmation: no capture/recovery, stun, inventory, crouch/hiding, flashlight, doors, carts, ventilation/radiation, heart-rate, haptics/hardware, learned AI, new mission, entity, environment, or final-art gameplay family was added.

### Prototype 2.2: Environment Change and Polish

Date: 2026-07-22

Question:

> Can the facility feel intentionally authored and presentation-ready while remaining a disposable greybox and preserving Prototype 2's route experiment?

Applicable design pillars:

- Spatial readability and route planning: West, Central, East, terminal rooms, and extraction need distinct silhouettes without changing their route families.
- Mission clarity: the objective board, intake rack, room identities, route portals, and closure warnings must read at player distance.
- Fair Owl counterplay: presentation geometry must preserve all required paths, sight-breaking cover footprints, typed anchors, and NavMesh behavior.
- Rapid iteration: the pass remains data-driven under the generated facility root so later manual dressing or wholesale replacement is not trapped inside bespoke scene edits.

Results:

- Authored greybox identity: all ten spaces now receive different scrappable profiles. Entry has an offset security booth and extraction canopy; Decontamination uses filters and ceiling ribs; Operations has an offset command side and gantry; Maintenance uses suspended service pipes; Central alternates structural buttresses; Laboratory uses an offset bench and vessels; North Hall uses irregular trusses; Storage adds shelving and a crane; Core uses diagonal braces and an overhead bridge; Reactor uses separated service stacks and a bus. West and East no longer present as mirrored layouts.
- Preserved gameplay geometry: the existing 21 cover footprints remain unchanged for collision and navigation, but now read as a modular equipment family with caps, bases, and alternating identity rails. Most added silhouette pieces are overhead or peripheral; structural pieces stay outside the required travel lanes.
- Construction cleanup: all thirteen portals now use a consistent 2.76 m clear opening, fitted 3.2 m headers, deeper jambs, two-sided labels, and corrected wall-plane positions for the West, Central, and East Hub exits. The route-profile blockers retain their authored positions and carving behavior.
- Surface cleanup: full-room identity plates were replaced by narrow room inlays. Room and corridor surfaces have explicit vertical separation, eliminating the overlapping coplanar layers that caused visible seams and Z-fighting risk while preserving cyan, amber, neutral, green, and red wayfinding meanings.
- UI and language: the mission display is a scale-one framed assembly suspended directly above the intake rack and visible from spawn. The rack has a mounted `LIVE ROD INTAKE / SCAN-CONFIRMED RODS ONLY` panel and numbered slots. Room panels, portal labels, socket stencils, shortcut guidance, and route-closure warnings use bounded rectangles, auto-sizing where appropriate, and no unsupported ellipsis glyph. Player-facing separators are ASCII-safe and the implementation term `assay` remains absent from guidance.
- Visual evidence: positioned Unity captures verified the spawn/intake composition, Hub, West Maintenance, East Laboratory, North Cross Hall, a fitted Decontamination portal, and mission/rack readability at player distance. The generated presentation remained visibly greyboxed rather than becoming a premature art pass.
- Automated validation: full Edit Mode passed 95/95. Integrated Play Mode passed 4/4, including carry/drop/throw physics, all seeded route profiles, every socket path, primary NavMesh behavior, objective/extraction/reset flow, plus a new P2.2 scene check for ten non-empty room profiles, thirteen fitted 2.76 m portals, separated route surfaces, scale-one mission/rack text, and removal of legacy full-room identity plates.
- Delivery validation: the strict clean `StandaloneWindows64` development build succeeded in 59.73 seconds at 187,642,038 bytes with zero errors. Its two reported warnings were the MCP WebSocket closing while Unity synchronously blocked to build; the previous 37 content warnings from unsupported TMP ellipsis mode were removed before this final build. The exact executable remained alive for the 25-second normal-graphics smoke window and was then closed deliberately. It reached managed-domain, PhysX, D3D12, and input initialization with no project exception, missing script, OpenXR runtime failure, Meta serialization warning, or invalid NavMesh-agent warning. The only matched low-level diagnostic was the graphics driver's benign D3D12 info-queue query.
- P2.3 backlog and human validation: walk every route at native play resolution, inspect all terminal-room socket approaches, judge room distinction without the operator overlay, tune lighting/material intensity, check signage while moving, inspect seams from crouched and jump viewpoints, and validate collision feel around the new peripheral structures. Use those sessions for tuning rather than expanding P2.2 now.
- Scope confirmation: no route topology, objective behavior, Owl adaptation, capture/recovery, stealth, inventory, hardware/haptics, VR comfort behavior, mission, entity, final-art family, or procedural geometry was added or redesigned.

### v0.2.2.1: Fullscreen and Versioning Maintenance

Date: 2026-07-22

Question:

> Can the supervisor-facing desktop build move cleanly between fullscreen and windowed presentation while carrying an unambiguous development version?

Applicable design pillars:

- Demo reliability: the operator can present borderless fullscreen or recover to a normal resizable window without rebuilding.
- Mission communication: the new display control is included in the existing controls and pause language without obscuring mission feedback.
- Rapid iteration: build identity and output paths derive from Unity's four-part application version instead of a hard-coded prototype label.

Implementation:

- `F11` toggles between borderless fullscreen and windowed mode through a desktop-only display controller. Windowed mode preserves the latest usable size and falls back to 1280x720; Unity's Windows player remains configured to start in native-resolution borderless fullscreen and permits platform fullscreen switching.
- The pause panel, first-run controls strip, and F1 operator overlay communicate the control; pause and operator information show `v0.2.2.1` from `Application.version`.
- Unity's application version is `0.2.2.1`, the window is resizable, and strict Windows builds now validate a four-part numeric version before writing `Builds/v0.2.2.1/InternshipGame_v0.2.2.1.exe`.
- The durable `v{x}.{y}.{z}.{w}` convention is recorded in `27_development_versioning.md` and decision D-082. The immediately preceding environment baseline is `v0.2.2.0` (legacy P2.2).

Validation:

- Script compilation and Unity structural validation completed with zero errors. The three existing obsolete-state warnings in legacy state-machine tests remain unchanged.
- Full Edit Mode passed 103/103, including eight focused cases for fullscreen classification and deterministic borderless/windowed toggling.
- Integrated Play Mode passed 4/4. The scene-level gate confirms exactly one display controller, its F11 status language, and `Application.version == 0.2.2.1` while retaining the established physics, route, objective, Owl, extraction, reset, and presentation checks.
- The strict clean `StandaloneWindows64` development build succeeded with zero errors and zero warnings at 187,643,890 bytes in 313.01 seconds. Its version-derived output is `Builds/v0.2.2.1/InternshipGame_v0.2.2.1.exe`.
- The exact executable remained alive until deliberately closed and reached managed-domain reset, PhysX, and runtime input initialization. Its smoke log contained no project exception, missing script, OpenXR startup failure, serialization failure, or invalid NavMesh-agent error. The only matched low-level diagnostic was the graphics driver's previously observed benign D3D12 info-queue query.

Human validation remaining: press F11 in the visible Windows player on the intended presentation monitor and confirm the borderless/windowed transition, preserved window size, UI scaling, and preferred supervisor-demo mode.

Scope confirmation: this maintenance increment adds no gameplay system, route, objective, threat behavior, mission, entity, environment, or hardware integration.
