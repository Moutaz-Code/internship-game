# Project Hub

Last updated: 2026-07-20

## Project Identity

Working title: VR Extraction Horror Prototype

Client/company context: Pixoul

Delivery target: 2026-08-28

Current phase: End of Sprint 01; Prototype 1 feature skeleton is approximately 90% implemented and awaiting design validation

Current production style: prototype-heavy agile, weekly loops

## One-Sentence Pitch

Players are contractors taking dangerous 15-20 minute jobs across hostile sites, completing physical objectives while learning and counter-adapting to intelligent entities, with site hazards and physically felt threats escalating the pressure until extraction.

## Game Versus Current Slice

Full-game direction:

- A roster of curated contracts with different sites, objectives, threats, and conditions.
- Examples may include recovering fuel rods from a ruined nuclear facility, repairing generators in a failing fort during a natural disaster, or other authored combinations.
- A recurring contractor fantasy and extraction structure connect the missions.

Current August vertical slice:

- Site: post-apocalyptic nuclear facility.
- Objective: identify and recover three live fuel rods.
- Entity: the Owl Thing.
- This is one proof contract, not the permanent identity or full content promise of the game.

## Current Vertical Slice Target

Build a 15-20 minute single-player VR MLP that proves:

- The player understands the objective within 2 minutes.
- The core loop is tense and replayable.
- At least 3 run variables change between sessions.
- At least 1 haptic or physical effect meaningfully improves immersion.
- The prototype can be demoed reliably without developer intervention.

## Working Pillars

1. Venue-only immersion: use Pixoul hardware and room effects to create an experience players cannot fully replicate at home.
2. Dangerous contract fantasy: accept a clear job, identify and transport the objective, survive complications, and extract.
3. Replayable uncertainty: vary authored contract states, hazards, routes, and threat behavior without overbuilding procedural generation.
4. Adaptive adversary: every scenario centers on an intelligent entity that learns enough about the player to force mutual adaptation; environmental dangers remain secondary pressure.
5. VR comfort and reliability: comfort, safety, and demo stability beat feature count.

## Current Assumptions

- Vertical slice is single-player.
- Engine is Unity.
- Target headset and venue runtime are not yet confirmed.
- Real haptic and sensor hardware is temporarily unavailable while the office is under renovation and development is remote.
- Haptics can initially be simulated before real hardware integration.
- Procedural generation should begin as authored room layouts with variable states, not fully dynamic level generation.
- "Roguelite" currently means run variation, not persistent progression.
- The threat should support several counterplay families; no single tactic should solve it.
- Capture should create a serious recoverable setback before it creates a terminal failure.
- Score may support replayability later, but completing the contract and surviving are the primary goals.
- The MLP ships one curated site-objective-entity combination; future combinations are product direction, not current deliverables.
- Site, objective, and threat modules should be reusable where practical, but compatibility should be authored rather than assumed.
- Contracts are assigned by an employer in fiction and selected randomly from eligible curated packages in gameplay.
- Contract scenarios are isolated arcade episodes; no narrative continuity or persistent progression is required.
- The intended tone is horror-adjacent for roughly age 12 and above, using implication and suspense rather than graphic harm.

## Live Questions

- Which VR headset and runtime will be used?
- What Pixoul haptic/physical devices are available for development access?
- Will the project be demoed in a controlled booth, arcade bay, or open VR space?
- What is the minimum acceptable visual fidelity for the Aug 28 demo?
- Who are the final stakeholders and demo audience?
- Which immediate consequence should an inert or invalid rod create: time loss, noise/threat escalation, forced return, or a combination?
- How long should an entity remember player behavior: encounter, current run, repeat runs, or a combination?
- Which player behaviors should the first adaptive model observe and counter?
- How should the game communicate adaptation without exposing debug-style numbers?

## Next Sprint Goal

By Friday 2026-07-24, turn the rudimentary Unity feature skeleton into a coherent desktop playtest build and gather human evidence about objective clarity, threat fairness, capture recovery, pacing, and replay desire. Headset-specific comfort and real hardware integration remain deferred until access is available.
