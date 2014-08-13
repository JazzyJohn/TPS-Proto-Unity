using UnityEngine;
using System.Collections;

public class AIPatrol : AIMovementState
{
	private float _lostTimer;
	
	public float lostTime=5.0f;

	public Transform[] patrolPoints;

	public int step=0;
	
	public override void Tick()
	{
		DirectVisibility (out _distanceToTarget);
        if (patrolPoints[step] == null)
        {
            return;
        }
	//	Debug.Log (Vector3.Distance (patrolPoints [step].position, controlledPawn.myTransform.position)+"  "+agent.size);
		if (AIAgentComponent.IsRiched(patrolPoints [step].position, controlledPawn.myTransform.position,agent.size)||agent.IsPathBad()) {
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
        if (patrolPoints[step] == null)
        {
            return;
        }
		stateSpeed =controlledPawn.groundWalkSpeed;
        agent.SetTarget(patrolPoints[step].position);
		agent.SetSpeed(controlledPawn.groundWalkSpeed);
		agent.ParsePawn (controlledPawn);
		base.StartState ();
		
	}
	public void FixedUpdate(){

        if (patrolPoints[step] == null)
        {
            return;
        }
			bool needJump = agent.needJump;
			agent.WalkUpdate ();
           // Debug.Log("Jump" + needJump + agent.needJump);
			needJump = !needJump&&agent.needJump;
            Vector3 translateVect = GetSteeringForce();
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
