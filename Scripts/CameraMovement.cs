
using UnityEngine;
 public class CameraMovement : MonoBehaviour{

    // public Transform target; // Reference to the object the camera will follow
    // public Vector3 distances = new Vector3(1.92f, 1.72f, -0.85f); // Distance between the camera and the target along each axis
    public Transform targetObject;
    public  Vector3 offset = new Vector3(-1.5f, 1.16f, -0.16f); // Desired offset from the target
    private Vector3 cameraPosition;
    // void Update()
    // {
    //     if (target == null)
    //     {
    //         Debug.LogWarning("Target object is not assigned in the inspector.");
    //         return;
    //     }

    //     // Calculate the desired position for the camera
    //     Vector3 desiredPosition = target.position -
    //                               target.right * distances.x +
    //                               target.up * distances.y -
    //                               target.forward * distances.z;

    //     // Set the position of the camera to the desired position
    //     transform.position = desiredPosition;

    //     // Make the camera look at the target
    //     transform.LookAt(target);
    // }

    void Start(){
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f); // Set rotation to 90 degrees on Y-axis

    }
    void FixedUpdate()
    {
        // transform.position = targetObject.position ;//+ offset;

    }

 }
    
        

