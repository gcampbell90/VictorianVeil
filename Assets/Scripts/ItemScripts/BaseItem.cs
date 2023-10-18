using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class BaseItem : MonoBehaviour
{
    [SerializeField] private string _itemName;
    public string ItemName { get => _itemName; set => _itemName = value; }
    public XRBaseInteractable BaseInteractable { get; private set; }
    private Rigidbody _rb;

    public enum ItemType { key, other };
    public ItemType itemType;

    public Vector3 TextPos { get => _textPos; set => _textPos = value; }
    [SerializeField] private Vector3 _textPos;


    public virtual void Awake()
    {
        BaseInteractable = GetComponent<XRGrabInteractable>();
        _rb = transform.GetComponent<Rigidbody>();

        if (_itemName == null || _itemName == "")
        {
            _itemName = transform.name;
        }

        //Debug.Log($"After setting in base: {_itemName}");
    }

    public virtual void OnEnable()
    {
        BaseInteractable.hoverEntered.AddListener(HoverEntered);
        BaseInteractable.selectEntered.AddListener(SelectEntered);
        BaseInteractable.selectExited.AddListener(SelectExited);
    }

    public virtual void OnDisable()
    {
        BaseInteractable.hoverEntered.RemoveListener(HoverEntered);
        BaseInteractable.selectEntered.RemoveListener(SelectEntered);
        BaseInteractable.selectExited.RemoveListener(SelectExited);
    }

    public virtual void HoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log($"Item  '{args.interactableObject.transform.name}' hovered by '{args.interactorObject.transform.name}'");
    }

    public virtual void SelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log($"Item  '{args.interactableObject.transform.name}' Selected by '{args.interactorObject.transform.name}'");
    }

    public virtual void SelectExited(SelectExitEventArgs args)
    {
        Debug.Log($"Item  '{args.interactableObject.transform.name}' select exited by '{args.interactorObject.transform.name}'");
    }

}