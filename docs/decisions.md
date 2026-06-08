# decisions.md — ADR-lite (running log)

One short entry per non-obvious decision (architecture, ruleset tuning, art direction, build/signing). Required at L3/L4 (the human isn't there to ask).

> At each **milestone checkpoint / major architectural fork**, promote the *significant architectural* entries here into formal ADRs in `adr/` (see `$SPEC_REPO/12-ADR-Process.md`). Trivial/reversible choices stay in this log.

## Template
```
### <date> — <decision title>
- Context: <what forced a choice>
- Options: <a / b / c>
- Decision: <chosen> by <role/rung>
- Rationale: <why>
- Reversible? <yes/no; how>
```

## Decisions
### 2026-06-09 — Unity check fallback for Unity 6 batch startup
- Context: `scripts/unity-check.sh` attempted `-executeMethod AutoRefresher.RefreshAndExit`, but Unity 6000.4.10f1 intermittently could not resolve the class in batch startup despite clean compilation.
- Options: (a) fail hard and block run startup, (b) remove executeMethod entirely, (c) keep executeMethod as preferred path and add plain batchmode fallback.
- Decision: (c) chosen by orchestrator (Config B/L3 run bootstrap).
- Rationale: preserves fast explicit compile path when available while keeping unattended reliability in this editor version.
- Reversible? yes; revert once executeMethod is stable in environment.

### 2026-06-09 — Start this run with local task board (Config B)
- Context: AGENT-BRIEF + RUN-PROTOCOL require local markdown tasks for run execution and milestone tracking.
- Options: (a) external tracker only, (b) local board only, (c) both.
- Decision: (b) local board as source of truth for bootstrap and milestone entry.
- Rationale: deterministic, repo-local, no external auth dependency.
- Reversible? yes; can mirror tasks to Linear later if requested.
