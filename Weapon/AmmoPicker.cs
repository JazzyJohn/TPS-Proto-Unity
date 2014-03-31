using UnityEngine;
using System;


public class AmmoPicker : MonoBehaviour {
	
	public AmmoType ammo;
	
	public int amount;
	
	virtual public void ActualUse(Pawn target){
		target.GetComponent<InventoryManager>.AddAmmo(ammo,amount);
		base.ActualUse(target);
	}
	


}