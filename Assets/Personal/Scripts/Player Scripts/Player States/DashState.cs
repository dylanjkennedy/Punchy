using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerState
{
    private Vector3 movement;
    float timer;
    float dashTime;
    PlayerMover playerMover;
    float dashSpeed;
    float dashJumpImpulse;
    bool charging;
    bool jumping;
    RaycastHit hit;

    private ChargeController chargeController;


    public DashState(PlayerMover pm) : base(pm)
    {
        playerMover = pm;
        dashTime = playerMover.playerValues.dashStateValues.DashTime;
        dashSpeed = playerMover.playerValues.dashStateValues.DashSpeed;
        dashJumpImpulse = playerMover.playerValues.dashStateValues.DashJumpImpulse;
        chargeController = playerMover.ChargeController;
        timer = 0;
        vulnerable = false;
    }

    public override PlayerState FixedUpdate()
    {
        hit = chargeController.Charge(charging);
        if (hit.collider != null)
        {
            return new ChargeAttackState(playerMover, hit);
        }

        if (jumping)
        {
            playerMover.gameObject.GetComponent<ImpactReceiver>().AddImpact(movement, dashJumpImpulse);
            return new AirState(playerMover, playerMover.jumpSpeed);
        }

        timer += Time.fixedDeltaTime;
        if (timer >= dashTime)
        {
            if (playerMover.isGrounded())
            {
                return new GroundState(playerMover);
            }
            return new AirState(playerMover);
        }

        playerMover.Move(movement);
        MouseLookFixedUpdate();

        return null;
    }

    public override void Update()
    {
        MouseLookUpdate();
        charging = Input.GetButton("Fire1");
        if (!jumping)
        {
            jumping = Input.GetButton("Jump");
        }
    }

    public override void Enter()
    {
        charging = Input.GetButton("Fire1");
        movement = GetStandardDesiredMove(dashSpeed);
        jumping = false;
        if (movement == new Vector3(0,0,0))
        {
            movement = playerMover.transform.forward;
            RaycastHit hitInfo = playerMover.GetSurfaceNormal();
            movement = Vector3.ProjectOnPlane(movement, hitInfo.normal).normalized;
            movement.x *= dashSpeed;
            movement.z *= dashSpeed;
        }
        movement.y =  0;
    }
}

