using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryObject : BaseInventoryItem
{
    [Header("Debug Position")]
    [SerializeField] private Transform _debugPos;

    public override void OnEnable() => base.OnEnable();
    public override void OnDisable() => base.OnDisable();
    public override void HoverEnter(HoverEnterEventArgs args)
    {
        base.HoverEnter(args);
    }
    public override void SelectEnter(SelectEnterEventArgs args)
    {
        base.SelectEnter(args);
    }

}
