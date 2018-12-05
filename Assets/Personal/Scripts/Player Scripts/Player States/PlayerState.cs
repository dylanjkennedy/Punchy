using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public abstract class PlayerState
{
    private PlayerMover playerMover;
    private MouseLook mouseLook;
    private Camera cam;
    public bool vulnerable = true;

    public PlayerState(PlayerMover pm)
    {
        playerMover = pm;
        mouseLook = playerMover.MouseLook;
        cam = Camera.main;
    }

    public virtual PlayerState FixedUpdate()
    {
        MouseLookFixedUpdate();
        return null;
    }

    public virtual void Update()
    {
        MouseLookUpdate();
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    protected virtual void MouseLookFixedUpdate()
    {
        mouseLook.UpdateCursorLock();
    }

    protected virtual void MouseLookUpdate()
    {
        RotateView();
    }

    private void RotateView()
    {
        mouseLook.LookRotation(playerMover.transform, cam.transform);
    }

	protected virtual Vector2 GetInput()
	{
		float horizontal = CrossPlatformInputManager.GetAxisRaw ("Horizontal");
		float vertical = CrossPlatformInputManager.GetAxisRaw ("Vertical");
		Vector2 input = new Vector2 (horizontal, vertical);

		if (input.sqrMagnitude > 1) 
		{
			input.Normalize ();
		}
		return input;
	}

	public virtual Vector3 GetStandardDesiredMove(float speed)
	{
		Vector3 move = GetInput ();

		// transfer from world coordinates to player coordinates
		Vector3 desiredMove = playerMover.transform.forward * move.y + playerMover.transform.right * move.x;

		RaycastHit hitInfo = playerMover.GetSurfaceNormal ();

		desiredMove = Vector3.ProjectOnPlane (desiredMove, hitInfo.normal).normalized;

		move.x = desiredMove.x * speed;
		move.z = desiredMove.z * speed;
		return move;
	}
}
