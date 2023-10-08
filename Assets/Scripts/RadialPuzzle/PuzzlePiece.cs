using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public RouteData routeData { get; set; }
    public bool IsSet { get; internal set; }

    private Direction _currentDirection;
    private WaypointInteractable waypointInteractable;

    public enum PieceLabel { W, X, Y, Z, None }
    public PieceLabel pieceLabel;

    //Enum
    private enum Direction
    {
        Forward,
        Reversed
    }

    //Sets waypoints when path entered
    private void OnTriggerEnter(Collider other)
    {
        //Ignore interactors
        if (other.name == "Pusher" || other.name == "Direct Interactor") return;
        //Debug.Log($"{transform.name}: Route {other.name}");
  
        waypointInteractable = GetComponent<WaypointInteractable>();
        //reset Curr node if present
        if (waypointInteractable.CurrentNode != null)
        {
            waypointInteractable.CurrentNode.isOccupied = false;
        }

        switch (other.name)
        {
            case "PathA_1":
                GetRadialRoute(other);
                break;
            case "PathA_2":
                GetRadialRoute(other);
                break;
            case "PathB_1":
                GetRadialRoute(other);
                break;
            case "PathB_2":
                GetRadialRoute(other);
                break;
            case "WXJunction":
                GetStaticRoute(other, 0);
                break;
            case "YZJunction":
                GetStaticRoute(other, 0);
                break;
            case "NorthJunction":
                GetStaticRoute(other, 0);
                break;
            case "SouthJunction":
                GetStaticRoute(other, 0);
                break;
            case "W":
                GetStaticRoute(other, 1);
                break;
            case "X":
                GetStaticRoute(other, 2);
                break;
            case "Y":
                GetStaticRoute(other, 1);
                break;
            case "Z":
                GetStaticRoute(other, 2);
                break;
            default:
                break;
        }
        transform.position = other.transform.position;
        
    }

    private void GetStaticRoute(Collider other, int nodeIndex)
    {
     
        var staticRouteData = RadialPuzzleController.getStaticRouteData(other.name);
        waypointInteractable.CurrentNode = staticRouteData[nodeIndex];
        waypointInteractable.SetWaypoints(staticRouteData);
        //Debug.Log($"TriggerEntered: {other.name}. Setting nodes: {staticRouteData.Length}");
        SetIsSet(other.transform);
    }

    private void GetRadialRoute(Collider other)
    {
        SetIsSet(other.transform);

        routeData = RadialPuzzleController.getRouteData(other.name);

        _currentDirection = other.name == "PathA_1" || other.name == "PathB_1" ? Direction.Forward : Direction.Reversed;
        switch (_currentDirection)
        {
            case Direction.Forward:
                waypointInteractable.CurrentNode = routeData.Nodes[1];
                break;
            case Direction.Reversed:
                waypointInteractable.CurrentNode = routeData.Nodes[routeData.Nodes.Length - 2];
                break;
            default:
                Debug.LogError("Directional Error in nodes array");
                break;
        }
        waypointInteractable.SetWaypoints(routeData.Nodes);

        //Debug.Log($"TriggerEntered: {other.name}. Setting nodes: {routeData.Nodes}");
    }

    private void SetIsSet(Transform other)
    {
        other.TryGetComponent<NodeObject>(out NodeObject nodeObject);
        if (nodeObject == null)
        {
            IsSet = false;
            return;
        }
        else
        {
            var nodeLabelString = nodeObject.nodeLabel.ToString();
            var pieceLabelString = pieceLabel.ToString();
            if (nodeLabelString == pieceLabelString)
            {
                Debug.Log($"Piece {pieceLabelString} set to {nodeLabelString}");
                IsSet = true;

            }
            else
            {
                IsSet = false;
            }
            RadialPuzzleController.checkIfCompleted();

        }
    }
}