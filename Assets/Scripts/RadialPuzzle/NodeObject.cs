using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class NodeObject : MonoBehaviour
{
    //public bool isJunction;
    public List<Transform> adjacentNodes = new List<Transform>();
    public enum NodeLabel { W,X,Y,Z,None }
    public NodeLabel nodeLabel;


    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, $"{nodeLabel} Place");

        Gizmos.DrawWireSphere(transform.position, 0.001f);
        for (int i = 0; i < adjacentNodes.Count; i++)
        {
            Gizmos.DrawLine(transform.position, adjacentNodes[i].position);

        }
    }
}
