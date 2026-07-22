# Project Hub

Last updated: 2026-07-22

## Project Identity

Working title: VR Extraction Horror Prototype

Client/company context: Pixoul

Delivery target: 2026-08-28

Current phase: Prototype 2 implementation at `v0.2.2.1`; PT2 Readable Route Adaptation scope frozen through 2026-07-24

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
- Entities are unkillable in the internship scope; player tactics focus on stealth, misdirection, delay, environmental control, and temporary close-range defense.
- The player carries only a small practical equipment set; mission-specific tools remain coherent with their site and objective.
- Shared stealth is graded, physical crouching has a controller alternative, and flashlight use trades navigation for visibility.
- Heart rate may later influence avatar breathing/noise, but never gives an entity raw direct detection.
- Prototype 1 uses deterministic adaptive game AI; pre-trained/model-driven strategy is post-MLP R&D.
- Player stealth feedback uses clear breathing, optional subtle indicators, and complete operator debug telemetry.
- A simulated stress slider/adapter precedes real heart-rate integration.
- Electrical probe is the current close-range stun-tool direction.
- The no-cart rod loop remains authoritative; a small staging cart is a separate design experiment.

## Live Questions

These questions are parked and do not block PT2:

- Which VR headset and runtime will be used?
- What Pixoul haptic/physical devices are available for development access?
- Will the project be demoed in a controlled booth, arcade bay, or open VR space?
- What is the minimum acceptable visual fidelity for the Aug 28 demo?
- Who are the final stakeholders and demo audience?
- Which immediate consequence should an inert or invalid rod create: time loss, noise/threat escalation, forced return, or a combination?
- What confidence and decay values make suspicion after one use and adaptation after two readable within 15-20 minutes?
- Which Owl animation, route change, and response best communicates route suspicion without UI?
- Which critical player behavior or affordance must entities never counter once the core verbs are stable?
- What physical equipment/holster model remains readable with two or three items and little UI?
- How should darkness, flashlight use, movement, noise, and emergency hiding spots combine into stealth?
- Which two or three environmental manipulation systems are worth teaching in Nuclear Recovery?
- Which diegetic feedback lets players judge their own noise without a mandatory HUD meter?
- What safe activation, stun duration, and failure feedback should the electrical probe use?
- What non-adaptation setback should follow the Owl's second capture?
- Should Nuclear Recovery test an optional noisy staging cart, or preserve the three separate transport trips?
- What primary and secondary effects justify the ventilation/radiation system?

## Next Sprint Goal

By Friday 2026-07-24, complete PT2 Readable Route Adaptation: credible Direct/Inferred route evidence, Suspicious/Adapted/Counter-Adapted behavior, diegetic expression, F1 telemetry, F3 baseline comparison, focused regression, and at least two observed desktop sessions where access allows. All broader feature families remain deferred until this gate is reviewed.
