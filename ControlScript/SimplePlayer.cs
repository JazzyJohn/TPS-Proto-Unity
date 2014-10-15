using UnityEngine;
using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;


public class SimplePlayer : Player {

	public override Pawn GetNewPawn(){
		if(team==1){
			return PlayerManager.instance.SpawmPlayer(PlayerManager.instance.pawnName[0],team,GetBuffs());
		}else{
			return PlayerManager.instance.SpawmPlayer(PlayerManager.instance.pawnName[1],team,GetBuffs());
		}
			
	}


}