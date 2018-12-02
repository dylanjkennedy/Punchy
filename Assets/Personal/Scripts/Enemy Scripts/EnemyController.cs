using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityScript.Steps;

public class EnemyController : MonoBehaviour
{
    protected EnemyAttacksManager enemyAttacksManager;
    protected GameObject player;
    protected NavMeshAgent nav;
    protected Rigidbody rb;
    protected SpawnManager.EnemyType type;

    // Use this for initialization
    protected virtual void Start()
    {
        player = GameObject.Find("Player");
        enemyAttacksManager = player.GetComponentInChildren<EnemyAttacksManager>();
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
        
    }

    protected virtual bool CheckLineOfSight()
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

    public SpawnManager.EnemyType Type
    {
        get
        {
            return type;
        }
    }

    protected virtual bool isVisible()
    {
        return this.GetComponent<MeshRenderer>().IsVisibleFrom(Camera.main);
    }
}