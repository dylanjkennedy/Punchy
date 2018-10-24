using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpawner : MonoBehaviour {
    [SerializeField] float spawnTime;
    [SerializeField] float spawnTimeRange;
    [SerializeField] GameObject enemy;
    float timeToSpawn;
    float timeSinceSpawn;
	// Use this for initialization
	void Start () {
        timeSinceSpawn = 0;
        timeToSpawn = spawnTime + Random.Range(-spawnTimeRange, spawnTimeRange);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        timeSinceSpawn += Time.deltaTime;
        if (timeToSpawn <= timeSinceSpawn)
        {
            spawn();
        }
	}

    void spawn()
    {
        GameObject spawnedEnemy = Instantiate(enemy, gameObject.transform.position, gameObject.transform.rotation);
        spawnedEnemy.SetActive(true);
        timeSinceSpawn = 0;
        timeToSpawn = spawnTime + Random.Range(-spawnTimeRange, spawnTimeRange);
    }
}
