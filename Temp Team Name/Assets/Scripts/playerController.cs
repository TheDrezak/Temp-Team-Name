using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    // Initilaize controller
    [SerializeField] CharacterController controller;

    // Player stats
    [SerializeField] int HP;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;

    Vector3 move;
    Vector3 playerVel;
    int jumpCount;
    float playerSpeed;


    void Start()
    {
        
    }


    void Update()
    {
        movement();
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
        
        // Gather movement inputs
        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        // Check for sprint or crouch. Set speed based on players state
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Set speed to sprint speed
            playerSpeed = sprintSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            // Set speed to crouch speed (doesn't crouch yet- only sneaks)
            playerSpeed = crouchSpeed;
        }
        else
        {
            // Set speed to base speed
            playerSpeed = walkSpeed;
        }

        // Move character
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
}
