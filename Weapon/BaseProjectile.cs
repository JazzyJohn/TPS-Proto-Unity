﻿using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;
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
    public BaseDamage()
    {
    }
	public BaseDamage(BaseDamage old){
		Damage = old.Damage;
		isVsArmor = old.isVsArmor;
		pushForce = old.pushForce;
        knockOut = old.knockOut;
	}

}
//We don't want to our projectile fly infinite time
[RequireComponent(typeof(DelayDestroyedObject))]
public class BaseProjectile : MonoBehaviour
{


    public int projId;
    public bool replication = true;
    public double lateTime;
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
    public LayerMask explosionLayerBlock;
    //звуки
    private AudioSource aSource;//источник звука задается в редакторе
    public AudioClip reactiveEngineSound;
    public AudioClip exploseSound;
    private soundControl sControl;//контроллер звука

    protected Transform mTransform;
    protected Rigidbody mRigidBody;
    protected bool used = false;

    protected DamagebleObject shootTarget;

    /// <summary>
    /// Define Projectile Hit Effect 
    /// </summary>
    public enum HITEFFECT { Destruction, Rebound, Penetration, Cluster, Sticking, Flak, NextTarget }

    public HITEFFECT projHtEffect;

    public float jumpDistance;

    public int proojHitCnt;

    public int projHitMax;
    /// <summary>
    /// Define is Any Detonator effects
    /// </summary>
    public enum DETONATOR { Impact, ImpactCharacter, Manual, Proximity, Timed }

    public DETONATOR detonator;

    public float detonateTimer = 3.0f;

    private float _detonateTimer = 0.0f;

    /// <summary>
    /// Define is Speed Change Effect
    /// </summary>
    public enum SPEEDCHANGE { Uniform, Acceleration, Deceleration }

    public SPEEDCHANGE speedChange;

    public float speedChangeCoef;

    /// <summary>
    /// Define is attraction of projectile
    /// </summary>
    public enum ATTRACTION { NoAttraction, Gravitation, Target, LaserGuidance, Homing }

    public ATTRACTION attraction;

    public float attractionCoef;
    /// <summary>
    /// Define is trajectory of projectile
    /// </summary>
    public enum TRAJECTORY { Line, Bend, Corkscrew, Scuttle, Wave }

    public TRAJECTORY trajectory;

    public float trajectoryCoef;

    public bool shouldInit= false;

	void Awake(){
		aSource = GetComponent<AudioSource>();	
		sControl = new soundControl(aSource);//создаем обьект контроллера звука и передаем указатель на источник
        sControl.playClip(reactiveEngineSound);
		mTransform = transform;
		mRigidBody = rigidbody;
	}
	
    void OnEnable()
    {
      
        used = false;
        mRigidBody.velocity = Vector3.zero;
    }
   public void Init(){
        shouldInit = false;
		switch (attraction)
        {
          
            case ATTRACTION.Homing:
            case ATTRACTION.LaserGuidance:
				if(target==null){
					attraction =ATTRACTION.NoAttraction;
				}
			break;
			
		}
        ProjectileManager.instance.AddProject(projId, this);
 
      
       
        startPosition = mTransform.position;

        mRigidBody.velocity = mTransform.TransformDirection(Vector3.forward * startImpulse);
		
        RaycastHit hit;
        float distance = mTransform.InverseTransformDirection(mRigidBody.velocity).z * 0.1f;
        if (replication)
        {
            distance += (float)(TimeManager.Instance.NetworkTime / 1000 - lateTime) * mRigidBody.velocity.magnitude;
        }

        if (distance>0&&Physics.Raycast(mTransform.position, mRigidBody.velocity.normalized, out hit, distance))
        {

                onBulletHit(hit);
         
        }
		//Debug.Log("id " + projId+ " position " + mTransform.position + " rotation "+ mTransform.rotation);
   
        if (replication)
        {
           // Debug.Log(lateTime + " " + TimeManager.Instance.NetworkTime / 1000);
           // transform.Translate(mRigidBody.velocity * (float)(TimeManager.Instance.NetworkTime/1000 - lateTime));
        }
	//	Debug.Log("id " + projId+ " position " + mTransform.position + " rotation "+ mTransform.rotation);
        mRigidBody.useGravity = false;
    }
   


