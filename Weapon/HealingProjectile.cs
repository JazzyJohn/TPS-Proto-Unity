using UnityEngine;
using System.Collections;

public class HealingProjectile : BaseProjectile {


	public override void onBulletHit(RaycastHit hit)
	{
		if (owner == hit.transform.gameObject||used) {
			return;
		}
		used = true;
		damage.pushDirection = mTransform.forward;
		damage.hitPosition = hit.point;
		if (hit.transform.gameObject.CompareTag ("decoration")) {
			//Debug.Log ("HADISH INTO " + hit.transform.gameObject.name);
			if(hitParticle!=null){
				Instantiate(hitParticle, hit.point, Quaternion.LookRotation(hit.normal));
			}
			Destroy (gameObject, 0.1f);
		}
		DamagebleObject obj = hit.transform.gameObject.GetComponent <DamagebleObject>();
		Pawn pawn = (Pawn)obj;
		if (pawn!=null) {
			if(pawn.team ==owner.GetComponent<Pawn>().team){
				pawn.Heal(damage.Damage,owner);
				Destroy (gameObject, 0.1f);
				return;
			}
			//Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);

		}
		if (obj != null) {
			obj.Damage(damage,owner);
			Debug.Log ("HADISH INTO SOME PLAYER! " + hit.transform.gameObject.name);
			Destroy (gameObject, 0.1f);
		}
	}
}
