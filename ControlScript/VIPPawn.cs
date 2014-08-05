using UnityEngine;
using System.Collections;

public class VIPPawn : Pawn {

	public void Start(){
        base.Start();
		foxView.VipSpawnedRequest(GetSerilizedData());	
	}


}