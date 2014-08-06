﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nstuff.juggerfall.extension.models;




public class BaseWeapon : DestroyableNetworkObject {
	
	private static LayerMask layer = -123909;
	
	
	public enum AMUNITONTYPE{SIMPLEHIT, PROJECTILE, RAY, HTHWEAPON, AOE};

	public AMUNITONTYPE amunitionType;
	
	public enum SLOTTYPE{PERSONAL, MAIN, ANTITANK,GRENADE};

	public SLOTTYPE slotType;
	
	public AMMOTYPE ammoType;
    //SHOOT LOGIC

    /// <summary>
    /// Define Fireing type like Auto SemiAuto
    /// </summary>
    public enum FIRETYPE {FULLAUTO,SEMIAUTO,BOLT}

    public FIRETYPE firetype;

	public float fireInterval;
    
    /// <summary>
    /// For SemiAuto ANd BoltAmmo
    /// </summary>
    private bool _waitForRelease = false;
    public bool waitForRelease = false;

    public float releaseInterval;
    protected float _releaseInterval;
    /// <summary>
    /// For semiAuto amout in shoot;
    /// </summary>
    public int amountInShoot;

    protected int _amountInShoot;
    /// <summary>
    /// Define Pre firing mode
    /// </summary>
    public enum PREFIRETYPE {Normal,Spooling, Salvo, ChargedPower, ChargedAccuracy,Guidance,ChargedRange}

    public PREFIRETYPE prefiretype;
    /// <summary>
    /// For semiAuto amout in shoot;
    /// </summary>
    public float pumpAmount;
    protected float _pumpAmount;
    /// <summary>
    ///  Use to Caclulate impact of pumping 
    /// </summary>
    public float pumpCoef;
    protected float _pumpCoef;
   

    /// <summary>
    /// After Pump Finish action
    /// </summary>
    public enum AFTERPUMPACTION { Wait, Fire, Ammo, Damage }

    public AFTERPUMPACTION afterPumpAction;
    /// <summary>
    ///  Use to Caclulate impact of pumping benead threshold
    /// </summary>
    public float afterPumpCoef;

    protected float _afterPumpAmount;

	public float reloadTime;

	public int clipSize;

	public BaseDamage damageAmount;
	/// <summary>
    ///  Rand to Shoot direction in normal state
    /// </summary>
	public float normalRandCoef;
	/// <summary>
    ///  Rand to Shoot direction in aim state
    /// </summary>
	public float aimRandCoef;
	/// <summary>
    ///  Rand to Shoot direction add after one shoot;
    /// </summary>
	public float randPerShoot;
	/// <summary>
    ///  Rand to Shoot direction summary from some effects;
    /// </summary>
	protected float _randShootCoef=0.0f;
	/// <summary>
    /// How cooling move to zero;
    /// </summary>
	public float randCoolingEffect;
	/// <summary>
    ///MAximum of all random Effect;
    /// </summary>
	public float maxRandEffect;

	public GameObject projectilePrefab;
	
	public GameObject pickupPrefabPrefab;

	public Transform muzzlePoint;

	public Transform leftHandHolder;

	protected Vector3 	muzzleOffset;

	public float weaponRange;

	public int curAmmo;
    /// <summary>
    ///  Ammo that ready to shot in barrel in other word
    /// </summary>
    protected int alredyGunedAmmo;

    /// <summary>
    ///  Ammo that ready to shot in barrel in other word maximum
    /// </summary>
    public int alredyGunedAmmoMax;

	protected Pawn owner;

	public Transform curTransform;

	private bool isReload = false;

	protected bool isShooting = false;

    protected bool isPumping = false;

	private float fireTimer =  0.0f;
	
	private float reloadTimer =  0.0f;

	protected RifleParticleController rifleParticleController;

	public string attackAnim;

	public string weaponName;
	
	public Texture2D HUDIcon;
	
	public float recoilMod;

	public bool init = false;

	public const float MAXDIFFERENCEINANGLE=0.7f;

	private bool shootAfterReload;
    private bool pumpAfterReload;

