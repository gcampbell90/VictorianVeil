using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LampController : MonoBehaviour
{
    [SerializeField] private GameObject socketGO;
    [SerializeField] private GameObject bustGO;
    [SerializeField] private float pos_Offset;

    [Header("Interaction")]
    private XRSocketInteractor socket;

    private void Awake()
    {
        socket = socketGO.transform.GetComponent<XRSocketInteractor>();
    }
    private void OnEnable()
    {
        socket.selectEntered.AddListener(CheckIsValid);
    }
    private void OnDisable()
    {
        socket.selectEntered.RemoveListener(CheckIsValid);
    }

    private void CheckIsValid(SelectEnterEventArgs arg0)
    {
        var type = arg0.interactableObject.transform.TryGetComponent(out BaseItem inventoryItem);
        if (!type)
        {
            Debug.Log("Not a valid itemObject");
        }
        else
        {
            BaseItem.ItemType itemType = arg0.interactableObject.transform.GetComponent<BaseItem>().itemType;
            var hasKey = InventoryManager.Instance.InventoryController.CheckInventory(itemType, arg0.interactableObject.transform.gameObject);
            if (true)
            {
                TriggerBustMovement();
            }
            arg0.interactableObject.transform.GetComponent<BoxCollider>().enabled = false;
        }

    }
    private void TriggerBustMovement()
    {
        StartCoroutine(MoveBust());
    }
    private IEnumerator MoveBust()
    {
        float dur = 5f;
        float t = 0f;

        Vector3 openPos = bustGO.transform.position + bustGO.transform.right * pos_Offset;
        Vector3 startPos = bustGO.transform.position;
        while (dur > t)
        {
            bustGO.transform.position = Vector3.Lerp(startPos, openPos, t);
            t += Time.deltaTime;
            yield return null;
        }
    }
    void OnDrawGizmos()
    {
        Vector3 worldEndPosition = bustGO.transform.position + -bustGO.transform.forward * pos_Offset;

        // Draw a yellow sphere at the transform's position
        //Debug.Log("DrawingGizmo");
        Gizmos.color = Color.white;

        Gizmos.DrawLine(bustGO.transform.position,
             worldEndPosition);
        Gizmos.DrawWireSphere(worldEndPosition, 0.1f);
    }

}
