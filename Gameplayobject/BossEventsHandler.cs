using UnityEngine;
using System.Collections;

public class BossEventsHandler : SpecialEventsHandler {



	public void Damage(GameObject killer,float damage){
		Pawn pawn  = killer.GetComponent<Pawn>();
		if(pawn!=0){
			NetworkController.Instance.BossHitRequest(damage,pawn.foxView.viewID);
		}
	}
	
	
}