	//звуки
	protected soundControl sControl;//глобальный обьект контроллера звука
	private AudioSource aSource;//источник звука. добавляется в редакторе
	public AudioClip fireSound;
	public AudioClip reloadSound;
	//use for switch
	public int animType;

	//ID for MySqlBAse
	public int SQLId;

	void Awake(){
		foxView = GetComponent<FoxView>();
	}
	
	// Use this for initialization
	protected void Start () {
		aSource = GetComponent<AudioSource> ();
		sControl = new soundControl (aSource);//создаем обьект контроллера звука

		//проверяем длительности звуков стрельбы и перезарядки
		if (fireSound!=null&&fireSound.length >= fireInterval) {
			//Debug.LogWarning("fireSound clip length is greater than fireIntrval value");
		}
		if (reloadSound!=null&&reloadSound.length >= reloadTime) {
			//Debug.LogError("reloadSound clip length is greater than reloadTime value");
		}
		
		curTransform = transform;
	
		rifleParticleController = GetComponentInChildren<RifleParticleController>();
	
		if (rifleParticleController != null&&foxView.isMine) {
			rifleParticleController.SetOwner (owner.collider);
		}
	}

	public virtual void AttachWeapon(Transform weaponSlot,Vector3 Offset, Quaternion weaponRotator,Pawn inowner){
		if (curTransform == null) {
			curTransform = transform;		
		}
		if (foxView == null) {
			foxView = GetComponent<FoxView>();
		}
		owner = inowner;

		curTransform.parent = weaponSlot;
		curTransform.localPosition = Offset;
		//Debug.Log (name + weaponRotator);
		curTransform.localRotation = weaponRotator;
        
		//RemoteAttachWeapon(inowner);
		
		
	}

	public void RemoteAttachWeapon(Pawn newowner){
		if (curTransform == null) {
			curTransform = transform;		
		}
		owner =newowner;
		if (owner == null) {
			//Destroy(photonView);
			Debug.Log ("DestoroyATTACHEs");
			Destroy(gameObject);
            return;
		}
		owner.setWeapon (this);
		if (rifleParticleController != null) {
			rifleParticleController.SetOwner (owner.collider);
		}
		init = true;
	}

	// Update is called once per frame
	void FixedUpdate () {
        UpdateWeapon(Time.fixedDeltaTime);
    }
    void UpdateWeapon(float deltaTime){
		if(init&&owner==null) {
			RequestKillMe();

		}
		//AimFix ();
		

		if(isReload){
			if(reloadTimer<0){
				Reload();
			}
            reloadTimer -= deltaTime;
			return;
		}
        switch(prefiretype){
            case PREFIRETYPE.Normal:
		        if (isShooting) {
                    ShootTick();
		        }
                if (fireTimer >= 0)
                {
                    fireTimer -= deltaTime;
                }

                break;
            default:
                if (isShooting||isPumping)
                {
                    if (_pumpAmount >= pumpAmount)
                    {
						
                        if (isShooting)
                        {
                            ShootTick();

                        }
                        else
                        {
                            switch (afterPumpAction) {
                                case AFTERPUMPACTION.Fire:
                                    if (fireTimer <= 0)
                                    {
                                        fireTimer = fireInterval;
                                        Fire();
                                    }
                                    break;
                                case AFTERPUMPACTION.Ammo:
                                    _afterPumpAmount += deltaTime*afterPumpCoef;
                                    if (_afterPumpAmount >= 1.0f) {
                                        _afterPumpAmount = 0;
                                        if (curAmmo > 0 )
                                        {
                                            curAmmo--;
                                          
                                        } 
                                     
                                    }
                                    break;
                                case AFTERPUMPACTION.Damage:
                                    BaseDamage selfdamage = new BaseDamage(damageAmount);
                                    selfdamage.Damage = deltaTime*afterPumpCoef;
                                    owner.Damage(selfdamage, owner.gameObject);
                                    break;
                            }
                        }
                        if (fireTimer >= 0)
                        {
                            fireTimer -=deltaTime;
                        }

                    }
                    else
                    {
						_pumpCoef += deltaTime*pumpCoef;
                        switch (prefiretype)
                        {
                            case PREFIRETYPE.Salvo:
                                if (curAmmo == 0 && alredyGunedAmmo == 0) {
                                    ReloadStart();
                                }
                               
                               if (_pumpCoef >= 1.0f)
                               {
                                   _pumpCoef = 0;
                                   if(curAmmo > 0 &&alredyGunedAmmoMax > alredyGunedAmmo){
                                       alredyGunedAmmo++;
                                       curAmmo--;
                                   }
                               }

                                break;
							default:
								  if (curAmmo == 0) {
                                    ReloadStart();
                                }
                               
								break;
                         }
                        _pumpAmount += deltaTime;
                    }
                }
                break;
        }
		_randShootCoef-=randCoolingEffect*deltaTime;
     
	}

