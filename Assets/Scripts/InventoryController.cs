using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject InventoryUIPanel;
    [SerializeField] private GameObject InventoryImagePanel;

    public List<InventoryItem> inventoryItems;

    public delegate void GainInventoryItem(GameObject item);
    public static GainInventoryItem OnAddInventoryItem;

    public delegate bool CheckInventoryHandler(InventoryItem.ItemType itemType, GameObject item);
    public static CheckInventoryHandler OnCheckInventory;

    private void OnEnable()
    {
        OnAddInventoryItem += AddItem;
        OnCheckInventory += CheckForItem;

    }
    private void OnDisable()
    {
        OnAddInventoryItem -= AddItem;
        OnCheckInventory -= CheckForItem;

    }
    private void AddItem(GameObject arg0)
    {
        var grabbedItem = arg0.gameObject;
        Debug.Log($"{grabbedItem.name} added to inventory ");

        inventoryItems.Add(grabbedItem.GetComponent<InventoryItem>());

        //var itemimg = Instantiate(InventoryImagePanel, InventoryUIPanel.transform);
        //grabbedItem.transform.SetParent(itemimg.transform);
        //grabbedItem.transform.localPosition = Vector3.zero;
        //grabbedItem.transform.rotation = Quaternion.Euler(new Vector3(40, 90, 0));

        //itemimg.GetComponentInChildren<TextMeshProUGUI>().text = $"{grabbedItem.name}";
        //itemimg.GetComponentInChildren<Button>().onClick.AddListener(delegate { GrabItem(grabbedItem); });
    }

    //private void GrabItem(GameObject item)
    //{
    //    Debug.Log("Button Select");
    //    item.SetActive(true);
    //}

    private bool CheckForItem(InventoryItem.ItemType itemType, GameObject item)
    {
        bool isPresent = inventoryItems.Exists(item => item.itemType == itemType);
        string presentStatus = isPresent ? "is present" : "is not present";
        Debug.Log($"{item.name} {presentStatus} in InventoryList");
        return isPresent;
    }
}
