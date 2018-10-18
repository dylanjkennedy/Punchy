using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestroyOverTime : MonoBehaviour {
	[SerializeField] float disappearTime;
	float timeAlive;
	[SerializeField] float fadeTime;
	Material material;
	Color fadeColor;
	float currentAlpha;
	float lerp;
	float timeFading;
	// Use this for initialization
	void Start () {
		timeAlive = 0;
        if (fadeTime > 0)
        {
            material = this.gameObject.GetComponent<MeshRenderer>().material;
            fadeColor = material.color;
        }
        timeFading = 0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timeAlive += Time.fixedDeltaTime;
		if (timeAlive >= disappearTime)
		{
			Destroy (this.gameObject);
		}

		if (disappearTime - timeAlive <= fadeTime && fadeTime > 0)
		{
			timeFading += Time.deltaTime;
			currentAlpha = 1 - (timeFading / fadeTime);
			fadeColor.a = currentAlpha;
			material.color = fadeColor;
		}
	}
}
