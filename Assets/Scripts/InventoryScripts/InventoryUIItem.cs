using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryUIItem : XRSimpleInteractable
{
    public int UISlotIndex { get; set; }
    public InventoryObject InventoryObject { get; set; }

    public delegate void ButtonGrabbed(InventoryUIItem inventoryObject, SelectEnterEventArgs args);
    public ButtonGrabbed onItemSelected;

    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(ItemSelected);
        hoverEntered.AddListener(ItemHovered);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        selectEntered.RemoveListener(ItemSelected);
        hoverEntered.RemoveListener(ItemHovered);
    }

    private void ItemSelected(SelectEnterEventArgs arg)
    {

        Debug.Log($"UI Item {InventoryObject.ItemName} selected");
        onItemSelected?.Invoke(this, arg);
    }

    private void ItemHovered(HoverEnterEventArgs arg)
    {
        Debug.Log($"UI Item hovered {InventoryObject.ItemName}");
    }

}
