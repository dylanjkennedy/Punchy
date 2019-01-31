using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson; //FirstPersonController made this the namespace
using UnityEngine.SceneManagement;

public class PlayerMover : MonoBehaviour
{
    bool paused;
	public float speed;
	public float stickToGroundForce;
    public PlayerValues playerValues;
	private CharacterController characterController;
    private TimeScaleManager timeScaleManager;
    private ChargeController chargeController;
    GameObject PauseMenu;
    public ChargeController ChargeController
    {
        get { return chargeController; }
    }
    private PlayerStamina playerStamina;
    public PlayerStamina PlayerStamina
    {
        get { return playerStamina; }
    }
	private CollisionFlags collisionFlags;
    public PlayerState currentState;
	public float jumpSpeed;
    [SerializeField] private MouseLook mouseLook;
    public MouseLook MouseLook
    {
        get { return mouseLook; }
    }
    private Camera playerCamera;
    bool dead;



    // Use this for initialization
    void Start () {
        playerValues = GetComponent<PlayerValues>();
        speed = playerValues.movementValues.Speed;
        jumpSpeed = playerValues.movementValues.JumpSpeed;
        stickToGroundForce = playerValues.movementValues.StickToGroundForce;
        playerCamera = GetComponentInChildren<Camera>();
        timeScaleManager = GetComponentInChildren<TimeScaleManager>();
		characterController = GetComponent<CharacterController>();
        MouseLook.Init(transform, playerCamera.transform);
        currentState = new GroundState(this);
        dead = false;
        chargeController = GetComponent<ChargeController>();
        playerStamina = GetComponent<PlayerStamina>();
        PauseMenu = playerValues.generalValues.PauseMenu;

        paused = false;
    }


	
	// Update is called once per frame
	void FixedUpdate ()
	{
        if (!dead && !paused)
        {
            PlayerState newState = currentState.FixedUpdate();
            if (newState != null)
            {
                currentState.Exit();
                newState.Enter();
                currentState = newState;
            }
        }
	}



    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (TogglePause())
            {
                return;
            }
        }
        if (!dead && !paused)
        {
            currentState.Update();
        }
    }

    private bool TogglePause()
    {
        if (!paused)
        {
            paused = true;
            timeScaleManager.Pause(true);
            PauseMenu.SetActive(true);
            MouseLook.SetCursorLock(false);
            return paused;
        }
        else
        {
            paused = false;
            timeScaleManager.Pause(false);
            PauseMenu.SetActive(false);
            MouseLook.SetCursorLock(true);
            return paused;
        }
    }


	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Rigidbody body = hit.collider.attachedRigidbody;
		//dont move the rigidbody if the character is on top of it
		if (collisionFlags == CollisionFlags.Below)
		{
			return;
		}

		if (body == null || body.isKinematic)
		{
			return;
		}
		body.AddForceAtPosition(characterController.velocity*0.1f, hit.point, ForceMode.Impulse);
	}








	public void Move(Vector3 movement){
		collisionFlags = characterController.Move(movement*Time.fixedDeltaTime);
	}

	public void PlayLandSound(){
		
	}

	public RaycastHit GetSurfaceNormal()
	{
		// get a normal for the surface that is being touched to move along it
		RaycastHit hitInfo;
		Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
			characterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
		return hitInfo;
	}

	public bool isGrounded(){
		return characterController.isGrounded;
    }

    public void Die()
    {
        gameObject.GetComponentInChildren<ScoreTracker>().PlayerDied();
        dead = true;
    }

    public bool isVulnerable()
    {
        return currentState.vulnerable;
    }

    public bool isPaused
    {
        get
        {
            return paused;
        }
    }

}
