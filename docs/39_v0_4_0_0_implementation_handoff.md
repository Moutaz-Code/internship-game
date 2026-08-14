# v0.4.0.0 Implementation Handoff

Status: Active implementation handoff

Last updated: 2026-08-02

Repository note: the 2026-08-11 cleanup moved the active builders to `Assets/_Project/Scripts/Editor/FacilityGeneration/` and removed the superseded `ExpandedFacilityBuilder`; references to it below describe the historical pre-v0.4 starting point.

## One-Sentence Assignment

Replace the current rectangular proof-facility greybox with the locked 7C three-lobed circuit, preserving the completed v0.3 stealth and contract systems while making lobe identity, connector choice, Owl pressure, and return navigation readable and testable.

## Read Order

Before changing code or Unity state, read:

1. 37_v0_4_0_0_spatial_readability_scope.md
2. 38_v0_4_0_0_7c_level_plan.md
3. this handoff
4. implementation_agent/01_authority_and_scope.md
5. implementation_agent/02_current_implementation_state.md
6. implementation_agent/03_system_ownership_map.md
7. implementation_agent/04_operating_and_validation_procedure.md
8. 10_technical_architecture_outline.md
9. 27_development_versioning.md
10. 33_v0_3_0_0_readable_stealth_scope.md for protected regression behavior
11. current builders, map definitions, route graph, Owl controller, and their tests

The v0.4 scope and level plan override older map descriptions. v0.3 remains authoritative for perception, sound, counterplay, and feedback behavior.

## Verified Starting Point

Read-only audit on 2026-08-02 found:

- branch master at published v0.2.3.0 commit 0636f5d;
- a substantial user-owned dirty working tree containing the local v0.3.0.0 implementation and documentation;
- Unity 6000.5.4f1 with Assets/_Project/Scenes/P01_CoreLoop.unity open in Edit Mode;
- generated root named _Generated Polished Prototype;
- current local v0.3 build at Builds/v0.3.0.0/InternshipGame_v0.3.0.0.exe;
- recorded local v0.3 evidence: 133/133 Edit Mode and 6/6 Play Mode passed, focused 22/22 passed, strict Windows build and smoke passed;
- current console with no errors and three obsolete-test warnings;
- v0.3 uninformed human validation still Not Tested;
- no XR, headset, haptics, physical effects, venue hardware, or heart-rate validation.

Re-audit everything. Treat every current modification and untracked file as user-owned. Do not reset, discard, overwrite, commit, or push.

## Existing Architecture To Extend

- PrototypeSceneBuilder orchestrates the generated scene and current facility generation.
- ExpandedFacilityBuilder owns declarative space, corridor, portal, cover, landmark, socket, waypoint, variation, and zone data.
- PrototypeMapDefinition owns map-zone semantics.
- FacilityRouteGraph owns static connectivity and variation-aware route checks.
- PursuitThreatController consumes navigation, perception, sound, search, and route-adaptation data.
- The current route-family model uses West/Central/East and includes position-based inference.
- The generated facility already wires nine portable props and three machinery panels.

Preserve these seams. A narrow semantic connector mapping is expected; a generalized level framework or sweeping enum migration is not.

## Task Order

### V040-001: Baseline And Ownership Record

Priority: Urgent

- Record the task, version, authority, observable question, affected systems, acceptance criteria, and exclusions.
- Audit Git status, Unity instance/project/scene/mode, generated-root state, current console, version, relevant code/tests, and build source.
- Run focused facility/route/navigation tests plus the focused v0.3 stealth tests before editing.
- Separate pre-existing warnings/failures from v0.4 changes.

Done when the baseline and preservation boundary are explicit.

### V040-002: Declarative 7C Map Model

Priority: Urgent

Likely files:

- Assets/_Project/Scripts/Editor/ExpandedFacilityBuilder.cs
- Assets/_Project/Scripts/Environment/PrototypeMapDefinition.cs
- Assets/_Project/Scripts/Environment/FacilityRouteGraph.cs
- focused Edit Mode tests

