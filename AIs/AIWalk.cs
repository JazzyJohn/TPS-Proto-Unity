using UnityEngine;
using System.Collections;

public enum  BattleState {Inclosing, InDangerArea,WaitForAttack,Attacking}

public class AIWalk : AIMovementState
{
	private float _lostTimer;

	public float lostTime=5.0f;
	
	protected bool isMoving = true;
	
	public float DangerRadius;
	
	public bool hasPermission;
	
	public BattleState state;
	
	protected float _lastTimeAttack=0.f;
	
	protected static float coolDown = 1.f;
	
	protected static int maxAttackers = 2;

	protected bool attacking= false;
	
	public void UpdtaeState(){
		if(_distanceToTarget>DangerRadius){
			state=Inclosing;
		
		}else{
			if( IsInWeaponRange()){
				if(hasPermision){
					state = Attacking;
				}else{
					state = WaitForAttack;
				}
			}else{
				state = InDangerArea;
			}
		}
	
	}
	
	public override void Tick()
    {
        if (DirectVisibility(out _distanceToTarget))
        {
            //code to animation attack
			DecideTacktick();
           // Debug.Log("Shot");
			if(isMelee){
				UpdateState();
				switch(state){
					case Inclosing:
					//move Close to enemy
						_pathCoef=pathCoef;
						StopAvoid();
						StopStrafe();
					break;
					case InDangerArea:
					
						//If we can attack
						if(_lastTimeAttack+coolDown<Time.time){
							//If amount of attackers small move close
							if(enemy.attackers.lenght>=maxAttackers){
								_pathCoef =1.f;
								StopAvoid();
								StopStrafe();
							}else{
							//else strafe around;
								_pathCoef =0.f;
								StopAvoid();
								StartStrafe(Enemy.myTransform);
							}
						}else{
							//strafe around;
							StartStrafe(Enemy.myTransform);
						}
					break;
					case WaitForAttack:
							//if we after last attack move away
							AskForPermisssion();
							//if we got permission  start attack
							if(hasPermision)
								Attack();	
								
							}else{
								//Else avoid
								_pathCoef =0.f;
								StopStrafe();
								StartAvoid(Enemy.myTransform);
							}
						}
						
					break;
					case Attacking;
						_pathCoef =1.f;
						StopAvoid();
						StopStrafe();
					break;
				
				}
			
			
			}else{
			
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
	public override void KickFinish(){
		StopAttack();
	
	}
	protected override void Attack(){
		attacking=true;
		hasPermission= false;
		base.Attack();
	}
	protected override void StopAttack(){
		attacking=false;
		_lastTimeAttack = Time.time;
		base.StopAttack();
	}
	public void AskForPermisssion(){
		return enemy.attackers.lenght<maxAttackers&&_lastTimeAttack+coolDown<Time.time;
	}
	public override void StartState(){
		agent = GetComponent<AIAgentComponent>();
		Debug.Log ("BATTLE");
		stateSpeed =controlledPawn.groundRunSpeed;
		agent.SetSpeed(controlledPawn.groundRunSpeed);
		agent.ParsePawn (controlledPawn);
	

		base.StartState ();
	}
    public override void EndState()
    {
		StopAvoid();
		StopStrafe();
        StopAttack();
        base.EndState();
    }
	public void FixedUpdate(){
		if (_enemy != null&&isMoving) {
            bool needJump = agent.needJump;
			agent.WalkUpdate ();
			//Debug.Log(agent.GetTranslate());
            needJump = !needJump && agent.needJump;
			Vector3 translateVect =  GetSteeringForce();
			
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
