using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class FuseboxSocketInteractor : XRSocketInteractor
{
    [Space]
    [SerializeField]
    [Tooltip("The required keyobject to interact with this socket.")]
    Lock m_Lock;

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        //Debug.Log("Checking Item");
        bool isInventory = args.interactableObject.transform.TryGetComponent(out InventoryObject inventoryObject);

        if (!isInventory)
        {
            //Debug.Log("Not a valid Item");

            return;
        }

        base.OnHoverEntered(args);

        inventoryObject.SocketEnter();
    }

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        Debug.Log($"Checking CanHover Item for {gameObject.name}");

        if (!base.CanHover(interactable))
            return false;
        Debug.Log("Checking Key");

        var keyChain = interactable.transform.GetComponent<IKeychain>();
        return m_Lock.CanUnlock(keyChain);
    }

    /// <inheritdoc />
    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        Debug.Log($"Checking CanSelect Item for {gameObject.name}");

        if (!base.CanSelect(interactable))
            return false;

        Debug.Log("Checking Key");

        var keyChain = interactable.transform.GetComponent<IKeychain>();
        return m_Lock.CanUnlock(keyChain);
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + new Vector3(0, 0.1f, 0), $"{transform.name}");
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.05f);
    }

}
