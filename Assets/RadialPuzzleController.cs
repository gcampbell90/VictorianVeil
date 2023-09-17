using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class RadialPuzzleController : MonoBehaviour
{
    [SerializeField] private Transform radialPiece;
    XRKnob xRKnob;

    [Header("Blue")]
    public RouteData routeA;
    [Header("Red")]
    public RouteData routeB;

    private RouteData _routeA, _routeB;

    [SerializeField] private Transform puzzlePiece1;
    [SerializeField] private Transform puzzlePiece2;

    //Events
    public delegate RouteData GetRouteData(string name);
    public static GetRouteData getRouteData;

    public delegate void ToggleXRKnob(bool isOn);
    public static ToggleXRKnob toggleXRKnob;

    public delegate bool CheckIfBlocked(Transform puzzlePiece);
    public static CheckIfBlocked checkIfBlocked;

    private void OnEnable()
    {
        getRouteData += GetRoute;
        toggleXRKnob += ToggleKnob;
        checkIfBlocked += IsRouteOccupied;
    }
    private void OnDisable()
    {
        getRouteData -= GetRoute;
        toggleXRKnob -= ToggleKnob;
        checkIfBlocked -= IsRouteOccupied;
    }
    private void Awake()
    {
        xRKnob = GetComponent<XRKnob>();

        routeA.Nodes = new Node[routeA.Waypoints.Length];
        routeB.Nodes = new Node[routeB.Waypoints.Length];
    }

    private void ToggleKnob(bool isOn)
    {
        xRKnob.enabled = isOn;
    }
    private RouteData GetRoute(string name)
    {
        if (name == "PathA_1" || name == "PathA_2")
        {
            //Debug.Log("Setting route A");
            for (int i = 0; i < routeA.Waypoints.Length; i++)
            {
                Node node = new Node();
                node.index = i;
                node.position = radialPiece.TransformPoint(routeA.Waypoints[i]);
                routeA.Nodes[i] = node;
                //Debug.Log($"{routeA.Nodes[i]} A: {routeA.Nodes[i].index},{routeA.Nodes[i].position}");
            }
            return routeA;
        }

        if (name == "PathB_1" || name == "PathB_2")
        {
            //Debug.Log("Setting route B");
            for (int i = 0; i < routeB.Waypoints.Length; i++)
            {
                Node node = new Node();
                node.index = i;
                node.position = radialPiece.TransformPoint(routeB.Waypoints[i]);
                routeB.Nodes[i] = node;
                //Debug.Log($"{routeB.Nodes[i]} B: {routeB.Nodes[i].index},{routeB.Nodes[i].position}");
            }
            return routeB;
        }
        else
        {
            return null;
        }
    }
    private RouteData ConvertRoutesToLocalSpace(RouteData route)
    {
        var tmpArr = new Vector3[route.Waypoints.Length];
        for (int i = 0; i < route.Waypoints.Length; i++)
        {
            tmpArr[i] = radialPiece.TransformPoint(route.Waypoints[i]);
            //Debug.Log(tmpArr[i]);
        }
        RouteData convertedRouteData = new RouteData()
        {
            StartTransform = route.StartTransform,
            EndTransform = route.EndTransform,
            Waypoints = tmpArr
        };
        return convertedRouteData;

        ////Debug.Log(tmpArr +" "+convertedRouteData.Waypoints);
        //return convertedRouteData;
    }
    public bool IsRouteOccupied(Transform puzzlePiece)
    {
        if (puzzlePiece.GetComponent<WaypointInteractable>().CurrentNode == routeA.Nodes[0])
        {
            Debug.Log("Same Node");
            return true;
        }
        else
        {
            return false;
        }
        //var otherPiece = puzzlePiece == puzzlePiece1 ? puzzlePiece2 : puzzlePiece1;
        //var puzzlePieceIndex = puzzlePiece.GetComponent<WaypointInteractable>().TargetNode.index;
        //var otherPieceIndex = otherPiece.GetComponent<WaypointInteractable>().CurrentNode.index;
        //Debug.Log(puzzlePieceIndex + " " + otherPieceIndex);
        //if (puzzlePiece.GetComponent<WaypointInteractable>().TargetNode.index == otherPiece.GetComponent<WaypointInteractable>().CurrentNode.index)
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}

    }
    private void OnDrawGizmos()
    {
        //RouteA
        Gizmos.color = Color.blue;
        for (int i = 0; i < routeA.Waypoints.Length; i++)
        {
            Gizmos.DrawWireSphere(radialPiece.TransformPoint(routeA.Waypoints[i]), 0.01f);
        }
        for (int i = 0; i < routeA.Waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(radialPiece.TransformPoint(routeA.Waypoints[i]), radialPiece.TransformPoint(routeA.Waypoints[i + 1]));
        }
        //RouteB
        Gizmos.color = Color.red;
        for (int i = 0; i < routeB.Waypoints.Length; i++)
        {
            Gizmos.DrawWireSphere(radialPiece.TransformPoint(routeB.Waypoints[i]), 0.01f);
        }
        for (int i = 0; i < routeB.Waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(radialPiece.TransformPoint(routeB.Waypoints[i]), radialPiece.TransformPoint(routeB.Waypoints[i + 1]));
        }
        Gizmos.color = Color.green;
        if (routeA.Nodes != null)
        {
            for (int i = 0; i < routeA.Nodes.Length; i++)
            {
                Gizmos.DrawWireSphere(routeA.Nodes[i].position, 0.05f);
            }
        }
        if (routeB.Nodes != null)
        {
            for (int i = 0; i < routeB.Nodes.Length; i++)
            {
                Gizmos.DrawWireSphere(routeB.Nodes[i].position, 0.05f);
            }
        }
    }
}

[System.Serializable]
public class RouteData
{
    [SerializeField] private Transform startTransform;
    public Transform StartTransform
    {
        get
        {
            return startTransform;
        }
        set
        {
            startTransform = value;
        }
    }
    [SerializeField] private Transform endTransform;
    public Transform EndTransform
    {
        get
        {
            return endTransform;
        }
        set
        {
            endTransform = value;
        }
    }
    [SerializeField] private Vector3[] waypoints;
    public Vector3[] Waypoints
    {
        get
        {
            return waypoints;
        }
        set
        {
            waypoints = value;
        }
    }
    public Node[] Nodes { get; set; }
    public RouteData() { }
}

[Serializable]
public class Node
{
    public int index;
    public Vector3 position;
}

