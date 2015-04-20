using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
// Скрипт вешается на часть тела с коллайдером.
// Его задача передавать получаемый урон в родительский скрипт
public class BodyHurt : DamagebleObject {

    public DamagebleObject TargetHarm;

	

	public enum BodySegment {Chest=0, Head, None};
	public BodySegment Organ;

  
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per fra
		void FixedUpdate () {
         //   Debug.DrawLine(transform.position, TargetHarm.transform.position);
		}

	public override void Damage(BaseDamage damage,GameObject killer)
	{
		if (TargetHarm)
		{
            switch (Organ)
            {
                case BodySegment.Head:
                    damage.isHeadshoot = true;
                    break;
                
            }
			
			TargetHarm.Damage(damage, killer);
			/*if (!TargetHarm.isDead) 
			{
				if (TargetHarm.health>damage*multiDamage) {TargetHarm.health -= damage*multiDamage;} else 
				{
					TargetHarm.health = 0f;
					TargetHarm.isDead = true;
				}
			}*/
		}
	}


}
