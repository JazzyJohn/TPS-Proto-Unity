using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AIBase : MonoBehaviour
{
    public AIType TypeofBehavior;

    public float
        //DetectionRadius,
        AngleRange,
        TickPause;

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
            //case AIType.Patrol:
            //    {
            //        _patrol = gameObject.AddComponent<AIPatrol>();
            //        _patrol.DistanceXRay = XRayDistance;
            //    }
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

public enum AIType
{
    Turret,
    Patrol,
    Holder,
    Rusher
}