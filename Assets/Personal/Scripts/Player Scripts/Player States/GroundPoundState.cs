using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPoundState : PlayerState
{
    PlayerMover playerMover;
    Vector3 move;
    private float groundPoundSpeed = 30f;
    private float groundPoundHopSpeed = 20f;
    bool grounded = false;
    float gravityMultiplier = 10;
    float speedMaximum = 35f;
    float physicsMaxForce = 40f;
    //float airSpeedMultiplier = 1;
    public bool vulnerable = true;
    bool charging;
    RaycastHit hit;
    private LayerMask enemyMask;
    ChargeController chargeController;

    public GroundPoundState(PlayerMover pm) : base(pm)
    {
        playerMover = pm;
        vulnerable = false;
        chargeController = playerMover.ChargeController;
        //enemyMask = LayerMask.GetMask("Enemy");
    }


    public override PlayerState FixedUpdate()
    {
        chargeController.Charge(charging);

        if (grounded)
        {
            Pound();
            return new GroundState(playerMover);
        }

        //move = new Vector3(desiredMove.x, move.y * -groundPoundSpeed, desiredMove.z);
        move += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
        if (-move.y > speedMaximum)
        {
            move = new Vector3(move.x, -speedMaximum, move.z);
        }
        playerMover.Move(move);

        MouseLookFixedUpdate();
        return null;
    }

    public override void Update()
    {
        grounded = playerMover.isGrounded();
        charging = Input.GetButton("Fire1");
        MouseLookUpdate();
    }

    public override void Enter()
    {
        move = new Vector3(0, groundPoundHopSpeed, 0);
        charging = Input.GetButton("Fire1");
    }

    private void Pound()
    {

        float speed = -move.y;
        if (speed > 25)
        {
            float damageRange = (speed - 20)/3;
            float physicsRange = (speed - 20)/2;
            dealPoundDamage(damageRange);
            dealPoundImpacts(physicsRange);
            dealPoundPhysics(physicsRange);
        }
    }

    private void dealPoundPhysics(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(playerMover.transform.position, range, LayerMask.GetMask("Debris"));
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            float forceMultiplier = physicsMaxForce / range;
            if (rb != null)
            {
                rb.AddExplosionForce(forceMultiplier*(Vector3.Distance(hit.gameObject.transform.position, 
                    playerMover.transform.position)), playerMover.transform.position, range, 0F, ForceMode.Impulse);
            }
        }
    }

    private void dealPoundDamage(float range)
    { 
        Collider[] colliders = Physics.OverlapSphere(playerMover.transform.position, range, LayerMask.GetMask("Enemy"));
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                hit.gameObject.GetComponent<EnemyController>().takeDamage(hit.gameObject.transform.position);
            }
        }
    }

    private void dealPoundImpacts(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(playerMover.transform.position, range, LayerMask.GetMask("Enemy"));
        foreach (Collider hit in colliders)
        {
            float forceMultiplier = physicsMaxForce / range;
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                //adds an impulse relative to how close they are to the center of the impact
                hit.gameObject.GetComponent<ImpactReceiver>().AddImpact(hit.gameObject.transform.position - playerMover.transform.position, forceMultiplier*(Vector3.Distance(hit.gameObject.transform.position, playerMover.transform.position)));
            }
        }
    }

}
