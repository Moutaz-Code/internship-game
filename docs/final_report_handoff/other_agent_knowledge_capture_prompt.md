# Prompt: Produce Your Specialist Knowledge Record for the Final Internship Report

Copy this prompt into a separate task with the game-design agent, then again with the level-design agent. Replace the bracketed role and output name each time.

---

You are the **[GAME DESIGN / LEVEL DESIGN] specialist agent** for Moutaz's eight-week Pixoul internship game project. The internship began on 29 June 2026 and the final report is now being prepared.

Your assignment is not to write the final report. Your assignment is to preserve what you know—especially information that may exist only in your conversational context, reasoning history, design decisions, rejected options, or interpretations of project evidence—so that a dedicated report agent can later synthesize an accurate report.

Create a detailed Markdown knowledge record at:

`docs/final_report_handoff/agent_records/[game_design_or_level_design]_agent_knowledge_record.md`

Do not overwrite another agent's record. Do not edit Unity assets, project code, existing design documentation, Git history, or report artifacts. Perform read-only inspection only, apart from creating your one knowledge-record file.

## First: establish your evidence boundary

Before writing, audit the relevant project material that is available to you. At minimum:

- inspect current Git branch, HEAD, status, and dated commit history;
- read your root-level handoff prompt completely;
- read `docs/06_prototype_log.md`, `docs/05_decision_log.md`, `docs/14_current_design_synthesis.md`, and the most relevant role-specific documents;
- inspect the approved-map references and P02 prompts if they fall within your role;
- inspect existing captures, plans, diagrams, validation sheets, and report artifacts that support your recollection;
- note when current live state differs from a dated document.

Treat every modified or untracked file as user-owned. Do not reset, clean, overwrite, commit, push, or publish.

Use these labels throughout your record:

- **Repository-verified** — directly supported by current or historical project files/Git.
- **Recorded validation** — supported by exact tests, builds, captures, observations, or logs.
- **User-reported** — the user told you this directly.
- **Agent-context recollection** — remembered from your own conversation but not independently proven.
- **Inference** — your interpretation, clearly marked as such.
- **Proposal/rejected option** — explored but not approved or implemented.
- **Not Tested / Unknown** — the evidence does not support a stronger statement.

Do not convert inference, intended design, generated concept art, an implementation request, or a proposed feature into a completed-work claim.

## What to capture

Write everything materially useful to an internship report, including facts that are probably not obvious from the repository alone.

### 1. Your role and scope

- What responsibilities did you hold?
- What did the user expect from your specialist role?
- What authority or boundaries governed your work?
- How did you interact with the user and the other specialist agents?

### 2. Project understanding

- Summarize the game concept, player fantasy, intended audience/tone, core loop, target platform, and project goal as you understood them.
- Separate the full product direction from the smaller internship proof contract.
- Record stable design laws and constraints that shaped decisions.

### 3. Chronology

Build the most accurate week-by-week or milestone-by-milestone record you can for all eight weeks:

- Week 1: 29 June–3 July 2026
- Week 2: 6–10 July
- Week 3: 13–17 July
- Week 4: 20–24 July
- Week 5: 27–31 July
- Week 6: 3–7 August
- Week 7: 10–14 August
- Week 8: 17–21 August

For each period, record:

- the question or problem being addressed;
- decisions and alternatives considered;
- outputs produced;
- what was approved, implemented, rejected, superseded, cut, or left pending;
- exact source paths or commit references where available;
- what the student did personally versus what an agent generated or analyzed;
- important learning or difficulty suggested by the work.

If Git commit dates cross week boundaries, say so. Do not force an activity into a week merely to fill space.

### 4. Decisions and reasoning that may not survive in files

Record:

- why important mechanics, layouts, themes, or workflows were chosen;
- alternatives that were rejected and why;
- tensions such as scope versus polish, readability versus horror, organic shape versus modular construction, adaptation versus fairness, desktop evidence versus VR ambition, or generative inspiration versus topology authority;
- user preferences and moments when the user corrected or overruled an agent;
- any terminology whose meaning changed during the project.

This section is especially important because a repository can show what exists but not always why.

### 5. Student contribution and agent contribution

Give a careful attribution map:

- work performed manually by the student;
- decisions owned by the student;
- work drafted, generated, analyzed, or automated by agents;
- work iterated jointly;
- areas where attribution is uncertain and must be confirmed with the user.

Never erase the student's directing, evaluating, integrating, or manual-authoring role. Equally, do not hide AI assistance if the final report or institution requires disclosure.

### 6. Problems, changes, and lessons

For each meaningful problem:

- state the symptom or design failure;
- state how it was diagnosed;
- state the chosen response;
- state whether it was actually resolved, partially resolved, rejected, or remains open;
- state the skill or lesson demonstrated;
- cite evidence if possible.

Include unsuccessful iterations and revisions. They often make stronger internship-report material than a perfect retrospective.

### 7. Deliverables and evidence

Inventory role-relevant outputs:

- documents, diagrams, floor plans, overlays, prompts, specifications, scene content, captures, tests, builds, presentations, videos, or validation sheets;
- exact file paths;
- dates or versions;
- whether each is durable source, generated output, historical material, or current unfinished work.

### 8. Human and experiential evidence

State exactly what was observed by a person and what was not. Keep separate:

- automated tests;
- operator-controlled desktop inspection;
- uninformed desktop playtesting;
- headset/XR testing;
- physical hardware/venue testing;
- subjective approval or feedback.

Do not claim readability, fairness, comfort, fear, fun, or understanding from structural tests alone.

### 9. Report themes and reflections

Suggest evidence-supported themes the report agent could develop, such as:

- design iteration;
- spatial reasoning;
- balancing player information and horror;
- modularity and reuse;
- scope control;
- testing and evidence discipline;
- multidisciplinary collaboration;
- AI-assisted workflow;
- professional communication;
- lessons that connect to university study.

Phrase personal-reflection points as questions or leads unless the student explicitly stated the reflection.

### 10. Contradictions, unknowns, and questions for the student

List every material conflict between:

- your recollection and the repository;
- old and new design documents;
- intended and implemented work;
- automated evidence and human evidence;
- user-authored and agent-authored work.

Then list concise questions the report agent should ask the student rather than guessing.

### 11. Report-ready claim table

End with a table containing:

| Candidate claim | Evidence/source | Confidence | Required qualification | Suggested report section |
|---|---|---|---|---|

Use confidence values `High`, `Medium`, or `Low`. Low-confidence items must not appear in the final report without confirmation.

## Writing rules

- Be comprehensive but factual.
- Prefer exact dates, file paths, decision IDs, version names, counts, and artifact names.
- Quote very little; summarize instead.
- Preserve chronology and attribution.
- Explain specialist terminology in plain language.
- Mark stale documents and superseded proposals.
- Do not make new design decisions.
- Do not implement anything.
- Do not draft polished first-person reflections for the student unless they were explicitly stated.
- Do not include passwords, private tokens, unrelated personal data, or signature images.
- Do not commit or push.

## Final response

After saving the record, report:

- the output path;
- the sources you inspected;
- the three most important context-only facts you preserved;
- the largest remaining uncertainty;
- confirmation that no project files were otherwise changed and no commit/push occurred.

---
