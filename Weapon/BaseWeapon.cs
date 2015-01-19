using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nstuff.juggerfall.extension.models;
using Random = UnityEngine.Random;


public class BaseWeapon : DestroyableNetworkObject {
	
	private static LayerMask layer = -123909;
	
	public enum DESCRIPTIONTYPE{NONE,MACHINE_GUN,ROCKET_LAUNCHER};
	
	public DESCRIPTIONTYPE descriptionType;
	
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

    private float lastShootTime;
    
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
    ///  Temp Target for guidance system
    /// </summary>
	protected Transform guidanceTarget= null;
	/// <summary>
    ///  Can be lock on friendly target
    /// </summary>
	public bool isFriendlyGuide;
	/// <summary>
    ///  One shoot pump do you need pump every shot;
    /// </summary>
	public bool oneShootPump =false;	
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
    /// flag that tells if our gun have fps in aiming mode
    /// </summary>
	public bool isAimingFPS;
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
    /// How cooling move to zero when not firing
    /// </summary>
    public float randCoolingEffectNotFire;
    
	/// <summary>
    ///MAximum of all random Effect;
    /// </summary>
	public float maxRandEffect;

    /// <summary>
    ///FOV if aiming with this weapon
    /// </summary>
    public float aimFOV;
	
	public bool shouldDrawTrajectory;
	
	protected TrajectoryDrawer drawer;

	public GameObject projectilePrefab;

    public BaseProjectile projectileClass;
	
	public GameObject pickupPrefabPrefab;

	public Transform muzzlePoint;

	public Transform leftHandHolder;

	protected Vector3 	muzzleOffset;

	public float weaponRange;

    public float weaponMinRange;

	public int curAmmo;
    /// <summary>
    ///  Ammo that ready to shot in barrel in other word
    /// </summary>
    protected int alredyGunedAmmo;

    /// <summary>
    ///  Ammo that ready to shot in barrel in other word maximum
    /// </summary>
    public int alredyGunedAmmoMax;

     /// <summary>
    ///  How hard camera shake when shoot
    /// </summary>
    public float camRecoilMod;

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
	
	public bool init = false;

	public const float MAXDIFFERENCEINANGLE=0.7f;

	private bool shootAfterReload;

    private bool aimAfterReload;

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


    public Vector3 muzzleCached;

    public Vector3 muzzleCachedforward;
    public event EventHandler FireStarted;
    public event EventHandler FireStoped;

   
	void Awake(){
		foxView = GetComponent<FoxView>();
        if (projectilePrefab != null)
        {
            projectileClass = projectilePrefab.GetComponent<BaseProjectile>();
            if (projectileClass.CountPooled() == 0)
            {
                projectileClass.CreatePool(50);
            }
        }
		if(shouldDrawTrajectory){
			drawer = GetComponentInChildren<TrajectoryDrawer>();
			drawer.Init(projectileClass);
            drawer.gameObject.SetActive(false);
		}
  
  
	}

