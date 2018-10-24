using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class ChargeAttackState : PlayerState
{
    private PlayerMover playerMover;
    RaycastHit attackTarget;
    Vector3 moveTarget;
    float endingDistance = 2f;
    private float dashTime = 0.16666667f;
    private float timer;
    private float postHitTime = 30f/60;
    //private float currentTimeScale;
    private float slowestTimeScale = 0.05f;
    private float slowmoLerpFactor = 0.01f;
    bool dashing;
    bool attacked;
    TimeScaleManager timeScaleManager;
    

    float explodeRadius = 1;
    float explodePower = 50;

    Vector3 initialPosition;

    public ChargeAttackState(PlayerMover pm, RaycastHit target) : base(pm)
    {
        playerMover = pm;
        attackTarget = target;
        timer = 0;
        attacked = false;
        vulnerable = false;
        timeScaleManager = Camera.main.GetComponent<TimeScaleManager>();
    }

    public override void Enter()
    {
        attackTarget.collider.gameObject.GetComponent<EnemyController>().freeze();
        Vector3 attackTargetPosition = attackTarget.collider.gameObject.transform.position;
        initialPosition = playerMover.transform.position;
        moveTarget = attackTargetPosition - Vector3.Normalize(attackTargetPosition - initialPosition)*endingDistance;
        if (Vector3.Distance(moveTarget,attackTargetPosition) > Vector3.Distance(initialPosition, attackTargetPosition))
        {
            moveTarget = initialPosition;
        }
        dashing = true;
    }

    public override void Exit()
    {

    }

    public override PlayerState FixedUpdate()
    {
        if (timer < dashTime)
        {
            playerMover.Move((moveTarget - playerMover.gameObject.transform.position) / (dashTime - timer));
            timer += Time.fixedDeltaTime;
        }
        else if (dashing)
        {
            playerMover.Move(moveTarget - playerMover.transform.position);
            dashing = false;
            timeScaleManager.changeTimeScale(slowestTimeScale, postHitTime, slowmoLerpFactor);
            timer += Time.fixedUnscaledDeltaTime;
        }
        else if (!attacked)
        {
            playerMover.Move(Vector3.zero);

            attackTarget.collider.gameObject.GetComponent<EnemyController>().takeDamage(attackTarget.point);
            attacked = true;
            AddExplosion(attackTarget.point);
            timer += Time.fixedDeltaTime;
        }
        else
        {
            MouseLookFixedUpdate();
            timer += Time.fixedUnscaledDeltaTime;
        }

        if (timer >= dashTime + postHitTime)
        {
            if (playerMover.isGrounded())
            {
                if (Input.GetButton("Jump"))
                {
                    return new AirState(playerMover, playerMover.jumpSpeed);
                }
                return new GroundState(playerMover);
            }
            else
            {
                return new AirState(playerMover);
            }
        }

        return null;
    }

    public override void Update()
    {
        if (attacked)
        {
            MouseLookUpdate();
        }
    }

    void AddExplosion(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, explodeRadius, LayerMask.GetMask("Debris"));
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explodePower, position, explodeRadius, 0F, ForceMode.Impulse);
            }
        }
    }
}