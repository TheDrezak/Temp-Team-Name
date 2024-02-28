using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] int speed;
    [SerializeField] float yVel;
    [SerializeField] int destroyTime;

    [SerializeField] GameObject explosion;

    void Start()
    {
        rb.velocity = (transform.forward + (Vector3.up * yVel)) * speed;
        StartCoroutine(destroyObject());
    }

    IEnumerator destroyObject()
    {
        yield return new WaitForSeconds(destroyTime);
        // Makes sure not to send a null ref
        if (explosion) { Instantiate(explosion, transform.position, transform.rotation); }
        Destroy(gameObject);
    }
}
