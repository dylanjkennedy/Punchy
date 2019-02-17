using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    bool inWave = false;
    float timeSinceLastWaveEnd = 0f;
    int totalEnemiesSpawned = 0;
    int waveCount = 0;
    [SerializeField] int totalEnemiesInWave = 3;
    [SerializeField] int timeBetweenWaves = 5; // seconds
    [SerializeField] SpawnManager spawnManager;
    [SerializeField] DifficultyManager difficultyManager;
    


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        // logic for checking the time since the wave ended and setting inWave = true
        // and reset that counter

        if (inWave)  
        {
            totalEnemiesSpawned = spawnManager.TotalEnemiesSpawned;
            if (totalEnemiesSpawned >= totalEnemiesInWave)
            {
                WaveIsStagnant();
            }
            else
            {
                InWave();
            }
        }
        else
        {
            if (timeSinceLastWaveEnd >= timeBetweenWaves || waveCount == 0)
            {
                StartWave(difficultyManager.Difficulty);
            }
            else
            {
                timeSinceLastWaveEnd += Time.unscaledDeltaTime;
            }
        }
    }


    void StartWave(float difficulty)
    {
        
        if (difficulty < 5)
        {
            totalEnemiesInWave = 10;
        }
        else
        {
            totalEnemiesInWave = 2 * (int)difficulty;
        }
        
        spawnManager.updateMaxSpawns();
        inWave = true;
        waveCount++;
        timeSinceLastWaveEnd = 0;
        
        Debug.Log(difficulty);
    }
    void InWave()
    {
        spawnManager.continuouslySpawn();
        
    }

    // The enemies that have already been spawned are the last ones in the wave. No more spawning
    void WaveIsStagnant()
    {
        if (spawnManager.CurrNumOfEnemiesAlive() == 0)
        {
            EndWave();
        }
    }
    
    void EndWave()
    {
        spawnManager.TotalEnemiesSpawned = 0; // reset the spawn manager's count
        inWave = false;
        EventManager.TriggerEvent("newWave", (waveCount+1).ToString());
    }

}
