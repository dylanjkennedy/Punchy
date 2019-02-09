using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	
	private int damage;
	private Rigidbody rb;
	private int Duration = 6;
	private float timer;
    private float force;
    [SerializeField] private float gravity;
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
        Vector3 newVelocity = new Vector3(rb.velocity.x, rb.velocity.y - gravity * Time.fixedDeltaTime, rb.velocity.z);
        rb.velocity = newVelocity;
	}

	public void Fire (Vector3 cylinderPosition, Vector3 playerPosition, float speed, int damage, float force){
        Vector3 unitDirection = (playerPosition - cylinderPosition).normalized;
        Vector3 velocityWithoutArc = unitDirection * speed;
        float distance = Vector3.Distance(cylinderPosition, playerPosition);
        float timeToPlayer = distance / speed;
        float initialYVelocity = gravity * timeToPlayer/2f;
        Vector3 desiredVelocity = new Vector3(velocityWithoutArc.x, velocityWithoutArc.y + initialYVelocity, velocityWithoutArc.z);
        rb.velocity = desiredVelocity;
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
