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
	//	Debug.Log (Vector3.Distance (patrolPoints [step].position, controlledPawn.myTransform.position)+"  "+agent.size);
		if (AIAgentComponent.IsRiched(patrolPoints [step].position, controlledPawn.myTransform.position,agent.size)) {
			NextPoint();
		}
			
		
	}
	protected void NextPoint(){
		step++;
		if(step>=patrolPoints.Length){
			
			step=0;
		}
		agent.SetTarget (patrolPoints[step].position);


	}

	public override void StartState(){
		agent = GetComponent<AIAgentComponent>();
		//Debug.Log (agent);
		agent.SetTarget (patrolPoints[0].position);
		agent.SetSpeed(controlledPawn.groundWalkSpeed);
		agent.ParsePawn (controlledPawn);
		base.StartState ();
		
	}
	public void FixedUpdate(){
			bool needJump = agent.needJump;
			agent.WalkUpdate ();
			needJump = !needJump&&agent.needJump;
			if (!needJump) {
					controlledPawn.Movement (agent.GetTranslate (), CharacterState.Walking);
			} else {
			//Debug.Log ("jump");
				controlledPawn.Movement (agent.GetTranslate () +controlledPawn.JumpVector(), CharacterState.Jumping);
			}
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
