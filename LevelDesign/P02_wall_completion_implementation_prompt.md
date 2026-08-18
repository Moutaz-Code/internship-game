# P02 Approved Facility Shell — Wall-Completion Implementation Prompt

Paste everything inside the following block into the dedicated implementation-agent conversation.

~~~text
Continue and complete the modular wall layout in `P02_ApprovedFacilityShell` by tracing the approved zoned floor plan and extending the wall-placement philosophy already established manually in the Entrance, Room 2, and Room 3.

This is an authorized Unity implementation task. Do not ask whether wall completion itself is approved. Stop only for the explicit ambiguity and scope conditions listed below.

PRIMARY OUTCOME

Complete the architectural wall shell represented by the approved floor plan while:

- preserving the user's existing manual Entrance, Room 2, and Room 3 work;
- following the same modular placement grammar, orientation, mirroring, thickness treatment, and corner construction;
- preserving all planned openings, hallways, inaccessible regions, room proportions, and connectivity;
- keeping prefab links intact wherever the existing workflow uses prefab instances;
- making only the smallest bounded deviations required by real wall thickness and modular dimensions;
- recording every meaningful deviation instead of silently redesigning the map.

This pass is wall-shell production, not a gameplay, decoration, lighting, furnishing, or final-art pass.

SCENE IDENTITIES — NON-NEGOTIABLE

Three scenes are currently loaded in the Unity Editor:

1. `Assets/_Project/Scenes/P02_ApprovedFacilityShell.unity`
   - This is the main working scene.
   - It is the only scene you may modify or save during this assignment, apart from creating the explicit baseline backup described below.
   - It must remain the active destination for all new GameObjects.

2. `Assets/_Project/Scenes/Showcases/CreepyCat_Walls_Showcase.unity`
   - This is a read-only inventory and dimension-reference scene.
   - Use it to inspect available wall prefabs and compare their silhouettes and measurements.
   - Do not modify, dirty, save, reorganize, or copy its showcase infrastructure into P02.

3. `Assets/Imports/Creepy_Cat/3D Scifi Kit Starter Kit_HD/Example_01.unity`
   - This is a read-only construction-inspiration scene made by the asset creator.
   - Study it for intended prefab orientation, snapping, visible faces, corner construction, trim continuity, module combinations, seam treatment, columns, portholes, consoles, pipes, and material rhythm.
   - Do not copy its topology, rooms, corridor widths, doors, circulation, lighting, prop density, or overall layout.
   - Never modify or save `Example_01`.

Because all three scenes are loaded additively, never rely only on whichever scene happens to be active at a given moment. Explicitly verify that every created object belongs to `P02_ApprovedFacilityShell`. Inspect loaded-scene dirtiness regularly. At delivery, the showcase and example scenes must remain clean.

MANDATORY BASELINE CHECKPOINT

The latest read-only audit found `P02_ApprovedFacilityShell` dirty with unsaved user work. Before placing, moving, deleting, renaming, or reparenting anything:

1. Audit the loaded scenes, active scene, Git status, console, current P02 hierarchy, and scene dirtiness.
2. Confirm that the user's Entrance, Room 2, and Room 3 objects are present under `Rooms`.
3. Save the current dirty P02 scene to its existing path so the user's latest manual work is persisted.
4. Immediately create a save-as-copy baseline:
   `Assets/_Project/Scenes/Backups/P02_ApprovedFacilityShell_ManualBaseline_2026-08-14.unity`
5. Keep `P02_ApprovedFacilityShell` as the active working scene after creating the copy.
6. Verify the baseline copy exists before continuing.

Do not overwrite an existing backup with that name. If it already exists, add a bounded numeric suffix and report the exact path.

Do not reset, discard, revert, clean, commit, push, publish, or rewrite Git history. Treat all existing modified and untracked files as user-owned.

SOURCE AUTHORITY

Use sources in this order:

1. The user's newest explicit instructions.
2. The floor-plan plane already placed in `P02_ApprovedFacilityShell` and the zoned image `InternshipHorrorMap_Zoned.png`.
3. The user's existing Entrance, Room 2, and Room 3 wall construction in P02.
4. The topology constraints in the original approved map shell.
5. `Example_01` for asset-author construction inspiration only.
6. `CreepyCat_Walls_Showcase` for inventory, names, dimensions, and direct visual comparison.

