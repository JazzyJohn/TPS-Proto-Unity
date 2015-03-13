using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum  BattleState {Inclosing, InDangerArea,WaitForAttack,Attacking,InclosingFromStuck, MoveToBack}

public enum BattleCircleDensity{ Normal,Swarm,Lonely}

public enum BattleCircleAttackSpeed{ Normal,Fast,Slow}

public class AIWalk : AIMovementState
{
	private float _lostTimer;

	public float lostTime=5.0f;
	
	public string desiredCoverTag="Cover";
	
	protected bool isMoving = true;
	
	public float DangerRadius;
	
	private float _DangerRadius;

	public BattleState state;
	
	protected float _lastTimeAttack=0.0f;
	
	protected static float coolDown = 4.0f;
	
	protected static int maxAttackers = 2;

	protected bool attacking= false;

    protected bool hasPermission = false;
	
	protected bool hasCover= false;
	

	
	protected Vector3 backPosition;
	
	protected Transform cover;
	
	

    public bool CirleAttack = false;
	
	public float meleeChance = 0.5f;
	
	private float _timeLastDecide = 20.0f;
	
	public static float TacticDelay = 20.0f;
	//Lower melee chance if target above pawn;	
	public static float aboveMeleeMod =-0.5f;
	//Height after which pawn try to jump to enemy
	public float aboveHeight=1.0f;
	
	public AISkillManager skillmanager;
	
	public BattleCircleDensity density;
	
	public BattleCircleAttackSpeed speed;
	
	public int enemyCount=0;
	
	public bool enemyAbove=false;
	
	protected float _lastJumpTime = 0.0f;
	
	public float jumpDelay = 10.0f;
	