    private void ShootTick()
    {
     
        if (fireTimer <= 0)
        {
            fireTimer = fireInterval;
            Fire();
        }
        if (_waitForRelease && !waitForRelease)
        {
            _releaseInterval += Time.deltaTime;
            if (releaseInterval < _releaseInterval)
            {
                _releaseInterval = 0.0f;
                _amountInShoot = 0;
                _waitForRelease = false;
            }
        }
    }
	public virtual void StartFire(){
		isShooting = true;

	}
    public virtual void StartPumping()
    {
        isPumping = true;

    }
    public virtual void StopPumping()
    {
        isPumping = false;
        if (!isShooting)
        {
            _pumpCoef = 0.0f;
            _pumpAmount = 0.0f;
        }
        pumpAfterReload = false;
    
    }
	public virtual void StopFire(){
        switch (prefiretype)
        {
            case PREFIRETYPE.Normal:
            case PREFIRETYPE.Spooling:

                break;
            default:
                Fire();
                break;
        }
		isShooting = false;
		shootAfterReload = false;
		ReleaseFire ();
        _waitForRelease = false;
        _amountInShoot = 0;
       
        if (!isPumping) {
            _pumpCoef = 0.0f;
            _pumpAmount = 0.0f;
        }

    }
	
	public void ReloadStart(){
		if (isReload) {
			return;
		}
		if(owner.GetComponent<InventoryManager>().HasAmmo(ammoType)){
			isReload= true;
			reloadTimer=reloadTime;
			if(isShooting){
				StopFire();
				shootAfterReload = true;
			}
            if (isPumping)
            {
                StopPumping();
                pumpAfterReload = true;
            }
			//играем звук перезарядки
			sControl.playClip (reloadSound);
			if(owner.animator!=null){
				owner.animator.ReloadStart();
			}

		}else{
			StopFire();
			return;
		}
		
	}
	public void Reload(){

		isReload = false;

		int oldClip = curAmmo;
		curAmmo =owner.GetComponent<InventoryManager>().GiveAmmo(ammoType,clipSize-curAmmo)+oldClip;	
		if (shootAfterReload) {
			shootAfterReload=false;
			StartFire();
		}
        if (pumpAfterReload) {
            pumpAfterReload = false;
            StartPumping();
        }
		if (owner.player != null) {
			EventHolder.instance.FireEvent (typeof(LocalPlayerListener), "EventPawnReload", owner.player);
		}

		//curAmmo =owner.GetComponent<InventoryManager>().GiveAmmo(ammoType,clipSize);

	}
	public bool IsReloading(){
		return isReload;
	}
	public bool IsShooting(){
		return isShooting && !isReload;
	}
	public float ReloadTimer(){
		if (isReload) {
			return reloadTimer;
		}
		return 0.0f;
	}
    public float PumpCoef()
    {
        return _pumpAmount / pumpAmount * 100.0f;
    }
	//temporary function to fix wrong aiming
	public virtual void AimFix(){

	}
	protected void Fire(){
		if (!CanShoot ()) {
			return;		
		}
       
        LogicShoot();
        
		if(curAmmo>0){
			curAmmo--;
		}else{
            if (alredyGunedAmmo <= 0)
            {
                if (clipSize > 0)
                {
                    ReloadStart();
                    return;
                }
            }
            else
            {
                alredyGunedAmmo--;
            }
		}
			//играем звук стрельбы
		
		if (rifleParticleController != null) {
			rifleParticleController.CreateShootFlame ();
		}
		
		owner.shootEffect();
	
        if (alredyGunedAmmo > 0)
        {
            for (int i = 0; i < alredyGunedAmmo; i++)
            {
                ActualFire();
            }
			_randShootCoef+=alredyGunedAmmo*randPerShoot;
            alredyGunedAmmo = 0;
        }
        else
        {
            ActualFire();
			_randShootCoef+=randPerShoot;
        }
	
		owner.HasShoot ();
        AfterShootLogic();
		//photonView.RPC("FireEffect",PhotonTargets.Others);
	}

