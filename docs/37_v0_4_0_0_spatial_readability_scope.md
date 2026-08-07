# v0.4.0.0 Scope: Spatial Readability And Pursuit Topology

Status: Authoritative implementation scope

Approved: 2026-08-02

Target implementation window: 2026-08-03 through 2026-08-09

## Authority And Naming

The user has locked concept 7C as the macro layout for v0.4.0.0. This document promotes that decision into implementation authority.

Use only the four-part version identity v0.4.0.0 and V040 task names in implementation work. “7C” may appear in design provenance and asset names, but it is not a development-version label.

The local v0.3.0.0 readable-stealth implementation is the systems baseline and must be preserved. Its human-validation gate remains open and may run in parallel; it is not permission to redesign v0.3 behavior during the level pass.

## Prototype Question

> Can uninformed players form a usable mental map of a three-lobed nuclear facility and deliberately switch between its distinct connectors and cover grammars to complete the existing three-rod contract while the Owl’s sight, sound, search, and route adaptation remain explainable?

## Locked Player Experience

The facility is a compact industrial circuit around an inaccessible shielding/service void, not a divided cube and not a central-hub dungeon.

The player should be able to:

1. leave a recognizable contractor annex;
2. identify Containment, Coolant, and Fuel Handling from silhouette and landmark, not only signs;
3. understand that each lobe supports a different kind of concealment and movement;
4. choose among three pairwise connectors with distinct risk profiles;
5. collect one live rod from every lobe;
6. change route after pressure without using a single universal safe corridor;
7. return to the annex, deposit three confirmed live rods, and extract;
8. explain surprising Owl behavior through visible geometry, audible evidence, and F1 telemetry.

## Locked Macro Topology

The playable topology is:

- Contractor Annex ↔ Containment & Control
- Containment & Control ↔ North Pipe Gallery ↔ Coolant & Maintenance
- Coolant & Maintenance ↔ Utility Spine ↔ Fuel Handling & Waste
- Fuel Handling & Waste ↔ South Maintenance Channel ↔ Containment & Control

This produces one full three-lobe circuit plus one complete local loop inside every lobe.

The inaccessible central shielding/service void:

- prevents a visually dominant central hub;
- blocks cross-map shortcuts;
- creates occlusion and orientation;
- is never a playable objective pocket;
- must not become a safe perimeter path.

The approximate bounding footprint is 57 m east/west by 40 m north/south. The main floor is Y 0.00 m. Only two shallow authored height changes are in scope: a +0.80 m control overlook and a +0.60 m fuel gantry. No ladder, jump-only route, or required steep ramp is allowed.

## Three Lobe Grammars

### A. Containment And Control

Player read: monumental, exposed, institutional.

- landmark: reactor drum;
- support masses: four full-cover concrete/shield blocks;
- deliberate sightlines: approximately 12–22 m;
- local loop: around the reactor drum and control shield;
- tension: crossing exposed gaps between reliable large occluders;
- contractor annex, intake rack, and exit sit on its outer edge.

### B. Coolant And Maintenance

Player read: mechanical, wet, dense but legible.

- landmark: coolant basin;
- support masses: pump and pipe clusters;
- deliberate sightlines: approximately 6–14 m;
- local loop: two sides of the basin/pump cluster;
- tension: short sight breaks, noisy machinery, and frequent decisions;
- density must not become a maze or repeated identical pipe clutter.

### C. Fuel Handling And Waste

Player read: ordered lanes beside a damaged hot-zone boundary.

- landmark: cask rows and red-striped hot-zone wall;
- support masses: casks, transfer machinery, and one full-height shield;
- deliberate sightlines: approximately 10–20 m;
- lateral cuts: approximately every 5–7 m;
- local loop: lanes plus crosscuts, including the shallow gantry only as an optional view route;
- tension: readable commitments with several lateral escapes.

## Three Connector Grammars

