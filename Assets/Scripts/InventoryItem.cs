using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryItem: MonoBehaviour
{
    public XRBaseInteractable Item { get; set; }
    public enum ItemType { key, other };
    public ItemType itemType;
}