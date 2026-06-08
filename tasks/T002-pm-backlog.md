---
id: T002
title: PM backlog and milestone decomposition from AGENT-BRIEF+PRD
status: done
milestone: M0
owner: game-pm
depends_on: [T001]
branch: feature/T002-pm-backlog
---

## Goal
Translate AGENT-BRIEF, RUN-PROTOCOL, GAME-SPEC, ACCEPTANCE, and PRD into an execution-ready local task backlog for Config B multi-agent delivery.

## Acceptance
- [ ] M0-M4 task map exists with ownership and dependencies.
- [ ] Priority order matches milestone requirements in RUN-PROTOCOL.
- [ ] QA gates represented at milestone boundaries.
- [ ] Board + task files are in sync.

## Notes
- Config B role ownership: orchestrator + game-pm + game-art + qa.
- PRD-specific checks (TG-01..TG-04 and verification 01..04) must be represented as QA tasks.

## Delivered
- Produced `docs/pm-status-report.md` with actionable M1–M4 backlog decomposition, critical path, immediate next 5 executable tasks with owners, and risks/mitigations.
- Aligned milestone ordering to RUN-PROTOCOL and ACCEPTANCE gates.
- Incorporated PRD TG-01..TG-04 and Verification 01..04 into QA gate planning while preserving GAME-SPEC as implementation authority.
- Synced task status to `done` and updated `tasks/BOARD.md` row for T002.
