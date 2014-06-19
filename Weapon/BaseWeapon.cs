using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class BaseWeapon : DestroyableNetworkObject {
	protected class ShootData{
		public double timeShoot;
		public Quaternion  direction;
		public Vector3 position;
		public float power;
		public float range;
		public int viewId;
		public void PhotonSerialization(PhotonStream stream){
			stream.SendNext (power);

			stream.SendNext( range);
			stream.SendNext( viewId);
			stream.SendNext (timeShoot);

			stream.SendNext( direction);
			ServerHolder.WriteVectorToShort (stream, position);
		}
		public void PhotonDeserialization(PhotonStream stream){
			power= (float)stream.ReceiveNext ();
			range= (float)stream.ReceiveNext ();
			timeShoot = (double)stream.ReceiveNext ();
			direction = (Quaternion)stream.ReceiveNext ();
			position = ServerHolder.ReadVectorFromShort (stream);
		}
	}

	private Queue<ShootData> shootsToSend = new Queue<ShootData>();

	protected Queue<ShootData> shootsToSpawn = new Queue<ShootData>();

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

	public Vector3 	muzzleOffset;

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
		photonView = GetComponent<PhotonView>();
		rifleParticleController = GetComponentInChildren<RifleParticleController>();
	
		if (rifleParticleController != null&&photonView.isMine) {
			rifleParticleController.SetOwner (owner.collider);
		}
	}

	public void AttachWeapon(Transform weaponSlot,Vector3 Offset, Quaternion weaponRotator,Pawn inowner){
		if (curTransform == null) {
			curTransform = transform;		
		}
		if (photonView == null) {
			photonView = GetComponent<PhotonView>();
		}
		owner = inowner;

		curTransform.parent = weaponSlot;
		curTransform.localPosition = Offset;
		//Debug.Log (name + weaponRotator);
		curTransform.localRotation = weaponRotator;
		if (photonView.isMine) {
			photonView.RPC("AttachWeaponRep",PhotonTargets.OthersBuffered,inowner.photonView.viewID);
		}
	}

	[RPC]
	public void AttachWeaponRep(int viewid){
		if (curTransform == null) {
			curTransform = transform;		
		}
		owner =PhotonView.Find (viewid).GetComponent<Pawn>();
		if (owner == null) {
			//Destroy(photonView);
			Debug.Log ("DestoroyATTACHEs");
			Destroy(gameObject);
		}
		owner.setWeapon (this);
		if (rifleParticleController != null) {
			rifleParticleController.SetOwner (owner.collider);
		}
		init = true;
	}

	// Update is called once per frame
	void Update () {
		if(init&&owner==null) {
			RequestKillMe();

		}
		//AimFix ();
		if (!photonView.isMine) {
			ReplicationGenerate ();
			return;
		}

		if(isReload){
			if(reloadTimer<0){
				Reload();
			}
			reloadTimer-=Time.deltaTime;
			return;
		}
        switch(prefiretype){
            case PREFIRETYPE.Normal:
		        if (isShooting) {
                    ShootTick();
		        }
                if (fireTimer >= 0)
                {
                    fireTimer -= Time.deltaTime;
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
                                    _afterPumpAmount += Time.deltaTime*afterPumpCoef;
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
                                    selfdamage.Damage = Time.deltaTime*afterPumpCoef;
                                    owner.Damage(selfdamage, owner.gameObject);
                                    break;
                            }
                        }
                        if (fireTimer >= 0)
                        {
                            fireTimer -= Time.deltaTime;
                        }

                    }
                    else
                    {
						_pumpCoef += Time.deltaTime*pumpCoef;
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
                        _pumpAmount += Time.deltaTime;
                    }
                }
                break;
        }
		_randShootCoef-=randCoolingEffect*Time.deltaTime;
     
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
        fireTimer = fireInterval;
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
			DamagebleObject target =(DamagebleObject) hitInfo.collider.GetComponent(typeof(DamagebleObject));
			if(target!=null){
				target.Damage(new BaseDamage(damageAmount) ,owner.gameObject);
			}
		}
	}
	/// <summary>
    /// Generate random distribution coef for projectile
    /// </summary>
	protected float GetRandomeDirectionCoef(){
		float effAimRandCoef = _randShootCoef;
		if (owner.isAiming) {
			effAimRandCoef=aimRandCoef;
		}else{
			effAimRandCoef=normalRandCoef;
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
	protected Pawn GetGuidanceTarget(){
		Transform target  = owner.curLookTarget;
		if(target!=null){
			return target.GetComponent<Pawn>();
		}
		return null;
	}
	protected virtual void GenerateProjectile(){
		Vector3 startPoint  = muzzlePoint.position+muzzleOffset;
		Quaternion startRotation = getAimRotation();
		GameObject proj;
		float effAimRandCoef = GetRandomeDirectionCoef();
		
		if (effAimRandCoef > 0) {
			startRotation = Quaternion.Euler (startRotation.eulerAngles + new Vector3 (Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef)));
		}
		proj=Instantiate(projectilePrefab,startPoint,startRotation) as GameObject;
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
					Pawn target = GetGuidanceTarget();
					GuidanceProjectile guidanceScript  = proj.GetComponent<GuidanceProjectile>();
					if(guidanceScript!=null&&Pawn!=null){
						guidanceScript.target = target;
						viewId = target.photonView.viewID;
					}
				}
			break;
		}
		if (photonView.isMine) {
			SendShoot(startPoint,startRotation,power,range,viewId);
		}
		
		BaseProjectile projScript =proj.GetComponent<BaseProjectile>();
		projScript.damage =new BaseDamage(damageAmount) ;
		projScript.owner = owner.gameObject;
		projScript.damage.Damage+=power;
		projScript.range+=range;
	}
	protected virtual void ReplicationGenerate (){
		if(shootsToSpawn.Count>0){
				sControl.playClip (fireSound);
				owner.shootEffect();
		}
		while(shootsToSpawn.Count>0){
			ShootData spawnShoot = shootsToSpawn.Dequeue();
			
			BaseProjectile  proj = GenerateProjectileRep(spawnShoot.position,spawnShoot.direction,spawnShoot.timeShoot);
			if (rifleParticleController != null) {
				rifleParticleController.CreateShootFlame ();
			}
			proj.damage.Damage+=spawnShoot.power;
			proj.range+=spawnShoot.range;
			switch(prefiretype){
				case PREFIRETYPE.Guidance:
					GuidanceProjectile guidanceScript  = proj.GetComponent<GuidanceProjectile>();
					if(guidanceScript!=null&&spawnShoot.viewId!=0){
						Pawn target =PhotonView.Find(spawnShoot.viewId).GetComponent<Pawn>();
				
					
						guidanceScript.target = target;
					}
				break;
			}
		}
	}
	protected BaseProjectile GenerateProjectileRep(Vector3 startPoint,Quaternion startRotation,double timeShoot){

		GameObject proj=Instantiate(projectilePrefab,startPoint,startRotation) as GameObject;
		BaseProjectile projScript = proj.GetComponent<BaseProjectile>();
		projScript.transform.Translate (startRotation*Vector3.forward*(float)(PhotonNetwork.time-timeShoot));
		projScript.damage =new BaseDamage(damageAmount) ;
		projScript.owner = owner.gameObject;
		return projScript;
	}

	protected void SendShoot(Vector3 position, Quaternion rotation,float power,float range,int viewId){
		ShootData send = new ShootData ();
		send.position = position;
		send.direction = rotation;
		send.power = power;
		send.range = range;
		send.viewId = viewId;
		send.timeShoot = PhotonNetwork.time;
		shootsToSend.Enqueue (send);
	}
	protected Quaternion getAimRotation(){
		/*Vector3 randVec = Random.onUnitSphere;
		Vector3 normalDirection  = owner.getAimRotation(weaponRange)-muzzlePoint.position;
		normalDirection =normalDirection + randVec.normalized * normalDirection.magnitude * aimRandCoef / 100;*/

		return Quaternion.LookRotation(owner.getAimRotation() -muzzlePoint.position);
		

	}
	
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(shootsToSend.Count);
			while(shootsToSend.Count>0){
				shootsToSend.Dequeue().PhotonSerialization(stream);
			}
			///stream.SendNext(transform.rotation);

		}
		else
		{
			// Network player, receive data
			int shootCount = (int) stream.ReceiveNext();
			for(int i=0;i<shootCount;i++){
				ShootData data = new ShootData();
				data.PhotonDeserialization(stream);
				shootsToSpawn.Enqueue(data);
			}
			//this.transform.rotation = (Quaternion) stream.ReceiveNext();

		}
	}
	public float MuzzleOffset(){

		return (muzzlePoint.position + muzzleOffset - curTransform.position).sqrMagnitude;
	}


  
}
