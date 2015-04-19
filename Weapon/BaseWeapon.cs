using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nstuff.juggerfall.extension.models;
using Random = UnityEngine.Random;

public enum SLOTTYPE { PERSONAL, MAIN, MELEE, GRENADE, HEAD, CHEST };

public enum CharacteristicType
{
    Bool,
    Float,
    Int
}
[System.Serializable]
public class CharacteristicForItems
{
    public CharacteristicList name;
    public EffectType effect;
    public CharacteristicType type;
    public float value;
}
public class BaseWeapon : DestroyableNetworkObject {

    private const float CRIT_MULTIPLIER = 2.0f;
	
	private static LayerMask layer = -123909;
	
	public enum DESCRIPTIONTYPE{NONE,SHOOTGUN,REVOLVER};
	
	public DESCRIPTIONTYPE descriptionType;
	
	public enum AMUNITONTYPE{SIMPLEHIT, PROJECTILE, RAY, HTHWEAPON, AOE};

	public AMUNITONTYPE amunitionType;

	public SLOTTYPE slotType;
	
	public AMMOTYPE ammoType;
    /// <summary>
    /// maxAmmmo for rewriting
    /// </summary>
    public int maxAmmoAmount;

	//ROOT OF ALL EVIL SECTION
	
    /// <summary>
    /// Charge that lowers gun efficiency
    /// </summary>
    private int charge;
	 /// <summary>
    ///How many shoots up charge
    /// </summary>
    public int shootPerCharge = 30;
	
	private int shootCounter= 0;
	
	private int hitCounter=0;
	
	private int totalShootCount=0;

    private static float DAMAGE_BROKE_PERCEN = 10.0f;

    private static float DAMAGE_BROKE_COEF = 5f;

    private static float DAMAGE_BROKE_MAX = 0.5f;

    private static float AIM_BROKE_PERCEN = 30.0f;

    private static float AIM_BROKE_COEF = 5f;

 
    
	//END MONEY SECTION

    public bool initStats = false;
    //SHOOT LOGIC
    /// <summary>
    ///critchance
    /// </summary>
    public int critChance =0;
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
    ///  How Much Crosshair goes up after shoot;
    /// </summary>
    public float aimDisplacment;
    /// <summary>
    ///  How Much Crosshair goes up after shoot;
    /// </summary>
    public float aimDisplacmentAiming;
    /// <summary>
    ///  How Much Crosshair goes up after shoot;
    /// </summary>
    public float aimDisplacmentCooled;
	/// <summary>
    ///  Rand to Shoot direction summary from some effects;
    /// </summary>
	protected float _randShootCoef=0.0f;
	/// <summary>
    /// How cooling move to zero;
    /// </summary>
	public float randCoolingEffect;
    /// <summary>
    ///  How fast aim go to normal
    /// </summary>
    public float aimDisplacmentCooling;
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

	public float curAmmo;
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
    /// <summary>
    ///  Is reload one clip at time during reload timer
    ///      /// </summary>
    public bool isContinuousReload;

    private float reloaderCounter=0.0f;

	protected RifleParticleController rifleParticleController;

	public string attackAnim;

	public string weaponName;
	
	public Texture2D HUDIcon;
	
	public bool init = false;

	public const float MAXDIFFERENCEINANGLE=0.7f;

	private bool shootAfterReload;

    private bool aimAfterReload;

    private bool pumpAfterReload;

    public GameObject[] effectInHand;

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

    public CharacteristicForItems[] passiveSkill;

    protected bool fullyBroken;

    protected BaseWeapon blueprint;

