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
    private float currentTimeScale;
    private float slowestTimeScale = 0.05f;
    private float slowmoLerpFactor = 0.01f;
    bool dashing;
    bool attacked;
    Vector3 initialPosition;

    public ChargeAttackState(PlayerMover pm, RaycastHit target) : base(pm)
    {
        playerMover = pm;
        attackTarget = target;
        timer = 0;
        attacked = false;
    }

    public override void Enter()
    {
        attackTarget.collider.gameObject.GetComponent<EnemyController>().freeze();
        Vector3 attackTargetPosition = attackTarget.collider.gameObject.transform.position;
        initialPosition = playerMover.transform.position;
        moveTarget = attackTargetPosition - Vector3.Normalize(attackTargetPosition - initialPosition)*endingDistance;
        dashing = true;
    }

    public override void Exit()
    {
        playerMover.changeTimeScale(1f);
    }

    public override PlayerState FixedUpdate()
    {
        if (timer < dashTime)
        {
            playerMover.Move((moveTarget - initialPosition) / (dashTime - timer));
            timer += Time.fixedDeltaTime;
        }
        else if (dashing)
        {
            playerMover.Move(moveTarget - playerMover.transform.position);
            dashing = false;
            //adjust timescale
            playerMover.changeTimeScale(slowestTimeScale);
            currentTimeScale = slowestTimeScale;
            timer += Time.fixedUnscaledDeltaTime;
        }
        else if (!attacked)
        {
            attackTarget.collider.gameObject.GetComponent<EnemyController>().takeDamage(attackTarget.point);
            attacked = true;
            timer += Time.fixedDeltaTime;
        }
        else
        {
            currentTimeScale = Mathf.Lerp(currentTimeScale, 1f, slowmoLerpFactor);

            playerMover.changeTimeScale(currentTimeScale);
            timer += Time.fixedUnscaledDeltaTime;
        }

        if (timer >= dashTime + postHitTime)
        {
            playerMover.changeTimeScale(1f);
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

    }
}