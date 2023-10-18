using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class FuseboxController : BasePuzzle
{
    [SerializeField] private XRLever xrLever;
    [SerializeField] private ParticleSystem sparks;
    [SerializeField] private XRSocketInteractor fuseSocket;
    private XRBaseInteractable _currSocketedInteractor;

    private enum FuseState { Broken, Empty, Fixed }
    private FuseState fuseState;

    private Action activatePFX;
    private Action deactivatePFX;

    //Unity Methods
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        activatePFX += EnablePFX;
        deactivatePFX += DisablePFX;

        fuseSocket.selectEntered.AddListener(CheckFuseObject);
        fuseSocket.selectExited.AddListener((args) => fuseState = FuseState.Empty);

        xrLever.onLeverActivate.AddListener(() => HandleNewState(true, activatePFX));
        xrLever.onLeverActivate.AddListener(() => ToggleInteractionLayer(_currSocketedInteractor, false));

        xrLever.onLeverDeactivate.AddListener(() => HandleNewState(false, deactivatePFX));
        xrLever.onLeverDeactivate.AddListener(() => ToggleInteractionLayer(_currSocketedInteractor, true));

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
        activatePFX -= EnablePFX;
        deactivatePFX -= DisablePFX;

        xrLever.onLeverActivate.RemoveAllListeners();
        xrLever.onLeverDeactivate.RemoveAllListeners();

        fuseSocket.selectEntered.RemoveAllListeners();
        fuseSocket.selectExited.RemoveAllListeners();
    }

    //Member Methods
    private void CheckFuseObject(SelectEnterEventArgs arg0)
    {
        var name = arg0.interactableObject.transform.name;
        //Debug.Log(name);
        _currSocketedInteractor = (XRBaseInteractable)arg0.interactableObject;
        if (name == "BrokenFuse")
        {
            if (fuseState == FuseState.Fixed || fuseState == FuseState.Empty)
                fuseState = FuseState.Broken;
            return;
        }
        else if (name == "Wrench")
        {
            if (fuseState == FuseState.Broken || fuseState == FuseState.Empty)
                fuseState = FuseState.Fixed;
        }
    }

    private void HandleNewState(bool isOn, Action pfxMethod)
    {
        Debug.Log("Checking fuse:" + fuseState);
        switch (fuseState)
        {
            case FuseState.Broken:
                pfxMethod?.Invoke();
                break;
            case FuseState.Fixed:
                if (isOn)
                    CompletePuzzle();
                break;
            case FuseState.Empty:
                break;
            default:
                break;
        }
    }

    private void DisablePFX()
    {
        var emission = sparks.emission;
        emission.enabled = false;
    }

    private void EnablePFX()
    {
        var emission = sparks.emission;
        emission.enabled = true;
    }

    private new void CompletePuzzle()
    {
        base.CompletePuzzle();
        DeinitialiseFusebox();
    }

    private void DeinitialiseFusebox()
    {
        sparks.Stop();
        ToggleInteractionLayer(_currSocketedInteractor, false);
        xrLever.enabled = false;
    }

    private void ToggleInteractionLayer(XRBaseInteractable interactor, bool isOn)
    {
        int layer = isOn ? 1 : 2;
        interactor.interactionLayers = 1 << layer;
    }



}
