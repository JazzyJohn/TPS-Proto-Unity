using UnityEngine;
using System.Collections;

public class AIBattleJugger : AIMovementState
{

    private float _waitTimer;

    public float waitTime = 5.0f;

    public bool waiting = false;

    public bool isCrushing = false;

    public Transform Target = null;

    public RoutePoint routePoint;

    private RoutePoint curPoint;

    //TODO: Replication of steps;

    public int step = 0;

    public override void Tick()
    {
        if (isCrushing)
        {
            if (Target != null)
            {
                if (IsInWeaponRange())
                {
                    Attack();
                    //isMoving = false;

                    agent.SetTarget(Target.position, true);
                  
                }
                else
                {
                    StopAttack();
                 
                    agent.SetTarget(Target.position, true);
                }
               
            }
            else
            {
                StopAttack();
                isCrushing = false;
            }
           
        }
        else
        {

            DirectVisibility(out _distanceToTarget);
            if (agent.IsRiched(curPoint.GiveTarget().position, controlledPawn.myTransform.position, agent.size))
            {
                NextPoint();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            if (collision.transform.parent.GetComponent<JuggerWall>().team != controlledPawn.team)
            {
                isCrushing = true;
                Target = collision.transform;

            }
        }
            
            if(collision.collider.CompareTag("Base"))
        {

            isCrushing = true;
            Target = collision.transform;

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
       // NetworkController.Instance.NextRouteRequest(nextTarget);
    }
    public void ReCreateRoute(int[] route)
    {
        for (int i = 0; i < route.Length; i++)
        {
            routePoint = routePoint.RecreateRoute(route[i]);
        }
    }
    public void MoveOn()
    {
        waiting = false;
        _waitTimer = 0;
    }
    public override void StartState()
    {
        agent = GetComponent<AIAgentComponent>();
        //Debug.Log (agent);
        Debug.Log("start state");
		stateSpeed =controlledPawn.groundWalkSpeed;
        agent.SetTarget(curPoint.GiveTarget().position);
        agent.SetSpeed(controlledPawn.groundWalkSpeed);
        agent.ParsePawn(controlledPawn);
        base.StartState();

    }
    protected void FixedUpdate(){
		base.FixedUpdate();
        if (waiting)
        {
            _waitTimer += Time.fixedDeltaTime;

            if (_waitTimer > waitTime)
            {
                _waitTimer = 0;
                waiting = false;
                GameRule.instance.MoveOn();
            }
            controlledPawn.Movement(Vector3.zero, CharacterState.Idle);
          
            return;
        }
        bool needJump = agent.needJump;
        agent.WalkUpdate();
        // Debug.Log("Jump" + needJump + agent.needJump);
        needJump = !needJump && agent.needJump;
        Vector3 translateVect = GetSteeringForce(); 
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
    public override bool IsEnemy(Pawn target)
    {
        if (target as BattleJugger != null && target.team != controlledPawn.team && !target.isDead)
        {
          
            return true;
        }else{

            return false;
        }

    }
	public override bool ShoudAvoid(Pawn pawn){
		return pawn.team==controlledPawn.team&&pawn as BattleJugger != null;
	}


    public void GeneratePath(Transform[] patrolPoints)
    {
        RoutePoint temppoint = routePoint;
        if (temppoint.myPoint!=null)
        {
            return;
        }
        foreach (Transform point in patrolPoints)
        {
            temppoint.myPoint = point;
            temppoint.maxWaitTime = 0;
            temppoint.avPoints = new RoutePoint[] { new RoutePoint() };
            temppoint = temppoint.avPoints[0];
        }
         curPoint = routePoint;
    }
    public override void WasHitBy(Pawn killer)
    {
        if (killer.GetComponent<AIWallTurret>()!=null)
        {
            Target = killer.GetComponent<AIBase>().GetAISwarm().transform;
            isCrushing = true;
        }
    }

    protected override bool IsInWeaponRange()
    {
	   float weaponDistance =controlledPawn.OptimalDistance(isMelee);

       Debug.Log(AIAgentComponent.FlatDifference(Target.position, controlledPawn.myTransform.position).sqrMagnitude - controlledPawn.GetSize() * controlledPawn.GetSize());
       return AIAgentComponent.FlatDifference(Target.position, controlledPawn.myTransform.position).sqrMagnitude - controlledPawn.GetSize() * controlledPawn.GetSize() < weaponDistance * weaponDistance;
	}

   
}
