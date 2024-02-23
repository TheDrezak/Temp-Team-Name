using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using TMPro;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [SerializeField] private GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] private GameObject menuWin;
    [SerializeField] private GameObject menuLose;
    [SerializeField] private GameObject menuShop;
    [SerializeField] private TMP_Text enemyCountText;
    [SerializeField] private TMP_Text keyCountText;
    [SerializeField] public GameObject itemUI;

    public Image playerHPBar;
    public GameObject playerDmgFlash;
    public GameObject playerSpawnPos;

    public GameObject playerTele;

    public GameObject player;
    public playerController playerScript;

    public bool isPaused;

    public int keysCollected;
    int enemyCount;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        playerSpawnPos = GameObject.FindWithTag("PlayerRespawnPoint");
        playerTele = GameObject.FindWithTag("GrappleTel");
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
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");
        //keyCountText.text = keysCollected.ToString("F0");

        //if (enemyCount <= 0)
        //{
        //    youWin();

        //}
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
