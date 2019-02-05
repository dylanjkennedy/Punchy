using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] pickupPrefabs;
    TetherController[] tethersTracker;
    float spawnTimer;
    [SerializeField] float nextSpawn;
    int pickupCount;
    [SerializeField] int pickupLimit;
    //GameObject[] pickup;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = 0;
        pickupCount = 0;
        tethersTracker = gameObject.GetComponentInChildren<TethersTracker>().Tethers;
    }

    // Update is called once per frame
    void Update()
    {  
            spawnTimer += Time.deltaTime;
            if (CheckSpawn())
            {
                SpawnPickup(FindSpawner());
            }
        
    }

    bool CheckSpawn()
    {
        if (spawnTimer >= nextSpawn && pickupCount < pickupLimit)
        {
            return true;
        }
        else return false;
    }

    GameObject FindSpawner()
    {
        int randomNumber = Random.Range(0, tethersTracker.Length);
        return tethersTracker[randomNumber].gameObject;
    }

    void SpawnPickup(GameObject tether)
    {
        int i = Random.Range(0, pickupPrefabs.Length);
        Vector3 pos = tether.transform.position - new Vector3(0, 5/4, 0);
        GameObject pickup = Instantiate(pickupPrefabs[i], pos, tether.transform.rotation);
        spawnTimer = 0;
        pickupCount++;
    }
    public void PickedUp()
    {
        pickupCount--;
    }
}

