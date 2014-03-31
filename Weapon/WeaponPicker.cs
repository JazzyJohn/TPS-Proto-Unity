using UnityEngine;
using System;


public class WeaponPicker : MonoBehaviour {
	
	public BaseWeapon prefabWeapon;
	
	virtual public void ActualUse(Pawn target){
		target.GetComponent<InventoryManager>.ChangePrefab(prefabWeapon);
		base.ActualUse(target);
	}
	


}