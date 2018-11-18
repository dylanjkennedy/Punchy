using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherManager : MonoBehaviour {
    [SerializeField] int traceUpdatesPerFrame;

    List<TetherController> tethers = new List<TetherController>();
    List<float> tetherTraceRatios = new List<float>();
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
            currentTrace++;

            if (currentTrace >= tethers[currentTether].numTraces)
            {
                currentTrace = 0;
                currentTether++;
                if (currentTether >= tethers.Count)
                {
                    currentTether = 0;
                }
            }

            tracesThisFrame++;
        }
	}

    public float[] TraceRatios
    {
        get
        {
            return tetherTraceRatios.ToArray();
        }
    }

    public TetherController[] Tethers
    {
        get
        {
            return tethers.ToArray();
        }
    }

    /*
    public GameObject FindBestTether(GameObject Enemy)
    {
        float[] weights = new float[tethers.Count];
        int max = 0;
        for (int i = 0; i < tethers.Count; i++)
        {

        }
        return tethers[max].gameObject;
    }
    */

    public void addTether(TetherController tether)
    {
        tethers.Add(tether);
        tetherTraceRatios.Add(tether.TraceRatio);
    }
}
