using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class BaseWeapon : DestroyableNetworkObject {
	class ShootData{
		public double timeShoot;
		public Quaternion  direction;
		public Vector3 position;
		public void PhotonSerialization(PhotonStream stream){
			stream.SendNext (timeShoot);

			stream.SendNext( direction);
			ServerHolder.WriteVectorToShort (stream, position);
		}
		public void PhotonDeserialization(PhotonStream stream){
			timeShoot = (double)stream.ReceiveNext ();
			direction = (Quaternion)stream.ReceiveNext ();
			position = ServerHolder.ReadVectorFromShort (stream);
		}
	}

	private Queue<ShootData> shootsToSend = new Queue<ShootData>();

	private Queue<ShootData> shootsToSpawn = new Queue<ShootData>();

	public enum AMUNITONTYPE{SIMPLEHIT, PROJECTILE, RAY, HTHWEAPON, AOE};

	public AMUNITONTYPE amunitionType;
	
	public enum SLOTTYPE{PERSONAL, MAIN, ANTITANK};

	public SLOTTYPE slotType;
	
	public AMMOTYPE ammoType; 

	public float fireInterval;
	
	public float reloadTime;

	public int clipSize;

	public BaseDamage damageAmount;

	public float normalRandCoef;

	public float aimRandCoef;

	public GameObject projectilePrefab;
	
	public GameObject pickupPrefabPrefab;

	public Transform muzzlePoint;

	public Transform leftHandHolder;

	public Vector3 	muzzleOffset;

	public float weaponRange;

	public int curAmmo;

	protected Pawn owner;

	public Transform curTransform;

	private bool isReload = false;

	protected bool isShooting = false;

	private float fireTimer =  0.0f;
	
	private float reloadTimer =  0.0f;

	protected RifleParticleController rifleParticleController;

	public string attackAnim;

	public string weaponName;

	public float recoilMod;

	public bool init = false;

	public const float MAXDIFFERENCEINANGLE=0.7f;

	private bool shootAfterReload;

	//звуки
	private soundControl sControl;//глобальный обьект контроллера звука
	private AudioSource aSource;//источник звука. добавляется в редакторе
	public AudioClip fireSound;
	public AudioClip reloadSound;


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
	
		if (rifleParticleController != null) {
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
		owner.setWeapon (this);
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
		if (isShooting) {
			if(fireTimer<=0){
				fireTimer = fireInterval;
				Fire();
			}
		}
		if (fireTimer >= 0) {
			fireTimer-=Time.deltaTime;
		}
	}
	public virtual void StartFire(){
		isShooting = true;

	}
	public virtual void StopFire(){
		isShooting = false;
		shootAfterReload = false;
		ReleaseFire ();
	}
	
	public void ReloadStart(){
		
		if(owner.GetComponent<InventoryManager>().HasAmmo(ammoType)){
			isReload= true;
			reloadTimer=reloadTime;
			if(isShooting){
				StopFire();
				shootAfterReload = true;
			}
			//играем звук перезарядки
			sControl.playClip (reloadSound);
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
	//temporary function to fix wrong aiming
	public virtual void AimFix(){

	}
	void Fire(){
		if (!CanShoot ()) {
			return;		
		}
		//играем звук стрельбы
		sControl.playClip (fireSound);

		if(curAmmo>0){
			curAmmo--;
		}else{
			if(clipSize>0){
				ReloadStart();
				return;
			}
		}
		if (rifleParticleController != null) {
			rifleParticleController.CreateShootFlame ();
		}
			

				
		switch (amunitionType) {
		case AMUNITONTYPE.SIMPLEHIT:
			DoSimpleDamage();
			break;
		case AMUNITONTYPE.PROJECTILE:
	
			GenerateProjectile();
			break;
		case AMUNITONTYPE.RAY:
			
			break;
		case AMUNITONTYPE.HTHWEAPON:
			owner.animator.StartAttackAnim(attackAnim);
			ChangeWeaponStatus(true);
			break;
			
		}
	
		owner.HasShoot ();
		//photonView.RPC("FireEffect",PhotonTargets.Others);
	}
	public virtual bool CanShoot (){

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
	protected virtual void GenerateProjectile(){
		Vector3 startPoint  = muzzlePoint.position+muzzleOffset;
		Quaternion startRotation = getAimRotation();
		GameObject proj;
		float effAimRandCoef = 0.0f;
		if (owner.isAiming) {
			effAimRandCoef=aimRandCoef;
		}else{
			effAimRandCoef=normalRandCoef;
		}

		effAimRandCoef+= owner.AimingCoef ();
		if (effAimRandCoef > 0) {
			startRotation = Quaternion.Euler (startRotation.eulerAngles + new Vector3 (Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef)));
		}
		proj=Instantiate(projectilePrefab,startPoint,startRotation) as GameObject;
		if (photonView.isMine) {
			SendShoot(startPoint,startRotation);
		}
		BaseProjectile projScript =proj.GetComponent<BaseProjectile>();
		projScript.damage =new BaseDamage(damageAmount) ;
		projScript.owner = owner.gameObject;
	}
	protected void ReplicationGenerate (){
		while(shootsToSpawn.Count>0){
			ShootData spawnShoot = shootsToSpawn.Dequeue();
			GenerateProjectileRep(spawnShoot.position,spawnShoot.direction,spawnShoot.timeShoot);
		}
	}
	protected void GenerateProjectileRep(Vector3 startPoint,Quaternion startRotation,double timeShoot){

		GameObject proj=Instantiate(projectilePrefab,startPoint,startRotation) as GameObject;
		BaseProjectile projScript = proj.GetComponent<BaseProjectile>();
		projScript.transform.Translate (startRotation*Vector3.forward*(float)(PhotonNetwork.time-timeShoot));
		projScript.damage =new BaseDamage(damageAmount) ;
		projScript.owner = owner.gameObject;
		if (rifleParticleController != null) {
			rifleParticleController.CreateShootFlame ();
		}
	}

	protected void SendShoot(Vector3 position, Quaternion rotation){
		ShootData send = new ShootData ();
		send.position = position;
		send.direction = rotation;
		send.timeShoot = PhotonNetwork.time;
		shootsToSend.Enqueue (send);
	}
	protected Quaternion getAimRotation(){
		/*Vector3 randVec = Random.onUnitSphere;
		Vector3 normalDirection  = owner.getAimRotation(weaponRange)-muzzlePoint.position;
		normalDirection =normalDirection + randVec.normalized * normalDirection.magnitude * aimRandCoef / 100;*/

		return Quaternion.LookRotation(owner.getAimRotation(weaponRange) -muzzlePoint.position);
		

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
	

}
