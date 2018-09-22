using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collidings : MonoBehaviour {

	void OnCollisionEnter(Collision col){
		Debug.Log ("enter");
	}

	void OnCollisionStay(Collision col){
		Debug.Log ("stay");
	}

	void OnCollisionExit(Collision col){
		Debug.Log ("Exit");
	}
}
