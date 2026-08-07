# v0.4.0.0 Locked 7C Level Plan

Status: Authoritative greybox specification

Last updated: 2026-08-02

## Purpose

This note converts the locked 7C concept into deterministic builder data. The image establishes composition and identity; the tables establish implementation values. Where the drawing and a table differ, the table wins.

## Visual References

### Locked Concept

![Locked 7C concept](_assets/v0_4_0_0_7c_locked_concept.png)

The concept image is mood and macro-composition reference. It is not a source of exact dimensions, props, labels, or path widths.

### Buildable Floor Plan

![Locked 7C floor plan](_assets/v0_4_0_0_7c_floor_plan.png)

Editable source: _assets/v0_4_0_0_7c_floor_plan.svg

### Gameplay Overlay

![Locked 7C gameplay overlay](_assets/v0_4_0_0_7c_gameplay_overlay.png)

Editable source: _assets/v0_4_0_0_7c_gameplay_overlay.svg

## Coordinate Convention

- Unity X is east/west.
- Unity Z is north/south.
- Main walking floor is Y 0.00 m.
- Coordinate origin is near the inaccessible central void.
- Plan grid is 5 m.
- Approximate playable bounding footprint is X -30 m to +27 m and Z -18 m to +22 m.
- Default structural wall height is 3.60 m.
- Default portal clear width is 2.76 m.
- Default portal clear height is 2.52 m.
- Critical carrying routes should preserve at least 1.8 m practical clearance after protrusions.

## Space Specifications

| ID | Display name | Approximate bounds X/Z | Floor Y | Primary grammar | Required landmark |
|---|---|---|---:|---|---|
| ANN | Contractor Annex | X -30 to -23, Z -5 to +3 | 0.00 | Safe operational threshold, not a hub | Rack/exit assembly |
| CON | Containment & Control | X -23 to -5, Z -8 to +16 | 0.00, overlook +0.80 | Monumental exposed gaps between large shields | Reactor drum |
| NPG | North Pipe Gallery | X -8 to +10, Z +14 to +20 | 0.00 | Quiet and exposed | Overhead pipe run |
| CLT | Coolant & Maintenance | X +8 to +27, Z +3 to +22 | 0.00 | Dense but legible short sight breaks | Coolant basin |
| UTL | Utility Spine | X +16 to +23, Z -4 to +5 | 0.00 | Short, noisy, cover-rich | Turbine/service stack |
| FHW | Fuel Handling & Waste | X +7 to +27, Z -18 to -4 | 0.00, gantry +0.60 | Structured lanes with crosscuts | Cask rows/hot-zone wall |
| SMC | South Maintenance Channel | X -6 to +10, Z -15 to -4 | 0.00 | Shielded and indirect | Shielded bend sequence |
| VOID | Shielding/service void | central irregular polygon | inaccessible | Hard occluder/non-playable | Shielding mass |

Bounds are authoring envelopes, not rectangular room requirements. Outer shells should keep the asymmetric/angled composition shown in the plan.

## Portal Thresholds

| ID | Connects | Approx. X | Approx. Z | Clear width | Notes |
|---|---|---:|---:|---:|---|
| P01 | Annex ↔ Containment | -23.0 | -2.0 | 2.76 m | Keep rack and spawn sightline clear |
| P02 | Containment ↔ North | -8.0 | +14.5 | 2.76 m | Angled threshold; framed view toward pipes |
| P03 | North ↔ Coolant | +8.5 | +18.0 | 2.76 m | Do not let basin fully block entry read |
| P04 | Coolant ↔ Utility | +18.0 | +4.0 | 2.76 m | No machinery protrusion into carrying path |
| P05 | Utility ↔ Fuel | +18.0 | -4.0 | 2.76 m | Preserve a shielded landing inside Fuel |
| P06 | Fuel ↔ South | +6.0 | -12.0 | 2.76 m | Angled; first bend prevents full connector view |
| P07 | South ↔ Containment | -5.0 | -5.0 | 2.76 m | Return view should reveal reactor silhouette |

