using UnityEngine;
using System.Collections;
using nstuff.juggerfall.extension.models;

public class TeamDeathmatch_PVPGameRule : PVPGameRule {

    public override void SetFromModel(GameRuleModel model)
    {
        PVPGameRuleModel pvpmodel = (PVPGameRuleModel)model;
        for (int i = 0; i < pvpmodel.teamScore.Count; i++)
        {

            teamScore[i] = (int)pvpmodel.teamScore[i];
        }
                                                                    
        timer = pvpmodel.time;
        if (!isGameEnded && pvpmodel.isGameEnded)
        {
            isGameEnded = true;
            GameEnded();
      
        }
      
    }
	/*	public override void ActuakKillCount(int team){
		Debug.Log ("KILL COUNT" + team);
			teamKill[team-1]++;
			teamScore[team-1]++;
		}*/
		
		
}

