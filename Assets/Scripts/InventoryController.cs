using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryController : IDisposable
{
    public event Action<GameObject> OnPickUpItem;
    public event Func<BaseInventoryItem.ItemType, GameObject, bool> OnCheckInventory;

    private InventoryManager inventoryManager;

    public TextMeshPro textMesh;

    public InventoryController(InventoryManager invManager)
    {
        inventoryManager = invManager;
        Debug.Log("Created");
        OnPickUpItem += HandleItemPickup;
        OnCheckInventory += CheckForItemInInventory;
    }

    //When an inventory item is hovered, update the UI
    public void ShowText(Transform transform, Vector3 position)
    {
        textMesh.text = transform.name;
        textMesh.transform.position = position;
        textMesh.gameObject.SetActive(true);
    }

    public void HideText()
    {
        textMesh.gameObject.SetActive(false);
    }

    //When an inventory item is picked up, add it to the inventory list
    internal void ItemPickedUp(GameObject gameObject)
    {
        OnPickUpItem?.Invoke(gameObject);
    }

    private void HandleItemPickup(GameObject arg0)
    {
        var grabbedItem = arg0.gameObject;
        Debug.Log($"{grabbedItem.name} added to inventory ");

        inventoryManager.inventoryItems.Add(grabbedItem.GetComponent<BaseInventoryItem>());
    }

    internal bool CheckInventory(BaseInventoryItem.ItemType itemType, GameObject item)
    {
        return OnCheckInventory.Invoke(itemType, item);
    }
    private bool CheckForItemInInventory(BaseInventoryItem.ItemType itemType, GameObject item)
    {
        bool isPresent = inventoryManager.inventoryItems.Exists(item => item.itemType == itemType);
        string presentStatus = isPresent ? "is present" : "is not present";
        Debug.Log($"{item.name} {presentStatus} in InventoryList");
        return isPresent;
    }

    public void Dispose()
    {
        OnPickUpItem -= HandleItemPickup;
        OnCheckInventory -= CheckForItemInInventory;
    }
}
