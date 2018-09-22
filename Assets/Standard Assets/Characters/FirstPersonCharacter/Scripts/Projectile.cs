using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	
	private int Damage;
	private Rigidbody rb;
	private int Duration = 360;
	private int Time;
	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody> ();
		Time = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		if (Time >= Duration) {
			Destroy (this.gameObject);
		}
		Time++;
	}

	public void Fire (Vector3 position, Vector3 direction, float speed, int damage){
		//this.transform.position = position;
		rb.velocity = direction * speed;
		Damage = damage;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag != "Enemy" && other.gameObject.tag != "Hitbox") {
			Destroy (this.gameObject);
		}
	}
}
