using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerState
{
    private PlayerMover playermover;
    private MouseLook mouseLook;
    private Camera cam;
    public PlayerState(PlayerMover pm)
    {
        playermover = pm;
        mouseLook = playermover.mouseLook;
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
    public virtual void MouseLookFixedUpdate()
    {
        Debug.Log("MouseLookFixedUpdate");
        mouseLook.UpdateCursorLock();
    }
    public virtual void MouseLookUpdate()
    {
        Debug.Log("MouseLookUpdate");
        RotateView();
    }

    private void RotateView()
    {
        mouseLook.LookRotation(playermover.transform, cam.transform);
    }

}
