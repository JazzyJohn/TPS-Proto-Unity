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

	private AIState _currentState;

	private Pawn controlledPawn;

    public int aiGroup;

    public int homeIndex;
	
	private AISwarm aiSwarm;
    //private AIPatrol _patrol;
    //private AIHolder _holder;
    //private AIRusher _rusher;

	public Transform[] patrolPoints;

	public void Init(int aiGroup,AISwarm aiSwarm,int homeIndex){
		this.aiGroup=aiGroup;
		this.aiSwarm=aiSwarm;
		this.homeIndex = homeIndex;
        this.aiSwarm.respawns[homeIndex].SpawnedSet(controlledPawn);
	}
	
	public void WasHitBy(GameObject killer){
		Pawn killerPawn = killer.GetComponent<Pawn> ();
		if (killerPawn != null) {
			_currentState.WasHitBy(killerPawn);
		}

	}
	void InitState(){
		//Debug.Log (_currentState.GetType ().Name);
		switch (_currentState.GetType().Name){
		case "AITurret":
		{

			_currentState.controlledPawn = controlledPawn;
			_currentState.AngleRange = AngleRange;		
		}
			break;
		case "AIWalk":
		{

			_currentState.controlledPawn = controlledPawn;
			_currentState.AngleRange = AngleRange;	
		}
			break;
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
		if(aiSwarm==null){
			Init(aiGroup,AIDirector.instance.swarms[aiGroup],homeIndex);
		}
		InitState ();
        isStarted = true;
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
    }



   public  bool IsEnemy(int group)
   {
       return aiSwarm.IsEnemy(group);
   }
}

