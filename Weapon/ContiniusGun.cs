using UnityEngine;
using System.Collections;

public class ContiniusGun : BaseWeapon
{
		
	private Vector3 colliderUnableSize = new Vector3(1,1,3);
	private Vector3 colliderDisableSize = new Vector3(0,0,0);

	void Start () {
		base.curTransform = transform;
		photonView = GetComponent<PhotonView>();
		base.rifleParticleController = GetComponentInChildren<RifleParticleController>();
		
		if (base.rifleParticleController != null) {
			base.rifleParticleController.SetOwner (base.owner.collider);
		}
		this.transform.GetComponent<BoxCollider>().size = colliderDisableSize;
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
		this.transform.GetComponent<BoxCollider>().size = colliderUnableSize;
	}
	public override void StopFire(){
		base.isShooting = false;
		this.transform.GetComponent<BoxCollider>().size = colliderDisableSize;
		base.ReleaseFire();
	}

}
