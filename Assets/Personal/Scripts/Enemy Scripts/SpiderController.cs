using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpiderController : EnemyController
{
    float neighborRadius;
    float cohesionWeight;
    float separationWeight;
    float goalWeight;
    bool frozen;
    Vector3 velocity;
    Vector3 heading;
    Collider wall;
    [SerializeField] CharacterController controller;
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
        if (!frozen && heading != Vector3.zero)
        {
            if (wall == null)
            {
                velocity += heading;
                velocity = velocity.normalized * maxSpeed;
                velocity.y -= 9.8f*Time.deltaTime;
                controller.Move(velocity * Time.deltaTime);
                //this.transform.position = this.transform.position + velocity*Time.deltaTime;
                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(heading), 1f);
            }
            else
            {
                //move up wall
                controller.Move(Vector3.up * maxSpeed * Time.deltaTime);
                Collider[] environmentColliders = Physics.OverlapSphere(transform.position, neighborRadius, LayerMask.GetMask("Default"));
                if (!environmentColliders.Contains(wall) && wall != null)
                {
                    {
                        Debug.Log("no longer on wall");
                        wall = null;
                        transform.rotation = Quaternion.LookRotation(heading, Vector3.up);
                    }
                }
            }
        }
    }

    public void SetHeading(Vector3 goal, Vector3 centerOfMass)
    {
        Vector3 goalVector = (goal - transform.position).normalized;
        Collider[] neighbors = GetNeighbors();
        Vector3 separationVector = Vector3.zero;
        Vector3 neighborCenter = Vector3.zero;
        foreach(Collider neighbor in neighbors)
        {
            separationVector.x += neighbor.transform.position.x - transform.position.x;
            separationVector.z += neighbor.transform.position.z - transform.position.z;
            neighborCenter.x += neighbor.transform.position.x;
            neighborCenter.z += neighbor.transform.position.z;
        }
        neighborCenter.x /= neighbors.Length;
        neighborCenter.z /= neighbors.Length;
        separationVector.x *= -1;
        separationVector.z *= -1;
        separationVector = Vector3.Normalize(separationVector);
        Vector3 cohesionVector = neighborCenter - transform.position;
        cohesionVector = Vector3.Normalize(cohesionVector);
        heading = separationVector * separationWeight + cohesionVector * cohesionWeight + goalVector * goalWeight;
        heading.y = 0f;
    }

    public void SetController(SpiderSwarmController controller)
    {
        swarmController = controller;
        neighborRadius = swarmController.NeighborRadius;
        cohesionWeight = swarmController.CohesionWeight;
        separationWeight = swarmController.SeparationWeight;
        goalWeight = swarmController.GoalWeight;
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

    void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (wall == null && collision.collider.gameObject.layer == LayerMask.NameToLayer("Default")) {
            Vector3 surfaceNormal = collision.normal;
            if (surfaceNormal != Vector3.up)
            {
                wall = collision.collider;
                transform.rotation = Quaternion.LookRotation(Vector3.up, surfaceNormal);
            }
        }
    }
    /*
    void OnCollisionExit(Collision collision)
    {
        Debug.Log(collision, collision.collider.gameObject);
        if (collision.collider == wall)
        {
            Debug.Log("no longer on wall");
            wall = null;
            transform.rotation = Quaternion.LookRotation(heading, Vector3.up);
        }
    }
    */
}
