﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityScript.Steps;
using UnityStandardAssets.Characters.ThirdPerson;

public class HumanoidEnemyController : EnemyController {
    
    //[SerializeField] GameObject fractures;
    [SerializeField] int scoreValue;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] float explodeRadius;
    [SerializeField] float explodePower;
    [SerializeField] float explodeTime;
    [SerializeField] float chaseDistance;
    [SerializeField] float minExplosionForce;
    [SerializeField] float explodeDamage;
    [SerializeField] float minExplosionDamage;

    ThirdPersonCharacter character;
    
	private float nextFire;
	private Vector3 direction;
	private float timer;
	bool dead;
    bool exploding;
    float maxDeadTime = 3;
    bool runningAway;
    bool frozen;

    Material material;
    

	// Use this for initialization
	public override void Start () {
        player = GameObject.Find("Player");
        character = gameObject.GetComponent<ThirdPersonCharacter>();
        material = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
        type = SpawnManager.EnemyType.Humanoid;
        //direction = player.transform.position - this.transform.position;
        //bulletScript = bullet.GetComponent<Projectile> ();
        timer = 0;
        nav = GetComponent<NavMeshAgent>();
        dead = false;
        rb = GetComponent<Rigidbody>();
        frozen = false;

        //base.Start();
	}
	
	// Update is called once per frame
	public override void Update () {

	}

	public override void FixedUpdate() {
		if (!dead && nav.enabled) {
            if (Vector3.Distance(player.transform.position, gameObject.transform.position) > chaseDistance && !exploding)
            {
                nav.SetDestination(player.transform.position);
                character.Move(nav.desiredVelocity, false, false);
            }
            else if (!exploding)
            {
                nav.SetDestination(gameObject.transform.position);
                character.Move(Vector3.zero, true, false);
                exploding = true;
            }
            else
            {
                character.Move(Vector3.zero, true, false);
                material.color = Color.Lerp(material.color, Color.red, 0.02f);
                timer += Time.deltaTime;
                if (timer >= explodeTime)
                {
                    Explode();
                }
            }
            /*
            if (nav.remainingDistance > nav.stoppingDistance)
                character.Move(nav.desiredVelocity, false, false);
            else
                character.Move(Vector3.zero, false, false);
                */

        }

        if (frozen)
        {
            character.Move(Vector3.zero, false, false);
        }

        if (dead)
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= maxDeadTime)
            {
                Destroy(this.gameObject);
            }
        }
	}
    
	public override void takeDamage(Vector3 point){
        if (!dead)
        {
            Camera.main.gameObject.GetComponent<ScoreManager>().changeScore(scoreValue);
            Explode();
        }
	}

	private void Explode()
	{
        dead = true;
        timer = 0;
        //Instantiate(fractures, transform.position, transform.rotation);
        DealExplosionDamage();
        DealExplosionPhysics();

        Instantiate(explosion, transform.position, transform.rotation);
        explosion.Play();
        Destroy(this.gameObject);
    }

    private void DealExplosionDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explodeRadius, LayerMask.GetMask("Enemy", "Player"));
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy") && hit.gameObject != this.gameObject)
            {
                hit.gameObject.GetComponent<EnemyController>().takeDamage(hit.gameObject.transform.position);
            }

            if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                float distanceRatio = 1 - Vector3.Distance(hit.gameObject.transform.position, transform.position)/explodeRadius;
                hit.gameObject.GetComponent<PlayerHealth>().TakeDamage( (int)(distanceRatio * explodeDamage), Vector3.Normalize(hit.gameObject.transform.position - transform.position), distanceRatio * explodePower);
            }
        }
    }

    public override void freeze()
    {
        nav.enabled = false;
        frozen = true;
        character.Freeze(); 
    }

    private void DealExplosionPhysics()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explodeRadius, LayerMask.GetMask("Debris"));
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explodePower, transform.position, explodeRadius, 0F, ForceMode.Impulse);
            }
        }
    }
}
