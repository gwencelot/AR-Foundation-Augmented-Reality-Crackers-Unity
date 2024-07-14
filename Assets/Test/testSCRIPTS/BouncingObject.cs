using UnityEngine;

public class BouncingObject : MonoBehaviour
{
    public float bounceForce = 10f;  // Force applied to bounce
    public float groundCheckDistance = 1.1f;  // Distance to check for the ground
    public LayerMask groundLayer;  // LayerMask to specify the ground

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if the object is on the ground
        if (IsGrounded())
        {
            // Apply an upward force to make the object bounce
            rb.velocity = new Vector3(rb.velocity.x, bounceForce, rb.velocity.z);
        }
    }

    bool IsGrounded()
    {
        // Check if there is ground within groundCheckDistance below the object
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}
