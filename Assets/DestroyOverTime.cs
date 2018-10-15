using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour {
	[SerializeField] float disappearTime;
	float timeAlive;
	// Use this for initialization
	void Start () {
		timeAlive = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timeAlive += Time.fixedDeltaTime;
		if (timeAlive >= disappearTime)
		{
			Destroy (this.gameObject);
		}
	}
}
