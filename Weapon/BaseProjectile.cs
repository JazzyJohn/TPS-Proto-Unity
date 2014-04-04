using UnityEngine;
using System.Collections;
//We don't want to our projectile fly infinite time
[RequireComponent (typeof (DelayDestroyedObject))]
public class BaseProjectile : UseObject {

	public float damage;
	
	public float speed = 100.0f;
	
	public GameObject owner;
	
	public GameObject Explode;
	
	public int splashRadius;

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
		//Debug.Log ("Trigger ENTER PROJ "+ this +  other);
		if (other.CompareTag ("decoration")) {
			Destroy (gameObject);
		}
		DamagebleObject obj =other.GetComponent <DamagebleObject>();
		if (obj != null) {
			obj.Damage(damage);
			Destroy (gameObject);
		}
		             
	}
	void OnDestroy() {
		if(splashRadius>0){
			 ExplosionDamage();		
		}
		if(Explode!=null){
			Instantiate(Explode,transform.position,transform.rotation);
		}
        
    }
	
	void ExplosionDamage() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, splashRadius);
       
        for(int i=0;i < hitColliders.Length;i++) {
			DamagebleObject obj = hitColliders[i].GetComponent <DamagebleObject>();
			if (obj != null) {
				obj.Damage(damage);
			}
        }
    }
}