Implement the spaces, portals, graph, landmarks, sightline probes, socket families, content anchors, and variation profiles from the level plan as declarative builder data.

Required:

- four playable macro zones: annex plus three lobes;
- three explicit connector zones;
- inaccessible central void;
- P01–P07;
- one local loop per lobe;
- eight sockets with lobe ownership;
- graph-level two-route guarantees from the Containment side of P01, with the annex spur treated as the only permitted shared edge;
- deterministic Open Circuit, Coolant Service, and Fuel Inspection profiles.

Build and test the data model before replacing generated geometry.

### V040-003: Connector Semantics And Route Evidence

Priority: Urgent

Likely files:

- Assets/_Project/Scripts/Environment/PrototypeMapDefinition.cs
- Assets/_Project/Scripts/Environment/FacilityRouteGraph.cs
- Assets/_Project/Scripts/Threats/PursuitThreatController.cs
- route-adaptation tests

- Add a narrow explicit mapping for North Pipe Gallery, Utility Spine, and South Maintenance Channel.
- Retain West/Central/East compatibility values internally if that minimizes regression risk.
- Stop active route evidence from relying on position-only X thresholds.
- Count route choice only at connector zones/nodes and direct events attributed to a connector.
- Render connector names in F1.
- Confirm lobe/objective movement alone creates no connector evidence.
- Preserve existing adaptation states, caps, variety weakening, direct/inferred distinction, and F3 behavior.

### V040-004: Generated Shell And Macro Navigation

Priority: Urgent

- Update the durable builder/specification first.
- Leave Play Mode.
- Regenerate only the disposable generated root through Unity MCP.
- Preserve manual content outside that root.
- Build the asymmetric shells, central void, connectors, floor heights, ramps, portals, and first-pass NavMesh.
- Inspect each portal from player and Owl scale.
- Verify non-jump player and Owl connectivity before detailed cover/content.

Do not hand-edit Unity YAML or serialize scene changes outside Unity MCP.

### V040-005: Lobe Grammars And Content Distribution

Priority: Urgent

Build in this order:

1. Containment reactor loop and four shield masses.
2. Coolant basin loop and three distinct pump/pipe clusters.
3. Fuel rows, crosscuts, full-height shield, and hot-zone boundary.
4. Connector-specific cover/exposure.
5. Annex, rack, spawn, and extraction.
6. C1–C3, K1–K3, F1–F2.
7. one machinery panel and three portable props per lobe.

Inspect after each lobe. Use silhouettes and spacing from the level plan; do not add cosmetic clutter to manufacture identity.

### V040-006: Objective Distribution And Seeded Variation

Priority: Urgent

- Select one live rod per lobe and exactly two inert decoys from unused sockets.
- Preserve scan, carry, rack, extraction, feedback, and reset behavior.
- Same seed reproduces socket and route profile selection.
- New seed changes within bounded supported profiles.
- Validate every supported seed/profile combination at graph level.
- Ensure all active pockets are pass-through and all content is reachable.

### V040-007: Owl Graph, Sightlines, And Anti-Camping

Priority: Urgent

- Author approximately 18 Owl nodes using the level-plan rules.
- Keep connector metadata only on connector nodes.
- Prevent a normal wait/hold point from controlling two portals.
- Provide two approaches to every lobe.
- Validate NavMesh navigation and fallback graph connectivity.
- Run controlled full-cover and quiet-relocation cases in each lobe.
- Inspect the 6–22 m sightline budgets and the three connector risk identities.
- Confirm no objective, rack, or extraction location becomes a normal Owl blocker.

### V040-008: Telemetry, Signs, And Runtime Inspection

Priority: High

- Show current lobe, current connector, connector evidence, knowledge source, movement target, and adaptation state in F1.
- Use player-facing connector and lobe names on restrained in-world signage.
- Keep normal HUD free of route/debug values.
- Inspect the required shots listed in the level plan from standing and crouched views.
- Exercise F3 OFF and ON, F2 behavior, reset paths, objective scan/deposit, extraction, pause, and fullscreen.

