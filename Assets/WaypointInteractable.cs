using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WaypointInteractable : MonoBehaviour
{
    private Node[] nodes;
    Node[] targets;
    public Node TargetNode { get; set; }
    public Node CurrentNode { get; set; }

    Vector3 currTarget;
    private Vector3 initialObjectPos;

    private Vector3 grabPos;
    private int currentNodeIndex = 0;
    private int newNodeIndex = 0;
    Color rayColor;

    [Header("Interaction")]
    public XRBaseInteractable interactable;
    public PhysicsMover mover;

    private void Awake()
    {
        interactable = gameObject.AddComponent<XRSimpleInteractable>();
        mover = gameObject.AddComponent<PhysicsMover>();
    }
    private void OnEnable()
    {
        interactable.selectEntered.AddListener(HandleCheck);
        //interactable.selectExited.AddListener(GoToTarget);
    }
    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(HandleCheck);
        //interactable.selectExited.RemoveListener(GoToTarget);
    }

    private void HandleCheck(SelectEnterEventArgs args)
    {
        grabPos = args.interactorObject.transform.position;
        StartCoroutine(TrackHandPos());

    }
    private IEnumerator TrackHandPos()
    {
        while (interactable.isSelected)
        {
            // Visualize the pull direction using Debug.DrawLine
            VisualisePullDir();
            UpdatePos();
            yield return null;
        }
    }
    private void VisualisePullDir()
    {
        Vector3 handPos = interactable.interactorsSelecting[0].transform.position;
        Vector3 pullDirection = (handPos - grabPos).normalized;

        // Calculate dot products for each direction
        float dotForward = Vector3.Dot(transform.forward, pullDirection);
        float dotRight = Vector3.Dot(transform.right, pullDirection);

        // Use the up vector as a proxy for 'behind' since we're in 3D space.
        // If you're in 2D space or want to consider the real 'behind' vector, use -transform.forward

        rayColor = Color.blue; // Default

        // Determine which direction has the highest alignment with pullDirection
        if (dotForward > 0.5f) // Adjust thresholds as necessary
        {
            rayColor = Color.green; // Forward
        }
        else if (dotForward < -0.5f)
        {
            rayColor = Color.red; // Behind
        }
        else if (dotRight > 0.5f)
        {
            rayColor = Color.yellow; // Right
        }
        else if (dotRight < -0.5f)
        {
            rayColor = Color.magenta; // Left
        }


        Debug.DrawRay(transform.position, pullDirection, rayColor);
    }
    private void UpdatePos()
    {
        // Find the target if it hasn't been found yet.
        CheckForTarget();
        MoveAlongTargetRoute();
    }
    private void CheckForTarget()
    {
        Vector3 handPos = interactable.interactorsSelecting[0].transform.position;
        Vector3 pullDirection = (handPos - grabPos).normalized;

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 directionToWaypoint = (targets[i].position - transform.position).normalized;
            float waypointDot = Vector3.Dot(pullDirection, directionToWaypoint);

            if (waypointDot > 0.95f)
            {
                TargetNode = targets[i];
                //if (IsNodeOccupied())
                //{
                //    Debug.Log("Piece already here");
                //    return;
                //}
                currTarget = targets[i].position;
                newNodeIndex = targets[i].index;
                Debug.DrawLine(transform.position, targets[i].position, rayColor);
                //Debug.Log("Target:" + targets[i].index);
                //Debug.Log("Target Pos:" + targets[i].position);

            }
        }
    }
    private bool IsNodeOccupied()
    {
        return RadialPuzzleController.checkIfBlocked(transform);
    }

    void MoveAlongTargetRoute()
    {
        // Step 1: Calculate Direction
        Vector3 directionToTarget = currTarget - initialObjectPos;

        // Step 2: Determine Movement Amount
        Vector3 handPos = interactable.interactorsSelecting[0].transform.position;
        Vector3 pullDir = (handPos - grabPos);

        float length = directionToTarget.magnitude;

        directionToTarget.Normalize();

        var posNormalised = Vector3.Dot(pullDir, directionToTarget) / length;

        var newPos = Vector3.Lerp(initialObjectPos, currTarget, posNormalised);

        if (Vector3.Distance(transform.position, currTarget) < 0.01f)
        {
            currentNodeIndex = newNodeIndex;
            CurrentNode = nodes[currentNodeIndex];
            MovementCompleted();
            return;
        }

        if (newNodeIndex != currentNodeIndex)
            mover.MoveTo(newPos);
        //Debug.Log(currTarget + "-" + initialObjectPos + "=" + directionToTarget + "\n" + " length:" + length);
    }
    void MovementCompleted()
    {
        //if (newNodeIndex == 0 || newNodeIndex == nodes.Length - 1)
        //{
        //    Debug.Log("EndOfPath");
        //    GetComponent<PuzzlePiece>().ToggleColliders(true);
        //}
        //else
        //{
        //    initialObjectPos = nodes[currentNodeIndex].position;
        //    targets = GetPossibleTargets();
        //}
        initialObjectPos = nodes[currentNodeIndex].position;
        targets = GetPossibleTargets();
        grabPos = interactable.interactorsSelecting[0].transform.position;

        Debug.Log($"{gameObject.name} Movement completed, Now at node {currentNodeIndex}");
    }

    #region Waypoint Methods
    public void SetWaypoints(Node[] nodes)
    {
        this.nodes = new Node[nodes.Length];
        Array.Copy(nodes, this.nodes, nodes.Length);

        currentNodeIndex = nodes[currentNodeIndex].index;
        CurrentNode = nodes[currentNodeIndex];

        targets = GetPossibleTargets();
        initialObjectPos = nodes[currentNodeIndex].position;
        //Debug.Log(this.nodes);
    }
    private Node[] GetPossibleTargets()
    {
        //Debug.Log("Getting Targets");
        List<Node> tmpList = new List<Node>();

        if (currentNodeIndex == 0)
        {
            if (nodes.Length > 1) // Check to ensure there's at least two nodes
                tmpList.Add(nodes[1]);
        }
        else if (currentNodeIndex == nodes.Length - 1)
        {
            if (nodes.Length > 1) // Check to ensure there's at least two nodes
                tmpList.Add(nodes[currentNodeIndex - 1]);
        }
        else if (currentNodeIndex > 0 && currentNodeIndex < nodes.Length - 1)
        {
            tmpList.Add(nodes[currentNodeIndex - 1]);
            tmpList.Add(nodes[currentNodeIndex + 1]);
        }
        //Debug.Log(nodes[0]);
        //Debug.Log(tmpList[0]);
        return tmpList.ToArray();
    }
    #endregion

    private void OnDrawGizmos()
    {
        Color nodeColor;
        Color lineColor = Color.cyan;
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].index == 0)
                {
                    nodeColor = Color.green;
                }
                else if (nodes[i].index == nodes.Length - 1)
                {
                    nodeColor = Color.red;
                }
                else
                {
                    nodeColor = Color.white;
                }
                Gizmos.color = nodeColor;
                Gizmos.DrawWireSphere(nodes[i].position, 0.025f);
            }
            Gizmos.color = lineColor;
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
            }
        }

        if (currTarget != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(currTarget, 0.025f);
        }
    }
}

