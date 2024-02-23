using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Componenets -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [SerializeField] private int HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpMax;
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;
    [SerializeField] float sprintMod;
    bool isSprinting;

    [Header("----- Guns -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;

    [SerializeField] private Transform shootPos;

    Vector3 move;
    Vector3 playerVelocity;
    int jumpCount;

    bool isShooting;
    private bool isGrappling;
    public int HPOrig;

    void Start()
    {
        HPOrig = HP;
        respawn();
    }

    void Update()
    {
        sprint();
        // Can't shoot if paused
        if (!gameManager.instance.isPaused)
        {
            Movement();

            // Add if statement to make sure there is a gun in gun list once added
            if (Input.GetButton("Shoot") && !isShooting)
            {
                StartCoroutine(Shoot());
            }
        } 
    }
    void Movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }
        move = Input.GetAxis("Horizontal") * transform.right
             + Input.GetAxis("Vertical") * transform.forward;

        controller.Move(move * playerSpeed * Time.deltaTime);
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVelocity.y = jumpForce;
            jumpCount++;
        }
        playerVelocity.y += gravity * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    void sprint()
    {
        // Multiply if sprinting
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= sprintMod;
            // Allows for stamina system to be created
            isSprinting = true;
        }
        // Divie to normal speed if not sprinting
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed /= sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (hit.transform != transform && dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        updatePlayerUI();
        StartCoroutine(flashDmg());

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator flashDmg()
    {

        gameManager.instance.playerDmgFlash.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        gameManager.instance.playerDmgFlash.SetActive(false);
    }

    public void respawn()
    {
        HP = HPOrig;
        updatePlayerUI();

        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    public void PullObject(Transform pos)
    {
        controller.enabled = false;
        transform.position = pos.position;
        controller.enabled = true;
    }

    //void pickitemUp()
    //{
    //    RaycastHit hit;

    //    if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, grabDist))
    //    {
    //        Debug.Log(hit.collider.name);

    //        IInteract interact = hit.collider.GetComponent<IInteract>();

    //        if (hit.transform != transform && interact != null)
    //        {
    //            GameObject clonedItem = Instantiate(hit.collider.gameObject);
    //            clonedItem.SetActive(false);
    //            gameManager.instance.keysCollected += 1;
    //            gameManager.instance.updateGameGoal(0);
    //            Destroy(hit.collider.gameObject);
    //        }
    //    }
    //}
    public void setHP(int amount)
    {
        HP = amount;
    }

    public int getHP() { return HP; }
}
