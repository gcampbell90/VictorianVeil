using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class RadialPuzzleController : MonoBehaviour
{
    [SerializeField] private Transform radialPiece;
    XRKnob xRKnob;
    [Header("PiecePlaces")]
    [SerializeField] private NodeObject[] wx_route;
    public Node[] WX_Route { get; set; } = new Node[4];

    [SerializeField] private NodeObject[] yz_route;
    public Node[] YZ_Route { get; set; }

    [SerializeField] private NodeObject[] northRoute;
    public Node[] NorthRoute { get; set; }
    [SerializeField] private NodeObject[] southRoute;
    public Node[] SouthRoute { get; set; }

    [Header("Blue")]
    public RouteData routeA;
    [Header("Red")]
    public RouteData routeB;

    [SerializeField] private Transform puzzlePiece1;
    [SerializeField] private Transform puzzlePiece2;
    [SerializeField] private Transform puzzlePiece3;
    [SerializeField] private Transform puzzlePiece4;

    //Events
    public delegate RouteData GetRouteData(string name);
    public static GetRouteData getRouteData;

    public delegate Node[] GetStaticRouteData(string name);
    public static GetStaticRouteData getStaticRouteData;

    public delegate void ToggleXRKnob(bool isOn);
    public static ToggleXRKnob toggleXRKnob;

    public delegate bool CheckIfBlocked(WaypointInteractable puzzlePiece);
    public static CheckIfBlocked checkIfBlocked;

    public delegate void CheckIfCompleted();
    public static CheckIfCompleted checkIfCompleted;

    private void OnEnable()
    {
        getRouteData += GetRoute;
        getStaticRouteData += GetStaticRoute;

        toggleXRKnob += ToggleKnob;
        checkIfBlocked += IsRouteOccupied;
        checkIfCompleted += AllPieceSet;
    }
    private void OnDisable()
    {
        getRouteData -= GetRoute;
        getStaticRouteData -= GetStaticRoute;

        toggleXRKnob -= ToggleKnob;
        checkIfBlocked -= IsRouteOccupied;
        checkIfCompleted -= AllPieceSet;
    }
    private void Awake()
    {
        xRKnob = GetComponent<XRKnob>();

        WX_Route = SetStaticRoute(route: wx_route);
        YZ_Route = SetStaticRoute(route: yz_route);
        NorthRoute = SetStaticRoute(route: northRoute);
        SouthRoute = SetStaticRoute(route: southRoute);

        SetRadialRoutes();

    }

    //private void SetStaticRoutes()
    //{
    //    Vector3[] staticNodeVectors = new[] {
    //        new Vector3(0f, 0f ,0.8f),
    //        new Vector3(0.5f, 0f, 0.8f),
    //        new Vector3(-0.5f, 0f ,0.8f),
    //        new Vector3(0f, 0f, 0.5f)
    //    };


    //    for (int i = 0; i < 4; i++)
    //    {
    //        Node staticNode = new Node();
    //        staticNode.index = i;
    //        staticNode.position = transform.TransformPoint(staticNodeVectors[i]);

    //        List<int> m_nodeList = new List<int>();
    //        //If route node(junction) add all nodes
    //        if (i == 0)
    //        {
    //            m_nodeList.Add(1);
    //            m_nodeList.Add(2);
    //            m_nodeList.Add(3);
    //        }
    //        else//add only junction node
    //        {
    //            m_nodeList.Add(0);
    //        }
    //        staticNode.adjacentNodes = m_nodeList;
    //        WX_Route[i] = staticNode;
    //    }

    //}
    Node[] SetStaticRoute(NodeObject[] route)
    {
        var targetRoute = new Node[route.Length];
        for (int i = 0; i < route.Length; i++)
        {
            Node staticNode = new Node();
            staticNode.index = i;
            staticNode.position = route[i].transform.position;
            List<int> m_nodeList = new List<int>();
            //If route node(junction) add all nodes
            if (i == 0)
            {
                m_nodeList.Add(1);
                m_nodeList.Add(2);
                m_nodeList.Add(3);
            }
            else//add only junction node
            {
                m_nodeList.Add(0);
            }
            staticNode.adjacentNodes = m_nodeList;

            targetRoute[i] = staticNode;
        }
        return targetRoute;
    }
    private void SetRadialRoutes()
    {
        //Radial Routes
        routeA.Nodes = new Node[routeA.Waypoints.Length];
        routeB.Nodes = new Node[routeB.Waypoints.Length];
        //Debug.Log("Setting route A");
        for (int i = 0; i < routeA.Waypoints.Length; i++)
        {
            Node node = new Node();
            node.index = i;
            node.position = radialPiece.TransformPoint(routeA.Waypoints[i]);
            routeA.Nodes[i] = node;
            //Debug.Log($"{routeA.Nodes[i]} A: {routeA.Nodes[i].index},{routeA.Nodes[i].position}");
        }
        //Set adjacent nodes
        SetAdjacentNodes(routeA);

        //Debug.Log("Setting route B");
        for (int i = 0; i < routeB.Waypoints.Length; i++)
        {
            Node node = new Node();
            node.index = i;
            node.position = radialPiece.TransformPoint(routeB.Waypoints[i]);
            routeB.Nodes[i] = node;
            //Debug.Log($"{routeB.Nodes[i]} B: {routeB.Nodes[i].index},{routeB.Nodes[i].position}");
        }
        //Set adjacent nodes
        SetAdjacentNodes(routeB);
    }
    private void SetAdjacentNodes(RouteData route)
    {
        for (int i = 0; i < route.Nodes.Length; i++)
        {
            List<int> m_nodeList = new List<int>();
            int m_index = route.Nodes[i].index;
            if (route.Nodes[i].index == 0 && route.Nodes.Length > 1)
            {
                m_nodeList.Add(route.Nodes[1].index);
            }
            else if (route.Nodes[i].index == route.Nodes.Length - 1 && route.Nodes.Length > 1)
            {
                m_nodeList.Add(route.Nodes[m_index - 1].index);
            }
            else if (route.Nodes[i].index > 0 && route.Nodes[i].index < route.Nodes.Length - 1)
            {
                m_nodeList.Add(route.Nodes[m_index - 1].index);
                m_nodeList.Add(route.Nodes[m_index + 1].index);
            }
            route.Nodes[i].adjacentNodes = m_nodeList;
        }
    }
    private void ToggleKnob(bool isOn)
    {
        xRKnob.enabled = isOn;
    }
    private RouteData GetRoute(string name)
    {
        //Debug.Log($"Setting Dynamic Route {name}");

        if (name == "PathA_1" || name == "PathA_2")
        {
            //Debug.Log("Setting route A");
            for (int i = 0; i < routeA.Nodes.Length; i++)
            {
                routeA.Nodes[i].position = radialPiece.TransformPoint(routeA.Waypoints[i]);
                //Debug.Log($"{routeA.Nodes[i]} A: {routeA.Nodes[i].index},{routeA.Nodes[i].position}");
            }
            return routeA;
        }

        if (name == "PathB_1" || name == "PathB_2")
        {
            //Debug.Log("Setting route B");
            for (int i = 0; i < routeB.Nodes.Length; i++)
            {
                routeB.Nodes[i].position = radialPiece.TransformPoint(routeB.Waypoints[i]);
            }
            return routeB;
        }
        else
        {
            return null;
        }
    }
    private Node[] GetStaticRoute(string name)
    {
        //Debug.Log($"Setting Static Route {name}");

        switch (name)
        {
            case "WXJunction":
                return WX_Route;
            case "YZJunction":
                return YZ_Route;
            case "NorthJunction":
                return NorthRoute;
            case "SouthJunction":
                return SouthRoute;
            case "W":
                return WX_Route;
            case "X":
                return WX_Route;
            case "Y":
                return YZ_Route;
            case "Z":
                return YZ_Route;
            default:
                return null;
        }
    }
    public bool IsRouteOccupied(WaypointInteractable waypointInteractable)
    {
        if (waypointInteractable.TargetNode != null)
        {
            Node activePuzzleTarget = waypointInteractable.TargetNode;

            if (activePuzzleTarget.isOccupied)
            {
                return true;
            }
            else
            {
                return false;
            }
            //Debug.Log(activePuzzleTarget);
        }
        else
        {
            return false;
        }

    }
    public void AllPieceSet()
    {
        var piece1Component = puzzlePiece1.GetComponent<PuzzlePiece>();
        var piece2Component = puzzlePiece2.GetComponent<PuzzlePiece>();
        var piece3Component = puzzlePiece3.GetComponent<PuzzlePiece>();
        var piece4Component = puzzlePiece4.GetComponent<PuzzlePiece>();
        if (piece1Component.IsSet == true && piece2Component.IsSet == true && piece3Component.IsSet == true && piece4Component.IsSet == true)
        {
            Debug.Log("PuzzleSolved");
        }
    }

    private void OnDrawGizmos()
    {
        Handles.Label(puzzlePiece1.transform.position, $"{puzzlePiece1.GetComponent<PuzzlePiece>().pieceLabel} Piece");
        Handles.Label(puzzlePiece2.transform.position, $"{puzzlePiece2.GetComponent<PuzzlePiece>().pieceLabel} Piece");
        Handles.Label(puzzlePiece3.transform.position, $"{puzzlePiece3.GetComponent<PuzzlePiece>().pieceLabel} Piece");
        Handles.Label(puzzlePiece4.transform.position, $"{puzzlePiece4.GetComponent<PuzzlePiece>().pieceLabel} Piece");

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
        Gizmos.color = Color.green;
        if (routeA.Nodes != null)
        {
            for (int i = 0; i < routeA.Nodes.Length; i++)
            {
                Gizmos.DrawWireSphere(routeA.Nodes[i].position, 0.025f);
            }
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
        if (routeB.Nodes != null)
        {
            for (int i = 0; i < routeB.Nodes.Length; i++)
            {
                Gizmos.DrawWireSphere(routeB.Nodes[i].position, 0.025f);
            }
        }

        if (WX_Route != null)
        {
            Gizmos.color = Color.gray;
            for (int i = 0; i < WX_Route.Length; i++)
            {
                Gizmos.DrawWireSphere(WX_Route[i].position, 0.01f);
                for (int j = 0; j < WX_Route[i].adjacentNodes.Count; j++)
                {
                    Gizmos.DrawLine(WX_Route[i].position, WX_Route[j].position);
                }
            }
        }

        if (YZ_Route != null)
        {
            Gizmos.color = Color.gray;
            for (int i = 0; i < YZ_Route.Length; i++)
            {
                Gizmos.DrawWireSphere(YZ_Route[i].position, 0.01f);
                for (int j = 0; j < YZ_Route[i].adjacentNodes.Count; j++)
                {
                    Gizmos.DrawLine(YZ_Route[i].position, YZ_Route[j].position);
                }
            }
        }
        if (NorthRoute != null)
        {
            Gizmos.color = Color.gray;
            for (int i = 0; i < NorthRoute.Length; i++)
            {
                Gizmos.DrawWireSphere(NorthRoute[i].position, 0.01f);
                for (int j = 0; j < NorthRoute[i].adjacentNodes.Count; j++)
                {
                    Gizmos.DrawLine(NorthRoute[i].position, NorthRoute[j].position);
                }
            }
        }
        if (SouthRoute != null)
        {
            Gizmos.color = Color.gray;
            for (int i = 0; i < SouthRoute.Length; i++)
            {
                Gizmos.DrawWireSphere(SouthRoute[i].position, 0.01f);
                for (int j = 0; j < SouthRoute[i].adjacentNodes.Count; j++)
                {
                    Gizmos.DrawLine(SouthRoute[i].position, SouthRoute[j].position);
                }
            }
        }
    }
}
[Serializable]
public class RouteData
{
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