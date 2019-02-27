using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int HealthAmount = 50;

    // called once per frame

    private void Update()
    {
        transform.Rotate(0, 1, 0);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerHealth>().GainHealth(HealthAmount);
            col.gameObject.GetComponent<PickupSpawner>().PickedUp();
            Destroy(gameObject);
        }
        if (col.gameObject.tag == "Enemy")
        {
            Physics.IgnoreCollision(col, gameObject.GetComponent<Collider>());
        }
    }

    private void SpawnPickup(GameObject gameObject, GameObject tether)
    {

    }
        
}
