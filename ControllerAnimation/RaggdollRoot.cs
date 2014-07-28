using UnityEngine;
using System.Collections;

public class RaggdollRoot : MonoBehaviour {

    public Rigidbody pawnTransform;

    private Transform myTransform;

    private Vector3 deffVect;
	// Use this for initialization
	void Awake () {
        myTransform = transform;
        this.enabled = false;
	}
    void OnEnable()
    {
        //deffVect = myTransform.localPosition;
    }
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 cahcechPos = myTransform.position;
     

        pawnTransform.MovePosition(cahcechPos);
	}
    
}
