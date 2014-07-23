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
		
			//Debug.Log(weaponDistance +" " +(_enemy.myTransform.position-controlledPawn.myTransform.position).sqrMagnitude);
		   if(IsInWeaponRange()){
				Attack();	
				//isMoving = false;
                if (!isMelee)
                {
                    isMoving = false;
                }
                else
                {
                    agent.SetTarget(_enemy.myTransform.position, true);
                }
		   }else{
				StopAttack();
				isMoving =true;
                agent.SetTarget(_enemy.myTransform.position, true);
		   }
		
			
			
			
        }
        else
        {
			if(_enemy!= null){
			
				//Debug.Log(Time.deltaTime+" "+_lostTimer+" "+lostTime);
				_lostTimer+=AIBase.TickPause;
				if(_lostTimer>lostTime){
					_lostTimer = 0.0f;
					//Debug.Log("enemyLOST");
					LostEnemy();

				}

			}
        }
    }
	public override void StartState(){
		agent = GetComponent<AIAgentComponent>();
		//Debug.Log (agent);
		agent.SetSpeed(controlledPawn.groundRunSpeed);
		agent.ParsePawn (controlledPawn);
	

		base.StartState ();
	}
    public override void EndState()
    {

        StopAttack();
        base.EndState();
    }
	public void FixedUpdate(){
		if (_enemy != null&&isMoving) {
            bool needJump = agent.needJump;
			agent.WalkUpdate ();
			//Debug.Log(agent.GetTranslate());
            needJump = !needJump && agent.needJump;
			Vector3 translateVect = agent.GetTranslate();
			if(translateVect.sqrMagnitude<0.1f){
				controlledPawn.Movement (Vector3.zero,CharacterState.Idle);

			}else{
                if (!needJump)
                {
					controlledPawn.Movement (translateVect,CharacterState.Running);
				} else {
					controlledPawn.Movement (translateVect +controlledPawn.JumpVector(), CharacterState.Jumping);
				}
			}


		}else{

			controlledPawn.Movement (Vector3.zero,CharacterState.Idle);
		}
		
	}

	public override void SetEnemy(Pawn enemy){
		if (_enemy != enemy) {
				controlledPawn.PlayTaunt ();
		}
		
		base.SetEnemy(enemy);

	}

}
