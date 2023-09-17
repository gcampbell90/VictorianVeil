using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public RouteData routeData { get; set; }
    //private List<Node> waypointVectors = new List<Node>();

    //Debug Get Stuff
    //public void SetWayPoints()
    //{
    //    GetComponent<WaypointInteractable>().SetWaypoints(routeData.Nodes);
    //}

    //Sets waypoints when path entered
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"{transform.name}: Route {other.name}");

        //Ignore interactors
        if (other.name == "Pusher" || other.name == "Direct Interactor") return;
        //GetComponent<CapsuleCollider>().enabled = true;
        routeData = RadialPuzzleController.getRouteData(other.name);
        //Debug.Log(routeData.Waypoints);

        //if (waypointVectors.Count > 0)
        //{
        //    waypointVectors.Clear();
        //}

        if (other.name == "PathA_2" || other.name == "PathB_2")
        {
            //Debug.Log("Reversing Array");
            //Array.Reverse(routeData.Waypoints);
            Array.Reverse(routeData.Nodes);
        }

        ////Create waypoint nodes
        //for (int i = 0; i < routeData.Waypoints.Length; i++)
        //{
        //    Node node = new Node();
        //    node.index = i;
        //    node.position = routeData.Waypoints[i];
        //    waypointVectors.Add(node);
        //}
        Debug.Log($"TriggerEntered: {other.name}. Setting nodes: {routeData.Nodes}");
        GetComponent<WaypointInteractable>().SetWaypoints(routeData.Nodes);
        //ToggleColliders(false);
    }

    public void ToggleColliders(bool isOn)
    {
        routeData.StartTransform.GetComponent<SphereCollider>().enabled = isOn;
        routeData.EndTransform.GetComponent<SphereCollider>().enabled = isOn;
    }
}