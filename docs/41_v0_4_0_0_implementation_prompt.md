# v0.4.0.0 Copy-Ready Implementation Prompt

Status: Approved assignment prompt

Last updated: 2026-08-02

Paste the text below into the dedicated implementation-agent conversation.

~~~text
Implement v0.4.0.0 Spatial Readability and Pursuit Topology.

The 7C three-lobed macro layout is approved and design-locked. Begin after your mandatory read-only repository and Unity MCP audit; do not ask whether the experiment is authorized.

READ FIRST

1. docs/implementation_agent/README.md
2. docs/implementation_agent/01_authority_and_scope.md
3. docs/37_v0_4_0_0_spatial_readability_scope.md
4. docs/38_v0_4_0_0_7c_level_plan.md
5. docs/39_v0_4_0_0_implementation_handoff.md
6. docs/implementation_agent/02_current_implementation_state.md
7. docs/implementation_agent/03_system_ownership_map.md
8. docs/implementation_agent/04_operating_and_validation_procedure.md
9. docs/33_v0_3_0_0_readable_stealth_scope.md
10. docs/10_technical_architecture_outline.md
11. docs/27_development_versioning.md
12. Current facility, map, route, threat, objective, and test code

AUTHORITY

- docs/37_v0_4_0_0_spatial_readability_scope.md is the player-facing implementation authority.
- docs/38_v0_4_0_0_7c_level_plan.md is the exact greybox data and topology authority.
- docs/39_v0_4_0_0_implementation_handoff.md defines task order, required tests, sequence, stop conditions, and final report.
- docs/33_v0_3_0_0_readable_stealth_scope.md remains authority for protected perception, sound, counterplay, and feedback behavior.
- Older map descriptions and concept iterations are historical. Do not blend them into 7C.
- Use v0.4.0.0 and V040 naming. “7C” is design provenance, not a development-version scheme.

VERIFIED BASELINE TO RE-AUDIT

- Published HEAD was master commit 0636f5d at v0.2.3.0.
- The current working tree contains user-owned, uncommitted v0.3.0.0 implementation and documentation.
- Recorded local v0.3 evidence is focused 22/22, full Edit Mode 133/133, Play Mode 6/6, strict Windows build success, and smoke success.
- v0.3 uninformed human validation remains Not Tested.
- Unity was 6000.5.4f1 with P01_CoreLoop open and no console errors.

Treat all current dirty and untracked files as user-owned. Re-audit before editing. Do not reset, discard, overwrite, commit, or push.

LOCKED RESULT

- Replace the current rectangular facility with a compact asymmetric circuit around an inaccessible central shielding/service void.
- Three unequal lobes: Containment & Control, Coolant & Maintenance, and Fuel Handling & Waste.
- Three pairwise connectors: North Pipe Gallery, Utility Spine, and South Maintenance Channel.
- Contractor annex, three-slot rack, and extraction remain at the outer Containment edge.
- One landmark and one complete local loop per lobe.
- Every objective pocket has two exits.
- Eight sockets: C1–C3, K1–K3, F1–F2.
- Exactly one live rod per lobe plus two inert decoys from unused sockets, deterministic by seed.
- One machinery panel and three portable metal props per lobe.
- Approximate footprint 57 m × 40 m, main floor Y 0, shallow +0.80 m overlook and optional +0.60 m gantry only.
- Seven 2.76 m-clear portals P01–P07.
- Three distinct cover/sightline grammars as documented.
- Explicit connector metadata drives route evidence. Lobe/objective traversal alone does not.
- West/Central/East may remain narrow internal compatibility IDs; F1 and signs use connector names.
- Approximately 18 Owl nodes, no normal hold point controlling two connectors, and two approaches to every lobe.
- Existing v0.3 sight, sound, evidence priority, hiding, distraction, feedback, reset, objective, and extraction behavior remains unchanged.

PROTOTYPE QUESTION

Can uninformed players form a usable mental map of the three-lobed facility and deliberately switch between its distinct connectors and cover grammars to complete the existing three-rod contract while the Owl’s sight, sound, search, and route adaptation remain explainable?

OPERATING RULES

- Use Unity MCP for all Unity Editor state, scene regeneration, serialized changes, compilation inspection, tests, screenshots, versioning, and builds.
- C#, C# tests, and Markdown may be edited as files. Return to Unity MCP after each coherent C# slice.
- Update declarative builder/specification data first, then regenerate only the disposable generated root.
- Do not hand-edit .unity, prefab, asset, material, or ProjectSettings YAML.
- Preserve all work outside the approved scope.
- Do not add new mechanics, objectives, enemies, doors, hazards, tools, inventory, procedural rooms, final art, hardware, or a generalized mission framework.
- Do not alter v0.3 stealth tuning or semantics unless a release-blocking regression is proven and reported.
- Do not commit or push unless separately requested.

EXECUTION

Follow V040-001 through V040-009 in docs/39_v0_4_0_0_implementation_handoff.md.

Build in risk order: baseline → data/graph → connector semantics → generated shell/NavMesh → lobe grammars → content/objective distribution → Owl graph/sightlines → telemetry/runtime inspection → regression/version/build/evidence.

Validate every supported route profile and preserve at least two graph-distinct non-jump routes from the Containment side of P01 to every active socket; the Annex–P01 spur is the only permitted shared access edge. Run focused new tests, the focused v0.3 regression set, all Edit Mode tests, and all Play Mode tests. Produce and smoke-test a fresh v0.4.0.0 Windows build.

Use docs/40_v0_4_0_0_human_validation_sheet.md for human sessions. Keep automated, operator desktop, uninformed desktop, XR, venue hardware, physical effects, and heart-rate evidence separate.

At delivery, use the exact Required Final Report in docs/39_v0_4_0_0_implementation_handoff.md. Report test counts, console/build/smoke evidence, topology, coordinate deviations, seeded profiles, connector-evidence behavior, inspection cases, human status, cuts, risks, and Git state.
~~~
