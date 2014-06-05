using UnityEngine;
using System.Collections;

public class AIWalk : AIState
{
	private float _lostTimer;

	public float lostTime=5.0f;
	
	protected  AIAgentComponent agent;
	
	protected bool isMoving = true;

    public override void Tick()
    {
        if (DirectVisibility(out _distanceToTarget))
        {
            //code to animation attack
			DecideTacktick();
           // Debug.Log("Shot");
		   float weaponDistance = controlledPawn.OptimalDistance(isMelee);
		   if((_enemy.myTransform.position-controlledPawn.myTransform.position).sqrMagnitude<weaponDistance*weaponDistance){
				Attack();	
				isMoving = false;
		   }else{
				StopAttack();
				isMoving =true;
		   }
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
	public override void StartState(){
		agent = GetComponent<AIAgentComponent>();
		//Debug.Log (agent);
		agent.SetSpeed(controlledPawn.groundRunSpeed);
		agent.size = controlledPawn.GetSize ();

		base.StartState ();
	}
	public void Update(){
		if (_enemy != null&&isMoving) {
			agent.WalkUpdate ();
			//Debug.Log(agent.GetTranslate());
			controlledPawn.Movement (agent.GetTranslate(),CharacterState.Running);

		}else{
			controlledPawn.Movement (Vector3.zero,CharacterState.Idle);
		}
		
	}
	public override bool IsEnemy(Pawn target){
		if(target.team==controlledPawn.team){
			return false;
		}
		return true;
	}
	public override void SetEnemy(Pawn enemy){
		if (_enemy != enemy) {
						controlledPawn.PlayTaunt ();
		}
		
		base.SetEnemy(enemy);

	}

}
