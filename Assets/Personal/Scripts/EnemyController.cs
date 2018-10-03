using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private GameObject player;
	[SerializeField] float frequency;
	[SerializeField] float frequencyRange;
	[SerializeField] float bulletSpeed;
	[SerializeField] int bulletDamage;
	private float nextFire;
	//private Projectile bulletScript;
	private Vector3 direction;
	private float timer;
	bool dead;

	NavMeshAgent nav;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		direction = player.transform.position - this.transform.position; 
		//bulletScript = bullet.GetComponent<Projectile> ();
		timer = 0;
		nextFire = frequency + Random.Range (-frequencyRange, frequencyRange);
		nav = GetComponent<NavMeshAgent> ();
		dead = false;
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate() {
		if (!dead) {
			
			transform.LookAt (player.transform);


			if (timer >= nextFire && CheckLineOfSight ()) {
				this.Fire ();
			} else {
				timer += Time.fixedDeltaTime;
			}

			transform.rotation = new Quaternion (0, transform.rotation.y, 0, transform.rotation.w);

			nav.SetDestination (player.transform.position);
		}
	}

	void Fire () {
		GameObject bullet = Instantiate (bulletPrefab, this.transform.position, this.transform.rotation);

		bullet.GetComponent<Projectile> ().Fire (this.transform.position, this.transform.forward, bulletSpeed, bulletDamage);
		nextFire = frequency + Random.Range (-frequencyRange, frequencyRange);
		timer = 0;
	}

	bool CheckLineOfSight() {
		RaycastHit seePlayer;
		Ray ray = new Ray(transform.position, player.transform.position - transform.position);

		if (Physics.Raycast (ray, out seePlayer, Mathf.Infinity)) {
			if (seePlayer.collider.gameObject.CompareTag ("Player")) {
				return true;
			}
		}
		return false;
	}

	public void takeDamage(){
		dead = true;
		rb.isKinematic = false;
		rb.useGravity = true;
		nav.enabled = false;
		rb.AddForceAtPosition (Vector3.Normalize (transform.position - player.transform.position)*50, rb.transform.position + new Vector3 (Random.Range(-1,1), Random.Range(-1,1), Random.Range(-1,1)), ForceMode.Impulse);
	}
}
