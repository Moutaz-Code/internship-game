# Risk and Scope Register

Last updated: 2026-07-22

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
| R-010 | Biofeedback data is noisy, inequitable, or ethically awkward. | High | Medium | Heart-rate spikes do not match fear, sensor quality drops, or anxious players are punished unfairly. | Defer | If researched later, map calibrated heart rate into avatar breathing/noise with consent, quality checks, simulation fallback, and an equivalent non-sensor path; never use raw heart rate as direct detection or win/loss. |
| R-011 | Recoverable capture creates a failure spiral or makes the remaining timer unwinnable. | High | Medium | One capture causes repeated recaptures, excessive lost progress, or players to give up. | Reduce | Guarantee a short recovery window, cap stacked penalties, preserve at least one viable route, and test time-to-recover. |
| R-012 | Desktop-first validation hides VR comfort or interaction problems. | High | High | Desktop play feels acceptable but carrying, turning, crouching, reach, or threat encounters cannot be reproduced comfortably in-headset. | Reduce | Label desktop evidence honestly, keep XR routes non-jump and comfort-first, and schedule headset validation as soon as the target device is known. |
| R-013 | The multi-contract vision creates combinatorial content, balance, and QA scope. | High | High | New sites, objectives, and entities are planned as freely interchangeable before one contract is validated. | Avoid | Build one August proof contract and define explicit compatibility for future curated combinations. |
| R-014 | The promise of highly intelligent entities drives premature AI complexity. | High | High | Work shifts toward generalized learning infrastructure before one readable adaptation is playtested. | Reduce | Prototype bounded behavioral memory and authored response biases; judge perceived intelligence before increasing technical sophistication. |
| R-015 | Horror intensity or implied predation exceeds the intended roughly 12+ venue audience. | High | Medium | Testers, parents, operators, or stakeholders find capture presentation too graphic or distressing. | Reduce | Use implication, off-screen transitions, content review, intensity settings, and age-appropriate playtests; confirm formal venue policy separately. |
| R-016 | Arm's-length defensive actions cause lunging, forceful swings, or boundary violations in VR. | High | Medium | Testers step toward the entity, punch, overreach, or collide with the play-space boundary. | Avoid | Trigger stun through a low-force deliberate action inside a conservative reach envelope; never require real impact or forward movement. |
| R-017 | Low-UI multi-item equipment becomes difficult to remember or select in VR. | High | Medium | Players cycle past tools, forget held equipment, drop objectives accidentally, or stare at controls under threat. | Reduce | Cap equipment at two or three items, use distinct physical/audio/haptic identities, and test visible holsters or a hybrid selector on target hardware. |
| R-018 | Environmental interactivity expands beyond what a 20-minute contract can teach or use. | Medium | High | Doors, lights, machinery, debris, and routes receive implementation work but rarely affect playtests. | Reduce | Give each contract a small interaction budget and require every system to support the objective, entity counterplay, or route choice. |
| R-019 | A trained entity model introduces nondeterminism, latency, hardware, data, explainability, or safety failures. | High | High | The model is discussed before a narrow role, dataset, offline evaluation, target runtime, fallback, or behavioral safety envelope exists. | Defer | Ship deterministic adaptive game AI for the MLP; later place any learned model behind bounded strategy outputs with deterministic validation and fallback. |
| R-020 | Physical crouching causes fatigue, inaccessible play, or tracking/collision problems. | High | Medium | Players avoid stealth, remain crouched too long, lose tracking, or cannot perform the action. | Reduce | Provide controller-assisted crouch with equivalent gameplay output, calibrate height, and avoid repeated mandatory crouch cycles. |
| R-021 | Movable cover creates physics exploits, obstruction, performance cost, or unsafe VR exertion. | High | Medium | Players block critical routes, trap the entity, lift heavy-looking objects unrealistically, or make forceful movements. | Reduce | Use a few constrained objects, validated destinations, conservative mass/interaction, route checks, and reset behavior. |
| R-022 | Heart-rate-driven breathing creates a positive feedback spiral: encounter raises stress, breathing raises detection, and detection causes another encounter. | High | High | Simulated or real stress remains elevated, recovery never becomes possible, or anxious players receive disproportionate pressure. | Reduce | Calibrate per player, cap contribution, decay simulated stress, require corroborating entity evidence, and let the pacing director preserve recovery windows. |
| R-023 | A visual stealth indicator causes tunneling, vection, eye strain, or horror readability problems. | High | Medium | Testers report discomfort, mistake the effect for damage, or cannot read the environment. | Reduce | Keep indicators optional and subtle, prefer world/tool/controller cues, avoid moving screen-space distortion, and comfort-test every visual treatment. |
| R-024 | An objective cart collapses the repeated transport loop or creates physics and navigation problems. | High | Medium | Players move all rods in one trip, block corridors, trap the Owl, lose the cart, or spend the run wrestling physics. | Defer | Keep the no-cart loop authoritative; test an optional low-capacity noisy cart separately with constrained routes and clean reset. |
| R-025 | Late-week scope leakage prevents Prototype 2 from producing a testable result. | High | High | Work begins on capture recovery, new tools, carts, biofeedback, learned AI, or environment systems before route adaptation passes its gate. | Avoid | Treat `24_prototype_02_scope.md` as the authority through 2026-07-24; preserve outside work but mark it Deferred and do not integrate it into PT2. |

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
