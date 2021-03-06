using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIMovementState : AIState
{
	public const float MAX_AHEAD  =2.0f;
	
	public const float VELOCITY_THRESHOLD  =1.0f;
	
	protected  AIAgentComponent agent;
	
	protected Transform avoid;
	
	protected Transform strafe;
	
	protected RaycastHit colliTarget;

    protected bool isCollisionAhead;

	public float avoidCoef = 1.0f;

    public float avoidnessRadius = 10.0f;
	
	public  float separationCoef=1.0f;
	
	public float pathCoef=1.0f;
	
	public float strafeCoef = 1.0f;
	
	public float collAvoidCoef = 1.0f;
	
	protected float _collAvoidCoef;
	
	protected float _avoidCoef;
	
	protected  float _separationCoef;
	
	protected float _pathCoef;
	
	protected float _strafeCoef;
	
	private int starfeRandCoef;
	
	public static float STRAFE_TIME_HALF = 1.0f;
	
	float strafeTimer;
	
	float strafeTimeHalf;
	
	public bool needJump = false;
	
	float stuckTimer=0;
	
	Vector3 lastStuckPosition= Vector3.zero;
	
	protected bool isStuck = false;

	protected void Awake(){
		base.Awake();
		_avoidCoef=avoidCoef;
		_separationCoef =separationCoef;
		_pathCoef  = pathCoef;
        _strafeCoef = strafeCoef;
		_collAvoidCoef =collAvoidCoef;
		strafeTimeHalf=GlobalGameSetting.instance. GetAiSettings(GlobalGameSetting.STRAFE_TIMER_HALF,STRAFE_TIME_HALF);
		strafeTimer= strafeTimeHalf+ UnityEngine.Random.Range(0,strafeTimeHalf);
	}
	protected float stateSpeed;

	public virtual bool ShoudAvoid(Pawn pawn){
		return  !IsEnemy(pawn);
	}
	protected void FixedUpdate(){
        if(_collAvoidCoef!=0){
		     RaycastHit hitInfo;
             Vector3 velocity = controlledPawn.GetVelocity() * MAX_AHEAD;
             if (Physics.SphereCast(controlledPawn.myTransform.position, controlledPawn.GetSize() / 2, velocity.normalized,out hitInfo, velocity.magnitude * MAX_AHEAD, agent.dynamicObstacleLayer))
             {
			    colliTarget =hitInfo;
                isCollisionAhead = true;
		 
		     }else{
                 isCollisionAhead = false;
		     }
	    }
		
		if(_strafeCoef!=0){
            RaycastHit hitInfo;
			if( controlledPawn.GetVelocity().sqrMagnitude<VELOCITY_THRESHOLD){
                if (Physics.SphereCast(controlledPawn.myTransform.position, controlledPawn.GetSize() / 2, StrafeOneTarget(), out hitInfo, MAX_AHEAD, agent.obstacleMask))
				{
					 ChangeStrafeCoef();
					
				}
			}
			strafeTimer-=Time.fixedDeltaTime;
			if(strafeTimer<=0){
				 ChangeStrafeCoef();
			}
			
			
		
		}
	}
	public Vector3 DynamicAwoidness(){
        int neighborCount = 0;
        Vector3 addForce = new Vector3();
		if(controlledPawn!=null){
			
			List<Pawn> list =controlledPawn.getAllSeenPawn();
            
			
			foreach(Pawn pawn in list){
					
				if(ShoudAvoid(pawn)){
                    if (pawn != null && pawn.myTransform != null)
                    {
                        Vector3 distance = (pawn.myTransform.position - controlledPawn.myTransform.position);
                        if (distance.sqrMagnitude <= avoidnessRadius)
                        {
                            addForce += distance;
                            neighborCount++;
                        }
                    }
					
				}
			}				
		}
		
		if(neighborCount==0){
			return Vector3.zero;
		}else{
			addForce= addForce/neighborCount;
            //addForce = addForce - controlledPawn.myTransform.position;
			addForce = addForce*(-1);
			return addForce.normalized;
		}
	}
    public Vector3 GetSteeringForce(){
        Debug.DrawRay(controlledPawn.myTransform.position, DynamicAwoidness() * _separationCoef, Color.black);
        Debug.DrawRay(controlledPawn.myTransform.position, AvoidOneTarget() * _avoidCoef, Color.blue);
        Debug.DrawRay(controlledPawn.myTransform.position, StrafeOneTarget() * _strafeCoef, Color.green);
        Debug.DrawRay(controlledPawn.myTransform.position, agent.GetTranslate() * _pathCoef, Color.red);
	    Debug.DrawRay(controlledPawn.myTransform.position, CollisionAvoidness()  * _collAvoidCoef, Color.yellow);
		Vector3 result = (PathFollow()*_pathCoef + DynamicAwoidness()*_separationCoef+AvoidOneTarget()*_avoidCoef +StrafeOneTarget()*_strafeCoef+CollisionAvoidness()*_collAvoidCoef).normalized;
		//Debug.DrawRay(controlledPawn.myTransform.position,result, Color.white);
		return result*stateSpeed;
		
	}
	
	public Vector3 GetUnStuckDirection(){
		return agent.GetUnStuckDirection();
	
	}
	public Vector3 PathFollow(){
		agent.WalkUpdate();
		if(_pathCoef==0){
			return Vector3.zero;
		}
		
		Vector3 result = agent.GetTranslate();
		if(result.sqrMagnitude==0){
			result= (agent.GetFinishPoint()-controlledPawn.myTransform.position);
		}
        return result.normalized;
	}
	
	public Vector3 AvoidOneTarget(){
		if(avoid==null){
			return Vector3.zero;
		}
		return  ( controlledPawn.myTransform.position -avoid.position).normalized;
	}
    public Vector3 StrafeOneTarget()
    {
		if(strafe==null){
			return Vector3.zero;
		}
        return Vector3.Cross(Vector3.up * starfeRandCoef, (controlledPawn.myTransform.position - strafe.position).normalized);
	}
	public void ChangeStrafeCoef(){
        //Debug.Log("strafe");
		starfeRandCoef =-starfeRandCoef;
		strafeTimer =strafeTimeHalf+ UnityEngine.Random.Range(0,strafeTimeHalf);
	}
	public void StartStrafe(Transform target){
		strafe= target;
		_strafeCoef= strafeCoef;
		if(starfeRandCoef==0){
			if(Random.value>0.5){

				starfeRandCoef = -1;
			}else{
				starfeRandCoef = +1;
			}
		}
	}
	public void StartStrafeSpiral(Transform target){
		StartFollow(target);
		StartStrafe(target);
	}
	public void StopStrafe(){
		strafe=null;
		_strafeCoef=0.0f;
		starfeRandCoef=0;
	}
	public void StartAvoid(Transform target){
		avoid= target;
		_avoidCoef= avoidCoef;
	}
	public void StartFollow(Transform target){
		avoid= target;
		_avoidCoef= -avoidCoef;
	}
	public void StopAvoidFollow(){
		avoid=null;
		_avoidCoef=0.0f;
	}
	public Vector3 CollisionAvoidness(){
        if (!isCollisionAhead)
        {
			return Vector3.zero;
		}
		
        Vector3 ahead = controlledPawn.myTransform.position + controlledPawn.GetVelocity() * MAX_AHEAD;
		Vector3 avoidForce= (ahead-colliTarget.point);
        float size = controlledPawn.GetSize();
		if(avoidForce.sqrMagnitude<size*size){
			return avoidForce.normalized;
		}else return Vector3.zero;
	}
    public void MoveAround(Transform target)
    {
        avoid = target;
        if ((target.position - controlledPawn.myTransform.position).sqrMagnitude <= controlledPawn.GetSize() * controlledPawn.GetSize())
        {
            _avoidCoef = avoidCoef / 4.0f;
        }
        else
        {
            _avoidCoef = 0.0f;
        }
        
        StopStrafe();
        StopAttack();
        _separationCoef = separationCoef;
        _pathCoef = pathCoef;
        _collAvoidCoef = collAvoidCoef;
    }
	public void NormalMovement(){
        StopAvoidFollow();
		StopStrafe();
        StopAttack();
		_separationCoef =separationCoef;
		_pathCoef  = pathCoef;
      	_collAvoidCoef =collAvoidCoef;
	}

    public bool CheckJump(Vector3 translate)
    {
		if(!controlledPawn.canJump){
			return false;
		}
        translate.y=0;
       /* RaycastHit hit;
        bool result = 
        if (result)
        {
            Debug.Log(hit.collider.gameObject);
        }*/
        return Physics.Raycast(controlledPawn.myTransform.position + Vector3.up * controlledPawn.stepHeight, translate.normalized, controlledPawn.GetSize() , agent.obstacleMask);
    }
	
	protected void CheckStuck(){
		stuckTimer += Time.fixedDeltaTime;
		if(stuckTimer>1.0f){
			stuckTimer= 0;
          
			if((lastStuckPosition-controlledPawn.myTransform.position).sqrMagnitude<stateSpeed/3.0f){
				isStuck= true;
           
			}else{
                if (isStuck)
                {
                    UnStuck();
                }
				isStuck= false;
			
			}
         ///s   Debug.Log("stuck check " + (lastStuckPosition - controlledPawn.myTransform.position).sqrMagnitude + "   " + stateSpeed / 2.0f);
			lastStuckPosition= controlledPawn.myTransform.position;
            if (isStuck)
            {
                agent.CheckPathPosition(lastStuckPosition);
            }
		}
		
	}
    protected void ClearStuck()
    {
        stuckTimer = 0;
        lastStuckPosition = Vector3.zero;
    }
    protected virtual void  UnStuck(){
        agent.RecalculatePath();
    }
}