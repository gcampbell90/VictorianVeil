using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    static string playerScene = "PlayerScene";
    static string uiScene = "UIScene";
    static string menuScene = "MenuScene";
    static string inventoryScene = "InventoryManagement";
    static string roomScene = "VictorianRoom";

    static string itemScene = "Debug_InteractableItems";
    static string puzzleScene = "Debug_Puzzles";

    List<string> sceneList = new List<string>() { playerScene, uiScene, menuScene, inventoryScene, roomScene, itemScene, puzzleScene };
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //Editor Mode
        if (!EditorApplication.isPlaying)
        {
            for (int i = 0; i < sceneList.Count; i++)
            {
                var scene = SceneManager.GetSceneByPath(GetScenePath(sceneList[i]));

                if (!scene.isLoaded)
                {
                    if (GUILayout.Button($"Open {sceneList[i]}"))
                    {
                        EditorSceneManager.OpenScene($"{GetScenePath(sceneList[i])}", OpenSceneMode.Additive);
                    }
                }
                else
                {
                    if (GUILayout.Button($"Close {scene.name}"))
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                }
            }
        }
        else if (EditorApplication.isPlaying)
        {
            GameManager gameManager = (GameManager)target;
            var sceneManager = gameManager.sceneLoadManager;

            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scene = SceneManager.GetSceneByBuildIndex(i);

                if (!scene.isLoaded)
                {
                    if (GUILayout.Button($"Load {sceneList[i-1]}"))
                    {
                        SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive);
                    }
                }
                else
                {
                    if (GUILayout.Button($"Unload {scene.name}"))
                    {
                        SceneManager.UnloadSceneAsync(i);
                    }
                }
            }
        }

    }

    private string GetScenePath(string sceneName)
    {
        var scenePath = $"Assets/Scenes/{sceneName}.unity";
        return scenePath;
    }
}
