using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SlideableItem
{
    [Header("Interaction")]
    public XRBaseInteractable interactable;
    public PhysicsMover mover;

    public Vector3 closePos;
    public Vector3 openPos;

    public float startPosPercentage;
    public float currPosPercentage;

    public bool IsUnlocked { get; set; }

    public SlideableItem(){ }
}
