using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public RouteData routeData { get; set; }

    //Sets waypoints when path entered
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"{transform.name}: Route {other.name}");

        //Ignore interactors
        if (other.name == "Pusher" || other.name == "Direct Interactor") return;
        routeData = RadialPuzzleController.getRouteData(other.name);
        transform.position = other.transform.position;
        if (other.name == "PathA_2" || other.name == "PathB_2")
        {
            Debug.Log("Reversing Array");
            var reversedNodes = new Node[routeData.Nodes.Length];
            Array.Copy(routeData.Nodes, reversedNodes, routeData.Nodes.Length);
            Array.Reverse(reversedNodes);
            GetComponent<WaypointInteractable>().SetWaypoints(reversedNodes);
        }
        else
        {
            GetComponent<WaypointInteractable>().SetWaypoints(routeData.Nodes);
        }

        Debug.Log($"TriggerEntered: {other.name}. Setting nodes: {routeData.Nodes}");
    }
}