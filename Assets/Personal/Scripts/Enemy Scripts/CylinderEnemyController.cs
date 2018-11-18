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
    private float tetherRadius;
    bool dead;
    float maxDeadTime = 3;
    private TetherManager tetherManager;
    [SerializeField] bool runningAway;

    private enum enemyState { movingState, tetheredState, attackingState };
    private enemyState state;

    // Use this for initialization
    protected override void Start()
    {
        player = GameObject.Find("Player");
        direction = player.transform.position - this.transform.position;
        tetherManager = Camera.main.gameObject.GetComponent<TetherManager>();
        //bulletScript = bullet.GetComponent<Projectile> ();
        type = SpawnManager.EnemyType.Cylinder;
        timer = 0;
        nextFire = frequency + Random.Range(-frequencyRange, frequencyRange);
        nav = GetComponent<NavMeshAgent>();
        dead = false;
        runningAway = false;
        rb = GetComponent<Rigidbody>();

        tether = this.findBestTether();
        state = enemyState.movingState;
    }

    // Update is called once per frame
    protected override void Update()
    {
    }

    protected override void FixedUpdate()
    {
        // state machine/swiiitch
        // need enumerable
        // moving to tether
        // in tether
        // attacking
        // escape! ?

        // move to the best tether you can
        // attack the player while your in a tether
        // reevaluate your tether (every once in a while and when player too close)

        
        
        switch (state) {
            case enemyState.movingState:
                state = MoveToTether();
                return;
            case enemyState.tetheredState:
                state = tetheredBehavior();
                return;
            case enemyState.attackingState:
                state = attackBehavior();
                return;
        }

     
        /*
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
        */
    }

    private enemyState MoveToTether()
    {
        //PROBLEM: tether = this.findBestTether();
        // check if position is in radius of tether
        tetherRadius = tether.GetComponent<TetherController>().Radius;
        if (Vector3.Distance(tether.transform.position, this.transform.position) < tetherRadius)
        {
            return enemyState.tetheredState;
        }
        else
        {
            nav.SetDestination(tether.transform.position);
            return enemyState.movingState;        
        }      
    }

    private enemyState tetheredBehavior()
    {
        // if player gets too close re-evaluate
        if (Vector3.Distance(player.transform.position, this.transform.position) < runAwayDistance)
        {
            tether = this.findBestTether();
            return enemyState.movingState;
        }

        // wait the appropriate amount of time to attack
        if (timer >= nextFire && CheckLineOfSight())
        {
            return enemyState.attackingState;
        }
        else
        {
            timer += Time.fixedDeltaTime;
            return enemyState.tetheredState;
        }
        
    }

    private enemyState attackBehavior()
    {
        this.Fire();
        // JIGGLE
        //nav.SetDestination(this.findNewPositionInTether());
        // reconsider current tether
        tether = this.findBestTether();
        return enemyState.movingState;
    }

    private Vector3 findNewPositionInTether()
    {
        float x = Random.Range(tether.transform.position.x - tetherRadius, tether.transform.position.x + tetherRadius);
        float z = Random.Range(tether.transform.position.z - tetherRadius, tether.transform.position.z + tetherRadius);
        return new Vector3(x, tether.transform.position.y, z);
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

    private GameObject findBestTether()
    {
        //4 data points to weigh
        //Sight Ratio (visibility)
        //Distance from player - probably want around 20 units of distance
        //Distance from tether
        //Current occupants of tether

        TetherController[] tethers = tetherManager.Tethers;
        int[] weights = new int[tethers.Length];
        int minWeightIndex = -1;
        int minWeight = int.MaxValue;

        for (int i = 0; i < tethers.Length; i++)
        {
            weights[i] += 10*tethers[i].Occupants^2;
            //weight distance from tether to AI as distance/4
            weights[i] += (int)(Vector3.Distance(tethers[i].gameObject.transform.position, gameObject.transform.position)/4);

            //We want distance from tether to player to be 20, so weight the difference of actual distance from that by difference*2
            weights[i] += (int)(Mathf.Abs(
                Vector3.Distance(tethers[i].gameObject.transform.position, player.transform.position) - 20))*2;

            //Weight inverse of trace ratio at inverseRatio*50. Perfect visibility weights 0, no visibility weights 50;
            weights[i] += (int)((1 - tethers[i].TraceRatio) * 50);

            if (weights[i] < minWeight)
            {
                minWeightIndex = i;
                minWeight = weights[i];
            }
        }

        return tethers[minWeightIndex].gameObject;
    }
}