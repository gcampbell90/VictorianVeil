using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using InventorySystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _debugMode;

    public static GameManager Instance { get; private set; }

    //Inventory System
    public InventoryManager inventoryManager;

    public enum Levels { Base, Debug_Mode, Debug_Items, Debug_Puzzles, VictorianRoom }
    [SerializeField] private Levels _level;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        string levelString = "";
        switch (_level)
        {
            case Levels.Base:
                levelString = "XRRigScene";
                break;
            case Levels.Debug_Items:
                levelString = "InteractableItems";
                break;
            case Levels.Debug_Puzzles:
                levelString = "Puzzles";
                break;
            case Levels.VictorianRoom:
                levelString = "VictorianRoom";
                break;
            case Levels.Debug_Mode:
                levelString = "Debug_Mode";
                break;
            default:
                break;
        }

        ValidateLoadScene(levelString);
    }

    public bool GetDebugMode()
    {
        return _debugMode;
    }

    public void ValidateLoadScene(string sceneName)
    {
        if(sceneName == "Debug_Mode")
        {
            for (int i = 1; i < SceneManager.sceneCount; i++)
            {
                SceneManager.UnloadSceneAsync(i);
            }
            StartCoroutine(LoadSceneAsync("InteractableItems"));
            StartCoroutine(LoadSceneAsync("Puzzles"));
        }
        else
        {
            
        //if (SceneManager.GetSceneByName(sceneName).isLoaded)
        //{
        //    Debug.Log($"{sceneName} is already loaded.");

        //    // Unload all other scenes except the root and the already loaded scene
        //    for (int i = 1; i < SceneManager.sceneCount; i++)
        //    {
        //        Scene scene = SceneManager.GetSceneAt(i);
        //        if (scene.name != sceneName)
        //        {
        //            Debug.Log($"Unloading {scene.name}");
        //            SceneManager.UnloadSceneAsync(scene);
        //        }
        //    }

        //    // Optionally, set the already loaded scene as active (if needed)
        //    SetSceneActive(sceneName);
        //}
        //else
        //{
        // Unload all other scenes except the root
        for (int i = 1; i < SceneManager.sceneCount; i++)
            {
                SceneManager.UnloadSceneAsync(i);
            }
            Debug.Log($"Loading {sceneName} scene");

            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = false;

        // Here's a mock threshold, since progress might not always exactly reach 0.9
        float loadingCompletedThreshold = 0.89f;

        while (asyncOperation.progress < loadingCompletedThreshold)
        {
            // Here you can use asyncOperation.progress for your loading UI.
            // For a smoother progress bar, you can map the 0 to 0.9 range to 0 to 1 like so:
            float smoothProgress = asyncOperation.progress / loadingCompletedThreshold;
            Debug.Log("Loading progress: " + smoothProgress);

            yield return null;
        }

        // Once we've reached the threshold, we can finalize loading
        asyncOperation.allowSceneActivation = true;

        // Now you can wait until the operation is truly done
        while (!asyncOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        SetSceneActive(sceneName);
    }

    private void SetSceneActive(string sceneName)
    {
        Scene m_sceneToActivate = SceneManager.GetSceneByName(sceneName);

        Debug.Log($"Setting active scene {m_sceneToActivate.name}");

        // Once loaded, set the loaded scene as active
        SceneManager.SetActiveScene(m_sceneToActivate);
    }

    void ToggleRayInteractor()
    {

    }
}

