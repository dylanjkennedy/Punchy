using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollision : MonoBehaviour {

	private CapsuleCollider melee;
	private bool m_Melee;
	// Use this for initialization
	void Start () {
		melee = GetComponent<CapsuleCollider> ();
	}
	
	// Update is called once per frame
	void Update () {
		m_Melee = Input.GetButton ("Fire1");
	}

	void FixedUpdate() {
		if (m_Melee) {
			melee.enabled = true;
		} else {
			melee.enabled = false;
		}

	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Enemy") {
			Destroy (other.gameObject);
		}
	}

}
