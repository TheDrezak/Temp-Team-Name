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

        gameManager.instance.playerScript.setScore(gameManager.instance.playerScript.getScore() - gameManager.instance.healthPrice);
        gameManager.instance.updateUI();
    }

    public void buySpeed()
    {
        gameManager.instance.playerScript.setSpeed(gameManager.instance.playerScript.getSpeed() * 1.15f);
        gameManager.instance.playerScript.setScore(gameManager.instance.playerScript.getScore() - gameManager.instance.speedPrice);
        gameManager.instance.updateUI();
    }

    public void buyJumpMax()
    {
        gameManager.instance.playerScript.setJumpMax(gameManager.instance.playerScript.getJumps() + 1);
        gameManager.instance.playerScript.setScore(gameManager.instance.playerScript.getScore() - gameManager.instance.jumpMaxPrice);
        gameManager.instance.updateUI();
    }

    public void buyJumpForce()
    {
        gameManager.instance.playerScript.setjumpForce(gameManager.instance.playerScript.getJumpForce() * 1.25f);
        gameManager.instance.playerScript.setScore(gameManager.instance.playerScript.getScore() - gameManager.instance.jumpForcePrice);
        gameManager.instance.updateUI();
    }
}