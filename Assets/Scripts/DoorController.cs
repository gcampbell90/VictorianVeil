using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorController : MonoBehaviour
{
    public GameObject interactableGO;

    [Header("Interaction")]
    private XRBaseInteractable handle;
    PhysicsMover mover;

    //[SerializeField] float openPosition = 0.355F;

    private Vector3 closePos;
    private Vector3 openPos;

    private Vector3 grabPos;

    float startPosPercentage;
    float currPosPercentage;

    [Header("Debug Options")]
    [SerializeField] bool isUnlocked;


    private void Awake()
    {
        handle = interactableGO.GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        //handle.selectEntered.AddListener(StoreGrabInfo);
    }

    private void OnDisable()
    {
        //handle.selectEntered.RemoveListener(StoreGrabInfo);
    }

    //private void Start()
    //{
    //    closePos = interactableGO.transform.GetComponent<BoxCollider>().transform.position;
    //    openPos = closePos - new Vector3(0, 0, openPosition);
    //    mover = interactableGO.AddComponent<PhysicsMover>();
    //}

    ////Get position of interactor to drive door pos
    //private void StoreGrabInfo(SelectEnterEventArgs arg0)
    //{
    //    startPosPercentage = currPosPercentage;
    //    grabPos = arg0.interactorObject.transform.position;
    //    if (InventoryController.OnCheckInventory.Invoke() ||
    //        isUnlocked)
    //    {
    //        StartCoroutine(TrackHandPos());
    //    }
    //    else
    //    {
    //        Debug.Log("Cabinet is locked");
    //    }
    //}

    //IEnumerator TrackHandPos()
    //{
    //    Debug.Log("Handle Tracking Enabled");
    //    while (handle.isSelected)
    //    {
    //        UpdatePos();
    //        yield return null;
    //    }
    //    Debug.Log("Handle released");
    //}

    //private void UpdatePos()
    //{
    //    float percentageOpen = startPosPercentage + CalculateNormalisedPosDifference();

    //    mover.Rotate(Mathf.Lerp(0f, -180f, percentageOpen));

    //    currPosPercentage = Mathf.Clamp01(percentageOpen);

    //    //Debug.Log($"Percentage movement {currPosPercentage}");
    //}

    //float CalculateNormalisedPosDifference()
    //{
    //    Vector3 handPos = handle.interactorsSelecting[0].transform.position;
    //    Vector3 pullDir = handPos - grabPos;
    //    Vector3 targetDirection = closePos - openPos;

    //    float length = -targetDirection.magnitude;

    //    targetDirection.Normalize();

    //    //Debug.Log($"{Vector3.Dot(pullDir, targetDirection) / length}");
    //    return Vector3.Dot(pullDir, targetDirection) / length;

    //}

    //void OnDrawGizmos()
    //{
    //    // Draw a yellow sphere at the transform's position
    //    Debug.Log("DrawingGizmo");
    //    Gizmos.color = Color.white;

    //    Gizmos.DrawLine(closePos, openPos);
    //    Gizmos.DrawWireSphere(openPos, 0.1f);
    //}

}
