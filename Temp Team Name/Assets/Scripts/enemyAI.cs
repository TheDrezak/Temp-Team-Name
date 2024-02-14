using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI : MonoBehaviour, IDamage
{
    // Initialize info
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    Color color;
    SphereCollider detectionCollider;

    // Enemy stats
    [SerializeField] int HP;
    [SerializeField] int viewCone;
    [SerializeField] int targetFacespeed;
    [SerializeField] Transform[] wayPoints;
    [SerializeField] float roamSpeed;
    [SerializeField] Image HPBar;
    int maxHP;
    int wayPointIndex;

    // Enemy weapon variables
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] int enemyStopDist;
    [SerializeField] int bulletsPerShot;
    [SerializeField] float spreadFactor;
    bool isShooting;

    // Detection & movement variables
    bool playerInRange;
    float angleToPlayer;
    Vector3 playerDir;
    [SerializeField] float detectionRadius;


    void Start()
    {
        // Set HP
        maxHP = HP;
        // Capture model color for red flash
        color = model.material.color;
        // Set waypoint index
        wayPointIndex = 0;
        // Update UI
        updateUI();
        // Set Enemy Detection Radius
        detectionCollider = GetComponent<SphereCollider>();
        detectionCollider.radius = detectionRadius;
        // Update win con
        gameManager.instance.updateGameGoal(1);
    }

    void Update()
    {
        // Checks if player is in range
        if (playerInRange && canSeePlayer())
        {
            agent.stoppingDistance = enemyStopDist;
        }
        else
        {
            agent.stoppingDistance = 0;
            roam();
        } 
    }

    void roam()
    {
        // Checks X & Z for enemy to verify if it's hit current waypoint
        if (agent.transform.position.x != wayPoints[wayPointIndex].position.x && agent.transform.position.z != wayPoints[wayPointIndex].position.z)
        {
            // Moves enemy to waypoint
            agent.SetDestination(wayPoints[wayPointIndex].position);
        }
        // Cycles through waypoints
        else { wayPointIndex = (wayPointIndex + 1) % wayPoints.Length; }
    }

    bool canSeePlayer()
    {
        // Finds where player is
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        // Debug
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

    // If player exits range set to false
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

        // Checks how many bullets shoult fire
        if (bulletsPerShot > 1)
        {
            //Fires all pellets in spread
            for (int i = 0; i < bulletsPerShot; i++)
            {
                // Create a rotation for spread
                Quaternion bulletRot = transform.rotation;
                // Randomize spread
                bulletRot.x += Random.Range(-spreadFactor, spreadFactor);
                bulletRot.y += Random.Range(-spreadFactor, spreadFactor);
                // Fire pellet in random direction
                Instantiate(bullet, shootPos.position, bulletRot);
            }  
        }
        else
        {
            // Fires single bullet at player
            Instantiate(bullet, shootPos.position, transform.rotation);
        }
        yield return new WaitForSeconds(shootRate);

        // End shooting
        isShooting = false; 
    }

    IEnumerator flashMat()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = color;
    }

    public void takeDamage(int amount)
    {
        // Take damage
        HP -= amount;

        // Flash red
        StartCoroutine(flashMat());

        // Go to player's last position
        agent.SetDestination(gameManager.instance.player.transform.position);

        // Check if HP hit 0
        if (HP <= 0)
        {
            //Update game goal and destroy object
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }

        // Update HP bar
        updateUI();
    }

    void updateUI()
    {
        // Updates health bar
        HPBar.fillAmount = (float)HP / maxHP;
    }
}
