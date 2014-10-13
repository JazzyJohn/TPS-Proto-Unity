using UnityEngine;
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
	public bool splash=false;
	[HideInInspector] 
	public bool weapon= false;
	[HideInInspector] 
	public Vector3 pushDirection;
	[HideInInspector] 
	public Vector3 hitPosition;
	[HideInInspector] 
	public bool sendMessage= true;
	[HideInInspector] 
	public bool isContinius =false;
	[HideInInspector] 
	public int shootWeapon = -1;
	
    public BaseDamage()
    {
    }
	public BaseDamage(BaseDamage old){
		Damage = old.Damage;
		isVsArmor = old.isVsArmor;
		pushForce = old.pushForce;
        knockOut = old.knockOut;
		splash = old.splash;
        weapon = old.weapon;
		shootWeapon= old.shootWeapon;
	}

}
//We don't want to our projectile fly infinite time
[RequireComponent(typeof(DelayDestroyedObject))]
public class BaseProjectile : MonoBehaviour
{
	public bool active;

    public int projId;
    public bool replication = true;
    public long lateTime;
    public BaseDamage damage;
    public float startImpulse;
    private float curSpeed;
    public GameObject owner;
    public Transform target;
    public Vector3 targetOffset; 
    public float range;
    public float minDamageRange;
    static float minimunProcent = 0.1f;
    public Vector3 startPosition;
	
	//EFFECTS
    public GameObject hitParticle;
	
	public float disableEffectDelay;
	
	public GameObject[] inactiveObjectInEffectStage;
	
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
    protected bool boomed = false;

    protected DamagebleObject shootTarget;

    /// <summary>
    /// Define Projectile Hit Effect 
    /// </summary>
    public enum HITEFFECT { Destruction, Rebound, Penetration, Cluster, Sticking, Flak, NextTarget }

    public HITEFFECT projHtEffect;
	
    public HITEFFECT projHtPersonEffect;

    public float jumpDistance;

    public int proojHitCnt;

    public int projHitMax;
	
	public float hitDelay;
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
    public enum ATTRACTION { NoAttraction, Target, LaserGuidance, Homing }

    public ATTRACTION attraction;

    public float attractionCoef;

