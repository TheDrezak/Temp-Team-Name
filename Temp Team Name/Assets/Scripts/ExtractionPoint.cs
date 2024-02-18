using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExtractionPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IInteract ep = other.GetComponent<IInteract>();

        if (ep != null)
        {
            gameManager.instance.youWin();
        }
    }
}
