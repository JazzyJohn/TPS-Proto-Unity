using UnityEngine;
using System.Collections;
using nstuff.juggerfall.extension.models;

public class Hunt_PVPGameRule : PVPGameRule {

	public override void SetFromModel(GameRuleModel model)
    {
        PVPHuntGameRuleModel pvpmodel = (PVPHuntGameRuleModel)model;
        for (int i = 0; i < pvpmodel.teamScore.Count; i++)
        {

            teamScore[i] = (int)pvpmodel.teamScore[i];
        }
        if (!isGameEnded && pvpmodel.isGameEnded)
        {
            GameEnded();
            isGameEnded = true;
        }
       
    }
		
	public void LastWave(){
		if(NetworkController.IsMaster()){
			NetworkController.Instance.LastWaveRequest();
		}
	}
	
	public void NextWave(int wave){
        PlayerMainGui.instance.Annonce(AnnonceType.WAVEFINISHONE);
	
	}
	public void LastWaveAnonce(){
		PlayerMainGui.instance.Annonce(AnnonceType.SWARMCLEAR);
	}
	
}