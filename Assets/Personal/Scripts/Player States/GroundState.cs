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
	private ChargeController chargeController;

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

		chargeController.Charge (charging);
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

		charging = Input.GetButton ("Fire1");
		grounded = playerMover.isGrounded ();

	}

	public override void Enter()
	{
		move.y = 0f;
		jumping = false;
		playerMover.PlayLandSound();
	}

		
}
