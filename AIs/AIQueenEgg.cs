using UnityEngine;
using System.Collections;

public class AIQueenEgg : AIState
{
	
	bool isLayed = false;
    public string eggPrefab;
	public Transform CloackPoint;
    public int eggDelay;
	public override void StartState(){
		isLayed = false;
        controlledPawn.Movement(Vector3.zero, CharacterState.Idle);
		((QueenPawn)controlledPawn).StartEggLay();
		base.StartState ();
	}

	public void LayedFinished(){
		isLayed = true;
	}
	public void CreateEgg(){
        if (NetworkController.IsMaster())
        {

            GameObject egg = NetworkController.Instance.QuuenEggSpawn(eggPrefab, CloackPoint.position, CloackPoint.rotation, aibase.aiGroup,eggDelay);
          
        }
	}

    public override bool IsSpecificFinish()
    {
		return isLayed;
	}
		
	
	

}
