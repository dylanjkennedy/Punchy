using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson; //FirstPersonController made this the namespace


public class PlayerMover : MonoBehaviour
{

    public PlayerState currentState;
    [SerializeField] private MouseLook _mouseLook;
    public MouseLook mouseLook
    {
        get { return _mouseLook; }
    }

    private Camera m_Camera;

    // Use this for initialization
    void Start () {
        m_Camera = Camera.main;
        mouseLook.Init(transform, m_Camera.transform);
        currentState = new PlayerState(this);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    PlayerState newState = currentState.FixedUpdate();
	    if (newState != null)
	    {
	        currentState.Exit();
	        newState.Enter();
	        currentState = newState;
	    }
	}

    void Update()
    {
        currentState.Update();
    }

}
