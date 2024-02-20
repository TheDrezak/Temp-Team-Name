using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    public enum SpaawnState { Spawning, Waiting, Counting};

    [SerializeField] Transform[] spawners;
    [SerializeField] float spawnInterval;
    [SerializeField] waves[] waves;
    [SerializeField] float timeBetweenWaves;
    [SerializeField] float waveCountdown;
    SpaawnState state = SpaawnState.Counting;
    int currentWave;
    [SerializeField] List<enemyParent> enemyList;

    void Start()
    {
        waveCountdown = timeBetweenWaves;
        // Resets waves
        currentWave = 0;
    }

    private void Update()
    {
        if (state == SpaawnState.Waiting)
        {
            // Check if enemies are dead or timer is still going
            if (enemyList.Count > 0)
            {
                return;
            }
            else
            {
                CompleteWave();
            }
        }
        if(waveCountdown <= 0)
        {
            // Prevents spawning twice
            if (state != SpaawnState.Spawning)
            {
                // Start wave spawn
                StartCoroutine(SpawnWave(waves[currentWave]));
            }
        }
        else
        {
            // Count down
            waveCountdown -= Time.deltaTime;
        }
    }

    void CompleteWave()
    {
        // Reset waveCountdown
        state = SpaawnState.Counting;
        waveCountdown = timeBetweenWaves;

        // Give feedback to player they completed wave **ADD IN**


        // Checks if all waves are done
        if (currentWave +1 > waves.Length - 1)
        {
           // Do something once they're all completed **ADD IN**
        }
        else
        {
            // Increase wave count
            currentWave++;
        }
    }

    IEnumerator SpawnWave(waves wave)
    {
        state = SpaawnState.Spawning;

        // Spawn enemies based on amount passed in
        for(int i = 0; i < wave.enemyAmount; i++)
        {
            // Spawn
            SpawnEnemy(wave.enemy);

            // Adds a delay so enemies aren't spawning on top of each other
            yield return new WaitForSeconds(wave.spawnDelay);
        }
        
        state = SpaawnState.Waiting;

        yield break;
    }

    void SpawnEnemy(GameObject enemy)
    {
        // Gets random number up to max spawners
        int randomInt = Random.Range(1, spawners.Length);

        // Spawns new enemy at random position
        GameObject newEnemy = Instantiate(enemy, spawners[randomInt].position, spawners[randomInt].rotation);

        // Get stats from parent
        enemyParent newEnemyStats = newEnemy.GetComponent<enemyParent>();

        // Add enemy to list
        enemyList.Add(newEnemyStats);
    }
}
