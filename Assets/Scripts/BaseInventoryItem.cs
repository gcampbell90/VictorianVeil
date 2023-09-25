using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class BaseInventoryItem : MonoBehaviour, IInventoryItem
{
    public XRBaseInteractable Item { get; private set; }
    private Rigidbody _rb;
    
    public enum ItemType { key, other };
    public ItemType itemType;

    [SerializeField] private Vector3 _textPos;

    private void Awake()
    {
        Item = GetComponent<XRGrabInteractable>();
        _rb = transform.GetComponent<Rigidbody>();


    }
    public virtual void OnEnable()
    {
        Item.hoverEntered.AddListener(HoverEnter);
        Item.selectEntered.AddListener(SelectEnter);
    }

    public virtual void OnDisable()
    {
        Item.hoverEntered.RemoveListener(HoverEnter);
        Item.selectEntered.RemoveListener(SelectEnter);
    }

    public virtual void HoverEnter(HoverEnterEventArgs args)
    {
        Debug.Log($"Item  '{args.interactableObject.transform.name}' hovered by '{args.interactorObject.transform.name}'");

        InventoryManager.Instance.InventoryController.ShowText(transform, transform.InverseTransformPoint(_textPos));

    }

    public virtual void SelectEnter(SelectEnterEventArgs args)
    {
        Item.selectEntered.RemoveListener(SelectEnter);

        InventoryManager.Instance.InventoryController.ItemPickedUp(Item.gameObject);
        Debug.Log($"Item  '{args.interactableObject.transform.name}' Selected by '{args.interactorObject.transform.name}'");
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.InverseTransformPoint(_textPos), Vector3.one * 0.01f);
    }
}