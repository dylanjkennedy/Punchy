using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	
	private int Damage;
	private Rigidbody rb;
	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Fire (Vector3 position, Vector3 direction, float speed, int damage){
		//this.transform.position = position;
		rb.velocity = direction * speed;
		Damage = damage;

		Debug.Log ("Position is" + this.transform.position);
		Debug.Log ("position should be: " + position);
	}
}
