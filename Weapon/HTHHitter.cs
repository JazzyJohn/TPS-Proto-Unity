using UnityEngine;
using System.Collections;

public class HTHHitter : MonoBehaviour {

	public string NameAnimate;
	public string NameAttack;
	protected int attackId;

	public float radiusOfImpact;

	protected Pawn owner;
	
	protected WeaponOfExtremities WeaponControl;  

	protected Animator anim;

	public BaseDamage damage;

	public enum KickType{Aim, AOE};
	public KickType AttakType;

	public float KDTime;
	
	protected bool KickPlay;

	//звуки
	private soundControl sControl;//глобальный обьект контроллера звука
	private AudioSource aSource;//источник звука. добавляется в редакторе
	public AudioClip HitSound;
	
	private HTHHitter ThisAttack;

	[HideInInspector]
	public bool isKick;
	[HideInInspector]
	public float timer;

	private bool Damage = false;

	public bool CanShoot ()
	{
		if (timer<=0f)
			return true;
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

	}

	public void Luke_I_am_your_father(WeaponOfExtremities Darth_Vader,Pawn  owner1)
	{
		WeaponControl=Darth_Vader;
		owner = owner1;

		StartCoroutine(GetAnim());

	}

	IEnumerator GetAnim()
	{
		yield return new WaitForSeconds (0.1f);
		anim = owner.animator.animator;
	}

	// Update is called once per frame
	void Update () 
	{
		if (timer >= 0) {
			timer-=Time.deltaTime;
		}

		if (anim != null)
		{
			bool ok = false;
			
			for(int i = 0; i< anim.layerCount; i++)
			{
				int CurAnim = anim.GetCurrentAnimatorStateInfo(i).nameHash;
				
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
				KickPlay = false;
				Damage = false;
			}
		}

		RaycastHit[] hits;
		
		if (KickPlay)
		{
			hits = Physics.SphereCastAll(transform.position,radiusOfImpact, transform.position+owner.gameObject.transform.forward, 1f);
			Debug.DrawLine(transform.position,transform.position+owner.gameObject.transform.forward*1.0f);
			foreach(RaycastHit hit in hits)
			{
				onBulletHit(hit);
			}
		}
	}

	void FixedUpdate()
	{

	}
	

	public virtual  void onBulletHit(RaycastHit hit)
	{
		if (!Damage)
		{
			if (owner == hit.transform.gameObject) {
				return;
			}
			if (hit.transform.gameObject.CompareTag ("decoration")) {
				sControl.playClip (HitSound);
				Damage = true;
				//Debug.Log ("HADISH INTO " + hit.transform.gameObject.name);
			}
			DamagebleObject obj = hit.transform.gameObject.GetComponent <DamagebleObject>();
			if (obj != null) {
				sControl.playClip (HitSound);
				Damage = true;
				obj.Damage(damage,owner.gameObject);
				//Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, radiusOfImpact);
	}
}
