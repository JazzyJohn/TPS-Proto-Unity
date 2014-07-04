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
        agent.SetTarget(patrolPoints[step].position);
		agent.SetSpeed(controlledPawn.groundWalkSpeed);
		agent.ParsePawn (controlledPawn);
		base.StartState ();
		
	}
	public void FixedUpdate(){
       
			bool needJump = agent.needJump;
			agent.WalkUpdate ();
           // Debug.Log("Jump" + needJump + agent.needJump);
			needJump = !needJump&&agent.needJump;
            Vector3 translateVect = agent.GetTranslate();
			if (!needJump) {
                controlledPawn.Movement(translateVect, CharacterState.Walking);

			} else {

                controlledPawn.Movement(translateVect + controlledPawn.JumpVector(), CharacterState.Jumping);
			}
            if (translateVect.sqrMagnitude == 0)
            {
                //Debug.Log("recalculate");
				agent.ForcedSetTarget(patrolPoints[step].position);
            }
			controlledPawn.SetAiRotation( agent.GetTarget());
		
	}

	public override void SetEnemy(Pawn enemy){
		controlledPawn. PlayTaunt();
		
		base.SetEnemy(enemy);
		
	}
	
}
