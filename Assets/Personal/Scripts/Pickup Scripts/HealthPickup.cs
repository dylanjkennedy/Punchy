using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : PickupController
{
    [SerializeField] private int HealthAmount = 50;


    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerHealth>().GainHealth(HealthAmount);
            col.gameObject.GetComponent<PickupSpawner>().PickedUp(gameObject);
        }
    }
}
