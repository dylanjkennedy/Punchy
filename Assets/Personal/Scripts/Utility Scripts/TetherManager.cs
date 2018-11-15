using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherManager : MonoBehaviour {
    [SerializeField] int traceUpdatesPerFrame;

    List<TetherController> tethers = new List<TetherController>();
    int currentTether = 0;
    int currentTrace = 0;
    int tracesThisFrame;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        tracesThisFrame = 0;
        if (tethers.Count == 0)
        {
            return;
        }
		while (traceUpdatesPerFrame > tracesThisFrame)
        {
            tethers[currentTether].updateTrace(currentTrace);
            currentTether++;

            if (currentTether >= tethers.Count)
            {
                currentTether = 0;
                currentTrace++;
                if (currentTrace >= 13)
                {
                    currentTrace = 0;
                }
            }

            tracesThisFrame++;
        }
	}

    public void addTether(TetherController tether)
    {
        tethers.Add(tether);
    }
}
