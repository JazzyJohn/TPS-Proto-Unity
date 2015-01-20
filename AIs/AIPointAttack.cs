using UnityEngine;
using System.Collections;

public class AIPointAttack : AIMovementState
{
	
	
	private bool roaming;
	
	public Transform roamTarget;

	public AssaultPoint[] attackPoints;

	public int step=0;
	
	public void SetAttackPoints(Transform[] attackPoints){
		this.attackPoints = new AssaultPoint[attackPoints.Lenght];
		for(int i =0; i<attackPoints.Lenght;i++){
			this.attackPoints[i] = attackPoints[i].GetComponent<AssaultPoint>();		
		}
		
	}
	
	public override void Tick()
	{
		DirectVisibility (out _distanceToTarget);
        if (attackPoints[step] == null)
        {
            return;
        }
		CheckPoint();
	//	Debug.Log (Vector3.Distance (patrolPoints [step].position, controlledPawn.myTransform.position)+"  "+agent.size);
		if(roaming){
			if (agent.IsRiched(roamTarget, controlledPawn.myTransform.position,agent.size*2)||agent.IsPathBad()) {
				StartRoam();
			}
		
		}else{
			if (agent.IsRiched(attackPoints [step].myTransform.position, controlledPawn.myTransform.position,attackPoints [step].aiRadius)||agent.IsPathBad()) {
				StartRoam();
			}
		}
	
			
		
	}
	protected void CheckPoint(){
		int oldstep= step;
		for(int i =0; i<attackPoints.Lenght;i++){
			if(attackPoint[i].startOwner ==0 ||attackPoint[i].startOwner==controlledPawn.team){
				//home or neutral  Logic Defend
				if(attackPoint[i].owner!=controlledPawn.team){
					step= i;
					break;
				}				
			}else{
				//EnemyPoint ignore for now( later count defender if more then needed then attack)
				if(attackPoint[i].owner==controlledPawn.team){
					step= i;
					break;
				}else{
					continue;
				}
			}			
		}
		if(step!=oldstep){
			roaming =false;
			agent.SetTarget(attackPoints[step].myTransform.position);
		}
	}
	public void StartRoam(){
		roamTarget = attackPoints [step].GetRoamTarget();
		roaming = true;
		agent.SetTarget(roamTarget);
	}
	public override void StartState(){
		NormalMovement();
		agent = GetComponent<AIAgentComponent>();
		//Debug.Log (agent);
		CheckPoint();
        if (attackPoints[step] == null)
        {
            return;
        }
		stateSpeed =controlledPawn.groundWalkSpeed;
        agent.ParsePawn(controlledPawn);
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
				if(roaming){
					agent.ForcedSetTarget(roamTarget);
				}else{
					agent.ForcedSetTarget(patrolPoints[step].myTransform.position);
				}
			}
			
			controlledPawn.SetAiRotation( agent.GetTarget());
			
		
	}

	public override void SetEnemy(Pawn enemy){
		controlledPawn. PlayTaunt();
		
		base.SetEnemy(enemy);
		
	}
	
}
