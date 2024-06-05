using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust this value to control movement speed

    void Update()
    {
        // Get input from WASD keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float depthInput = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            depthInput = -1f; // Negative value for Q key (move along negative Z-axis)
        }
        else if (Input.GetKey(KeyCode.E))
        {
            depthInput = 1f; // Positive value for E key (move along positive Z-axis)
        }


        // Calculate movement direction
        Vector3 movement = new Vector3(horizontalInput, verticalInput, depthInput).normalized;

        // If there is input, move the player
        if (movement.magnitude >= 0.1f)
        {
            // Use Lerp to smoothly move the player
            Vector3 targetPosition = transform.position + movement * moveSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
        }
    }
}
