using UnityEngine;

public class SphereMovement : MonoBehaviour
{
    public float moveForce = 50.0f; // Force to apply for movement
    public float drag = 25.0f; // Drag to control movement resistance
    public Transform target; // The target object to move towards
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = drag; // Set the drag
    }

    private void Update()
    {
        if (target != null)
        {
            // Calculate the direction towards the target
            Vector3 moveDirection = target.position - transform.position;

            // Apply force to move the sphere towards the target
            rb.AddForce(moveDirection * moveForce);
        }
    }
}
