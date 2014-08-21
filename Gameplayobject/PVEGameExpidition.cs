using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using nstuff.juggerfall.extension.models;

public class PVEGameExpidition : GameRule {

    public Pawn VipPawn;

    public bool isArrived;
    

	// Use this for initialization
	void Start () {
	
	}

    void Update()
    {


        base.Update();
    }

    public bool IsGameEnded()
    {
        if (!start)
        {
            return false;
        }
        return isArrived || VipPawn.health <= 0;
    }
    public override PlayerMainGui.GameStats GetStats()
    {
        PlayerMainGui.GameStats  stats = new PlayerMainGui.GameStats();
			stats.gameTime = gameTime-timer;
			stats.score = new int[]{(int)VipPawn.health, 0};
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
        curStage = 0;
    }
    public void Battle() {
        curStage = 1;
    
    }

    public override void SetFromModel(GameRuleModel model)
	{
		PVEGameRuleModel pvemodel = (PVEGameRuleModel)model;
		if (!isGameEnded && pvemodel.isGameEnded)
		{
			
			isGameEnded = true;
		}
        teamScore = new int[pvemodel.teamScore.Count];
        for (int i = 0; i < pvemodel.teamScore.Count; i++)
		{
		  
			teamScore[i] = (int)pvemodel.teamScore[i];
		}
		if(VipPawn==null||VipPawn.foxView.viewID!=pvemodel.vipID){
			VipPawn = NetworkController.GetView(pvemodel.vipID).pawn;
		}
	}

	public virtual void ReadMasterInfo(ISFSObject dt){
		if(dt.ContainsKey("route")){
			VipPawn.GetComponent<AIVipRoute>().ReCreateRoute(dt.GetIntArray("route"));
            
		}
	}
}