    public Pawn GetOwner()
    {
        return owner;
    }
	// Use this for initialization
	protected void Start () {
		aSource = GetComponent<AudioSource> ();
		sControl = new soundControl (aSource);//создаем обьект контроллера звука
        damageAmount.shootWeapon = SQLId;
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
    public void AttachWeaponToChar(Pawn newowner)
    {
        if (curTransform == null)
        {
            curTransform = transform;
        }
        owner = newowner;
        if (owner == null)
        {
            //Destroy(photonView);
            //Debug.Log("DestoroyATTACHEs");
            Destroy(gameObject);
            return;
        }
        curTransform.parent = owner.GetSlotForWeapon(slotType);
        curTransform.localPosition = Vector3.zero;
        curTransform.localRotation = Quaternion.identity;
        curTransform.localScale = Vector3.one;
        if (rifleParticleController != null)
        {
            rifleParticleController.SetOwner(owner.collider);
        }
        init = true;
        enabled = false;
        if(foxView.isMine){
            curAmmo = owner.GetComponent<InventoryManager>().GiveAmmo(ammoType, clipSize - curAmmo);
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

		TakeInHand(weaponSlot,Offset,weaponRotator);
	
        
		//RemoteAttachWeapon(inowner);
		reloadTime =reloadTime*owner.GetPercentValue(CharacteristicList.RELOAD_SPEED);
        fireInterval = fireInterval * owner.GetPercentValue(CharacteristicList.FIRE_RATE);

        clipSize =  Mathf.RoundToInt(clipSize * owner.GetPercentValue(CharacteristicList.AMMO_AMOUNT));
				
        float recoilmod = owner.GetValue(CharacteristicList.RECOIL_ALL);	
		switch(descriptionType){
			
				
			
			case DESCRIPTIONTYPE.MACHINE_GUN:
                recoilmod += owner.GetValue(CharacteristicList.RECOIL_MACHINEGUN);	
			break;
            case DESCRIPTIONTYPE.ROCKET_LAUNCHER:
            recoilmod += owner.GetValue(CharacteristicList.RECOIL_ROCKET);	
			break;
		}
		recoilmod =((float)recoilmod)/100f+1f;
		normalRandCoef=normalRandCoef*recoilmod;
		aimRandCoef = aimRandCoef*recoilmod;
        damageAmount.Damage = damageAmount.Damage * owner.GetPercentValue(CharacteristicList.DAMAGE_ALL);
		damageAmount.weapon= true;
	}

	public void RemoteAttachWeapon(Pawn newowner,bool state){
		if(state){
            newowner.setWeapon(this); 
		}else{
            AttachWeaponToChar(newowner);
		}
	}
    void Update()
    {
        UpdateWeapon(Time.deltaTime);
        if (foxView.isMine)
        {
            if (isShooting)
            {
                owner.animator.StartShootAniamtion("shooting");
            }
            else
            {
                owner.animator.StopShootAniamtion("shooting");

            }
			if(shouldDrawTrajectory&&drawer!=null&&drawer.gameObject.activeSelf){
                drawer.Draw(muzzleCached + muzzleOffset, getAimRotation(), GetRandomeDirectionCoef());
			}
        }
        else
        {
            if (lastShootTime + 0.5f < Time.time)
            {
                owner.animator.StopShootAniamtion("shooting");
            }else
            {
                owner.animator.StartShootAniamtion("shooting");
            }

        }

    }
	//Detected if replication should continue logic loop;
	protected virtual bool ReplicationContinue(){
		return !foxView.isMine;
	}
	// Update is called once per frame
    public void UpdateCahedPosition()
    {
        if (muzzlePoint != null)
        {
            muzzleCached = muzzlePoint.position;
            muzzleCachedforward = muzzlePoint.forward;
        }
      
    }
    void UpdateWeapon(float deltaTime){
		if(init&&owner==null) {
			RequestKillMe();

		}
        if (_randShootCoef > 0)
        {
            if (isShooting)
            {
                _randShootCoef -= randCoolingEffect * deltaTime;
            }
            else
            {
                _randShootCoef -= randCoolingEffectNotFire * deltaTime;
            }
        }
		//AimFix ();
		if(ReplicationContinue()){
			return;
		}

		if(isReload){
			if(reloadTimer<0){
				Reload();
			}
            reloadTimer -= deltaTime;
			return;
		}
        if (fireTimer >= 0)
        {
            fireTimer -= deltaTime;
        }
        switch(prefiretype){
            case PREFIRETYPE.Normal:
		        if (isShooting) {
                    ShootTick(deltaTime);
		        }
              

                break;
            default:
                if (isShooting||isPumping)
                {
                    if (_pumpAmount >= pumpAmount)
                    {
						
                        if (isShooting)
                        {
                            ShootTick(deltaTime);

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
									selfdamage.sendMessage= false;
                                    owner.Damage(selfdamage, owner.gameObject);
                                    break;
                            }
                        }
                       

                    }
                    else
                    {
						switch (prefiretype)
                        {
                            case PREFIRETYPE.Salvo:
								Pumping();
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
							case PREFIRETYPE.Guidance:
								if(guidanceTarget==owner.curLookTarget){
									Pumping();
								}else{
									if(guidanceTarget==null){
										Pawn target = owner.curLookTarget.GetComponent<Pawn>();
										if(target!=null&&(isFriendlyGuide||target.team!=owner.team)){
                                            guidanceTarget = owner.curLookTarget;
										}									
									}
								}
                                Debug.Log(guidanceTarget);
							break;
							default:
								Pumping();
								if (curAmmo == 0) {
                                    ReloadStart();
                                }
                               
								break;
                         }
                     
                    }
                }
                break;
        }
        
     
	}
	private void Pumping(){
		_pumpCoef += Time.deltaTime*pumpCoef;
        _pumpAmount += Time.deltaTime;
	}

    protected virtual void ShootTick(float deltaTime)
    {
       
        if (fireTimer <= 0)
        {
           
            fireTimer = fireInterval;
            Fire();
        }
        if (_waitForRelease && !waitForRelease)
        {
            _releaseInterval += deltaTime;
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
                if (fireTimer <= 0)
                {
                    fireTimer = fireInterval;
                    Fire(true);
                }
                break;
        }
		isShooting = false;
		shootAfterReload = false;
		ReleaseFire ();
        _waitForRelease = false;
        _amountInShoot = 0;
        alredyGunedAmmo = 0;
      
        if (!isPumping) {
            _pumpCoef = 0.0f;
            _pumpAmount = 0.0f;
            guidanceTarget = null;
        }

    }
	
	public virtual void ReloadStart(){
		if (isReload) {
			return;
		}
        alredyGunedAmmo = 0;
		if(owner.GetComponent<InventoryManager>().HasAmmo(ammoType)){
			isReload= true;
            if (isAimingFPS)
            {
                aimAfterReload = owner.isAiming;
                if (aimAfterReload)
                {
                    owner.ToggleAim(false);
                }

            }
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
    public void OnDestroy()
    {
        owner.animator.ReloadStop();
    }
	public virtual void Reload(){
      
		isReload = false;
        owner.animator.ReloadStop();
		int oldClip = curAmmo;
		curAmmo =owner.GetComponent<InventoryManager>().GiveAmmo(ammoType,clipSize-curAmmo)+oldClip;	
		if (shootAfterReload) {
			shootAfterReload=false;
      //if (IsFired != null) IsFired(this, EventArgs.Empty);
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

    public float ReloadProgress(){
    
        if(reloadTime==0){
            return 0;
        }
        return (reloadTime - reloadTimer) / reloadTime;
       
    }
	public float ReloadTimer(){
		if (isReload) {
			return reloadTimer;
		}
		return 0.0f;
	}
    public float PumpCoef()
    {
        return _pumpAmount;
    }
	
	public bool AfterActing(){
		if(afterPumpAction!=AFTERPUMPACTION.Wait){
			return _pumpAmount>=pumpAmount&&!isShooting;
		}
		return false;
	
	}
	
	public bool PumpIsActing(){
		switch (prefiretype)
        {
            case PREFIRETYPE.Normal:
				return false;
		    case PREFIRETYPE.ChargedPower:
			case PREFIRETYPE.ChargedAccuracy:
			
			case PREFIRETYPE.ChargedRange:
                return _pumpCoef >= 1.0f;
                
			case PREFIRETYPE.Salvo:
                return _pumpAmount / pumpCoef >= 1.0f;
               
			 case PREFIRETYPE.Spooling:
				return _pumpAmount>=pumpAmount;
              
             case PREFIRETYPE.Guidance:
                return _pumpAmount >= pumpAmount;
               
			default:
               	return false;
               
        }
	}
	//temporary function to fix wrong aiming
	public virtual void AimFix(){

	}
	protected void Fire(bool fromReload = false){
		if (!CanShoot ()) {
			return;		
		}
  
        LogicShoot();
        
		if(curAmmo>0){
			curAmmo--;
		}else{
            if (alredyGunedAmmo <= 0)
            {
                if (clipSize > 0 )
                {
                    if (!fromReload) { 
                        ReloadStart();
                    }
                    return;
                }
            }
            else
            {
               // alredyGunedAmmo--;
            }
		}
			//играем звук стрельбы
		
		if (rifleParticleController != null) {
			rifleParticleController.CreateShootFlame ();
		}
		
		owner.shootEffect();
        FiredEffect();
        if (alredyGunedAmmo > 0)
        {
            curAmmo++;
            for (int i = 0; i < alredyGunedAmmo; i++)
            {
                ActualFire();
            }
            IncreseRandFromShoot(alredyGunedAmmo);
			
          
        }
        else
        {
            ActualFire();
            IncreseRandFromShoot(1);
			
        }
	
		owner.HasShoot ();
        AfterShootLogic();
		//photonView.RPC("FireEffect",PhotonTargets.Others);
	}

    private void IncreseRandFromShoot(int count)
    {
        if (randPerShoot > 0)
        {
            if (!IsRandMax())
            {
                _randShootCoef += count * randPerShoot;
            }
        }
    }
    protected void FiredStopEffect()
    {

        if (FireStoped != null) FireStoped(this, EventArgs.Empty);
    }
    protected void FiredEffect()
    {

		
        if (FireStarted != null&&(!foxView.isMine||!isAimingFPS||!owner.isAiming))
        {
            FireStarted(this, EventArgs.Empty);
            //Debug.Log("fire" + FireStarted);
        }
    }

    protected virtual void ActualFire()
    {
       
       // Debug.Log("fire");
	   if(foxView.isMine){
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
				case AMUNITONTYPE.AOE:
					   sControl.playClip(fireSound);
					DoSimpleDamage();
					break;
				case AMUNITONTYPE.HTHWEAPON:
					sControl.playClip(fireSound);
					owner.animator.StartAttackAnim(attackAnim);
					ChangeWeaponStatus(true);
					break;

			}
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
		if(oneShootPump){
			_pumpCoef=0;
            guidanceTarget = null;
		}

    }

    public virtual void AfterShootLogic()
    {
        if (oneShootPump)
        {
            alredyGunedAmmo = 0;
        }
		
    }
	public virtual bool CanShoot (){
        if (_waitForRelease)
        {
           
            return false;
        }
	  if (muzzlePoint==null)
	    return true;
        Vector3 aimDir = (owner.getCachedAimRotation() - muzzleCached).normalized;
        Vector3 realDir = muzzleCachedforward;
		float angle = Vector3.Dot (aimDir, realDir);

		if (angle < MAXDIFFERENCEINANGLE) {
           /// Debug.Log("angle");
			//return false;		
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
    public virtual void StartFireRep()
    {

    }
	protected virtual void DoSimpleDamage(){
        Vector3 startPoint = muzzleCached + muzzleOffset;
		Quaternion startRotation = getAimRotation();
	
		float effAimRandCoef = GetRandomeDirectionCoef();
		
		if (effAimRandCoef > 0) {
			startRotation = Quaternion.Euler (startRotation.eulerAngles + new Vector3 (Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef)));
		}
	
		Vector3 direction = startRotation*Vector3.forward;
		Ray centerRay=new Ray(startPoint,direction);
		RaycastHit hitInfo;
		float power=0;
		float range = 0;
		
		switch(prefiretype){
			case PREFIRETYPE.ChargedPower:
				power += _pumpCoef;
			break;
			case PREFIRETYPE.ChargedRange:
				range += _pumpCoef;
			break;		
		}
     
		if (Physics.Raycast (centerRay, out hitInfo, weaponRange+range)) {
		
			HitEffect(hitInfo,power);
			rifleParticleController.CreateLine(startPoint,hitInfo.point);
		}else{
			rifleParticleController.CreateRay(startPoint,direction);
		}
	}
	
	/// <summary>
    /// Hit logic for SimpleDamage
    /// </summary>
	protected virtual void HitEffect(RaycastHit hitInfo,float power){
			DamagebleObject target =(DamagebleObject) hitInfo.collider.GetComponent(typeof(DamagebleObject));
			if(target!=null){
				BaseDamage dmg =new BaseDamage(damageAmount);
				dmg.Damage += power;
				target.Damage(dmg ,owner.gameObject);
			}
	}
    public virtual void RemoteSimpleHit(Vector3 position, Quaternion rotation, float power, float range, float minRange, int viewId, int projId, long timeShoot)
    {
		Vector3 direction = rotation *Vector3.forward;
		Ray centerRay=new Ray(position,direction);
		RaycastHit hitInfo;
		if (Physics.Raycast (centerRay, out hitInfo, weaponRange+range)) {
		
			HitEffect(hitInfo,power);
            rifleParticleController.CreateLine(position, hitInfo.point);
		}else{
            rifleParticleController.CreateRay(position, direction);
		}
	}
	/// <summary>
    /// Generate random distribution coef for projectile
    /// </summary>
	public float GetRandomeDirectionCoef(){
		float effAimRandCoef =0;
		
		if (owner.isAiming) {
			effAimRandCoef+=aimRandCoef;
		}else{
			effAimRandCoef+=normalRandCoef;
		}
		effAimRandCoef*= owner.AimingCoefMultiplier ();

        effAimRandCoef += _randShootCoef;
		effAimRandCoef+= owner.AimingCoef ();
		
		if(effAimRandCoef>maxRandEffect){
			effAimRandCoef =maxRandEffect;
		}
		if(prefiretype==PREFIRETYPE.ChargedAccuracy){
			effAimRandCoef-= _pumpCoef;
			
		}
        if (effAimRandCoef < 0)
        {
            effAimRandCoef = 0;
        }
		return effAimRandCoef;
	}
    public bool IsRandMax()
    {
        float effAimRandCoef = 0;
        if (owner.isAiming)
        {
            effAimRandCoef += aimRandCoef;
        }
        else
        {
            effAimRandCoef += normalRandCoef;
        }
		effAimRandCoef*= owner.AimingCoefMultiplier ();

        effAimRandCoef += _randShootCoef;
        effAimRandCoef += owner.AimingCoef();
		
        return effAimRandCoef > maxRandEffect;
        
    }
	
	protected virtual void GenerateProjectile(){
		Vector3 startPoint  = muzzleCached+muzzleOffset;
		Quaternion startRotation = getAimRotation();
		GameObject proj;
		float effAimRandCoef = GetRandomeDirectionCoef();
		
		if (effAimRandCoef > 0) {
			startRotation = Quaternion.Euler (startRotation.eulerAngles + new Vector3 (Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef)));
		}
       // Debug.DrawLine(transform.position, startPoint, Color.red,10 );
   
		proj=projectilePrefab.Spawn(startPoint,startRotation);
        //Debug.DrawLine(transform.position,proj.transform.position, Color.blue, 10);
		BaseProjectile projScript =proj.GetComponent<BaseProjectile>();
		float power=0;
		float range = weaponRange;
        float minRange = weaponMinRange;
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
					Transform target = 	guidanceTarget;
                    
					
					if(target!=null){
						projScript.target = target;
						viewId = target.GetComponent<FoxView>().viewID;
                        Pawn targPawn = target.GetComponent<Pawn>();
                        if(targPawn!=null){
                           // Debug.Log(targPawn.headOffset);
                            projScript.targetOffset = targPawn.headOffset/2;
                        }
					}
				}
			break;
		}
     
        projScript.projId = ProjectileManager.instance.GetNextId();
        projScript.replication = false;
		if (foxView.isMine) {
            foxView.PrepareShoot(startPoint, startRotation, power, range, weaponMinRange, viewId, projScript.projId);
		}
		
		
		projScript.damage =new BaseDamage(damageAmount) ;
		projScript.owner = owner.gameObject;
		projScript.damage.Damage+=power;
		projScript.range=range;
        projScript.minRange = minRange;
        projScript.Init();
	}
	public virtual void  RemoteShot(Vector3 position, Quaternion rotation, float power, float range,float minRange, int viewId, int projId, long timeShoot)
    {
		switch (amunitionType)
        {
            case AMUNITONTYPE.SIMPLEHIT:
                RemoteSimpleHit(position, rotation, power, range, minRange,viewId, projId, timeShoot);
                break;
            case AMUNITONTYPE.PROJECTILE:
				RemoteGenerate(position, rotation, power, range,minRange, viewId, projId, timeShoot);
                break;
      
          

        }
	}
    public virtual void RemoteGenerate(Vector3 position, Quaternion rotation, float power, float range,float minRange, int viewId, int projId, long timeShoot)
    {
        lastShootTime = Time.time;
        BaseProjectile proj = GenerateProjectileRep(position, rotation, timeShoot);
			if (rifleParticleController != null) {
				rifleParticleController.CreateShootFlame ();
			}
            proj.projId = projId;
			proj.damage.Damage+=power;
			proj.range=range;
            proj.minRange = minRange;
			switch(prefiretype){
				case PREFIRETYPE.Guidance:
					if(viewId!=0){
						Transform target =NetworkController.GetView(viewId).GetComponent<Transform>();
				
					
						proj.target = target;
                        Pawn targPawn = target.GetComponent<Pawn>();
                        if (targPawn != null)
                        {
                            //Debug.Log(targPawn.headOffset);
                            proj.targetOffset = targPawn.headOffset / 2;
                        }
					}
				break;
			}
            proj.Init();

		
	}
    protected BaseProjectile GenerateProjectileRep(Vector3 startPoint, Quaternion startRotation, long timeShoot)
    {

		GameObject proj=projectilePrefab.Spawn(startPoint,startRotation);
		BaseProjectile projScript = proj.GetComponent<BaseProjectile>();
        projScript.lateTime = timeShoot;

        projScript.damage =new BaseDamage(damageAmount) ;
		projScript.owner = owner.gameObject;
		return projScript;
	}
  
	protected virtual Quaternion getAimRotation(){
		/*Vector3 randVec = Random.onUnitSphere;
		Vector3 normalDirection  = owner.getAimRotation(weaponRange)-muzzlePoint.position;
		normalDirection =normalDirection + randVec.normalized * normalDirection.magnitude * aimRandCoef / 100;*/
        if (projectileClass != null)
        {

            return Quaternion.LookRotation(owner.getAimpointForWeapon(projectileClass.startImpulse) - muzzleCached);
        }
        else
        {
            return Quaternion.LookRotation(owner.getAimpointForWeapon() - muzzleCached);
        }
		

	}
	
	
	public float MuzzleOffset(){
        if (muzzlePoint != null)
        {
            return (muzzlePoint.position + muzzleOffset - curTransform.position).sqrMagnitude;
        }
        else
        {
            return 0.0f;
        }
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
	
	public bool ToggleAim(bool value){
        aimAfterReload = value;
        
		if(shouldDrawTrajectory&&foxView.isMine){
			drawer.gameObject.SetActive(value);
		}
        return isReload;
	}
	
	public void PutAway(){
		enabled = false;
		curTransform.parent = owner.GetSlotForWeapon(slotType);
        curTransform.localPosition = Vector3.zero;
        curTransform.localRotation = Quaternion.identity;
		curTransform.localScale=  Vector3.one;
		StopFire();
		if(foxView.isMine){
			foxView.PutAway();
		}
	}
	public void TakeInHand(){
		enabled = true;
		if(foxView.isMine){
			//owner.ForcedWeaponAttach(this);
			foxView.TakeInHand();
		}else{
			owner.setWeapon(this);
		}
	}
	public void TakeInHand(Transform weaponSlot,Vector3 Offset, Quaternion weaponRotator){
		enabled = true;
		curTransform.parent = weaponSlot;
        curTransform.localScale = Vector3.one;
		curTransform.localPosition = Offset;
		//Debug.Log (name + weaponRotator);
		curTransform.localRotation = weaponRotator;
	}

}
