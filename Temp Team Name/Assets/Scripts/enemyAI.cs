using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class enemyAI : MonoBehaviour, IDamage, IPhysics
{
    [Header("----- Componenets -----")]
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] AudioSource aud;

    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int viewCone;
    [SerializeField] int shootCone;
    [SerializeField] int targetFaceSpeed;
    [SerializeField] int animSpeedTrans;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDis;
    [SerializeField] int pointsGiven;
    [SerializeField] int physicsResolve;

    [Header("----- Guns -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] int pelletAmount;
    [SerializeField] float spread;

    [Header("----- UI-----")]
    [SerializeField] Image HPBar;

    [Header("Audio")]
    [SerializeField] AudioClip[] soundsSteps;
    [Range(0, 1)][SerializeField] float soundStepVol;
    [SerializeField] AudioClip soundsShoot;
    [Range(0, 1)][SerializeField] float soundShootVol;
    [SerializeField] AudioClip hurtSound;
    [Range(0, 1)][SerializeField] float hurtVol;
    [SerializeField] AudioClip deathSound;
    [Range(0, 1)][SerializeField] float deathVol;

    // Picks target
    [SerializeField] bool targetsPayload;
    string targetChoice;

    bool isShooting;
    bool targetInRange;
    bool isPlayingSteps;

    // Player dest info
    float angleToPlayer;
    Vector3 playerDir;

    // Payload dest info
    float angleToPayload;
    Vector3 payloadDir;

    int HPOrig;
    Color color;
    Vector3 startingPos;
    bool destChosen;
    float stoppingDistanceOrig;
    


    void Start()
    {
        // Initialize 
        startingPos = transform.position;
        HPOrig = HP;
        updateUI();
        color = model.material.color;
        stoppingDistanceOrig = agent.stoppingDistance;

        // Find target
        chooseTargert();
    }

    void Update()
    {
        // Capture velocity normalized to lerp animations as needed
        float animSpeed = agent.velocity.normalized.magnitude;

        // Lerp animations
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans));

        // Checks if player is in range
        if (!targetInRange)
        {
            // Roam because player isn't in range
            StartCoroutine(roam());
        }
        else
        {
            if (!targetsPayload)
            {
                canSeePlayer();
            }
            else
            {
                canSeePayload();
            }
        }
    }

    // Chooses to shoot player or payload
    void chooseTargert()
    {
        if (!targetsPayload)
        {
            targetChoice = "Player";
        }
        else
        {
            targetChoice = "Payload";
        }
    }

    public void physicsDir(Vector3 dir)
    {
        agent.velocity += dir;
    }

    IEnumerator roam()
    {
        if (!isPlayingSteps && agent.remainingDistance > 0.01f)
            StartCoroutine(playFootSteps());
        // Make sure reamining distance is very small, or on point, & destChosen is false
        if (agent.remainingDistance < 0.05f && !destChosen)
        {
            // Chooses destination, updates stopping distance to allow roam, pause time
            destChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamPauseTime);

            // Randomizes roam points
            Vector3 randomPos = Random.insideUnitSphere * roamDis;
            // Connects back to starting pos
            randomPos += startingPos;

            // Roams enemy to random position on the layer selected (1 for this case)
            // Makes sure the point hits inside the NavMesh
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDis, 1);
            agent.SetDestination(hit.position);

            destChosen = false;
        }
    }

    void canSeePlayer()
    {
        // Finds where player is
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        // Check if Raycast hits player or something else
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            // Did we hit both the player & the player is in the cone
            if (hit.collider.CompareTag(targetChoice))
            {
                // Moves to player
                agent.SetDestination(gameManager.instance.player.transform.position);
                // Starts shooting if not already shooting & in cone of gun
                if (!isShooting && angleToPlayer <= shootCone)
                {
                    StartCoroutine(shoot());
                }

                if (agent.remainingDistance < agent.stoppingDistance) { faceTarget(); }

                // Reset stopping distance to original number
                agent.stoppingDistance = stoppingDistanceOrig;
            }
        }
    }

    void canSeePayload()
    {
        // Finds where payload is
        payloadDir = gameManager.instance.payload.transform.position - headPos.position;
        angleToPayload = Vector3.Angle(new Vector3(payloadDir.x, 0, payloadDir.z), transform.forward);

        // Check if Raycast hits the payload or something else
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, payloadDir, out hit))
        {
            // Did we hit both the player & the player is in the cone
            if (hit.collider.CompareTag(targetChoice))
            {
                // Moves to payload
                agent.SetDestination(gameManager.instance.payload.transform.position);
                // Starts shooting if not already shooting & in cone of gun
                if (!isShooting && angleToPayload <= shootCone)
                {
                    StartCoroutine(shoot());
                }

                if (agent.remainingDistance < agent.stoppingDistance) { faceTarget(); }

                // Reset stopping distance to original number
                agent.stoppingDistance = stoppingDistanceOrig;
            }
        }
    }

    void faceTarget()
    {
        // Rotates to player if they're within stopping range
        // Makes rot ignore player's Y pos

        if (!targetsPayload)
        {
            // For Player
            Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
        }
        else
        {
            // For Payload
            Quaternion rot = Quaternion.LookRotation(new Vector3(payloadDir.x, transform.position.y, payloadDir.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetChoice))
        {
            targetInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetChoice))
        {
            targetInRange = false;
            agent.stoppingDistance = 0;
        }
    }

    public void TakeDamage(int amount)
    {
        // Play damage animation
        anim.SetTrigger("Damage");

        //Plays damage audio
        aud.PlayOneShot(hurtSound, hurtVol);
        // Move to player if enemy takes damage
        agent.SetDestination(gameManager.instance.player.transform.position);

        // Take damage
        HP -= amount;

        // Flash red
        StartCoroutine(flashMat());
        if (HP <= 0)
        {
            // Give points
            gameManager.instance.increaseMoney(pointsGiven);
            //Plays death sound
             
            aud.PlayOneShot(deathSound, deathVol);
            
            Destroy(gameObject);
        }
        // Lower HP on HP bar
        updateUI();
    }
    
    IEnumerator flashMat()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = color;
    }

    IEnumerator shoot()
    {
        isShooting = true;

        // Triggers shoot animation
        anim.SetTrigger("Shoot");
        aud.PlayOneShot(soundsShoot, soundShootVol);
        // Check if shotgun
        if (bullet.GetComponent<bulletClass>().shotgun)
        {
            for (int i = 0; i < pelletAmount; i++)
            {
                // Create a rotation for spread
                Quaternion bulletRot = transform.rotation;

                // Randomize spread
                bulletRot.x += Random.Range(-spread, spread);
                bulletRot.y += Random.Range(-spread, spread);

                // Fire pellet in random direction
                Instantiate(bullet, shootPos.position, bulletRot);
            }
            
        }
        else
        {
            // Create single bullet and fire
            Instantiate(bullet, shootPos.position, transform.rotation);
        }
        
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void updateUI()
    {
        // Updates HP bar
        HPBar.fillAmount = (float)HP / HPOrig;
    }
    IEnumerator playFootSteps() 
    {
        isPlayingSteps = true;
        aud.PlayOneShot(soundsSteps[0], soundStepVol);
        yield return new WaitForSeconds(.5f);
        isPlayingSteps = false;
    }
}
