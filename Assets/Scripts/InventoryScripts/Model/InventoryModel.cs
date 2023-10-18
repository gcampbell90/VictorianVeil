using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [Serializable]
    public class InventoryModel
    {
        public delegate void InventoryAddedDelegate(InventoryObject item);
        public event InventoryAddedDelegate OnInventoryAdded;

        public delegate void InventoryRemovedDelegate(InventoryUIItem item);
        public event InventoryRemovedDelegate OnInventoryRemoved;

        private List<InventoryObject> _inventoryItems = new List<InventoryObject>();
        public Dictionary<InventoryObject, GameObject> InventoryObjects { get; private set; } = new Dictionary<InventoryObject, GameObject>();

        public void AddItemToInventory(InventoryObject item)
        {
            _inventoryItems.Add(item);
            InventoryObjects[item] = item.transform.gameObject;
            OnInventoryAdded?.Invoke(item);
        }

        public bool RemoveItemFromInventory(InventoryUIItem item)
        {
            InventoryObjects.Remove(item.InventoryObject);
            OnInventoryRemoved?.Invoke(item);
            return _inventoryItems.Remove(item.InventoryObject);
        }

        public GameObject GetInventoryObject(InventoryUIItem item)
        {
            OnInventoryRemoved?.Invoke(item);
            return InventoryObjects[item.InventoryObject];
        }

        public bool IsItemInInventory(InventoryObject item)
        {
            return InventoryObjects.ContainsKey(item);
        }
    }

}
