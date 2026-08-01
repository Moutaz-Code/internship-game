# v0.3.0.0 Human Validation Sheet

Status: Active validation aid; no additional feature authority

Last updated: 2026-08-01

## Test Question

> Can uninformed players deliberately manage graded visibility, movement noise, spatial hiding, and improvised distractions, and correctly explain whether the Owl acted on current sight, an accepted sound, or last credible evidence without a stealth meter?

## Build And Modes

- Build identity: `v0.3.0.0`
- Platform: Windows desktop
- Main scene: `P01_CoreLoop`
- Primary sessions: F3 route adaptation `OFF`
- Integration check: F3 route adaptation `ON`
- F1 telemetry: hidden from tester; observer may inspect after an event
- F2 Geiger comparison: normal/default unless a separate issue requires isolation
- Headset/hardware status: Not Tested unless physically used

Record the exact commit or local build source if available. Do not test an unlabeled or mixed build.

## Minimum Evidence

- Three uninformed desktop sessions preferred and required for the stated majority thresholds.
- One route-adaptation-enabled integration run.
- At least one controlled full-cover comparison per tester.
- At least one opportunity to use thrown metal and one maintenance test panel.
- Observer notes of what happened before interpretation.

If fewer than three people are available, mark the result `Preliminary`; do not convert a one-person developer session into a player result.

## Pre-Test Checklist

- [ ] Correct build and main scene launched.
- [ ] Fresh run/reset confirmed.
- [ ] F3 is OFF for the primary session and F1 is hidden.
- [ ] At least one throwable prop is reachable in the opening route area.
- [ ] At least one machinery panel is ready and reachable.
- [ ] Owl, objective, audio, and normal controls work.
- [ ] Observer can access F1 after a surprising event.
- [ ] Tester has not been told the detection thresholds or intended solution.
- [ ] Headphones/speakers are at a safe, audible level.

## Neutral Introduction

Tell the tester only:

> Complete the contract and survive. You can crouch, lean, pick up and throw loose objects, and interact with marked machinery. Please say what you think the Owl knows and why whenever its behavior surprises you.

Do not explain visible-sample counts, noise radii, the hearing threshold, last-known-position search, or which counterplay should work.

## Observation Timeline

### First Movement Contact

Record:

- whether the tester walks, crouches, or sprints;
- whether they notice changing footstep loudness;
- the first Owl response they attribute to movement;
- whether the attribution matches telemetry.

### First Partial And Full Cover

Record separately:

- one partial peek with one or more samples visible;
- one full-cover event with zero samples;
- whether the tester remains at the last-seen point or relocates;
- what the Owl searches;
- whether the tester reports wall tracking.

### First Portable Distraction

Record:

- whether use is intentional, accidental, or prompted;
- throw/drop position and whether impact was accepted;
- whether the Owl currently had confirmed sight;
- tester prediction before the response, if spoken;
- actual response and tester explanation.

### First Fixed Distraction

Record:

- whether the panel is recognized as usable;
- whether its active/cooldown state is understood;
- whether the tester uses it before or after breaking sight;
- relocation chosen after activation;
- whether its limitation compared with portable metal is understood.

### End Of Run

Record:

- objective outcome and approximate duration;
- captures or obvious unavoidable states;
- tactics repeated;
- whether crouch became an always-on behavior;
- whether the tester asked for a stealth meter;
- strongest confusion or perceived unfairness.

## Controlled Full-Cover Comparison

After natural play, ask for one short controlled comparison without explaining the answer:

1. Let the Owl establish visual contact at an agreed landmark.
2. Move behind known full cover.
3. Quietly relocate to a second nearby cover position without throwing.
4. Observe the Owl's first search target and local probes.
5. Inspect F1 only after the behavior completes.

Pass when the Owl searches credible last visual evidence/local probes and does not receive the hidden destination. Mark any accepted movement sound separately; a noisy relocation is not a valid full-cover-only trial.

## Neutral Post-Test Questions

