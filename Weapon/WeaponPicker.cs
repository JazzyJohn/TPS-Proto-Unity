using UnityEngine;
using System;


public class WeaponPicker : UseObject {
	
	public BaseWeapon prefabWeapon;
	
	override public bool ActualUse(Pawn target){
		target.GetComponent<InventoryManager>().ChangePrefab(prefabWeapon);
		return base.ActualUse(target);
	}
	


}