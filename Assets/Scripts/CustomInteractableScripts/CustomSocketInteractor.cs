using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomSocketInteractor : XRSocketInteractor
{
    [Space]
    [SerializeField]
    [Tooltip("The required keyobject to interact with this socket.")]
    Lock m_Lock;

    public delegate void SocketHandler(InventoryObject inventoryObject);

    //This event will forcefully detach the correct inventory item from the interactor to enter the socket
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        
        Debug.Log("Checking Item ");

        if (!CanHover(args.interactableObject))
        {
            Debug.Log("Not a valid Item");

            return;
        }

        base.OnHoverEntered(args);

        bool isInventory = args.interactableObject.transform.TryGetComponent(out InventoryObject inventoryObject);

        if(isInventory && !hasSelection)
        {
            inventoryObject.SocketEnter();

        }
        else
        {
            return;
        }
    }

    /// <inheritdoc />
    public override bool CanHover(IXRHoverInteractable interactable)
    {
        //Debug.Log($"Checking CanHovekr Item for {gameObject.name}");


        if (!base.CanHover(interactable))
            return false;
        //Debug.Log("Checking Key");

        var keyChain = interactable.transform.GetComponent<IKeychain>();
        return m_Lock.CanUnlock(keyChain);
    }

    /// <inheritdoc />
    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        //Debug.Log($"Checking CanSelect Item for {gameObject.name}");

        if (!base.CanSelect(interactable))
            return false;

        //Debug.Log("Checking Key");

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
