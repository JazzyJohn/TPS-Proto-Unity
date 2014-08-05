using UnityEngine;
using System.Collections;

public class PVEGameExpidition : GameRule {

    public Pawn VipPawn;

    public bool isArrived;
    

	// Use this for initialization
	void Start () {
	
	}

    void Update()
    {
        
        
        timer += Time.deltaTime;
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
    public void MoveOn()
    {
        curStage = 0;
    }
    public void Battle() {
        curStage = 1;
    
    }

    public override void SetFromModel(PVEGameRuleModel model)
	{
		PVEGameRuleModel pvemodel = (PVEGameRuleModel)model;
		if (!isGameEnded && pvemodel.isGameEnded)
		{
			GameEnded();
			isGameEnded = true;
		}
		for (int i = 0; i < pvemodel.teamKill.Count; i++)
		{
		  
			teamScore[i] = (int)pvemodel.teamScore[i];
		}
		if(VipPawn==null||VipPawn.foxView.viewID!=pvemodel.vipID){
			VipPawn = GetView(pvemodel.vipID).pawn;
		}
	}

	public virtual void ReadMasterInfo(ISFSObject dt){
		if(dt.ContainsKey("route")){
			VipPawn.GetComponent<AIVipRoute>().ReCreateRoute(dt.GetIntArray("route"));
		}
	}
}
