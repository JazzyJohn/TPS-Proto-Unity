using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nstuff.juggerfall.extension.models;
using Random = UnityEngine.Random;


public class BaseGrenade : BaseWeapon {
	
	public Quaternion addAngle;
	
	public override void StartFire(){
		Fire();
	
	}

	protected override Quaternion getAimRotation(){
		/*Vector3 randVec = Random.onUnitSphere;
		Vector3 normalDirection  = owner.getAimRotation(weaponRange)-muzzlePoint.position;
		normalDirection =normalDirection + randVec.normalized * normalDirection.magnitude * aimRandCoef / 100;*/
        if (projectileClass != null)
        {

            return Quaternion.LookRotation(owner.getAimpointForWeapon(projectileClass.startImpulse) - muzzleCached)*addAngle;
        }
        else
        {
            return Quaternion.LookRotation(owner.getAimpointForWeapon() - muzzleCached)*addAngle;
        }
		

	}
	

}