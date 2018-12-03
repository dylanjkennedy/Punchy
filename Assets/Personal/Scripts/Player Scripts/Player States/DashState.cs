using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerState
{
    private Vector3 movement;
    float timer;
    float dashTime = 10f/60;
    PlayerMover playerMover;
    float dashSpeed = 50f;
    bool charging;
    RaycastHit hit;

    private ChargeController chargeController;


    public DashState(PlayerMover pm) : base(pm)
    {
        playerMover = pm;
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
    }

    public override void Enter()
    {
        charging = Input.GetButton("Fire1");
        movement = getStandardDesiredMove(dashSpeed);
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

