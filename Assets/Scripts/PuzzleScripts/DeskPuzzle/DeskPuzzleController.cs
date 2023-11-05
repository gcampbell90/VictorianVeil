using System.Collections.Generic;
using UnityEngine;
using static TyperwriterController;

public class DeskPuzzleController : BasePuzzle
{
    //public GameObject drawer;
    [SerializeField] private List<XRSlideable> moveableDrawers = new List<XRSlideable>();
    bool drawer3Open, drawer2Open;

    //Unity Events
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        TyperwriterController.OnUnlockDesk += UnlockDeskSideDrawer;
    }


    protected override void Start()
    {
        base.Start();
        if (DebugMode)
        {
            UnlockDeskSideDrawer();
            UnlockDeskCenterDrawer();
            return;
        }
        //lock middle drawer only at start
        moveableDrawers[0].slideableItem.IsUnlocked = false;
        moveableDrawers[1].slideableItem.IsUnlocked = false;
        moveableDrawers[2].slideableItem.IsUnlocked = true;
    }

    private void OnDisable()
    {
        TyperwriterController.OnUnlockDesk -= UnlockDeskSideDrawer;
    }

    //Member Methods
    private void UnlockDeskSideDrawer()
    {
        moveableDrawers[1].slideableItem.IsUnlocked = true;
    }

    //TODO: Fix overflow with constant calls 
    public void SetDrawer2Value(float value)
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

    public void SetDrawer3Value(float value)
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

    private void CheckUnlockVals()
    {
        if (drawer3Open && drawer2Open)
        {
            UnlockDeskCenterDrawer();
        }
    }

    private void UnlockDeskCenterDrawer()
    {
        moveableDrawers[0].slideableItem.IsUnlocked = true;
        CompletePuzzle();
    }

    private new void CompletePuzzle()
    {
        base.CompletePuzzle();
    }
}
