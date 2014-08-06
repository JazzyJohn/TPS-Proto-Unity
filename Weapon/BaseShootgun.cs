using UnityEngine;
using System.Collections;

public class BaseShootgun : BaseWeapon {

	public int NumerOfPeace=8;

	protected override void GenerateProjectile(){
		Vector3 startPoint  = muzzlePoint.position+muzzleOffset;
		Quaternion startRotation = getAimRotation();
        float power = 0;
        float range = 0;
        int viewId = 0;
        Transform target = null;
        switch (prefiretype)
        {
            case PREFIRETYPE.ChargedPower:
                power += _pumpCoef;
                break;
            case PREFIRETYPE.ChargedRange:
                range += _pumpCoef;
                break;
            case PREFIRETYPE.Guidance:
                if (_pumpCoef >= 1.0f)
                {
                     target = GetGuidanceTarget();
                     if (target != null)
                     {
                         viewId = target.GetComponent<FoxView>().viewID;
                     }
                    
                }
                break;
        }
		for (int i =0; i<NumerOfPeace; i++) {
				GameObject proj;
				float effAimRandCoef = aimRandCoef;
				if (effAimRandCoef > 0) {
						startRotation = Quaternion.Euler (startRotation.eulerAngles + new Vector3 (Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef), Random.Range (-1 * effAimRandCoef, 1 * effAimRandCoef)));
				}
				proj = Instantiate (projectilePrefab, startPoint, startRotation) as GameObject;
                BaseProjectile projScript = proj.GetComponent<BaseProjectile>();
                if ( target != null)
                {
                    projScript.target = target;
                   
                }
               
                projScript.projId = ProjectileManager.instance.GetNextId();
                projScript.replication = false;
				if (foxView.isMine) {
                    foxView.SendShoot(startPoint, startRotation, power, range, viewId, projScript.projId);
				}
				
               
				projScript.damage = new BaseDamage (damageAmount);
				projScript.owner = owner.gameObject;
		}
	}


}
