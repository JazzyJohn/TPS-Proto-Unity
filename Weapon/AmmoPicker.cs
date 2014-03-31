using UnityEngine;
using System;


public class AmmoPicker : UseObject {
	
	public AMMOTYPE ammo;
	
	public int amount;
	
	override public bool ActualUse(Pawn target){
		target.GetComponent<InventoryManager>().AddAmmo(ammo,amount);
		return base.ActualUse(target);
	}
	


}