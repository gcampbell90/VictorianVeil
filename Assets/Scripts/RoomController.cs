using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomController : MonoBehaviour
{
    [Header("Debug Values")]
    [Tooltip("Game starts with InventoryObjects in their debugPositions")]
    [SerializeField]private bool _debugMode;
    public bool DebugMode { get => _debugMode; }

    [Tooltip("Game starts with lights on/off")]
    [SerializeField] private bool lightsOn;
    public enum Areas { Start, FuseBox, Desk, Fireplace, Bookcase, Bust, Clock, Cabinet }
    [Tooltip("Player Start Pos")]
    public Areas _teleportAreaState;

    [SerializeField] private GameObject XROrigin;

    [SerializeField] private GameObject[] teleportationAnchors;
    [SerializeField] private GameObject debugCanvas;
    MeshRenderer[] renderers;

    public static RoomController Instance
    {
        get; set;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(Instance);
    }

    private void Start()
    {
        DisableRealtimeShadowsForStaticObjects();
        OnStateChanged(_teleportAreaState);

        if (lightsOn)
        {
            SwitchOnLights();
        }
        else
        {
            SwitchOffLights();
        }

    }

    public void LoadLightOnProbes()
    {
        if (SceneManager.GetSceneByBuildIndex(2).isLoaded)
        {
            SceneManager.UnloadSceneAsync("LightOffProbes");
        }
        LoadScene("LightOnProbes");
    }
    public void LoadLightOffProbes()
    {
        LoadScene("LightOffProbes");
    }
    public void LoadScene(string sceneName)
    {
        Debug.Log($"Loading {sceneName} scene");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

        // After loading the scene, disable real-time shadows
        DisableRealtimeShadowsForStaticObjects();
    }

    private void DisableRealtimeShadowsForStaticObjects()
    {
        renderers = FindObjectsOfType<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.gameObject.isStatic)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
    }
    private void OnStateChanged(Areas value)
    {
        Debug.Log($"Starting on anchor: {value}");
        Transform xrNewTransform = null;
        switch (value)
        {
            case Areas.Start:
                xrNewTransform = teleportationAnchors[7].transform;
                break;
            case Areas.FuseBox:
                xrNewTransform = teleportationAnchors[0].transform;
                break;
            case Areas.Desk:
                xrNewTransform = teleportationAnchors[1].transform;
                break;
            case Areas.Fireplace:
                xrNewTransform = teleportationAnchors[2].transform;
                break;
            case Areas.Bookcase:
                xrNewTransform = teleportationAnchors[3].transform;
                break;
            case Areas.Bust:
                xrNewTransform = teleportationAnchors[4].transform;
                break;
            case Areas.Clock:
                xrNewTransform = teleportationAnchors[5].transform;
                break;
            case Areas.Cabinet:
                xrNewTransform = teleportationAnchors[6].transform;
                break;
        }

        XROrigin.transform.position = xrNewTransform.position;
        XROrigin.transform.rotation = xrNewTransform.rotation;
    }

    internal void SwitchOnLights()
    {
        var lightmapSwitcher = GetComponent<Lightmap_Switcher>();
        lightmapSwitcher.DayLight();
    }
    internal void SwitchOffLights()
    {
        var lightmapSwitcher = GetComponent<Lightmap_Switcher>();
        lightmapSwitcher.DarkLightmaps();
    }
}



