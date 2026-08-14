# Approved Map Shell and Modular Facility Kit — Copy-Ready Implementation Prompt

Paste everything inside the block below into the dedicated Unity implementation-agent conversation. Attach the original map image if that conversation cannot access the workspace.

~~~text
Implement the first playable 3D base of the newly approved nuclear-facility map and, as part of the same work, create a small reusable modular environment kit that I can use to extend or rebuild spaces myself later.

This is an authorized implementation task. It supersedes older topology descriptions only for this new map workstream. It does not authorize destroying the delivered v0.4 scene, rewriting protected gameplay systems, committing, pushing, or treating concept art as exact geometry.

OUTCOME

Deliver two connected results:

1. A separate playable Unity scene or safely selectable generated variant that reproduces the approved map shell, is navigable by the player and Owl, and supports the existing extraction-horror verbs.
2. A reusable modular facility kit—prefabs, simple materials, editor-safe generators or catalogs, documentation, and a palette/demo scene—used to construct that map rather than created afterward as unrelated examples.

The finished first pass should be an art-directed greybox: geometrically faithful and functionally readable, with enough large industrial silhouettes to communicate each room, but without a final-art or decorative-clutter pass.

MANDATORY FIRST STEP: AUDIT

Before editing, perform a read-only audit of:

- Git status, branch, HEAD, ignored/untracked state, and existing user changes;
- the connected Unity project, Unity version, open scene, Editor mode, console, and active generated root;
- `level_design_agent_handoff_prompt.md`;
- `docs/implementation_agent/README.md` and its authority, ownership, and operating-procedure documents;
- `docs/10_technical_architecture_outline.md`;
- `docs/33_v0_3_0_0_readable_stealth_scope.md`;
- `docs/37_v0_4_0_0_spatial_readability_scope.md`;
- `docs/38_v0_4_0_0_7c_level_plan.md`;
- `docs/39_v0_4_0_0_implementation_handoff.md`;
- the current `V040FacilityBuilder`, `PrototypeSceneBuilder`, map/route definitions, NavMesh setup, objective anchors, Owl nodes, distraction components, prefabs, materials, tests, and naming/folder conventions.

Re-audit all recorded state rather than assuming old counts or build evidence are still current. Treat every existing edit and untracked file as user-owned. Do not reset, discard, clean, overwrite, commit, or push.

UNITY AND PROJECT BOUNDARIES

- Project: `UnityProject/InternshipGameSynced`
- Verified Unity version: 6000.5.4f1
- Existing main scene: `Assets/_Project/Scenes/P01_CoreLoop.unity`
- Use Unity MCP for scene, GameObject, prefab, material, serialized-asset, NavMesh, test, capture, and build operations.
- C#, C# tests, and Markdown may be edited as files; return to Unity after each coherent slice to compile and inspect the console.
- Never hand-edit `.unity`, `.prefab`, `.asset`, material, or ProjectSettings YAML.
- Preserve `P01_CoreLoop` and the existing v0.4 builder/result. By default, make a separate scene or selectable layout definition for this map. A sensible name is `P02_ApprovedFacilityShell`, but follow existing conventions and report the actual name. Stop and ask before replacing the active delivered scene.
- Do not assign a release version, change build identity, or produce a release build unless I separately approve it.
- Do not add packages or external dependencies unless an existing project capability is demonstrably insufficient and I approve the dependency.

SOURCE AUTHORITY

Use these sources in this order:

1. My newest explicit instructions.
2. `LevelDesign/InternshipHorrorMap_Options_2026-08-11/InternshipHorrorMap_locked.png` — exact 2048 × 2048 topology authority. Its recorded SHA-256 is `857275437965E59390FBE8939B8D0844FE404FB4DB77F94042F7A21BA1DC7969`.
3. `LevelDesign/InternshipHorrorMap_Options_2026-08-11/artistic_room_themes.md` — room identity, material family, landmark, and horror-direction authority.
4. `LevelDesign/InternshipHorrorMap_Options_2026-08-11/artistic_room_themes_composite.png` — semantic placement reference.
5. `LevelDesign/InternshipHorrorMap_Options_2026-08-11/artistic_facility_concept_generated.png` — mood and large-form inspiration only. It is generative concept art and is not topology authority.
6. Existing project architecture and protected gameplay behavior.

Do not infer approved wall geometry from the generative concept. When it conflicts with the locked PNG, the locked PNG wins.

MAP-SHELL RULES

Reconstruct the approved shell in 3D without intentionally changing:

- the outer silhouette;
- existing interior walls;
- doorway and wall-opening locations;
- room shapes and relative proportions;
- connectors;
- blocked or hatched areas;
- the large circular reactor's shape and relative position.

