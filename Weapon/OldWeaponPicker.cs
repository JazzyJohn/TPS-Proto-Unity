using UnityEngine;
using System;


public class OldWeaponPicker : UseObject {
	
	public BaseWeapon prefabWeapon;
	
	public InventoryManager.WeaponBackUp info;
	
	override public bool ActualUse(Pawn target){
		target.GetComponent<InventoryManager>().ChangePrefab(prefabWeapon,info);
		return base.ActualUse(target);
	}
	


}