using UnityEngine;
using System.Collections;

public class AIWalk : AIState
{
	private float _lostTimer;

	public float lostTime=5.0f;
	
	protected  AIAgentComponent agent;

    public override void Tick()
    {
        if (DirectVisibility(out _distanceToTarget))
        {
            //code to animation attack
			
           // Debug.Log("Shot");
		   
			agent.SetTarget(_enemy.myTransform.position);
        }
        else
        {
			if(_enemy!= null){
			
				//Debug.Log(Time.deltaTime+" "+_lostTimer+" "+lostTime);
				_lostTimer+=AIBase.TickPause;
				if(_lostTimer>lostTime){
					_lostTimer = 0.0f;
					Debug.Log("enemyLOST");
					LostEnemy();

				}

			}
        }
    }
	public virtual void StartState(){
		agent = GetComponent<AIAgentComponent>();
		agent.SetSpeed(controlledPawn.groundRunSpeed);
	 
	}
	public void Update(){
		agent.WalkUpdate();
		controlledPawn.Movement (agent.GetTranslate(),CharacterState.Running);
		controlledPawn.myTransform.rotation =agent.GetRotation();
		
		
	}
	public override bool IsEnemy(Pawn target){
		if(target.team==controlledPawn.team){
			return false;
		}
		return true;
	}
	public virtual void SetEnemy(Pawn enemy){
		controlledPawn. PlayTaunt();
		
		base.SetEnemy(enemy);

	}

}
