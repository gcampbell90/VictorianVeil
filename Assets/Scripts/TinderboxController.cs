using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TinderboxController : MonoBehaviour
{
    [SerializeField] private GameObject key;
    [SerializeField] private float openPosition;
    private FireplaceKey fireplaceKey;

    private void Awake()
    {
        fireplaceKey = key.GetComponent<FireplaceKey>();
    }

    private void OnEnable()
    {
        fireplaceKey.XRSlideable.onMovementCompleted += UnlockBox;
    }

    private void OnDisable()
    {
        fireplaceKey.XRSlideable.onMovementCompleted -= UnlockBox;
    }
    private void UnlockBox()
    {
        fireplaceKey.XRSlideable.onMovementCompleted -= UnlockBox;
        transform.GetChild(2).GetComponent<BoxCollider>().enabled = true;
        Destroy(this);
    }
}
