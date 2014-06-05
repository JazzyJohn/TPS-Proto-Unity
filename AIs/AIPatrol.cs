using UnityEngine;
using System.Collections;

public class AIPatrol : AIState
{
	private float _lostTimer;
	
	public float lostTime=5.0f;
	
	protected  AIAgentComponent agent;

	public Transform[] patrolPoints;

	public int step=0;
	
	public override void Tick()
	{
		DirectVisibility (out _distanceToTarget);

		if (Vector3.Distance (patrolPoints [step].position, controlledPawn.myTransform.position)<agent.size*2) {
			step++;
			if(step>=patrolPoints.Length){

				step=0;
			}
			agent.SetTarget (patrolPoints[step].position);

		}
			
		
	}
	public override void StartState(){
		agent = GetComponent<AIAgentComponent>();
		//Debug.Log (agent);
		agent.SetTarget (patrolPoints[0].position);
		agent.SetSpeed(controlledPawn.groundWalkSpeed);
		agent.size = controlledPawn.GetSize ()/2;
		base.StartState ();
		
	}
	public void FixedUpdate(){

			agent.WalkUpdate ();
			//Debug.Log(agent.GetTranslate());
			controlledPawn.Movement (agent.GetTranslate(),CharacterState.Walking);
			
			controlledPawn.SetAiRotation( agent.GetTarget());
		
	}
	public override bool IsEnemy(Pawn target){
		if(target.team==controlledPawn.team){
			return false;
		}
		return true;
	}
	public override void SetEnemy(Pawn enemy){
		controlledPawn. PlayTaunt();
		
		base.SetEnemy(enemy);
		
	}
	
}
