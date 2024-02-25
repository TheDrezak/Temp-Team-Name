using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpaused();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpaused();
    }

    public void quit()
    {
        Application.Quit();
    }

    public void respawn()
    {

        gameManager.instance.playerScript.respawn();
    }

    public void buyHealth()
    {
        gameManager.instance.playerScript.setHP(gameManager.instance.playerScript.HPOrig);
        gameManager.instance.playerScript.updatePlayerUI();
    }

    public void buySpeed()
    {
        gameManager.instance.playerScript.setSpeed(gameManager.instance.playerScript.getSpeed() * 1.15f);
    }

    public void buyJumpMax()
    {
        gameManager.instance.playerScript.setJumpMax(gameManager.instance.playerScript.getJumps() + 1);
    }

    public void buyJumpForce()
    {
        gameManager.instance.playerScript.setjumpForce(gameManager.instance.playerScript.getJumpForce() * 1.25f);
    }
}