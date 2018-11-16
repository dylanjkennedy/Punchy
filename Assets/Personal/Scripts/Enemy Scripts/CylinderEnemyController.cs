using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityScript.Steps;

public class CylinderEnemyController : EnemyController
{

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] float frequency;
    [SerializeField] float frequencyRange;
    [SerializeField] float bulletSpeed;
    [SerializeField] int bulletDamage;
    [SerializeField] float minPlayerDistance;
    [SerializeField] float runAwayDistance;
    [SerializeField] float maxRunAwayDistance;
    [SerializeField] GameObject cylinder;
    [SerializeField] GameObject fractures;
    [SerializeField] int scoreValue;
    [SerializeField] float bulletForce;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] float explodeRadius;
    [SerializeField] float explodePower;

    private float nextFire;
    //private Projectile bulletScript;
    private Vector3 direction;
    private float timer;
    private GameObject tether;
    bool dead;
    float maxDeadTime = 3;
    [SerializeField] bool runningAway;

    // Use this for initialization
    protected override void Start()
    {
        player = GameObject.Find("Player");
        direction = player.transform.position - this.transform.position;
        //bulletScript = bullet.GetComponent<Projectile> ();
        type = SpawnManager.EnemyType.Cylinder;
        timer = 0;
        nextFire = frequency + Random.Range(-frequencyRange, frequencyRange);
        nav = GetComponent<NavMeshAgent>();
        dead = false;
        runningAway = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected override void Update()
    {
    }

    protected override void FixedUpdate()
    {
        if (!dead && nav.enabled)
        {

            transform.LookAt(player.transform);


            if (timer >= nextFire && CheckLineOfSight())
            {
                this.Fire();
            }
            else
            {
                timer += Time.fixedDeltaTime;
            }

            transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);

            float distance = Vector3.Distance(gameObject.transform.position, player.transform.position);

            if (distance < runAwayDistance)
            {
                runningAway = true;
            }
            if (distance >= maxRunAwayDistance)
            {
                runningAway = false;
            }

            if (runningAway)
            {
                nav.SetDestination(gameObject.transform.position + Vector3.Normalize(gameObject.transform.position - player.transform.position) * 2);
            }
            else if (distance > minPlayerDistance)
            {
                nav.SetDestination(player.transform.position);
            }
            else
            {
                nav.SetDestination(gameObject.transform.position);
            }
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

    protected virtual void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, this.transform.position, this.transform.rotation);

        bullet.GetComponent<Projectile>().Fire(this.transform.position, this.transform.forward, bulletSpeed, bulletDamage, bulletForce);
        nextFire = frequency + Random.Range(-frequencyRange, frequencyRange);
        timer = 0;
    }

    protected override bool CheckLineOfSight()
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

    public override void takeDamage(Vector3 point)
    {
        if (!dead)
        {
            Camera.main.gameObject.GetComponent<ScoreManager>().changeScore(scoreValue);
            dead = true;
            timer = 0;
            cylinder.SetActive(false);
            Instantiate(fractures, transform.position, transform.rotation);
            Instantiate(explosion, point, transform.rotation);
            explosion.Play();

            Destroy(this.gameObject);
        }
    }

    public override void freeze()
    {
        nav.enabled = false;
    }
}