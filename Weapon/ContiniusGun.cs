using UnityEngine;
using System.Collections;

public class ContiniusGun : BaseWeapon
{
		
	public Vector3 colliderUnableSize = new Vector3(1,1,3);
	private Vector3 colliderDisableSize = new Vector3(0,0,0);

	private BoxCollider AOECollider;

    public RayController rayController;

    public bool isSend;
	new void Start () {
		base.Start ();
        rayController = rifleParticleController as RayController;
        AOECollider = muzzlePoint.GetComponent<MuzzlePoint>().damager.GetComponent<BoxCollider>();
		AOECollider.size = colliderDisableSize;
		sControl.setLooped(true);
        isSend = false;
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

	protected override void DoSimpleDamage(){
		
		sControl.playFullClip (fireSound);
		rifleParticleController.StartFlame ();
		AOECollider.GetComponent<BoxCollider>().size = colliderUnableSize;
        if (foxView.isMine && !isSend)
        {
            isSend = true;
			foxView.ChangeWeaponShootState(true);
		}
	}

    protected override void ShootTick()
    {
        base.ShootTick();
        rayController.SetRay(muzzlePoint.position, owner.getAimpointForWeapon(0));
    }
    public override void StartFireRep()
    {
        sControl.playFullClip(fireSound);
        rifleParticleController.StartFlame();
        AOECollider.GetComponent<BoxCollider>().size = colliderUnableSize;
   
    }
	public override void StopFire(){
        base.StopFire();
		rifleParticleController.StopFlame ();
		AOECollider.GetComponent<BoxCollider>().size = colliderDisableSize;
		base.ReleaseFire();
        if (foxView.isMine && isSend)
        {
            isSend = false;
			foxView.ChangeWeaponShootState(false);
		}
        rayController.StopRay();
	}
	
	public override void AimFix(){
		Vector3 aimTarget =owner.getCachedAimRotation ();
		//Debug.Log ((aimTarget - muzzlePoint.position).normalized);

		muzzlePoint.rotation =Quaternion.LookRotation((aimTarget - muzzlePoint.position).normalized);
	}

}
