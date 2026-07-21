# Risk and Scope Register

Last updated: 2026-07-20

Risk ratings:

- Impact: Low, Medium, High
- Probability: Low, Medium, High
- Response: Avoid, Reduce, Transfer, Accept, Defer

## Risk Register

| ID | Risk | Impact | Probability | Early Warning | Response | Fallback |
|---|---|---:|---:|---|---|---|
| R-001 | Hardware integration takes longer than expected. Real devices are currently unavailable during office renovation and remote work. | High | High | Headset, device model, SDK access, permissions, or on-site test date remains unconfirmed. | Reduce | Continue behind Disabled/Simulated/Live interfaces; prepare a device inventory and integration checklist before office access returns. |
| R-002 | Procedural generation expands beyond the timeline. | High | Medium | Time is spent on generation rules instead of playable tension. | Reduce | Use authored rooms with variable objective, hazard, and route states. |
| R-003 | The smart creature and its mixed counterplay become overbuilt. | High | Medium | Each counterplay requires unique AI logic, tuning, animation, or content before the baseline loop is tested. | Reduce | Implement several readable responses through a small shared state machine and stimulus model; no machine learning is required. |
| R-004 | VR comfort issues limit movement or horror intensity. | High | Medium | Testers report nausea, disorientation, or fatigue. | Reduce | Use comfort locomotion, snap turn, readable lighting, and warning cues. |
| R-005 | Scope creep from roguelite/multiplayer/progression ideas. | High | High | Tasks appear that do not improve the first playable loop. | Avoid | Treat these as post-MLP future directions unless explicitly approved. |
| R-006 | Haptic effects feel gimmicky or confusing. | Medium | Medium | Players cannot identify what the physical cue means. | Reduce | Tie haptics to clear in-world events and test with/without comparison. |
| R-007 | Player does not understand objective quickly enough. | High | Medium | First-time players ask what to do after the opening minute. | Reduce | Add diegetic guidance, simple objective language, and environmental cues. |
| R-008 | Visual darkness hurts usability in VR. | Medium | Medium | Players miss interactables or routes. | Reduce | Keep horror lighting readable and add silhouette/contrast rules. |
| R-009 | Demo requires developer intervention. | High | Medium | Build needs manual console commands or scene setup. | Reduce | Add operator reset, clear start/end states, and debug toggles. |
| R-010 | Biofeedback data is noisy or ethically awkward. | Medium | Medium | Heart-rate spikes do not match fear or punish anxious players unfairly. | Defer | Use heart rate as optional intensity flavor, not core success/failure. |
| R-011 | Recoverable capture creates a failure spiral or makes the remaining timer unwinnable. | High | Medium | One capture causes repeated recaptures, excessive lost progress, or players to give up. | Reduce | Guarantee a short recovery window, cap stacked penalties, preserve at least one viable route, and test time-to-recover. |
| R-012 | Desktop-first validation hides VR comfort or interaction problems. | High | High | Desktop play feels acceptable but carrying, turning, crouching, reach, or threat encounters cannot be reproduced comfortably in-headset. | Reduce | Label desktop evidence honestly, keep XR routes non-jump and comfort-first, and schedule headset validation as soon as the target device is known. |
| R-013 | The multi-contract vision creates combinatorial content, balance, and QA scope. | High | High | New sites, objectives, and entities are planned as freely interchangeable before one contract is validated. | Avoid | Build one August proof contract and define explicit compatibility for future curated combinations. |
| R-014 | The promise of highly intelligent entities drives premature AI complexity. | High | High | Work shifts toward generalized learning infrastructure before one readable adaptation is playtested. | Reduce | Prototype bounded behavioral memory and authored response biases; judge perceived intelligence before increasing technical sophistication. |
| R-015 | Horror intensity or implied predation exceeds the intended roughly 12+ venue audience. | High | Medium | Testers, parents, operators, or stakeholders find capture presentation too graphic or distressing. | Reduce | Use implication, off-screen transitions, content review, intensity settings, and age-appropriate playtests; confirm formal venue policy separately. |

## Scope Guardrails

### Must Protect

- Single-player 15-20 minute vertical slice.
- Clear extraction loop.
- One curated site-objective-entity combination for the MLP.
- One environment style and one adaptive entity in the current proof contract.
- One meaningful haptic or physical integration.
- At least 3 run variables.
- Demo reliability.
- VR comfort and safety.

### Can Simplify

- Procedural generation can become authored rooms with randomized states.
- Adaptive AI can become pressure scoring plus behavior state changes.
- Haptics can begin as simulated or controller rumble if device access is delayed.
- Threat can begin as unseen pressure through sound, lighting, and haptics.
- The number of required fuel rods can be reduced if repeated transport harms pacing.

### Should Cut First If Timeline Tightens

1. Multiplayer.
2. Persistent progression.
3. Complex procedural layout generation.
4. Multiple enemy types.
5. Advanced heart-rate mechanics.
6. Multiple hardware integrations.
7. Large map size.
8. Nonessential lore delivery.
9. A second complete contract scenario.

## Definition Of Scope Creep

A feature is scope creep if it does not directly improve one of the following before 2026-08-28:

- Understandable extraction loop.
- Repeatable tension.
- VR comfort and safety.
- Meaningful venue-specific immersion.
- Demo reliability.
- Stakeholder-visible value.
