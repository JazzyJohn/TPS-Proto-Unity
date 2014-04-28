using UnityEngine;
using System.Collections;

public class ShellControl : MonoBehaviour {

	float createTime =0.0f;
	public float colliderTimeOut = 0.5f;
	// Use this for initialization
	void Awake () {
		createTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (createTime + 0.5f < Time.time) {
			collider.enabled = true;
			Destroy(this);
		}
	}
}
