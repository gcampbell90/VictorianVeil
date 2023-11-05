using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CabinetPuzzleController : BasePuzzle
{
    [SerializeField] private GameObject interactableDoor;
    [SerializeField] private XRSocketInteractor _keySocket;

    private XRBaseInteractable cabinethandleInteractable;
    private new HingeJoint hingeJoint;

    float closedRot = -0.1f;
    float openRot = -180f;

    //Unity methods
    protected override void Awake()
    {
        base.Awake();

        cabinethandleInteractable = interactableDoor.GetComponent<XRBaseInteractable>();
        hingeJoint = interactableDoor.GetComponent<HingeJoint>();

        UpdateHingeJoint(closedRot);
    }

    private void OnEnable()
    {
        _keySocket.selectEntered.AddListener((args) => UnlockDoor());
    }

    protected override void Start()
    {
        base.Start();
        if (DebugMode)
        {
            UnlockDoor();
        }
    }

    private void OnDisable()
    {
        _keySocket.selectEntered.RemoveListener((args) => UnlockDoor());
    }

    //Member methods
    private void UnlockDoor()
    {
        UpdateHingeJoint(openRot);
        CompletePuzzle();
    }

    private new void CompletePuzzle()
    {
        base.CompletePuzzle();
    }

    private void UpdateHingeJoint(float rot)
    {
        JointLimits limits = hingeJoint.limits;
        limits.min = rot;
        limits.max = 0;
        hingeJoint.limits = limits;
    }

}
