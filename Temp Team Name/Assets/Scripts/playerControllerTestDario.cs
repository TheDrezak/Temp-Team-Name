using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControllerTestDario : MonoBehaviour, IDamage
{
    // Initilaize controller
    [SerializeField] CharacterController controller;
    [SerializeField] private GameObject grapple;
    [SerializeField] private Transform shootPos;

    // Player stats
    [SerializeField] int HP;
    [SerializeField] float playerSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;

    [SerializeField] private float grappleCooldown;

    Vector3 move;
    Vector3 playerVel;
    int jumpCount;

    private bool isGrappling;


    void Start()
    {

    }


    void Update()
    {
        movement();
        if (Input.GetButton("Shoot") && !isGrappling)
        {
            StartCoroutine(shoot());
        }
    }

    // Function that moves player & allows double jump
    void movement()
    {
        if (controller.isGrounded)
        {
            // Reset jump count
            jumpCount = 0;
            // Reset gravity
            playerVel = Vector3.zero;
        }


        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(move * playerSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpForce;
            jumpCount++;
        }
        playerVel.y += gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);
    }

    public void takeDamage(int amount)
    {
        // Lose if HP hits 0
        HP -= amount;
        if (HP < 0)
        {

        }
    }

    IEnumerator shoot()
    {
        // Begin shooting
        isGrappling = true;

        // Fire at player
        Instantiate(grapple, shootPos.position, Camera.main.transform.rotation);
        yield return new WaitForSeconds(grappleCooldown);

        // End shooting
        isGrappling = false;
    }
}
