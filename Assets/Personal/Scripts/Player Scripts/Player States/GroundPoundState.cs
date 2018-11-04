using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPoundState : PlayerState
{
    PlayerMover playerMover;
    Vector3 move;
    private float groundPoundSpeed = 30f;
    bool grounded = false;
    float gravityMultiplier = 10;
    float speedMaximum = 50f;
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
        chargeController = playerMover.gameObject.GetComponent<ChargeController>();
        //enemyMask = LayerMask.GetMask("Enemy");
    }


    public override PlayerState FixedUpdate()
    {
        chargeController.Charge(charging);

        if (grounded)
        {
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
        move = new Vector3(0, -groundPoundSpeed, 0);
        charging = Input.GetButton("Fire1");
    }

    public override void Exit()
    {

        float speed = -move.y;
        if (speed > 35)
        {
            float damageRange = (speed - 30)/5;
            float physicsRange = (speed - 30)/3;
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

            if (rb != null)
            {
                rb.AddExplosionForce(physicsMaxForce * (1 - (Vector3.Distance(hit.gameObject.transform.position, 
                    playerMover.transform.position) / range)), playerMover.transform.position, range, 0F, ForceMode.Impulse);
            }
        }
    }

    public void dealPoundDamage(float range)
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

    public void dealPoundImpacts(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(playerMover.transform.position, range, LayerMask.GetMask("Enemy"));
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                //adds an impulse relative to how close they are to the center of the impact
                hit.gameObject.GetComponent<ImpactReceiver>().AddImpact(hit.gameObject.transform.position - playerMover.transform.position, 
                    physicsMaxForce * (1-(Vector3.Distance(hit.gameObject.transform.position, playerMover.transform.position)/range)));
                // add an impulse instead of doing damage
                //hit.gameObject.GetComponent<EnemyController>().takeDamage(hit.gameObject.transform.position);
            }
        }
    }

}
