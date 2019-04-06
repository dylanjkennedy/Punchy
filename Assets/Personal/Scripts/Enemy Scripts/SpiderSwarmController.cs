using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class SpiderSwarmController : MonoBehaviour
{
    public enum SwarmState : int { Chase = 0, Scatter, Regroup };
    List<SpiderController> swarm;
    Vector3 centerOfMass;
    Vector3 CenterOfMass { get { return centerOfMass; } }
    Vector3 averageHeading;
    Vector3 AverageHeading { get { return averageHeading; } }
    NavMeshPath path;
    Vector3 goalPoint;
    [SerializeField] float lengthOfScatter;
    [SerializeField] float scatterLengthVariance;
    [SerializeField] GameObject player;
    float scatterTimer;
    float scatterTimeout;

    [SerializeField] public readonly float cohesionWeight;
    [SerializeField] public readonly float separationWeight;
    [SerializeField] public readonly float goalWeight;
    [SerializeField] public readonly float neighborRadius;

    SwarmState currentState;
    public SwarmState CurrentState { get { return currentState; } }

    // Start is called before the first frame update
    void Start()
    {
        swarm = this.GetComponentsInChildren<SpiderController>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case (SwarmState.Chase):
                {
                    centerOfMass = CalculateCenterOfMass();
                    NavMesh.CalculatePath(centerOfMass, player.transform.position, NavMesh.AllAreas, path);
                    foreach (SpiderController spider in swarm)
                    {
                        spider.SetHeading(path.corners[1], centerOfMass);
                    }
                    break;
                }
            case (SwarmState.Scatter):
                {
                    scatterTimer += Time.deltaTime;
                    if (scatterTimer >= scatterTimeout)
                    {
                        currentState = SwarmState.Regroup;
                    }
                    break;
                }
            case (SwarmState.Regroup):
                {
                    //what does regroup detection look like? maybe calculate center of mass and check max distance from CoM?
                    break;
                }
        }
    }

    public void Scatter()
    {
        currentState = SwarmState.Scatter;
        scatterTimer = 0;
        scatterTimeout = lengthOfScatter + Random.Range(-scatterLengthVariance, scatterLengthVariance);
    }

    private Vector3 CalculateCenterOfMass()
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;
        foreach(SpiderController spider in swarm)
        {
            x += spider.gameObject.transform.position.x;
            y += spider.gameObject.transform.position.y;
            z += spider.gameObject.transform.position.z;
        }
        return new Vector3(x / swarm.Count, y / swarm.Count, z / swarm.Count);
    }
}
