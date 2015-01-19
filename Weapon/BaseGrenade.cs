using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nstuff.juggerfall.extension.models;
using Random = UnityEngine.Random;


public class BaseGrenade : BaseWeapon {
	
	public Quaternion addAngle;

    public Vector3 eluer;
	
	public override void StartFire(){
		Fire();
		Reload();
		if(curAmmo<=0){
			Renderer[] renderers = GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderers) {
				renderer.enabled = false;
			}
		}
	}
	public override void ReloadStart(){
		return;
	
	}
	public override void Reload(){
		curAmmo =owner.GetComponent<InventoryManager>().GiveAmmo(ammoType,1);	
		if(curAmmo>0){
			Renderer[] renderers = GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderers) {
				renderer.enabled = true;
			}
		}
	}
	protected override Quaternion getAimRotation(){
		/*Vector3 randVec = Random.onUnitSphere;
		Vector3 normalDirection  = owner.getAimRotation(weaponRange)-muzzlePoint.position;
		normalDirection =normalDirection + randVec.normalized * normalDirection.magnitude * aimRandCoef / 100;*/

        Debug.Log(Quaternion.Euler(eluer));
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