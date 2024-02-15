using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grapplePoint : MonoBehaviour, IGrapple
{

    [SerializeField] private Rigidbody rb;


    private void OnTriggerEnter(Collider other)
    {
        return;
    }
}
