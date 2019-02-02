using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    bool inWave = true;
    float timeSinceWaveStart = 0f;
    float timeSinceLastWaveEnd = 0f;
    int totalEnemiesSpawned = 0;
    [SerializeField] int totalEnemiesInWave = 5;
    [SerializeField] SpawnManager spawnManager;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        // logic for checking the time since the wave ended and setting inWave = true
        // and reset that counter

        if (inWave || timeSinceLastWaveEnd > 1000f)  
        {
            InWave();
            if (spawnManager.TotalEnemiesSpawned >= totalEnemiesInWave)
            {
                WaveIsStagnant();
            }
        }
        else
        {
            timeSinceLastWaveEnd += Time.unscaledDeltaTime;
        }
    }



    // way to start a wave
    // a wave needs to have either a amount of time or a number of enemies
    //     probably a number of enemies

    // need a way to determine that the wave has ended

    void InWave()
    {
        inWave = true;
        spawnManager.continuouslySpawn();
        timeSinceLastWaveEnd = 0;
        //Debug.Log(spawnManager.numOfEnemiesSpawned());

    }

    void WaveIsStagnant()
    {
        inWave = true;
        if (spawnManager.CurrNumOfEnemiesAlive() == 0) //&& timeSinceWaveStart < 10f)
        {
            EndWave();

            //Debug.Log("There are no enemies, end the wave");
        }
    }
    
    void EndWave()
    {
        spawnManager.TotalEnemiesSpawned = 0;
        inWave = false;
    }
}
