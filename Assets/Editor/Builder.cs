using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Pully.Editor
{
    /// <summary>
    /// Batchmode build entry point for CI/CD APK production.
    /// Usage: Unity -batchmode -quit -executeMethod Pully.Editor.Builder.BuildAndroid
    /// </summary>
    public static class Builder
    {
        private static readonly string OutputDir = "Builds/Android";
        private const string ApkName = "pully.apk";

        public static void BuildAndroid()
        {
            Console.WriteLine("[Builder] Starting Android build...");

            // Ensure build settings are correct
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            EditorUserBuildSettings.buildAppBundle = false;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.saaavvyy.pully");
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 1;

            // Portrait orientation
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

            // Ensure scenes are in build settings
            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                Debug.LogError("[Builder] No scenes in Build Settings! Run BuildTools.GenerateCoreScenes first.");
                EditorApplication.Exit(1);
                return;
            }

            Console.WriteLine($"[Builder] Building {scenes.Length} scenes...");

            // Ensure output directory exists
            Directory.CreateDirectory(OutputDir);

            string outputPath = Path.Combine(OutputDir, ApkName);

            // Delete existing APK
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            // Build
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Console.WriteLine($"[Builder] Build succeeded: {summary.outputPath}");
                    Console.WriteLine($"[Builder] Size: {summary.totalSize / 1024 / 1024} MB");
                    Console.WriteLine($"[Builder] Time: {summary.totalTime.TotalSeconds:F1}s");
                    EditorApplication.Exit(0);
                    break;

                case BuildResult.Failed:
                    Debug.LogError("[Builder] Build failed!");
                    EditorApplication.Exit(1);
                    break;

                case BuildResult.Cancelled:
                    Debug.LogWarning("[Builder] Build cancelled.");
                    EditorApplication.Exit(1);
                    break;

                default:
                    Debug.LogError($"[Builder] Unknown build result: {summary.result}");
                    EditorApplication.Exit(1);
                    break;
            }
        }

        /// <summary>
        /// Development build with debug symbols.
        /// </summary>
        public static void BuildAndroidDebug()
        {
            Console.WriteLine("[Builder] Starting Android DEBUG build...");

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.allowDebugging = true;

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.saaavvyy.pully");
            PlayerSettings.bundleVersion = "1.0.0-debug";
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            Directory.CreateDirectory(OutputDir);
            string outputPath = Path.Combine(OutputDir, "pully-debug.apk");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);

            if (report.summary.result == BuildResult.Succeeded)
            {
                Console.WriteLine($"[Builder] Debug build succeeded: {report.summary.outputPath}");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError("[Builder] Debug build failed!");
                EditorApplication.Exit(1);
            }
        }
    }
}