## Landmark And Occluder Anchors

All coordinates are starting values. Local adjustments up to approximately 1 m are technical implementation choices if they preserve graph, sightline, portal, and gameplay constraints.

| ID | Type | Center X/Z | Approx. footprint | Yaw | Gameplay purpose |
|---|---|---|---|---:|---|
| L-CON-01 | Reactor drum | -15.0 / +6.0 | 10.7 m diameter | 0° | Map-scale identity and circular local loop |
| COV-CON-01 | Concrete shield | -21.0 / +12.2 | 4.9 × 0.8 m | 20° | Full cover near north approach |
| COV-CON-02 | Concrete shield | -8.3 / +6.5 | 4.2 × 0.8 m | 90° | Full cover near void edge |
| COV-CON-03 | Concrete shield | -16.2 / -4.4 | 5.8 × 0.8 m | -15° | Full cover on annex return |
| COV-CON-04 | Control shield | -9.2 / +14.3 | 3.9 × 1.5 m | -15° | Overlook cover and North preview |
| L-CLT-01 | Coolant basin | +17.0 / +12.0 | 6.4 m diameter | 0° | Lobe identity and loop center |
| MAC-CLT-01 | Pump cluster | +11.5 / +10.7 | 3.6 × 1.5 m | -20° | Short sight break |
| MAC-CLT-02 | Pump cluster | +21.5 / +10.1 | 3.6 × 1.5 m | 18° | Short sight break |
| MAC-CLT-03 | Pipe bank | +13.3 / +18.0 | 4.1 × 1.4 m | 0° | North-entry framing |
| L-FHW-01 | Fuel row | +13.0 / -7.2 | 5.8 × 1.7 m | 0° | Lane cover |
| L-FHW-02 | Fuel row | +17.0 / -10.8 | 6.7 × 1.7 m | 0° | Lane cover |
| L-FHW-03 | Fuel row | +13.0 / -14.8 | 5.8 × 1.7 m | 0° | Lane cover |
| COV-FHW-01 | Full-height shield | +18.8 / -11.0 | 0.8 × 11.8 m | 0° | Splits long lane and frames crosscuts |
| L-FHW-04 | Hot-zone boundary | +24.5 / -11.0 | 3.0 × 10.0 m | 0° | Red-striped non-playable landmark |

## Objective Sockets

| Socket | Lobe | Approx. X/Z | Pocket rule |
|---|---|---|---|
| C1 | Containment | -9.0 / +11.5 | Between control shield and reactor route; two exits |
| C2 | Containment | -19.0 / +14.0 | North shield pocket; two exits |
| C3 | Containment | -20.0 / 0.0 | Annex-side reactor loop; two exits |
| K1 | Coolant | +22.0 / +16.0 | Basin perimeter; two exits |
| K2 | Coolant | +21.0 / +9.0 | Pump-side pocket; two exits |
| K3 | Coolant | +11.0 / +14.0 | North entry loop; two exits |
| F1 | Fuel | +21.0 / -8.0 | Upper lane crosscut; two exits |
| F2 | Fuel | +21.0 / -13.0 | Lower lane crosscut; two exits |

Selection law:

- one live socket from C;
- one live socket from K;
- one live socket from F;
- two inert sockets from the five unused candidates;
- deterministic by run seed.

## Contract And Counterplay Anchors

| Anchor | Approx. X/Z | Rule |
|---|---|---|
| Player spawn | -28.0 / -2.0 | Faces rack and P01; Owl cannot see spawn |
| Intake rack | -24.5 / -2.0 | Existing three-slot behavior; clear approach while carrying |
| Extraction boundary | -30.0 / -2.0 | Existing extraction behavior; no new exit mechanic |
| Panel A | -20.0 / +9.0 | Containment; nearby sight break and alternate route |
| Panel B | +11.0 / +17.0 | Coolant; nearby sight break and alternate route |
| Panel C | +9.0 / -14.0 | Fuel; nearby sight break and alternate route |
| Owl initial anchor | +18.0 / -2.0 | Utility/Fuel boundary; not a two-connector watch point |

