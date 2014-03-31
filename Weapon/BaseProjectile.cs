using UnityEngine;
using System.Collections;

public class BaseProjectile : MonoBehaviour {

	public float damage;
	public float speed = 100.0f;

	public GameObject owner;

	private Transform mTransform;
	// Use this for initialization
	void Start () {
		mTransform = transform;
	}
	
	// Update is called once per frame

	void FixedUpdate() {
		Vector3 distance = mTransform.forward * Time.fixedDeltaTime * speed;
		rigidbody.MovePosition(rigidbody.position + distance);
	}
	void OnCollisionEnter(Collision collision) {
		//Debug.Log ("COLLISION ENTER PROJ "+ this + collision);
	}
	void OnTriggerEnter	(Collider other) {
		if (owner == other.gameObject) {
			return;
		}
		Debug.Log ("Trigger ENTER PROJ "+ this +  other);
		if (other.CompareTag ("decoration")) {
			Destroy (gameObject);
		}
		DamagebleObject obj = (DamagebleObject)other.GetComponent (typeof(DamagebleObject));
		if (obj != null) {
			obj.Damage(damage);
			Destroy (gameObject);
		}
		             
	}
}
