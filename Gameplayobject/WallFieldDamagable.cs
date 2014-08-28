using UnityEngine;
using System.Collections;

public class WallFieldDamagable : DamagebleObject {

    public DamagebleObject TargetHarm;

    public GameObject effect;
    public float FixInctanceScalePercent = 100;
    public Transform ActualEffect;
    public Transform parentForEffect;

    public float damageMod;
	// Use this for initialization
	void Start () {
	
	}
	
	

	public override void Damage(BaseDamage damage,GameObject killer)
	{
        var inst1 = Instantiate(effect) as GameObject;
        inst1.transform.parent = parentForEffect;
        inst1.transform.position = damage.hitPosition;
        inst1.transform.rotation = ActualEffect.rotation;
        inst1.transform.localScale = transform.localScale * FixInctanceScalePercent / 100f;
        damage.Damage = damage.Damage * damageMod;
        TargetHarm.Damage(damage, killer);
	}


}

