# Final Internship Report Handoff

Prepared: 2026-08-26, Asia/Dubai

This folder separates source collection from final-report writing so that agent memory, repository evidence, and the user's own experience are not accidentally blended into unsupported claims.

## Files

- `implementation_agent_knowledge_record.md` — the implementation agent's evidence-labelled record of the project, including chronology, technical work, problems, attribution, and known gaps.
- `other_agent_knowledge_capture_prompt.md` — a reusable prompt for the game-design and level-design agents. Give each agent a separate copy and ask it to save a role-specific record.
- `final_report_agent_master_prompt.md` — the synthesis and document-production prompt for the dedicated report agent after the official template and samples are supplied.

Recommended additional inputs:

- `agent_records/game_design_agent_knowledge_record.md`
- `agent_records/level_design_agent_knowledge_record.md`
- the official final-report brief, rubric, template, and samples;
- the three bi-weekly reports;
- any supervisor feedback, presentation material, screenshots, videos, timesheets, or personal notes the user wants represented.

## Recommended workflow

1. Run `other_agent_knowledge_capture_prompt.md` separately with the game-design and level-design agents.
2. Place their outputs under `agent_records/` without merging them.
3. Attach the official final-report requirements and samples to a dedicated report-agent task.
4. Give that agent `final_report_agent_master_prompt.md` and this entire folder.
5. Let the report agent conduct its own read-only audit, build an evidence matrix, and identify conflicts or missing personal information.
6. Review the outline before generating the polished document if the official format leaves major content choices open.
7. Render and visually verify the final document before submission.

Agent records are evidence leads, not automatic truth. Repository state and user testimony should be used to confirm them. Conversely, the repository cannot capture every design discussion, manual Unity operation, learning outcome, or personal reflection, which is why the role-specific records matter.
