using UnityEngine;
using System;


public class AmmoPicker : UseObject {
	[Serializable]
	public class AmmoBoxPicker{
		public AMMOTYPE ammo;
		
		public int amount;
	}

	public AmmoBoxPicker[] ammoList;
	override public bool ActualUse(Pawn target){
		foreach (AmmoBoxPicker abp in ammoList) {
			target.GetComponent<InventoryManager> ().AddAmmo (abp.ammo, abp.amount);
		}
		return base.ActualUse(target);

	}
	


}