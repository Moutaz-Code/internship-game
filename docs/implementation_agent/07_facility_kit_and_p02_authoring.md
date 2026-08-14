# Approved Facility P02 and Facility Kit Authoring Guide

Status: first playable greybox source and extension guide

Last verified: 2026-08-11

This guide covers the separately generated approved-facility shell and the modular kit used to build it. It does not change the authority or ownership rules in this folder. `P01_CoreLoop`, the v0.4 builder, protected gameplay behavior, objectives, and extraction selection remain outside this generator's ownership.

## Open and rebuild

- Playable traversal scene: `Assets/_Project/Scenes/P02_ApprovedFacilityShell.unity`
- Palette scene: `Assets/_Project/Scenes/FacilityKit_Palette.unity`
- Rebuild kit and palette: `Prototype > Facility Kit > Rebuild Kit Prefabs and Palette`
- Rebuild P02: `Prototype > Approved Facility > Rebuild P02 Traversal Shell`

Run the kit rebuild before the P02 rebuild after changing a catalog entry or generated prefab recipe. Both actions are idempotent. P02 regeneration may replace only the exact scene root `_Generated P02 Approved Facility`. Put hand-authored additions outside that root. Anything placed inside it is disposable.

The scenes are intentionally not added to Build Settings. Open them directly from the Project window. Do not replace `P01_CoreLoop` or change build identity as part of facility authoring.

## Durable source and generated output

Durable source:

- `Assets/_Project/Scripts/Environment/ApprovedFacilityLayout.cs` — image-space rooms, blocked zones, wall centerlines, portal gaps, anchors, hero placements, and transform.
- `Assets/_Project/Scripts/Environment/FacilityKitCatalog.cs` — stable module IDs, categories, bounds, prefab paths, and cover classes.
- `Assets/_Project/Scripts/Environment/FacilityModuleMetadata.cs` — prefab metadata and selected-bounds gizmos.
- `Assets/_Project/Scripts/Environment/FacilitySemanticAnchor.cs` — semantic anchor identity and gizmos.
- `Assets/_Project/Scripts/Environment/OneWayTraversalController.cs` — reusable one-way traversal policy and state.
- `Assets/_Project/Scripts/Editor/FacilityGeneration/FacilityKitAssetBuilder.cs` — generated-prefab and palette recipes.
- `Assets/_Project/Scripts/Editor/FacilityGeneration/ApprovedFacilityBuilder.cs` — kit-based shell assembly.
- `Assets/_Project/Scripts/Editor/FacilityGeneration/P02ApprovedFacilitySceneBuilder.cs` — separate scene and protected-gameplay adapters.

Generated but reusable assets:

- `Assets/_Project/Prefabs/Environment/FacilityKit/`
- `Assets/_Project/Materials/FacilityKit/`
- `Assets/_Project/Scenes/FacilityKit_Palette.unity`

Disposable scene instances are descendants of `_Generated P02 Approved Facility`. Rebuild source assets or the catalog instead of repairing generated instances by hand.

## Source-image transform and dimensions

Topology source: `LevelDesign/InternshipHorrorMap_Options_2026-08-11/InternshipHorrorMap_locked.png`

- Source SHA-256: `857275437965E59390FBE8939B8D0844FE404FB4DB77F94042F7A21BA1DC7969`
- Image dimensions: 2048 x 2048 pixels; the aspect ratio is unchanged.
- Image origin `(1024, 1024)` maps to world origin `(0, 0, 0)`.
- World yaw: 0 degrees. Image right is world `+X`; image up is world `+Z`; world `+Y` is vertical.
- Uniform scale: `0.025 m/pixel`, making the full reference extent 51.2 m square.
- Conversion: `X = (imageX - 1024) * 0.025`; `Z = (1024 - imageY) * 0.025`.
- Wall thickness: 0.30 m, centered on declared centerlines.
- Wall height: 3.60 m.
- Primary portal clearance: 2.76 m wide x 2.52 m high.
- Minimum carried-object clearance target: 1.80 m.
- Reactor center: image `(586, 906)`, world `(-10.95, 0, 2.95)`.
- Reactor source radius: 294 pixels = 7.35 m.

An `ApprovedFacilitySourceReference` component records these values on the generated shell. The editor-only reference material and source-reference gizmos are excluded from runtime geometry.

## Kit conventions

All kit roots use local `+Z` as forward, local `+Y` as up, metre units, stable `FK-*` identifiers, and an origin at the floor/contact plane unless a wall module requires a centerline pivot. Architecture uses a 1 m length cadence; use 1 m position snapping, 90-degree rotation snapping for orthogonal construction, and 45-degree rotation snapping only for declared angled runs. Never non-uniformly scale doors, windows, cover, or gameplay machinery. Use a catalog length variant instead.

Prefab folders:

- `Architecture` — 1 m, 2 m, and 4 m walls; inside/outside corners; 45-degree wall; floor and ceiling; end cap; column; trim; portal frame; 12-degree reactor segment; blocked mass.
- `DoorsAndWindows` — sliding bulkhead, service door, inactive locked door, three one-way presentations, reinforced observation window, narrow service window, and reactor lead glass.
- `Cover` — straight and corner full-cover shielding.
- `Machinery` — consoles, assay pieces, coolant basin, pumps, pipe clusters, scrubber, duct bank, casks, crane, lockers, tool cage, workbench, parts cart, and portable-metal storage.
- `RoomIdentity` — contractor decon frame and intake.
- `Extraction` — vehicle airlock, maintenance lift, and emergency egress surrounds.

