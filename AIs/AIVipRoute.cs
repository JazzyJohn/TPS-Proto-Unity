using UnityEngine;
using System.Collections;
[System.Serializable]
public class RoutePoint
{
    public Transform myPoint;
    public RoutePoint[] avPoints;
  
    public float maxWaitTime = 100.0f;

    public Transform GiveTarget()
    {
        return myPoint;
    }
    public RoutePoint NextPoint(out int nextTarget)
    {
        if (avPoints.Length == 0) {
            nextTarget=0;
            return null;
        }
		nextTarget   =(int)(UnityEngine.Random.value * avPoints.Length);
        return avPoints[nextTarget];
    }
	public RoutePoint RecreateRoute(int nextTarget){
		return avPoints[nextTarget];
	}
}

public class AIVipRoute : AIState {

    private float _waitTimer;

    public float waitTime = 5.0f;

    public bool waiting = false;

    protected AIAgentComponent agent;

    public RoutePoint routePoint;

    private RoutePoint curPoint;

    //TODO: Replication of steps;
    
    public int step = 0;

    public override void Tick()
    {
        if (agent.IsRiched(curPoint.GiveTarget().position, controlledPawn.myTransform.position, agent.size))
        {
            NextPoint();
        }
    }
    protected void NextPoint()
    {
        waitTime = curPoint.maxWaitTime;
		int nextTarget;
        curPoint = curPoint.NextPoint(out nextTarget);
        if (curPoint != null)
        {
            
            agent.SetTarget(curPoint.GiveTarget().position);
           
            waiting = true;
        }
		NetworkController.Instance.NextRouteRequest(nextTarget);
    }
	public void ReCreateRoute(int[] route){
		for(int i=0;i<route.Length;i++){
			routePoint = routePoint.RecreateRoute(route[i]);
		}
	}
    public void MoveOn() {
        waiting = false;
        _waitTimer = 0;
    }
    public override void StartState()
    {   
        agent = GetComponent<AIAgentComponent>();
        //Debug.Log (agent);
        curPoint = routePoint;
        agent.SetTarget(curPoint.GiveTarget().position);
        agent.SetSpeed(controlledPawn.groundWalkSpeed);
        agent.ParsePawn(controlledPawn);
        base.StartState();

    }
	protected void FixedUpdate(){
		//  base.FixedUpdate();
        if (waiting)
        {
            _waitTimer += Time.fixedDeltaTime;

            if (_waitTimer > waitTime) {
                _waitTimer = 0;
                waiting = false;
                FindObjectOfType<PVEGameExpidition>().MoveOn();
            }
            controlledPawn.Movement(Vector3.zero, CharacterState.Idle);
            controlledPawn.animator.SetSome("dance");
            return;
        }
        bool needJump = agent.needJump;
        agent.WalkUpdate();
        // Debug.Log("Jump" + needJump + agent.needJump);
        needJump = !needJump && agent.needJump;
        Vector3 translateVect = agent.GetTranslate();
        if (!needJump)
        {
            controlledPawn.Movement(translateVect, CharacterState.Walking);

        }
        else
        {

            controlledPawn.Movement(translateVect + controlledPawn.JumpVector(), CharacterState.Jumping);
        }
        if (translateVect.sqrMagnitude == 0)
        {
            //Debug.Log("recalculate");
            if (curPoint != null)
            {
                agent.ForcedSetTarget(curPoint.GiveTarget().position);
            }
        }
        controlledPawn.SetAiRotation(agent.GetTarget());

    }

    public override void SetEnemy(Pawn enemy)
    {
     

    }
}
