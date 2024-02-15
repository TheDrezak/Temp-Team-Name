using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grappleShot : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;

    [SerializeField] private int speed;
    [SerializeField] private float pullSpeed;
    [SerializeField] private int destroyTime;

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
            
            gameManager.instance.playerScript.PullObject(other.transform);
        }

        Destroy(gameObject);
    }



}
