using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    [SerializeField] int damageAmount;


    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Make sure triggers don't trigger triggers
        if (other.isTrigger) return;


        IDamage dmg = other.GetComponent<IDamage>();
        IPhysics push = other.GetComponent<IPhysics>();

        if (dmg != null)
        {
            dmg.TakeDamage(damageAmount);
        }
        if (push != null)
        {
            push.physicsDir((other.transform.position - transform.position).normalized * damageAmount * 25);
        }
    }
}
