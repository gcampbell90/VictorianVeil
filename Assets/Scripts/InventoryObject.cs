using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryObject : InventoryItem
{
    [Header("Debug Position")]
    [SerializeField] private Transform _debugPos;
    private Rigidbody rb;
    private void Awake()
    {
        Item = GetComponent<XRGrabInteractable>();
        rb = transform.GetComponent<Rigidbody>();

    }

    private void OnEnable()
    {
        Item.selectEntered.AddListener(AddItemToInventory);
        Item.selectExited.AddListener(RemoveGravity);
    }



    private void OnDisable()
    {
        Item.selectEntered.RemoveListener(AddItemToInventory);
        Item.selectExited.RemoveListener(RemoveGravity);

    }

    private void RemoveGravity(SelectExitEventArgs arg0)
    {
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void Start()
    {
        if (GameController.Instance.DebugMode && (_debugPos != null)) transform.position = _debugPos.position;

    }

    private void AddItemToInventory(SelectEnterEventArgs arg0)
    {
        Item.selectEntered.RemoveListener(AddItemToInventory);

        Debug.Log($"Item  '{arg0.interactableObject.transform.name}' Selected by '{arg0.interactorObject.transform.name}'");
        InventoryController.OnAddInventoryItem?.Invoke(Item.gameObject);

        //if(Item.gameObject.GetComponent<InventoryItem>().itemType == ItemType.key)
        //{
        //    //Item.gameObject.SetActive(false);
        //}

    }

}
