using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node
{
    public int index;
    public Vector3 position;
    public List<int> adjacentNodes;
    public bool isOccupied;
}

