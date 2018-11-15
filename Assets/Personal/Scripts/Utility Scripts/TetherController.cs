using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherController : MonoBehaviour {

    Vector3[] tracePositions = { new Vector3 (0,0,0), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 0, -1),
        new Vector3(2, 0, 0), new Vector3(0, 0, -2), new Vector3(0, 0, -1), new Vector3(0, 0, 1), new Vector3(0, 0, 2),
        new Vector3(-1, 0, -1), new Vector3(-1, 0, 0), new Vector3(-1, 0, 1), new Vector3(-2, 0, 0) };

    bool[] traceCanSeePlayer = new bool[13];

    LayerMask mask;

    TetherManager tetherManager;
    GameObject player;

    // Use this for initialization
    void Awake() {
        //subject to change
        tetherManager = Camera.main.gameObject.GetComponent<TetherManager>();
        tetherManager.addTether(this);

        player = GameObject.Find("Player");


        //create mask to ignore these layers, as player visibility should not be affected by these factors
        mask = LayerMask.GetMask("Projectiles","Debris","TransparentFX","UI");
        mask = ~mask;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        for (int i = 0; i < traceCanSeePlayer.Length; i++)
        {
            if (traceCanSeePlayer[i])
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireSphere(gameObject.transform.position + tracePositions[i] - new Vector3(0, 2, 0), 0.1f);
        }
    }

    public float updateTrace(int traceNum)
    {
        traceCanSeePlayer[traceNum] = CheckLineOfSight(traceNum);
        return 0;
    }

    bool CheckLineOfSight(int traceNum)
    {
        RaycastHit seePlayer;
        Ray ray = new Ray(transform.position + tracePositions[traceNum], player.transform.position - (transform.position + tracePositions[traceNum]));

        if (Physics.Raycast(ray, out seePlayer, Mathf.Infinity, mask))
        {
            if (seePlayer.collider.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}
