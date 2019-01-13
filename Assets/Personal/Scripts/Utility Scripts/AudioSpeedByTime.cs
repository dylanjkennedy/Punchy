using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpeedByTime : MonoBehaviour
{
    [SerializeField] TimeScaleManager timeScaleManager;
    [SerializeField] AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeScaleManager != null)
        {
            audioSource.pitch = timeScaleManager.Timescale;
        }
        else
        {
            Debug.LogWarning("Unassigned TimeScaleManager on " + this.gameObject + ". Please ensure the TimeScaleManager is passed in on initialization or is serialized.");
        }
    }

    public void AssignTimeScaleManager(TimeScaleManager manager)
    {
        timeScaleManager = manager;
    }
}