Ask in this order:

1. What made the Owl notice or find you?
2. What did sprinting change?
3. What did crouching change?
4. When you broke sight, where did you think the Owl expected you to be?
5. What did the thrown metal do? When did it fail?
6. What did the machinery panel do? When did it fail?
7. How were the portable and fixed distractions different?
8. Which Owl sound or behavior told you it was investigating rather than actively seeing you?
9. Did any detection feel like wall tracking? Describe the exact moment.
10. Did you need a meter or icon to understand stealth? What information was missing?
11. What would you deliberately do differently in another run?

Do not correct the tester until all answers are recorded.

## Per-Session Record

```text
Session ID:
Date/time:
Tester familiarity with game:
Build/commit:
F3 mode:
Run seed:
Run outcome/duration:

Observed facts:
-

Tester statements, quoted briefly:
-

Correctly identified sound risk: Pass / Fail / Unclear
Used full cover and quiet relocation intentionally: Pass / Fail / Not Observed
Used portable distraction intentionally: Pass / Fail / Not Observed
Used fixed distraction intentionally: Pass / Fail / Not Observed
Distinguished sight from investigation/search: Pass / Fail / Unclear
Explained one success without debug UI: Pass / Fail
Explained one failure without debug UI: Pass / Fail
Controlled full-cover comparison: Pass / Fail / Contaminated by sound
Crouch dominance observed: Yes / No / Unclear
Requested mandatory stealth meter: Yes / No / Optional only
Surprising event telemetry explanation:
Most important finding:
Potential tuning only; not yet approved:
```

## Aggregate Acceptance Table

| Criterion | Session 1 | Session 2 | Session 3 | Aggregate |
|---|---|---|---|---|
| Names sprint or hard impact as major sound risk | Not Tested | Not Tested | Not Tested | Not Tested |
| Uses full cover and quiet relocation intentionally | Not Tested | Not Tested | Not Tested | Not Tested |
| Uses a distraction deliberately | Not Tested | Not Tested | Not Tested | Not Tested |
| Distinguishes current pursuit from evidence search | Not Tested | Not Tested | Not Tested | Not Tested |
| Explains one success and one failure without F1/meter | Not Tested | Not Tested | Not Tested | Not Tested |
| Controlled full-cover trial appears fair | Not Tested | Not Tested | Not Tested | Not Tested |
| Uses both counterplay forms and states their tradeoff | Not Tested | Not Tested | Not Tested | Not Tested |

## Result Classification

### Passed

- Every majority criterion reaches at least two of three.
- Controlled full-cover behavior is fair and telemetry-explainable.
- Neither crouch nor a distraction is a universal answer.
- Existing contract completion remains viable.

### Conditional

The causal model is mostly understood, but a bounded cue, placement, threshold, or timing issue blocks one criterion. Record one evidence-driven `v0.3.z.w` refinement with a retest condition.

### Failed

Players consistently confuse sight and sound, perceive wall tracking, cannot use counterplay deliberately, or require operator information to understand outcomes. Do not hide this with content polish.

### Inconclusive

Fewer than three uninformed testers, incorrect build/mode, contaminated controlled trials, broken audio, or implementation defects prevent a trustworthy judgment.

## Integration Run With Adaptation Enabled

Record one separate run with F3 ON:

```text
Build/seed:
Tester or operator:
Route evidence observed:
Stealth behavior still explainable:
Sight-over-sound priority preserved:
Full cover still protected:
Changed route creates an opening:
Regression or confusion:
Result: Pass / Fail / Inconclusive
```

This integration run checks compatibility. It does not replace the still-pending uninformed human validation of Prototype 2 route adaptation.

## Findings To Action

For every proposed change, record:

```text
Observed evidence:
Interpretation:
Affected acceptance criterion:
Tuning, defect, or new feature:
Smallest reversible change:
Retest required:
Approved by user: Yes / No
Target version:
```

Do not turn an appealing tester suggestion into active scope without approval.
