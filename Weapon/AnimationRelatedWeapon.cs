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
	
		
	}
	public override void StartFire(){
        base.StartFire();
		owner.StartShootAnimation(animatioName);

	}
    public override void StopFire()
    {
        base.StopFire();
		owner.StopShootAnimation(animatioName);
	}
}
