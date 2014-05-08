using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
// Скрипт вешается на часть тела с коллайдером.
// Его задача передавать получаемый урон в родительский скрипт
public class BodyHurt : DamagebleObject {

	public Pawn TargetHarm;

	public float multiDamage;

	public enum BodySegment {Chest=0, Head, None};
	public BodySegment Organ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per fra
	void Update () {
	if (!Application.isPlaying)
		{
			switch (Organ)
			{
			case BodySegment.Chest:
				multiDamage = 5f;
				break;
			case BodySegment.Head:
				multiDamage = 1f;
				break;
			case BodySegment.None:
				break;
			}
			destructableObject = false;
		}
	}

	public virtual void Damage(BaseDamage damage,GameObject killer)
	{
		if (TargetHarm)
		{
			damage.Damage *= multiDamage;
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
