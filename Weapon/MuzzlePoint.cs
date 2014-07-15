using UnityEngine;
using System.Collections;

public class MuzzlePoint : MonoBehaviour {


    public Transform myTransform;
    public Transform damager;
    public Vector3 latePosition;
    public Quaternion lateRotation;
	// Use this for initialization
    void Awake()
    {
        myTransform = transform;
        damager.transform.parent = null;
    }
	void Start () {
	
	}
    void OnDestroy() {
        Destroy(damager.gameObject);
    }
	// Update is called once per frame
	void LateUpdate () {
        damager.position = myTransform.position;
        damager.rotation = myTransform.rotation;
      //  Debug.DrawLine(this.transform.position, Vector3.zero, Color.blue, 5.0f);
	}
 
  
}
