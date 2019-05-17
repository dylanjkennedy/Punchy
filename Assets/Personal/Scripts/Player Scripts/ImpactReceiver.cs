using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactReceiver : MonoBehaviour {

    float mass;
    float groundFriction;
    float airFriction;
    float recoveryImpact;
    Vector3 impact = Vector3.zero;
    CharacterController character;
 
    void Start()
    {
        ActorValues actorValues = GetComponent<ActorValues>();
        mass = actorValues.impactValues.Mass;
        groundFriction = actorValues.impactValues.GroundFriction;
        airFriction = actorValues.impactValues.AirFriction;
        recoveryImpact = actorValues.impactValues.RecoveryImpact;
        character = GetComponent<CharacterController>();
    }

    // call this function to add an impact force:
    public void AddImpact(Vector3 direction, float force)
    {
        direction.Normalize();
        if (direction.y < 0 && character.isGrounded)
        {
            direction.y = 0; //prevents player from being launched into the air and unable to jump?
        }
        impact += direction.normalized * force / mass;
    }

    public void Reflect(Vector3 normal)
    {
        impact = impact - 2 * (Vector3.Dot(impact, normal) * normal);
    }

    void FixedUpdate()
    {
        // apply the impact force:
        if ((impact.magnitude > recoveryImpact) || character.isGrounded)
        {
            character.Move(impact * Time.fixedDeltaTime);
        }
        else
        {
            impact = Vector3.zero;
        }

        impact += Physics.gravity * Time.deltaTime;

        if (character.isGrounded)
        {
            //Apply ground friction
            impact = Vector3.Lerp(impact, Vector3.zero, Time.fixedDeltaTime * groundFriction);
        }
        else
        {
            //Apply air friction
            impact = Vector3.Lerp(impact, Vector3.zero, Time.fixedDeltaTime * airFriction);
        }
    }
}
