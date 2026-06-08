---
id: T001
title: Run scaffold + baseline compile loop
status: done
milestone: M0
owner: orchestrator
depends_on: []
branch: chore/T001-scaffold
---

## Goal
Initialize a fresh run repository from the Pully templates and verify the headless compile loop works in this environment.

## Acceptance
- [x] Template payload copied into run repo root.
- [x] Unity project initialized (ProjectSettings/ + Packages/ created).
- [x] `scripts/unity-check.sh` executes clean in CLI mode.
- [x] Package dependencies updated so test assemblies resolve NUnit/TestFramework.

## Notes
- Unity 6.0.4f1 batchmode did not resolve `-executeMethod AutoRefresher.RefreshAndExit` during startup; compile wrapper now falls back to plain batch compile and still parses CS errors.
- Related decision logged in `docs/decisions.md`.
