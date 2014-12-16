using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AIType
{
    Turret,
	Walk,
    Patrol,
    Holder,
    Rusher
}

public class AIBase : MonoBehaviour
{
    public AIState defaultAIState;

    public float
        //DetectionRadius,
				AngleRange;

    public bool isStarted = false;
	public int PatrolPointCount;

    public float timer=0f;
        
	public static float TickPause = 0.3f;
   // private SphereCollider _SC;

    private List<GameObject> _arrayPlayerInRadius = new List<GameObject>();

	protected AIState _currentState;

	protected Pawn controlledPawn;

    public bool standAlone = false;

    public int aiGroup;

    public int homeIndex;
	
	private AISwarm aiSwarm;
    //private AIPatrol _patrol;
    //private AIHolder _holder;
    //private AIRusher _rusher;

	public Transform[] patrolPoints;

	public Dictionary<string,int> cahcedDataForTick= new Dictionary<string,int>();
	
	
    public void Death(){

        if (aiSwarm != null) {
            aiSwarm.AgentKilled(this);
        }

        if (controlledPawn!=null&&controlledPawn.enemy != null)
        {
            AITargetManager.RemoveAttacker(controlledPawn, controlledPawn.enemy);
		}
    }

	public void Init(int aiGroup,AISwarm aiSwarm,int homeIndex){
        if (controlledPawn == null) {
            controlledPawn = GetComponent<Pawn>();
        }
		this.aiGroup=aiGroup;
        //Debug.Log("Group after set" + this.aiGroup + "  " + aiGroup);
		this.aiSwarm=aiSwarm;
		this.homeIndex = homeIndex;
		
	}
	public void RemoteInit(int group, int homeindex){
        controlledPawn = GetComponent<Pawn>();
		aiGroup= group;
		this.homeIndex =homeindex;
        this.aiSwarm = AIDirector.instance.swarms[aiGroup];
		AIDirector.instance.swarms[aiGroup].RemoteAdd(this);
	}
	public void WasHitBy(GameObject killer,float amount){
		if(killer==null){
			return;
		}
		Pawn killerPawn = killer.GetComponent<Pawn> ();
		if (killerPawn != null&&_currentState!=null) {
			_currentState.WasHitBy(killerPawn);
		}

	}
	public void KickFinish(){
        if (_currentState != null)
        {
            _currentState.KickFinish();
        }
	
	}
	void InitState(){
		_currentState.controlledPawn = controlledPawn;
        _currentState.AngleRange = AngleRange;	
		switch (_currentState.GetType().Name){

			//    break;
		case "AIPatrol":
		{

			_currentState.controlledPawn = controlledPawn;
			
			if(patrolPoints.Length==0){
				patrolPoints = aiSwarm.GetPointOfInterest(PatrolPointCount);
			}
			_currentState.AngleRange = AngleRange;	
			((AIPatrol)_currentState).patrolPoints = patrolPoints;
		}
			break;
        case "AIBattleJugger":
            {

                _currentState.controlledPawn = controlledPawn;

                
                patrolPoints = aiSwarm.GetRoutePoint();
             
                _currentState.AngleRange = AngleRange;
                ((AIBattleJugger)_currentState).GeneratePath(patrolPoints);
            }
            break;
         


            break;
			//case AIType.Rusher:
			//    {
			//        _rusher = gameObject.AddComponent<AIRusher>();
			//        _rusher.DistanceXRay = XRayDistance;
			//    }
			//    break;
			
		}
		_currentState.enabled = true;
		_currentState.StartState();
	}
	
	public void StartAI()
	{
		/*_SC = GetComponent<SphereCollider>();
        _SC.isTrigger = true;
        _SC.radius = DetectionRadius;*/
		_currentState = defaultAIState;
		controlledPawn = GetComponent<Pawn> ();
		if(aiSwarm==null&&!standAlone){
			Init(aiGroup,AIDirector.instance.swarms[aiGroup],homeIndex);
		}
		InitState ();
        isStarted = true;
        enabled = true;
    }
   void Update()
    {
        timer += Time.deltaTime;
        if (isStarted && timer >= TickPause && !controlledPawn.isDead)
        {
            timer = 0f;
			_currentState.PawnList = controlledPawn.getAllSeenPawn().ToArray();
			_currentState.Tick();                  
			foreach(AIState.AITransition trans in _currentState.Transition){
				if(trans.Trasite()){
					_currentState.EndState();
					_currentState.enabled = false;

					_currentState= trans.target;
					
					defaultAIState= _currentState;
                    InitState();
					break;
				}

			}

			
        }
        if (!standAlone && aiSwarm == null)
        {
            controlledPawn.RequestKillMe();
        }
    }



   public virtual bool IsEnemy(Pawn target)
   {
       return aiSwarm.IsEnemy(target.mainAi.aiGroup);
   }
   
   public virtual bool IsPlayerEnemy(Pawn target)
   {
       return true;
   }
   public AISwarm GetAISwarm()
   {
       return aiSwarm;
   }
   public Pawn GetPawn(){
		return controlledPawn;
   }
   
   public void SetEnemy(Pawn enemy){
		if(aiSwarm!=null){
            aiSwarm.NewEnemy(enemy);
		}
   }
   public virtual void EnemyFromSwarm(Pawn enemy){
		_currentState.EnemyFromSwarm(enemy);
   }

   public void AllyKill()
   {
       if (_currentState != null)
       {
           _currentState.AllyKill();
       }
   }
}

