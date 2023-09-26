using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CabinetController : MonoBehaviour
{
    public GameObject interactableDoor;
    [SerializeField] private List<GameObject> drawers = new List<GameObject>();
    private List<SlideableItem> moveableDrawers = new List<SlideableItem>();

    [Header("Interaction")]
    private XRBaseInteractable cabinethandleInteractable;
    PhysicsMover mover;

    private new HingeJoint hingeJoint;

    [Header("Debug Options")]
    [SerializeField] bool isUnlocked;

    //HingeJointLimits(transform origin dependent)
    float closedRot = -0.1f;
    float openRot = -180f;

    private Vector3 grabPos;


    private void Awake()
    {
        cabinethandleInteractable = interactableDoor.GetComponent<XRBaseInteractable>();

        hingeJoint = interactableDoor.GetComponent<HingeJoint>();
    }
    private void OnEnable()
    {
        cabinethandleInteractable.selectEntered.AddListener(CabinetHandleCheck);
    }
    private void OnDisable()
    {
        cabinethandleInteractable.selectEntered.RemoveListener(CabinetHandleCheck);
    }

    private void CabinetHandleCheck(SelectEnterEventArgs arg0)
    {
        var hasKey = InventoryManager.Instance.InventoryController.CheckInventory(BaseItem.ItemType.key, arg0.interactableObject.transform.gameObject);
        if (hasKey || isUnlocked)
        {
            Debug.Log("Door Open");

            UpdateHingeJoint(openRot);
        }
        else
        {
            Debug.Log("Door Locked");

            UpdateHingeJoint(closedRot);
        }
    }
    private void UpdateHingeJoint(float rot)
    {
        JointLimits limits = hingeJoint.limits;
        limits.min = rot;
        limits.max = 0;
        hingeJoint.limits = limits;
    }
}
