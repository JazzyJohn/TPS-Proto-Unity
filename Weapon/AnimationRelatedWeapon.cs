using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


class AnimationRelatedWeapon: BaseWeapon {
	 
	public string animatioName;
	 
	public void WeaponShoot(){
		Fire();
	}
	// Update is called once per frame
	void Update () {
		if(init&&owner==null) {
			RequestKillMe();

		}
		//AimFix ();
		if (!photonView.isMine) {
			ReplicationGenerate ();
			return;
		}

		
	}
	public virtual void StartFire(){
		owner.StartShootAnimation(animatioName);

	}
	public virtual void StopFire(){
		owner.StopShootAnimation(animatioName);
	}
}
