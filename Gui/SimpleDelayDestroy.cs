using UnityEngine;
using System.Collections;

public class SimpleDelayDestroy : MonoBehaviour {


    public float delay;

	// Use this for initialization
	void Start () {
        if (this.enabled)
        {
            Invoke("Destroy", delay);
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void Destroy()
    {
        Destroy(gameObject);
    }
}
