using UnityEngine;
using System;
using System.Collections;
using Sfs2X.Entities.Data;
using nstuff.juggerfall.extension.models;

public class HoldPosition_PVEGameRule : GameRule {

    

    public bool isArrived;
    

	// Use this for initialization
	void Start () {
	
	}

    void Update()
    {


        base.Update();
    }

    
    public override PlayerMainGui.GameStats GetStats()
    {
        PlayerMainGui.GameStats  stats = new PlayerMainGui.GameStats();
			stats.gameTime = gameTime-timer;
			stats.score = new int[]{teamScore[0] , teamScore[1] };
			stats.maxScore =maxScore;
			return stats;	
		
    }
    public void Arrived(){
        isArrived = true;
        if(NetworkController.IsMaster()){
            NetworkController.Instance.GameRuleArrivedRequest();
        }
    }
    public override int Winner()
    {
        if (isArrived)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public override void MoveOn()
    {
        curStage = MUSIC_STAGE.EXPLORATION;
    }
    public void Battle() {
        curStage = MUSIC_STAGE.BATLLE;
    
    }

    public override void SetFromModel(GameRuleModel model)
	{
		HoldPositionGameRuleModel pvemodel = (HoldPositionGameRuleModel)model;
		if (!isGameEnded && pvemodel.isGameEnded)
		{
			
			isGameEnded = true;
		}
        teamScore = new int[pvemodel.teamScore.Count];
        for (int i = 0; i < pvemodel.teamScore.Count; i++)
		{
		  
			teamScore[i] = (int)pvemodel.teamScore[i];
		}
	
	}

	public override void ReadMasterInfo(ISFSObject dt){
		
	}
		
	public virtual string GetWinnerText(){
		string text = TextGenerator.instance.GetSimpleText("WallPostHoldWinner");
        text = String.Format(text, (Player.localPlayer.Score.AIKill + Player.localPlayer.Score.Kill),teamScore[0]);
		return text;
	}
    public void NextWave(int wave)
    {
        switch (wave)
        {
            case 1:
                PlayerMainGui.instance.Annonce(AnnonceType.WAVEFINISHONE);
                break;
       
        }

    }
}
