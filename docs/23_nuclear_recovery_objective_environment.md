# Nuclear Recovery Objective And Environment

Status: Current objective baseline confirmed; cart and environmental mechanics remain design candidates

Last updated: 2026-07-22

## Design Question

> Which environmental systems deepen fuel-rod identification and transport enough to justify their onboarding, implementation, and playtest cost inside one 15-20 minute contract?

## Current Objective Baseline

The no-cart loop remains authoritative until a separate cart prototype proves a better experience.

- Three live rods are required.
- Inert candidates create search uncertainty.
- Geiger guidance helps discover candidate rods but is not authoritative truth.
- A completed three-second assay determines truth.
- Heat uniquely confirms a live rod.
- The player carries one rod at a time.
- Deposited rods remain safe.
- Invalid deposits create immediate feedback and bounded pressure rather than instant failure.
- The third valid deposit starts the extraction cycle.
- The player survives the holdout and crosses the exit.

## Objective Experience

The objective should create repeated decisions across four activities:

1. Discover candidate rods.
2. Commit attention and time to assay.
3. Choose a transport route under entity pressure.
4. Decide whether an environmental system is worth manipulating before continuing.

The entity should pressure these activities without making rod truth unreliable or permanently blocking the critical path.

## Environmental Interaction Budget

Nuclear Recovery currently has two preferred environmental families:

- ventilation/radiation control,
- constrained movable shielding.

The cart is a separate objective-loop experiment, not a third automatic commitment.

Each implemented environmental family should have:

- one primary gameplay purpose,
- at most one secondary purpose,
- one readable interaction,
- one clear cost or tradeoff,
- deterministic reset behavior,
- measurable use during playtests.

## Ventilation And Radiation Control

Candidate primary purposes:

- clear or reduce danger in one route while worsening another,
- open temporary access to an objective or shortcut,
- redirect a facility hazard.

Candidate secondary purposes:

- mask some player noise,
- create remote machinery noise as a distraction,
- alter visibility through steam, dust, or warning lighting.

Do not implement every effect in the first version. Select one primary and one secondary effect.

Possible physical interactions:

- rotate an industrial valve,
- hold or sequence two controls,
- replace or align a control component,
- reroute flow on a physical panel.

Costs may include interaction time, occupied hands, local noise, route tradeoff, or drawing the Owl toward facility activity.

### Feedback Conflicts

- Heat already means authoritative live-rod confirmation. Do not use Heat as a general radiation warning.
- Wind currently means extraction cycling. General ventilation cannot reuse Wind casually.
- Geiger behavior already guides live-rod discovery. Ambient radiation feedback must not make the tool unreadable or corrupt assay truth.

Possible Wind resolution:

1. Keep physical Wind exclusive to extraction; ventilation uses audio, steam, lights, and environmental motion.
2. Redefine Wind as one coherent facility-airflow system, with extraction as its strongest and most recognizable state.

The first option preserves the current unique haptic vocabulary with less risk.

## Movable Shielding

Preferred form:

- a small number of heavy-looking panels, carts, or barriers,
- movement constrained to authored useful areas or rails,
- local line-of-sight and route value,
- too awkward or bounded to accompany the player for the entire run.

Required rules:

- cannot permanently block a critical route,
- cannot trap or disable the Owl indefinitely,
- cannot be lifted or swung as a weapon,
- cannot require unsafe force or broad real-world movement,
- resets reliably,
- gives the Owl a readable response such as rerouting, investigating, waiting briefly, or moving the panel under controlled rules.

## Objective Cart Fork

A small cart could support R.E.P.O.-like physical staging, but it changes the current objective loop substantially.

### Variant A: No Cart

- Current authoritative loop.
- One rod is carried at a time.
- Three deliveries create repeated exposure and route decisions.
- Lowest physics, navigation, and onboarding risk.

### Variant B: Optional Staging Cart

- Cart may hold a small number of rods or practical objects.
- Pushing is noisy, slower, and occupies the player's hands.
- Some routes or doors cannot accommodate it.
- The player parks it and manually carries rods through constrained spaces.
- Entity contact may jostle or scatter contents.
- Depositing still requires manual transfer to the extraction rack.
- The cart must not also provide permanent universal shielding unless that combined role is explicitly tested.

### Variant C: Required Multi-Rod Cart

- Replaces one-at-a-time transport with a cargo-escort loop.
- Changes route, pacing, physics, capture recovery, and extraction design.
- Not recommended for the current MLP without evidence from Variant B.

## Cart Prototype Question

> Does an optional noisy staging cart create better physical planning and risk than one-at-a-time carrying without shortening the contract, dominating routes, or becoming a physics burden?

Cart success indicators:

- player deliberately chooses when to bring, park, or abandon it,
- cart noise creates readable risk,
- narrow routes preserve hand-carry decisions,
- item scatter creates recoverable drama rather than cleanup frustration,
- total completion time remains inside the target,
- the player does not use it as permanent Owl-proof cover.

Cut the cart if it:

- reduces the three-delivery loop to one safe trip,
- repeatedly blocks navigation,
- requires constant physics correction,
- confuses objective ownership or assay state,
- consumes more explanation than strategic thought.

## Flashlight And Visibility

The flashlight may expose the player through a combination of:

- direct beam contact,
- illuminated surfaces,
- the visible light source,
- the player's lit body or silhouette.

These channels should be tuned separately and exposed in debug view. A visually oriented entity should still require valid geometry, attention, and evidence rather than detecting any flashlight use globally.

## Debug Requirements

- objective stage and rod truth,
- Geiger target and interference state,
- environmental-system state and active effects,
- route accessibility before and after manipulation,
- movable-cover location and blocked-path validation,
- cart contents, capacity, ownership, noise, and reset state if prototyped,
- flashlight visibility contributions,
- Owl evidence produced by environment, cart, cover, or light.

## Implementation Boundary

This document does not authorize the implementation agent to add a cart, ventilation/radiation controls, movable shielding, flashlight stealth, or biofeedback under the current Sprint 02 tasks.

Each system needs a separate prototype question, task, fallback, and acceptance criteria. The no-cart rod loop remains the baseline.

## Decisions Needed

1. Select the ventilation/radiation system's one primary and one secondary effect.
2. Decide whether Wind remains extraction-exclusive or becomes a unified airflow cue.
3. Define how ambient radiation and rod-seeking Geiger feedback remain distinguishable.
4. Decide whether to paper-prototype or implement optional staging-cart Variant B.
5. Set cart capacity, route restrictions, noise, and item-scatter rules if tested.
6. Decide whether the cart and movable shielding are separate objects or mutually exclusive alternatives.
7. Define the Owl's response to moved shielding.
