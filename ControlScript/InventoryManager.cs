using UnityEngine;
using System;



public class InventoryManager : MonoBehaviour {

	public BaseWeapon[] prefabWeapon;

	private BaseWeapon currentWeapon;
	
	private int indexWeapon;
	
	private Pawn owner;
	
	void Start(){
		owner= (Pawn)GetComponent(typeof(Pawn));
		if(owner==null){
			Destroy(gameObject);
		}
		 
		if(prefabWeapon.Length!=0){
			BaseWeapon firstWeapon;
			if(Network.connections.Length==0){
				firstWeapon =Instantiate(prefabWeapon[0]) as BaseWeapon;
			}else{
				firstWeapon =Network.Instantiate(prefabWeapon[0],Vector3.zero,Quaternion.identity,0) as BaseWeapon;
			}
			owner.setWeapon(firstWeapon);
		}
	}
}
