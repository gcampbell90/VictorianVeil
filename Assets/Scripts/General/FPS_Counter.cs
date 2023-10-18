using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class FPS_Counter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI fpsLimit;

    private const float _targetFPS = 72f;
    private float _currFPS = 0.0f;
    private float _deltaTime = 0.0f;
    private void Awake()
    {
        //fpsText = GetComponentInChildren<TextMeshProUGUI>();

        StartCoroutine(DisplayFPS());

        Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate(90);
        Unity.XR.Oculus.Performance.TrySetGPULevel(4);

        float rate;
        Unity.XR.Oculus.Performance.TryGetDisplayRefreshRate(out rate);

        fpsLimit.text = $"{rate}";
    }
    private void Update()
    {
        GenerateFPS();
    }

    private void GenerateFPS()
    {
        // Calculate the delta time since last frame
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

        // Calculate FPS
        _currFPS = 1.0f / _deltaTime;

    }

    private IEnumerator DisplayFPS()
    {
        while (true)
        {
            fpsText.color = _currFPS >= _targetFPS ? Color.green : Color.red;
         
            // Update the TextMeshProUGUI component
            fpsText.text = string.Format("FPS: {0:0.0}", _currFPS);
            yield return new WaitForEndOfFrame();
        }
    }
}
