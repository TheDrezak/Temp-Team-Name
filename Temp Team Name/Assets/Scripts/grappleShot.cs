using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grappleShot : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;

    [SerializeField] private int speed;
    [SerializeField] private float pullSpeed;
    [SerializeField] private int destroyTime;


    private PlayerController pc = gameManager.instance.playerScript;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed;

        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IGrapple grp = other.GetComponent<IGrapple>();

        if (grp != null)
        {

            rb.velocity = Vector3.zero;

            Vector3 force = CalculateForceTowardsGrapple(gameManager.instance.player.GetComponent<Rigidbody>(), transform.position,
                pullSpeed);

            pc.enabled = false;

            pullObject(gameManager.instance.player.GetComponent<Rigidbody>(), force);
        }

        Destroy(gameObject);
    }

    void pullObject(Rigidbody rb, Vector3 force)
    {

        if (rb != null)
        {
            rb.AddForce(force);
        }

    }

    Vector3 CalculateForceTowardsGrapple(Rigidbody playerRb, Vector3 grapplePoint, float desiredSpeed)
    {
        Vector3 direction = (grapplePoint - playerRb.position).normalized; // Direction from player to grapple
        float forceMagnitude = desiredSpeed * playerRb.mass; // Assuming you want to reach the desired speed instantly
        return direction * forceMagnitude;
    }
}
