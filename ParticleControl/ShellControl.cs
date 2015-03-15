using UnityEngine;
using System.Collections;

public class ShellControl : MonoBehaviour {
    public static int shellCount=0;
    public static int MAX_SHELL_COUNT = 30;
	float createTime =0.0f;
	public float colliderTimeOut = 0.5f;
	// Use this for initialization
	void OnEnable () {
		createTime = Time.time;
	}

    void OnDisable()
    {
        createTime = Time.time;
        shellCount--;
    }
	// Update is called once per frame
	void Update () {
		if (createTime + 0.5f < Time.time) {
			collider.enabled = true;
			enabled= false;
		}
	}
}
