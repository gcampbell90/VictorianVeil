using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Lightmap_Switcher : MonoBehaviour
{
    [SerializeField] private Texture2D _lightCol;
    [SerializeField] private Texture2D _lightDir;

    [SerializeField] private Texture2D _darkCol;
    [SerializeField] private Texture2D _darkDir;

    private LightmapData[] _dayLightmaps = new LightmapData[1];
    private LightmapData[] _nightLightmaps = new LightmapData[1];

    [SerializeField] private Material _emissionMaterial;
    [SerializeField] private GameObject _reflectionProbe;

    [SerializeField] private LightProbeGroup _lightProbeGroupON;
    [SerializeField] private LightProbeGroup _lightProbeGroupOFF;

    private Color32 environmentalLightingOff = new Color32(20, 20, 20, 0);
    private Color32 environmentalLightingOn = new Color32(140, 110, 45, 0);


    public void Awake()
    {
        _dayLightmaps[0] = new LightmapData();
        _nightLightmaps[0] = new LightmapData();

        _dayLightmaps[0].lightmapColor = _lightCol;
        _dayLightmaps[0].lightmapDir = _lightDir;

        _nightLightmaps[0].lightmapColor = _darkCol;
        _nightLightmaps[0].lightmapDir = _darkDir;
    }

    public void DayLight()
    {
        //Debug.Log("Day light");
        LightmapSettings.lightmaps = _dayLightmaps;
        RenderSettings.ambientLight = environmentalLightingOn;
        _emissionMaterial.EnableKeyword("_EMISSION");
        _reflectionProbe.SetActive(true);

        _lightProbeGroupON.enabled = true;
        _lightProbeGroupOFF.enabled = false;
    }

    public void DarkLightmaps()
    {
        //Debug.Log("Night light");
        LightmapSettings.lightmaps = _nightLightmaps;
        RenderSettings.ambientLight = environmentalLightingOff;
        _emissionMaterial.DisableKeyword("_EMISSION");
        _reflectionProbe.SetActive(false);

        _lightProbeGroupON.enabled = false;
        _lightProbeGroupOFF.enabled = true;
    }
}