    private void ActualFire()
    {
        switch (amunitionType)
        {
            case AMUNITONTYPE.SIMPLEHIT:
                sControl.playClip(fireSound);
                DoSimpleDamage();
                break;
            case AMUNITONTYPE.PROJECTILE:
                sControl.playClip(fireSound);
                GenerateProjectile();
                break;
            case AMUNITONTYPE.RAY:

                break;
            case AMUNITONTYPE.HTHWEAPON:
                sControl.playClip(fireSound);
                owner.animator.StartAttackAnim(attackAnim);
                ChangeWeaponStatus(true);
                break;

        }
		
    }

    public virtual  void LogicShoot()
    {
        _amountInShoot++;
        _releaseInterval = 0.0f;
        switch (firetype)
        {
            case FIRETYPE.SEMIAUTO:
                if (_amountInShoot >= amountInShoot) {
                    _waitForRelease = true; 
                }
                break;
            case FIRETYPE.BOLT:
                _waitForRelease = true;
                break;
        }

    }

    public virtual void AfterShootLogic()
    {

    }
	public virtual bool CanShoot (){
        if (_waitForRelease)
        {
           
            return false;
        }
		Vector3 aimDir = (owner.getCachedAimRotation() -muzzlePoint.position).normalized;
		Vector3 realDir = muzzlePoint.forward;
		float angle = Vector3.Dot (aimDir, realDir);

		if (angle < MAXDIFFERENCEINANGLE) {
           // Debug.Log("angle");
			return false;		
		}

//		Vector3 aimDir = (owner.getCachedAimRotation() -muzzlePoint.position).normalized;
//		Vector3 realDir = muzzlePoint.forward;
//		float angle = Vector3.Dot (aimDir, realDir);
//
//		if (angle < 0.8) {
//			return false;		
//		}

		return true;
	}

	protected void ReleaseFire(){
		switch (amunitionType) {

		case AMUNITONTYPE.HTHWEAPON:
			owner.animator.StopAttackAnim(attackAnim);
			ChangeWeaponStatus(false);
			break;
			
		}
	}

	public virtual void ChangeWeaponStatus(bool status){


	}
	void DoSimpleDamage(){
		Camera maincam = Camera.main;
		Ray centerRay= maincam.ViewportPointToRay(new Vector3(.5f, 0.5f, 1f));
		RaycastHit hitInfo;

		if (Physics.Raycast (centerRay, out hitInfo, weaponRange)) {
		
			HitEffect(hitInfo);
			
		}
	}
	
	/// <summary>
    /// Hit logic for SimpleDamage
    /// </summary>
	protected virtual void HitEffect(RaycastHit hitInfo){
			DamagebleObject target =(DamagebleObject) hitInfo.collider.GetComponent(typeof(DamagebleObject));
			if(target!=null){
				target.Damage(new BaseDamage(damageAmount) ,owner.gameObject);
			}
	}
	
