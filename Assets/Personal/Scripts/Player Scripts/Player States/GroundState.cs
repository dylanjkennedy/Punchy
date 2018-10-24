using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class GroundState : PlayerState {
	PlayerMover playerMover;
	Vector3 move;
	bool jumping;
	bool charging;
	bool grounded;
    bool dashing;
    public bool vulnerable = true;
    readonly float staminaCost = 25f;

    private ChargeController chargeController;
    RaycastHit hit;

    public GroundState(PlayerMover pm) : base(pm)
	{
		playerMover = pm;
		chargeController = playerMover.gameObject.GetComponent<ChargeController> ();

	}

	public override PlayerState FixedUpdate()
	{
		move = getStandardDesiredMove (playerMover.speed);
		move.y = -playerMover.stickToGroundForce;


		if (jumping) 
		{
			return new AirState(playerMover, playerMover.jumpSpeed);
		}
		if (!grounded)
		{
			return new AirState (playerMover, 0);
		}
        if (dashing)
        {
            if (playerMover.UseStamina(staminaCost))
            {
                return new DashState(playerMover);
            }
        }
        if (charging)
        {
            hit = chargeController.checkAttack();
            if (hit.collider != null)
            {
                if (playerMover.UseStamina(10f))
                {
                    return new ChargeAttackState(playerMover, hit);

                }
            }
        }
        

        playerMover.Move (move);
		MouseLookFixedUpdate ();
		return null;
	}

	public override void Update ()
	{
		MouseLookUpdate ();
		move.y = 0f;

		if (!jumping) 
		{
			jumping = Input.GetButtonDown ("Jump");
		}
        dashing = Input.GetButtonDown("Dash");
		charging = Input.GetButtonDown ("Fire1");
		grounded = playerMover.isGrounded ();

	}

	public override void Enter()
	{
		move.y = 0f;
		jumping = false;
		playerMover.PlayLandSound();
        charging = Input.GetButton("Fire1");
    }


}
