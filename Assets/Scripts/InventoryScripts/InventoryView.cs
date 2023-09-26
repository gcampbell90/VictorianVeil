using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{

    [SerializeField] private GameObject _textMesh;
    private ItemTextMesh _itemTextMesh;

    [SerializeField] private GameObject _itemImagePrefab;
    [SerializeField] private RectTransform _itemPanel;

    private Dictionary<InventoryObject, GameObject> _inventoryButtons = new Dictionary<InventoryObject, GameObject>();

    public void Awake()
    {
        SetUpUIText();
    }

    private void OnEnable()
    {
        InventoryManager.Instance.OnItemSelected += ShowItemText;
        InventoryManager.Instance.OnItemAdded += HandleItemAdded;
        InventoryManager.Instance.OnItemRemoved += HandleItemRemoved;
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnItemSelected -= ShowItemText;
        InventoryManager.Instance.OnItemAdded -= HandleItemAdded;
        InventoryManager.Instance.OnItemRemoved -= HandleItemRemoved;
    }

    private void SetUpUIText()
    {
        //Create UI text mesh
        var m_textMesh = Instantiate(_textMesh);
        _itemTextMesh = m_textMesh.GetComponent<ItemTextMesh>();
    }

    private void ShowItemText(InventoryObject item)
    {
        _itemTextMesh.SetItemText(item);
    }

    private void HideItemText()
    {
        _itemTextMesh.HideText();
    }

    private void HandleItemAdded(InventoryObject item)
    {
        HideItemText();
        AddItemToInventoryPanel(item);
    }

    private void AddItemToInventoryPanel(InventoryObject item)
    {
        if (_itemPanel != null)
        {
            var itemObject = Instantiate(_itemImagePrefab, _itemPanel);
            itemObject.GetComponentInChildren<TextMeshProUGUI>().text = item.ItemName;
            Button itemButton = itemObject.GetComponent<Button>();

            if (itemButton != null)
            {
                itemButton.onClick.AddListener(() => HandleItemButtonClicked(item));
            }

            _inventoryButtons.Add(item, itemObject);
        }
        else
        {
            Debug.LogError("ItemPanel reference is missing in InventoryView!");
        }
    }

    private void HandleItemButtonClicked(InventoryObject item)
    {
        InventoryManager.Instance.ReactivateItem(item);
    }

    private void HandleItemRemoved(InventoryObject item)
    {
        if (_inventoryButtons.ContainsKey(item))
        {
            var go = _inventoryButtons[item];

            _inventoryButtons.Remove(item);

            Destroy(go);
        }
    }

}
