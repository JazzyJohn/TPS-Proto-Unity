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
		switch(wave){
			case 1:
				PlayerMainGui.instance.Annonce(AnnonceType.WAVEFINISHONE);
			break;
			case 2:
				PlayerMainGui.instance.Annonce(AnnonceType.WAVEFINISHTWO);
			break;
			case 3:
				PlayerMainGui.instance.Annonce(AnnonceType.WAVEFINISHTHREE);
			break;
		}
	
	}
	public void LastWaveAnonce(){
		PlayerMainGui.instance.Annonce(AnnonceType.SWARMCLEAR);
	}
	
}