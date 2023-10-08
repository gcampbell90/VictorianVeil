using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DeskController : MonoBehaviour
{
    //public GameObject drawer;
    [SerializeField] private List<XRSlideable> moveableDrawers = new List<XRSlideable>();
    bool drawer3Open, drawer2Open;
    [Header("Debug Options")]
    [SerializeField] bool isUnlocked;
    private void OnEnable()
    {
        TyperwriterController.OnUnlockDesk += UnlockDeskSideDrawer;
    }
    private void OnDisable()
    {
        TyperwriterController.OnUnlockDesk -= UnlockDeskSideDrawer;
    }
    private void Start()
    {
        if (GameManager.Instance.GetDebugMode() || isUnlocked)
        {
            moveableDrawers[0].slideableItem.IsUnlocked = true;
            moveableDrawers[1].slideableItem.IsUnlocked = true;
            moveableDrawers[2].slideableItem.IsUnlocked = true;
        }
        else
        {
            //lock middle drawer only at start
            moveableDrawers[0].slideableItem.IsUnlocked = false;
            moveableDrawers[1].slideableItem.IsUnlocked = false;
            moveableDrawers[2].slideableItem.IsUnlocked = true;
        }
    }

    private void UnlockDeskSideDrawer()
    {
        moveableDrawers[1].slideableItem.IsUnlocked = true;
    }
    private void UnlockDeskCenterDrawer()
    {
        Debug.Log("CenterDrawerUnlocked");
        moveableDrawers[0].slideableItem.IsUnlocked = true;
    }

    public void GetDrawer2Value(float value)
    {
        Debug.Log(value);
        if (value <= .80f && value >= .70f)
        {
            drawer2Open = true;
        }
        else
        {
            drawer2Open = false;
        }
        CheckUnlockVals();
    }
    public void GetDrawer3Value(float value)
    {
        Debug.Log(value);
        if (value <= .80f && value >= .70f)
        {
            drawer3Open = true;
        }
        else
        {
            drawer3Open = false;
        }
        CheckUnlockVals();
    }
    void CheckUnlockVals()
    {
        if (drawer3Open && drawer2Open)
        {

            UnlockDeskCenterDrawer();
        }
    }
}
