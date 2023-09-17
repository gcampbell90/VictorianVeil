using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSlideableWaypoint : MonoBehaviour
{
    public SlideableItem slideableItem { get; private set; }
    private Vector3 grabPos;

    [Serializable]
    public class ValueChangeEvent : UnityEvent<float> { }

    [SerializeField]
    private ValueChangeEvent m_OnValueChange;
    public ValueChangeEvent onValueChange => m_OnValueChange;

    public delegate void MovementCompleted();
    public MovementCompleted onMovementCompleted;

    [SerializeField] private Vector3 direction;
    public float openPosition;

    [SerializeField] private Transform[] waypoints;

    int currIndex = 0;
    private void Awake()
    {
        slideableItem = new SlideableItem()
        {
            interactable = gameObject.AddComponent<XRSimpleInteractable>(),
            mover = gameObject.AddComponent<PhysicsMover>(),
            closePos = transform.position,
            openPos = transform.position + transform.TransformDirection(direction) * openPosition,
            IsUnlocked = true
        };
    }

    internal void AddWaypoints(Transform[] transforms)
    {
        waypoints = transforms;
        slideableItem.closePos = transform.position;
        slideableItem.openPos = waypoints[currIndex].position;
        slideableItem.currPosPercentage = 0;
        slideableItem.startPosPercentage = 0;
    }

    private void OnEnable()
    {
        slideableItem.interactable.selectEntered.AddListener((args) => HandleCheck(args));
    }
    private void OnDisable()
    {
        if (slideableItem != null)
            slideableItem.interactable.selectEntered.RemoveListener((args) => HandleCheck(args));
    }

    #region Interactions
    private void HandleCheck(SelectEnterEventArgs args)
    {
        bool m_isOpen = slideableItem.IsUnlocked;

        if (m_isOpen)
        {
            slideableItem.startPosPercentage = slideableItem.currPosPercentage;
            grabPos = args.interactorObject.transform.position;
            StartCoroutine(TrackHandPos());
        }
        else
        {
            Debug.Log("Locked Drawer");
        }
    }
    IEnumerator TrackHandPos()
    {
        //Debug.Log("Handle Tracking Enabled");
        while (slideableItem.interactable.isSelected)
        {
            UpdatePos();
            yield return null;
        }
        //Debug.Log("Handle released");
    }
    private void UpdatePos()
    {
        float percentageOpen = slideableItem.startPosPercentage + CalculateNormalisedPosDifference();

        slideableItem.mover.MoveTo(Vector3.Lerp(slideableItem.closePos, slideableItem.openPos, percentageOpen));

        slideableItem.currPosPercentage = Mathf.Clamp01(percentageOpen);
        m_OnValueChange.Invoke(slideableItem.currPosPercentage);

        if (slideableItem.currPosPercentage >= 1f)
        {
            MovementCompletedEventCall();
            Debug.Log($"{gameObject.name} onMovementCompleted Invoked");
        }
    }
    private void MovementCompletedEventCall()
    {
        //onMovementCompleted?.Invoke();
        currIndex++;
        slideableItem.interactable = GetComponent<XRSimpleInteractable>();
        slideableItem.mover = GetComponent<PhysicsMover>();
        slideableItem.closePos = transform.position;
        slideableItem.openPos = waypoints[currIndex].position;
        slideableItem.currPosPercentage = 0;
        slideableItem.startPosPercentage = 0;
        grabPos = slideableItem.interactable.interactorsSelecting[0].transform.position;
    }
    float CalculateNormalisedPosDifference()
    {
        Vector3 handPos = slideableItem.interactable.interactorsSelecting[0].transform.position;
        Vector3 pullDir = handPos - grabPos;
        Vector3 targetDirection = slideableItem.closePos - slideableItem.openPos;

        float length = -targetDirection.magnitude;

        targetDirection.Normalize();

        //Debug.Log($"{Vector3.Dot(pullDir, targetDirection) / length}");
        //Debug.Log(pullDir);
        return Vector3.Dot(pullDir, targetDirection) / length;
    }
    #endregion

    void OnDrawGizmos()
    {
        Vector3 startPos;
        Vector3 worldEndPos;

        if (slideableItem == null)
        {
            //Debug.Log("Drawing Gizmo " + slideableItem);

            startPos = transform.position;
            worldEndPos = transform.position + transform.TransformDirection(direction) * openPosition;
        }
        else
        {
            //Debug.Log("Drawing Gizmo " + slideableItem);

            startPos = slideableItem.closePos;
            worldEndPos = slideableItem.openPos;
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPos, 0.05f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(worldEndPos, 0.05f);

        Gizmos.DrawLine(startPos, worldEndPos);
        Gizmos.DrawWireSphere(worldEndPos, 0.05f);

        Gizmos.color = Color.white;
        foreach (var item in waypoints)
        {
            Gizmos.DrawWireSphere(item.position, 0.1f);
        }

    }

    private void OnDestroy()
    {
        Destroy(slideableItem.mover);
        Destroy(slideableItem.interactable);
    }
}
