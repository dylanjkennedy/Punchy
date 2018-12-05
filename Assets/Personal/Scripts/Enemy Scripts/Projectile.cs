using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	
	private int damage;
	private Rigidbody rb;
	private int Duration = 6;
	private float timer;
    private float force;
	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody> ();
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		if (timer >= Duration) {
			Destroy (this.gameObject);
		}
		timer+= Time.fixedDeltaTime;
	}

	public void Fire (Vector3 position, Vector3 direction, float speed, int damage, float force){
		rb.velocity = direction * speed;
		this.damage = damage;
        this.force = force;
        //bit of a stopgap cuz I don't really know how shaders work yet
        this.transform.Rotate(0,90,0);
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag != "Enemy" && other.gameObject.tag != "Hitbox") {
			if (other.gameObject.tag == "Player") {
				other.gameObject.GetComponent<PlayerHealth> ().TakeDamage (damage, rb.velocity.normalized, force);
			}
			Destroy (this.gameObject);
		}
	}
}
