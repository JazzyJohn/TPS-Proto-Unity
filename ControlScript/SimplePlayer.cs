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
    public override String GetNewBot()
    {
        if (team == 1)
        {
            return PlayerManager.instance.avaibleBots[0];
        }
        else
        {
            return PlayerManager.instance.avaibleBots[1];
        }
    }
    public override void PawnKill(Pawn deadPawn, Player victim, Vector3 position, KillInfo killinfo)
    {
        if (!playerView.isMine)
        {
            return;

        }
        if (victim == this)
        {
            return;
        }

        if (GetCurrentPawn().IsMount())
        {
            killinfo.killerMount = true;
        }
          String victimName;
        AnnonceAddType addtype = AnnonceAddType.NONE;
        if (killinfo.isHeadShoot)
        {
            addtype = AnnonceAddType.HEADSHOT;
        }
        if (victim != null)
        {
            //TODO: move text to config

            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventPawnKillPlayer", this, killinfo);
            PlayerMainGui.instance.KillerAnnonce(PlayerName, victim.PlayerName, killinfo.weaponId);

            if (isPlayerFriend(victim.UID))
            {
                EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventKilledAFriend", this, victim, killinfo);
            }
             victimName = victim.PlayerName;

           
            
                Score.Kill++;
            

        }
        else
        {
            PlayerMainGui.instance.KillerAnnonce(PlayerName, deadPawn.publicName, killinfo.weaponId);
            //TODO: move text to config
            victimName= deadPawn.publicName;
            Score.AIKill++;
            aiKillInRow++;
            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventPawnKillAI", this, killinfo);
         //   StatisticHandler.SendPlayerKillNPC(UID, PlayerName);
        }
   
   
       PlayerMainGui.instance.Annonce(ActionResolver.instance.GetLastKill(), addtype, victimName);
        
    }


}