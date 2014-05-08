using UnityEngine;
using System.Collections;

public class HTHHitter : MonoBehaviour {

	private bool isReadyToHit;

	//Id of attack if Pawn Have multiplie hth attack, then ID allow as to know which turn on on each attack 
	public int attackId;

	public float radiusOfImpact;

	public GameObject  owner;

	public BaseDamage damage;

	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit[] hits;
		
		if (isReadyToHit){
			hits =Physics.SphereCastAll(transform.position,radiusOfImpact,transform.forward,1.0f);
			Debug.DrawLine(transform.position,transform.position+transform.forward*1.0f);
			foreach(RaycastHit hit in hits){
				onBulletHit(hit);
			}
			



		}
	}
	public void Activate(BaseDamage newDamage,GameObject newOwner){
		isReadyToHit = true;
		damage = newDamage;
			owner = newOwner;
	}
	public void DeActivate(){
		isReadyToHit = false;

	}
	public virtual  void onBulletHit(RaycastHit hit)
	{
		if (owner == hit.transform.gameObject) {
			return;
		}
		if (hit.transform.gameObject.CompareTag ("decoration")) {
			//Debug.Log ("HADISH INTO " + hit.transform.gameObject.name);
			isReadyToHit=false;
		}
		DamagebleObject obj = hit.transform.gameObject.GetComponent <DamagebleObject>();
		if (obj != null) {
			obj.Damage(damage,owner);
			//Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);
			isReadyToHit=false;
		}
	}
	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, radiusOfImpact);
	}
}
