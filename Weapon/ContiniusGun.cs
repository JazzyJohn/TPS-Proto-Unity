using UnityEngine;
using System.Collections;

public class ContiniusGun : NetSyncGun
{
		
///	public Vector3 colliderUnableSize = new Vector3(1,1,3);
	//private Vector3 colliderDisableSize = new Vector3(0,0,0);

	private BoxCollider AOECollider;

 
  
	new void Start () {
		base.Start ();
       // AOECollider = muzzlePoint.GetComponent<MuzzlePoint>().damager.GetComponent<BoxCollider>();
		//AOECollider.size = colliderDisableSize;
		sControl.setLooped(true);
        
	}

	public void fireDamage (DamagebleObject target)
	{
		if (target != null) {
            //Debug.Log(damageAmount.Damage);
            target.addDPS(new BaseDamage(damageAmount), owner.gameObject, fireInterval);
		}
	}
	public void ClearDps (DamagebleObject target)
	{
        target.clearDps(owner.gameObject);
	}
	
	public singleDPS getId ()
	{
		singleDPS newDPS = new singleDPS ();
		newDPS.damage = new BaseDamage (damageAmount);
		newDPS.killer = owner.gameObject;
      
		return newDPS;
	}

	protected override void DoSimpleDamage(){
		
		sControl.playFullClip (fireSound);
		//rifleParticleController.StartFlame ();
		//AOECollider.GetComponent<BoxCollider>().size = colliderUnableSize;
        FiredEffect();
	}

  
    public override void StartFireRep()
    {
        sControl.playFullClip(fireSound);
       /// rifleParticleController.StartFlame();
        //AOECollider.GetComponent<BoxCollider>().size = colliderUnableSize;
        FiredEffect();
   
    }
	public override void StopFire(){
        base.StopFire();
		//rifleParticleController.StopFlame ();
		//AOECollider.GetComponent<BoxCollider>().size = colliderDisableSize;
        FiredStopEffect();
      
	}
	
	public override void AimFix(){
		Vector3 aimTarget =owner.getCachedAimRotation ();
		//Debug.Log ((aimTarget - muzzlePoint.position).normalized);

		muzzlePoint.rotation =Quaternion.LookRotation((aimTarget - muzzlePoint.position).normalized);
	}

}
