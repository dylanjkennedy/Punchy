using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    public enum EnemyType : int { Cylinder = 0, Humanoid };
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] DifficultyManager difficultyManager;
    List<GameObject>[] enemies = new List<GameObject>[System.Enum.GetValues(typeof(EnemyType)).Length];
    [SerializeField] int[] baseSpawnLimits;
    [SerializeField] float[] spawnLimitsGrowth;
    [SerializeField] int[] spawnLimits; // = { 15, 5 };
    [SerializeField] GameObject enemiesParent;
    float[] spawnTimers = { 0, 0 };
    float[] baseTimesToNextSpawn = {1, 3 };
    float[] spawnTimeRanges = {0.5f, 2};
    float[] timesOfNextSpawn = {0, 0};
    GameObject player;
    TethersTracker tethersTracker;
    DifficultyValues difficultyValues;


    // Use this for initialization
    void Start () {
        //might need to be done differently? 
        difficultyValues = difficultyManager.DifficultyValues;
        baseSpawnLimits = difficultyValues.SpawnDifficultyValues.BaseSpawnLimits;
        spawnLimitsGrowth = difficultyValues.SpawnDifficultyValues.SpawnLimitsGrowth;


        spawnLimits = new int[baseSpawnLimits.Length];
        for (int i = 0; i < spawnLimits.Length; i++)
        {
            spawnLimits[i] = baseSpawnLimits[i];
        }

        //subject to change where this is relative to other things?
        player = gameObject;
        tethersTracker = player.GetComponentInChildren<TethersTracker>();
        for (int i = 0; i < System.Enum.GetValues(typeof(EnemyType)).Length; i++)
        {
            enemies[i] = new List<GameObject>();
        }

        foreach (EnemyController enemy in enemiesParent.GetComponentsInChildren<EnemyController>())
        {
            enemies[(int)enemy.Type].Add(enemy.gameObject);
        }

        for (int i = 0; i < spawnTimers.Length; i++)
        {
            timesOfNextSpawn[i] = baseTimesToNextSpawn[i] + Random.Range(-spawnTimeRanges[i], spawnTimeRanges[i]);
        }
	}

    // Update is called once per frame
    void Update() {
        for (int i = 0; i < spawnTimers.Length; i++)
        {
            spawnTimers[i] += Time.deltaTime;
            if (CheckSpawn((EnemyType)i))
            {
                GameObject spawner = FindSpawner();
                SpawnEnemy((EnemyType)i, spawner);
            }
        }
        updateMaxSpawns();
    }

    bool CheckSpawn(EnemyType type)
    {
        if (enemies[(int)type].Count < spawnLimits[(int)type] && (spawnTimers[(int)type] >= timesOfNextSpawn[(int)type]))
        {
            return true;
        }
        return false;
    }

    GameObject FindSpawner()
    {
        TetherController[] tethers = tethersTracker.Tethers;
        int[] weights = new int[tethers.Length];
        int minWeightIndex = -1;
        int minWeight = int.MaxValue;

        for (int i = 0; i < tethers.Length; i++)
        {
            weights[i] += 10 * tethers[i].Occupants ^ 2;

            //We want distance from tether to player to be 30, so weight the difference of actual distance from that by difference*2
            weights[i] += (int)(Mathf.Abs(
                Vector3.Distance(tethers[i].gameObject.transform.position, player.transform.position) - 30)) * 2;

            //Weight of trace ratio at inverseRatio*50. Perfect visibility weights 50, no visibility weights 0;
            weights[i] += (int)((tethers[i].TraceRatio) * 50);
            

            if (weights[i] < minWeight)
            {
                minWeightIndex = i;
                minWeight = weights[i];
            }
        }

        return tethers[minWeightIndex].gameObject;
    }

    void SpawnEnemy(EnemyType type, GameObject tether)
    {
        GameObject enemy = Instantiate(enemyPrefabs[(int)type], tether.transform.position, tether.transform.rotation, enemiesParent.transform);
        enemy.SetActive(true);
        enemy.layer = 9;
        spawnTimers[(int)type] = 0;
        timesOfNextSpawn[(int)type] = baseTimesToNextSpawn[(int)type] + Random.Range(-spawnTimeRanges[(int)type], spawnTimeRanges[(int)type]);
        enemies[(int)type].Add(enemy);
    }

    public void DestroyEnemy(GameObject enemy)
    {
        enemies[(int)enemy.GetComponent<EnemyController>().Type].Remove(enemy);
        Destroy(enemy);
    }

    public void updateMaxSpawns()
    {
        float difficulty = difficultyManager.Difficulty;
        for (int i = 0; i < spawnLimits.Length; i++)
        {
            spawnLimits[i] = baseSpawnLimits[i] + Mathf.FloorToInt(spawnLimitsGrowth[i] * difficulty);
        }
    }
}
