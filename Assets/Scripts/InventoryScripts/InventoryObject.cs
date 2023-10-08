using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using InventorySystem;

public class InventoryObject : BaseItem, IInventoryItem
{
    private InventoryController _inventoryController;
    [Header("Debug Position")]
    [SerializeField] private Transform _debugPos;

    public event Action<InventoryObject> ItemPickedUp = delegate { };

    public delegate void OnItemAddToSocket(InventoryObject gameObject);
    public OnItemAddToSocket onItemAddToSocket;

    public override void Awake()
    {
        base.Awake();
        _inventoryController = GameManager.Instance.inventoryManager.InventoryController;

        if (GameManager.Instance.GetDebugMode() && _debugPos != null)
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

        _inventoryController.onShowItemText?.Invoke(this);
    }

    public override void SelectExited(SelectExitEventArgs args)
    {
        base.SelectExited(args);
        if (args.interactorObject.transform.name != "Direct Interactor") return;

        var rb = args.interactableObject.transform.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        Debug.Log($"SelectExited on {args.interactableObject.transform.name} called");
        _inventoryController.onItemAddToInventory?.Invoke(gameObject);
    }

    public void SocketEnter()
    {
        _inventoryController.onItemAddToSocket(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.TransformPoint(TextPos), 0.1f);
    }
}
