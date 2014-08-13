using UnityEngine;
using System.Collections;

public class HTHHitter : MonoBehaviour {

	public string NameAnimate;
	public string NameAttack;
	protected int attackId;

    public bool OnMove = true;

	public float radiusOfImpact;

	protected Pawn owner;
	
	protected WeaponOfExtremities WeaponControl;  

	protected Animator anim;

	public BaseDamage damage;

	public enum KickType{Aim, AOE};
	public KickType AttakType;

	public float KDTime;

	public Transform myTransform;
	
	protected bool KickPlay;

	//звуки
	private soundControl sControl;//глобальный обьект контроллера звука
	private AudioSource aSource;//источник звука. добавляется в редакторе
	public AudioClip HitSound;
	
	private HTHHitter ThisAttack;

	[HideInInspector]
	public bool isKick;
	
	private bool checkAnimation=false;
	[HideInInspector]
	public float timer;

	private bool wasDamage = false;

	public bool CanShoot ()
	{
        if (timer <= 0f)
        {
            
            return true;

        }
        else
            return false;
	}

	// Use this for initialization
	void Start () 
	{

		ThisAttack = this.GetComponent<HTHHitter>();

		if (NameAnimate == null)
			this.enabled = false;
		attackId = Animator.StringToHash(NameAnimate);

		aSource = GetComponent<AudioSource> ();
		sControl = new soundControl (aSource);//создаем обьект контроллера звука
		myTransform = transform;
	}

    public void SetOwner(WeaponOfExtremities weapon, Pawn owner)
	{
        WeaponControl = weapon;
		this.owner = owner;

		anim = owner.animator.animator;

	}
	public void StartKick(){
		isKick= true;
		checkAnimation= true;
	}
	
	public void StopKick(){
		isKick= false;
		wasDamage = false;

	}
	
	public void AttackFinish(){
		if(KickPlay){
			WeaponControl.AttackFinish();
			checkAnimation=false;
			wasDamage = false;
		}
		KickPlay = false;		
		
	
	}

	// Update is called once per frame
	void Update () 
	{
		if (timer >= 0) {
			timer-=Time.deltaTime;
		}
	//	Debug.Log (this + "  " + isKick);
		if(checkAnimation){
			bool ok = false;
				
			for(int i = 0; i< anim.layerCount; i++)
			{
				int CurAnim = anim.GetCurrentAnimatorStateInfo(i).nameHash;
				//Debug.Log (CurAnim + "  " + attackId);
				if (CurAnim==attackId)
				{
					//Debug.Log(anim.GetCurrentAnimatorStateInfo(i).length);
					KickPlay = true;
					ok = true;
					break;
				}
			}
			
			if (!ok)
			{
				AttackFinish();
				
			}
			
			//Debug.Log (this + "  " + KickPlay);
			RaycastHit[] hits;
			if (KickPlay)
			{
				if(!wasDamage){
					hits = Physics.RaycastAll(myTransform.position, 	myTransform.forward, 2.0f);
					Debug.DrawRay(myTransform.position,myTransform.forward*2.0f,Color.red,5.0f);
					foreach(RaycastHit hit in hits)
					{
						onBulletHit(hit);
					}
					if(wasDamage){
						return;
					}

					hits = Physics.SphereCastAll(myTransform.position,radiusOfImpact, myTransform.forward, 2.0f);
					//Debug.DrawLine(transform.position,transform.position+owner.gameObject.transform.forward*1.0f,Color.red,5.0f);
					foreach(RaycastHit hit in hits)
					{
						onBulletHit(hit);
					}
				}
			}
		}
	}

	void FixedUpdate()
	{

	}
	

	public virtual  void onBulletHit(RaycastHit hit)
	{
       // Debug.Log("HADISH INTO " + hit.transform.gameObject + owner);
		if (!wasDamage)
		{
			if (owner.gameObject == hit.transform.gameObject) {
				return;
			}
			if (hit.transform.gameObject.CompareTag ("decoration")) {
				sControl.playClip (HitSound);
				wasDamage = true;
				//Debug.Log ("HADISH INTO " + hit.transform.gameObject.name);
			}
			DamagebleObject obj = hit.transform.gameObject.GetComponent <DamagebleObject>();
			if (obj != null) {
				sControl.playClip (HitSound);
				wasDamage = true;
				BaseDamage lDamage  = new BaseDamage(damage);
				//lDamage.pushDirection = hit.point;
				lDamage.hitPosition = hit.point;
                lDamage.pushDirection = (hit.point - owner.myTransform.position).normalized;
				obj.Damage(lDamage,owner.gameObject);
				//Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);
			}
		}
	}

	void OnDrawGizmos() {
		//Gizmos.color = Color.yellow;
		//Gizmos.DrawSphere(transform.position, radiusOfImpact);
	}
}
