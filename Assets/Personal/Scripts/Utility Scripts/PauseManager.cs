using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    PlayerValues playerValues;
    GameObject PauseMenu;

    private UnityAction<string> pauseListener;

    private void Awake()
    {
        pauseListener = new UnityAction<string>(TogglePause);
    }

    private void OnEnable()
    {
        EventManager.StartListening("pause", pauseListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening("pause", pauseListener);
    }

    private void Start()
    {
        playerValues = transform.parent.gameObject.GetComponent<PlayerValues>();
        PauseMenu = playerValues.generalValues.PauseMenu;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            EventManager.TriggerEvent("pause");
        }
    }

    private void TogglePause(string data)
    {
        if (PauseMenu.activeSelf)
        {
            PauseMenu.SetActive(false);
        }
        else
        {
            PauseMenu.SetActive(true);
        }
    }
}
