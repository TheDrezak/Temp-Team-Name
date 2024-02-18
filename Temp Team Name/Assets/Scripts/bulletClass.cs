using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletClass : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    // Settings for bullets
    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    void Start()
    {
        // Set speed
        rb.velocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Make sure triggers don't trigger triggers
        if (other.isTrigger) return;

        // Deal designated damage amount
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.TakeDamage(damageAmount);
        }

        // Destroy object
        Destroy(gameObject);
    }
}
