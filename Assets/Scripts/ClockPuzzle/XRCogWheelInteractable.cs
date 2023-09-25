using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRCogWheelInteractable : MonoBehaviour
{
    private XRSimpleInteractable _interactable;
    private Vector3 _grabPos;
    private Color _rayColor;
    public Transform CogTransform { get; private set; }

    public int currNum { get; set; } = 0;
    private void OnEnable()
    {
        _interactable.selectEntered.AddListener(HandleCheck);
    }
    private void OnDisable()
    {
        _interactable.selectEntered.RemoveListener(HandleCheck);
    }

    private void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        CogTransform = transform.GetChild(0).transform;
    }
    private void HandleCheck(SelectEnterEventArgs args)
    {
        _grabPos = args.interactorObject.transform.position;
        StartCoroutine(TrackHandPos());
    }

    private IEnumerator TrackHandPos()
    {
        //Debug.Log("Object Grabbed");

        while (_interactable.isSelected)
        {
            // Visualize the pull direction using Debug.DrawLine
            VisualisePullDir();
            MoveCog();
            yield return null;
        }
        //Debug.Log("Let go of object");
    }

    private void MoveCog()
    {
        Vector3 handPos = _interactable.interactorsSelecting[0].transform.position;
        Vector3 pullDirection = (handPos - _grabPos).normalized;

        float dotUp = Vector3.Dot(Vector3.up, pullDirection);

        if (dotUp > 0.5f) // Adjust this threshold as necessary
        {
            Debug.Log("MoveWheel Up");
            StartCoroutine(RotateCog(true));
        }
        else if (dotUp < -0.5f) // Adjust this threshold as necessary
        {
            Debug.Log("MoveWheel Down");
            StartCoroutine(RotateCog(false));
        }
    }

    private IEnumerator RotateCog(bool upwards)
    {
        float timer = 0f;
        float dur = 0.5f;

        Quaternion startRotation = CogTransform.localRotation;

        Quaternion rotationChange = Quaternion.AngleAxis(36, Vector3.right);  // 36-degree rotation around the x-axis
        Quaternion endRotation = upwards ? CogTransform.localRotation * rotationChange : CogTransform.localRotation * Quaternion.Inverse(rotationChange);


        _interactable.enabled = false;
        while (timer < dur)
        {
            CogTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, timer / dur);
            timer += Time.deltaTime;
            yield return null;
        }

        CogTransform.localRotation= endRotation;  // Ensure it finishes in the desired state
        _interactable.enabled = true;

        SetNumber(upwards);
    }

    private void SetNumber(bool upwards)
    {

        if (upwards && currNum == 0)
        {
            currNum = 9;
        }
        else if (!upwards && currNum == 9)
        {
            currNum = 0;
        }
        else if (upwards)
        {
            currNum--;
        }
        else
        {
            currNum++;
        }
        CombinationLock.setCogNumber?.Invoke(this);
    }

    private void VisualisePullDir()
    {
        Vector3 handPos = _interactable.interactorsSelecting[0].transform.position;
        // Remove the y component
        handPos.x = 0;
        handPos.z = 0;

        Vector3 pullDirection = (handPos - _grabPos).normalized;
        //Debug.Log(_grabPos);
        pullDirection.x = 0; // Ensure y component is 0
        pullDirection.z = 0; // Ensure y component is 0

        // Calculate dot products for each direction
        float dotForward = Vector3.Dot(transform.forward, pullDirection);
        float dotRight = Vector3.Dot(transform.right, pullDirection);

        // Use the up vector as a proxy for 'behind' since we're in 3D space.
        // If you're in 2D space or want to consider the real 'behind' vector, use -transform.forward

        _rayColor = Color.blue; // Default

        // Determine which direction has the highest alignment with pullDirection
        if (dotForward > 0.5f) // Adjust thresholds as necessary
        {
            _rayColor = Color.green; // Forward
        }
        else if (dotForward < -0.5f)
        {
            _rayColor = Color.red; // Behind
        }
        else if (dotRight > 0.5f)
        {
            _rayColor = Color.yellow; // Right
        }
        else if (dotRight < -0.5f)
        {
            _rayColor = Color.magenta; // Left
        }
        Debug.DrawRay(transform.position, pullDirection, _rayColor);
    }
}
