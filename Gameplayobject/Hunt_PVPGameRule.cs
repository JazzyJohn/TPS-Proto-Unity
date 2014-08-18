using UnityEngine;
using System.Collections;

public class Hunt_PVPGameRule : PVPGameRule {

		public override void SetFromModel(GameRuleModel model)
        {
            PVPHuntGameRuleModel pvpmodel = (PVPHuntGameRuleModel)model;
        
            if (!isGameEnded && pvpmodel.isGameEnded)
            {
                GameEnded();
                isGameEnded = true;
            }
            for (int i = 0; i < pvpmodel.baseHealth.Count; i++)
            {
               
                teamScore[i] = (int)pvpmodel.teamScore[i];
            }
        }

}