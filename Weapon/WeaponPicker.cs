using UnityEngine;
using System;


public class WeaponPicker : UseObject {
	
	public BaseWeapon prefabWeapon;

	public InventoryManager.WeaponBackUp info;

	protected void Start(){

		tooltip = prefabWeapon.weaponName;
		base.Start ();
	}

	override public bool ActualUse(Pawn target){
		if (info == null) {
			target.GetComponent<InventoryManager> ().ChangePrefab (prefabWeapon);		
		} else {
			target.GetComponent<InventoryManager> ().ChangePrefab (prefabWeapon,info);
		}
		return base.ActualUse(target);
	}

	public void SetNewData( InventoryManager.WeaponBackUp info){


		photonView.RPC("RPCSetPicker",PhotonTargets.All,info.amount,(int)info.type);

	}
	[RPC] 
	public void RPCSetPicker(int amount,int type){
		info = new InventoryManager.WeaponBackUp (amount,(AMMOTYPE)type);

	}


}