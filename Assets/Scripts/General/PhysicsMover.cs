using UnityEngine;

public class PhysicsMover : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();    
    }
    public void MoveTo(Vector3 newPos)
    {
        rb.MovePosition(newPos);
    }
}