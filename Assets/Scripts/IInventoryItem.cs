using UnityEngine.XR.Interaction.Toolkit;

internal interface IInventoryItem
{
    void HoverEnter(HoverEnterEventArgs args);
    void SelectEnter(SelectEnterEventArgs args);
}