    protected void Update()
    {
        
        RaycastHit hit;

        switch (attraction)
        {
            case ATTRACTION.Target:
                mRigidBody.AddForce((target.position - mTransform.position).normalized * attractionCoef, ForceMode.Acceleration);
                break;
            case ATTRACTION.Homing:
            case ATTRACTION.LaserGuidance:
                Quaternion rotation = Quaternion.LookRotation((target.position - mTransform.position).normalized);
                mRigidBody.velocity = rotation * mRigidBody.velocity;
                break;
            case ATTRACTION.Gravitation:

                mRigidBody.AddForce(new Vector3(0, -Pawn.gravity * rigidbody.mass * attractionCoef, 0));
                break;


        }
        mTransform.rotation = Quaternion.LookRotation(mRigidBody.velocity);
        if (Physics.Raycast(transform.position, mRigidBody.velocity.normalized, out hit, mRigidBody.velocity.magnitude*0.1f))
        {

          
                onBulletHit(hit);
            
        }

        switch (speedChange)
        {
            case SPEEDCHANGE.Acceleration:
                mRigidBody.AddForce(mRigidBody.velocity.normalized * Time.deltaTime * speedChangeCoef, ForceMode.Acceleration);
                break;
            case SPEEDCHANGE.Deceleration:
                mRigidBody.AddForce(mRigidBody.velocity.normalized * Time.deltaTime * speedChangeCoef, ForceMode.Acceleration);
                break;
        }
        switch (trajectory)
        {
            case TRAJECTORY.Corkscrew:
                mRigidBody.AddForce(mTransform.right * Time.deltaTime * trajectoryCoef, ForceMode.Acceleration);
                break;

        }
        if (!replication)
        {
            switch (detonator)
            {
                case DETONATOR.Manual:
                    if (InputManager.instance.GetButtonDown("Detonate"))
                    {
                        //Debug.Log("boom");
                        ExplosionDamage(mTransform.position);
                        ProjectileManager.instance.InvokeRPC("Detonate", projId, mTransform.position);
                    }
                    break;
                case DETONATOR.Timed:
                    if (_detonateTimer > detonateTimer)
                    {
                        //Debug.Log("boom");
                        ExplosionDamage(mTransform.position);
                        ProjectileManager.instance.InvokeRPC("Detonate", projId, mTransform.position);
                    }
                    break;
            }
            _detonateTimer += Time.deltaTime;
        }

    }
    public void Detonate(Vector3 position)
    {
        
        if (replication)
        {
            RaycastHit hit;

            Vector3 direction = position - mTransform.position;
            //Debug.DrawLine(mTransform.position, mTransform.position +  mRigidBody.velocity*(direction.magnitude+0.1f),Color.red,10.0f);
            if (Physics.Raycast(mTransform.position, mRigidBody.velocity.normalized, out hit,direction.magnitude+ mRigidBody.velocity.magnitude * 0.1f))
            {

                onBulletHit(hit);

            }
            mTransform.position = position - mRigidBody.velocity.normalized * 0.05f;
          //  Debug.DrawLine(mTransform.position, mTransform.position + mRigidBody.velocity * (direction.magnitude + mRigidBody.velocity.magnitude * 0.15f), Color.blue, 10.0f);
            if (Physics.Raycast(mTransform.position,  mRigidBody.velocity.normalized, out hit, direction.magnitude + mRigidBody.velocity.magnitude * 0.15f))
            {

                onBulletHit(hit);

            }
           ExplosionDamage(position);

        }
       // Debug.Log("DETONATE " +position +" id " + projId + " position " + mTransform.position + " rotation " + mTransform.rotation);
        //Debug.Log("detonate"+ position);
    }
   

