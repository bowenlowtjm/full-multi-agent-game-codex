# PM Status Report — Backlog Plan (M1–M4)

Date: 2026-06-09
Owner: game-pm
Sources: tasks/BOARD.md, tasks/T002-pm-backlog.md, docs/decisions.md, docs/run-log.md, AGENT-BRIEF, RUN-PROTOCOL, GAME-SPEC, ACCEPTANCE, PRD

## 1) Milestone-by-milestone task decomposition

M1 — Core loop playable + deterministic foundations
- T003 (game-logic): Core loop foundations (state, spawn, gesture, scoring).
  - Deliverables: 60s round, lives=3 penalties, combo reset on fail, deterministic seeded spawn path from RulesetDefinition.
- T004 (unity-scene): Bootstrap scene/state flow wiring for INIT/MAIN_MENU/GAMEPLAY/PAUSE/GAME_OVER.
  - Deliverables: playable loop entry from menu, pause/retry flow, HUD baseline.
- T006 (test-author, new): EditMode unit suite for scoring/combo/gesture classification/ruleset loading/determinism.
- T007 (test-author, new): PlayMode integration for input→gesture→score happy path + miss/wrong path.
- T008 (qa, new): M1 QA gate against ACCEPTANCE core-loop gates + PRD TG-01..TG-04 behavior sanity checks.

M2 — UX flow completeness + art integration + Android artifact
- T009 (unity-scene, new): Full screen flow implementation (splash/how-to-play/menu/settings/game/pause/game-over/menu).
- T010 (game-art, new): Replace placeholders with generated sprite set + atlas; consistent style/palette from DESIGN.md.
- T011 (game-logic, new): Persistent high score + settings persistence (audio/haptics/colorblind toggle, tutorial replay).
- T012 (build/ci, new): CI/build hardening (`ci.yml` green on PR, `build.yml` APK artifact on main).
- T013 (qa, new): M2 QA gate (flow completeness, art pass evidence, APK install/launch proof, lives/timer integration tests).

M3 — Balance, robustness, and gameplay-quality harness
- T014 (game-logic, new): Difficulty ramp + spawn fairness tuning (telegraphing, overlap safety, readability).
- T015 (test-author, new): Determinism replay tests + seeded multi-run scenario tests.
- T016 (qa, new): Bot-player + recorder + judge harness runbook and baseline scores.
- T017 (staff-engineer, new): Robustness pass (error handling, state edge cases, backgrounding safety).
- T018 (qa, new): M3 QA gate (stability/perf trend check, determinism evidence, softlock/crash sweep).

M4 — Release-quality polish (debug APK, release feel)
- T019 (unity-scene, new): First-run skippable tutorial + animated transitions + complete pause/settings UX polish.
- T020 (game-art, new): Final UI polish, button states, score popups, high-score celebration effects.
- T021 (game-logic, new): Audio + haptics system (BGM loop, SFX set, mute/volume controls, hit/miss haptic feedback).
- T022 (build/ci, new): Performance pass (60fps target, startup profiling, GC hitch reduction) + soak script.
- T023 (qa, new): Final acceptance gate (all ACCEPTANCE checks, uninstall test verdict, final self-report inputs).
- T024 (game-pm, new): Milestone ADR promotion and release-readiness documentation (`adr/`, run-log finalization, board truth check).

Notes on spec alignment
- Source-of-truth priority for implementation: GAME-SPEC + RULESET + ACCEPTANCE.
- PRD TG-01..TG-04 and Verification 01..04 are included as explicit QA checks/tasks (T008/T013/T018/T023) and mapped without violating 5-gesture authoritative spec.

## 2) Critical path

1. T005 (M0 QA preflight) must pass before M1 coding expansion.
2. M1 functional baseline: T003 + T004 -> T006/T007 -> T008 QA gate.
3. M2 completion path: T009 + T010 + T011 + T012 -> T013 QA gate (APK/install proof required).
4. M3 readiness path: T014 + T015 + T016 + T017 -> T018 QA gate.
5. M4 release path: T019 + T020 + T021 + T022 -> T023 final QA -> T024 docs/ADR closure.

Hard blockers on critical path
- Data-driven RulesetDefinition compliance (cannot proceed if hardcoded mapping appears).
- CI red status (cannot merge milestone completion).
- Missing APK artifact or install proof at M2+.
- Gameplay quality < 8/10 or polish gate misses at M4.

## 3) Immediate next 5 executable tasks with owners

1. T005 — QA gate for M0 scaffold (owner: qa)
   - Execute headless compile/test/CI preflight and log risks.
2. T003 — Core loop foundations implementation start (owner: game-logic)
   - Implement deterministic spawner + ruleset-driven gesture validation + score/lives/combo core.
3. T004 — Scene/bootstrap flow baseline (owner: unity-scene)
   - Wire INIT→MENU→GAMEPLAY→PAUSE→GAME_OVER transitions and HUD shell.
4. T006 — EditMode unit tests scaffold and first pass (owner: test-author)
   - Add scoring/combo/ruleset/determinism tests with seeded fixtures.
5. T007 — PlayMode integration seed test (owner: test-author)
   - Validate one input→score path + one failure path (combo reset/life penalty).

## 4) Risks and mitigations

1. Spec mismatch risk (PRD vs GAME-SPEC gesture details)
- Risk: PRD defines Tap/DoubleTap/LongPress/Drag variants that can conflict with 5-gesture canonical spec.
- Mitigation: enforce GAME-SPEC/RULESET as implementation authority; keep PRD checks as QA compatibility scenarios only.

2. Input ambiguity under multi-touch and drag/tap overlap
- Risk: gesture misclassification (especially swipe-tap vs tap; drag threshold).
- Mitigation: explicit gesture thresholds, conflict-resolution tests, seeded PlayMode edge-case suite (T006/T007/T015).

3. Art pipeline blocking functional progress
- Risk: waiting on final sprites slows gameplay/testing.
- Mitigation: strict placeholder-first in M1/M2; art replacement isolated in T010/T020 with atlas checks.

4. Performance/polish gate failure late in M4
- Risk: functional game passes but fails 60fps/juice/audio/polish gates.
- Mitigation: start perf telemetry in M3, not M4; integrate audio/haptics no later than early M4; run periodic QA sweeps.

5. CI/build drift and false "done"
- Risk: local pass but CI/build.yml red or missing APK artifact evidence.
- Mitigation: milestone definition includes QA gate with explicit artifact checklist and run-log evidence before status=done.