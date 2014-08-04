using UnityEngine;
using System.Collections;

public class VIPPawn : Pawn {

	public Awake(){
		base.Awake();
		foxView.VipSpawnedRequest(GetSerilizedData());	
	}


}