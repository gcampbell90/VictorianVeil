using UnityEngine;
using InventorySystem;
using System;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [Tooltip("Sets puzzles and items to their debug states")]
    public bool DebugMode { get; private set; }
    [SerializeField] private bool _completeAllPuzzles;
    [SerializeField] private bool _itemsDebugPositions;

    [Tooltip("Level to play on Start")]
    [SerializeField] private Mode _mode;
    public static GameManager Instance { get; private set; }

    //Properties
    public InventoryManager inventoryManager { get; set; }
    public PuzzleManager puzzleManager { get; set; }
    public SceneLoadManager sceneLoadManager { get; set; }

    public event Action onGameQuit;
    public enum Mode { Debug_Mode, Game_Mode }

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

        sceneLoadManager = this.AddComponent<SceneLoadManager>();
        puzzleManager = new PuzzleManager();
    }

    private void Start()
    {
        if (_mode == Mode.Game_Mode)
        {
            BaseModeSequence();
        }
    }

    private async void BaseModeSequence()
    {
        await sceneLoadManager.BaseSceneLoadQueueAsync();
        Debug.Log("GameManager - SceneLoad complete");
        sceneLoadManager.UnloadScene(0);
    }

    public async void StartGame()
    {
        await sceneLoadManager.StartGameSceneLoadQueueAsync();
        Debug.Log("GameManager - Game Started");
    }

    public void QuitGame()
    {
        sceneLoadManager.UnloadScene(4);
        sceneLoadManager.UnloadScene(5);
        onGameQuit?.Invoke();

        Debug.Log("GameManager - Game Quit");
    }

    public bool GetPuzzleDebugMode()
    {
        return _completeAllPuzzles;
    }

    public bool GetItemDebugMode()
    {
        return _completeAllPuzzles;
    }

}

