using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildTools
{
    [MenuItem("Pully/Generate Core Scenes")]
    public static void GenerateCoreScenes()
    {
        EnsureScene("Assets/_Game/Scenes/SplashScene.unity", "SplashRoot", "Pully.Game.SplashController");
        EnsureScene("Assets/_Game/Scenes/TutorialScene.unity", "TutorialRoot", "Pully.Game.TutorialController");
        EnsureScene("Assets/_Game/Scenes/MenuScene.unity", "MenuRoot", "Pully.Game.MenuController");
        EnsureScene("Assets/_Game/Scenes/SettingsScene.unity", "SettingsRoot", "Pully.Game.SettingsController");
        EnsureScene("Assets/_Game/Scenes/GameScene.unity", "GameRoot", "Pully.Game.GameSceneController");
        EnsureScene("Assets/_Game/Scenes/GameOverScene.unity", "GameOverRoot", "Pully.Game.GameOverController");

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/_Game/Scenes/SplashScene.unity", true),
            new EditorBuildSettingsScene("Assets/_Game/Scenes/TutorialScene.unity", true),
            new EditorBuildSettingsScene("Assets/_Game/Scenes/MenuScene.unity", true),
            new EditorBuildSettingsScene("Assets/_Game/Scenes/SettingsScene.unity", true),
            new EditorBuildSettingsScene("Assets/_Game/Scenes/GameScene.unity", true),
            new EditorBuildSettingsScene("Assets/_Game/Scenes/GameOverScene.unity", true)
        };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[BuildTools] Core scenes generated and Build Settings updated.");
    }

    private static void EnsureScene(string path, string rootName, string componentTypeName)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var cameraGo = new GameObject("Main Camera");
        cameraGo.tag = "MainCamera";
        var cam = cameraGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.11f, 0.12f, 0.23f);

        var root = new GameObject(rootName);
        var t = System.Type.GetType(componentTypeName + ", Pully.Game");
        if (t != null)
        {
            root.AddComponent(t);
        }
        else
        {
            Debug.LogWarning($"[BuildTools] Could not find component {componentTypeName} in Pully.Game");
        }

        var dir = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
        EditorSceneManager.SaveScene(scene, path);
    }
}
