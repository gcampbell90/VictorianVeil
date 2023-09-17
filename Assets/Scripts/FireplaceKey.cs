using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireplaceKey : MonoBehaviour
{
    private XRSlideable xrSlideable;
    public XRSlideable XRSlideable { get => GetComponent<XRSlideable>();}
    XRBaseInteractable XRBase { get => GetComponent<XRBaseInteractable>(); }
    XRInteractionManager XRManager { get => XRBase.interactionManager; }

    IXRSelectInteractor interactorSel;
    public enum KeyObjectState { Initial, Key, Completed }
    public KeyObjectState KeyState { get; set; }

    public delegate void KeyPlaced();
    public KeyPlaced onKeyPlaced;

    public delegate void KeyEntered();
    public KeyEntered onKeyEntered;

    public delegate void KeyBehaviour();
    public KeyBehaviour keyBehaviour;

    private void OnEnable()
    {
        XRSlideable.onMovementCompleted += UpdateKeyState;
        onKeyPlaced += UpdateKeyState;
        onKeyEntered += UpdateKeyState;
    }
    private void OnDisable()
    {
        XRSlideable.onMovementCompleted -= UpdateKeyState;
        onKeyPlaced -= UpdateKeyState;
        onKeyEntered -= UpdateKeyState;
    }
    private void UpdateKeyState()
    {
        float delay = 0f;
        switch (KeyState)
        {
            case KeyObjectState.Initial:
                KeyState = KeyObjectState.Key;
                keyBehaviour += SwitchToGrabInteractable;
                break;
            case KeyObjectState.Key:
                KeyState = KeyObjectState.Completed;
                keyBehaviour -= SwitchToGrabInteractable;
                keyBehaviour += SwitchToSimpleInteractable;
                delay = 1f;
                break;
            case KeyObjectState.Completed:
                FireplaceController.onKeyEntered?.Invoke();
                keyBehaviour -= SwitchToGrabInteractable;
                keyBehaviour += DestroyBehaviours;
                break;
            default:
                break;
        }
        SetKeyState(KeyState);
        StartCoroutine(SwitchInteractableDelay(keyBehaviour, delay));
        //Debug.Log(keyBehaviour.Method.Name);
    }
    private void SetKeyState(KeyObjectState keyState)
    {
        KeyState = keyState;
        //Debug.Log($"Key State: {KeyState}");
    }
    private void SwitchToGrabInteractable()
    {
        XRSlideable.enabled = false;
        DestroyImmediate(XRBase);
        XRGrabInteractable m_XRGrab = gameObject.AddComponent<XRGrabInteractable>();
        m_XRGrab.attachTransform = transform.GetChild(0);
        m_XRGrab.throwOnDetach = false;
        m_XRGrab.enabled = true;
        XRManager.SelectEnter(interactorSel, m_XRGrab);
    }
    private void SwitchToSimpleInteractable()
    {
        DestroyImmediate(XRBase);
        gameObject.AddComponent<XRSimpleInteractable>();
        ToggleCollider(false);
        XRSlideable.slideableItem.interactable = XRBase;
        XRSlideable.slideableItem.startPosPercentage = 0;
        XRSlideable.slideableItem.currPosPercentage = 0;
        XRSlideable.slideableItem.closePos = transform.position;
        XRSlideable.slideableItem.openPos = transform.position + transform.TransformDirection(Vector3.left) * -0.15f;

        XRSlideable.enabled = true;
        XRBase.enabled = true;
        ToggleCollider(true);
    }
    void ToggleCollider(bool isOn)
    {
        var colliders = GetComponentsInChildren<SphereCollider>();
        foreach (var collider in colliders)
        {
            collider.enabled = isOn;
        }
    }
    private void DestroyBehaviours()
    {
        Destroy(XRBase);
        Destroy(XRSlideable);
        Destroy(this);
    }
    private IEnumerator SwitchInteractableDelay(KeyBehaviour keyBehaviour, float delay)
    {
        interactorSel = XRBase.interactorsSelecting[0];

        XRBase.interactionLayers = ToggleInteractionLayer(XRBase.interactionLayers, false);

        yield return new WaitForSeconds(delay);
        XRBase.interactionLayers = ToggleInteractionLayer(XRBase.interactionLayers, true);
        keyBehaviour?.Invoke();
    }
    private InteractionLayerMask ToggleInteractionLayer(InteractionLayerMask layerMask, bool isOn)
    {
        int layer = isOn ? 2 : 30;
        layerMask = (1 << layer);
        return layerMask;
    }
}
