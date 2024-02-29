using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveSpawner : MonoBehaviour
{
    [SerializeField] float spawnRate = 1f;
    [SerializeField] GameObject[] enemies;
    [SerializeField] bool canSpawn = true;
    public bool payloadMoving;

    void Start()
    {

    }
    void Update()
    {
        payloadMoving = gameManager.instance.payloadScript.isMoving;
        // Once Payload moves stop spawning & don't allow it
        if (payloadMoving)
        {
            //canSpawn = false;
            StopCoroutine(spawner());
        }
    }

    IEnumerator spawner()
    {
        // Set time between enemy spawns
        WaitForSeconds wait = new WaitForSeconds(spawnRate);
        // Check if it can spawn and that payload stoped
        while (canSpawn && !payloadMoving)
        {
            yield return wait;
            spawn();
        }
    }

    void spawn()
    {
        // Choose random enemy
        int rand = Random.Range(0, enemies.Length);
        GameObject enemyToSpawn = enemies[rand];
        // Create enemy
        Instantiate(enemyToSpawn, transform.position, transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Payload") || other.CompareTag("Player"))
        {
            canSpawn = true;
        }
    }
}
