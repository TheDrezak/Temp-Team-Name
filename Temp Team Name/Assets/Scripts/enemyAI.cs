using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    // Initialize info
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    // Enemy stats
    [SerializeField] int HP;
    [SerializeField] int viewCone;
    [SerializeField] int targetFacespeed;

    // Enemy weapon variables
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    bool isShooting;

    // Detection & movement variables
    bool playerInRange;
    float angleToPlayer;
    Vector3 playerDir;

    void Start()
    {
        
    }

    void Update()
    {
        // Checks if player is in range
        if (playerInRange && canSeePlayer())
        {

        }
    }

    bool canSeePlayer()
    {
        // Finds where player is
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        // Debug
        // Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDir);

        // Raycast check for what enemy sees
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            // Check if we hit both the player & the player is within our vision cone
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                // Chase player & shoot
                agent.SetDestination(gameManager.instance.player.transform.position);
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }

                // Allows unit to turn if in range
                if (agent.remainingDistance < agent.stoppingDistance) { faceTarget(); }

                return true;
            }
        }
        return false;
    }

    void faceTarget()
    {
        // Rotate to player if they're within stopping range
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFacespeed);
    }

    // If player enters range set to true
   void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // IF player exits range set to false
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    IEnumerator shoot()
    {
        // Begin shooting
        isShooting = true;

        // Fire at player
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);

        // End shooting
        isShooting = false; 
    }

    IEnumerator flashMat()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    public void takeDamage(int amount)
    {
        // Take damage
        HP -= amount;

        // Flash red
        StartCoroutine(flashMat());

        // Check if HP hit 0
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
