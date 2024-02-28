using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [SerializeField] private GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] private GameObject menuWin;
    [SerializeField] private GameObject menuLose;
    [SerializeField] private GameObject menuShop;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text multiplier;
    [SerializeField] private TMP_Text keyCountText;
    [SerializeField] private TMP_Text ammoCountCurr;
    [SerializeField] private TMP_Text ammoCountTotal;
    [SerializeField] public GameObject itemUI;
    [SerializeField] public int money;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip purchaseSound;
    [SerializeField] AudioSource backgroundAud;
    [Range(0f, 1f)][SerializeField] float purchaseVol;

    // For multiplying score when player doesn't take damage
    [SerializeField] int scoremultiplierMin;
    [SerializeField] int scoremultiplierMax;
    int currentScoremultiplier;
    [SerializeField] float multiplierTimer;
    [SerializeField] int increasemultiplierBy;
    float timer = 0.0f;

    [Header("----- Shop Prices -----")]

    [SerializeField] public int healthPrice;
    [SerializeField] public int speedPrice;
    [SerializeField] public int jumpMaxPrice;
    [SerializeField] public int jumpForcePrice;
    [SerializeField] public int dmgPrice;

    public Image playerHPBar;
    public Image CartHPbar;
    public Image progressBar;
    public Image reloadcirc;
    public GameObject reloadUI;
    public GameObject playerDmgFlash;
    public GameObject playerSpawnPos;
    public GameObject playerTele;
    public GameObject player;
    public playerController playerScript;


    public GameObject payload;
    public Payload payloadScript;

    // Checkpoint Variables
    public float checkpointTimer = 0.0f;
    public float checkpointCountdown;
    public bool checkpointReached;

    public bool isPaused;
    public bool lastCheck;

    public int keysCollected;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPos = GameObject.FindWithTag("PlayerRespawnPoint");

        payload = GameObject.FindWithTag("Payload");
        payloadScript = payload.GetComponent<Payload>();

        currentScoremultiplier = scoremultiplierMin;
        multiplier.text = currentScoremultiplier.ToString("F0");
        updateUI();
        checkpointReached = false;
        StartCoroutine(timeLoop());
    }

    // Update is called once per frame
    void Update()
    {

        updateProgressBar();
        if (Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePaused();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }

        if (Input.GetKeyDown(KeyCode.J) && playerScript.canOpenShop == true)
        {
            statePaused();
            menuActive = menuShop;
            menuShop.SetActive(isPaused);
        }
    }

    public void updateBulletCount()
    {
        ammoCountCurr.text = playerScript.ammoCurr.ToString();
        ammoCountTotal.text = playerScript.ammoMax.ToString();
    }

    // Loops to keep counting every one second
    IEnumerator timeLoop()
    {
        while (true)
        {
            timeCount();
            yield return new WaitForSeconds(1);
        }
    }

    // Increases timer every one second
    void timeCount()
    {
        // Timer for multiplier
        timer += 1;
        if ((timer % multiplierTimer) == 0)
        {
            increasemultiplier();
        }
        
        // Timer for payload
        if (checkpointReached)
        {
            checkpointTimer += 1;
        } 
    }

    void increasemultiplier()
    {
        // Ensure enough time has passed and score isn't at max yet
        if (currentScoremultiplier < scoremultiplierMax)
        {
            // Increment score, update UI, capture new time
            currentScoremultiplier += 1;
            multiplier.text = currentScoremultiplier.ToString("F0");
        }
    }

    public void resetmultiplier()
    {
        // Resets multiplier
        currentScoremultiplier = scoremultiplierMin;
        // Updates text
        multiplier.text = currentScoremultiplier.ToString("F0");
        // Resets timer
        timer = 0;
    }

    public void updateProgressBar()
    {
        progressBar.fillAmount = (float)payloadScript.checkPointsHit / payloadScript.waypoints.Count;
    }
   
    public void statePaused()
    {
        isPaused = !isPaused;

        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpaused()
    {
        isPaused = !isPaused;

        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    // Adds enemy amounts
    public void increaseMoney(int amount)
    {
        money += (amount * currentScoremultiplier);
        updateUI();
    }

    // For cash spending
    public void decreasMoney(int amount)
    {
        money -= amount;
        aud.PlayOneShot(purchaseSound, purchaseVol);
        updateUI();
    }

    public void updateUI()
    {
        ScoreText.text = money.ToString("F0");
    }

    public void youLose()
    {
        statePaused();

        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void youWin()
    {
        menuActive = menuWin;
        menuActive.SetActive(true);
        statePaused();
    }
}
