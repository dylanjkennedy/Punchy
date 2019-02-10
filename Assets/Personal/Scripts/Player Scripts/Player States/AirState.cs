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
    float gravityMultiplier;
	float airSpeedMultiplier;
	float initialVerticalSpeed;
    float groundPoundStaminaCost;
	private ChargeController chargeController;
    private PlayerStamina playerStamina;
    RaycastHit hit;
    SpherecastCommand walljump;
    bool initialJumpDone;
    bool jumpRemaining;
    bool isJumping;

    public AirState(PlayerMover pm) : base(pm)
	{
		playerMover = pm;
        gravityMultiplier = playerMover.playerValues.airStateValues.AirGravityMultiplier;
        airSpeedMultiplier = playerMover.playerValues.airStateValues.AirSpeedMultiplier;
        groundPoundStaminaCost = playerMover.playerValues.airStateValues.GroundPoundStaminaCost;
		initialVerticalSpeed = 0;
        chargeController = playerMover.ChargeController;
        playerStamina = playerMover.PlayerStamina;
        groundPound = false;
        vulnerable = true;
        jumpRemaining= true;
        initialJumpDone = false;

    }

    public AirState(PlayerMover pm, float verticalSpeed) : base(pm)
	{
		playerMover = pm;
        gravityMultiplier = playerMover.playerValues.airStateValues.AirGravityMultiplier;
        airSpeedMultiplier = playerMover.playerValues.airStateValues.AirSpeedMultiplier;
        groundPoundStaminaCost = playerMover.playerValues.airStateValues.GroundPoundStaminaCost;
        initialVerticalSpeed = verticalSpeed;
        chargeController = playerMover.ChargeController;
        playerStamina = playerMover.PlayerStamina;
        groundPound = false;
        vulnerable = true;
        jumpRemaining = true;
        initialJumpDone = false;
    }

	public override PlayerState FixedUpdate()
	{
		if (grounded)
		{
			return new GroundState (playerMover);
		}
        else if (groundPound)
		{
            if (playerStamina.UseStamina(groundPoundStaminaCost))
            {
                return new GroundPoundState(playerMover);
            }
		}

		Vector3 desiredMove = GetStandardDesiredMove (playerMover.speed * airSpeedMultiplier);

		move = new Vector3 (desiredMove.x, move.y, desiredMove.z);
        if (isJumping)
        {
            Vector3 head = playerMover.gameObject.transform.position;
            head.y += .5f;
            Vector3 toe = head;
            toe.y += -1f;

            Collider[] closeWalls = Physics.OverlapCapsule(head, toe, 1f,LayerMask.GetMask("Default"));
            if (closeWalls.Length > 0)
            {
                Vector3 pointOfContact=closeWalls[0].ClosestPointOnBounds(toe);
                float xDistance = toe.x - pointOfContact.x;
                float zDistance = toe.z - pointOfContact.z;

                if (xDistance> zDistance)
                {
                    move.z += playerMover.jumpSpeed;
                }
                else
                {
                    move.x += playerMover.jumpSpeed;
                }
                move.y += playerMover.jumpSpeed;
                isJumping = false;
            }
        }
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
        if(!Input.GetButton("Jump"))
        {
            initialJumpDone=true;
        }
        if (initialJumpDone && jumpRemaining && Input.GetButton("Jump"))
        {
            isJumping = true;
            jumpRemaining = false;
        }

    }

	public override void Enter ()
	{
		Vector3 desiredMove = GetStandardDesiredMove (playerMover.speed);
		move = new Vector3 (desiredMove.x, move.y+ initialVerticalSpeed, desiredMove.z);
		playerMover.Move (move);
        charging = Input.GetButton("Fire1");
    }
}