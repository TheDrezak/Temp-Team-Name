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
    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IextractionPoint ep = other.GetComponent<IextractionPoint>();

        if (ep != null)
        {
            gameManager.instance.youWin();
        }
    }
}
