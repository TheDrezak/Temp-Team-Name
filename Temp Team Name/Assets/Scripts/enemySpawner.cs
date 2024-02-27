using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    public enum SpawnState { Spawning, Waiting, Counting};

    [SerializeField] Transform[] spawners;
    [SerializeField] float spawnInterval;
    [SerializeField] List<waves> waves;
    [SerializeField] float timeBetweenWaves;
    [SerializeField] float waveCountdown;
    SpawnState state = SpawnState.Counting;
    int currentWave;
    int deadCount;
    [SerializeField] List<enemyParent> enemyList;

    void Start()
    {
        waveCountdown = timeBetweenWaves;
        // Resets waves
        currentWave = 0;
    }

    private void Update()
    {
        if (state == SpawnState.Waiting)
        {
            // Check if enemies are dead or timer is still going
            if (deadCount < enemyList.Count)
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
            if (state != SpawnState.Spawning)
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
        // Reset dead count
        deadCount = 0;

        // Reset waveCountdown
        state = SpawnState.Counting;
        waveCountdown = timeBetweenWaves;

        // Give feedback to player they completed wave **ADD IN**

       

        // Checks if all waves are done
        if (currentWave +1 > waves.Count - 1)
        {
            // They won! Trigger event **ADD IN** Should move cart
        }
        else
        {
            // Increase wave count
            currentWave++;
        }
    }

    IEnumerator SpawnWave(waves wave)
    {
        state = SpawnState.Spawning;

        // Spawn enemies based on amount passed in
        for(int i = 0; i < wave.enemyAmount; i++)
        {
            // Spawn
            SpawnEnemy(wave.enemy);

            // Adds a delay so enemies aren't spawning on top of each other
            yield return new WaitForSeconds(wave.spawnDelay);
        }
        
        state = SpawnState.Waiting;

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

    public void isDead()
    {
        deadCount++;
    }
}
