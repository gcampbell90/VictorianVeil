using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClockPuzzleController : BasePuzzle
{
    [SerializeField] private XRSocketInteractor _squareSocket;
    [SerializeField] private Transform _cogDrawer;
    [SerializeField] private Transform _coinDrawer;
    [SerializeField] private CombinationLock _comboLock;

    //Unity methods
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        _squareSocket.selectEntered.AddListener(OpenDrawer);
        _comboLock.OnCogWheelCompleted += CompletePuzzle;
    }

    protected override void Start()
    {
        base.Start();
        if (DebugMode)
        {
            CompletePuzzle();
        }
    }

    private void OnDisable()
    {
        _squareSocket.selectEntered.RemoveListener(OpenDrawer);
        _comboLock.OnCogWheelCompleted -= CompletePuzzle;
    }

    //Member methods
    private void OpenDrawer(SelectEnterEventArgs arg0)
    {
        StartCoroutine(OpenCogDrawer());
    }

    private new void CompletePuzzle()
    {
        base.CompletePuzzle();
        StartCoroutine(CloseCogDrawer());
        StartCoroutine(OpenCoinDrawer());
        Debug.Log("ReleaseCoin");
    }

    private IEnumerator OpenCogDrawer()
    {
        float timer = 0;
        float dur = 2f;

        var startPos = _cogDrawer.localPosition;
        var endPos = startPos - new Vector3(0.29f, 0, 0);

        while (timer < 1)
        {
            _cogDrawer.localPosition = Vector3.Lerp(startPos, endPos, timer);
            timer += Time.deltaTime / dur;
            yield return null;
        }
    }

    private IEnumerator OpenCoinDrawer()
    {
        float timer = 0;
        float dur = 2f;

        var startPos = _coinDrawer.localPosition;
        var endPos = startPos + new Vector3(0.29f, 0, 0);

        while (timer < 1)
        {
            _coinDrawer.localPosition = Vector3.Lerp(startPos, endPos, timer);
            timer += Time.deltaTime / dur;
            yield return null;
        }
    }

    private IEnumerator CloseCogDrawer()
    {
        float timer = 0;
        float dur = 2f;

        var startPos = _cogDrawer.localPosition;
        var endPos = startPos + new Vector3(0.29f, 0, 0);

        while (timer < 1)
        {
            _cogDrawer.localPosition = Vector3.Lerp(startPos, endPos, timer);
            timer += Time.deltaTime / dur;
            yield return null;
        }
    }

}
