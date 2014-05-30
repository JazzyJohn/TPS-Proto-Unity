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
    public AIType TypeofBehavior;

    public float
        //DetectionRadius,
				AngleRange;
        
	public static float TickPause = 0.3f;
   // private SphereCollider _SC;

    private List<GameObject> _arrayPlayerInRadius = new List<GameObject>();

	private AIState _currentState;

	private Pawn controlledPawn;
    //private AIPatrol _patrol;
    //private AIHolder _holder;
    //private AIRusher _rusher;

	public void WasHitBy(GameObject killer){
		Pawn killerPawn = killer.GetComponent<Pawn> ();
		if (killerPawn != null) {
			_currentState.WasHitBy(killerPawn);
		}

	}

    private void Awake()
    {
        /*_SC = GetComponent<SphereCollider>();
        _SC.isTrigger = true;
        _SC.radius = DetectionRadius;*/
		controlledPawn = GetComponent<Pawn> ();
        switch (TypeofBehavior)
        {
            case AIType.Turret:
                {
				_currentState = gameObject.AddComponent<AITurret>();
				_currentState.controlledPawn = controlledPawn;
				_currentState.AngleRange = AngleRange;		
                }
                break;
            case AIType.Walk:
                {
                _currentState = gameObject.AddComponent<AIWalk>();
                _currentState.controlledPawn = controlledPawn;
				_currentState.AngleRange = AngleRange;	
                }
				break;
            //    break;
            //case AIType.Holder:
            //    {
            //        _holder = gameObject.AddComponent<AIHolder>();
            //        _holder.DistanceXRay = XRayDistance;
            //    }
            //    break;
            //case AIType.Rusher:
            //    {
            //        _rusher = gameObject.AddComponent<AIRusher>();
            //        _rusher.DistanceXRay = XRayDistance;
            //    }
            //    break;

        }
		_currentState.StartState();
		StartCoroutine("Tick");
    }
    private IEnumerator Tick()
    {
        while (true)
        {
          
			_currentState.PawnList = controlledPawn.getAllSeenPawn().ToArray();
			_currentState.Tick();                  
      
            yield return new WaitForSeconds(TickPause);
            //Debug.Log("Work");
        }
    }


}

