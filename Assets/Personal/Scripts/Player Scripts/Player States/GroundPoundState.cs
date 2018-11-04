using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPoundState : PlayerState
{
    PlayerMover playerMover;
    Vector3 move;
    private float groundPoundSpeed = 50f;
    bool grounded = false;
    float gravityMultiplier = 10;
    //float airSpeedMultiplier = 1;
    public bool vulnerable = true;
    RaycastHit hit;
    private float heightValue;
    private LayerMask enemyMask;

    public GroundPoundState(PlayerMover pm) : base(pm)
    {
        playerMover = pm;
        vulnerable = false;
        heightValue = 0;
        //enemyMask = LayerMask.GetMask("Enemy");
    }


    public override PlayerState FixedUpdate()
    {
        if (grounded)
        {
            return new GroundState(playerMover);
        }

        //move = new Vector3(desiredMove.x, move.y * -groundPoundSpeed, desiredMove.z);
        move += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
        playerMover.Move(move);

        MouseLookFixedUpdate();
        heightValue += Time.fixedDeltaTime;
        return null;
    }

    public override void Update()
    {
        grounded = playerMover.isGrounded();
        MouseLookUpdate();
    }

    public override void Enter()
    {
        move = getStandardDesiredMove(groundPoundSpeed);
        move.y = move.y * -groundPoundSpeed;
    }

    public override void Exit()
    {
        dealPoundDamage();
        dealPoundPhysics();
    }

    public void dealPoundDamage()
    {

        Collider[] colliders = Physics.OverlapSphere(playerMover.transform.position, heightValue/2, LayerMask.GetMask("Enemy"));
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                hit.gameObject.GetComponent<EnemyController>().takeDamage(hit.gameObject.transform.position);
            }
        }
    }

    public void dealPoundPhysics()
    {
        Collider[] colliders = Physics.OverlapSphere(playerMover.transform.position, heightValue/2, LayerMask.GetMask("Enemy"));
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // add an impulse instead of doing damage
                //hit.gameObject.GetComponent<EnemyController>().takeDamage(hit.gameObject.transform.position);
            }
        }
    }

}
