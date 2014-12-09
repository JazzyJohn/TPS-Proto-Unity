using UnityEngine;
using System.Collections;

public class AIPatrol : AIMovementState
{
	private float _lostTimer;
	
	public float lostTime=5.0f;

	public Transform[] patrolPoints;

	public int step=0;
	
	public NavMeshAgent agent;

	public override void Tick()
	{
		DirectVisibility (out _distanceToTarget);
        if (patrolPoints[step] == null)
        {
            return;
        }
	//	Debug.Log (Vector3.Distance (patrolPoints [step].position, controlledPawn.myTransform.position)+"  "+agent.size);
		if (agent.remainingDistance<=agent.radius || agent.pathStatus==NavMeshPathStatus.PathInvalid) {
			NextPoint();
		}
			
		
	}
	protected void NextPoint(){
		step++;
		if(step>=patrolPoints.Length){
			
			step=0;
		}
		agent.SetDestination(patrolPoints[step].position);


	}

	public override void StartState(){
		agent = GetComponent<NavMeshAgent>();
		agent.enabled= true;
		//Debug.Log (agent);
        if (patrolPoints[step] == null)
        {
            return;
        }
		stateSpeed =controlledPawn.groundWalkSpeed;
        agent.SetDestination(patrolPoints[step].position);
		agent.speed =controlledPawn.groundWalkSpeed;
		
		base.StartState ();
		
	}
	public override void EndState(){
		agent.enabled= false;
	}
	protected void FixedUpdate(){
		base.FixedUpdate();
     
        controlledPawn.SetAiRotation( agent.GetTarget());
		
	}

	public override void SetEnemy(Pawn enemy){
		controlledPawn. PlayTaunt();
		
		base.SetEnemy(enemy);
		
	}
	
}
