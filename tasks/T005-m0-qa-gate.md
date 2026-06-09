---
id: T005
title: QA gate for M0 scaffold (compile/tests/ci preflight)
status: done
milestone: M0
owner: qa
depends_on: [T001, T002]
branch: chore/T005-m0-qa
---

## Goal
Run independent M0 preflight checks before entering milestone feature implementation.

## Acceptance
- [x] Headless compile check is clean.
- [x] Test asmdef references resolve with com.unity.test-framework present.
- [x] CI workflow definitions present and valid.
- [x] Known risks and assumptions documented in run log.

## Artifact Evidence
- Compile gate: `UNITY_BIN="/Applications/Unity/Hub/Editor/6000.4.10f1/Unity.app/Contents/MacOS/Unity" ./scripts/unity-check.sh` => `[unity-check] CLEAN` (fallback path handled AutoRefresher miss and retried plain batch compile).
- Compiler log: `Logs/compile.log` contains no `error CSxxxx` and no "Scripts have compiler errors" marker.
- Test framework wiring: `Packages/manifest.json` includes `com.unity.test-framework: 1.6.0`; test asmdefs reference `UnityEngine.TestRunner` and `nunit.framework.dll`.
- CI workflow presence: `.github/workflows/ci.yml` and `.github/workflows/build.yml` both present.
- Run-log coverage: `docs/run-log.md` includes scaffold compile/test-framework mitigation + QA-plan and subsequent clean compile entries.

## Notes
- QA ownership is independent from implementation tasks.
- Environment caveat: batch compile cannot run while another Unity instance has this same project open; resolved by closing project Unity process before gate run.
