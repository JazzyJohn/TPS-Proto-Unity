using UnityEngine;
using System.Collections;

public class RaggdollRoot : MonoBehaviour {

    public Rigidbody pawnTransform;

    private Transform myTransform;
	
	private Rigidbody[] allrigidbody;

    private Vector3 deffVect;
	// Use this for initialization
	void Awake () {
        myTransform = transform;
       
		allrigidbody = GetComponentsInChildren<Rigidbody>();
        Stop();
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
	public void Start(){
		enabled = true;
		foreach(Rigidbody rigid in allrigidbody){
			rigid.isKinematic =false;
		}
	}
	public void Stop(){
		enabled = false;
		foreach(Rigidbody rigid in allrigidbody){
			rigid.isKinematic =true;
		}
	}
    
}
