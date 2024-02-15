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
    [SerializeField] private TMP_Text enemyCountText;
    public Image playerHPBar;
    public GameObject playerDmgFlash;
    public GameObject playerSpawnPos;

    public GameObject player;
<<<<<<< Updated upstream
    public playerController playerScript;

    // Number of enemies
    int enemyCount;

    // Captures timeScale for consistency
    float timeScale;
=======
    public PlayerController playerScript;

    public bool isPaused;
>>>>>>> Stashed changes

    int enemyCount;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();

        playerSpawnPos = GameObject.FindWithTag("PlayerRespawnPoint");
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

        Time.timeScale = timeScale;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

<<<<<<< Updated upstream
    // Update game goal
    public void updateGameGoal (int amount)
    {
        // Add or subtract enemy
        enemyCount += amount;

        // Update UI of count

        if (enemyCount <= 0)
        {
            // Player wins
            menuActive = menuWin;
            menuActive.SetActive(true);
            statePaused();
        }
        
=======
    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");

        if (enemyCount <= 0)
        {
            //you win!!
            menuActive = menuWin;
            menuActive.SetActive(true);
            statePaused();

        }
    }

    public void youLose()
    {
        statePaused();

        menuActive = menuLose;
        menuActive.SetActive(true);
>>>>>>> Stashed changes
    }
}