	/// <summary>
    /// Generate random distribution coef for projectile
    /// </summary>
	protected float GetRandomeDirectionCoef(){
		float effAimRandCoef = _randShootCoef;
		if (owner.isAiming) {
			effAimRandCoef+=aimRandCoef;
		}else{
			effAimRandCoef+=normalRandCoef;
		}

		effAimRandCoef+= owner.AimingCoef ();
		if(effAimRandCoef>maxRandEffect){
			effAimRandCoef =maxRandEffect;
		}
		if(prefiretype==PREFIRETYPE.ChargedAccuracy){
			effAimRandCoef-= _pumpCoef;
			_pumpCoef=0;
		}
		return effAimRandCoef;
	}
	/// <summary>
    /// Return target from owner;
    /// </summary>
	protected Transform GetGuidanceTarget(){
		Transform target  = owner.curLookTarget;
	
		return target;
	}
	protected virtual void GenerateProjectile(){
		Vector3 startPoint  = muzzlePoint.position+muzzleOffset;
		Quaternion startRotation = getAimRotation();
		GameObject proj;
		float effAimRandCoef = GetRandomeDirectionCoef();
		
		if (effAimRandCoef > 0) {
			startRotation = Quaternion.Euler (startRotation.eulerAngles + new Vector3 (Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef)));
		}
       // Debug.DrawLine(transform.position, startPoint, Color.red,10 );
   
		proj=Instantiate(projectilePrefab,startPoint,startRotation) as GameObject;
        //Debug.DrawLine(transform.position,proj.transform.position, Color.blue, 10);
		BaseProjectile projScript =proj.GetComponent<BaseProjectile>();
		float power=0;
		float range = 0;
		int viewId = 0;
		switch(prefiretype){
			case PREFIRETYPE.ChargedPower:
				power += _pumpCoef;
			break;
			case PREFIRETYPE.ChargedRange:
				range += _pumpCoef;
			break;
			case PREFIRETYPE.Guidance:
				if(_pumpCoef>=1.0f){
					Transform target = GetGuidanceTarget();
				
					if(target!=null){
						projScript.target = target;
						viewId = target.GetComponent<FoxView>().viewID;

					}
				}
			break;
		}
     
        projScript.projId = ProjectileManager.instance.GetNextId();
        projScript.replication = false;
		if (foxView.isMine) {
            foxView.SendShoot(startPoint, startRotation, power, range, viewId, projScript.projId);
		}
		
		
		projScript.damage =new BaseDamage(damageAmount) ;
		projScript.owner = owner.gameObject;
		projScript.damage.Damage+=power;
		projScript.range+=range;
	}
	public virtual void RemoteGenerate (Vector3 position, Quaternion rotation, float power, float range, int viewId, int projId,double timeShoot){

        BaseProjectile proj = GenerateProjectileRep(position, rotation, timeShoot);
			if (rifleParticleController != null) {
				rifleParticleController.CreateShootFlame ();
			}
            proj.projId = projId;
			proj.damage.Damage+=power;
			proj.range+=range;
			switch(prefiretype){
				case PREFIRETYPE.Guidance:
					if(viewId!=0){
						Transform target =NetworkController.GetView(viewId).GetComponent<Transform>();
				
					
						proj.target = target;
					}
				break;
			}
		
	}
	protected BaseProjectile GenerateProjectileRep(Vector3 startPoint,Quaternion startRotation,double timeShoot){

		GameObject proj=Instantiate(projectilePrefab,startPoint,startRotation) as GameObject;
		BaseProjectile projScript = proj.GetComponent<BaseProjectile>();
        projScript.lateTime = timeShoot;

        projScript.damage =new BaseDamage(damageAmount) ;
		projScript.owner = owner.gameObject;
		return projScript;
	}

	protected Quaternion getAimRotation(){
		/*Vector3 randVec = Random.onUnitSphere;
		Vector3 normalDirection  = owner.getAimRotation(weaponRange)-muzzlePoint.position;
		normalDirection =normalDirection + randVec.normalized * normalDirection.magnitude * aimRandCoef / 100;*/

		return Quaternion.LookRotation(owner.getAimRotation() -muzzlePoint.position);
		

	}
	
	
	public float MuzzleOffset(){

		return (muzzlePoint.position + muzzleOffset - curTransform.position).sqrMagnitude;
	}
    WeaponModel sirWep = new WeaponModel();
    public WeaponModel GetSerilizedData()
    {
        sirWep.id = foxView.viewID;
		return sirWep;
	}
    public void NetUpdate(WeaponModel model)
    {

    }
  
}
