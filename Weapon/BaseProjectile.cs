using UnityEngine;
using System.Collections;

//We don't want to our projectile fly infinite time
[RequireComponent (typeof (DelayDestroyedObject))]
public class BaseProjectile : UseObject {
	
	public float damage;
	public float startImpulse;
	public GameObject owner;
	public GameObject hitParticle;
	public int splashRadius;
	
	private Transform mTransform;
	private Rigidbody mRigidBody;
	
	void Start () {
		mTransform = transform;
		mRigidBody = rigidbody;
		mRigidBody.velocity = mTransform.TransformDirection(Vector3.forward * startImpulse);
		
		RaycastHit hit;
		
		if (Physics.Raycast (transform.position, mRigidBody.velocity.normalized, out hit)){
			
			if (hit.distance < mTransform.InverseTransformDirection (mRigidBody.velocity).z * 0.1f)
			{
				onBulletHit(hit);
			}
		}
	}
	
	void Update() {
		
		RaycastHit hit;
		mTransform.rotation = Quaternion.LookRotation ( mRigidBody.velocity);
		if (Physics.Raycast (transform.position, mRigidBody.velocity.normalized, out hit)){
			
			if (hit.distance < mTransform.InverseTransformDirection (mRigidBody.velocity).z * 0.1f)
			{
				onBulletHit(hit);
			}
		}
	}
	
	public virtual  void onBulletHit(RaycastHit hit)
	{
		if (owner == hit.transform.gameObject) {
			return;
		}

		DamagebleObject obj = hit.transform.gameObject.GetComponent <DamagebleObject>();
		if (obj != null) {
			obj.Damage(damage,owner);
			Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);
			Destroy (gameObject, 0.1f);
		}

		if(hitParticle!=null){
			Instantiate(hitParticle, hit.point, Quaternion.LookRotation(hit.normal));
		}
		Destroy (gameObject, 0.1f);

	}
	
	void OnDestroy() {
		if(splashRadius>0){
			ExplosionDamage();		
		}
	}
	
	void ExplosionDamage() {
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, splashRadius);
		
		for(int i=0;i < hitColliders.Length;i++) {
			//Debug.Log(hitColliders[i]);
			DamagebleObject obj = hitColliders[i].GetComponent <DamagebleObject>();
			if (obj != null) {
				obj.Damage(damage,owner);
			}
		}
	}
}