When sources conflict:

- The zoned floor plan wins for topology.
- Existing Entrance/Room 2/Room 3 work wins for this project's placement method.
- `Example_01` never overrides topology or the user's established placement grammar.
- The showcase never becomes a source of layout decisions.

ZONED PLAN SEMANTICS

- Blue-shaded spaces are playable hallways/connectors and must remain clear.
- Black-hatched spaces are non-player-accessible and must not become circulation or rooms.
- Named and numbered areas identify room or spatial groups.
- Existing wall openings and doorway gaps are intentional unless the user explicitly states otherwise.
- Do not infer additional meaning from the colors of handwritten room numbers. Only the explicitly stated blue-hallway and black-inaccessible semantics are authoritative.
- The large circular reactor remains inaccessible.
- Do not introduce a door, hatch, shortcut, NavMesh route, or player space into the reactor.

ESTABLISHED USER PLACEMENT GRAMMAR

Before continuing, inspect the user's existing work directly and write a short internal placement record covering module choice, transform spacing, visible face, corner construction, mirroring, and floor-plan offset. Continue from that evidence rather than reconstructing a new standard.

The latest audit found the following repeatable tendencies; verify them in the current Editor state:

- `Rooms/Entrance` contains 34 direct placed roots, predominantly the `Wall_Simple_01` family.
- `Rooms/Room 2` contains 25 direct placed roots and four custom 45-degree corner roots.
- `Rooms/Room 3` contains 11 direct placed roots and four custom 45-degree corner roots.
- Long straight runs generally use approximately six-metre module increments.
- Wall bases sit consistently around world/local `Y = 0.5`, depending on hierarchy context.
- Orthogonal walls use 90-degree rotations.
- Mirrored variants commonly use negative X scale where needed to present the correct face.
- Half, coin, inverted-coin, squared, and long variants terminate or transition wall runs.
- Custom Blender-made `eighth_column` and `inverted_eighth_column` pieces are used for 45-degree transitions at their established imported scale, approximately `0.01`.
- Existing 45-degree assemblies may combine the custom corner pieces with deliberately rotated wall meshes.

Treat those values as observed starting evidence, not permission to normalize all transforms globally. Sample adjacent manual pieces before continuing each run.

PLACEMENT RULES

- Extend from existing walls whenever possible instead of independently retracing both ends of a run.
- Duplicate an established neighboring module or assembly when it already solves the required condition.
- Use the original imported wall prefabs without modifying their source assets.
- Preserve prefab connections; do not unpack instances merely to make placement easier.
- Reuse the existing custom 45-degree assemblies. Do not reopen Blender, recut meshes, or create another angled-wall family unless the current pieces demonstrably cannot solve a required junction.
- Preserve the imported scale of custom Blender pieces.
- Keep long modules at their natural scale whenever possible; solve length differences with long, half, corner, or termination variants rather than arbitrary non-uniform stretching.
- Use mirroring only in the same manner already demonstrated by the user.
- Match the neighboring wall's intended visible face, trim direction, vertical position, and seam rhythm.
- Do not replace the user's selected wall family with a different aesthetic family because another prefab appears more convenient.
- Do not alter the transform, scale, rotation, material, mesh, or collider of the floor-plan reference.
- Place all new walls under `Rooms/<room name or number>` or an explicitly named `Hallways/<hallway name or number>` group in P02.
- Use stable descriptive hierarchy names. Do not leave large numbers of ambiguous `(Clone)` objects.
- Keep room and hallway ownership understandable even when two surfaces occupy the same architectural boundary.

WALL-THICKNESS POLICY

The source drawing is a conceptual line plan and does not fully account for physical wall thickness. Pixel-perfect centering is not the goal when it would damage playable space.

When thickness creates a conflict, preserve the design in this order:

1. Doorways and intentional wall openings.
2. Blue hallway and connector clearance.
3. Room connectivity and escape directions.
4. Recognizable room shape and relative proportion.
5. Exterior silhouette fidelity.

Placement guidance:

