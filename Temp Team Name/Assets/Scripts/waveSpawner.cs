using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveSpawner : MonoBehaviour
{
    [SerializeField] float spawnRate = 1f;
    [SerializeField] GameObject[] enemies;
    [SerializeField] bool canSpawn = true;

    void Start()
    {
        StartCoroutine(spawner());
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
