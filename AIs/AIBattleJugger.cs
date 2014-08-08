﻿using UnityEngine;
using System.Collections;

public class AIBattleJugger : AIState
{

    private float _waitTimer;

    public float waitTime = 5.0f;

    public bool waiting = false;

    public bool isCrushing = false;

    public GameObject Target = null;

    protected AIAgentComponent agent;

    public RoutePoint routePoint;

    private RoutePoint curPoint;

    //TODO: Replication of steps;

    public int step = 0;

    public override void Tick()
    {
        if (isCrushing)
        {
            if (Target == null)
            {

                isCrushing = false;
               
            }
            else
            {
                Attack();
            }

        }
        else
        {

            DirectVisibility(out _distanceToTarget);
            if (AIAgentComponent.IsRiched(curPoint.GiveTarget().position, controlledPawn.myTransform.position, agent.size))
            {
                NextPoint();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Base"))
        {

            isCrushing = true;
            Target = collision.gameObject;

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
      
        agent.SetTarget(curPoint.GiveTarget().position);
        agent.SetSpeed(controlledPawn.groundWalkSpeed);
        agent.ParsePawn(controlledPawn);
        base.StartState();

    }
    public void FixedUpdate()
    {
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
    public override bool IsEnemy(Pawn target)
    {
        if (target as BattleJugger != null && target.team != controlledPawn.team && !target.isDead)
        {
          
            return true;
        }else{

            return false;
        }

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
        
    }
}
