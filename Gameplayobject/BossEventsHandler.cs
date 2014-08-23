using UnityEngine;
using System.Collections;

public class BossEventsHandler : SpecialEventsHandler {



	public void Damage(GameObject killer,float damage){
		Pawn pawn  = killer.GetComponent<Pawn>();
		if(pawn!=null){
			NetworkController.Instance.BossHitRequest(damage,pawn.foxView.viewID);
		}
	}
	
	
}