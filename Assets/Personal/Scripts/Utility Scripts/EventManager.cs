using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//See below link for details on how to use this to pass gave objects and unlimited # of parameters
//currently the strings actually just behave as strings, but this can be used for unlimited parameters with JSON parse of serialized object
//https://forum.unity.com/threads/messaging-system-passing-parameters-with-the-event.331284/#post-3518881


[System.Serializable]
public class ThisEvent : UnityEvent<string>
{

}

public class EventManager : MonoBehaviour
{

    private Dictionary<string, ThisEvent> eventDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, ThisEvent>();
        }
    }

    public static void StartListening (string eventName, UnityAction<string> listener)
    {
        ThisEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new ThisEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening( string eventName, UnityAction<string> listener)
    {
        if (eventManager == null) return;
        ThisEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent (string eventName)
    {
        ThisEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(null);
        }
    }

    public static void TriggerEvent(string eventName, string data)
    {
        ThisEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(data);
        }
    }
}
