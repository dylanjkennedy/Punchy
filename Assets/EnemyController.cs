using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private GameObject player;
	[SerializeField] int frequency;
	//private Projectile bulletScript;
	private Vector3 direction;
	private int timer;
	private Quaternion bulletRotation;

	// Use this for initialization
	void Start () {
		direction = player.transform.position - this.transform.position; 
		//bulletScript = bullet.GetComponent<Projectile> ();
		timer = 0;
		bulletRotation = new Quaternion (0f, -1.52f, -1.52f, 0f);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate() {
		if (timer >= frequency) {
			this.Fire ();
			timer = 0;
		}
		else {
			timer++;
		}

		transform.LookAt (player.transform);
	}

	void Fire () {
		//GameObject bullet = Instantiate (bulletPrefab, this.transform.position, bulletRotation);
		GameObject bullet = Instantiate (bulletPrefab, this.transform);

		bullet.GetComponent<Projectile> ().Fire (this.transform.position, this.transform.forward, 10f, 2);
	}
}
