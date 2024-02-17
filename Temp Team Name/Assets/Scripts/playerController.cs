using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{

    [SerializeField] private int itemsInInventory;

    [SerializeField] private int HP;
    [SerializeField] CharacterController controller;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpMax;
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] private int grabDist;
    [SerializeField] float shootRate;

    [SerializeField] private Transform shootPos;

    [SerializeField] private GameObject grappleShot;
    [SerializeField] private float grappleCooldown;

    Vector3 move;
    Vector3 playerVelocity;
    int jumpCount;
    private List<GameObject> inventory = new List<GameObject>();

    bool isShooting;
    private bool isGrappling;
    private int HPOrig;

    void Start()
    {
        HPOrig = HP;
        respawn();
    }

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.blue);
        Movement();
        itemsInInventory = inventory.Count;

        if (itemsInInventory >= 1)
        {
            gameManager.instance.itemUI.SetActive(true);
        }
        else
        {
            gameManager.instance.itemUI.SetActive(false);
        }

        if (Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(Shoot());
        }
        if (Input.GetButton("ShootGrap") && !isGrappling)
        {
            StartCoroutine(shootGrapple());
        }
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            pickitemUp();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            dropItem(inventory[0]);
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

    void updatePlayerUI()
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

    IEnumerator shootGrapple()
    {

        isGrappling = true;

        Instantiate(grappleShot, shootPos.position, Camera.main.transform.rotation);
        yield return new WaitForSeconds(grappleCooldown);
        isGrappling = false;
    }

    public void PullObject(Transform pos)
    {
        controller.enabled = false;
        transform.position = pos.position;
        controller.enabled = true;
    }

    void pickitemUp()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, grabDist))
        {
            Debug.Log(hit.collider.name);

            IInteract interact = hit.collider.GetComponent<IInteract>();

            if (hit.transform != transform && interact != null)
            {
                GameObject clonedItem = Instantiate(hit.collider.gameObject);
                clonedItem.SetActive(false);
                inventory.Add(clonedItem);
                gameManager.instance.keysCollected += 1;
                gameManager.instance.updateGameGoal(0);
                Destroy(hit.collider.gameObject);
            }
        }
    }

    void dropItem(GameObject item)
    {
        for (int i = 0; i < itemsInInventory; i++)
        {
            if (inventory[i] == item)
            {
                item.SetActive(true);
                item.transform.position = gameManager.instance.player.transform.position;
                 
                inventory.RemoveAt(0);

            }
        }
    }
}
