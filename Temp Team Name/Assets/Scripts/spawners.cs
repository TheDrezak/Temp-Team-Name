using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawners : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int spawnTimer;
    [SerializeField] Transform[] spawnPos;

    int spawnCount;
    bool isSpawning;
    bool startSpawning;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning && !isSpawning && spawnCount < numToSpawn )
        {
            StartCoroutine(spawn());
        }
    }

    IEnumerator spawn()
    {
        isSpawning = true;

        // Picks random spawn location
        int arrayPos = Random.Range(0, spawnPos.Length);
        // Creates object
        Instantiate(objectToSpawn, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        // Increase count
        spawnCount++;
        yield return new WaitForSeconds(spawnTimer);
        isSpawning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Payload") || other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }
}