Implementation necessarily converts a hand-drawn plan into metric geometry. Make that conversion explicit and reproducible:

- retain the source image at its original aspect ratio;
- use one uniform pixel-to-meter scale—never stretch X and Z independently;
- calibrate the scale from comfortable VR portal widths and the current player, Owl, carried-rod, and NavMesh dimensions;
- use the existing 2.76 m primary portal width and 3.60 m wall height as initial references where they remain compatible;
- record world origin, rotation, uniform scale, wall thickness, wall height, portal dimensions, and every bounded deviation;
- if practical, store wall centerlines, room polygons, portal gaps, and blocked zones in a declarative layout definition using original image-space or normalized coordinates, then convert them to world space through one documented transform;
- place a hidden editor-only reference plane or gizmo for alignment if useful, but exclude it from runtime and builds.

Do not smooth away meaningful corners, invent connections, close existing openings, or turn blocked hatching into playable floor. Small coordinate adjustments are allowed only for wall thickness, collider stability, modular snapping, player/Owl clearance, or NavMesh reliability; report them.

The large circular reactor is an inaccessible reactor core:

- no player route, Owl route, door, hatch, teleport, NavMesh surface, objective, or collectible inside it;
- give it continuous physical collision and a clear visual boundary;
- create exactly one broad lead-glass observation window where the reactor boundary faces/intersects the playable facility;
- the window blocks bodies and carried objects but is not opaque full cover; configure perception layers deliberately rather than relying on accidental collider behavior.

The objective and extraction labels in the source drawing are provisional gameplay markers whose surrounding room geometry is fixed. Create clearly named semantic anchors/groups for A, B, and C, but do not silently change the active objective distribution or extraction-selection logic. Preserve current contract behavior unless this task explicitly requires an adapter. Any newly placed inactive anchors must remain inactive and clearly documented.

ROOM IDENTITIES

Use architecture and large silhouettes—not tiny signs or clutter—to establish these identities:

- Bottom entrance: Contractor Decontamination and Dispatch. Blue-grey modular decon frames, intake equipment, lockers, and contractor staging.
- Bottom-left Objective A room: Radiological Assay Laboratory. Stainless work surfaces, pale clinical-green accents, sample isolation equipment, and thick observation glass.
- Top Objective C room: Reactor Control and Instrumentation Gallery. Broad analog control banks mixed with newer contractor modules and restrained amber instrumentation.
- Central playable volume: Containment Shield Gallery. Warm-grey brutalist concrete, thick lead shielding fins, exposed crossings, and dependable recovery cover.
- Upper-right complex: Primary Coolant Pump Hall. A circular pump/basin landmark, pipe trunks, oxidized machinery, wet cyan accents, and frequent short sight breaks.
- Right-middle service rooms: Air Filtration and Scrubber Service. Vertical filter vessels, ducts, condensation, and muted chemical-green accents.
- Lower-right Objective B room: Spent Fuel Transfer and Waste Handling. Ordered shielded-cask lanes, lateral crosscuts, overhead handling forms, ochre/rust material accents, and restrained hot-zone markings.
- Bottom-center adjoining spaces: Tool Crib and Maintenance Stores. Caged storage, lockers, parts carts, and plausible portable-metal distraction sources.
- Extraction A: broad vehicle airlock identity.
- Extraction B: tighter maintenance-lift identity.
- Extraction C: remote emergency-control-egress identity.

Keep the age-12 horror direction: institutional nuclear brutalism under improvised contractor maintenance. Suggest interrupted work, patched systems, corrosion, condensation, and procedural abandonment. No gore, bodies, excessive darkness, illegible navigation, lore-text dependency, or random clutter.

MODULAR-KIT REQUIREMENTS

Do not build the scene as a single fused mesh or an unstructured pile of bespoke cubes. Create reusable prefabs and use them in the actual map. Follow current project conventions; if none exist, use a clear structure similar to:

`Assets/_Project/Prefabs/Environment/FacilityKit/`

- `Architecture/`
- `DoorsAndWindows/`
- `Cover/`
- `Machinery/`
- `RoomIdentity/`
- `Extraction/`

Use consistent naming, pivots, forward direction, scale, layers, tags, static flags, and hierarchy. Document all conventions. Prefer nested prefabs and variants over duplication.

Minimum architecture kit:

