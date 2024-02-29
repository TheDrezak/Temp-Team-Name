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
        if (gameManager.instance.money > gameManager.instance.healthPrice)
        {
            gameManager.instance.playerScript.setHP(gameManager.instance.playerScript.HPOrig);
            gameManager.instance.decreasMoney(gameManager.instance.healthPrice);
            gameManager.instance.playerScript.updatePlayerUI();
        }
    }

    public void buySpeed()
    {
        if (gameManager.instance.money > gameManager.instance.speedPrice && gameManager.instance.playerScript.speedStacks < 6)
        {
            gameManager.instance.playerScript.setSpeed(gameManager.instance.playerScript.getSpeed() * 1.1f);
            gameManager.instance.playerScript.speedStacks++;
            gameManager.instance.decreasMoney(gameManager.instance.speedPrice);

        }

    }

    public void buyJumpMax()
    {
        if (gameManager.instance.money > gameManager.instance.jumpMaxPrice && gameManager.instance.playerScript.jumpMax < 4)
        {
            gameManager.instance.playerScript.setJumpMax(gameManager.instance.playerScript.getJumps() + 1);
            gameManager.instance.decreasMoney(gameManager.instance.jumpMaxPrice);
        }
    }

    public void buyJumpForce()
    {
        if (gameManager.instance.money > gameManager.instance.jumpForcePrice && gameManager.instance.playerScript.jumpForceStacks < 4)
        {
            gameManager.instance.playerScript.setjumpForce(gameManager.instance.playerScript.getJumpForce() * 1.25f);
            gameManager.instance.playerScript.jumpForceStacks++;
            gameManager.instance.decreasMoney(gameManager.instance.jumpForcePrice);
        }
    }

    public void buyDmg()
    {
        if (gameManager.instance.money > gameManager.instance.dmgPrice)
        {
            gameManager.instance.playerScript.setDmgMult(gameManager.instance.playerScript.getDmgMult() + .25f);
            gameManager.instance.playerScript.dmgStacks++;
            gameManager.instance.decreasMoney(gameManager.instance.dmgPrice);
        }
    }

    public void buyGrenade()
    {
        if (gameManager.instance.money > gameManager.instance.grenadePrice)
        {

        }
    }

    public void buyCartHealth()
    {
        if (gameManager.instance.money > gameManager.instance.cartHealthPrice)
        {
            gameManager.instance.payloadScript.HP = gameManager.instance.payloadScript.HPOrig;
            gameManager.instance.decreasMoney(gameManager.instance.cartHealthPrice);
            gameManager.instance.payloadScript.updateUI();
        }
    }

    IEnumerator maxStacksMessage()
    {

        yield return new WaitForSeconds(2);
    }
}