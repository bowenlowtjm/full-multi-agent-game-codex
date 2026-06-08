# QA Plan — Setup through Release Gates (M0-M4)

Scope: QA gate checklist for this run repo, aligned to `tasks/BOARD.md`, `docs/GOTCHAS.md`, `scripts/unity-check.sh`, `spec/ACCEPTANCE.md`, and PRD Section 7 verification checks.

References
- Board/index: `tasks/BOARD.md`
- Known traps: `docs/GOTCHAS.md`
- Compile gate script: `scripts/unity-check.sh`
- Acceptance gates: `/Users/bowenlow/Documents/Shared_Notes/Dev-Hermes-Pully/spec/ACCEPTANCE.md`
- PRD verification section (QA 01-04): `/Users/bowenlow/Documents/Shared_Notes/Dev-Hermes-Pully/Game Design PRD.md`

## Gate policy
- No task/milestone is marked done without artifact evidence.
- QA status is binary per gate: PASS / FAIL / BLOCKED.
- Fail-honest reporting is mandatory (template below).

## M0 Gate — Scaffold + Environment Readiness
Checklist
- [ ] Project compiles clean via `scripts/unity-check.sh` (no CS errors).
- [ ] Test framework dependencies resolve (EditMode + PlayMode assemblies compile).
- [ ] Unity Android toolchain is available (SDK/NDK/JDK not missing at build time).
- [ ] CI workflows exist and are configured (`.github/workflows/ci.yml`, `build.yml`).
- [ ] Board reflects QA tasking for M0 and onward.

Artifact evidence required
- `Logs/compile.log` (or unity-check output) showing CLEAN.
- Zero-error compile snippet or saved terminal output.
- Package/config evidence for test framework/input system (manifest diff or compile pass proof).
- Presence of workflow files with Unity secrets requirement documented.
- `tasks/BOARD.md` snapshot showing milestone/task state consistency.

## M1 Gate — Core Loop Functional Correctness
Checklist
- [ ] TG-01 Pop gesture recognized and scored correctly.
- [ ] TG-02 Heavy gesture recognized and scored correctly.
- [ ] TG-03 Charge hold behavior recognized and scored/fails correctly.
- [ ] TG-04 Trash drag behavior recognized and scored/fails correctly.
- [ ] Combo, life penalty, and miss/wrong-input handling works.
- [ ] Ruleset remains data-driven (from `RulesetDefinition`, no hardcoded mapping).
- [ ] EditMode and PlayMode tests exist for implemented behavior.

PRD verification checks (must be represented and tested)
- [ ] Verification 01: Tap on TG-03 breaks combo and registers failure.
- [ ] Verification 02: TG-04 drag outside trash zone resets/penalizes.
- [ ] Verification 03: >=60 FPS under dense spawn (>10 targets).
- [ ] Verification 04: High score persists across app restart.

Artifact evidence required
- PlayMode run evidence (test output and/or short capture) for TG-01..TG-04.
- Test reports including EditMode + PlayMode pass counts.
- Ruleset asset + code pointer showing data-driven mapping path.
- QA notes for each PRD verification item with PASS/FAIL and reproduction steps.

## M2 Gate — Scene Flow + UX Completeness
Checklist
- [ ] Full flow navigates: splash -> how-to-play -> menu -> settings -> game -> pause -> game-over -> menu.
- [ ] Settings include audio mute/volume controls and expected persistence behavior.
- [ ] Pause/resume/retry are stable and do not corrupt state.
- [ ] No placeholder UX blockers for intended user path.

Artifact evidence required
- End-to-end flow capture (video or step log with screenshots).
- Settings persistence evidence (before/after restart).
- Defect list for UX blockers with severity and disposition.

## M3 Gate — Performance, Stability, Determinism
Checklist
- [ ] Stable target performance (>=60 FPS on target profile for dense spawn).
- [ ] No crashes/softlocks in extended seeded sessions.
- [ ] Deterministic behavior validated under seeded runs where applicable.
- [ ] Background/foreground transitions handled safely.

