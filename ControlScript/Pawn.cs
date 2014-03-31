using UnityEngine;
using System.Collections;

public class Pawn : DamagebleObject {

	private BaseWeapon CurWeapon;

	public Transform weaponSlot;

	public Vector3 weaponOffset;

	public bool isDead=false;

	public string publicName;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartFire(){
		if (CurWeapon != null) {
			CurWeapon.StartFire ();
		}
	}
	public void StopFire(){
		if (CurWeapon != null) {
			CurWeapon.StopFire ();
		}
	}
	public void setWeapon(BaseWeapon newWeapon){
		CurWeapon = newWeapon;
		Debug.Log (newWeapon);
		CurWeapon.AttachWeapon(weaponSlot,weaponOffset,gameObject);
	}

	void OnCollisionEnter(Collision collision) {
		Debug.Log ("COLLISION ENTER PAWN " + this + collision);
	}
	void OnTriggerEnter	(Collider other) {
		Debug.Log ("TRIGGER ENTER PAWN "+ this +  other);
	}

}
