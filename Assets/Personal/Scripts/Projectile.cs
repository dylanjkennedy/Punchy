using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	
	private int Damage;
	private Rigidbody rb;
	private int Duration = 6;
	private float Timer;
	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody> ();
		Timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		if (Timer >= Duration) {
			Destroy (this.gameObject);
		}
		Timer+= Time.fixedDeltaTime;
	}

	public void Fire (Vector3 position, Vector3 direction, float speed, int damage){
		rb.velocity = direction * speed;
		Damage = damage;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag != "Enemy" && other.gameObject.tag != "Hitbox") {
			if (other.gameObject.tag == "Player") {
				other.gameObject.GetComponent<PlayerHealth> ().TakeDamage (Damage);
			}
			Destroy (this.gameObject);
		}
	}
}
