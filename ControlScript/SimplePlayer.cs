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
    public override void PawnKill(Pawn deadPawn, Player victim, Vector3 position, KillInfo killinfo)
    {
        if (!playerView.isMine)
        {
            return;

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


            if (isPlayerFriend(victim.UID))
            {
                EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventKilledAFriend", this, victim, killinfo);
            }
             victimName = victim.PlayerName;

           
            
                Score.Kill++;
            

        }
        else
        {
            //TODO: move text to config
            victimName= deadPawn.publicName;
            Score.AIKill++;
            aiKillInRow++;
            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventPawnKillAI", this, killinfo);
         //   StatisticHandler.SendPlayerKillNPC(UID, PlayerName);
        }
        killInRow++;
        switch (killInRow)
        {
            case 1:
                PlayerMainGui.instance.Annonce(AnnonceType.KILL, addtype, victimName);
                break;
            case 2:
                PlayerMainGui.instance.Annonce(AnnonceType.DOUBLEKILL, addtype, victimName);
                break;
            case 3:
                PlayerMainGui.instance.Annonce(AnnonceType.TRIPLIKILL, addtype, victimName);
                break;
            case 4:
                PlayerMainGui.instance.Annonce(AnnonceType.ULTRAKILL, addtype, victimName);
                break;
            case 5:
                PlayerMainGui.instance.Annonce(AnnonceType.MEGAKILL, addtype, victimName);
                break;
            default:
                PlayerMainGui.instance.Annonce(AnnonceType.RAMPAGE, addtype, victimName);
                break;



        }

    }


}