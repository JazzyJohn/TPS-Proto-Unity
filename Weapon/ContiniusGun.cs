using UnityEngine;
using System.Collections;

public class ContiniusGun : BaseWeapon
{
		
	public Vector3 colliderUnableSize = new Vector3(1,1,3);
	private Vector3 colliderDisableSize = new Vector3(0,0,0);

	private BoxCollider AOECollider; 
	new void Start () {
		base.Start ();

        AOECollider = muzzlePoint.GetComponent<MuzzlePoint>().damager.GetComponent<BoxCollider>();
		AOECollider.size = colliderDisableSize;
		sControl.setLooped(true);
	}

	public void fireDamage (Pawn target)
	{
		if (target != null) {
            //Debug.Log(damageAmount.Damage);
            target.addDPS(new BaseDamage(damageAmount), owner.gameObject, fireInterval);
		}
	}

	public singleDPS getId ()
	{
		singleDPS newDPS = new singleDPS ();
		newDPS.damage = new BaseDamage (damageAmount);
		newDPS.killer = owner.gameObject;
      
		return newDPS;
	}

	public override void StartFire(){
		base.isShooting = true;
		sControl.playFullClip (fireSound);
		rifleParticleController.StartFlame ();
		AOECollider.GetComponent<BoxCollider>().size = colliderUnableSize;
	}
	public override void StopFire(){
		base.isShooting = false;
		sControl.stopSound ();
		rifleParticleController.StopFlame ();
		AOECollider.GetComponent<BoxCollider>().size = colliderDisableSize;
		base.ReleaseFire();
	}
	
	public override void AimFix(){
		Vector3 aimTarget =owner.getCachedAimRotation ();
		//Debug.Log ((aimTarget - muzzlePoint.position).normalized);

		muzzlePoint.rotation =Quaternion.LookRotation((aimTarget - muzzlePoint.position).normalized);
	}

}
