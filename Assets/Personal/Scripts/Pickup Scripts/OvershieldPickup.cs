using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvershieldPickup : PickupController
{
    [SerializeField] public float OvershieldAmount;


private void OnTriggerEnter(Collider col)
{
    col.gameObject.GetComponent<PlayerHealth>().GainOvershield(OvershieldAmount);
    col.gameObject.GetComponent<PickupSpawner>().PickedUp(gameObject);
}
}