| Connector | Clear width | Player-facing risk | Route-adaptation meaning |
|---|---:|---|---|
| North Pipe Gallery | 3.2 m | Quiet and exposed; periodic 18–22 m view | Legacy internal West compatibility ID |
| Utility Spine | 3.4 m | Short, noisy, and cover-rich | Legacy internal Central compatibility ID |
| South Maintenance Channel | 3.6 m | Shielded, indirect, and turn-heavy | Legacy internal East compatibility ID |

The old West/Central/East identifiers may remain internally for one-week compatibility. Player-facing signs and F1 telemetry must use the connector names above.

Only presence or events inside authored connector zones count as route-choice evidence. Entering a lobe, objective pocket, annex, or destination room must not be misread as choosing a connector. Remove position-only X-threshold inference from active v0.4 route evidence; use explicit zone or waypoint metadata.

## Objective And Content Distribution

- Keep exactly three live rods and two inert decoys active.
- Provide eight candidate sockets: C1–C3 in Containment, K1–K3 in Coolant, and F1–F2 in Fuel.
- Every seed must select exactly one live rod per lobe.
- Select two inert decoys from unused sockets without breaking the one-live-per-lobe rule.
- Same-seed restart reproduces selection; new-seed restart may change sockets.
- Every objective pocket is pass-through with at least two player exits.
- Provide one existing machinery test panel and three existing portable metal props per lobe.
- Do not add a new objective, tool, interaction family, or distraction type.

## Owl And Stealth Constraints

All v0.3 fairness laws remain active:

- full cover blocks every visual sample;
- quiet relocation does not provide hidden tracking;
- confirmed current sight outranks sound;
- accepted sound reveals its event origin, not the player’s later position;
- search uses last credible evidence and bounded probes;
- route adaptation changes pressure without invalidating geometry or teleporting.

Author an approximately 18-node Owl navigation/patrol graph:

- connector nodes carry one explicit route-family label;
- lobe, annex, and objective-pocket nodes carry no route-family evidence;
- every lobe has at least two approaches;
- no normal Owl hold position can directly watch two connectors;
- no patrol target sits inside an objective socket or directly blocks the rack/exit;
- the Owl and player must retain non-jump NavMesh paths through every valid seed.

## Seeded Variation

Variation may:

- change the active rod and decoy sockets under the distribution rule;
- close one local secondary branch or optional gantry segment;
- change bounded lighting/audio profiles;
- alter patrol weights and existing Owl profile values.

Variation may not:

- close an entire connector;
- remove a lobe’s local loop;
- create a terminal objective pocket;
- reduce the facility to one mandatory route;
- move landmarks so the macro map changes identity;
- procedurally generate room geometry.

Every supported variation must preserve at least two graph-distinct non-jump paths from the Containment side of P01 to each active socket. The Annex–P01 spur is the only permitted shared access edge; each socket pocket still needs two exits.

## Presentation Boundary

Use three modular greybox kits:

- concrete shields and institutional control surfaces;
- pipe, pump, basin, and wet-service silhouettes;
- crane, cask, lane, and hot-zone silhouettes.

The goal is silhouette, navigation, cover, and risk readability. Final art, bespoke environmental storytelling, cosmetic prop density, generalized modular kits, and a mission generator are out of scope.

## Must Have

- The exact four-zone topology: annex plus three unequal lobes connected in a circuit.
- The three named pairwise connectors with distinct risk profiles.
- An inaccessible central service void.
- One readable landmark and one complete local loop per lobe.
- Two exits from every objective pocket.
- Eight sockets with one live rod per lobe.
- Three machinery panels and nine portable metal props, distributed one panel and three props per lobe.
- Explicit connector metadata for route evidence.
- Valid player and Owl navigation under every supported seed.
- Preserved v0.3 perception, sound-priority, feedback, reset, debug, objective, and extraction behavior.
- Operator-readable lobe, connector, current target, and route-evidence telemetry.
- A fresh Windows build identified as v0.4.0.0.

## Should Have

- Restrained view framing that previews the next landmark without revealing the whole map.
- A player-readable hot-zone boundary in Fuel Handling.
- An optional shallow overlook/gantry that improves orientation without becoming a dominant safe route.
- Evidence screenshots from representative standing and crouched views.