Select a module to see its authored bounds and cover-class gizmo. Full cover is orange, perception-transparent is cyan, partial cover is yellow, and ordinary modules are blue-grey. Ceilings remain in the separate `Ceilings (Toggleable For Overhead Inspection)` hierarchy so they can be disabled for overhead work.

## Drag-and-drop assembly

1. Place walls on a 1 m grid with their pivot on the intended wall centerline and their local `+Z` pointing into the route direction used by the module.
2. Use inside/outside corners rather than overlapping long walls. Use the 45-degree module only where the plan calls for an angled segment.
3. Leave the full declared portal gap before adding a portal or door frame. The frame's clearance gizmo is the body-and-rod keep-out volume.
4. Place observation windows as collision-bearing boundaries. The glass child uses `PerceptionTransparent`, so it blocks bodies and carried objects while remaining outside the Owl's opaque-vision mask.
5. Keep cover modules at authored scale and verify every protected sight sample is occluded; ordinary glass is never full cover.
6. Create new presentation variants through Unity's prefab-variant workflow, retain the stable root metadata, and give a genuinely new module a unique catalog ID and path. Do not duplicate a generated prefab and silently reuse its ID.

Ordinary manual room building never requires editing the P02 coordinate arrays. The declarative arrays exist only for reproducible regeneration of the locked map.

## One-way traversal setup

The three palette examples share `OneWayTraversalController`; pressure bulkhead, crash-bar security door, and maintenance valve are presentations, not separate rule implementations.

- Local `+Z` is the allowed traversal direction and is shown by the selected gizmo. Rotate the prefab root to set direction; do not invert child meshes.
- `Player Permission` gates the player.
- `Owl Policy` is `FollowPlayerRule`, `AllowBothDirections`, or `AlternateRouteOnly`. Use `AlternateRouteOnly` only when a tested pursuit route exists.
- `Initial State` is Open, Closed, or Locked. Reset returns to that state.
- `Close After Pass` and `Close Delay Seconds` schedule closing after passage.
- `Clearance Volume` and `Obstruction Layers` prevent closing while the player, hands, Owl, or carried object occupies the throat.
- `Allowed`, `Denied`, `Opened`, and `Closed` events are presentation/audio hooks. Denial feedback is not coupled to a particular mesh.

The allowed side is the negative side of the controller's direction plane, moving toward local `+Z`. Confirm the handle/control/light cue is readable from both approaches before using a variant. No one-way prefab is placed in P02 yet because placement would change approved macro traversal; the examples remain active only in the palette.

## Navigation and rebuilding

P02 bakes its `NavMeshSurface`, adds one bidirectional `NavMeshLink` for each of the 17 declared portals, and creates an explicit 18-node Owl fallback graph. After manual changes, rebuild P02 through the menu, then verify player clearance physically and rebake through the same builder so the physical route, NavMesh links, and Owl graph stay aligned.

Do not move a portal, block a connector, change Owl access, or add a one-way restriction without approval. A manual prop should not overlap a portal clearance volume or either end of a navigation link.

Current automated evidence finds 810 baked vertices, all 17 portal links, no wall collider in any declared portal throat, and an 18/18 connected Owl fallback graph. Direct single-surface NavMesh sampling reaches 9 of the 18 patrol nodes from the current start; the links and fallback graph cover the declared route graph, but full Owl circulation across every link and XR traversal still require human validation.

## Bounded conversion deviations

These are the complete known geometry/conversion accommodations in this first pass:

1. Hand-drawn strokes are treated as wall centerlines and receive 0.15 m physical thickness on each side.
2. Walls are emitted as 1 m modules. At a declared portal, only intersecting modules are removed, bounding a cut to the smallest wall module. The clearance box uses half-width `max(0.90 m, portalWidth/2 - 0.13 m)`, half-height `portalHeight/2 - 0.06 m`, and 0.72 m depth. At compact approved junctions, an adjacent frame collider may be disabled only inside the shared throat.
3. Every declared portal receives a 2.76 m-wide, 4.0 m-long, 0.12 m-thick floor bridge whose top is coplanar at `Y=-0.10 m`. These bridges connect semantic floor polygons without adding wall openings.
4. The entrance/decon semantic floor envelope overlaps adjoining approved circulation so the existing wall openings remain walkable. Containment is triangulated as four bounded floor areas, including the source-visible south-central passage `(1098,1100)` to `(1232,1475)`. Blocked polygons and wall centerlines still carve physical boundaries.
5. The source opening between the two southern hatched masses remains open; no closing segment from `(1195,1170)` to `(1121,1157)` is generated.
6. The reactor circle is a deterministic 30-segment, 12-degree ring. Four facility-facing east segments are replaced by exactly one 6.2 m-wide lead-glass window, covering the resulting approximately 6.0 m chord. Faceting differs from the drawn circle by at most about 0.04 m chord sagitta; the core center and 7.35 m radius are unchanged.
7. Large room-identity modules, the tool cage, parts cart, and the fuel patrol node are placed away from portal clearance volumes. These are fixture/navigation placements, not changes to locked walls or openings.

No other locked wall, connector, doorway, blocked zone, room silhouette, or reactor center/radius is intentionally changed.

## Inactive placeholders and current limits

`ANCHOR-OBJECTIVE-A/B/C` and `ANCHOR-EXTRACTION-A/B/C` are present, named, and inactive. P02 uses a zero-rod traversal harness only to activate the existing run/Owl loop; it does not replace objective distribution or extraction selection. Activate or adapt these markers only after explicit approval.

Human desktop circulation, physical rod-carry traversal, and headset/XR validation are not yet recorded. Treat them as Not Tested even when automated clearance, component, and graph tests pass.
