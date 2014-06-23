﻿using UnityEngine;
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
	
	public float jumpDistance;
	
	public int proojHitCnt;
	
	public int projHitMax;
	/// <summary>
    /// Define is Any Detonator effects
    /// </summary>
    public enum DETONATOR {Impact,ImpactCharacter, Manual,Proximity,Timed}
	
	public DETONATOR detonator;
	
	public float detonateTimer = 3.0f;
	
	private float _detonateTimer =0.0f;
	
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
	
	public float trajectoryCoef;
	
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
				mRigidBody.AddForce(mRigidBody.velocity.normalized*Time.deltaTime*speedChangeCoef,ForceMode.Acceleration);
			break;
			case SPEEDCHANGE.Deceleration:
				mRigidBody.AddForce( mRigidBody.velocity.normalized*Time.deltaTime*speedChangeCoef,ForceMode.Acceleration);
			break;
		}
		switch(trajectory){
			case TRAJECTORY.Corkscrew:
				mRigidBody.AddForce( mTransform.right*Time.deltaTime*trajectoryCoef,ForceMode.Acceleration);
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
				case DETONATOR.Timed
				  if (_detonateTimer>detonateTimer)
                    {
                        //Debug.Log("boom");

                        ProjectileManager.instance.InvokeRPC("Detonate", projId);
                    }
				break;
            }
			_detonateTimer+= Time.deltaTime;
        }

	}
    public void Detonate() {
        Destroy(gameObject);
    
    }
	
    public virtual void DamageLogic(DamagebleObject obj){
    	obj.Damage(damage,owner);
			
			//Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);
			//Destroy (gameObject, 0.1f);
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
		DamageLogic(obj);
		shootTarget= obj;
		if (obj != null) {
			switch(projHtEffect){
				
				case HITEFFECT.NextTarget:
					if(!replication){
						proojHitCnt++;
						//TODO NEW VELOCITY	
						Pawn hitPawn = obj as Pawn, ownerPawn  = owner as Pawn;
						List<Pawn> allPawn = PlayerManager.instance.FindAllPawn();
						Pawn nextTarget = null;
						float lastDistance = jumpDistance*jumpDistance;
						//For easy calculation;
						float sqreJump = jumpDistance*jumpDistance;
						for (int i = 0; i < allPawn.Count; i++)
						{
							if (allPawn[i] == null)
							{
								continue;
							}
							if (allPawn[i] == hitPawn)
							{
								continue;
							}
							if(ownerPawn==allPawn[i]){
								continue;
							}
							Vector3 distance = (allPawn[i].myTransform.position - myTransform.position);

							if (distance.sqrMagnitude < sqreJump&&lastDistance>distance.sqrMagnitude )
							{
							
								RaycastHit hitInfo;
								Vector3 normalDist = distance.normalized;
								Vector3 startpoint = mTransform.position;

								if (allPawn[i].team != ownerPawn.team && Physics.Raycast(startpoint, normalDist, out hitInfo))
								{

								
									if (allPawn[i].myCollider != hitInfo.collider)
									{
										//Debug.Log ("WALL"+hitInfo.collider);
										continue;
									}
									lastDistance=distance.sqrMagnitude;
									nextTarget=allPawn[i];
								}
								
							}
						}
						if(nextTarget!=null){
							mRigidBody.velocity = 	mRigidBody.velocity.magnitude *(nextTarget.myTransform.position-mTransform.position).normalized;
							ProjectileManager.instance.InvokeRPC("NewVelocity", projId,mRigidBody.velocity,proojHitCnt);
						}else{
							
							ProjectileManager.instance.InvokeRPC("Detonate", projId);
						}

					}else{
						if(proojHitCnt>=projHitMax){
							Destroy (gameObject, 0.1f);
						}
					}	
				
				break;
				default:
					Destroy (gameObject, 0.1f);
				break;
			}
		
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
						
						mRigidBody.velocity = 	mRigidBody.velocity + 2* Vector3.Project(	mRigidBody.velocity,hit.normal);
						
						ProjectileManager.instance.InvokeRPC("NewVelocity", projId,mRigidBody.velocity,proojHitCnt);
					}else{
						if(proojHitCnt>=projHitMax){
							Destroy (gameObject, 0.1f);
						}
					}		
				break;
				case HITEFFECT.Penetration:
					if(!replication){
						proojHitCnt++;
						ProjectileManager.instance.InvokeRPC("NewHitCount", projId,proojHitCnt);
					}else{
						if(proojHitCnt>=projHitMax){
							Destroy (gameObject, 0.1f);
						}
					}		
				break;
				case HITEFFECT.Cluster:
					//TODO CLASTER LOGIC:
					if(!replication){
					
					}				
				break;
				case HITEFFECT.Sticking:
				
					if(!replication){
						StickPosition(hit.point);
						ProjectileManager.instance.InvokeRPC("StickPosition", projId,hit.point);
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
					DamageLogic(obj);
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