    public virtual void DamageLogic(DamagebleObject obj, BaseDamage inDamage)
    {
        obj.Damage(inDamage, owner);

        //Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);
        //Destroy (gameObject, 0.1f);
    }
    public void onBulletHit(RaycastHit hit)
    {
        if (owner == hit.transform.gameObject || used)
        {
            return;
        }
        float distance = (startPosition - mTransform.position).magnitude;
        BaseDamage ldamage = new BaseDamage(damage);
        if (range < distance)
        {
            float coef = 1 - (distance - range) / minDamageRange;
            if (coef < minimunProcent)
            {
                coef = minimunProcent;
            }
            ldamage.Damage = damage.Damage * coef;
        }

        ldamage.pushDirection = mTransform.forward;
        ldamage.hitPosition = hit.point;
        DamagebleObject obj = hit.transform.gameObject.GetComponent<DamagebleObject>();
        //move explosion center from surface so raycast dont hit it
        Vector3 exploPosition = hit.point + hit.normal;
        if (obj != null)
        {
            DamageLogic(obj, ldamage);
            shootTarget = obj;
            switch (projHtEffect)
            {

                case HITEFFECT.NextTarget:
                    if (!replication)
                    {
                        proojHitCnt++;
                        //TODO NEW VELOCITY	
                        Pawn hitPawn = obj as Pawn, ownerPawn = owner.GetComponent<Pawn>();
                        List<Pawn> allPawn = PlayerManager.instance.FindAllPawn();
                        Pawn nextTarget = null;
                        float lastDistance = jumpDistance * jumpDistance;
                        //For easy calculation;
                        float sqreJump = jumpDistance * jumpDistance;
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
                            if (ownerPawn == allPawn[i])
                            {
                                continue;
                            }
                            Vector3 distanceBeatwen = (allPawn[i].myTransform.position - mTransform.position);

                            if (distanceBeatwen.sqrMagnitude < sqreJump && lastDistance > distanceBeatwen.sqrMagnitude)
                            {

                                RaycastHit hitInfo;
                                Vector3 normalDist = distanceBeatwen.normalized;
                                Vector3 startpoint = mTransform.position;

                                if (allPawn[i].team != ownerPawn.team && Physics.Raycast(startpoint, normalDist, out hitInfo))
                                {


                                    if (allPawn[i].GetCollider() != hitInfo.collider)
                                    {
                                        //Debug.Log ("WALL"+hitInfo.collider);
                                        continue;
                                    }
                                    lastDistance = distanceBeatwen.sqrMagnitude;
                                    nextTarget = allPawn[i];
                                }

                            }
                        }
                        if (nextTarget != null)
                        {
                            mRigidBody.velocity = mRigidBody.velocity.magnitude * (nextTarget.myTransform.position - mTransform.position).normalized;
                            ProjectileManager.instance.InvokeRPC("NewVelocity", projId, mRigidBody.velocity, proojHitCnt);
                        }
                        else
                        {

                            ProjectileManager.instance.InvokeRPC("Detonate", projId, hit.point);
                        }

                    }
                    else
                    {
                        if (proojHitCnt >= projHitMax)
                        {
                            ExplosionDamage(exploPosition);
                         
                        }
                    }

                    break;
                default:
                    used = true;

                    ExplosionDamage(exploPosition);
                    break;
            }

        }
        else
        {
            switch (projHtEffect)
            {
                case HITEFFECT.Destruction:
                    used = true;
                    if (hitParticle != null)
                    {
                        hitParticle.Spawn( hit.point, Quaternion.LookRotation(hit.normal));
                    }
                    ExplosionDamage(exploPosition);
                 
                    break;
                case HITEFFECT.Rebound:
                    if (!replication)
                    {
                        proojHitCnt++;

                        mRigidBody.velocity = mRigidBody.velocity + 2 * Vector3.Project(mRigidBody.velocity, hit.normal);

                        ProjectileManager.instance.InvokeRPC("NewVelocity", projId, mRigidBody.velocity, proojHitCnt);
                    }
                    else
                    {
                        if (proojHitCnt >= projHitMax)
                        {
                            ExplosionDamage(exploPosition);
                            
                        }
                    }
                    break;
                case HITEFFECT.Penetration:
                    if (!replication)
                    {
                        proojHitCnt++;
                        ProjectileManager.instance.InvokeRPC("NewHitCount", projId, proojHitCnt);
                    }
                    else
                    {
                        if (proojHitCnt >= projHitMax)
                        {

                            ExplosionDamage(exploPosition);
                            
                          
                        }
                    }
                    break;
                case HITEFFECT.Cluster:
                    //TODO CLASTER LOGIC:
                    if (!replication)
                    {

                    }
                    break;
                case HITEFFECT.Sticking:

                    if (!replication)
                    {
                        StickPosition(hit.point);
                        ProjectileManager.instance.InvokeRPC("StickPosition", projId, hit.point);
                    }

                    break;


            }
        }


    }

    void NewVelocity(Vector3 velocity, int cnt)
    {
        mRigidBody.velocity = velocity;
        proojHitCnt = cnt;
    }
    void NewHitCount(int cnt)
    {
        proojHitCnt = cnt;
    }
    void StickPosition(Vector3 position)
    {
        mTransform.position = position;
        mRigidBody.velocity = Vector3.zero;
    }



    void ExplosionDamage(Vector3 Position)
    {
        if(splashRadius==0){
            if (!replication)
            {
                ProjectileManager.instance.InvokeRPC("Detonate", projId, Position);
            }
            Invoke("DeActivate", 0.1f);
                return;
        }
        sControl.stopSound();//останавливаем звук реактивного двигателя
        sControl.playClip(exploseSound);//звук взрыва
        //Debug.Log(splashRadius);
        Collider[] hitColliders = Physics.OverlapSphere(Position, splashRadius);
   
        
        for (int i = 0; i < hitColliders.Length; i++)
        {

            //Debug.Log(hitColliders[i]);
          
          
            float distance = (hitColliders[i].transform.position - Position).magnitude;
            RaycastHit hitInfo;
           // Debug.DrawLine(Position, hitColliders[i].transform.position,Color.red,100);
           // Debug.Log( hitColliders[i]);
            if (!Physics.Raycast(Position, (hitColliders[i].transform.position - Position).normalized,out hitInfo,  distance, explosionLayerBlock))
           {
               //Debug.Log(hitInfo.collider + "==" + hitColliders[i]);
               
           
                DamagebleObject obj = hitColliders[i].GetComponent<DamagebleObject>();
               
                if (obj != null && obj != shootTarget)
                {
                    BaseDamage lDamage = new BaseDamage(damage);
                    lDamage.pushDirection = mTransform.forward;
                    lDamage.hitPosition = mTransform.position;
                    DamageLogic(obj, lDamage);
                }

               
            }
          
                
               
            
        }
        if (!used)
        {
            if (hitParticle != null)
            {
                hitParticle.Spawn( Position, mTransform.rotation);
            }

        }
        if (!replication)
        {
            ProjectileManager.instance.InvokeRPC("Detonate", projId, Position);
        }
        Invoke("DeActivate", 0.1f);
    }
	public void DeActivate(){
		gameObject.Recycle();
        
	}
	public void OnDisable(){
		CancelInvoke();
	}
}
