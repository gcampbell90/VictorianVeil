using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InventorySystem
{
    public class InventoryController : IInventoryController, IDisposable
    {
        private InventoryModel _inventoryModel;
        private InventoryView _inventoryView;
        private XRInteractionManager _interactionManager = null;

        //Item Events to update view
        public delegate void OnShowItemText(InventoryObject inventoryObject);
        public static OnShowItemText onShowItemText;

        public delegate void OnHideItemText(InventoryObject inventoryObject);
        public static OnHideItemText onHideItemText;

        public delegate void OnItemAddToInventory(GameObject gameObject);
        public static OnItemAddToInventory onItemAddToInventory;

        public delegate void OnItemAddToSocket(InventoryObject inventoryObject);
        public static OnItemAddToSocket onItemAddToSocket;

        public void Initialise(InventoryModel inventoryModel, InventoryView inventoryView)
        {
            _inventoryModel = inventoryModel;
            _inventoryView = inventoryView;

            _inventoryModel.OnInventoryAdded += UpdateViewAddItemToInventory;
            _inventoryModel.OnInventoryRemoved += UpdateViewRemovedItemFromInventory;
            _inventoryView.OnUIItemSelected += GetItemFromInventory;

            onShowItemText += UpdateViewShowText;
            onHideItemText += UpdateViewHideText;
            onItemAddToInventory += ItemAddToInventory;

            onItemAddToSocket += ItemAddToSocket;
        }

        //When an inventory item is hovered, update the UI to show text
        public void UpdateViewShowText(InventoryObject item) => _inventoryView.ShowItemText(item);
        //When an inventory item is released or added to socket, update the UI to hide text
        public void UpdateViewHideText(InventoryObject item) => _inventoryView.HideItemText(item);

        private void UpdateViewAddItemToInventory(InventoryObject item) => _inventoryView.AddItemToInventory(item);

        private void UpdateViewRemovedItemFromInventory(InventoryUIItem item) => _inventoryView.RemoveItemFromInventory(item);

        //When an inventory item pick up event is fired, add it to the inventory list
        private void ItemAddToInventory(GameObject arg0)
        {

            var inventoryObject = arg0.GetComponent<InventoryObject>();

            _inventoryModel.AddItemToInventory(inventoryObject);
            arg0.SetActive(false);

            Debug.Log($"{arg0.name} added to inventory ");
        }

        //When an inventory item is selected from the inventory panel, reactivate the hidden gameobject
        private void GetItemFromInventory(InventoryUIItem item, SelectEnterEventArgs args)
        {
            var m_item = item.InventoryObject;
            //GameManager.Instance.inventoryManager.ReactivateItem(item);
            if (_inventoryModel.IsItemInInventory(m_item))
            {
                GameObject itemGO = _inventoryModel.GetInventoryObject(item);

                if (args != null)
                {
                    //attach GO to hand again
                    Debug.Log($"UI Item: {args.interactableObject.transform.name} selected by {args.interactorObject.transform.name}");
                    var m_xRManager = args.manager;

                    //get direct interactor
                    var interactorSel = args.interactorObject;
                    var directInteractor = interactorSel.transform.parent.GetComponentInChildren<XRDirectInteractor>();
                    var m_XRGrab = itemGO.GetComponent<XRGrabInteractable>();
                    itemGO.transform.position = directInteractor.transform.position;
                    m_xRManager.SelectEnter((IXRSelectInteractor)directInteractor, m_XRGrab);
                }

                _inventoryModel.RemoveItemFromInventory(item);

                //set active
                itemGO.SetActive(true);
            }
        }

        private void ItemAddToSocket(InventoryObject inventoryObject)
        {
            var m_baseInteractable = inventoryObject.BaseInteractable;

            m_baseInteractable.hoverEntered.RemoveAllListeners();
            m_baseInteractable.selectEntered.RemoveAllListeners();
            m_baseInteractable.selectExited.RemoveAllListeners();

            if (_interactionManager == null)
            {
                SetInteractionManager(m_baseInteractable);
            }

            var m_interactor = m_baseInteractable.interactorsSelecting[0];
            _interactionManager.SelectExit(m_interactor, m_baseInteractable);

            UpdateViewHideText(inventoryObject);
            Debug.Log($"{inventoryObject.ItemName} entered into socket, removing listeners and deselecting");
        }

        void SetInteractionManager(XRBaseInteractable interactableObject)
        {
            _interactionManager = interactableObject.interactionManager;
        }

        public void Dispose()
        {
            _inventoryModel.OnInventoryAdded -= UpdateViewAddItemToInventory;
            _inventoryModel.OnInventoryRemoved -= UpdateViewRemovedItemFromInventory;
            _inventoryView.OnUIItemSelected -= GetItemFromInventory;

            onShowItemText -= UpdateViewShowText;
            onHideItemText -= UpdateViewHideText;

            onItemAddToInventory -= ItemAddToInventory;

            onItemAddToSocket -= ItemAddToSocket;

        }
    }
}