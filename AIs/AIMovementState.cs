using UnityEngine;
using System.Collections;

public class AIMovementState : AIState
{

	protected  AIAgentComponent agent;
	
	public Tranform avoid;
	
	public Transform strafe;
	
	public float avoidCoef = 1.f;
	
	public  float separationCoef=1.f;
	
	public float pathCoef=1.f;
	
	public float strafeCoef = 1.f;
	
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
		if(agentAvoidance){
			if(controlledPawn!=null){
				Vector3 addForce = new Vector3();
				List<Pawn> list =controlledPawn.getAllSeenPawn();
				float sizeSqrt =4*controlledPawn.GetSize()*controlledPawn.GetSize() ;
				int neighborCount=0;
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
		}
		if(neighborCount==0){
			return Vector3.zero;
		}else{
			addForce= addForce/neighborCount;
			addForce =  new Vector3(addForce.x-controlledPawn.myTransform.position.x,0,addForce.x-controlledPawn.myTransform.position.y);
			addForce = addForce*(-1);
			return addForce.noprmalized;
		}
	}
    public Vector3 GetSteeringForce(){
		
		return Vector3.ClampMagnitude(agent.GetTranslate()*_pathCoef + DynamicAwoidness()*_separationCoef+AvoidOneTarget()*_avoidCoef +StrafeOneTarget(),stateSpeed );
		
	}
	public Vetcor3 AvoidOneTarget(){
		if(avoid==null){
			return Vector3.zero;
		}
		return  ( myTransform.position -avoid.position).normalized;
	}
	public Vetcor3 StrafeOneTarget(){
		if(strafe==null){
			return Vector3.zero;
		}
		return  Vector3.Cross(Vector3.up*starfeRandCoef,( myTransform.position -strafe.position).normalized);
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
		_strafeCoef=0.f;
		starfeRandCoef=0;
	}
	public void StartAvoid(Transform target){
		avoid= target;
		_avoidCoef= avoidCoef;
	}
	public void StopAvoid(){
		avoid=null;
		_avoidCoef=0.f;
	}
}