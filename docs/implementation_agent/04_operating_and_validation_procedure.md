# Operating and Validation Procedure

Last verified: 2026-07-28

## 1. Establish Authority

Before changing anything, write a compact task record:

```text
Approved task:
Authority source:
Version, if specified:
Relevant decision IDs:
Relevant specification sections:
Observable behavior:
Acceptance criteria:
Systems likely affected:
Required fallback or comparison toggle:
Open question:
```

If the open question changes player-facing behavior, scope, comfort, fairness, pacing, or experiment validity, ask the user before implementing.

## 2. Audit the Repository

Inspect:

- current branch and `HEAD`;
- relationship to the configured remote;
- staged, modified, deleted, untracked, and ignored files;
- current Unity bundle version;
- relevant recent commits;
- relevant implementation and tests.

Do not rely on a previous agent’s status note when the live repository contradicts it. Do not stage or alter unrelated dirty files.

## 3. Connect Through Unity MCP

Use the resource-first workflow:

1. Read `mcpforunity://editor/state`.
2. Confirm the connected project and intended editor instance.
3. Confirm active scene and Play Mode state.
4. Inspect relevant scene/object resources before searching broadly.
5. Read the console and compilation status.
6. Leave Play Mode before structural scene edits.

If the server is offline or connected to the wrong Unity project, pause Unity mutations and ask the user to correct it.

## 4. Establish a Green Baseline

For a defect, reproduce it before changing code when practical.

For a feature or tuning task:

- run the narrow existing tests nearest to the affected system;
- inspect the relevant runtime state or scene object;
- record any pre-existing failures separately;
- do not silently absorb unrelated failures into the task.

If the baseline is already red, determine whether the failure blocks trustworthy implementation. Fix it only if it is in scope or request direction.

## 5. Implement a Small Coherent Slice

- Prefer the existing contract and event seams.
- Put calculation/state-transition logic outside MonoBehaviours when practical.
- Keep Unity integration adapters thin.
- Avoid unrelated renames, formatting churn, and architecture rewrites.
- For generated facility changes, alter the builder/specification and rebuild only the generated root through Unity MCP.
- Preserve manual scene dressing outside the generated root.
- Keep reset and deterministic seed behavior in the same slice.
- Add tests alongside behavior rather than after the whole feature.

C# files may be edited with normal text tools. Scene, prefab, asset, settings, package, Play Mode, test, screenshot, and build operations stay in Unity MCP.

## 6. Compile and Inspect Immediately

After each coherent C# edit:

1. Allow Unity to import and compile through MCP.
2. Wait until the editor is no longer updating or compiling.
3. Read console errors and warnings.
4. Resolve project-code errors before stacking more changes.

Do not use a successful external syntax check as a substitute for Unity compilation.

## 7. Validate in Layers

### Pure/Edit Mode

Use focused Edit Mode tests for:

- state transitions;
- deterministic seeds;
- evidence weights/decay/rate limits;
- geometry and posture math;
- target selection and occlusion logic;
- physics-state capture/restore calculations;
- architecture dependencies and contract seams.

### Integrated Play Mode

Use Play Mode tests for:

- scene wiring and missing references;
- generated anchors and portal clearances;
- player/Owl route connectivity;
- actual grab/drop/deposit integration;
- objective, extraction, failure, and reset flow;
- NavMesh behavior;
- camera, collider, and perception integration;
- current serialized tuning values.

### Runtime visual inspection

For any scene, UI, lighting, camera, portal, material, or animation change:

- enter Play Mode through MCP;
- view it from the player’s actual camera at representative locations;
- inspect at intended resolution, including fullscreen when relevant;
- check normal, crouched, leaned, and moving viewpoints when affected;
- capture screenshots as evidence;
- check console again after leaving Play Mode.

Automated geometry tests do not prove readability or feel.

### Build and smoke test

When the user requests a release candidate or the task changes platform/build-sensitive behavior:

- produce the requested target through Unity MCP;
- use a fresh versioned output path;
- report build duration, size, errors, and warnings;
- launch/smoke test when practical and authorized by the environment;
- distinguish “process stayed alive” from “gameplay was manually completed”;
- inspect the player log for project exceptions, missing scripts, XR startup failure, serialization errors, and invalid NavMesh behavior.

## 8. Validate Reset and Comparisons

Every gameplay change should consider:

- current-seed restart;
- new-seed restart;
- run failure and success;
- leaving and re-entering affected zones;
- held/released/deposited object state;
- Owl state and memory;
- pause/cursor/fullscreen state when relevant;
- F1/F2/F3 operator comparison controls when relevant.

If a feature has a baseline/comparison toggle, test both states.

## 9. Document Evidence

Update `../06_prototype_log.md` after a real implementation pass with:

- prototype question;
- applicable design pillar or acceptance criterion;
- behavior changed;
- automated evidence;
- build/smoke evidence;
- human validation still required;
- scope confirmation.

Update `../05_decision_log.md` only for a user-approved design, scope, version, or architecture decision. Do not log ordinary code choices as game-design decisions.

Do not modify exploratory game-design documents to “complete” them.

## 10. Git and Delivery

Before staging:

- re-run status;
- identify only task-owned files;
- review the diff;
- exclude unrelated user or agent work;
- verify no generated/large artifacts slipped in unintentionally.

Do not commit or push without the user’s request. When the user says “commit” without a narrower instruction, this project’s standing convention includes pushing the resulting commit.

After a requested commit/push, report:

- exact commit hash and subject;
- branch;
- remote push result;
- any remaining dirty files;
- build artifact status if relevant.

## Final Report Template

```text
Approved task and authority:
Implemented behavior:
Files changed:
Unity scenes/assets/settings changed:
Tests added or updated:
Edit Mode result:
Play Mode result:
Console result:
Build result:
Smoke-test result:
Technical assumptions:
Human validation remaining:
Blockers or deferred work:
Out-of-scope confirmation:
Version/commit/push, if requested:
```

Never turn “not verified” into “works.” State environmental limitations plainly.

