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
    [SerializeField] float runAwayDistance;
    [SerializeField] GameObject cylinder;
    [SerializeField] GameObject fractures;
    [SerializeField] int scoreValue;
    [SerializeField] float bulletForce;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] float fireTime;
    [SerializeField] float reevaluateTetherTime;
    //[SerializeField] float explodeRadius;
    //[SerializeField] float explodePower;
    //[SerializeField] float minPlayerDistance;
    //[SerializeField] float maxRunAwayDistance;

    Color defaultColor;
    Color fireColor = Color.magenta;
    Material material;
    private bool firing;
    private float nextFire;
    //private Projectile bulletScript;
    private Vector3 direction;
    private float stateTimer;
    private float fireTimer;
    private GameObject tether;
    private float tetherRadius;
    bool dead;
    float maxDeadTime = 3;
    private TetherManager tetherManager;
    EnemyAttacksManager.Token token;
    [SerializeField] bool runningAway;

    private enum enemyState { movingState, tetheredState };
    private enemyState state;

    // Use this for initialization
    protected override void Start()
    {
        player = GameObject.Find("Player");
        material = gameObject.GetComponent<MeshRenderer>().material;
        defaultColor = material.color;
        enemyAttacksManager = player.GetComponentInChildren<EnemyAttacksManager>();
        direction = player.transform.position - this.transform.position;
        tetherManager = Camera.main.gameObject.GetComponent<TetherManager>();
        //bulletScript = bullet.GetComponent<Projectile> ();
        type = SpawnManager.EnemyType.Cylinder;
        stateTimer = 0;
        nextFire = frequency + Random.Range(-frequencyRange, frequencyRange);
        nav = GetComponent<NavMeshAgent>();
        dead = false;
        runningAway = false;
        firing = false;
        fireTimer = 0;
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

        if (dead)
        {
            stateTimer += Time.unscaledDeltaTime;
            if (stateTimer >= maxDeadTime)
            {
                Destroy(this.gameObject);
            }
        }
        if (!dead && nav.enabled)
        {
            switch (state)
            {
                case enemyState.movingState:
                    state = MoveToTether();
                    break;
                case enemyState.tetheredState:
                    state = tetheredBehavior();
                    break;
                //case enemyState.attackingState:
                //    state = attackBehavior();
                //    break;
            }
            fireTimer += Time.fixedDeltaTime;
            if (!firing && fireTimer >= nextFire && CheckLineOfSight())
            {
                //only called with 0 for now, since only one attack type exists
                if (tryAttack(0))
                {
                    firing = true;
                    fireTimer = 0;
                }
            }

            if (firing)
            {
                material.color = Color.Lerp(defaultColor, fireColor, fireTimer / fireTime);
                if(fireTimer >= fireTime)
                {
                    Fire();
                }
            }
        }
    }

    private enemyState MoveToTether()
    {
        //PROBLEM: tether = this.findBestTether();
        // check if position is in radius of tether
        tetherRadius = tether.GetComponent<TetherController>().Radius;
        if (Vector3.Distance(tether.transform.position, this.transform.position) < tetherRadius)
        {
            stateTimer = 0;
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
        // if player gets too close re-evaluate or wait the appropriate amount of time to evaluate next tether
        if (playerTooClose || stateTimer >= reevaluateTetherTime)
        {
            tether = this.findBestTether();
            return enemyState.movingState;
        }

        // advance timer
        stateTimer += Time.fixedDeltaTime;
        return enemyState.tetheredState;
    }

    /*
    private enemyState attackBehavior()
    {
        //temporary
        transform.LookAt(player.transform);
        this.Fire();
        // JIGGLE
        //nav.SetDestination(this.findNewPositionInTether());
        // reconsider current tether
        GameObject newTether = this.findBestTether();
        if (newTether == tether)
        {
            nav.SetDestination(this.findNewPositionInTether());
            return enemyState.tetheredState;
        }
        else
        {
            tether = newTether;
            return enemyState.movingState;
        }
        
    }
    */

    private Vector3 findNewPositionInTether()
    {
        float x = Random.Range(tether.transform.position.x - tetherRadius, tether.transform.position.x + tetherRadius);
        float z = Random.Range(tether.transform.position.z - tetherRadius, tether.transform.position.z + tetherRadius);
        return new Vector3(x, tether.transform.position.y, z);
    }

    private bool tryAttack(int attackType)
    {
        //since there's only one attack type for now, this can only really be called with attackType = 0
        token = enemyAttacksManager.RequestToken(this.gameObject, attackType);
        return (token != null);
    }

    private void endAttack()
    {
        enemyAttacksManager.ReturnToken(this.type, token);
        token = null;
        firing = false;
        material.color = defaultColor;
        nextFire = frequency + Random.Range(-frequencyRange, frequencyRange);
        fireTimer = 0;
    }

    private void Fire()
    {
        transform.LookAt(player.gameObject.transform.position);
        GameObject bullet = Instantiate(bulletPrefab, this.transform.position, this.transform.rotation);
        bullet.GetComponent<Projectile>().Fire(this.transform.position, this.transform.forward, bulletSpeed, bulletDamage, bulletForce);

        endAttack();
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
            stateTimer = 0;
            cylinder.SetActive(false);
            Instantiate(fractures, transform.position, transform.rotation);
            Instantiate(explosion, point, transform.rotation);
            explosion.Play();

            Destroy(this.gameObject);
        }
    }

    private bool playerTooClose
    {
        get
        {
            return (Vector3.Distance(player.transform.position, this.transform.position) < runAwayDistance);
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