Artifact evidence required
- FPS evidence (profiler/counter logs with scenario details).
- Soak test log (duration, seed set, crash/softlock count).
- Determinism comparison log (same seed => same key outcomes).
- Incident report for any instability with reproduction steps.

## M4 Gate — Release Readiness + Delivery
Checklist
- [ ] Android debug APK builds locally via `Editor/Builder.cs` batchmode.
- [ ] APK installs and launches on device/emulator.
- [ ] CI `ci.yml` is green for PR branch.
- [ ] `build.yml` produced APK artifact on main.
- [ ] Art/polish gate met per acceptance (icon/splash/audio/SFX/haptics/no programmer placeholders).
- [ ] Run-log final QA outcome is recorded honestly.

Artifact evidence required
- Local build log + APK path (`Builds/Android/pully.apk`).
- Install/launch proof (`adb install` output and launch confirmation).
- CI run URL/evidence for `ci.yml` pass.
- Build workflow artifact evidence for `build.yml` output.
- Final QA report attached to run-log entry (PASS/FAIL/BLOCKED + scope).

## Command checklist — compile/tests/build verification
Use repository root: `/Users/bowenlow/Documents/agent-game-explore/full-multi-agent-game-codex`

0) Preconditions
- Set one compile path:
  - Live editor mode: `export PULLY_REFRESH_PORT=8090` (or unique per run)
  - CLI mode: `export UNITY_BIN="/Applications/Unity/Hub/Editor/6000.4.10f1/Unity.app/Contents/MacOS/Unity"`

1) Compile gate (required after each C# edit)
- `./scripts/unity-check.sh`
- PASS condition: output contains `[unity-check] CLEAN` and no compiler errors in `Logs/compile.log`.

2) Local EditMode tests
- `$UNITY_BIN -batchmode -quit -projectPath . -runTests -testPlatform EditMode -testResults Logs/editmode-results.xml -logFile Logs/editmode.log`
- PASS condition: test runner result pass, no failed assertions.

3) Local PlayMode tests
- `$UNITY_BIN -batchmode -quit -projectPath . -runTests -testPlatform PlayMode -testResults Logs/playmode-results.xml -logFile Logs/playmode.log`
- PASS condition: test runner result pass, no failed assertions.

4) Local Android debug build (agent/local path)
- `$UNITY_BIN -batchmode -quit -projectPath . -executeMethod Builder.BuildAndroid -logFile Logs/build-android.log`
- PASS condition: build success log and output file at `Builds/Android/pully.apk`.

5) Install/launch smoke (device/emulator)
- `adb install -r Builds/Android/pully.apk`
- `adb shell monkey -p <applicationId> 1`
- PASS condition: install success + app launches without immediate crash.

6) CI/build verification (remote)
- Confirm `ci.yml` latest run is green for PR branch.
- Confirm `build.yml` latest `main` run has APK artifact (`build/Android` packaged upload).
- PASS condition: both checks green with retrievable artifacts.

## Fail-honest reporting template
Use this when any check fails or is blocked.

QA Gate Report
- Milestone/Gate: Mx - <name>
- Status: PASS | FAIL | BLOCKED
- Scope tested: <features/tests/build paths>
- Environment: <unity version, device/emulator, branch, commit>

Results
- Passed checks:
  - <item>
- Failed checks:
  - <item> - expected: <...> ; actual: <...>
- Blocked checks:
  - <item> - blocker: <reason>

Evidence
- Logs: <paths>
- Test reports: <paths or run links>
- Build artifact: <path or link>
- Capture/screenshot: <path or link>

Risk and impact
- User impact: <high/med/low + explanation>
- Release impact: <ship blocker or deferrable>

Next action recommendation
- Required fix(es): <owner + concrete action>
- Retest scope: <exact tests/checks to rerun>
- Honest summary statement: <what is done, what is missing, what is unstable>
