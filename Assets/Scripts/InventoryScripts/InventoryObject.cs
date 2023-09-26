using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryObject : BaseItem
{
    private InventoryController _inventoryController;
    [Header("Debug Position")]
    [SerializeField] private Transform _debugPos;

    public event Action<InventoryObject> ItemPickedUp = delegate { };

    public override void Awake()
    {
        base.Awake();
        _inventoryController = InventoryManager.Instance.InventoryController;
    }

    public override void OnEnable() => base.OnEnable();

    public override void OnDisable() => base.OnDisable();

    public override void HoverEntered(HoverEnterEventArgs args)
    {
        base.HoverEntered(args);
    }

    public override void SelectEntered(SelectEnterEventArgs args)
    {
        base.SelectEntered(args);

        _inventoryController.onShowItemText?.Invoke(this);
    }

    public override void SelectExited(SelectExitEventArgs args)
    {
        base.SelectExited(args);

        Item.selectExited.RemoveListener(SelectExited);

        _inventoryController.onItemAddToInventory?.Invoke(gameObject);

        gameObject.SetActive(false);
    }
}
