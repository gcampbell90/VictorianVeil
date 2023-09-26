using System;
using TMPro;
using UnityEngine;


public class InventoryController : IDisposable
{
    public event Func<InventoryObject.ItemType, GameObject, bool> OnCheckInventory;

    //Item Events
    public delegate void OnShowItemText(InventoryObject inventoryObject);
    public OnShowItemText onShowItemText;

    public delegate void OnItemAddToInventory(GameObject gameObject);
    public OnItemAddToInventory onItemAddToInventory;

    public InventoryController()
    {
        OnCheckInventory += CheckForItemInInventory;
        onShowItemText += ShowItemText;
        onItemAddToInventory += ItemAddToInventory;
    }

    //When an inventory item is hovered, update the UI
    public void ShowItemText(InventoryObject item)
    {
        //OnShowText(name, transform, position);
        InventoryManager.Instance.ItemGrabbed(item);
    }

    //When an inventory item pick up event is fired, add it to the inventory list
    private void ItemAddToInventory(GameObject arg0)
    {
        var grabbedItem = arg0.gameObject;

        var inventoryItem = grabbedItem.GetComponent<InventoryObject>();

        InventoryManager.Instance.AddItemToInventory(inventoryItem);
        Debug.Log($"{grabbedItem.name} added to inventory ");
    }

    internal bool CheckInventory(BaseItem.ItemType itemType, GameObject item)
    {
        return OnCheckInventory.Invoke(itemType, item);
    }

    private bool CheckForItemInInventory(BaseItem.ItemType itemType, GameObject item)
    {
        //bool isPresent = InventoryManager.Instance.inventoryItems.Exists(item => item.itemType == itemType);
        //string presentStatus = isPresent ? "is present" : "is not present";
        //Debug.Log($"{item.name} {presentStatus} in InventoryList");
        //return isPresent;
        return true;
    }

    public void Dispose()
    {
        OnCheckInventory -= CheckForItemInInventory;
        onShowItemText -= ShowItemText;
        onItemAddToInventory -= ItemAddToInventory;
    }
}
