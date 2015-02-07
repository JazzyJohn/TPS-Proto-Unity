using UnityEngine;
using System;
using System.Collections;
using Sfs2X.Entities.Data;
using nstuff.juggerfall.extension.models;
using System.Collections.Generic;

public class HoldPosition_PVEGameRule : GameRule {

    

    public bool isArrived;
    

	// Use this for initialization
	void Start () {
        teamScore = new int[2];
	}

    void Update()
    {


        base.Update();
    }

    public override void GameEnded()
    {
        //PhotonNetwork.automaticallySyncScene = true;

        isGameEnded = true;
        //Player player = GameObject.Find ("Player").GetComponent<Player> ();
        //player.GameEnd ();

        GlobalPlayer.instance.MathcEnd();
        Player.localPlayer.GameEnd();
        ItemManager.instance.SendChargeData();
        List<Pawn> pawns = PlayerManager.instance.FindAllPawn();
        foreach (Pawn pawn in pawns)
        {
            pawn.gameEnded();
        }
    }

    public override int Winner()
    {
        return Player.localPlayer.Score.WaveCnt;
    }
    
    public override PlayerMainGui.GameStats GetStats()
    {
        PlayerMainGui.GameStats  stats = new PlayerMainGui.GameStats();
			stats.gameTime = gameTime-timer;
            stats.score = new int[] {maxScore - teamScore[0], Player.localPlayer.Score.WaveCnt };
			stats.maxScore =maxScore;
			return stats;	
		
    }
    public void Arrived(){
        isArrived = true;
        if(NetworkController.IsMaster()){
            NetworkController.Instance.GameRuleArrivedRequest();
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

	
	public override string GetWinnerText(){
		string text = TextGenerator.instance.GetSimpleText("WallPostHoldWinner");
        text = String.Format(text, (Player.localPlayer.Score.AIKill + Player.localPlayer.Score.Kill), Player.localPlayer.Score.WaveCnt);
		return text;
	}
    public void NextWaveGame(int wave)
    {
        PlayerMainGui.instance.Annonce(AnnonceType.WAVEFINISHONE,AnnonceAddType.NONE,wave.ToString());
        Player.localPlayer.Score.WaveCnt++; Debug.Log("next wave" + Player.localPlayer.Score.WaveCnt);

    }
}
