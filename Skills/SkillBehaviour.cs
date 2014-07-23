using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TargetType{SELF,PAWN,POINT,GROUPOFPAWN_BYSELF,GROUPOFPAWN_BYPAWN,GROUPOFPAWN_BYPOINT  }

public class SkillBehaviour : MonoBehaviour
{
	public string name;
	
	public float coolDown;
	
	private float _coolDown;
	
	public float activationTime;
	
	private float _activationTime;
	
	public bool isUse = false;
	
	protected Pawn owner;
	
	public float radius;
	
	public Transform target;
	
	public Vector3 targetPoint;
		
	public TargetType type;
	
	public GameObject targetEffect;
	
	public GameObject casterEffect;
	
	public void Init(Pawn owner){
		this.owner = owner;
	}
	
	public void Update(){
		Tick(Time.deltaTime);
	}
	protected  void Activate(){
		
		switch(type){
			case TargetType.SELF:
			case TargetType.PAWN:
				ActualUse(target.GetComponent<Pawn>());
				TargetVisualEffect(target.position);
			break;
			case TargetType.POINT:
				ActualUse(targetPoint);
				TargetVisualEffect(targetPoint);
			break;
			case TargetType.GROUPOFPAWN_BYPAWN:
			case TargetType.GROUPOFPAWN_BYSELF:
			TargetVisualEffect(target.position);
			Collider[] hitColliders = Physics.OverlapSphere(target.position, radius);
			List<Pawn> pawns  = new List<Pawn>();
			for(int i =0;i < hitColliders.Length;i++){
				Pawn _target =hitColliders[i].GetComponent<Pawn>();
				pawns.Add(_target);
			}
			ActualUse(pawns);
			break;
			case TargetType.GROUPOFPAWN_BYPOINT:
			TargetVisualEffect(targetPoint);
			Collider[] hitColliders = Physics.OverlapSphere(targetPoint, radius);
			List<Pawn> pawns  = new List<Pawn>();
			for(int i =0;i < hitColliders.Length;i++){
				Pawn _target =hitColliders[i].GetComponent<Pawn>();
				pawns.Add(_target);
			}
			ActualUse(pawns);
			break;
		}
	}
	

	public virtual void TargetVisualEffect(Vector3 Position){
		if(targetEffect!=null){
			Instantiate(targetEffect, position, Quaternion );
		}
	}
	public virtual void CasterVisualEffect(){
		if(casterEffect!=null){
			GameObject effect  =    Instantiate(casterEffect, owner.myTransform.position,  owner.myTransform.rotation) as GameObject;
			effect.transform.parent = owner.myTransform;
		}
		
	}
	public void Tick(float delta){
		if(isUse){
			if(activationTime<=_activationTime){
				isUse = false;
				_coolDown =0;
				Activate();
				activationTime = 0;
			}else{
				_activationTime+= delta;
			}
		}
	}
	public void Use(){
		if(_coolDown>=coolDown){
			isUse = true;
			CasterVisualEffect();
			switch(type){
				case TargetType.SELF:
				case TargetType.GROUPOFPAWN_BYSELF:	
					target = owner;
				case TargetType.PAWN:
				case TargetType.GROUPOFPAWN_BYPAWN:
					target =owner.curLookTarget.GetComponent<Pawn>();				
				break;
				case TargetType.POINT:
				case TargetType.GROUPOFPAWN_BYPOINT:
					targetPoint =owner.getCachedAimRotation();				
				break;				
			}
		}		
	}
	public void Use(Pawn pawn){
		if(_coolDown>=coolDown){
			isUse = true;
			CasterVisualEffect();
			target =pawn;
			
		}		
	}
	public void Use(Pawn pawn){
		if(_coolDown>=coolDown){
			isUse = true;
			CasterVisualEffect();
			target =pawn;
			
		}		
	}
	public void Use(Vector3 point){
		if(_coolDown>=coolDown){
			isUse = true;
			CasterVisualEffect();
			targetPoint =point;
			
		}		
	}
	public float CoolDown(){
		return coolDown -_coolDown;
	}
	protected virtual void ActualUse(Pawn pawn){
	
	}
	
	protected virtual void ActualUse(List<Pawn> pawn){
	
	}
	
	protected virtual void ActualUse(Vector3 target){
	
	}

}