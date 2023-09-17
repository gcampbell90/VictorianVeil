using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class FuseboxController : MonoBehaviour
{
    [SerializeField] private XRLever xrLever;
    [SerializeField] private ParticleSystem sparks;
    [SerializeField] private XRSocketInteractor fuseSocket;
    [SerializeField] private XRBaseInteractable socketedInteractor;

    private enum FuseState { Broken, Empty, Fixed }
    private FuseState fuseState;

    private Action activatePFX;
    private Action deactivatePFX;

    private void OnEnable()
    {
        activatePFX += EnablePFX;
        deactivatePFX += DisablePFX;

        fuseSocket.selectEntered.AddListener(CheckFuseObject);
        fuseSocket.selectExited.AddListener((args) => fuseState = FuseState.Empty);

        xrLever.onLeverActivate.AddListener(() => HandleNewState(true, activatePFX));
        xrLever.onLeverActivate.AddListener(() => ToggleInteractionLayer(socketedInteractor, false));

        xrLever.onLeverDeactivate.AddListener(() => HandleNewState(false, deactivatePFX));
        xrLever.onLeverDeactivate.AddListener(() => ToggleInteractionLayer(socketedInteractor, true));

    }

    private void CheckFuseObject(SelectEnterEventArgs arg0)
    {
        var name = arg0.interactableObject.transform.name;
        //Debug.Log(name);
        socketedInteractor = (XRBaseInteractable)arg0.interactableObject;
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
        //fuseSocket.socketActive = !isOn;
        if (isOn)
        {
            fuseSocket.interactionLayers = InteractionLayerMask.GetMask("NonGrabbable");
        }
        else
        {
            fuseSocket.interactionLayers = InteractionLayerMask.GetMask("SocketObjects", "NonGrabbable");
        }

        switch (fuseState)
        {
            case FuseState.Broken:
                pfxMethod?.Invoke();
                break;
            case FuseState.Fixed:
                if (isOn)
                    CompleteFusebox();
                break;
            case FuseState.Empty:
                break;
            default:
                break;
        }
    }

    private void CompleteFusebox()
    {
        //GameController.Instance.transform.GetComponent<Lightmap_Switcher>().DayLight();
        //GameController.Instance.LoadLightOnProbes();
        GameController.Instance.SwitchOnLights();
        DeinitialiseFusebox();
    }

    private void DeinitialiseFusebox()
    {
        sparks.Stop();
        ToggleInteractionLayer(socketedInteractor, false);
        xrLever.enabled = false;
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

    private void ToggleInteractionLayer(XRBaseInteractable interactor, bool isOn)
    {
        int layer = isOn ? 2 : 30;
        interactor.interactionLayers = 1 << layer;
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

}
