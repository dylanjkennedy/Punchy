using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : EnemyController
{
    float neighborRadius;
    float cohesionWeight;
    float separationWeight;
    float goalWeight;
    bool frozen;
    Vector3 heading;
    [SerializeField] SpiderSwarmController swarmController;
    [SerializeField] float maxSpeed;
    [SerializeField] Rigidbody rb;

    // Start is called before the first frame update
    protected override void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!frozen)
        {
            rb.velocity = maxSpeed * heading;
        }
    }

    public void SetHeading(Vector3 goal, Vector3 centerOfMass)
    {
        Vector3 goalVector = (goal - transform.position).normalized;
        Collider[] neighbors = GetNeighbors();
        Vector3 separationVector = Vector3.zero;
        foreach(Collider neighbor in neighbors)
        {
            separationVector.x += transform.position.x - neighbor.transform.position.x;
            separationVector.y += transform.position.y - neighbor.transform.position.y;
            separationVector.z += transform.position.z - neighbor.transform.position.z;
        }
        Vector3.Normalize(separationVector);
        Vector3 cohesionVector = goal - transform.position;
        heading = Vector3.Normalize(separationVector * separationWeight + cohesionVector * cohesionWeight + goalVector * goalWeight);
    }

    public void SetController(SpiderSwarmController controller)
    {
        swarmController = controller;
        neighborRadius = swarmController.neighborRadius;
        cohesionWeight = swarmController.cohesionWeight;
        separationWeight = swarmController.separationWeight;
        goalWeight = swarmController.goalWeight;
    }

    private Collider[] GetNeighbors()
    {
        return Physics.OverlapSphere(transform.position, neighborRadius, LayerMask.GetMask("Spiders"));
    }

    public override void freeze()
    {
        rb.velocity = Vector3.zero;
        frozen = true;
    }
}
