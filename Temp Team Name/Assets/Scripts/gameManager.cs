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
    
    [Header("Audio")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip purchaseSound;
    [SerializeField] AudioSource backgroundAud;
    [Range(0f, 1f)][SerializeField] float purchaseVol;
    [SerializeField] AudioClip backgroundTune;
    [SerializeField] AudioClip victory;
    [SerializeField] AudioClip defeat;

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
    [SerializeField] public int grenadePrice;
    [SerializeField] public int cartHealthPrice;

    [Header("-----UI-----")] 
    public Image MaxStacksMessage;

    public TMP_Text speedStacks;
    public TMP_Text jumpStacks;
    public TMP_Text jumpForceStacks;
    public TMP_Text dmgStacks;
    public TMP_Text grenadeStacks;
    public Image playerHPBar;
    public Image CartHPbar;
    public Image progressBar;
    public Image reloadcirc;
    public GameObject MaxStacksUI;
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
        backgroundAud.Play();
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            statePaused();
            menuActive = menuShop;
            menuShop.SetActive(isPaused);
        }
    }

    public IEnumerator maxStacksUIPrompt()
    {

        MaxStacksUI.SetActive(true);
        yield return new WaitForSeconds(1);
        MaxStacksUI.SetActive(false);
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
        jumpForceStacks.text = playerScript.jumpForceStacks.ToString();
        speedStacks.text = playerScript.speedStacks.ToString();
        dmgStacks.text = playerScript.dmgStacks.ToString();
        jumpStacks.text = playerScript.jumpMax.ToString();
        grenadeStacks.text = playerScript.grenadeAmmount.ToString();
    }

    public void youLose()
    {
        
        statePaused();
        aud.PlayOneShot(defeat, purchaseVol);
        menuActive = menuLose;
        menuActive.SetActive(true);
        backgroundAud.Stop();
    }

    public void youWin()
    {
        menuActive = menuWin;
        menuActive.SetActive(true);
        statePaused();
        aud.PlayOneShot(victory, purchaseVol);
        backgroundAud.Stop();
    }
}
