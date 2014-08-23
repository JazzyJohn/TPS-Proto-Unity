using UnityEngine;
using System.Collections;

public class QueenEgg : MonoBehaviour {
    public float delay=2.0f;

    public bool ready=false;


	// Use this for initialization
	void Start () {
        Invoke("Ready", delay);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void Ready()
    {
        ready = true;
    }
}
