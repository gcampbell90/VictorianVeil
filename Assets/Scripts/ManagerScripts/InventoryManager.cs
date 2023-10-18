using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using InventorySystem;

namespace InventorySystem
{
    public class InventoryManager : BaseManager
    {
        //Model and Controller fields
        private InventoryModel _inventoryModel;
        private InventoryView _inventoryView { get; set; }
        public InventoryController InventoryController { get; private set; }

        private void Awake()
        {
            _inventoryModel = new InventoryModel();
            _inventoryView = GetComponent<InventoryView>();
            InventoryController = new InventoryController();

            InventoryController.Initialise(_inventoryModel, _inventoryView);
        }

        private void Start()
        {
            
        }

        public bool CheckInventory(InventoryObject item)
        {
            return _inventoryModel.IsItemInInventory(item);
        }

        private void OnDisable()
        {
            InventoryController.Dispose();
        }
    }
}
