using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class AirState : PlayerState {
	PlayerMover playerMover;
	Vector3 move;
	bool charging;
	bool grounded;
    bool groundPound;
    float gravityMultiplier = 2;
	float airSpeedMultiplier = 1;
	float initialVerticalSpeed;
    float groundPoundCost = 25f;
	private ChargeController chargeController;
    private PlayerStamina stamina;
    public bool vulnerable = true;
    RaycastHit hit;

	public AirState(PlayerMover pm) : base(pm)
	{
		playerMover = pm;
		initialVerticalSpeed = 0;
		chargeController = playerMover.gameObject.GetComponent<ChargeController> ();
        stamina = playerMover.gameObject.gameObject.GetComponent<PlayerStamina>();
        groundPound = false;
	}

	public AirState(PlayerMover pm, float verticalSpeed) : base(pm)
	{
		playerMover = pm;
		initialVerticalSpeed = verticalSpeed;
		chargeController = playerMover.gameObject.GetComponent<ChargeController> ();
        stamina = playerMover.gameObject.gameObject.GetComponent<PlayerStamina>();
        groundPound = false;
	}

	public override PlayerState FixedUpdate()
	{
		if (grounded)
		{
			return new GroundState (playerMover);
		}
        else if (groundPound)
		{
            if (stamina.UseStamina(groundPoundCost))
            {
                return new GroundPoundState(playerMover);
            }
		}

		Vector3 desiredMove = getStandardDesiredMove (playerMover.speed * airSpeedMultiplier);

		move = new Vector3 (desiredMove.x, move.y, desiredMove.z);
		move += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;

		hit = chargeController.Charge (charging);
        if (hit.collider != null)
        {
            return new ChargeAttackState(playerMover, hit);
        }
		playerMover.Move (move);

		MouseLookFixedUpdate ();

		return null;
	}

	public override void Update()
	{
		charging = Input.GetButton ("Fire1");
		grounded = playerMover.isGrounded ();
		MouseLookUpdate ();
	    if (Input.GetButton("Crouch"))
	    {
	        groundPound = true;
	    }
	}

	public override void Enter ()
	{
		Vector3 desiredMove = getStandardDesiredMove (playerMover.speed);
		move = new Vector3 (desiredMove.x, move.y+ initialVerticalSpeed, desiredMove.z);
		playerMover.Move (move);
        charging = Input.GetButton("Fire1");
    }
}