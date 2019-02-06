using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPickup : MonoBehaviour
{
    private int StaminaAmount = 100;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 1, 0);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerStamina>().RegainStaminaWithoutRegen(StaminaAmount);
            col.gameObject.GetComponent<PickupSpawner>().PickedUp();
            Destroy(gameObject);
        }
       
       if (col.gameObject.tag == "Enemy")
       {
           Physics.IgnoreCollision(col, gameObject.GetComponent<Collider>());
       }
       
    }

}
