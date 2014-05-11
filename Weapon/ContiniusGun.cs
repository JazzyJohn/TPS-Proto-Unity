using UnityEngine;
using System.Collections;

public class ContiniusGun : BaseWeapon
{
		
	public Vector3 colliderUnableSize = new Vector3(1,1,3);
	private Vector3 colliderDisableSize = new Vector3(0,0,0);

	private BoxCollider AOECollider; 
	void Start () {
		base.curTransform = transform;
		photonView = GetComponent<PhotonView>();
		base.rifleParticleController = GetComponentInChildren<RifleParticleController>();
		
		if (base.rifleParticleController != null) {
			base.rifleParticleController.SetOwner (base.owner.collider);
		}
		AOECollider = muzzlePoint.GetComponent<BoxCollider> ();
		AOECollider.size = colliderDisableSize;
	}

	public void fireDamage (Pawn target)
	{
		if (target != null) {
			target.addDPS (new BaseDamage (damageAmount), owner.gameObject);
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
		rifleParticleController.StartFlame ();
		AOECollider.GetComponent<BoxCollider>().size = colliderUnableSize;
	}
	public override void StopFire(){
		base.isShooting = false;
		rifleParticleController.StopFlame ();
		AOECollider.GetComponent<BoxCollider>().size = colliderDisableSize;
		base.ReleaseFire();
	}
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(isShooting);

			///stream.SendNext(transform.rotation);
			
		}
		else
		{
			// Network player, receive data
			bool newIsShooting = (bool) stream.ReceiveNext();
			if(newIsShooting&&!isShooting){
				StartFire();
			}else if(!newIsShooting&&isShooting){
				StopFire();
			}
			//this.transform.rotation = (Quaternion) stream.ReceiveNext();
			
		}
	}
	public override void AimFix(){
		Vector3 aimTarget =owner.getCachedAimRotation ();
		//Debug.Log ((aimTarget - muzzlePoint.position).normalized);

		muzzlePoint.rotation =Quaternion.LookRotation((aimTarget - muzzlePoint.position).normalized);
	}

}
