using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactReceiver : MonoBehaviour {

    float mass;
    Vector3 impact = Vector3.zero;
    private CharacterController character;
    //PlayerMover playerMover;
 
    void Start()
    {
        mass = GetComponent<ActorValues>().impactValues.Mass;
        character = GetComponent<CharacterController>();
        //playerMover = gameObject.GetComponent<PlayerMover>();
    }

    // call this function to add an impact force:
    public void AddImpact(Vector3 direction, float force)
    {
        direction.Normalize();
        if (direction.y < 0 && character.isGrounded)
        {
            //direction.y = -direction.y; // reflect down force on the ground
            direction.y = 0; //prevents player from being launched into the air and unable to jump?
        }
        impact += direction.normalized * force / mass;
    }

    void Update()
    {
        // apply the impact force:
        if (impact.magnitude > 0.2) character.Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }
}
