using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{

    float mass;
    public Vector3 impact;
    private CharacterController enemy;
    public bool skidding;

    void Start()
    {
        impact = Vector3.zero;
        mass = GetComponent<ActorValues>().impactValues.Mass;
        enemy = GetComponent<CharacterController>();
    }

    // call this function to add an impact force:
    public void AddKnock(Vector3 direction, float force)
    {
        direction.Normalize();
        if (direction.y < 0 && enemy.isGrounded)
        {
            direction.y = 0;
            skidding = true;
        }
        impact = (impact + direction.normalized * force / mass);
    }

    void Update()
    {
        if (!enemy.isGrounded)
        {
            impact += Physics.gravity * 2 * Time.fixedDeltaTime;
        }
        // apply the impact force:
        if (impact.magnitude > 0.2)
        {
            if (skidding)
            {
                // consumes the impact energy each cycle:
                impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime);
            }
            enemy.Move(impact * Time.fixedDeltaTime);
        }
    }
}
