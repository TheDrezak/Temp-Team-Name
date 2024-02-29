using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveSpawner : MonoBehaviour
{
    [SerializeField] float spawnRate = 1f;
    [SerializeField] GameObject[] enemies;
    [SerializeField] bool canSpawn;

    void Start()
    {
        canSpawn = false;
        
    }

    IEnumerator spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate);
        while (canSpawn)
        {
            yield return wait;
            spawn();
        }
    }

    void spawn()
    {
        int rand = Random.Range(0, enemies.Length);
        GameObject enemyToSpawn = enemies[rand];

        Instantiate(enemyToSpawn, transform.position, transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Payload") || other.CompareTag("Player"))
        {
            canSpawn = true;
            StartCoroutine(spawner());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Payload") || other.CompareTag("Player"))
        {
            canSpawn = false;
            StopCoroutine(spawner());
        }
    }
}