    private float _attractionCoef;
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
        boomed = false;
        mRigidBody.velocity = Vector3.zero;
		foreach(GameObject go in inactiveObjectInEffectStage){
			go.SetActive(true);
		}
    }
   public void Init(){
        shouldInit = false;

		switch (attraction)
        {
          
            case ATTRACTION.Homing:
            case ATTRACTION.Target:
            case ATTRACTION.LaserGuidance:
				if(target==null){
					attraction =ATTRACTION.NoAttraction;
				}
			break;
			
		}
        ProjectileManager.instance.AddProject(projId, this);
        _attractionCoef = attractionCoef;
        damage.splash = splashRadius>0;
		
        startPosition = mTransform.position;

        mRigidBody.velocity = mTransform.TransformDirection(Vector3.forward * startImpulse);
        curSpeed = startImpulse;
		
        RaycastHit hit;
        float distance = mTransform.InverseTransformDirection(mRigidBody.velocity).z * 0.1f;
        if (replication)
        {
            distance += (float)(TimeManager.Instance.NetworkTime  - lateTime) * mRigidBody.velocity.magnitude;
        }

        if (distance>0&&Physics.Raycast(mTransform.position, mRigidBody.velocity.normalized, out hit, distance))
        {

                onBulletHit(hit);
         
        }
	//Debug.Log("id " + projId+ " position " + mTransform.position + " rotation "+ mTransform.rotation);
   
        if (replication)
        {
        //    Debug.Log(lateTime + " " + TimeManager.Instance.NetworkTime);
         //  transform.Translate(mRigidBody.velocity * (float)((TimeManager.Instance.NetworkTim - lateTime)/1E+3));
        }
	//	Debug.Log("id " + projId+ " position " + mTransform.position + " rotation "+ mTransform.rotation);
       // mRigidBody.useGravity = false;
    }
	
	public float GetSpeedChange(){
		
		switch (speedChange)
        {
            case SPEEDCHANGE.Acceleration:
                return  speedChangeCoef;
               
            case SPEEDCHANGE.Deceleration:
                return  -speedChangeCoef;
                
        }
        return 0;
	}


    protected void FixedUpdate()
    {
        if(active){
			return;
		}
        RaycastHit hit;
      
        switch (speedChange)
        {
            case SPEEDCHANGE.Acceleration:
                curSpeed += Time.deltaTime * speedChangeCoef;
                break;
            case SPEEDCHANGE.Deceleration:
                curSpeed -= Time.deltaTime * speedChangeCoef;
                break;
        }
        Vector3 result = Vector3.zero;
        switch (attraction)
        {
            case ATTRACTION.Target:
                //Debug.Log(targetOffset);
                Vector3 preVel = ((target.position + targetOffset) - mTransform.position);
                _attractionCoef += _attractionCoef * trajectoryCoef*Time.fixedDeltaTime;
                preVel = Vector3.Cross(preVel, mTransform.up) + preVel.normalized * _attractionCoef;
                result = Vector3.Lerp(mRigidBody.velocity, preVel.normalized * curSpeed, Time.deltaTime * trajectoryCoef) - mRigidBody.velocity;
               // Quaternion rotation = Quaternion.LookRotation(((target.position + targetOffset) - mTransform.position).normalized);
               // mRigidBody.velocity = rotation * mRigidBody.velocity;
                break;
            case ATTRACTION.Homing:
            case ATTRACTION.LaserGuidance:


                result = Vector3.Lerp(mRigidBody.velocity, ((target.position + targetOffset) - mTransform.position).normalized * curSpeed, Time.deltaTime * _attractionCoef) - mRigidBody.velocity;
                break;
            case ATTRACTION.NoAttraction:
              //  Debug.Log(curSpeed);
                switch (speedChange)
                {
                    case SPEEDCHANGE.Acceleration:
                       result = mRigidBody.velocity.normalized * Time.deltaTime * speedChangeCoef;
                        break;
                    case SPEEDCHANGE.Deceleration:
                        result =- mRigidBody.velocity.normalized * Time.deltaTime * speedChangeCoef;
                        break;
                }
             
                break;
           


        }
     
		if(mRigidBody.velocity.sqrMagnitude >0&&hitDelay<Time.time){
			mTransform.rotation = Quaternion.LookRotation(mRigidBody.velocity);
			if (Physics.Raycast(transform.position, mRigidBody.velocity.normalized, out hit, mRigidBody.velocity.magnitude*0.1f))
			{

			  
					onBulletHit(hit);

            }
            else
            {
                if (result.sqrMagnitude != 0)
                {
                    Debug.Log(result);
                    mRigidBody.AddForce(result, ForceMode.Acceleration);

                }
                switch (trajectory)
                {
                    case TRAJECTORY.Corkscrew:
                        mRigidBody.AddForce(mTransform.right * Time.deltaTime * trajectoryCoef, ForceMode.Acceleration);
                        break;

                }
            }
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
            switch (projHtPersonEffect)
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
				case HITEFFECT.Penetration:
                   
                    proojHitCnt++;
                    if (proojHitCnt > projHitMax)
                    {

                        ExplosionDamage(exploPosition);
                            
                          
                    }else{
						if (!replication)
						{
						  
							
							hitDelay =  Time.time+ Mathf.Max(1.0f/startImpulse,0.1f);
							
							ProjectileManager.instance.InvokeRPC("NewHitCount", projId, proojHitCnt);
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
                   
                    proojHitCnt++;
                    if (proojHitCnt >projHitMax)
                    {
                        ExplosionDamage(exploPosition);
                            
                    }else{
						if (!replication)
						{
							

							mRigidBody.velocity = mRigidBody.velocity - 2 * Vector3.Project(mRigidBody.velocity, hit.normal);
                            mTransform.position = hit.point;
							ProjectileManager.instance.InvokeRPC("NewVelocity", projId, mRigidBody.velocity, proojHitCnt);
						}
					}
                    
                    break;
                case HITEFFECT.Penetration:
                   
                    proojHitCnt++;
                    if (proojHitCnt > projHitMax)
                    {

                        ExplosionDamage(exploPosition);
                            
                          
                    }else{
						if (!replication)
						{
						  
							
							hitDelay =  Time.time+ Mathf.Max(1.0f/startImpulse,0.1f);
							
							ProjectileManager.instance.InvokeRPC("NewHitCount", projId, proojHitCnt);
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
        if (boomed)
        {
            return;
        }
        boomed = true;
        sControl.stopSound();//останавливаем звук реактивного двигателя
        sControl.playClip(exploseSound);//звук взрыва
        Debug.Log("DOOOM");
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
		active= false;
		foreach(GameObject go in inactiveObjectInEffectStage){
			go.SetActive(false);
		}
		 Invoke("_DeActivate", disableEffectDelay);
	}
	void _DeActivate(){
        ProjectileManager.instance.RemoveProjectile(projId);
		gameObject.Recycle();
       
	}
	public void OnDisable(){
		CancelInvoke();
	}
}
