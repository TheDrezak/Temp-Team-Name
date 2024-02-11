using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    // Initilaize controller
    [SerializeField] CharacterController controller;

    // Player stats
    [SerializeField] int HP;
    [SerializeField] float playerSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;

    Vector3 move;
    Vector3 playerVel;
    int jumpCount;


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
}
