# v0.4.0.0 Human Validation Sheet

Status: Active validation aid; no additional feature authority

Last updated: 2026-08-02

## Test Question

> Can uninformed players form a usable mental map of the three-lobed facility and deliberately switch between its distinct connectors and cover grammars to complete the existing three-rod contract while the Owl remains explainable?

## Build And Modes

- Build identity: v0.4.0.0
- Platform: Windows desktop
- Main scene: P01_CoreLoop
- Primary spatial sessions: F3 route adaptation ON
- Optional navigation control: F3 OFF, recorded separately
- F1: hidden from tester; observer may inspect only after an event
- External map/floor plan: hidden from tester
- F2: normal/default unless intentionally isolated
- Headset/hardware: Not Tested unless physically used

The still-pending v0.3 stealth validation uses its own sheet and primary F3 OFF procedure. Do not merge the two result sets.

## Minimum Evidence

- Three uninformed desktop sessions for majority claims.
- One reverse-direction or intentionally varied-route run.
- At least one full-cover comparison in two different lobes per tester.
- At least one observed deliberate connector change after Owl pressure.
- Exact build/commit or local source, run seed, and route profile.

With fewer than three testers, mark the result Preliminary. A developer walkthrough is automated/operator evidence, not uninformed human evidence.

## Pre-Test Checklist

- [ ] Correct v0.4.0.0 build launched.
- [ ] Fresh run and seed/profile recorded.
- [ ] F3 ON and F1 hidden.
- [ ] External plan and intended route are not shown.
- [ ] One live rod exists in each lobe and two inert decoys are active.
- [ ] Every portal, rack, panel, prop family, Owl, objective, and extraction is functional.
- [ ] Observer can inspect F1 after a surprising event.
- [ ] Audio is at a safe, audible level.
- [ ] Tester has not been told lobe names, connector tradeoffs, or intended traversal order.

## Neutral Introduction

Tell the tester only:

> Complete the contract and survive. The facility has multiple routes. You can crouch, lean, pick up and throw loose objects, and interact with marked machinery. Please think aloud when you choose a route or when the Owl surprises you.

Do not name landmarks, explain the circuit, identify live-rod distribution, or suggest which connector is safer.

## Observation Timeline

### Departure And First Lobe

Record:

- first route chosen and why;
- whether the annex/Containment boundary is understood;
- first landmark noticed;
- first wrong turn, hesitation, or return to a known place;
- whether signs or geometry drove recognition.

### First Connector Choice

Record:

- connector entered;
- whether it is recognized as a transition;
- perceived risk before Owl contact;
- actual Owl pressure and evidence source;
- whether the tester predicts where the connector leads.

### Lobe Recognition

For each lobe, record:

- words the tester uses to identify it;
- landmark and cover grammar noticed;
- route through the local loop;
- whether an objective pocket becomes a dead end;
- one full-cover/quiet-relocation attempt where available.

### Route Change Under Pressure

Record:

- Owl state/evidence before the decision;
- whether the connector change is deliberate, accidental, or prompted;
- expected destination and risk;
- actual result;
- whether adaptation creates pressure and another opening.

### Return And Extraction

Record:

- whether the tester returns to the annex without direction;
- unnecessary full loops or repeated dead ends;
- rod deposit and extraction outcome;
- approximate run duration;
- any reported mandatory choke or universally safe route.

## Neutral Post-Test Questions

Ask in this order:

1. Draw or describe the facility from memory.
2. What were the three main areas? How could you tell them apart?
3. What connected each pair of areas?
4. Which route felt quiet but exposed?
5. Which route felt noisy but gave nearby cover?
6. Which route felt indirect or shielded?
7. When did you deliberately change route, and why?
8. Where did you feel trapped or forced through one choke?
9. In which area was full cover easiest or hardest to use?
10. What did you think the Owl knew during the most surprising moment?
11. Could you find the contractor annex again without a map?
12. What would you do differently in another run?

Do not correct the tester until every answer is recorded.

## Per-Session Record

    Session ID:
    Date/time:
    Tester familiarity:
    Build/commit:
    Run seed/profile:
    F3 mode:
    Outcome/duration:

    Observed facts:
    -

    Tester statements, quoted briefly:
    -

    Recognized Containment: Pass / Fail / Unclear
    Recognized Coolant: Pass / Fail / Unclear
    Recognized Fuel: Pass / Fail / Unclear
    Returned to annex without guidance: Pass / Fail
    Named a meaningful connector tradeoff: Pass / Fail / Unclear
    Changed connector deliberately after pressure: Pass / Fail / Not Observed
    Used full cover and quiet relocation in two lobes: Pass / Fail / Not Observed
    Reported mandatory choke: Yes / No / Unclear
    Reported universal safe connector: Yes / No / Unclear
    Most surprising Owl event:
    F1 reconstruction:
    Map sketch/description quality:
    Most important finding:
    Bounded tuning proposal; not approved:

## Aggregate Acceptance

| Criterion | Session 1 | Session 2 | Session 3 | Aggregate |
|---|---|---|---|---|
| Identifies all three lobe identities | Not Tested | Not Tested | Not Tested | Not Tested |
| Returns from a visited lobe to annex unaided | Not Tested | Not Tested | Not Tested | Not Tested |
| Names a meaningful connector risk difference | Not Tested | Not Tested | Not Tested | Not Tested |
| Deliberately changes connector after pressure | Not Tested | Not Tested | Not Tested | Not Tested |
| Uses full cover/quiet relocation in two lobe grammars | Not Tested | Not Tested | Not Tested | Not Tested |
| Reports no mandatory single choke | Not Tested | Not Tested | Not Tested | Not Tested |
| Surprising Owl behavior is reconstructable | Not Tested | Not Tested | Not Tested | Not Tested |

## Result Classification

### Passed

- Every majority criterion reaches at least two of three.
- No repeated mandatory-choke or universal-safe-route failure appears.
- Surprising Owl behavior is reconstructable.
- The existing contract remains completable.

### Conditional

The mental map and route choices mostly work, but one bounded sign, sightline, occluder, node, or connector tuning issue blocks a criterion. Record one v0.4.z.w refinement and a retest condition.

### Failed

Players consistently confuse lobes, need directions, treat one connector as universal, encounter unavoidable connector camping, or cannot connect Owl behavior to evidence.

### Inconclusive

Too few testers, wrong build/mode, broken audio, a navigation defect, coached navigation, or invalid objective distribution prevents a trustworthy judgment.

## Evidence Separation

Record each layer separately:

| Evidence layer | Status | Notes |
|---|---|---|
| Automated topology/navigation | Not Tested | |
| Operator desktop walkthrough | Not Tested | |
| Uninformed desktop human | Not Tested | |
| XR/headset | Not Tested | |
| Venue haptics/physical effects | Not Tested | |
| Heart-rate hardware | Not Tested | |

Desktop success does not establish XR comfort, fatigue, reach, throwing, headset-scale readability, spatial audio, or physical safety.

## Findings To Action

Use this order:

1. release-blocking navigation, reset, or regression defect;
2. repeated spatial comprehension failure;
3. repeated mandatory choke or Owl-camping failure;
4. bounded landmark, sign, sightline, cover, or node tuning;
5. presentation polish only after the experiment is answered.

Do not add a minimap, objective arrow, new mechanic, or new route until evidence shows the locked geometry cannot answer the question through bounded tuning.

