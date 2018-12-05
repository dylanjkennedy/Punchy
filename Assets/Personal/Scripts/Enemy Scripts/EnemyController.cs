using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityScript.Steps;

public abstract class EnemyController : MonoBehaviour
{
    [SerializeField] protected EnemyAttacksManager enemyAttacksManager;
    [SerializeField] protected GameObject player;
    [SerializeField] protected NavMeshAgent nav;
    protected SpawnManager.EnemyType type;
    [SerializeField] protected Camera playerCamera;

    // Use this for initialization
    protected virtual void Start()
    {
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
        return this.GetComponent<MeshRenderer>().isVisibleFrom(Camera.main);
    }
}