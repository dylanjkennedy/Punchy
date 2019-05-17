using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityScript.Steps;

public abstract class EnemyController : MonoBehaviour
{
    [SerializeField] protected EnemyAttackTokenPool enemyAttackTokenPool;
    [SerializeField] protected GameObject player;
    [SerializeField] protected NavMeshAgent nav;
    [SerializeField] protected SpawnManager spawnManager;
    [SerializeField] protected Camera playerCamera;

    protected SpawnManager.EnemyType type;
    protected CharacterController enemyMover;
    protected Vector3 old_velocity;
    protected ImpactReceiver impacter;
    protected EnemyValues enemyValues;
    protected float impactToKill;
    protected float health;
    protected float knockbackModifier;

    // Use this for initialization
    protected virtual void Start()
    {
        enemyMover = gameObject.GetComponent<CharacterController>();
        impacter = gameObject.GetComponent<ImpactReceiver>();
        enemyValues = gameObject.GetComponent<EnemyValues>();
        impactToKill = enemyValues.generalValues.ImpactToKill;
        health = enemyValues.generalValues.HealthValue;
        knockbackModifier = enemyValues.generalValues.KnockbackModifier;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
        old_velocity = enemyMover.velocity;
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

    public virtual void takeDamage(Vector3 direction)
    {
        health--;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            impacter.AddImpact(direction, knockbackModifier);
        }
    }

    public virtual void Die()
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

    protected virtual void DestroyThis()
    {
        spawnManager.DestroyEnemy(this.gameObject);
    }

    protected virtual void KnockbackUpdate()
    {
        if (enemyMover.velocity.magnitude <= 1
            && enemyMover.velocity.y <= 0
            && enemyMover.isGrounded)
        {
            nav.enabled = true;
        }
    }

    protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float impact = -Vector3.Dot(hit.normal, old_velocity);
        if (impact > impactToKill)
        {
            if (hit.gameObject.layer == 9)
            {
                hit.gameObject.GetComponent<EnemyController>().takeDamage(impact * hit.moveDirection);
            }
            takeDamage(impact * hit.normal);
        }
        else
        {
            impacter.Reflect(hit.normal);
        }
    }
}