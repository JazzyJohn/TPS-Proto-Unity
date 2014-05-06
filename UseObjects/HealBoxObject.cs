using UnityEngine;
using System;


public class HealBoxObject : DestroyableNetworkObject {
	public float healAmount;
	
	override public bool ActualUse(Pawn target){
		target.Heal(healAmount,gameObject)
		return base.ActualUse(target);
	}



}