### V040-009: Regression, Version, Build, And Evidence

Priority: Urgent

- Run focused new Edit Mode tests.
- Run all Edit Mode and Play Mode tests.
- Inspect console after compile, tests, regeneration, runtime, and build.
- Set the build identity to v0.4.0.0 through Unity MCP.
- Produce a fresh Windows desktop build.
- Launch and smoke-test a meaningful circuit, one rod scan/deposit, one Owl contact/escape, restart, and clean exit.
- Update 06_prototype_log.md with exact automated, build, smoke, desktop-human, XR, and hardware evidence.
- Report every cut and known limitation.
- Do not commit or push unless separately requested.

## One-Week Sequence

### Day 1: Data And Graph

- V040-001 through V040-003.
- End with a tested map specification, connector semantics, and preserved v0.3 focused tests.

### Day 2: Shell And Navigation

- V040-004.
- End with a complete circuit, valid portals, central void, and player/Owl connectivity.

### Day 3: Lobe Grammars

- V040-005.
- End with each lobe visually distinct and mechanically traversable.

### Day 4: Content, Owl, And Variation

- V040-006 through V040-008.
- End with the complete contract, explicit route evidence, and representative runtime evidence.

### Day 5: Human Test And Delivery Gate

- V040-009 and 40_v0_4_0_0_human_validation_sheet.md.
- Protect time for at least three uninformed desktop sessions where practical.
- Apply only bounded evidence-driven fixes that preserve the prototype question.

## Required Automated Coverage

At minimum, cover:

- expected space/connector/portal identifiers and counts;
- locked macro graph edges;
- one complete local loop per lobe;
- two graph-distinct routes from the Containment side of P01 to every socket under each supported profile, with only the Annex–P01 spur shared;
- exact one-live-per-lobe and two-inert selection over a representative seed range;
- same-seed determinism and new-seed validity;
- connector-only route-evidence classification;
- North/Utility/South compatibility mapping and F1 names;
- lobe traversal producing no route evidence;
- player/Owl non-jump connectivity probes;
- no normal Owl hold node controlling two portal sight probes;
- all v0.3 perception, hearing, priority, full-cover, distraction, reset, and feedback cases;
- objective/rack/extraction/reset regressions.

## Required Runtime Cases

- Complete Annex → Containment → North → Coolant → Utility → Fuel → South → Containment → Annex.
- Complete the reverse circuit.
- Reach every socket pocket and leave by a second exit.
- Carry a rod through every portal.
- Observe one full-cover and quiet-relocation case in each lobe.
- Trigger one portable and one fixed distraction in each lobe.
- Show route evidence on all three connectors and no route evidence during lobe-only movement.
- Demonstrate F3 OFF clears route memory; demonstrate F3 ON creates bounded, explainable pressure.
- Exercise every supported route profile.
- Confirm the Owl cannot hold two connectors from a normal patrol point.

## Stop-And-Ask Conditions

Stop and ask before:

- changing the locked topology or lobe identity;
- removing a connector, lobe loop, socket family, or two-exit pocket rule;
- altering v0.3 perception, hearing, target priority, controls, or feedback semantics;
- adding a door, hazard, new mechanic, new objective, new tool, or generalized framework;
- direct-editing Unity YAML because MCP lacks a capability;
- deleting or overwriting user-owned files;
- committing or pushing.

## Required Final Report

Use this structure:

1. Outcome and build identity.
2. Exact files/systems changed.
3. Implemented topology and any bounded coordinate adjustments.
4. Connector-evidence mapping and compatibility choices.
5. Objective distribution and seeded profiles.
6. Focused, full Edit Mode, and Play Mode test counts.
7. Console, build, launch, and smoke evidence.
8. Runtime inspection cases and screenshots.
9. Human evidence: per-session results or Not Tested.
10. XR/hardware evidence: separately stated, normally Not Tested.
11. Cuts, deviations, known risks, and next evidence-driven action.
12. Git status and explicit confirmation that no commit/push occurred.
