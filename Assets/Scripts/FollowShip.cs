using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowShip : MonoBehaviour
{
    public Transform shipTransform;
    private Vector3 initialOffset;
    private Quaternion initialRotation;

    void Start()
    {
        initialOffset = transform.position - shipTransform.position;
        initialRotation = Quaternion.Inverse(shipTransform.rotation) * transform.rotation;
    }

    void FixedUpdate()
    {
        transform.position = shipTransform.position + initialOffset;
        transform.rotation = shipTransform.rotation * initialRotation;
    }
}
