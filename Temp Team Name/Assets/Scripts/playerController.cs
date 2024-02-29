using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class playerController : MonoBehaviour, IDamage, IPhysics
{
    [Header("----- Componenets -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;

    [Header("----- Player Stats -----")]
    [SerializeField] private int HP;
    [SerializeField] float playerSpeed;
    [SerializeField] public int jumpMax;
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;
    [SerializeField] float sprintMod;
    [SerializeField] private int score;
    [SerializeField] private float dmgMult;
    [SerializeField] int pushBackResolve;
    int selectedGun;

    [Header("----- Guns -----")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] public int ammoCurr;
    [SerializeField] public int ammoMax;
    [SerializeField] public float reloadTime;

    [SerializeField] private Transform shootPos;

    [Header("----- Grenades -----")] 
    [SerializeField] private GameObject grenade;
    [SerializeField] int grenadeCooldown;
    [SerializeField] public int grenadeAmmount;

    [Header("Audio")]
    [SerializeField] AudioClip[] hurtSounds;
    [Range(0f, 1f)][SerializeField] float hurtVol;
    [SerializeField] AudioClip[] stepSounds;
    [Range(0f, 1f)][SerializeField] float stepVol;
    [SerializeField] AudioClip jumpSound;
    [Range(0f, 1f)][SerializeField] float jumpVol;
    [SerializeField] List<AudioClip> shootSounds;
    [Range(0f, 1f)][SerializeField] float shootVol;
    [SerializeField] AudioClip reloadSound;
    [Range(0f, 1f)][SerializeField] float reloadVol;
    [SerializeField] AudioClip switchSound;
    [Range(0f, 1f)][SerializeField] float switchVol;
    [SerializeField] AudioClip deathSound;
    [Range(0f, 1f)][SerializeField] float deathVol;

    public int speedStacks;
    public int jumpForceStacks;
    public int dmgStacks;
    

    Vector3 move;
    Vector3 playerVelocity;
    int jumpCount;
    Vector3 pushBack;

    bool isShooting;
    bool isPlayingSteps;
    //bool isJumping;
    private bool isReloading;
    private bool grenadeReloading;
    private bool hasGun;
    public int HPOrig;
    public bool canOpenShop;
    public bool canThrow;
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

            if(gunList.Count > 0)
            {
                selectGun();
            }

            // Add if statement to make sure there is a gun in gun list once added
            if (Input.GetButton("Shoot") && !isShooting && !isReloading && hasGun && ammoCurr > 0)
            {
                StartCoroutine(Shoot());
            }
            else if (Input.GetButton("Shoot") && ammoCurr <= 0 && !isReloading && !isShooting && hasGun)
            {
                StartCoroutine(reload());
            }
            if(Input.GetButtonDown("Fire2") && grenadeAmmount > 0)
            {
                Debug.Log("grenade out!");
                StartCoroutine(throwG());
            }
            if (Input.GetKeyDown(KeyCode.R) && !isReloading)
            {
                StartCoroutine(reload());
            }
        } 
    }
    IEnumerator throwG()
    {
        canThrow = false;
        Instantiate(grenade, shootPos.position, transform.rotation);
        grenadeAmmount--;
        gameManager.instance.updateUI();   
        yield return new WaitForSeconds(grenadeCooldown);
        canThrow = true;
    }
    void Movement()
    {
        // Handles our physics
        pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushBackResolve);

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
            aud.PlayOneShot(jumpSound, jumpVol);
        }
        playerVelocity.y += gravity * Time.deltaTime;

        controller.Move((playerVelocity + pushBack) * Time.deltaTime);
        if (!isPlayingSteps && move.normalized.magnitude > 0.1f && controller.isGrounded)
            StartCoroutine(playSteps());
    }
    IEnumerator playSteps()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(stepSounds[UnityEngine.Random.Range(0, stepSounds.Length)], stepVol);
        //I multed the waittime by the player speed since we manipulate the playerspeed in sprint
        yield return new WaitForSeconds(.5f * playerSpeed / 10);
        isPlayingSteps = false;
    }
    void sprint()
    {
        // Multiply if sprinting
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= sprintMod;
        }
        // Divie to normal speed if not sprinting
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed /= sprintMod;
        }
    }

    public void physicsDir(Vector3 dir)
    {
        pushBack += dir;
    }

    IEnumerator Shoot()
    {
        aud.PlayOneShot(shootSounds[selectedGun], shootVol);
        isShooting = true;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (hit.transform != transform && dmg != null)
            {
                // Multi's dmg by multiplier
                float playerDmg = (float)shootDamage * dmgMult;
                dmg.TakeDamage((int)playerDmg);
            }
        }

        gunList[selectedGun].ammoCur--;
        ammoCurr = gunList[selectedGun].ammoCur;
        gameManager.instance.updateBulletCount();
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator reload()
    {
        isReloading = true;
        aud.PlayOneShot(reloadSound, reloadVol);
        gunList[selectedGun].ammoCur = gunList[selectedGun].ammoMax;
        ammoCurr = gunList[selectedGun].ammoCur;
        gameManager.instance.reloadUI.SetActive(true);

        int temp = ((int)reloadTime * 100);

        for (int i = 0; i < (reloadTime*100); i++)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            temp--;
            gameManager.instance.reloadcirc.fillAmount = temp / (reloadTime * 100) ;
        }
        gameManager.instance.reloadUI.SetActive(false);

        gameManager.instance.updateBulletCount();
        isReloading = false;
    }


    public void TakeDamage(int amount)
    {
        HP -= amount;
        aud.PlayOneShot(hurtSounds[UnityEngine.Random.Range(0, hurtSounds.Length)], hurtVol);
        updatePlayerUI();
        StartCoroutine(flashDmg());
        gameManager.instance.resetmultiplier();

        if (HP <= 0)
        {
            aud.PlayOneShot(deathSound, deathVol);
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
        pushBack = Vector3.zero;

        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    public void getGunstats(gunStats gun)
    {
        gunList.Add(gun);
        hasGun = true;

        gun.ammoCur = gun.ammoMax;

        shootDamage = gun.shootDamage;
        shootDistance = gun.shootDist;
        shootRate = gun.shootRate;
        ammoMax = gun.ammoMax;
        ammoCurr = gun.ammoCur;
        reloadTime = gun.reloadTime;
        shootVol = gun.shootSoundVol;
        shootSounds.Add(gun.shootSound);

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;

        selectedGun = gunList.Count - 1;
        gameManager.instance.updateBulletCount();
    }  
    
    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            changeGun();

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            changeGun();

        }
    }

    void changeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootDistance = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;
        shootVol = gunList[selectedGun].shootSoundVol;
        ammoCurr = gunList[selectedGun].ammoCur;
        ammoMax = gunList[selectedGun].ammoMax;
        reloadTime = gunList[selectedGun].reloadTime;
        aud.PlayOneShot(switchSound, switchVol);

        gameManager.instance.updateBulletCount();

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
    }

      

    public void PullObject(Transform pos)
    {
        controller.enabled = false;
        transform.position = pos.position;
        controller.enabled = true;
    }

    public int getHP() { return HP; }

    public float getSpeed() { return playerSpeed; }

    public int getJumps() { return jumpMax; }

    public float getJumpForce() { return jumpForce; }

    public int getScore() { return score; }
    
    public float getDmgMult() { return dmgMult; }


    public void setSpeed(float amount)
    {
        playerSpeed = amount;
    }

    public void setHP(int amount)
    {
        HP = amount;
    }

    public void setJumpMax(int amount)
    {
        jumpMax = amount;
    }

    public void setjumpForce(float amount)
    {
        jumpForce = amount;
    }

    public void setScore(int amount)
    {
        score = amount;
    }

    public void setDmgMult(float amount)
    {
        dmgMult = amount;
    }

}
