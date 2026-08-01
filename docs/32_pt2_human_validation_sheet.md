# PT2 Human Validation Sheet

Status: Active validation aid; no new feature authority

Last updated: 2026-07-26

## Test Question

> Does the Owl appear to notice repeated route-family use, become suspicious after one credible observation, adapt after two, and encourage the player to vary routes without feeling omniscient or making any route unusable?

## Build

- Target local build: `v0.2.3.0`.
- Windows player: `UnityProject/InternshipGameSynced/Builds/v0.2.3.0/InternshipGame_v0.2.3.0.exe`.
- Verify the executable and repository state before testing.
- Record any tuning or code difference from the named build.

## Minimum Evidence

- Two uninformed desktop testers are required.
- Three testers are preferred.
- Do not explain the adaptation rule before the session.
- Do not show the F1 operator overlay to the tester.
- Keep F3 adaptation enabled for the primary sessions.
- Use an adaptation-disabled comparison only after collecting the tester's unprompted impressions.

Desktop evidence does not validate XR comfort, physical crouching, haptics, hardware, or venue operation.

## Pre-Test

```text
Date:
Build:
Seed:
Tester:
Observer:
Prior familiarity:
Adaptation enabled:
Geiger interference enabled:
Recording/logging:
Known deviations:
```

Observer setup:

- Launch from a clean start.
- Confirm input, audio, restart, and extraction work.
- Use the same seed for enabled/disabled comparison where practical.
- Keep verbal intervention to safety or a true blocker.
- Record what the player does before asking what they believed.

## Observation Timeline

### First Two Minutes

- When does the tester understand the contract?
- Which route do they choose first?
- Do they notice West, Central, and East as meaningful route families?
- Do they require verbal help?

### First Credible Route Repetition

- What route behavior did the Owl plausibly observe or infer?
- Did the Owl's response change?
- Did the tester notice anything without prompting?
- What did the tester do next?

### Adapted Pressure

- Did the tester repeat the route enough to trigger adaptation?
- Was the pressure readable through behavior rather than UI?
- Did the tester remain able to use the route?
- Did they vary route, timing, movement, or distraction?
- Did route variety create a believable opening?

### End Of Run

- Outcome and duration:
- Rods secured:
- Captures/failures:
- Routes used:
- Most repeated route:
- Did the tester request help:
- Did the tester express replay interest:

## Neutral Post-Test Questions

Ask in this order without explaining the system:

1. How did you think the Owl decided where to go?
2. Did its behavior change during the run? What made you think that?
3. Did anything you repeatedly did become more or less reliable?
4. When you changed your approach, what happened?
5. Did the Owl ever seem to know something it should not know?
6. Was any route effectively unusable?
7. What would you do differently in another run?
8. Would you replay immediately? Why or why not?

Only after those answers:

9. The Owl can build confidence about repeatedly used route families. Did that match your experience?
10. Which behavior communicated that most clearly?

## Ratings

Use 1 for strongly negative and 5 for strongly positive.

| Measure | 1 | 2 | 3 | 4 | 5 | Notes |
|---|---:|---:|---:|---:|---:|---|
| Owl felt observant |  |  |  |  |  |  |
| Owl felt fair |  |  |  |  |  |  |
| Behavior changes were readable |  |  |  |  |  |  |
| My decisions affected the encounter |  |  |  |  |  |  |
| Routes remained viable |  |  |  |  |  |  |
| I want another run |  |  |  |  |  |  |

## Result Classification

### Passed

- At least two testers notice changed Owl behavior without being told.
- At least one correctly connects a change to repeated route use without leading.
- Testers deliberately vary their approach or clearly describe doing so next run.
- Route variety creates a perceived opening.
- No tester reports hidden exact tracking or a completely unusable route.

### Conditional

The core relationship is perceived, but one tuning, presentation, or fairness problem repeatedly obscures it. Record one narrow evidence-driven correction and retest.

### Failed

Players do not perceive adaptation, perceive it as arbitrary/omniscient, or lose meaningful route choice. Do not fix this by adding unrelated mechanics.

### Inconclusive

The system did not receive enough credible evidence, a defect invalidated the run, the observer coached the player, or the sample was too small.

## Session Results

| Session | Tester | Seed | Duration | Adaptation Perceived | Route Cause Inferred | Route Varied | Fair | Replay | Result |
|---|---|---:|---:|---|---|---|---|---|---|
| 1 |  |  |  |  |  |  |  |  |  |
| 2 |  |  |  |  |  |  |  |  |  |
| 3 |  |  |  |  |  |  |  |  |  |

## Findings To Action

| Evidence | Interpretation | Priority | Smallest Response | Retest Needed |
|---|---|---|---|---|
|  |  |  |  |  |

## Closeout

```text
PT2 result: Passed / Conditional / Failed / Inconclusive
Evidence summary:
Accepted tuning change:
Deferred ideas:
Remaining desktop evidence:
Remaining XR/hardware evidence:
Decision-log update:
Prototype-log update:
```

Once classified, keep PT2 feature-frozen and move design attention to the next weekly experiment.
