using UnityEngine;
using System.Collections;

public class DamageArea : MonoBehaviour {

	public BaseDamage damageAmount;
	
	public void fireDamage (Pawn target)
	{
		if (target != null) {
			target.addDPS (new BaseDamage (damageAmount), gameObject);
		}
	}
	public singleDPS getId ()
	{
		singleDPS newDPS = new singleDPS ();
		newDPS.damage = new BaseDamage (damageAmount);
		newDPS.killer = gameObject;

		return newDPS;
	}


}