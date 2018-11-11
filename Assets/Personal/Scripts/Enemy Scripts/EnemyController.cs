using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityScript.Steps;

public class EnemyController : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent nav;
    public Rigidbody rb;
    public SpawnManager.EnemyType type;

    // Use this for initialization
    public virtual void Start()
    {
        player = GameObject.Find("Player");
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
        
    }

    public virtual bool CheckLineOfSight()
    {
        RaycastHit seePlayer;
        Ray ray = new Ray(transform.position, player.transform.position - transform.position);

        if (Physics.Raycast(ray, out seePlayer, Mathf.Infinity))
        {
            if (seePlayer.collider.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    public virtual void takeDamage(Vector3 point)
    {
        Destroy(this.gameObject);
    }

    public virtual void freeze()
    {
        nav.enabled = false;
    }
}