- Infer whether the user's existing wall face, center, or opposite face follows the plan line at nearby completed boundaries; continue that local convention.
- At a hallway boundary, preserve the playable blue width by shifting thickness away from the hallway whenever possible.
- At a black inaccessible boundary, absorb thickness into the inaccessible side whenever possible.
- At a shared room boundary, back-to-back/double walls are permitted when each room needs its own inward-facing modular surface.
- Double walls must be intentionally offset back-to-back, not unknowingly coplanar.
- Avoid z-fighting and exact collider duplication.
- Do not narrow a critical opening merely to make exterior faces align perfectly.
- Small gaps hidden inside non-playable thickness are preferable to blocking or narrowing a required route.
- Do not use decorative columns to conceal an unresolved topology mistake.

The earlier audit found one exact duplicate-transform group in Room 2. Inspect it visually. Do not automatically delete it: determine whether it is accidental duplication, layered construction, or an incomplete assembly. Stop and ask before removing an existing user object.

WORK SEQUENCE

### Phase 1 — Baseline and grammar

- Complete the mandatory checkpoint.
- Inventory every direct child of Entrance, Room 2, and Room 3.
- Record frequently used prefab sources, common module spacing, wall elevation, mirroring, corner assemblies, and local plan-line offset.
- Inspect relevant construction examples in `Example_01` without modifying it.
- Inspect required candidate modules in the showcase without modifying it.

### Phase 2 — Immediate continuation pilot

- Continue from the already completed Entrance/Room 2/Room 3 region into the immediately adjoining unfinished boundary.
- Complete one small, contiguous pilot section first.
- Use only established modules and custom 45-degree assemblies.
- Compare the pilot overhead against the floor-plan plane.
- Inspect it at player height.
- Confirm with evidence that the pilot follows the user's construction grammar before expanding.

Do not ask for approval simply because the pilot is complete if it satisfies the documented rules. Continue unless a stop condition is encountered.

### Phase 3 — Room-by-room shell completion

- Progress contiguously outward from completed work rather than placing disconnected walls around the entire map.
- Create or use one parent group per room and hallway.
- Finish one bounded room/hallway group at a time.
- Preserve every doorway/opening before closing the surrounding wall runs.
- Resolve straight runs first, then orthogonal corners, then 45-degree transitions, then shared-wall thickness.
- After each group, perform an overhead comparison and a collider/clearance inspection.
- Save P02 after each completed bounded group.

### Phase 4 — Full-shell cleanup and validation

- Inspect every junction, doorway, hallway boundary, shared wall, and angled transition.
- Check all new objects belong to P02.
- Check the showcase and example scenes remain clean.
- Check the reactor and every black area remain inaccessible.
- Check that no blue hallway has been narrowed or obstructed by wall thickness.
- Check the scene from player height and overhead.
- Save the final P02 scene cleanly.

UNITY OPERATING RULES

- Use Unity MCP for scene state, GameObjects, prefab instantiation, reparenting, transforms, scene saving, screenshots, hierarchy inspection, console inspection, and validation.
- Do not hand-edit `.unity`, `.prefab`, `.asset`, `.mat`, model, or ProjectSettings YAML.
- Keep P02 active for all placement operations.
- When instantiating a prefab, explicitly place or move it into the P02 scene before editing its transform.
- Work in Edit Mode.
- Do not enter Play Mode with unsaved placement changes.
- Use bounded batches and verify after each batch rather than placing the entire facility in one uninspected operation.
- Never regenerate or delete the current `Rooms` hierarchy.
- Do not use an automated floor-plan tracer that replaces the user's authored modular construction.
- Do not modify imported Creepy Cat prefabs, meshes, materials, textures, or the two custom Blender assets.
- Do not create a generalized modular-building framework as part of this task.

VALIDATION REQUIREMENTS

After every completed room or hallway group:

