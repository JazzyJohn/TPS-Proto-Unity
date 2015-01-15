using UnityEngine;
using System.Collections;

public class BaseShootgun : BaseWeapon {

	public int NumerOfPeace=8;

	protected override void GenerateProjectile(){
        Vector3 startPoint = muzzleCached + muzzleOffset;
		Quaternion startRotation = getAimRotation();
        float power = 0;
        float range = weaponRange;
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
                    target = guidanceTarget;
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
				proj =projectilePrefab.Spawn(startPoint, startRotation);
                BaseProjectile projScript = proj.GetComponent<BaseProjectile>();
                if ( target != null)
                {
                    projScript.target = target;
                   
                }
               
                projScript.projId = ProjectileManager.instance.GetNextId();
                projScript.replication = false;
				if (foxView.isMine) {
                    foxView.PrepareShoot(startPoint, startRotation, power, range, viewId, projScript.projId);
				}
				
               
				projScript.damage = new BaseDamage (damageAmount);
				projScript.owner = owner.gameObject;
				projScript.damage.Damage+=power;
				projScript.range=range;
				projScript.Init();
		}
	}


}
