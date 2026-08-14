# Internship Horror Map — Functional Overlay Options

Date: 2026-08-11

Status: Level-design proposals only. No Unity implementation authority.

## Locked Base

- Source: `C:/Users/Mou/Pictures/Krita Drawings/InternshipHorrorMap.png`
- Delivered copy: `InternshipHorrorMap_locked.png`
- Canvas: 2048 × 2048 pixels
- Source SHA-256: `857275437965e59390fBE8939B8D0844FE404FB4DB77F94042F7A21BA1DC7969`
- Locked-copy SHA-256: `857275437965e59390FBE8939B8D0844FE404FB4DB77F94042F7A21BA1DC7969`
- Byte-for-byte comparison: identical

Every composite is generated from this same locked PNG. The SVG files are transparent overlays and do not embed, trace, or redraw the base.

## Option 1 — Operational Clarity

Files:

- `option_1_operational_clarity_composite.png`
- `option_1_operational_clarity_overlay.svg`

Functional thesis: organize the shell as a believable work sequence from entrance and intake through rod receiving, containment observation, coolant processing, fuel storage, waste service, and control.

Main gameplay strength: the room functions and large silhouettes are immediately understandable, with generous circulation around a small number of dependable cover masses.

Primary risk: the strong functional zoning may become predictable after repeated runs, and the central hall may still feel too open if Owl pressure is high.

Objective/extraction movement: none. The existing Objective Zones A–C and Extraction Points A–C remain in their original locations.

Meaningful difference: this is the clearest operational facility and the least mechanically opinionated option.

## Option 2 — Stealth Grammars

Files:

- `option_2_stealth_grammars_composite.png`
- `option_2_stealth_grammars_overlay.svg`

Functional thesis: create three contrasting stealth spaces—large dependable containment shields, a coolant basin with short machinery sight breaks, and ordered fuel lanes with lateral gaps.

Main gameplay strength: each major area asks for a different movement and concealment response while preserving readable full cover and recovery space.

Primary risk: the fuel room has limited shell depth, so cask density could become cramped in VR; the coolant cluster also needs first-person sightline validation to avoid accidental maze behavior.

Objective/extraction movement: none. Existing Objective Zones A–C and Extraction Points A–C remain unchanged.

Meaningful difference: this option is organized primarily around pursuit, line-of-sight breaking, distraction panels, and distinct cover cadence rather than operational realism.

## Option 3 — Experimental Contract Layout

Files:

- `option_3_experimental_contract_composite.png`
- `option_3_experimental_contract_overlay.svg`

Functional thesis: turn the central containment space into a pass-through inspection loop, place proposed rods in containment, coolant, and fuel work pockets, use a mid-right proposed intake, and activate upper-left Extraction Point C for the final escape.

Main gameplay strength: repeated rod transport converges on a pressured mid-facility intake, while the final exit requires a distinct cross-facility escape rather than a return to the entrance-side bay.

Primary risk: the mid-right intake could become an Owl-camping focus; transport distance and final-run duration may increase; the immutable base labels will describe provisional locations that are inactive in this proposal.

Objective/extraction movement: yes, proposal only. Active rod locations move out of the original A/C terminal rooms into pass-through containment and coolant pockets, with a fuel-lane rod retained in the lower-right area. The proposed active extraction changes from entrance-side Point A to Point C. The shell is unchanged.

Meaningful difference: this option changes the contract’s spatial rhythm and return logic, not only the cover arrangement.

## Validation Record

- Locked base copy verified byte-for-byte against the source.
- All composites verified at 2048 × 2048 with unchanged orientation.
- All overlays use `viewBox="0 0 2048 2048"` and contain no raster `<image>` element.
- No original wall, doorway, room shape, connector, hatch, blocked region, label, or reactor geometry was redrawn.
- No new divider wall was added. Fixture and full-cover footprints were visually inspected against wall, doorway, and hatch locations.
- The reactor remains inaccessible. The cyan arc is a single observation-window notation on the facility-facing side of the existing reactor boundary; it creates no door or route.
- Added labels were inspected for spelling, legibility, and collision with existing labels.
- Existing doorway openings remain unobstructed in the overlays.
- Exact real-world VR and carried-rod clearance cannot be proven from a scale-free image. The overlays preserve visible openings and avoid placing fixtures across them; metric clearance remains a required Unity/scale validation gate before implementation.

Machine-readable hashes and canvas checks are in `validation_manifest.json`. `render_and_validate.js` reproduces the composites from the locked base and the four overlays.

## Artistic Room-Theme Pass

Additional files:

- `artistic_room_themes_composite.png`
- `artistic_room_themes_overlay.svg`
- `artistic_room_themes.md`

This pass applies environmental functions, material/color families, hero silhouettes, and distinct horror identities without selecting a new functional-layout option or moving an objective/extraction point.
