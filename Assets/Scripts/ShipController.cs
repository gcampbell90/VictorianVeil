using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{

    public void RotateShip(float rotation)
    {
        var rot = new Vector3(0, rotation, 0);
        transform.Rotate(rot);
    }
}
