using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using InventorySystem;

public class InventoryObject : BaseItem, IInventoryItem
{
    [Header("Debug Options")]
    [SerializeField] private bool _setDebugPosition;
    [SerializeField] private Transform _debugPos;

    public event Action<InventoryObject> ItemPickedUp = delegate { };

    public delegate void OnItemAddToSocket(InventoryObject gameObject);
    public OnItemAddToSocket onItemAddToSocket;

    public override void Awake()
    {
        base.Awake();
        if (GameManager.Instance.GetItemDebugMode() || _setDebugPosition && (_debugPos !=null))
        {
            transform.position = _debugPos.position;
        }
    }

    public override void OnEnable() => base.OnEnable();

    public override void OnDisable() => base.OnDisable();

    public override void HoverEntered(HoverEnterEventArgs args)
    {
        base.HoverEntered(args);
        if (args.interactorObject.transform.name != "Direct Interactor") return;
    }

    public override void SelectEntered(SelectEnterEventArgs args)
    {
        base.SelectEntered(args);
        if (args.interactorObject.transform.name != "Direct Interactor") return;

        InventoryController.onShowItemText?.Invoke(this);
    }

    public override void SelectExited(SelectExitEventArgs args)
    {
        base.SelectExited(args);
        if (args.interactorObject.transform.name != "Direct Interactor") return;

        var rb = args.interactableObject.transform.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        Debug.Log($"SelectExited on {args.interactableObject.transform.name} called");
        InventoryController.onItemAddToInventory?.Invoke(gameObject);
    }

    public void SocketEnter()
    {
        InventoryController.onItemAddToSocket(this);
        BaseInteractable.interactionLayers = ToggleInteractionLayer(BaseInteractable.interactionLayers, false);
    }

    private InteractionLayerMask ToggleInteractionLayer(InteractionLayerMask layerMask, bool isOn)
    {
        int layer = isOn ? 1 : 2;
        layerMask = (1 << layer);
        return layerMask;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.TransformPoint(TextPos), 0.1f);
    }
}
