using UnityEngine;
using System.Collections;

public class BaseShootgun : BaseWeapon {

	public int NumerOfPeace=8;

	protected override void GenerateProjectile(){
		Vector3 startPoint  = muzzlePoint.position+muzzleOffset;
		Quaternion startRotation = getAimRotation();
		for (int i =0; i<NumerOfPeace; i++) {
				GameObject proj;
				float effAimRandCoef = aimRandCoef;
				if (effAimRandCoef > 0) {
						startRotation = Quaternion.Euler (startRotation.eulerAngles + new Vector3 (Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef)));
				}
				proj = Instantiate (projectilePrefab, startPoint, startRotation) as GameObject;
				if (photonView.isMine) {
					SendShoot(startPoint,startRotation);
				}
				BaseProjectile projScript = proj.GetComponent<BaseProjectile> ();
				projScript.damage = new BaseDamage (damageAmount);
				projScript.owner = owner.gameObject;
		}
	}
}
