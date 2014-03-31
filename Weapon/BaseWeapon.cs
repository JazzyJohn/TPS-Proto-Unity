using UnityEngine;
using System.Collections;



public class BaseWeapon : MonoBehaviour {

	public enum AMUNITONTYPE{SIMPLEHIT, PROJECTILE, RAY};

	public AMUNITONTYPE amunitionType;

	public float fireInterval;

	public float clipSize;

	public float damageAmount;

	public float aimRandCoef;

	public GameObject projectilePrefab;

	public Transform muzzlePoint;

	public Vector3 muzzleOffset;

	public float weaponRange;

	private float curAmmo;

	private GameObject owner;

	private Transform curTransform;

	private bool isShooting = false;

	private float timer =  0.0f;


	// Use this for initialization
	void Start () {
		curTransform = transform;
	}

	public void AttachWeapon(Transform weaponSlot,Vector3 Offset, GameObject inowner){
		if (curTransform == null) {
			curTransform = transform;		
		}
		owner = inowner;
		curTransform.parent = weaponSlot;
		curTransform.localPosition = Offset;
	}
	// Update is called once per frame
	void Update () {
		if (isShooting) {
			if(timer<=0){
				timer = fireInterval;
				Fire();
			}
		}
		if (timer >= 0) {
			timer-=Time.deltaTime;
		}
	}
	public void StartFire(){
		isShooting = true;

	}
	public void StopFire(){
		isShooting = false;
		
	}
	void Fire(){
		switch (amunitionType) {
		case AMUNITONTYPE.SIMPLEHIT:
			DoSimpleDamage();
			break;
		case AMUNITONTYPE.PROJECTILE:
			
			GenerateProjectile();
			break;
		case AMUNITONTYPE.RAY:
			
			break;
			
		}
	}

	void DoSimpleDamage(){
		Camera maincam = Camera.main;
		Ray centerRay= maincam.ViewportPointToRay(new Vector3(.5f, 0.5f, 1f));
		RaycastHit hitInfo;
		Vector3 targetpoint;
		if (Physics.Raycast (centerRay, out hitInfo, weaponRange)) {
			DamagebleObject target =(DamagebleObject) hitInfo.collider.GetComponent(typeof(DamagebleObject));
			if(target!=null){
				target.Damage(damageAmount);
			}
		}
	}
	void GenerateProjectile(){
		Vector3 startPoint  = muzzlePoint.position+muzzleOffset;
		Quaternion startRotation = getAimRotation();
		GameObject proj;
		if(Network.connections.Length>0){
			proj=Network.Instantiate(projectilePrefab,startPoint,startRotation,0) as GameObject;
		}
		else{
			proj=Instantiate(projectilePrefab,startPoint,startRotation) as GameObject;
		}
	
		BaseProjectile projScript = (BaseProjectile)proj.GetComponent (typeof(BaseProjectile));
		projScript.damage =damageAmount ;
		projScript.owner = owner;
	}
	Quaternion getAimRotation(){
		Camera maincam = Camera.main;
		Ray centerRay= maincam.ViewportPointToRay(new Vector3(.5f, 0.5f, 1f));
		RaycastHit hitInfo;
		Vector3 targetpoint = Vector3.zero;
		if (Physics.Raycast (centerRay,out hitInfo, weaponRange)) {
			targetpoint =hitInfo.point;
			//Debug.Log(hitInfo.collider);
		}else{
			targetpoint =maincam.transform.forward*weaponRange +maincam.ViewportToWorldPoint(new Vector3(.5f, 0.5f, 1f));
		}
		return Quaternion.LookRotation(targetpoint-muzzlePoint.position);

	}
}
