using UnityEngine.XR.Interaction.Toolkit;

internal interface IInventoryItem
{
    void HoverEntered(HoverEnterEventArgs args);
    void SelectEntered(SelectEnterEventArgs args);
}