using UnityEngine;
using System.Collections;
using nstuff.juggerfall.extension.models;

public class Hunt_PVPGameRule : PVPGameRule {

		public override void SetFromModel(GameRuleModel model)
        {
            PVPHuntGameRuleModel pvpmodel = (PVPHuntGameRuleModel)model;
        
            if (!isGameEnded && pvpmodel.isGameEnded)
            {
                GameEnded();
                isGameEnded = true;
            }
            for (int i = 0; i < pvpmodel.teamScore.Count; i++)
            {
               
                teamScore[i] = (int)pvpmodel.teamScore[i];
            }
        }

}