using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    public Transform pickupDestination; // The destination where the picked up object should be placed.
    private Rigidbody objectRigidbody; // The rigidbody component of the object.
    private Vector3 smoothVelocity; // Velocity used for smoothing the movement.

    private void Start()
    {
        objectRigidbody = GetComponent<Rigidbody>(); // Get the rigidbody component of the object.
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectRigidbody.isKinematic)
            {
                DropObject();
            }
            else
            {
                PickUpObject();
            }
        }
    }

    private void PickUpObject()
    {
        objectRigidbody.isKinematic = true; // Disable gravity and physics simulation.
        objectRigidbody.velocity = Vector3.zero; // Reset the object's velocity.

        // Set the object's position to the pickup destination.
        transform.position = pickupDestination.position;

        // Parent the object to the pickup destination.
        transform.SetParent(pickupDestination);

        // Disable the object's gravity.
        objectRigidbody.useGravity = false;
    }

    private void DropObject()
    {
        // Unparent the object from the pickup destination.
        transform.SetParent(null);

        // Enable the object's gravity.
        objectRigidbody.useGravity = true;

        // Enable physics simulation.
        objectRigidbody.isKinematic = false;
    }

    private void LateUpdate()
    {
        // Smoothly move the object along with the player's camera.
        if (transform.parent == pickupDestination)
        {
            Vector3 targetPosition = pickupDestination.position;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref smoothVelocity, 0.1f);
        }
    }
}