- straight wall modules in a small practical length set;
- inside and outside corners;
- diagonal/angled wall modules needed by the plan;
- floor modules or reliably generated floor polygons;
- ceiling modules, kept separable or toggleable for overhead inspection;
- end caps, structural columns, wall trims, and portal/bulkhead frames;
- curved reactor-wall segments or a deterministic arc generator with stable colliders;
- blocked-zone/inaccessible-mass modules;
- standard reinforced observation window, narrow service window, and the broad reactor lead-glass window;
- a clear material split between ordinary walls, full-cover shielding, glass, doors, floors, hot-zone boundaries, and editor-only reference geometry.

Minimum door and valve kit:

- standard manual or automatic sliding bulkhead;
- hinged personnel/service door if compatible with current interaction code;
- locked/inactive door presentation variant for non-routes;
- at least three diegetically distinct one-way traversal variants, such as a pressure bulkhead, crash-bar security door, and maintenance pressure valve.

Implement one-way behavior as a reusable data-driven traversal component separated from presentation. It must expose at least:

- allowed traversal direction, visualized with an editor gizmo;
- player permission;
- Owl permission or navigation policy;
- initial/open/closed/locked state;
- close-after-pass behavior and timing;
- safe obstruction handling for the player, hands, and carried rod;
- denial feedback from the forbidden side;
- reset behavior;
- events/hooks for sound and animation without coupling to a specific mesh.

The different door prefabs should reuse that behavior instead of each implementing its own rules. A one-way valve must be legible before commitment through handles, controls, frame shape, light state, or view through the door. Do not implement it as an unexplained invisible blocker. Do not force-move the player or camera, crush the player, trap an objective in a terminal pocket, create a universal safe route, or permanently exclude the Owl without a documented alternate pursuit route. Navigation and route graphs must agree with the physical result.

Minimum gameplay/identity kit:

- two or more thick containment-shield/full-cover modules;
- control-console straight and corner modules;
- assay bench and isolation-cabinet modules;
- circular coolant basin/pump landmark;
- several pump/pipe-cluster variants with meaningfully different silhouettes;
- vertical scrubber/filter vessel and duct-bank modules;
- individual shielded cask plus row/end-cap/crosscut-friendly variants;
- overhead handling/crane silhouette;
- lockers, tool cage, workbench, parts cart, and portable-metal storage modules;
- extraction surrounds for vehicle airlock, maintenance lift, and emergency egress;
- adapters or anchors for existing rod sockets, machinery distractions, zone triggers, extraction, Owl nodes, and cover/vision semantics where required.

Use primitive or deterministic generated meshes and restrained URP materials for this pass. Keep materials instancing-friendly and geometry appropriate for VR. Do not chase final textures, bespoke decals, unnecessary small props, shaders, LOD systems, or external art pipelines unless profiling demonstrates a need.

AUTHORING FOR ME TO USE LATER

Provide a simple, discoverable authoring workflow:

- a prefab palette/catalog scene showing every module upright, named, and grouped by category;
- grid and rotation-snapping recommendations;
- visible editor gizmos for module bounds, door forward/allowed direction, portal clearance, cover classification, and gameplay anchors where useful;
- one concise Markdown guide explaining folder structure, naming, pivots, snapping, how to place walls/corners/doors/windows, how to create prefab variants, how to configure a one-way valve, how to rebake/update navigation, and how to rebuild the generated map without damaging manual content;
- a clear distinction between durable source prefabs/layout data and disposable generated scene instances;
- no hidden magic that requires editing hard-coded coordinate arrays for ordinary drag-and-drop use.

If an editor menu or builder action is added, make it idempotent: rebuilding twice produces the same hierarchy and does not duplicate objects. Regeneration may replace only its explicitly owned generated root. Never delete manual content outside that root.

GAMEPLAY AND VR CONSTRAINTS

- Measure all critical clearances against the actual player capsule/XR rig, Owl agent, rod dimensions, hand reach, and NavMesh settings.
- Primary routes and portals must comfortably support carrying one rod without hand/wall collisions.
- Avoid sustained crouching, mandatory squeezing, forced camera motion, and blind threshold collisions.
- Full cover must be physically and visually thick enough to block every protected visibility sample.
- Transparent windows are not full cover unless deliberately configured otherwise.
- Each exposed commitment should lead toward dependable cover or another recovery option.
- Objective pockets must be pass-through or offer two escape directions.
- No normal Owl position may control every escape from an objective or both available connectors.
- One-way valves may alter local circulation but may not remove the facility's useful loops, make a terminal trap, or become a guaranteed Owl-reset exploit.
- Preserve existing sight, sound, confirmed-visual priority, quiet-relocation, portable-metal, fixed-machinery, scanning, carrying, deposit, reset, and extraction semantics.

IMPLEMENTATION ORDER

