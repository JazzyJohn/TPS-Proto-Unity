using UnityEngine;
using System.Collections;

public class AIQueenEgg : AIState
{
	
	bool isLayed = false;
    public GameObject eggPrefab;
	public Transform CloackPoint;
	public override void StartState(){
		isLayed = false;
		((QueenPawn)controlledPawn).StartEggLay();
		base.StartState ();
	}

	public void LayedFinished(){
		isLayed = false;
	}
	public void CreateEgg(){
		GameObject egg =  Instantiate(eggPrefab,CloackPoint.position,CloackPoint.rotation) as GameObject;
        ((AISwarm_QueenSwarm)aibase.GetAISwarm()).AddEgg(egg.transform);
	}

    public override bool IsSpecificFinish()
    {
		return isLayed;
	}
		
	
	

}
