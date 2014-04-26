using UnityEngine;
using System.Collections;

public class HTHWeapon : BaseWeapon {

	public int attackId;

	public override bool CanShoot (){

		return true;
	}

	public override void ChangeWeaponStatus(bool status){
		HTHHitter[] allhitters =owner.GetComponentsInChildren<HTHHitter> ();
		foreach (HTHHitter hitter in allhitters){
			if(hitter.attackId==attackId){
				if(status==true){
					hitter.Activate(damageAmount,owner.gameObject);
				}else{
					hitter.DeActivate();
				}
			}
		}
		
	}

}