	void Awake(){
		foxView = GetComponent<FoxView>();
        if (projectilePrefab != null)
        {
            projectileClass = projectilePrefab.GetComponent<BaseProjectile>();
            if (projectileClass != null)
            {
                if (projectileClass.CountPooled() == 0 && projectileClass.CountSpawned() == 0)
                {
                    projectileClass.CreatePool(300);
                }
            }
          
        }
		if(shouldDrawTrajectory){
			drawer = GetComponentInChildren<TrajectoryDrawer>();
			drawer.Init(projectileClass);
            drawer.gameObject.SetActive(false);
		}
  
		charge = ItemManager.instance.GetCharge(SQLId);
		shootCounter = ItemManager.instance.GetShootCounter(SQLId);
        blueprint = this.Default();
        Debug.Log(blueprint + "  " +( blueprint == this));
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
        curTransform.parent = owner.GetSlotForItem(slotType);
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
            curAmmo = owner.GetComponent<InventoryManager>().GiveAmmo(ammoType, clipSize - (int)curAmmo);
        }
        InitStats();
      
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
        InitStats();
		


	}
    protected void InitStats()
    {
        if (initStats)
        {
            return;

        }
        initStats = true;
       
        int maxCharge = ItemManager.instance.GetWeaponMaxChargebByID(SQLId);
        int minWear = Mathf.RoundToInt((float)(maxCharge * owner.GetValue(CharacteristicList.MAX_WEAR)) / 100.0f);
        fullyBroken = maxCharge == minWear + charge && maxCharge!=0;

        if (!fullyBroken)
        {


            foreach (CharacteristicForItems oneChar in passiveSkill)
            {
                
                switch (oneChar.type)
                {
                    case CharacteristicType.Bool:
                        {

                            Effect<bool> effect = new Effect<bool>(oneChar.value == 1.0f);
                            effect.endByDeath = true;
                            effect.timeEnd = -1;
                            effect.type = oneChar.effect;
                            owner.AddBaseBuff((int)oneChar.name, effect);
                        }
                        break;
                    case CharacteristicType.Float:
                        {

                            Effect<float> effect = new Effect<float>(oneChar.value);
                            effect.endByDeath = true;
                            effect.timeEnd = -1;
                            effect.type = oneChar.effect;
                            owner.AddBaseBuff((int)oneChar.name, effect);
                        }
                        break;
                    case CharacteristicType.Int:
                        {

                            Effect<int> effect = new Effect<int>((int)oneChar.value);
                            effect.endByDeath = true;
                            effect.timeEnd = -1;
                            effect.type = oneChar.effect;
                            owner.AddBaseBuff((int)oneChar.name, effect);
                        }
                        break;

                }

            }
        }
        RecalculateStats();
    }

    public void RecalculateStats(){
        int maxCharge = ItemManager.instance.GetWeaponMaxChargebByID(SQLId);
        int minWear = Mathf.RoundToInt((float)(maxCharge * owner.GetValue(CharacteristicList.MAX_WEAR)) / 100.0f);
        if (foxView.isMine)
        {
            owner.ivnMan.RewrtieMaxAmmo(maxAmmoAmount, ammoType);
        }

        reloadTime = blueprint.reloadTime * owner.GetPercentValue(CharacteristicList.RELOAD_SPEED);
        float firerate =100f+owner.GetValue(CharacteristicList.FIRE_RATE);
        if (slotType == SLOTTYPE.PERSONAL)
        {
             firerate +=owner.GetValue(CharacteristicList.FIRE_RATE_MAIN);

        }

        fireInterval = blueprint.fireInterval * firerate / 100f;

        clipSize = Mathf.RoundToInt(blueprint.clipSize * owner.GetPercentValue(CharacteristicList.AMMO_AMOUNT));

        float recoilmod = owner.GetValue(CharacteristicList.AIM_ALL);
       /* switch (descriptionType)
        {



            case DESCRIPTIONTYPE.MACHINE_GUN:
                recoilmod += owner.GetValue(CharacteristicList.RECOIL_MACHINEGUN);
                break;
            case DESCRIPTIONTYPE.ROCKET_LAUNCHER:
                recoilmod += owner.GetValue(CharacteristicList.RECOIL_ROCKET);
                break;
        }*/
        if (slotType == SLOTTYPE.PERSONAL)
        {
            recoilmod +=  owner.GetValue(CharacteristicList.AIM_DOP);
        }
        if (slotType == SLOTTYPE.MAIN)
        {
            recoilmod += owner.GetValue(CharacteristicList.AIM_MAIN);
        }
        recoilmod = ((float)recoilmod) / 100f + 1f;
     
      
        float damageAdd = owner.GetValue(CharacteristicList.DAMAGE_ALL);
        if (slotType == SLOTTYPE.PERSONAL)
        {
            damageAdd += owner.GetValue(CharacteristicList.DAMAGE_ADD_DOP);
        }
        if (slotType == SLOTTYPE.MAIN)
        {
            damageAdd += owner.GetValue(CharacteristicList.DAMAGE_ADD_MAIN);
        }
        
        damageAmount.Damage = blueprint.damageAmount.Damage * ((float)damageAdd / 100f + 1);

        damageAdd = 0;
        if (slotType == SLOTTYPE.PERSONAL)
        {
            damageAdd += owner.GetValue(CharacteristicList.DAMAGE_ADD_DOP_POINT);
        }
        if (slotType == SLOTTYPE.MAIN)
        {
            damageAdd += owner.GetValue(CharacteristicList.DAMAGE_ADD_MAIN_POINT);
        }


        damageAmount.Damage += damageAdd;
        
        float  vsarmor = 0;
        if (slotType == SLOTTYPE.PERSONAL)
        {
            vsarmor += owner.GetValue(CharacteristicList.VS_ARMOR_DOP);
        }
        if (slotType == SLOTTYPE.MAIN)
        {
            vsarmor += owner.GetValue(CharacteristicList.VS_ARMOR_MAIN);
        }

        damageAmount.vsArmor = blueprint.damageAmount.vsArmor +vsarmor;
        if (SQLId > 0)
        {
            damageAmount.weapon = true;
          
             float damageMode = ((float)charge - minWear) / DAMAGE_BROKE_PERCEN * DAMAGE_BROKE_COEF / 100;
            if (damageMode > 0)
            {
                if (damageMode > DAMAGE_BROKE_MAX)
                {
                    damageMode = DAMAGE_BROKE_MAX;
                }

                damageAmount.Damage -= damageAmount.Damage * damageMode;
            }
          
                float aimMode = ((float)charge - minWear) / AIM_BROKE_PERCEN * AIM_BROKE_COEF / 100;
            if (damageMode > 0)
            {
                recoilmod += aimMode * recoilmod;
            }
           
        }
        aimRandCoef = blueprint.aimRandCoef * recoilmod;
        normalRandCoef = blueprint.normalRandCoef * recoilmod;
        float randPerShootMod = owner.GetValue(CharacteristicList.RECOIL_ALL);
        if (slotType == SLOTTYPE.PERSONAL)
        {
            randPerShootMod += owner.GetValue(CharacteristicList.RECOIL_DOP);
        }
        if (slotType == SLOTTYPE.MAIN)
        {
            randPerShootMod += owner.GetValue(CharacteristicList.RECOIL_MAIN);
        }
        if (descriptionType == DESCRIPTIONTYPE.REVOLVER)
        {
            randPerShootMod += owner.GetValue(CharacteristicList.RECOIL_REVOLVER);
        }
        if (descriptionType == DESCRIPTIONTYPE.SHOOTGUN)
        {
            randPerShootMod += owner.GetValue(CharacteristicList.RECOIL_SHOOTGUN);
        }
        randPerShoot = blueprint.randPerShoot * ((float)randPerShootMod / 100f + 1);

        if (slotType == SLOTTYPE.MAIN)
        {
            weaponRange = blueprint.weaponRange * owner.GetPercentValue(CharacteristicList.DAMAGE_ADD_MAIN);
            weaponMinRange = blueprint.weaponMinRange * owner.GetPercentValue(CharacteristicList.DAMAGE_ADD_MAIN);
        }
        critChance =blueprint.critChance + owner.GetValue(CharacteristicList.CRIT_CHANCE_ALL);
        if (slotType == SLOTTYPE.MAIN)
        {
            critChance += owner.GetValue(CharacteristicList.CRIT_CHANCE_MAIN);
        }

        shootPerCharge = Mathf.RoundToInt((float)blueprint.shootPerCharge * owner.GetPercentValue(CharacteristicList.WEAR));
    }
	
	public void PawnDeath(){
		if(foxView.isMine&&!owner.isAi){
			ItemManager.instance.SetShootCount(SQLId,shootCounter);
			StatisticManager.instance.AmmoSpent(totalShootCount);
			StatisticManager.instance.AmmoHit(hitCounter);
		}
	}
	public void RemoteAttachWeapon(Pawn newowner,bool state){
		if(state){
            Debug.Log(name + " EQUIP");
            newowner.setWeapon(this); 
		}else{
            AttachWeaponToChar(newowner);
		}
	}
    void Update()
    {
		UpdateCahedPosition();
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
   protected void UpdateWeapon(float deltaTime){
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
            reloadTimer -= deltaTime;
            if (isContinuousReload)
            {
                reloaderCounter += deltaTime * ((float)clipSize) / reloadTime;
                Reload();
                if (reloadTimer < 0)
                {
                    //double for time accuracy of float
                    reloaderCounter=1.0f;
                    Reload();
                    ReloadFinish();
                }
                if(isShooting&&curAmmo>=1){
                    ReloadFinish();
                }

            }
            else
            {
                if (reloadTimer < 0)
                {
                    Reload();
                }
               
                
            }
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
            if (curAmmo == clipSize)
            {
                return;
            }
			isReload= true;
            if (isAimingFPS)
            {
                aimAfterReload = owner.isAiming;
                if (aimAfterReload)
                {
                    owner.ToggleAim(false);
                }

            }
            if (isContinuousReload)
            {
                reloadTimer = reloadTime *((float)clipSize - curAmmo)/(float)clipSize;
            }
            else
            {
                reloadTimer = reloadTime;
            }
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
        owner.animator.ReloadStop(true);
    }

    public void StopReload(bool needIk=true)
    {
        isReload = false;
        shootAfterReload = false;
        owner.animator.ReloadStop(needIk);
    }
    public void ReloadFinish()
    {
        isReload = false;
        owner.animator.ReloadStop(true);
        if (shootAfterReload)
        {
            shootAfterReload = false;
            //if (IsFired != null) IsFired(this, EventArgs.Empty);
            StartFire();
        }
        if (pumpAfterReload)
        {
            pumpAfterReload = false;
            StartPumping();
        }
        if (owner.player != null)
        {
            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventPawnReload", owner.player);
        }
    }
	public virtual void Reload(){
        if (isContinuousReload)
        {
            if (reloaderCounter >= 1.0f)
            {
                int oldClip = (int)curAmmo;
                if (curAmmo >= clipSize)
                {
                    return;
                }
                curAmmo = owner.GetComponent<InventoryManager>().GiveAmmo(ammoType, 1) + oldClip;
                if (curAmmo == oldClip)
                {
                   
                    ReloadFinish();
                }
                reloaderCounter = 0.0f;
            }
        }
        else
        {
            //owner.animator.ReloadStop();
            int oldClip = (int)curAmmo;
            curAmmo = owner.GetComponent<InventoryManager>().GiveAmmo(ammoType, clipSize - (int)curAmmo) + oldClip;
            ReloadFinish();
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
		if(foxView.isMine&&!owner.isAi){
			shootCounter++;
			totalShootCount++;
			if(shootCounter>=shootPerCharge){
				shootCounter= 0;
				charge = ItemManager.instance.LowerCharge(SQLId);
			}
            owner.AfterShoot();
		}
		
    }
    public float GetAimDisplacment(bool isAiming)
    {
        if (isAiming)
        {
            return aimDisplacmentAiming;
        }
        else
        {
            return aimDisplacment;
        }
    }
	
	public virtual void HitWithProjectile(){
		if(foxView.isMine&&!owner.isAi){
			hitCounter++;
			
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
				BaseDamage dmg =GetDamage(damageAmount);
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


        projScript.damage = GetDamage(damageAmount, power);
      
		projScript.owner = owner.gameObject;
		
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

    public virtual void PutAway()
    {
		enabled = false;
		curTransform.parent = owner.GetSlotForItem(slotType);
        curTransform.localPosition = Vector3.zero;
        curTransform.localRotation = Quaternion.identity;
		curTransform.localScale=  Vector3.one;
		StopFire();
        StopReload(false);
		if(foxView.isMine){
			foxView.PutAway();
		}
        foreach (GameObject obj in effectInHand)
        {
            obj.SetActive(false);
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
	public virtual void TakeInHand(Transform weaponSlot,Vector3 Offset, Quaternion weaponRotator){
		enabled = true;
		curTransform.parent = weaponSlot;
        curTransform.localScale = Vector3.one;
		curTransform.localPosition = Offset;
		//Debug.Log (name + weaponRotator);
		curTransform.localRotation = weaponRotator;
        owner.animator.SetMuzzle(muzzlePoint);
        foreach (GameObject obj in effectInHand)
        {
            obj.SetActive(true);
        }
	}
    public virtual bool IsMelee()
    {
        return false;
    }
    public virtual void StartDamage()
    {

    }
    public BaseDamage GetDamage(BaseDamage dmg,float addDamage=0)
    {
         BaseDamage lDamage = new BaseDamage(damageAmount);
        
         lDamage.Damage += addDamage;
         if (critChance > 0)
         {
             float dice = UnityEngine.Random.Range(0, 100f);
             if (dice < critChance)
             {
                 lDamage.Damage *= CRIT_MULTIPLIER;
                 lDamage.crit = true;
             }
         }
         return lDamage;
    }
}

