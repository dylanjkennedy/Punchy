using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson; //FirstPersonController made this the namespace
using UnityEngine.SceneManagement;

public class PlayerMover : MonoBehaviour
{
	[SerializeField] public float speed;
	[SerializeField] public float stickToGroundForce;
	private CharacterController characterController;
    private ChargeController chargeController;
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
	[SerializeField] public float jumpSpeed;
    [SerializeField] private MouseLook mouseLook;
    public MouseLook MouseLook
    {
        get { return mouseLook; }
    }
    private Camera playerCamera;
    bool dead;



    // Use this for initialization
    void Start () {
        playerCamera = GetComponentInChildren<Camera>();
		characterController = GetComponent<CharacterController>();
        MouseLook.Init(transform, playerCamera.transform);
        currentState = new GroundState(this);
        dead = false;
        chargeController = GetComponent<ChargeController>();
        playerStamina = GetComponent<PlayerStamina>();
    }


	
	// Update is called once per frame
	void FixedUpdate ()
	{
        if (!dead)
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
        if (!dead)
        {
            currentState.Update();
        }
        else
        {
            if (Input.GetButtonDown("Submit"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            }
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
        dead = true;
    }

    public bool isVulnerable()
    {
        return currentState.vulnerable;
    }

}
