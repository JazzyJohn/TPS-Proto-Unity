using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIMovementState : AIState
{

	protected  AIAgentComponent agent;
	
	public Transform avoid;
	
	public Transform strafe;
	
	public float avoidCoef = 1.0f;
	
	public  float separationCoef=1.0f;
	
	public float pathCoef=1.0f;
	
	public float strafeCoef = 1.0f;
	
	protected float _avoidCoef;
	
	protected  float _separationCoef;
	
	protected float _pathCoef;
	
	protected float _strafeCoef;
	
	private int starfeRandCoef;
	
	protected void Awake(){
		base.Awake();
		_avoidCoef=avoidCoef;
		_separationCoef =separationCoef;
		_pathCoef  = pathCoef;
		
	}
	protected float stateSpeed;

	public virtual bool ShoudAvoid(Pawn pawn){
		return  !IsEnemy(pawn);
	}
	public Vector3 DynamicAwoidness(){
        int neighborCount = 0;
        Vector3 addForce = new Vector3();
		if(controlledPawn!=null){
			
			List<Pawn> list =controlledPawn.getAllSeenPawn();
			float sizeSqrt =4*controlledPawn.GetSize()*controlledPawn.GetSize() ;
			
			foreach(Pawn pawn in list){
					
				if(ShoudAvoid(pawn)){
					Vector3 distance = (pawn.myTransform.position -controlledPawn.myTransform.position);
					if(distance.sqrMagnitude<=sizeSqrt){
						addForce.z+= distance.z;
						addForce.x+= distance.x;
						neighborCount++;
					}
				}
			}				
		}
		
		if(neighborCount==0){
			return Vector3.zero;
		}else{
			addForce= addForce/neighborCount;
			addForce =  new Vector3(addForce.x-controlledPawn.myTransform.position.x,0,addForce.x-controlledPawn.myTransform.position.y);
			addForce = addForce*(-1);
			return addForce.normalized;
		}
	}
    public Vector3 GetSteeringForce(){
		
		return Vector3.ClampMagnitude(agent.GetTranslate()*_pathCoef + DynamicAwoidness()*_separationCoef+AvoidOneTarget()*_avoidCoef +StrafeOneTarget(),stateSpeed );
		
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
	public void StopStrafe(){
		strafe=null;
		_strafeCoef=0.0f;
		starfeRandCoef=0;
	}
	public void StartAvoid(Transform target){
		avoid= target;
		_avoidCoef= avoidCoef;
	}
	public void StopAvoid(){
		avoid=null;
		_avoidCoef=0.0f;
	}
}