using UnityEngine;
using System;


public class DelayDestroyedObject : MonoBehaviour {

	public float timeLimit=10.0f;
	
	void Start () {
		Destroy(gameObject,timeLimit);
		
	}

}