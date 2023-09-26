using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;
    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InventoryManager>();
                if (_instance == null)
                {
                    Debug.LogError("An instance of InventoryManager is needed in the scene, but none was found.");
                }
            }
            return _instance;
        }
    }

    public InventoryController InventoryController { get; private set; }

    private List<InventoryObject> _inventoryItems = new List<InventoryObject>();
    private Dictionary<InventoryObject, GameObject> _inventoryObjects = new Dictionary<InventoryObject, GameObject>();


    // Events to notify the InventoryView
    public event Action<InventoryObject> OnItemSelected = delegate { };
    public event Action<InventoryObject> OnItemAdded = delegate { };
    public event Action<InventoryObject> OnItemRemoved = delegate { };

    private void Awake()
    {
        SetUpInventoryController();
    }

    private void SetUpInventoryController()
    {
        //Set up inventory controller
        InventoryController = new InventoryController();
    }

    internal void ItemGrabbed(InventoryObject inventoryItem)
    {
        OnItemSelected.Invoke(inventoryItem);
    }

    internal void AddItemToInventory(InventoryObject item)
    {
        _inventoryItems.Add(item);
        _inventoryObjects[item] = item.transform.gameObject;

        OnItemAdded.Invoke(item); // Notify the InventoryView
    }

    internal void RemoveItemFromInventory(InventoryObject item)
    {
        _inventoryItems.Remove(item);
        OnItemRemoved.Invoke(item); // Notify the InventoryView
    }

    public bool IsItemInInventory(BaseItem.ItemType itemType, GameObject item)
    {
        bool isPresent = _inventoryItems.Exists(item => item.itemType == itemType);
        string presentStatus = isPresent ? "is present" : "is not present";
        Debug.Log($"{item.name} {presentStatus} in InventoryList");
        return isPresent;
    }

    public void ReactivateItem(InventoryObject item)
    {
        if (_inventoryObjects.ContainsKey(item))
        {
            GameObject itemGO = _inventoryObjects[item];
            itemGO.SetActive(true);

            //AttachToHand(itemGO);

            _inventoryItems.Remove(item);
            _inventoryObjects.Remove(item);

            OnItemRemoved.Invoke(item);
        }
    }

    // TODO: add in interactor select logic
    private void AttachToHand(GameObject itemGO)
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        InventoryController.Dispose();
    }

}
