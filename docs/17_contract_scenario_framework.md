# Contract Scenario Framework

Status: Early game-design framework

Last updated: 2026-07-20

## Purpose

The game is not exclusively about the Owl Thing, fuel rods, or a nuclear facility. Those elements form the first proof contract.

The wider game fantasy is:

> Take dangerous paid jobs in hostile places, perform physical work under escalating pressure, survive, and extract.

## Content Model

A contract is an authored combination of:

| Layer | Purpose | Current Proof Contract |
|---|---|---|
| Site | Defines navigation, atmosphere, affordances, and environmental hazards. | Post-apocalyptic nuclear facility |
| Job | Defines the physical objective and progress structure. | Identify and recover three live fuel rods |
| Entity | Defines the primary hostile intelligence that observes, learns, and pressures the player. | The Owl Thing |
| Secondary dangers | Add suspense, route pressure, and complications without replacing the entity as the main opponent. | Radiation, lockdown, unstable facility systems |
| Conditions | Change routes, resources, timing, lighting, hazards, or threat profile between runs. | Seeded route, rod, lighting, and Owl profiles |
| Extraction rule | Defines the final commitment and escape condition. | Deposit rods, survive cycling, cross the exit |
| Feedback map | Assigns unique audio, visual, controller, haptic, or room-effect meanings. | Heat for live rods, vibration for Owl proximity, Wind for extraction |

## Authoring Rule

These layers are modular for production and code ownership, but not automatically interchangeable in design.

Every proposed combination needs explicit compatibility checks:

- Does the site physically support the objective interactions?
- Can the entity pressure the objective without making it impossible?
- Does the entity have a distinct learning model rather than merely higher speed or aggression?
- Are warning and counterplay cues readable in this environment?
- Do the haptic meanings remain unique?
- Can the mission fit the 15-20 minute venue window?
- Can the combination be reset and operated reliably?
- Does it offer a distinct experience rather than cosmetic replacement?

Use curated contract packages first. Compatibility tags may later allow safe selective remixing, but unrestricted procedural combinations are not an MLP promise.

## Current Proof Contract

Working label: Nuclear Recovery

- Site: ruined nuclear facility.
- Job: identify live fuel rods through Geiger discovery and authoritative assay, then transport three to extraction.
- Entity: Owl Thing, a smart adaptive hunter.
- Secondary dangers: radiation fiction, route changes, lockdown pressure, and unstable machinery.
- Objective pressure: the Owl interrupts both identification and transport.
- Capture fiction: the Owl knocks the contractor unconscious; predation is implied but never shown directly.
- Recovery: banked rods remain safe, carried items scatter into authored positions, and the player resumes from one of roughly five authored recovery zones.
- Escalation target: two survivable captures, with the second more severe and a third potentially terminal.

## Future Contract Examples

These are direction examples, not committed production scope. Every future contract still requires an intelligent entity as its primary threat.

### Failing Fort

- Site: damaged fort under evacuation or collapse conditions.
- Job: restart or repair distributed generators.
- Entity: an intelligent hunter suited to the fort and repair objective; not yet designed.
- Secondary pressure: worsening natural disaster, structural failure, route loss, wind, debris, flooding, fire, or power instability.
- Physical possibilities: carrying replacement parts, aligning components, priming machinery, rerouting power, and holding systems stable.

### Same Site, Different Job

- Site: the nuclear facility.
- Job: containment repair, data recovery, survivor rescue, decontamination, or reactor shutdown.
- Design requirement: the new job must change route decisions and physical behavior, not merely replace fuel-rod labels.

### Same Job, Different Entity

- Site and fuel-rod job remain familiar.
- A different entity changes detection rules, warnings, counterplay, capture fiction, and pressure timing.
- Design requirement: reusable objective rules must not make the new entity feel like an Owl reskin.

## Persistent Rules Across Contracts

- The player is a contractor with a clear job.
- Sessions target approximately 15-20 minutes.
- Physical work and survival pressure overlap.
- Every contract has an intelligent entity as the main threat; hazards are secondary.
- The entity observes player habits and changes strategy while preserving counterplay.
- Mistakes have immediate, understandable consequences.
- Initial failure pressure is recoverable enough for arcade fairness.
- Threats provide readable warnings and situational counterplay.
- Haptics communicate narrow in-world meanings.
- The run ends clearly and can restart without developer intervention.

## Contract Template

Use this before approving a future mission:

```text
Contract name:
One-sentence job:
Site:
Primary physical objective:
Primary intelligent entity:
How it observes and adapts:
Why the entity pressures this objective:
Secondary dangers:
Three counterplay options:
Primary run variables:
Capture/failure fiction:
Recovery rules:
Extraction climax:
Unique haptic/physical meanings:
What makes this contract distinct:
Reusable systems:
New systems required:
15-20 minute pacing outline:
Cut-down prototype:
```

## MLP Boundary

The 2026-08-28 deliverable proves the framework through Nuclear Recovery only.

Before building a second contract, Nuclear Recovery should demonstrate:

- objective understanding within two minutes,
- a complete and restartable run,
- fair pressure during physical work,
- noticeable seeded variation,
- replay interest,
- at least one meaningful venue-specific physical effect when hardware is available.

## Open Questions

1. Is the employer one recurring organization with a recognizable voice and identity, or an intentionally anonymous dispatcher?
2. How much should the randomly assigned contract be revealed before entry?
3. Does entity adaptation persist only inside a run, or can it remember a repeat player without requiring customer profiles?
4. Which adaptation signals are shared across all entities, and which remain entity-specific?
5. Which systems must remain identical across all contracts so the game still feels like one product?
6. Should future score or reputation exist only for arcade comparison, or ever affect contract assignment?
