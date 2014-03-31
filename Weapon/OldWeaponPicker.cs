using UnityEngine;
using System;


public class OldWeaponPicker : MonoBehaviour {
	
	public BaseWeapon prefabWeapon;
	
	public InventoryManager.WeaponBackUp info;
	
	virtual public void ActualUse(Pawn target){
		target.GetComponent<InventoryManager>.ChangePrefab(prefabWeapon,info);
		base.ActualUse(target);
	}
	


}