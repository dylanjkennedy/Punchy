using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	// Use this for initialization
	void Start () {
		direction = player.transform.position - this.transform.position; 
		//bulletScript = bullet.GetComponent<Projectile> ();
		timer = 0;
		nextFire = frequency + Random.Range (-frequencyRange, frequencyRange);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate() {
		transform.LookAt (player.transform);


		if (timer >= nextFire && CheckLineOfSight()) {
			this.Fire ();
		}
		else {
			timer+= Time.fixedDeltaTime;
		}

		transform.rotation = new Quaternion (0, transform.rotation.y, 0, transform.rotation.w);
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
}
