using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class BaseItem : MonoBehaviour, IInventoryItem
{
    [SerializeField] private string _itemName;
    public string ItemName { get => _itemName; private set => _itemName = value; }
    public XRBaseInteractable Item { get; private set; }
    private Rigidbody _rb;

    public enum ItemType { key, other };
    public ItemType itemType;

    public Vector3 TextPos { get => _textPos; }
    [SerializeField] private Vector3 _textPos;

    public virtual void Awake()
    {
        Item = GetComponent<XRGrabInteractable>();
        _rb = transform.GetComponent<Rigidbody>();

        if (_itemName == "")
        {
            _itemName = transform.name;
        }
    }

    public virtual void OnEnable()
    {
        Item.hoverEntered.AddListener(HoverEntered);
        Item.selectEntered.AddListener(SelectEntered);
        Item.selectExited.AddListener(SelectExited);
    }

    public virtual void OnDisable()
    {
        Item.hoverEntered.RemoveListener(HoverEntered);
        Item.selectEntered.RemoveListener(SelectEntered);
        Item.selectExited.RemoveListener(SelectExited);
    }

    public virtual void HoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log($"Item  '{args.interactableObject.transform.name}' hovered by '{args.interactorObject.transform.name}'");
    }

    public virtual void SelectEntered(SelectEnterEventArgs args)
    {
        Item.selectEntered.RemoveListener(SelectEntered);
        Debug.Log($"Item  '{args.interactableObject.transform.name}' Selected by '{args.interactorObject.transform.name}'");
    }

    public virtual void SelectExited(SelectExitEventArgs args)
    {
        Debug.Log($"Item  '{args.interactableObject.transform.name}' select exited by '{args.interactorObject.transform.name}'");
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawCube(TextPos, Vector3.one * 0.01f);
    }
}