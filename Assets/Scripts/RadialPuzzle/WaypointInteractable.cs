using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
[RequireComponent(typeof(PhysicsMover))]
public class WaypointInteractable : MonoBehaviour
{
    //Fields
    public XRBaseInteractable _interactable;
    public PhysicsMover _mover;
    private Node[] _nodes;
    private Node[] _targets;
    private Vector3 _initialObjectPos;
    private Vector3 _grabPos;
    bool _targetFound;
    Color _rayColor;

    //Properties
    public Node CurrentNode { get; set; }
    public Node TargetNode { get; set; }

    private void Awake()
    {
        _interactable = gameObject.GetComponent<XRSimpleInteractable>();
        _mover = gameObject.GetComponent<PhysicsMover>();
    }
    private void OnEnable()
    {
        _interactable.selectEntered.AddListener(HandleCheck);
    }
    private void OnDisable()
    {
        _interactable.selectEntered.RemoveListener(HandleCheck);
    }

    #region InteractableTrackingMethods
    private void HandleCheck(SelectEnterEventArgs args)
    {
        _grabPos = args.interactorObject.transform.position;
        StartCoroutine(TrackHandPos());

    }
    private IEnumerator TrackHandPos()
    {
        Debug.Log("Object Grabbed");

        while (_interactable.isSelected)
        {
            // Visualize the pull direction using Debug.DrawLine
            VisualisePullDir();
            UpdatePos();
            yield return null;
        }
        Debug.Log("Let go of object");
    }
    private void VisualisePullDir()
    {
        Vector3 handPos = _interactable.interactorsSelecting[0].transform.position;
        // Remove the y component
        handPos.y = 0;

        Vector3 pullDirection = (handPos - _grabPos).normalized;
        //Debug.Log(_grabPos);
        pullDirection.y = 0; // Ensure y component is 0

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
    #endregion

    #region Movement Methods
    private void UpdatePos()
    {
        // Find the target if it hasn't been found yet.
        CheckForTarget();

        if (!IsNodeOccupied())
        {
            MoveAlongTargetRoute();
        }
    }
    private void CheckForTarget()
    {
        Vector3 handPos = _interactable.interactorsSelecting[0].transform.position;
        Vector3 pullDirection = (handPos - _grabPos).normalized;

        for (int i = 0; i < _targets.Length; i++)
        {
            Vector3 directionToWaypoint = (_targets[i].position - transform.position).normalized;
            float waypointDot = Vector3.Dot(pullDirection, directionToWaypoint);

            if (waypointDot > 0.95f)
            {
                TargetNode = _targets[i];
                _targetFound = true;
                Debug.DrawLine(transform.position, _targets[i].position, _rayColor);
            }
        }
    }
    private void MoveAlongTargetRoute()
    {
        if (!_targetFound) return;
        //Debug.Log($"Moving from {CurrentNode.index} to {TargetNode.index}");
        // Step 1: Calculate Direction
        Vector3 directionToTarget = TargetNode.position - _initialObjectPos;

        // Step 2: Determine Movement Amount
        Vector3 handPos = _interactable.interactorsSelecting[0].transform.position;
        Vector3 pullDir = (handPos - _grabPos);

        float length = directionToTarget.magnitude;
        directionToTarget.Normalize();

        var posNormalised = Vector3.Dot(pullDir, directionToTarget) / length;
        var newPos = Vector3.Lerp(_initialObjectPos, TargetNode.position, posNormalised);

        if (Vector3.Distance(transform.position, TargetNode.position) < 0.05f)
        {
            //Debug.Log("MoveAlongTargetRoute - Target Reached");
            transform.position = TargetNode.position;
            MovementCompleted();
            return;
        }

        _mover.MoveTo(newPos);

        //Debug.Log(currTarget + "-" + initialObjectPos + "=" + directionToTarget + "\n" + " length:" + length);
    }
    private void MovementCompleted()
    {
        //Set prev node isOccupied to false
        CurrentNode.isOccupied = false;

        CurrentNode = TargetNode;
        //Set new current node to true
        CurrentNode.isOccupied = true;

        _initialObjectPos = CurrentNode.position;
        _targets = GetPossibleTargets();
        _targetFound = false;

        //regrab object TODO: find more effective method to reset grabPos
        var xrmanager = _interactable.interactionManager;
        var interactor = _interactable.interactorsSelecting[0];
        xrmanager.SelectExit(_interactable.interactorsSelecting[0], _interactable);
        xrmanager.SelectEnter(interactor, _interactable);
        //_grabPos = _interactable.interactorsSelecting[0].transform.position;
        //Debug.Log($"{gameObject.name} Movement completed, Now at node {CurrentNode.index}");
    }
    private bool IsNodeOccupied()
    {
        bool isOccupied = RadialPuzzleController.checkIfBlocked(this);
        //if (isOccupied) { 
        //    Debug.Log("Node Occupied"); 
        //}
        return isOccupied;
    }
    #endregion

    #region Waypoint/Target Methods
    public void SetWaypoints(Node[] nodes)
    {
        //clear target if exists
        TargetNode = null;
        _targetFound = false;

        //Set new waypoint array
        _nodes = new Node[nodes.Length];
        Array.Copy(nodes, this._nodes, nodes.Length);

        //Set target and curr position
        _targets = GetPossibleTargets();
        _initialObjectPos = CurrentNode.position;
        CurrentNode.isOccupied = true;
        //Debug.Log(this.nodes);
    }
    private Node[] GetPossibleTargets()
    {
        List<Node> tmpList = new List<Node>();
        for (int i = 0; i < CurrentNode.adjacentNodes.Count; i++)
        {
            tmpList.Add(_nodes[CurrentNode.adjacentNodes[i]]);
            //Debug.Log($"CurrentNode: {CurrentNode.index} AdjacentNode: {_nodes[CurrentNode.adjacentNodes[i]].index}");
        }

        return tmpList.ToArray();

    }
    #endregion

    private void OnDrawGizmos()
    {
        Color nodeColor;
        Color lineColor = Color.cyan;
        if (_nodes != null)
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i].index == 0)
                {
                    nodeColor = Color.green;
                }
                else if (_nodes[i].index == _nodes.Length - 1)
                {
                    nodeColor = Color.red;
                }
                else
                {
                    nodeColor = Color.white;
                }
                Gizmos.color = nodeColor;
                Gizmos.DrawWireSphere(_nodes[i].position, 0.025f);
            }
            Gizmos.color = lineColor;
            for (int i = 0; i < _nodes.Length - 1; i++)
            {
                Gizmos.DrawLine(_nodes[i].position, _nodes[i + 1].position);
            }
        }

        if (TargetNode != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(TargetNode.position, 0.05f);
        }
    }
}

