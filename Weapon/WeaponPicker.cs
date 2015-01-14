using UnityEngine;
using System;


public class WeaponPicker : UseObject {
	
	public BaseWeapon prefabWeapon;

	public int WeaponId;


	protected void Start(){
		prefabWeapon = ItemManager.instance.weaponPrefabsListbyId [WeaponId];
		tooltip = prefabWeapon.weaponName;
		base.Start ();
	}

	override public bool ActualUse(Pawn target){
		
		target.GetComponent<InventoryManager> ().ChangePrefab (prefabWeapon);		
	
		return base.ActualUse(target);
	}

	

}