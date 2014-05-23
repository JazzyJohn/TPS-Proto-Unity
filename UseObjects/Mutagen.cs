using UnityEngine;
using System.Collections;

public class Mutagen :  UseObject {

	public Pawn prefab;
	
	override public bool ActualUse(Pawn target){
		if (target.player != null) {
			//target.player.Respawn(prefab);
			return base.ActualUse(target);
		}
		return false;
	}
}
