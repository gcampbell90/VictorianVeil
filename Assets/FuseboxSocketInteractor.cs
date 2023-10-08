using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class FuseboxSocketInteractor : XRSocketInteractor
{
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log("Checking Item");
        if (!CanHover(args.interactableObject))
        {
            Debug.Log("Not a valid Item");

            return;
        }

        base.OnHoverEntered(args);

        args.interactableObject.transform.TryGetComponent(out InventoryObject inventoryObject);
        inventoryObject.SocketEnter();
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + new Vector3(0, 0.1f, 0), $"{transform.name}");
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.05f);
    }

}
