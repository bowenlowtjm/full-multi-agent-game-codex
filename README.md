# Pully — Multi-Agent Unity Game (Codex Run)

Unity 6 Android portrait arcade game built from the Pully spec.
Core loop: gesture-matching targets, combo scoring, lives, timed rounds, and full screen flow.

## Project snapshot
- Engine: Unity 6000.4.10f1
- Target: Android (debug APK)
- Main gameplay code: `Assets/_Game/`
- Tests: `Assets/Tests/EditMode`, `Assets/Tests/PlayMode`
- Local compile gate: `scripts/unity-check.sh`

## CI/CD workflows
Located in `.github/workflows/`:

- `ci.yml`
  - Runs EditMode + PlayMode tests
  - No separate compile-check job (EditMode already compiles)

- `build.yml`
  - Builds Android APK artifact on `main`
  - Uses minimal GameCI builder config:
    - `unityVersion`
    - `targetPlatform: Android`
    - `androidExportType: androidPackage`
    - `buildName: pully`

- `activate.yml`
  - Manual only (`workflow_dispatch`)
  - Used to generate Unity activation file (`*.alf`) when setting up license secrets

## Required GitHub secrets
Set in repo Settings → Secrets and variables → Actions:
- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

## Local development commands
From repo root:

1) Compile check (required after C# edits)
- `UNITY_BIN="/Applications/Unity/Hub/Editor/6000.4.10f1/Unity.app/Contents/MacOS/Unity" ./scripts/unity-check.sh`

2) EditMode tests
- `$UNITY_BIN -batchmode -quit -projectPath . -runTests -testPlatform EditMode -testResults Logs/editmode-results.xml -logFile Logs/editmode.log`

3) PlayMode tests
- `$UNITY_BIN -batchmode -quit -projectPath . -runTests -testPlatform PlayMode -testResults Logs/playmode-results.xml -logFile Logs/playmode.log`

4) Local Android build (batchmode)
- `$UNITY_BIN -batchmode -quit -projectPath . -executeMethod Pully.Editor.Builder.BuildAndroid -logFile Logs/build-android.log`

## Notes
- If local Android build fails with unsupported build target, install Android Build Support (SDK/NDK/OpenJDK) for this Unity editor in Unity Hub.
- Task tracking and run evidence are documented in `tasks/` and `docs/run-log.md`.
