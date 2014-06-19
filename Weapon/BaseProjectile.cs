using UnityEngine;
using System;

using System.Collections;
[Serializable]
public class BaseDamage{
	public float Damage;
	public bool isVsArmor;
	public float pushForce;
	[HideInInspector] 
	public Vector3 pushDirection;
	[HideInInspector] 
	public Vector3 hitPosition;
	[HideInInspector] 
	public bool sendMessage= true;
	[HideInInspector] 
	public bool isContinius =false;
	public BaseDamage(BaseDamage old){
		Damage = old.Damage;
		isVsArmor = old.isVsArmor;
		pushForce = old.pushForce;
	}

}
//We don't want to our projectile fly infinite time
[RequireComponent (typeof (DelayDestroyedObject))]
public class BaseProjectile : MonoBehaviour {



	public BaseDamage damage;
	public float startImpulse;
	public GameObject owner;
	public float range;
	public float minDamageRange;
	static float minimunProcent = 0.1;
	public Vector3 startPosition;
	public GameObject hitParticle;

	public float splashRadius;

	//звуки
	private AudioSource aSource;//источник звука задается в редакторе
	public AudioClip reactiveEngineSound;
	public AudioClip exploseSound;
	private soundControl sControl;//контроллер звука

	protected Transform mTransform;
	private Rigidbody mRigidBody;
	protected bool used=false;

	protected DamagebleObject shootTarget;
	
	/// <summary>
    /// Define Projectile Hit Effect 
    /// </summary>
    public enum HITEFFECT {Destruction,Rebound, Penetration,Cluster,Sticking,Flak,NextTarget,AdjacentTarget}
	
	public HITEFFECT projHtEffect;
	
	/// <summary>
    /// Define is Any Detonator effects
    /// </summary>
    public enum DETONATOR {Impact,ImpactCharacter, Manual,Proximity,Timed}
	
	public DETONATOR detonator;
	
	/// <summary>
    /// Define is Speed Change Effect
    /// </summary>
    public enum SPEEDCHANGE {Uniform,Acceleration,Deceleration}
	
	public SPEEDCHANGE speedchange;
	
	/// <summary>
    /// Define is attraction of projectile
    /// </summary>
    public enum ATTRACTION {NoAttraction,Gravitation,Target,LaserGuidance,Homing}
	
	public ATTRACTION attarction;
	
	/// <summary>
    /// Define is trajectory of projectile
    /// </summary>
    public enum TRAJECTORY {Line,Lobbed,Mortar,Bend,Corkscrew,Scuttle,Wave}
	
	public TRAJECTORY attarction;
	
	void Start () {
		aSource = GetComponent<AudioSource> ();
		sControl = new soundControl (aSource);//создаем обьект контроллера звука и передаем указатель на источник
		sControl.playClip (reactiveEngineSound);

		mTransform = transform;
		startPosition= mTransform.position;
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
	
	protected void Update() {
		
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
		if (owner == hit.transform.gameObject||used) {
			return;
		}
		float distance = (startPosition-mTransform.position).magnitude
		if(range<distance){
			float coef = 1 -(distance -range)/minDamageRange;
			if(coef<minimunProcent){
				coef =minimunProcent;
			}
			damage.Damage=damage.Damage*coef;
		}
	
		damage.pushDirection = mTransform.forward;
		damage.hitPosition = hit.point;
		DamagebleObject obj = hit.transform.gameObject.GetComponent <DamagebleObject>();
		if (obj != null) {
			obj.Damage(damage,owner);
			shootTarget= obj;
			//Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);
			Destroy (gameObject, 0.1f);
		}
		
		used = true;
		
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

		sControl.stopSound ();//останавливаем звук реактивного двигателя
		sControl.playClip (exploseSound);//звук взрыва

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, splashRadius);
		Vector3 Position = transform.position;
		RaycastHit[] hits;
		for(int i=0;i < hitColliders.Length;i++) {

			//Debug.Log(hitColliders[i]);
			Collider isHit = null;
			float distance = splashRadius*splashRadius;
			hits = Physics.RaycastAll(Position, hitColliders[i].transform.position);
			for (int j = 0; j < hits.Length; j++) {
				float localDistance =Vector3.Distance(hitColliders[i].transform.position,Position);
				if(hitColliders[i]!=collider &&localDistance<=distance)
				{
					distance =localDistance;
					isHit=hitColliders[i];
				}
			}
			if (isHit==hitColliders[i]) {
				DamagebleObject obj = hitColliders[i].GetComponent <DamagebleObject>();
				BaseDamage lDamage  = new BaseDamage(damage);
				lDamage.pushDirection = mTransform.forward;
				lDamage.hitPosition = mTransform.position;
				if (obj != null&&obj!=shootTarget) {
					obj.Damage(lDamage,owner);
				}	
			}
		}
	}
}