	public void Awake(){
        base.Awake();
        AISkillManager skillmanager  = GetComponent<AISkillManager>();
		maxAttackers =GlobalGameSetting.instance. GetAiSettings(GlobalGameSetting.MAX_ATTACKERS,(int)density,maxAttackers);
        coolDown = GlobalGameSetting.instance.GetAiSettings(GlobalGameSetting.COOL_DOWN, (int)speed, coolDown);
		aboveMeleeMod=GlobalGameSetting.instance. GetAiSettings(GlobalGameSetting.MAX_ATTACKERS,aboveMeleeMod);
		
	}
	
	
	public void UpdateState(){
        BattleState nextstate;
		if(isMelee){
			if(_distanceToTarget>_DangerRadius){
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
            //Debug.Log(_DangerRadius);
			if( IsInWeaponRange()){
			
				
					switch(state){
						case BattleState.InclosingFromStuck:
							if(_distanceToTarget<_DangerRadius){
                               // Debug.Log("indanger");
								nextstate = BattleState.InDangerArea;
							}else{
								nextstate = BattleState.InclosingFromStuck;
							}
						break;
						case BattleState.MoveToBack:
								if(agent.IsRiched(backPosition, controlledPawn.myTransform.position,agent.size*2)){
									nextstate = BattleState.Attacking;
								}else{
									nextstate = BattleState.MoveToBack;
								}
						break;
						default:
						if(isStuck){
                           // Debug.Log("_DangerRadius " + _DangerRadius + " controlledPawn.GetSize() " + controlledPawn.GetSize() + " _enemy.GetSize() " + _enemy.GetSize());
                            if (_DangerRadius / 2.0f >(controlledPawn.GetSize() + _enemy.GetSize()))
                            {

                                _DangerRadius = _DangerRadius / 2.0f;
                              
                                nextstate = BattleState.InclosingFromStuck;
							}else{
                              //  Debug.Log("MoveToBack");
                                _DangerRadius = DangerRadius;
                                FindBackPosition();
                                nextstate = BattleState.MoveToBack;
                              
							}
					
						}else{
							if(_distanceToTarget<_DangerRadius){
								if(_enemy.attackers.Count>=maxAttackers/2){
									nextstate = BattleState.InDangerArea;
								}else{
									nextstate = BattleState.InDangerArea;
									DecideTacktick();
								}
							}else{
								nextstate = BattleState.Attacking;
							}
						}	
						break;
					}
					
				
			}else{
			   nextstate = BattleState.Inclosing;
			}
		
		}

        controlledPawn.ToggleAim(!isMelee);
        state = nextstate;
	
	}   
	protected void DecideTacktick(float addToMelee=0.0f){
		if(_timeLastDecide+TacticDelay<Time.time){
			return;
		}
        DecideTacktickTimeLess(addToMelee);
	}

    public void FindBackPosition()
    {
        Vector3 direction = _enemy.myTransform.position - controlledPawn.myTransform.position;
        backPosition = _enemy.myTransform.position + direction.normalized * _DangerRadius;
        agent.SetTarget(backPosition);
    }
	protected void DecideTacktickTimeLess(float addToMelee=0.0f){
		
		_timeLastDecide =Time.time;
		
		if(controlledPawn.naturalWeapon!=null){
			if(controlledPawn.CurWeapon==null){
				
				
				 MeleeFight();
				return;
			}
			
			float melee =Random.value;
			if(melee<(meleeChance+addToMelee)){
				if(!isMelee){
                    StopAttack();
					
				}
               
				 MeleeFight();
			}else{
				_enemy.attackers.Remove(controlledPawn);
				if(isMelee){
                    StopAttack();
					
				}
               
				RangeFight();
			}
            //Debug.Log("I decide melee" + isMelee);
			return;
		
		
		}
		
		RangeFight();
	}
	protected void MeleeFight(){
		isMelee = true;
		
	}
	protected void RangeFight(){
		isMelee = false;
		FindCover(_enemy.myTransform.position);
	}
	public class CoverRating{
		public int index;
		
		public float rating;
		public CoverRating(int index,float rating){
			this.index= index;
			this.rating = rating;
		}
	}
	public class ByCoverRating: IComparer<CoverRating>
	{
		public int Compare(CoverRating x, CoverRating y)
		{
			if(x.rating==y.rating){
				return 0;
			}else{
				if(x.rating>y.rating){
					return 1;
				}else{
					return -1;
				}
			}			
		}
	}
	protected void FindCover(Vector3 position){
        	
//        Debug.Log("FindCover");
		float weaponDistance =controlledPawn.OptimalDistance(false); 
		Collider[] hitColliders = Physics.OverlapSphere(position, weaponDistance);
        List<CoverRating> allCovers =
                new List<CoverRating>();
		for(int i =0; i<hitColliders.Length;i++){
			if(hitColliders[i].CompareTag(desiredCoverTag)){
                float distance = -(hitColliders[i].transform.position - controlledPawn.myTransform.position).magnitude - (hitColliders[i].transform.position - _enemy.myTransform.position).magnitude;
				allCovers.	Add(new CoverRating(i,distance));
			}
		}
        allCovers.Sort(new ByCoverRating());
		if(allCovers.Count>0){
            cover = hitColliders[allCovers[0].index].transform;
			hasCover= true;
		}else{
			cover = null;
			hasCover=false;
		}
       /// Debug.Log(cover.transform + " has cover " + hasCover);
	}
	public override void Tick()
    {
        if (DirectVisibility(out _distanceToTarget))
        {
            //code to animation attack
			 if (CirleAttack)
            {
				
				
               _pathCoef =0.0f;
				if(SkillUse()){
					isMoving = false;
				}else{
					isMoving = true;
                   
					UpdateState();
                    bool newEnemyAbove = (_enemy.myTransform.position.y - controlledPawn.myTransform.position.y) > aboveHeight;
					if(enemyAbove!=newEnemyAbove){
						enemyAbove=newEnemyAbove;
						DecideTacktickTimeLess(aboveMeleeMod);
					}
					
					if(isMelee){
						
						switch(state){
							case BattleState.Inclosing:
							//move Close to enemy
								StartFollow(_enemy.myTransform);
						
								StopStrafe();
							break;
							case BattleState.InDangerArea:
							
								//If we can attack
								if(_lastTimeAttack+coolDown<Time.time){
									//If amount of attackers small move close
									if(_enemy.attackers.Count<maxAttackers){
										StartFollow(_enemy.myTransform);
									
										StopStrafe();
										
									}else{
									//else strafe around;
										
										StopAvoidFollow();
									   
										StartStrafe(_enemy.myTransform);
									}
								}else{
									//strafe around;
									
									StopAvoidFollow();
								  
									StartStrafe(_enemy.myTransform);
								}
							break;
							case BattleState.WaitForAttack:
									//if we after last attack move away
									AskForPermisssion();
									//Debug.Log("My permission " + hasPermission);
									//if we got permission  start attack
									if(hasPermission){
										Attack();	
										
									}else{
										//Else avoid
									
										StopStrafe();
										StartAvoid(_enemy.myTransform);
									}
								
								
							break;
							case BattleState.Attacking:
								StartFollow(_enemy.myTransform);
								
								StopStrafe();
							break;
						
						}
					}else{
						if(hasCover){
							NormalMovement();
							Attack();	
							Vector3 coverBestPosition =cover.position+(cover.position-_enemy.myTransform.position).normalized *cover.collider.bounds.size.magnitude;
							agent.SetTarget(coverBestPosition, true);
								
						}else{
						
						
							switch(state){
								case BattleState.Inclosing:
									//StopAttack();
									//move Close to enemy
									
									StartFollow(_enemy.myTransform);
									StopStrafe();
									break;	
								case BattleState.Attacking:
									
									StopAvoidFollow();
									Attack();	
									StartStrafe(_enemy.myTransform);
								break;
								case BattleState.InDangerArea:
									StartAvoid(_enemy.myTransform);
									
									StopStrafe();
									Attack();
								break;
								case BattleState.MoveToBack:
                                    if (agent.IsPathBad())
                                    {
                                        FindBackPosition();
                                    }
                                    MoveAround(_enemy.myTransform);
									Attack();
								break; 
								case BattleState.InclosingFromStuck:
									StartStrafeSpiral(_enemy.myTransform);
									Attack();
								break; 
							}
														
						}						
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
			NormalMovement();
			if(_enemy!= null){
				agent.SetTarget(_enemy.myTransform.position, true);
				//Debug.Log(Time.deltaTime+" "+_lostTimer+" "+lostTime);
				_lostTimer+=AIBase.TickPause;
				if(_lostTimer>lostTime){
					_lostTimer = 0.0f;
					//Debug.Log("enemyLOST");
					LostEnemy();

				}

			}
        }
        aibase.cahcedDataForTick["ALLY_COUNT"] = AITargetManager.GetAttackersCount(_enemy);
		aibase.cahcedDataForTick["ALLY_KILL"] = 0;
    }
	
	
	
	public bool SkillUse(){
		if(skillmanager==null){
			return false;
		}	
		if(skillmanager.IsActive()){
			return true;
		}
        foreach (AISkillManager.SkillTrigger skilltrigger in skillmanager.allSkills)
        {
			if(skilltrigger.skill.Available()){
				switch(skilltrigger.type){
					case SkillTriggerType.ENEMY_SEE:
						if(_enemy!=null){
							skillmanager.Use(skilltrigger.skill,_enemy);
							return true;
						}
					break;
					case SkillTriggerType.ENEMY_COUNT:
						if(enemyCount>skilltrigger.count){
                            skillmanager.Use(skilltrigger.skill, _enemy);
							return true;
						}
					break;
					case SkillTriggerType.HEALTH_LEFT:
						if(controlledPawn.health<skilltrigger.count){
                            skillmanager.Use(skilltrigger.skill, _enemy);
							return true;
						}
					break;
					case SkillTriggerType.TIME_PASS:
						if(Time.time-timeStart>skilltrigger.count){
                            skillmanager.Use(skilltrigger.skill, _enemy);
							return true;
						}
					break;
					case SkillTriggerType.ALLY_COUNT:
                    if (AITargetManager.GetAttackersCount(_enemy) > skilltrigger.count)
                    {
                            skillmanager.Use(skilltrigger.skill, _enemy);
							return true;
						}
					break;
					case SkillTriggerType.ALLY_KILL:
						if(aibase.cahcedDataForTick["ALLY_KILL"]>0){
                            skillmanager.Use(skilltrigger.skill, _enemy);
							return true;
						}
					break;
				}
			}
		}
		return false;
		
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
        if (isMelee)
        {
            attacking = true;
            _lastTimeAttack = Time.time;
            _enemy.attackers.Add(controlledPawn);
            hasPermission = false;
        }
		base.Attack();
	}
    public virtual void LostEnemy()
    {
        _enemy.attackers.Remove(controlledPawn);
         base.LostEnemy();

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
		
		stateSpeed =controlledPawn.groundRunSpeed;
		agent.SetSpeed(controlledPawn.groundRunSpeed);
		agent.ParsePawn (controlledPawn);
        aibase.cahcedDataForTick["ALLY_COUNT"] = AITargetManager.GetAttackersCount(_enemy);
		aibase.cahcedDataForTick["ALLY_KILL"] = 0;
        _DangerRadius= DangerRadius;
		base.StartState ();
	}
	
    public override void EndState()
    {
		NormalMovement();
        if (_enemy != null) {
            _enemy.attackers.Remove(controlledPawn);
            _enemy = null;
        }
        controlledPawn.enemy = null;
        base.EndState();
    }
	protected void FixedUpdate(){
		base.FixedUpdate();
		if (_enemy != null&&isMoving) {
           
			//agent.WalkUpdate ();
				//Debug.Log(agent.GetTranslate());
			   
				Vector3 translateVect =  GetSteeringForce();
				
				if(translateVect.sqrMagnitude<0.3f){
					controlledPawn.Movement (Vector3.zero,CharacterState.Idle);

				}else{
					if ( JumpToEnemy(translateVect))
					{
						controlledPawn.Movement (translateVect +controlledPawn.JumpVector(), CharacterState.Jumping);
						
					} else {
						controlledPawn.Movement (translateVect,CharacterState.Running);
					}
				}
			
				CheckStuck();
		}else{

			controlledPawn.Movement (Vector3.zero,CharacterState.Idle);
		}
		
	}
	
	
    public bool JumpToEnemy(Vector3 translateVect)
    {
		if(!controlledPawn.canJump){
			return false;
		}
		if(enemyAbove){
			//If (Melee in At tack)  
			 
			if(isMelee&&(state==BattleState.Attacking||state==BattleState.WaitForAttack)){
				if(_lastJumpTime + jumpDelay<Time.time){
					_lastJumpTime = Time.time;
					return true;
				}else{
					return false;
				}
            }
            else
            {
				needJump = CheckJump(translateVect);
                return needJump;
            }
		}else{
			if(isMelee&&(state==BattleState.Attacking||state==BattleState.WaitForAttack)){
                return false;
			}else{
				needJump = CheckJump(translateVect);
                return needJump;
			}
		}
	}
	public override void SetEnemy(Pawn enemy){
		if (_enemy != enemy) {
				controlledPawn.PlayTaunt ();
                _DangerRadius = DangerRadius;
		}
		
		base.SetEnemy(enemy);

	}
    public override bool ShoudAvoid(Pawn pawn)
    {
        return pawn.team == controlledPawn.team;
    }
	 protected override Pawn SelectTarget()
    {
        //select target by distance
        //but i have mind select by argo
		float curTargetPrior =controlledPawn.seenDistance;
        Pawn target = null;
		if (_enemy != null&&targetVisible) {
            curTargetPrior -= (controlledPawn.seenDistance * 0.9f);
            target = _enemy;
		}
		
        
		
		enemyCount=0;
		foreach (Pawn pawn in _pawnArray)
		{
           
			if(!IsEnemy(pawn)){
				continue;
			}
			enemyCount++;
			Vector3 myPos = controlledPawn.myTransform.position; // моя позиция
			Vector3 targetPos = pawn.myTransform.position; // позиция цели
			Vector3 myFacingNormal = controlledPawn.myTransform.forward; //направление взгляда нашей турели
			Vector3 toTarget = (targetPos - myPos);
			toTarget.Normalize();
			
			float angle = Mathf.Acos(Vector3.Dot(myFacingNormal, toTarget)) * 180 / 3.141596f;
			
			if (angle <= _angleRange / 2)
			{
				if(curTargetPrior>toTarget.sqrMagnitude){
					curTargetPrior =toTarget.sqrMagnitude;
				    target = pawn;
				}
            
               
			}

		}
		return target;
    }
	public override void AllyKill(){
		aibase.cahcedDataForTick["ALLY_KILL"] = 1;
	}
}
