using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    public enum EnemyType : int { Cylinder = 0, Humanoid };
    List<GameObject>[] enemies = new List<GameObject>[System.Enum.GetValues(typeof(EnemyType)).Length];
    int[] spawnLimits = { 15, 5 };
    float[] spawnTimers = { 0, 0 };
    float[] spawnTimes = { };
    float[] spawnRanges = { };
    float[] timesOfNextSpawn = { };

    // Use this for initialization
    void Start () {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("enemy"))
        {
            enemies[(int)enemy.GetComponent<EnemyController>().Type].Add(enemy);
        }

        for (int i = 0; i < spawnTimers.Length; i++)
        {
            timesOfNextSpawn[i] = spawnTimes[i] + Random.Range(-spawnRanges[i], spawnRanges[i]);
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
            }
        }
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
        return null;
    }

    void SpawnedEnemy(GameObject enemy)
    {
        enemies[(int)enemy.GetComponent<EnemyController>().Type].Add(enemy);
    }

    void DestroyEnemy(GameObject enemy)
    {
        enemies[(int)enemy.GetComponent<EnemyController>().Type].Remove(enemy);
        Destroy(enemy);
    }
}
