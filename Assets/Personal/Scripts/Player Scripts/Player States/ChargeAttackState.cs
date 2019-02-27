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
    float endingDistance;
    private float dashTime;
    private float timer;
    private float postHitTime;
    //private float currentTimeScale;
    private float slowestTimeScale;
    private float slowmoLerpFactor;
    float staminaRegain;
    bool dashing;
    bool attacked;
    TimeScaleManager timeScaleManager;
    PlayerStamina playerStamina;
    

    float explodeRadius;
    float explodePower;

    Vector3 initialPosition;

    public ChargeAttackState(PlayerMover pm, RaycastHit target) : base(pm)
    {
        playerMover = pm;
        endingDistance = playerMover.playerValues.chargeAttackStateValues.EndingDistance;
        dashTime = playerMover.playerValues.chargeAttackStateValues.DashTime;
        postHitTime = playerMover.playerValues.chargeAttackStateValues.PostHitTime;
        slowestTimeScale = playerMover.playerValues.chargeAttackStateValues.SlowestTimeScale;
        slowmoLerpFactor = playerMover.playerValues.chargeAttackStateValues.SlowmoLerpFactor;
        staminaRegain = playerMover.playerValues.chargeAttackStateValues.StaminaRegain;
        explodePower = playerMover.playerValues.chargeAttackStateValues.ExplodePower;
        explodeRadius = playerMover.playerValues.chargeAttackStateValues.ExplodeRadius;

        attackTarget = target;
        timer = 0;
        attacked = false;
        vulnerable = false;
        timeScaleManager = Camera.main.GetComponent<TimeScaleManager>();
        playerStamina = playerMover.PlayerStamina;
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
            timeScaleManager.ChangeTimeScale(slowestTimeScale, postHitTime, slowmoLerpFactor);
            timer += Time.fixedUnscaledDeltaTime;
        }
        else if (!attacked)
        {
            playerMover.Move(Vector3.zero);
            if (attackTarget.collider != null)
            {
                attackTarget.collider.gameObject.GetComponent<EnemyController>().takeDamage(attackTarget.point);
            }
            
            attacked = true;

            //HOTFIX
            //teleports player to moveTarget in case the player did not reach the destination properly (repro: attack enemy that is on platform above while on a lower level)
            playerMover.transform.position = moveTarget;


            AddExplosion(attackTarget.point);
            playerStamina.RegainStamina(staminaRegain);
            timer += Time.fixedDeltaTime;
        }
        else
        {
            MouseLookFixedUpdate();
            timer += Time.fixedUnscaledDeltaTime;
        }

        if (timer >= dashTime + postHitTime)
        {
            if (Input.GetButton("Dash"))
            {
                    if (playerStamina.UseStamina(playerMover.playerValues.groundStateValues.DashCost))
                    {
                        return new DashState(playerMover);
                    }
            }

            if (Input.GetButton("Jump"))
            {
                return new AirState(playerMover, playerMover.jumpSpeed);
            }


            if (playerMover.isGrounded())
            {
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