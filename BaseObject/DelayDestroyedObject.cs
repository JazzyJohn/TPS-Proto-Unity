using UnityEngine;
using System;


public class DelayDestroyedObject : MonoBehaviour {

	public float timeLimit=10.0f;
	
	void OnEnable()
	{
		  Invoke("DeActivate", timeLimit);
	}
	void OnDisable()
	{
		CancelInvoke();
	}
	public void DeActivate(){
		gameObject.Recycle();
	}
}