using UnityEngine;
using System;


[RequireComponent (typeof (Pawn))]
public class AIInventoryManager : InventoryManager {

	//Check is there ammo in bag
	public override bool HasAmmo(AMMOTYPE ammo){
		
		return true;
	}
	//Get amount ammo for current ammotype
	public override int GetAmmo(AMMOTYPE ammo){
		return 0;
	}
	//Return ammo from bag
	public override int GiveAmmo(AMMOTYPE ammo,int amount){
		return amount;
	}
	//Add Ammo to bag
	public override void AddAmmo(AMMOTYPE ammo,int amount){
		return;
		
	}
	public override void Init(){
		if (owner == null) {
			owner = GetComponent<Pawn> ();
			
			
		}
        SpawnWeaponFromNameList();
	}


}