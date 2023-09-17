using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Lightmap_Switcher : MonoBehaviour
{
    [SerializeField] Texture2D lightCol;
    [SerializeField] Texture2D lightDir;

    [SerializeField] Texture2D darkCol;
    [SerializeField] Texture2D darkDir;

    LightmapData[] dayLightmaps = new LightmapData[1];
    LightmapData[] nightLightmaps = new LightmapData[1];

    [SerializeField] Material emissionMaterial;
    [SerializeField] GameObject reflectionProbe;

    Color32 environmentalLightingOff = new Color32(20, 20, 20, 0);
    Color32 environmentalLightingOn = new Color32(140, 110, 45, 0);

    public void Awake()
    {
        dayLightmaps[0] = new LightmapData();
        nightLightmaps[0] = new LightmapData();

        dayLightmaps[0].lightmapColor = lightCol;
        dayLightmaps[0].lightmapDir = lightDir;

        nightLightmaps[0].lightmapColor = darkCol;
        nightLightmaps[0].lightmapDir = darkDir;
    }

    public void DayLight()
    {
        //Debug.Log("Day light");
        LightmapSettings.lightmaps = dayLightmaps;
        RenderSettings.ambientLight = environmentalLightingOn;
        emissionMaterial.EnableKeyword("_EMISSION");
        reflectionProbe.SetActive(true);
    }

    public void DarkLightmaps()
    {
        //Debug.Log("Night light");
        LightmapSettings.lightmaps = nightLightmaps;
        RenderSettings.ambientLight = environmentalLightingOff;
        emissionMaterial.DisableKeyword("_EMISSION");
        reflectionProbe.SetActive(false);

    }
}
