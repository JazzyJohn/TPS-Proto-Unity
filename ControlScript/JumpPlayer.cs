using UnityEngine;
using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;


public class JumpPlayer : Player {

	public override Pawn GetNewPawn(){
	
			return PlayerManager.instance.SpawmPlayer(PlayerManager.instance.pawnName[0],team,GetBuffs());
		
			
	}
    public override void ButtonControll()
    {
    }


}