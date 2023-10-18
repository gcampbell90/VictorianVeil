using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    private List<string> _initSceneQueue = new List<string> {
        "PlayerScene", "UIScene", "MenuScene"
    };
    private List<string> _gameSceneQueue = new List<string> {
        "InventoryManagement", "VictorianRoom"
    };

    public async Task BaseSceneLoadQueueAsync()
    {
        Debug.Log("Starting Game Menu");
        foreach (string scene in _initSceneQueue)
        {
            Debug.Log($"Loading {scene}");
            TaskCompletionSource<bool> sceneLoaded = new TaskCompletionSource<bool>();
            StartCoroutine(LoadSceneAsync(scene, sceneLoaded));
            await sceneLoaded.Task;
        }
        // All scenes are loaded and initialized
        Debug.Log("All scenes loaded and initialized");
    }

    public async Task StartGameSceneLoadQueueAsync()
    {
        Debug.Log("Starting Game");

        foreach (string scene in _gameSceneQueue)
        {
            Debug.Log($"Loading {scene}");
            TaskCompletionSource<bool> sceneLoaded = new TaskCompletionSource<bool>();
            StartCoroutine(LoadSceneAsync(scene, sceneLoaded));
            await sceneLoaded.Task;
        }

        SetSceneActive("VictorianRoom");

        // All scenes are loaded and initialized
        Debug.Log("All scenes loaded and initialized");
    }

    private void SetSceneActive(string sceneName)
    {
        Scene m_sceneToActivate = SceneManager.GetSceneByName(sceneName);

        Debug.Log($"Setting active scene {m_sceneToActivate.name}");

        // Once loaded, set the loaded scene as active
        SceneManager.SetActiveScene(m_sceneToActivate);
    }

    private IEnumerator LoadSceneAsync(string sceneName, TaskCompletionSource<bool> sceneLoaded)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        float smoothProgress = 0f;
        float loadingCompletedThreshold = 0.89f;

        while (!asyncOperation.isDone)
        {
            // Monitor the loading progress here
            float progress = asyncOperation.progress;
            smoothProgress = progress / loadingCompletedThreshold;

            // You can update your loading screen with the progress
            UIController.onLoadScene?.Invoke(smoothProgress);
            //Debug.Log(smoothProgress);
            yield return null;
        }

        // Ensure the scene is fully loaded and initialized
        asyncOperation.allowSceneActivation = true;

        // Notify that the scene has been loaded
        sceneLoaded.SetResult(true);
    }

    public async void LoadSceneAsync(string name)
    {
        Debug.Log(name);
        TaskCompletionSource<bool> sceneLoaded = new TaskCompletionSource<bool>();
        StartCoroutine(LoadSceneAsync(name, sceneLoaded));
        await sceneLoaded.Task;
    }

    public void UnloadScene(int i)
    {
        SceneManager.UnloadSceneAsync(i);
    }
}
