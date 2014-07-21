using UnityEngine;
using System.Collections;

public class AIWalk : AIState
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
		((AISwarm_SimpleWave)aibase.aiSwarm).AddEgg(egg.transform);
	}

	public override bool SpecificFinish(){
		return isLayed;
	}
		
	
	

}
