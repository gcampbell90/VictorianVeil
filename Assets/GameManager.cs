using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Inventory System
    public InventoryManager inventoryManager;
    
    [SerializeField] private bool _debugMode;
    public bool DebugMode
    {
        get => _debugMode;
    }

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
        //LoadScene("InteractableItems");
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log($"Loading {sceneName} scene");
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

}

