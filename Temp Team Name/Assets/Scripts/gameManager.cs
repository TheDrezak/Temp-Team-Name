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
    [SerializeField] public GameObject itemUI;

    // For multiplying score when player doesn't take damage
    [SerializeField] int scoremultiplierMin;
    [SerializeField] int scoremultiplierMax;
    int currentScoremultiplier;
    [SerializeField] float multiplierTimer;
    [SerializeField] int increasemultiplierBy;
    float timer = 0.0f;

    public Image playerHPBar;
    public GameObject playerDmgFlash;
    public GameObject playerSpawnPos;

    public GameObject playerTele;

    public GameObject player;
    public playerController playerScript;

    public bool isPaused;

    public int keysCollected;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        playerSpawnPos = GameObject.FindWithTag("PlayerRespawnPoint");
        playerTele = GameObject.FindWithTag("GrappleTel");

        currentScoremultiplier = scoremultiplierMin;
        multiplier.text = currentScoremultiplier.ToString("F0");
        updateUI();
        StartCoroutine(timeLoop());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePaused();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            statePaused();
            menuActive = menuShop;
            menuShop.SetActive(isPaused);
        }
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
        timer += 1;
        if ((timer % multiplierTimer) == 0)
        {
            increasemultiplier();
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

    public void updateGameGoal(int amount)
    {
        playerScript.setScore(amount * currentScoremultiplier);
        updateUI();
    }

    public void updateUI()
    {
        ScoreText.text = playerScript.getScore().ToString("F0");
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
