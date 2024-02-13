using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrapple
{

    void pullPlayer(int speed)
    {
        playerController controller = gameManager.instance.player.GetComponent<playerController>();

        controller.enabled = false;



    }

    void pullToPlayer(int speed)
    {


    }
}