Distribute three portable metal props in each lobe. Props must be reachable, throwable, and outside portals, socket interaction space, and required NavMesh width.

## Sightline Budgets

| Area | Intended ordinary band | Allowed authored peak | Design test |
|---|---:|---:|---|
| Containment | 12–18 m | 22 m | Exposed crossing always terminates at reliable full cover |
| Coolant | 6–12 m | 14 m | Dense machinery still yields a clear basin-oriented loop |
| Fuel | 10–16 m | 20 m | Lateral cut appears every 5–7 m |
| North | 14–18 m | 22 m | Quiet route is visibly risky, not empty |
| Utility | 5–10 m | 12 m | Cover does not make the shortest connector universally safe |
| South | 6–12 m | 15 m | Bends block long control without becoming blind maze turns |

Measure representative sightlines with explicit probe transforms or scene rays. Do not treat these as decorative estimates.

## Route-Evidence Mapping

| Player-facing connector | Explicit metadata | Compatibility value |
|---|---|---|
| North Pipe Gallery | RouteFamily.NorthPipeGallery or equivalent semantic wrapper | ThreatWing.West |
| Utility Spine | RouteFamily.UtilitySpine or equivalent semantic wrapper | ThreatWing.Central |
| South Maintenance Channel | RouteFamily.SouthMaintenance or equivalent semantic wrapper | ThreatWing.East |
| Any lobe, annex, void, or objective pocket | None | No route-choice evidence |

Prefer a narrow semantic mapping layer over renaming the existing enum across the whole codebase. F1 must render player/operator-facing connector names.

## Owl Graph

Author approximately 18 route/patrol nodes:

- three in Containment, three in Coolant, and three in Fuel;
- two in each connector;
- one near but outside the annex;
- two local search probes distributed where needed.

Graph rules:

- connector nodes have explicit connector metadata;
- lobe/search nodes have none;
- no normal wait/hold node watches two portals;
- each lobe is entered from both adjoining connectors;
- active socket pockets are never terminal Owl traps;
- fallback graph and NavMesh represent the same macro connectivity.

Exact node transforms are an implementation detail validated through the gameplay overlay, visibility checks, and runtime inspection.

## Seeded Route Profiles

Keep route variation bounded. A profile may close one local secondary branch or make the shallow Fuel gantry visual-only. It may not close P01–P07 or disconnect a local loop.

Recommended initial profiles:

1. Open Circuit: all local branches open.
2. Coolant Service: one minor basin-side branch closed; the opposite side remains open.
3. Fuel Inspection: optional gantry access closed or visual-only; all ground crosscuts remain open.

Do not implement more profiles until all three pass graph, NavMesh, Owl, reset, and human-readability checks.

## Builder Data Boundary

Represent the layout as small declarative specifications comparable to the existing SpaceSpec, corridor, portal, socket, landmark, and waypoint data. Avoid hand-placing durable objects only in the generated scene.

The builder should separate:

- macro space shells;
- portals/connectors;
- cover and landmark masses;
- objective/content anchors;
- route and patrol metadata;
- seeded local variations;
- presentation profiles.

Regenerate only the disposable generated root through Unity MCP and preserve authored/manual content outside it.

## Inspection Shots

Capture at minimum:

1. annex view into Containment;
2. Containment view toward North;
3. North entry into Coolant;
4. basin loop from standing and crouched player views;
5. Utility approach from each side;
6. Fuel lane and crosscut read;
7. South bend and Containment return reveal;
8. one full-cover comparison in each lobe;
9. one Owl position demonstrating it cannot control two connectors.

Images are evidence of composition and obstruction only. They do not replace runtime navigation or human evidence.