- Confirm its parent scene is P02.
- Confirm its parent hierarchy is correct.
- Confirm wall bases and tops align with neighboring manual work.
- Confirm prefab scale is unchanged except for established mirroring/custom-import scale.
- Confirm seams do not visibly drift along straight runs.
- Confirm angled assemblies meet adjoining straight walls without route-blocking protrusions.
- Confirm no intentional doorway has been filled.
- Confirm no wall crosses blue hallway space.
- Confirm no new playable opening enters a black area.
- Confirm there are no exact duplicate transforms unless explicitly justified.
- Confirm there is no z-fighting or coplanar double-wall surface.
- Confirm colliders do not produce an invisible obstruction beyond the visible wall.

At major checkpoints capture:

- an aligned overhead Scene view with the floor plan visible;
- the same overhead view with the floor-plan renderer temporarily hidden only if this can be done without saving that temporary state;
- player-height views of representative straight, orthogonal, 45-degree, shared-wall, and doorway conditions.

Before final delivery:

- Walk the completed accessible shell at player height.
- Traverse every completed blue hallway in both directions.
- Pass through every intended doorway/opening.
- Inspect all 45-degree transitions closely.
- Inspect the entire outer shell overhead.
- Verify the circular reactor and black zones remain blocked.
- Run Unity scene validation.
- Check the console for errors and new warnings.
- Verify P02 is saved and clean.
- Verify `CreepyCat_Walls_Showcase` and `Example_01` are still clean.

NON-GOALS

- Objectives, extraction logic, rod sockets, Owl nodes, AI, route adaptation, NavMesh, or gameplay-system changes.
- Doors, one-way valves, windows, gates, or interactable behavior unless an existing static frame is already part of the user's wall grammar and needed to preserve an opening.
- Floors, ceilings, furnishing, cover placement, machinery, props, decals, signage, VFX, audio, final lighting, or environmental storytelling.
- Replacing the floor-plan reference.
- Rebuilding Entrance, Room 2, or Room 3.
- Optimizing or merging the imported asset pack.
- Editing the wall showcase or asset-author example scene.
- Modifying source prefabs or Blender assets.
- A release-version change, build, commit, push, or PR.

STOP AND ASK BEFORE

- moving, deleting, replacing, or materially altering any existing user-placed object in Entrance, Room 2, or Room 3;
- deciding that the exact duplicate group in Room 2 should be removed;
- changing the floor-plan transform or source image;
- closing, moving, narrowing, or inventing a doorway/opening;
- changing a blue hallway's topology or making a black area playable;
- placing a wall where the plan is genuinely ambiguous at useful Editor zoom;
- choosing between two thickness offsets that would materially change hallway clearance or room shape;
- creating or modifying a Blender mesh;
- introducing a wall family not already established in the user's completed region;
- unpacking a prefab;
- modifying or saving the showcase or example scene;
- touching gameplay systems, NavMesh, AI, objectives, extraction, versioning, builds, or Git history.

Do not stop merely because a wall does not sit exactly on a hand-drawn line. Apply the documented thickness priorities and record the bounded deviation.

DELIVERABLES

1. Saved, clean `P02_ApprovedFacilityShell` with the planned wall shell completed as far as the reference unambiguously permits.
2. A preserved baseline backup of the user's manual starting state.
3. Clear room and hallway hierarchy groups.
4. Prefab-linked modular walls following the established Entrance/Room 2/Room 3 grammar.
5. Reused custom 45-degree assemblies where needed.
6. Overhead and representative player-height validation captures.
7. A concise deviations and open-questions record.
8. Confirmation that `CreepyCat_Walls_Showcase` and `Example_01` remain unchanged and clean.

FINAL REPORT

Lead with whether the wall shell was completed and what remains ambiguous. Then report:

1. Baseline backup path and confirmation that it predates agent edits.
2. Exact P02 hierarchy groups added or extended.
3. Rooms, hallways, and wall runs completed.
4. Prefab families and custom 45-degree pieces used.
5. How the user's placement grammar was preserved.
6. Every wall-thickness deviation from the floor-plan line.
7. Every double-wall boundary and how coplanar overlap was avoided.
8. Doorway, hallway, black-zone, and reactor validation.
9. Overhead and player-height captures.
10. Unity scene-validation and console results.
11. Remaining ambiguities, cuts, and recommended next wall pass.
12. Confirmation that the showcase and example scenes remained clean.
13. Git status and explicit confirmation that no commit or push occurred.
~~~
