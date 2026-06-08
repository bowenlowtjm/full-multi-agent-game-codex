# tasks/ — local task board (replaces Linear)

Coordination is **local markdown** — no external tracker, no API key. **Source of truth = one file per task:** `tasks/T###-slug.md` (frontmatter holds status/owner/etc). This file is the human-readable **index**; the **Game PM keeps it in sync** with the task files.

One-file-per-task (not a single shared board) is deliberate: parallel role worktrees can each edit their own task file without merge conflicts.

## Statuses
`todo` → `in-progress` → `in-review` → `done`  (or `blocked`).
**Who may set `status: done` is the autonomy gate** — a human at **L1**, the team itself at **L3/L4** (see [Autonomy Ladder](../../05-Autonomy-Ladder.md)). `in-review` = QA gate running.

## Board (index — keep in sync with the task files)
| ID | Title | Status | Milestone | Owner | Branch |
|----|-------|--------|-----------|-------|--------|
| T001 | Run scaffold + baseline compile loop | done | M0 | orchestrator | chore/T001-scaffold |
| T002 | PM backlog and milestone decomposition from AGENT-BRIEF+PRD | done | M0 | game-pm | feature/T002-pm-backlog |
| T003 | Core loop foundations (state, spawning, gesture, scoring) | in-progress | M1 | game-logic | feature/T003-core-loop |
| T004 | Scene + bootstrap flow (menu/game/pause/gameover wiring) | todo | M1 | unity-scene | feature/T004-scene-flow |
| T005 | QA gate for M0 scaffold (compile/tests/ci preflight) | todo | M0 | qa | chore/T005-m0-qa |

## How to use
- **Create a task:** copy `TEMPLATE.md` → `T###-slug.md`, fill the frontmatter, add a row here.
- **Update status:** edit the task file's `status:` field **and** its row here (keep them consistent).
- Each **branch / PR / commit references `T###`**. Significant changes (incl. status→done, build, blocker) → Discord + `docs/run-log.md`.
- No fake `done`: a task is `done` only with a verification artifact (clean compile, passing test, APK path, atlas…).
