using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public enum PickupType : int { HealthPickup, StaminaPickup };
    [SerializeField] GameObject[] pickupPrefabs;
    TetherController[] tethersTracker;
    List<GameObject> pickups = new List<GameObject>(); //tracks pickups -- used for pickup despawning
    //List<Vector3> locations = new List<Vector3>(); //tracks locations of tethers to ensure no double spawning
    float spawnTimer;
    float nextSpawn;
    int pickupCount;
    [SerializeField] int pickupLimit;
    //GameObject[] pickup;
    [SerializeField] float upperBoundTime;
    [SerializeField] float lowerBoundTime;
    [SerializeField] GameObject pickupsParent;
    [SerializeField] float despawnTime;
    float[] spawnChances = { 0.5f, 1.0f }; //Goes from 0 to 1 to control spawn rate chances.

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

        for(int i = 0; i < pickups.Count; i++)
        {
            if (pickups[i].GetComponent<PickupController>().timer >= despawnTime)
            {
                PickedUp(pickups[i]);
                //spawnTimer = 0;
            }
        }
    }

    bool CheckSpawn()
    {
        nextSpawn = Random.Range(lowerBoundTime, upperBoundTime);
        if (spawnTimer >= nextSpawn && pickupCount < pickupLimit)
        {
            return true;
        }
        else if (spawnTimer >= nextSpawn && pickupCount >= pickupLimit)
        {
            spawnTimer = 0;
            return false;
        }
        else return false;
    }

    GameObject FindSpawner()
    {
        int randomNumber = Random.Range(0, tethersTracker.Length);
        for (int i = 0; i < pickups.Count; i++)
        {
            if((pickups[i].transform.position + new Vector3(0, 5/4, 0)) == tethersTracker[randomNumber].gameObject.transform.position)
            {
                FindSpawner();
            }
        }
        return tethersTracker[randomNumber].gameObject;
    }

    GameObject GrabPickupType()
    {
        float value = Random.Range(0.0f, 1.0f);
        GameObject obj = pickupPrefabs[0];
        for (int j = 0; j < spawnChances.Length; j++)
        {
            if (value <= spawnChances[j])
            {
                return obj = pickupPrefabs[j];
            }

        }
        return obj;
    }

    void SpawnPickup(GameObject tether)
    {
        Vector3 pos = tether.transform.position;
        GameObject pickup = Instantiate(GrabPickupType(), pos - new Vector3(0, 5 / 4, 0), tether.transform.rotation, pickupsParent.transform);
        spawnTimer = 0;
        pickups.Add(pickup); //
        pickupCount++;

        //locations.Add(pos);
    }
    public void PickedUp(GameObject obj)
    {
        pickupCount--;
        pickups.Remove(obj);
        Destroy(obj);

        //locations.Remove(obj.transform.position + new Vector3(0, 5/4, 0));
    }


}