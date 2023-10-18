using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TinderboxPuzzleController : BasePuzzle
{
    [SerializeField] private GameObject key;
    private FireplaceKey _fireplaceKey;
    private CapsuleCollider _lidCollider;

    protected override void Awake()
    {
        base.Awake();
        _lidCollider = transform.GetChild(1).GetComponent<CapsuleCollider>();
        if (_lidCollider.enabled == true)
        {
            _lidCollider.enabled = false;
        }

        _fireplaceKey = key.GetComponent<FireplaceKey>();
    }

    private void OnEnable()
    {
        _fireplaceKey.XRSlideable.onMovementCompleted += UnlockBox;
    }

    private void OnDisable()
    {
        _fireplaceKey.XRSlideable.onMovementCompleted -= UnlockBox;
    }
    private void UnlockBox()
    {
        _fireplaceKey.XRSlideable.onMovementCompleted -= UnlockBox;
        _lidCollider.enabled = true;
        CompletePuzzle();
        Destroy(this);
    }

    private new void CompletePuzzle()
    {
        base.CompletePuzzle();
    }
}
