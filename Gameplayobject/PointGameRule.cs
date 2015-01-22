using UnityEngine;
using System.Collections;
using nstuff.juggerfall.extension.models;
using System.Collections.Generic;
using Sfs2X.Entities.Data;

public class PointGameRule : GameRule {

	public AssaultPoint[] points;

	public Dictionary<int,AssaultPoint> pointsDictionary = new Dictionary<int,AssaultPoint>();
	
	protected void Awake(){
		base.Awake();
		
		teamScore= new int[PlayerManager.instance.MaxTeam];
		foreach(AssaultPoint point in points){
			pointsDictionary[point.id] = point;
			point.Init();
		}
	}
	

		
		
	public void Update(){
		base.Update();
		if(NetworkController.IsMaster()){
			ISFSArray sendData = new SFSArray();
			foreach(AssaultPoint point in points){
				if(point.NeedUpdate()){
					sendData.AddClass(point.GetModel());				
				}
			}
			if(sendData.Size()>0){
				NetworkController.Instance.GamePointDataRequest(sendData);
			}
		
		}	
	
	}
	public void PointUpdate(AssaultPointModel point){
     
		pointsDictionary[point.id].NetUpdate(point);
	}
	public override int Winner(){
			int maxScore = 0;
			int winner = 0;
			for(int i=0;i<teamScore.Length;i++){
				if(maxScore<teamScore[i]){
					maxScore=teamScore[i];
					winner = i;
				}
			}
			return (winner+1);
		}
		
		
	
	
	public override void SetFromModel(GameRuleModel model)
	{
        PointGameRuleModel pvpmodel = (PointGameRuleModel)model;
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


    public override PlayerMainGui.GameStats GetStats()
    {
        PlayerMainGui.GameStats stats = new PlayerMainGui.GameStats();
        stats.gameTime = gameTime - timer;
        stats.score = new int[] { 0, 0 };
        stats.maxScore = maxScore;
        return stats;
    }
}