1. Baseline audit and preservation boundary.
2. Map reference transform, metric calibration, declarative shell data, and topology tests.
3. Architecture prefab kit and palette scene.
4. Separate map scene/layout variant assembled from the kit.
5. Reactor boundary and observation window.
6. Door/window framework and one-way-valve prefab variants, initially placed only where they do not silently redefine the approved topology; use inactive palette examples if placement is not approved.
7. Large room-identity silhouettes and dependable cover.
8. Existing gameplay anchors, route graph, Owl graph, NavMesh, and current contract adapters.
9. Automated tests, runtime inspection, screenshots, and authoring guide.

Prefer a complete, traversable shell and durable kit over polishing one room. Do not hide incomplete topology behind art detail.

AUTOMATED AND INSPECTION REQUIREMENTS

Add focused tests or deterministic validation for at least:

- expected room, portal, blocked-zone, and reactor identifiers;
- locked topology connectivity and absence of invented reactor access;
- consistent source-image-to-world transform;
- prefab/catalog completeness and unique stable identifiers;
- idempotent generation with no duplicate roots;
- colliders and required components on wall, window, door, cover, and machinery prefabs;
- one-way traversal permission, denied-side behavior, obstruction safety, reset, and Owl-policy configuration;
- objective/extraction anchors remaining inactive or preserving existing active behavior as intended;
- player and Owl NavMesh or graph reachability through every approved route;
- two-exit objective pockets and no door/fixture overlap across required clearance zones;
- protected full-cover and distraction regressions;
- no compiler errors, missing scripts, broken prefab references, or new unexpected console errors.

Perform runtime inspections from player scale, not only overhead:

- complete the full facility circulation in both directions where approved topology permits it;
- carry a rod through every primary portal and each relevant door type;
- approach every one-way variant from both sides and verify its cue before commitment;
- confirm doors cannot close on the player or carried rod;
- confirm the Owl has valid, explainable pursuit routes and does not become stuck at door thresholds;
- check every objective room can be exited through a second direction;
- verify full cover in each major area and quiet relocation after breaking sight;
- verify the reactor has no playable or navigable entry;
- inspect lobe identity from entrances and major decision points;
- capture one overhead comparison aligned to the locked map plus representative first-person views of the entrance, reactor observation, containment, coolant, scrubber, fuel, assay/control rooms, door kit, and palette scene.

NON-GOALS

- Final environment art or production textures.
- Decorative-clutter pass.
- New enemy, objective family, weapon, inventory, hazard, or extraction mechanic.
- Procedural facility generation or generalized mission framework.
- Rewriting protected Owl perception or route-adaptation behavior.
- Moving provisional objectives/extractions without separate approval.
- Replacing the existing delivered P01 scene.
- Version bump, release build, commit, push, or PR.

STOP AND ASK BEFORE

- changing any locked wall, room, connector, doorway, blocked area, or reactor geometry beyond a documented clearance adjustment;
- choosing active objective or extraction relocations;
- placing a one-way valve where it changes macro connectivity or makes reverse traversal impossible;
- changing Owl access rules or protected stealth semantics;
- replacing P01 or removing the v0.4 builder;
- adding a dependency, importing an external asset pack, or invoking a cloud asset generator;
- deleting/overwriting user-owned files;
- changing build identity, making a release build, committing, pushing, or publishing.

DELIVERABLES

At completion provide:

1. The separate playable map scene/layout variant and its generated-root ownership boundary.
2. The modular prefab kit and palette/catalog scene.
3. The source-image-to-world transform and declarative shell/layout data.
4. Door/window/one-way-valve components and prefab variants.
5. First-pass room-identity and cover/machinery prefabs used by the actual map.
6. NavMesh/route/Owl/objective integration required for a meaningful traversal test.
7. The authoring and extension guide for future manual work.
8. Automated test results, console status, runtime cases, and screenshots.
9. A list of every coordinate deviation, cut, inactive placeholder, and known risk.
10. Git status and explicit confirmation that no commit or push occurred.

FINAL REPORT FORMAT

Lead with the playable outcome. Then report:

- scene/layout created and how to open or rebuild it;
- exact files and systems changed;
- topology, metric scale, dimensions, and deviations;
- modular-kit inventory and conventions;
- one-way-valve semantics and current placements;
- room identities and hero silhouettes implemented;
- gameplay, Owl, NavMesh, objective, and extraction integration;
- focused/full test counts and console result;
- runtime inspection and screenshot evidence;
- human/XR evidence, explicitly marked Not Tested unless actually observed;
- cuts, risks, and recommended next pass;
- Git status and no-commit/no-push confirmation.
~~~
