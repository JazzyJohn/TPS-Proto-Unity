using UnityEngine;
using System.Collections;



public class BaseWeapon : DestroyableNetworkObject {

	public enum AMUNITONTYPE{SIMPLEHIT, PROJECTILE, RAY, HTHWEAPON};

	public AMUNITONTYPE amunitionType;
	
	public enum SLOTTYPE{PERSONAL, MAIN, ANTITANK};

	public SLOTTYPE slotType;
	
	public AMMOTYPE ammoType; 

	public float fireInterval;
	
	public float reloadTime;

	public int clipSize;

	public bool vsArmor;

	public float damageAmount;

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

	private bool isShooting = false;

	private float fireTimer =  0.0f;
	
	private float reloadTimer =  0.0f;

	private RifleParticleController rifleParticleController;

	public string attackAnim;

	// Use this for initialization
	void Start () {
		curTransform = transform;
		photonView = GetComponent<PhotonView>();
		rifleParticleController = GetComponentInChildren<RifleParticleController>();
		if (transform.parent == null) {

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
		Debug.Log (name + weaponRotator);
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

	}

	// Update is called once per frame
	void Update () {
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
	public void StartFire(){
		isShooting = true;

	}
	public void StopFire(){
		isShooting = false;
		ReleaseFire ();
	}
	
	public void ReloadStart(){
		
		if(owner.GetComponent<InventoryManager>().HasAmmo(ammoType)){
			isReload= true;
			reloadTimer=reloadTime;
		}else{
			isShooting = false;
			return;
		}
		
	}
	public void Reload(){
		isReload = false;
		curAmmo =owner.GetComponent<InventoryManager>().GiveAmmo(ammoType,clipSize);	
	}
	public bool IsReloading(){
		return isReload;
	}
	void Fire(){
		if (!CanShoot ()) {
			return;		
		}
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
		photonView.RPC("FireEffect",PhotonTargets.Others);
	}
	public virtual bool CanShoot (){
		Vector3 aimDir = (owner.getCachedAimRotation() -muzzlePoint.position).normalized;
		Vector3 realDir = muzzlePoint.forward;
		float angle = Vector3.Dot (aimDir, realDir);

		if (angle < 0.8) {
			return false;		
		}
		return true;
	}

	void ReleaseFire(){
		switch (amunitionType) {

		case AMUNITONTYPE.HTHWEAPON:
			owner.animator.StopAttackAnim(attackAnim);
			ChangeWeaponStatus(false);
			break;
			
		}
	}
	[RPC]
	void FireEffect(){
		if (rifleParticleController != null) {
			rifleParticleController.CreateShootFlame ();
		}
	}
	public virtual void ChangeWeaponStatus(bool status){


	}
	void DoSimpleDamage(){
		Camera maincam = Camera.main;
		Ray centerRay= maincam.ViewportPointToRay(new Vector3(.5f, 0.5f, 1f));
		RaycastHit hitInfo;
		Vector3 targetpoint;
		if (Physics.Raycast (centerRay, out hitInfo, weaponRange)) {
			DamagebleObject target =(DamagebleObject) hitInfo.collider.GetComponent(typeof(DamagebleObject));
			if(target!=null){
				target.Damage(damageAmount,owner.gameObject);
			}
		}
	}
	void GenerateProjectile(){
		Vector3 startPoint  = muzzlePoint.position+muzzleOffset;
		Quaternion startRotation = getAimRotation();
		GameObject proj;
		float effAimRandCoef = aimRandCoef + owner.AimingCoef ();
		if (effAimRandCoef > 0) {
			startRotation = Quaternion.Euler (startRotation.eulerAngles + new Vector3 (Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef)));
		}
		proj=Instantiate(projectilePrefab,startPoint,startRotation) as GameObject;
		if (photonView.isMine) {
			photonView.RPC("GenerateProjectileRep",PhotonTargets.Others,startPoint,startRotation);
		}
		BaseProjectile projScript =proj.GetComponent<BaseProjectile>();
		projScript.damage =damageAmount ;
		projScript.owner = owner.gameObject;
	}
	[RPC]
	void GenerateProjectileRep(Vector3 startPoint,Quaternion startRotation){
		GameObject proj=Instantiate(projectilePrefab,startPoint,startRotation) as GameObject;
		BaseProjectile projScript = proj.GetComponent<BaseProjectile>();
		projScript.damage =damageAmount ;
		projScript.owner = owner.gameObject;
	}
	Quaternion getAimRotation(){
		/*Vector3 randVec = Random.onUnitSphere;
		Vector3 normalDirection  = owner.getAimRotation(weaponRange)-muzzlePoint.position;
		normalDirection =normalDirection + randVec.normalized * normalDirection.magnitude * aimRandCoef / 100;*/

		return Quaternion.LookRotation(owner.getAimRotation(weaponRange) -muzzlePoint.position);
		

	}
	
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		/*if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);

		}
		else
		{
			// Network player, receive data
			this.transform.position = (Vector3) stream.ReceiveNext();
			this.transform.rotation = (Quaternion) stream.ReceiveNext();

		}*/
	}
	

}
