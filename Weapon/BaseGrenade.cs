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
		
        owner.animator.StartShootAniamtion("shooting");
		
	}
    public override void StartDamage()
    {
        Fire();
        Reload();
        if (curAmmo <= 0)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
            if (drawer != null)
            {
                drawer.GetComponent<Renderer>().enabled = false;
            }
        }
        //StartHit();
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
            if (drawer != null)
            {
                drawer.GetComponent<Renderer>().enabled = true;
            }
		}
	}
	protected override Quaternion getAimRotation(){
		/*Vector3 randVec = Random.onUnitSphere;
		Vector3 normalDirection  = owner.getAimRotation(weaponRange)-muzzlePoint.position;
		normalDirection =normalDirection + randVec.normalized * normalDirection.magnitude * aimRandCoef / 100;*/

//        Debug.Log(Quaternion.Euler(eluer));
        if (projectileClass != null)
        {

            return Quaternion.LookRotation(owner.getAimpointForWeapon(projectileClass.startImpulse) - muzzleCached)*addAngle;
        }
        else
        {
            return Quaternion.LookRotation(owner.getAimpointForWeapon() - muzzleCached)*addAngle;
        }


	}
    public override void TakeInHand(Transform weaponSlot, Vector3 Offset, Quaternion weaponRotator)
    {
        base.TakeInHand(weaponSlot, Offset, weaponRotator);
        owner.animator.IKOff();

    }

    public override void PutAway()
    {
        base.PutAway();
     
    }

    void Update()
    {
        UpdateWeapon(Time.deltaTime);
        if (foxView.isMine)
        {

            if (shouldDrawTrajectory && drawer != null && drawer.gameObject.activeSelf)
            {
                drawer.Draw(muzzleCached + muzzleOffset, getAimRotation(), GetRandomeDirectionCoef());
            }
        }


    }
}