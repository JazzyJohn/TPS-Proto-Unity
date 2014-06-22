using UnityEngine;
using System;

using System.Collections;
[Serializable]
public class BaseDamage{
	public float Damage;
	public bool isVsArmor;
	public float pushForce;
    public bool knockOut= false;
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
        knockOut = old.knockOut;
	}

}
//We don't want to our projectile fly infinite time
[RequireComponent (typeof (DelayDestroyedObject))]
public class BaseProjectile : MonoBehaviour {


    public int projId;
    public bool replication= true;
	public BaseDamage damage;
	public float startImpulse;
	public GameObject owner;
	public Transform target;
	public float range;
	public float minDamageRange;
	static float minimunProcent = 0.1f;
	public Vector3 startPosition;
	public GameObject hitParticle;

	public float splashRadius;

	//звуки
	private AudioSource aSource;//источник звука задается в редакторе
	public AudioClip reactiveEngineSound;
	public AudioClip exploseSound;
	private soundControl sControl;//контроллер звука

	protected Transform mTransform;
    protected Rigidbody mRigidBody;
	protected bool used=false;

	protected DamagebleObject shootTarget;
	
	/// <summary>
    /// Define Projectile Hit Effect 
    /// </summary>
    public enum HITEFFECT {Destruction,Rebound, Penetration,Cluster,Sticking,Flak,NextTarget}
	
	public HITEFFECT projHtEffect;
	
	public int proojHitCnt;
	
	public int projHitMax;
	/// <summary>
    /// Define is Any Detonator effects
    /// </summary>
    public enum DETONATOR {Impact,ImpactCharacter, Manual,Proximity,Timed}
	
	public DETONATOR detonator;
	
	/// <summary>
    /// Define is Speed Change Effect
    /// </summary>
    public enum SPEEDCHANGE {Uniform,Acceleration,Deceleration}
	
	public SPEEDCHANGE speedChange;
	
	public float speedChangeCoef;
	
	/// <summary>
    /// Define is attraction of projectile
    /// </summary>
    public enum ATTRACTION {NoAttraction,Gravitation,Target,LaserGuidance,Homing}
	
	public ATTRACTION attraction;
	
	public float attractionCoef;
	/// <summary>
    /// Define is trajectory of projectile
    /// </summary>
    public enum TRAJECTORY {Line,Bend,Corkscrew,Scuttle,Wave}
	
	public TRAJECTORY trajectory;
	
	void Start () {
        ProjectileManager.instance.AddProject(projId, this);
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
		
		
			mRigidBody.useGravity  = false;
    }
	
	
	protected void Update() {
		
		RaycastHit hit;
	
		switch(attraction){
			case ATTRACTION.Target:
				mRigidBody.AddForce((target.position -mTransform.position).normalized*attractionCoef,ForceMode.Acceleration);
                break;
			case ATTRACTION.Homing:
			case ATTRACTION.LaserGuidance:
				Quaternion rotation = Quaternion.LookRotation((target.position -mTransform.position).normalized);
				mRigidBody.velocity =  rotation*mRigidBody.velocity;
			break;
			case ATTRACTION.Gravitation:
				
				mRigidBody.AddForce(new Vector3(0,-Pawn.gravity * rigidbody.mass,0));		
			break;

		
		}		
		mTransform.rotation = Quaternion.LookRotation ( mRigidBody.velocity);
		if (Physics.Raycast (transform.position, mRigidBody.velocity.normalized, out hit)){
			
			if (hit.distance < mTransform.InverseTransformDirection (mRigidBody.velocity).z * 0.1f)
			{
				onBulletHit(hit);
			}
		}

		switch(speedChange){
			case SPEEDCHANGE.Acceleration:
				mRigidBody.velocity+= mRigidBody.velocity.normalized*Time.deltaTime*speedChangeCoef;
			break;
			case SPEEDCHANGE.Deceleration:
				mRigidBody.velocity-= mRigidBody.velocity.normalized*Time.deltaTime*speedChangeCoef;
			break;
		}
        if (!replication)
        {
            switch(detonator){
                case DETONATOR.Manual:
                    if (Input.GetButtonDown("Detonate"))
                    {
                        //Debug.Log("boom");
                        ProjectileManager.instance.InvokeRPC("Detonate", projId);
                    }
                break;
            }
        }

	}
    public void Detonate() {
        Destroy(gameObject);
    
    }
	
    public virtual void DamageLogic(DamagebleObject obj){
    	obj.Damage(damage,owner);
			shootTarget= obj;
			//Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);
			Destroy (gameObject, 0.1f);
    }
	public  void onBulletHit(RaycastHit hit)
	{
		if (owner == hit.transform.gameObject||used) {
			return;
		}
        float distance = (startPosition - mTransform.position).magnitude;
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
            DamageLogic(obj);
		
		}else{
			switch(projHtEffect){
				case HITEFFECT.Destruction:
					used = true;
					if(hitParticle!=null){
						Instantiate(hitParticle, hit.point, Quaternion.LookRotation(hit.normal));
					}
                    Debug.Log("destroy");
		
					Destroy (gameObject, 0.1f);
				break;
				case HITEFFECT.Rebound:
					if(!replication){
						proojHitCnt++;
						//TODO: REBOUND LOGIC
					}else{
						if(proojHitCnt>=projHitMax){
							Destroy (gameObject, 0.1f);
						}
					}		
				break;
				case HITEFFECT.Penetration:
					if(!replication){
						proojHitCnt++;
						//TODO: PRC CALL
					}else{
						if(proojHitCnt>=projHitMax){
							Destroy (gameObject, 0.1f);
						}
					}		
				break;
				case HITEFFECT.Cluster:
					//TODO CLASTER LOGIC:
				
				
				break;
				case HITEFFECT.Sticking:
					//TODO CLASTER LOGIC:
					if(!replication){
				
					}
				
				break;
				case HITEFFECT.NextTarget:
					//TODO CLASTER NextTarget: same RPC taht in rebound
					if(!replication){
						proojHitCnt++;
						//TODO: REBOUND LOGIC
					}else{
						if(proojHitCnt>=projHitMax){
							Destroy (gameObject, 0.1f);
						}
					}	
				
				break;
			
			}
		}
	

	}
	
	void NewVelocity(Vector3 velocity ,int cnt){
		mRigidBody.velocity =velocity;
		proojHitCnt=cnt;
	}
	void NewHitCount(int cnt){
		proojHitCnt=cnt;
	}
	void StickPosition(Vector3 position){
		mTransform.position=position;
		mRigidBody.velocity =Vector3.zero;
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
        if (!used) {
            if (hitParticle != null)
            {
                Instantiate(hitParticle, mTransform.position, mTransform.rotation);
            }
        }
	}
}
