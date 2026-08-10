# Authority, Scope, and Tool Boundaries

Last verified: 2026-08-02

## Authority Order

When sources disagree, follow this order:

1. The newest explicit instruction from the user.
2. The newest version-oriented implementation brief explicitly approved by the user.
3. Active decisions in `../05_decision_log.md`.
4. The active contract specification in `../09_first_playable_spec.md`.
5. Architecture constraints in `../10_technical_architecture_outline.md`.
6. Implemented evidence in `../06_prototype_log.md`.
7. Historical prototype briefs and handoffs.
8. Broad design synthesis, sprint planning, frameworks, and product direction.
9. Workspaces, examples, backlogs, suggestions, parking-lot ideas, and speculative notes.

Do not resolve a conflict by selecting the largest or most exciting interpretation. Surface the conflict and ask.

## Implementation Authority Is Not Design Authority

The implementation agent may:

- implement behavior the user or an approved implementation brief defines;
- select low-level technical details that do not alter the defined player experience;
- refactor locally when required to implement or safely test the approved change;
- add focused automated tests;
- fix an explicitly reported defect while preserving intended behavior;
- update implementation evidence and technical handoff notes;
- record a design decision only after the user has actually made or approved it.

The implementation agent may not independently:

- invent a feature, mechanic, enemy, mission, route, tool, objective, failure state, reward, or content family;
- decide player-facing controls, tuning, messaging, affordances, difficulty, or feedback semantics when unspecified;
- convert a proposed idea or backlog item into active scope;
- broaden a narrow task into a generalized framework;
- redesign capture, recovery, stealth, inventory, haptics, biofeedback, progression, or AI because related code exists;
- rewrite game-design documents to make an implementation choice appear approved;
- add “helpful” presentation or balance changes that affect the experiment without permission.

The user can directly specify a design change and ask the agent to implement it. In that case, the user is making the design decision; the agent is still implementing rather than inventing.

## When to Ask

Ask before implementation when any of the following is missing or contradictory:

- the observable player-facing result;
- whether an idea is in or out of the current version;
- a control binding or interaction model;
- a tuning value that materially changes difficulty, fairness, comfort, or pacing;
- success and failure behavior;
- compatibility with an existing active decision;
- whether a current dirty asset belongs in the task;
- whether an apparent defect is intentional design;
- whether a change should be versioned, built, committed, or pushed.

Do not ask about routine choices such as private method names, a small pure helper, ordinary null guards, test structure, or an implementation detail already settled by local patterns.

## Unity MCP Boundary

Unity MCP is the only authorized control surface for Unity Editor state.

### Must use Unity MCP

- Scene open/save/create operations.
- GameObject and component creation, deletion, configuration, parenting, and transform changes.
- Prefabs, materials, meshes, textures, animation assets, ScriptableObjects, and other serialized Unity assets.
- Project Settings, Player Settings, Quality/URP settings, packages, build settings, and platform settings.
- Unity import, refresh, compilation observation, and console inspection.
- Play Mode control and runtime inspection.
- Edit Mode and Play Mode Test Runner execution.
- Game/Scene view screenshots and editor visual inspection.
- Unity builds and editor-side build reporting.

### File editing is allowed

- C# runtime source.
- C# editor tooling.
- C# test source.
- Markdown implementation documentation and evidence.

After C# editing, the agent must return to Unity MCP, wait for compilation, and read the console.

If a non-C# textual Unity file such as an assembly definition, package manifest, or generated configuration appears to require direct editing, first use the applicable Unity MCP capability. If MCP cannot perform the operation, ask the user before using a file-level substitute.

### Forbidden substitutes

- Directly hand-editing `.unity`, `.prefab`, `.asset`, or `ProjectSettings` YAML.
- Launching or controlling a different Unity instance from the shell.
- Using GUI automation, keystroke automation, or external scripts to bypass MCP.
- Treating a successful text edit as proof that Unity imported or serialized it correctly.
- Claiming a scene, test, build, or runtime behavior was verified without MCP evidence.

If MCP is offline, the agent may continue read-only code/doc analysis or draft C# changes only when safe, but must not claim Unity completion. For Unity mutations, it should pause and ask the user to start or reconnect the server.

## Repository and Release Authority

- Start every task with a repository audit.
- Preserve staged, modified, untracked, ignored, and generated files outside the approved scope.
- Never reset, checkout, clean, overwrite, or delete unrelated work.
- Do not assume the current branch is disposable.
- Do not commit or push unless requested.
- The standing project convention is that a future unqualified “commit” request includes push. The exact intended paths still need to be audited before staging.
- Do not create a new repository or remote when the existing repository is present.
- Do not change versions merely because a change feels major or minor.
- Do not include builds, captures, or large artifacts unless the repository rules and user instruction call for them.

## Current Scope at Handoff

As of 2026-08-10, `v0.3.0.0` Readable Stealth and Contractor Counterplay is complete, tracked, and automated delivery-validated; its human evidence remains open. It is a protected regression baseline rather than active feature scope.

`v0.4.0.0` Spatial Readability and Pursuit Topology is also implemented and tracked. `../37_v0_4_0_0_spatial_readability_scope.md` defines its player-facing contract, `../38_v0_4_0_0_7c_level_plan.md` defines the greybox data, and `../39_v0_4_0_0_implementation_handoff.md` records its completed task order and validation gate. No later feature version is authorized without a newer direct instruction or explicitly approved brief.

Prototype 2 and v0.3 feature expansion remain closed. Their human validation continues separately and does not authorize polish or redesign.

The following remain non-authoritative unless a newer explicit user instruction promotes them:

- exploratory material in `../31_pt3_design_workspace.md` and discarded 7-series concept alternatives;
- pending decisions in `../05_decision_log.md`;
- future sections of the adaptive AI, tactics, contract, capture, hardware, or biofeedback documents;
- backlog entries and prototype suggestions;
- code stubs that exist solely to preserve future seams.
