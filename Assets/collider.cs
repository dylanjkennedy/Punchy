using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collider : MonoBehaviour {

	void onTriggerEnter(collider col){
		Debug.Log ("enter");
	}

	void onTriggerStay(collider col){
		Debug.Log ("stay");
	}

	void onTriggerExit(collider col){
		Debug.Log ("Exit");
	}
}
