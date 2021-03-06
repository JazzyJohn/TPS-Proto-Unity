﻿using UnityEngine;
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
		if (agent.IsRiched(patrolPoints [step].position, controlledPawn.myTransform.position,agent.size*2)||agent.IsPathBad()) {
			NextPoint();
		}
			
		
	}
	protected void NextPoint(){
		step++;
		if(step>=patrolPoints.Length){
			
			step=0;
		}
		agent.SetTarget (patrolPoints[step].position);
        ClearStuck();

	}

	public override void StartState(){
		NormalMovement();
		agent = GetComponent<AIAgentComponent>();
		//Debug.Log (agent);
        if (patrolPoints[step] == null)
        {
            return;
        }
		stateSpeed =controlledPawn.groundWalkSpeed;
        agent.ParsePawn(controlledPawn);
        agent.SetTarget(patrolPoints[step].position);
		agent.SetSpeed(controlledPawn.groundWalkSpeed);
		
		base.StartState ();
		
	}
	protected void FixedUpdate(){
		base.FixedUpdate();
     
        if (patrolPoints[step] == null)
        {
            return;
        }
        
			//agent.WalkUpdate ();
           // Debug.Log("Jump" + needJump + agent.needJump);
			CheckStuck();
            Vector3 translateVect;
			if(isStuck){
				translateVect = GetUnStuckDirection();
			}else{
				translateVect = GetSteeringForce();
			}
			needJump = CheckJump(translateVect  );
			if (!needJump) {
				if(translateVect.sqrMagnitude == 0){
				controlledPawn.Movement(translateVect, CharacterState.Idle);
				}else{
				controlledPawn.Movement(translateVect, CharacterState.Walking);
				}

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