## Could Have

- Minor prop, light, sign, or audio-profile tuning after the first human session.
- Additional decorative pipe/cable variants using already-approved geometry families.

Could Have work may not displace graph validation, navigation, regression tests, the build, or human observation.

## Explicitly Out Of Scope

- Any new mechanic, enemy, contract, objective type, capture/recovery system, tool, weapon, inventory, door system, hazard simulation, radiation mechanic, or hardware integration.
- Learned AI, persistent memory, procedural rooms, a generalized mission generator, or multiple entities.
- Final materials, production lighting, bespoke animation, or content-complete dressing.
- A playable multilevel facility, ladder traversal, elevators, jump-required routes, or comfort claims.
- Balance changes to v0.3 stealth unless a release-blocking regression is proven.
- Reopening Prototype 2 as a polish bucket.

## Automated Acceptance

- Every expected lobe, connector, portal, landmark, socket, panel, prop group, route node, and metadata label exists exactly as specified.
- The authored graph contains the locked circuit and one local loop per lobe.
- Every active socket has two graph-distinct, non-jump routes from the Containment side of P01 under every supported seeded variation; the Annex–P01 spur is the only permitted shared access edge.
- Every active socket is reachable by the player and Owl where intended.
- Exactly one live rod appears in each lobe and exactly two inert decoys occupy unused sockets.
- All seven portal thresholds preserve at least 2.76 m clear width and carrying clearance.
- Connector-only route evidence maps North/Utility/South to the three compatibility IDs and ignores lobe-only traversal.
- No authored Owl hold/watch point has unobstructed control of two connector thresholds.
- All existing v0.3 focused tests remain green.
- Full Edit Mode and Play Mode suites pass.
- Same-seed and new-seed reset preserve their defined deterministic behavior.
- The v0.4.0.0 Windows build succeeds, launches, resets, and completes a meaningful smoke run without project-code exceptions.

## Human Acceptance

Use 40_v0_4_0_0_human_validation_sheet.md with at least three uninformed desktop testers.

- At least two of three correctly identify all three lobe identities after one run.
- At least two of three can return from a visited lobe to the annex without operator guidance or an external map.
- At least two of three name a meaningful risk difference between at least two connectors.
- At least two of three deliberately choose a different connector after Owl pressure.
- At least two of three use full cover and quiet relocation in two different lobe grammars.
- At least two of three complete or nearly complete the existing contract without reporting a mandatory single choke.
- Every surprising Owl response examined is reconstructable from geometry, sound, and telemetry.

Desktop evidence does not validate VR scale, locomotion comfort, crouch fatigue, reach, throwing, spatial audio in-headset, venue effects, haptics, or physical safety.

## Failure Conditions

v0.4.0.0 fails its prototype question if:

- testers cannot distinguish or remember the three lobes;
- a lobe reads as clutter rather than a coherent place;
- one connector is an obvious universal answer;
- a single Owl position reliably controls two connectors;
- an objective pocket becomes a dead end or capture trap;
- players repeatedly need operator directions or an external map;
- route adaptation records destination-room movement as connector choice;
- seeded variation removes meaningful route choice;
- the level pass regresses the v0.3 protected stealth response, reset, objective, or build;
- presentation work prevents a tested, navigable, deterministic greybox.

## Cut Order

If the week slips, cut in this order:

1. decorative pipe, cable, and signage variants;
2. extra lighting/audio profiles;
3. the playable fuel gantry, retaining it as a non-playable visual landmark;
4. nonessential local branch variation.

Do not cut the three lobes, three connectors, central void, lobe landmarks, local loops, two-exit objective pockets, explicit route metadata, one-live-per-lobe distribution, regression tests, or build gate.

## Evidence Discipline

Automated evidence can prove topology, counts, metadata, deterministic selection, navigation probes, reset, regressions, and build health. It cannot prove that people form a mental map, perceive route tradeoffs, experience fair pursuit, or remain comfortable in XR.

Keep the status of desktop human, XR, venue hardware, physical effects, and heart-rate evidence separate.
