using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseButton : MonoBehaviour
{
    public void Pause()
    {
        EventManager.TriggerEvent("pause");
    }
}
