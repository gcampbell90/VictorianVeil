using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace InventorySystem
{
    public class InventoryView : MonoBehaviour
    {
        [Header("Debug UI Prefabs")]
        [SerializeField] private GameObject _itemImagePrefab;

        [SerializeField] private GameObject _textMesh;

        [Header("3D UI")]
        [SerializeField] private ItemUIPanel _itemUIPanel;
        [SerializeField] private GameObject _itemUIPrefab;

        private ItemTextMesh _itemTextMesh;
        private Dictionary<InventoryObject, GameObject> _inventory3DButtons = new Dictionary<InventoryObject, GameObject>();

        // Events to notify the InventoryView
        public delegate void ShowItemTextDelegate(InventoryObject item);
        public event ShowItemTextDelegate OnShowItemText;

        public delegate void HideItemTextDelegate(InventoryObject item);
        public event HideItemTextDelegate OnHideItemText;

        public delegate void ShowItemAddedDelegate(InventoryObject item);
        public event ShowItemAddedDelegate OnAddItem;

        public delegate void ShowItemRemovedDelegate(InventoryUIItem item);
        public event ShowItemRemovedDelegate OnRemoveItem;

        public delegate void UIItemSelectedDelegate(InventoryUIItem item, SelectEnterEventArgs args);
        public event UIItemSelectedDelegate OnUIItemSelected;

        public void Awake()
        {
            SetUpUIText();
        }

        private void OnEnable()
        {
            OnShowItemText += HandleShowItemText;
            OnHideItemText += HandleHideItemText;
            OnAddItem += HandleItemAdded;
            OnRemoveItem += HandleItemRemoved;
        }

        private void OnDisable()
        {
            OnShowItemText -= HandleShowItemText;
            OnHideItemText -= HandleHideItemText;
            OnAddItem -= HandleItemAdded;
            OnRemoveItem -= HandleItemRemoved;
        }

        private void SetUpUIText()
        {
            //Create UI text mesh
            var m_textMesh = Instantiate(_textMesh);
            _itemTextMesh = m_textMesh.GetComponent<ItemTextMesh>();
        }

        //Public methods for calling events
        public void ShowItemText(InventoryObject item)
        {
            OnShowItemText?.Invoke(item);
        }

        public void HideItemText(InventoryObject item)
        {
            OnHideItemText?.Invoke(item);
        }

        public void AddItemToInventory(InventoryObject item)
        {
            OnAddItem?.Invoke(item);
        }

        public void RemoveItemFromInventory(InventoryUIItem item)
        {
            OnRemoveItem?.Invoke(item);
        }

        //Private methods for logic
        private void HandleShowItemText(InventoryObject item)
        {
            _itemTextMesh.SetItemText(item);
        }

        private void HandleHideItemText(InventoryObject item)
        {
            _itemTextMesh.HideText();
        }

        private void HandleItemAdded(InventoryObject item)
        {
            OnHideItemText?.Invoke(item);
            AddItemToInventoryPanel(item);
        }

        private void HandleItemRemoved(InventoryUIItem uiItem)
        {
            var m_inventoryObject = uiItem.InventoryObject;
            if (_inventory3DButtons.ContainsKey(m_inventoryObject))
            {
                var uiButton = _inventory3DButtons[m_inventoryObject];

                uiButton.GetComponent<InventoryUIItem>().onItemSelected -= HandleUIItemSelected;

                _inventory3DButtons.Remove(m_inventoryObject);
                _itemUIPanel.Remove(uiItem);

                Destroy(uiButton);
            }
        }

        private void AddItemToInventoryPanel(InventoryObject item)
        {
            if (_itemUIPanel != null)
            {
                ItemSlot m_ItemSlot = _itemUIPanel.GetItemSlot();

                var itemObject = Instantiate(_itemUIPrefab, _itemUIPanel.transform);
                itemObject.transform.localPosition = m_ItemSlot.Position;

                itemObject.GetComponentInChildren<TextMeshPro>().text = item.ItemName;

                var uiItem = itemObject.GetComponent<InventoryUIItem>();
                uiItem.UISlotIndex = m_ItemSlot.Index;
                uiItem.InventoryObject = item;
                uiItem.onItemSelected += HandleUIItemSelected;

                _inventory3DButtons.Add(item, itemObject);
            }
            else
            {
                Debug.LogError("ItemPanel reference is missing in InventoryView!");
            }
        }

        private void HandleUIItemSelected(InventoryUIItem item, SelectEnterEventArgs args)
        {
            OnUIItemSelected?.Invoke(item, args);
        }
    }
}
