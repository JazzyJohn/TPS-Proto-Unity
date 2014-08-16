using UnityEngine;
using System.Collections;

public enum  BattleState {Inclosing, InDangerArea,WaitForAttack,Attacking}

public class AIWalk : AIMovementState
{
	private float _lostTimer;

	public float lostTime=5.0f;
	
	protected bool isMoving = true;
	
	public float DangerRadius;
	

	public BattleState state;
	
	protected float _lastTimeAttack=0.0f;
	
	protected static float coolDown = 4.0f;
	
	protected static int maxAttackers = 2;

	protected bool attacking= false;

    protected bool hasPermission = false;

    public bool CirleAttack = false;
	
	public float meleeChance = 0.5f;
	
	private float _timeLastDecide = 0.0f;
	
	public static float TacticDelay = 20.0f;
	
	public void UpdateState(){
        BattleState nextstate;
		if(isMelee){
			if(_distanceToTarget>DangerRadius){
			   DecideTacktick();
			   nextstate = BattleState.Inclosing;
			
			}else{
				if( IsInWeaponRange()){
					if (attacking)
					{
					   
						nextstate = BattleState.Attacking;
					}else{
						nextstate = BattleState.WaitForAttack;
					}
				}else{
					nextstate = BattleState.InDangerArea;
				}
			}
			if (state == BattleState.Attacking && nextstate != BattleState.Attacking)
			{
				_enemy.attackers.Remove(controlledPawn);
			}
		}else{
			if( IsInWeaponRange()){
				if(_distanceToTarget<DangerRadius){
					if(_enemy.attackers.Count>=maxAttackers/2){
						nextstate = BattleState.InDangerArea;
					}else{
						nextstate = BattleState.InDangerArea;
						DecideTacktick();
					}
				}else{
					nextstate = BattleState.Attacking;
				}
			}else{
			   nextstate = BattleState.Inclosing;
			}
		
		}
	
      
        state = nextstate;
	
	}
	protected  void DecideTacktick(){
		if(_timeLastDecide+TacticDelay<Time.time){
			return;
		}
		_timeLastDecide =Time.time;
		
		if(controlledPawn.naturalWeapon!=null){
			if(controlledPawn.CurWeapon==null){
				if(!isMelee){
					controlledPawn.ToggleAim(false);
				}
				isMelee= true;
				return;
			}
			
			float melee =Random.value;
			if(melee<meleeChance){
				if(!isMelee){
					controlledPawn.ToggleAim(false);
				}
				isMelee = true;
			}else{
				_enemy.attackers.Remove(controlledPawn);
				if(!isMelee){
					controlledPawn.ToggleAim(true);
				}
				isMelee= false;
			}
            Debug.Log("is melee" + isMelee);
			return;
		
		
		}
		isMelee = false;
	
	}
	public override void Tick()
    {
        if (DirectVisibility(out _distanceToTarget))
        {
            //code to animation attack
			
           // Debug.Log("Shot");
            if (CirleAttack)
            {
				
				
                isMoving = true;
                agent.SetTarget(_enemy.myTransform.position, true);
				UpdateState();
				if(isMelee){
				
					switch(state){
						case BattleState.Inclosing:
						//move Close to enemy
							_pathCoef=pathCoef;
							StopAvoid();
							StopStrafe();
						break;
						case BattleState.InDangerArea:
						
							//If we can attack
							if(_lastTimeAttack+coolDown<Time.time){
								//If amount of attackers small move close
								if(_enemy.attackers.Count<maxAttackers){
									_pathCoef = pathCoef;
									StopAvoid();
									StopStrafe();
									
								}else{
								//else strafe around;
									_pathCoef =0.0f;
									StopAvoid();
								   
									StartStrafe(_enemy.myTransform);
								}
							}else{
								//strafe around;
								_pathCoef = 0.0f;
								StopAvoid();
							  
								StartStrafe(_enemy.myTransform);
							}
						break;
						case BattleState.WaitForAttack:
								//if we after last attack move away
								AskForPermisssion();
								Debug.Log("My permission " + hasPermission);
								//if we got permission  start attack
								if(hasPermission){
									Attack();	
									
								}else{
									//Else avoid
									_pathCoef =0.0f;
									StopStrafe();
									StartAvoid(_enemy.myTransform);
								}
							
							
						break;
						case BattleState.Attacking:
						_pathCoef = pathCoef;
							StopAvoid();
							StopStrafe();
						break;
					
					}
				}else{
					switch(state){
						case BattleState.Inclosing:
							//move Close to enemy
							_pathCoef=pathCoef;
							StopAvoid();
							StopStrafe();
							break;	
						case BattleState.Attacking:
							_pathCoef = 0.0f;
							StopAvoid();
							Attack();	
							StartStrafe(_enemy.myTransform);
						break;
						case BattleState.InDangerArea:
							StartAvoid(_enemy.myTransform);
							_pathCoef = 0.0f;
							StopStrafe();
						break;
					}						
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
        if (CirleAttack)
        {
            
            StopAttack();
        }
	
	}
    void OnDestroy()
    {
        if (_enemy != null)
        {
            _enemy.attackers.Remove(controlledPawn);
        }

    }
	protected override void Attack(){
		attacking=true;
        _lastTimeAttack = Time.time;
        _enemy.attackers.Add(controlledPawn);
        hasPermission = false;
		base.Attack();
	}
	protected override void StopAttack(){
		attacking=false;
		
		base.StopAttack();
	}
	public void AskForPermisssion(){
        
        hasPermission =   (_enemy.attackers.Count < maxAttackers)&& (_lastTimeAttack + coolDown < Time.time);
       
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
        _enemy.attackers.Remove(controlledPawn);
        _enemy = null;
        controlledPawn.enemy = null;
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
    public override bool ShoudAvoid(Pawn pawn)
    {
        return pawn.team == controlledPawn.team;
    }

}
