using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class GrabVisualization : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = -1;
    public float moveThreshold = 0.1f;  // Adjust this value to your preference

    private XRSimpleInteractable simpleInteractable;
    private Vector3 initialGrabPosition;
    private Vector3 initialObjectPosition;
    private bool isGrabbed = false;

    private void Awake()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        simpleInteractable.selectEntered.AddListener(OnGrab);
        simpleInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        simpleInteractable.selectEntered.RemoveListener(OnGrab);
        simpleInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        initialGrabPosition = args.interactorObject.transform.position;
        initialObjectPosition = args.interactableObject.transform.position;
        //Debug.Log($"Initial Grab Position: {initialGrabPosition}, Initial Object Position: {initialObjectPosition}");

    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    private void Update()
    {
        if (isGrabbed && currentWaypointIndex + 1 < waypoints.Length)
        {
            Debug.Log("Grabbing");
            Vector3 pullDirection = simpleInteractable.interactorsSelecting[0].transform.position - initialGrabPosition;
            Vector3 directionToNextWaypoint = waypoints[currentWaypointIndex + 1].position - initialObjectPosition;
            Vector3 directionToPrevWaypoint = (currentWaypointIndex - 1 >= 0) ? initialObjectPosition - waypoints[currentWaypointIndex - 1].position : Vector3.zero;

            float dotForward = Vector3.Dot(pullDirection.normalized, directionToNextWaypoint.normalized);
            float dotBackward = Vector3.Dot(pullDirection.normalized, directionToPrevWaypoint.normalized);

            if (dotForward > dotBackward)
            {
                Debug.DrawRay(transform.position, directionToNextWaypoint, Color.green); // Visualize direction to the next waypoint
            }
            else
            {
                Debug.DrawRay(transform.position, -directionToPrevWaypoint, Color.red); // Visualize direction to the previous waypoint
            }

            Debug.DrawRay(transform.position, pullDirection, Color.blue); // Visualize pull direction
            //Debug.Log($"DotForward: {dotForward} Dotbackward: {dotBackward} PullDir: {pullDirection}");

            // At the first waypoint, only check forward movement
            if (currentWaypointIndex == 0 && dotForward > 0.9f)
            {
                // ... move forward code ...
                float distanceToNextWaypoint = Vector3.Distance(initialObjectPosition, waypoints[currentWaypointIndex + 1].position);
                float movedDistance = Mathf.Min(Vector3.Distance(initialGrabPosition, simpleInteractable.interactorsSelecting[0].transform.position), distanceToNextWaypoint);

                transform.position = initialObjectPosition + directionToNextWaypoint.normalized * movedDistance;

                if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex + 1].position) < 0.01f)
                {
                    currentWaypointIndex++;
                    initialObjectPosition = transform.position;
                    initialGrabPosition = simpleInteractable.interactorsSelecting[0].transform.position;
                    Debug.Log("Moving Forward");
                }
            }
            // At the last waypoint, only check backward movement
            else if (currentWaypointIndex == waypoints.Length - 1 && dotBackward > 0.9f)
            {
                // ... move backward code ...
                float distanceToPrevWaypoint = Vector3.Distance(initialObjectPosition, waypoints[currentWaypointIndex - 1].position);
                float movedDistance = Mathf.Min(Vector3.Distance(initialGrabPosition, simpleInteractable.interactorsSelecting[0].transform.position), distanceToPrevWaypoint);

                transform.position = initialObjectPosition - directionToPrevWaypoint.normalized * movedDistance;

                if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex - 1].position) < 0.01f)
                {
                    currentWaypointIndex--;
                    initialObjectPosition = transform.position;
                    initialGrabPosition = simpleInteractable.interactorsSelecting[0].transform.position;
                    Debug.Log("Moving Backward");
                }
            }

            // For waypoints in between
            else
            {
                // Move towards the next waypoint
                if (dotForward > dotBackward && dotForward > 0.9f)
                {
                    float distanceToNextWaypoint = Vector3.Distance(initialObjectPosition, waypoints[currentWaypointIndex + 1].position);
                    float movedDistance = Mathf.Min(Vector3.Distance(initialGrabPosition, simpleInteractable.interactorsSelecting[0].transform.position), distanceToNextWaypoint);

                    transform.position = initialObjectPosition + directionToNextWaypoint.normalized * movedDistance;

                    if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex + 1].position) < 0.01f)
                    {
                        currentWaypointIndex++;
                        initialObjectPosition = transform.position;
                        initialGrabPosition = simpleInteractable.interactorsSelecting[0].transform.position;
                        Debug.Log("Moving Forward");
                    }
                }
                // Move towards the previous waypoint
                else if (dotBackward > dotForward && dotBackward > 0.9f)
                {
                    float distanceToPrevWaypoint = Vector3.Distance(initialObjectPosition, waypoints[currentWaypointIndex - 1].position);
                    float movedDistance = Mathf.Min(Vector3.Distance(initialGrabPosition, simpleInteractable.interactorsSelecting[0].transform.position), distanceToPrevWaypoint);

                    transform.position = initialObjectPosition - directionToPrevWaypoint.normalized * movedDistance;

                    if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex - 1].position) < 0.01f)
                    {
                        currentWaypointIndex--;
                        initialObjectPosition = transform.position;
                        initialGrabPosition = simpleInteractable.interactorsSelecting[0].transform.position;
                        Debug.Log("Moving Backward");
                    }
                }
            }
        }

    }
    private void OnDrawGizmos()
    {
        foreach (var item in waypoints)
        {
            Gizmos.DrawWireSphere(item.position, 0.1f);
        }
    }
}
