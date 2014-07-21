using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	

public enum AITriggerType {SeeEnemy,LostEnemy,SpecificFinish,TimedChange};
public interface AITrigger
{
	bool isTriggered (AIState owner, Params[] parametrs);
}

[System.Serializable]
public class Params{
	public string name;
	public float value;

}



public class AIState : MonoBehaviour {
	[System.Serializable]
	public class AITransition{
		public AITriggerType triggerType;
		public AIState target;
		public Params[] parameters;
		public AITrigger trigger; 
		private AIState owner;
		public void Start(AIState sOwner){
			switch (triggerType) {
			case AITriggerType.SeeEnemy:
				trigger = new SeeEnemy();
				break;
			case AITriggerType.LostEnemy:
				trigger = new LostEnemyTrigger();
				break;
			case AITriggerType.SpecificFinish:
				trigger = new SpecificFinish();
				break;
			case AITriggerType.TimedChange:
				trigger = new TimedTrigger();
			break;

			}
			owner = sOwner;
		}
		public bool Trasite(){
			return trigger.isTriggered (owner,parameters);
		}
			
	}


	public class SeeEnemy:AITrigger{
		public bool isTriggered(AIState owner, Params[] parametrs){
			if (owner._enemy != null) {
								return true;
			} else {
				return false;
			}		
				
		}
		
	}	
	public class LostEnemyTrigger:AITrigger{
		public bool isTriggered(AIState owner, Params[] parametrs){
			if (owner._enemy == null) {
				return true;
			} else {
				return false;
			}		
			
		}
		
	}	
	public class SpecificFinish:AITrigger{
		public bool isTriggered(AIState owner, Params[] parametrs){
			if (owner.SpecificFinish()) {
				return true;
			} else {
				return false;
			}		
			
		}
		
	}	
	public class TimedTrigger:AITrigger{
		public float timeStart;
		public TimedTrigger(){
			timeStart = Time.time;
		}
		public bool isTriggered(AIState owner, Params[] parametrs){
			float delay = 0;
			foreach(Params param as parametrs){
				if(param.name=="TimedTrigger"){
					delay =param.value;
				}
			}
			if (delay+timeStart<Time.time) {
				return true;
			} else {
				return false;
			}		
			
		}
		
	}	
	public  Pawn controlledPawn;

	protected Pawn _enemy;
	
	public bool isAgressive;

	public AITransition[] Transition;

	protected Pawn[] _pawnArray;
	
	protected float
				_distanceToTarget,
				_angleRange;
				
	protected bool isMelee= false;

    public AIBase aibase;
	
	public Pawn[] PawnList
	{
		set
		{
			_pawnArray = value;
			
		}
	}
	
	public float AngleRange
	{
		set
		{
			if (value > 0)
				_angleRange = value;
		}
	}

	// Use this for initialization
	void Awake () {
        aibase = GetComponent<AIBase>();
        enabled = false;
	}
	
	// Update is called once per frame
	public virtual void Tick () {
	
	}

	
	public virtual void SetEnemy(Pawn enemy){
		_enemy = enemy;
		controlledPawn.enemy = enemy;

	}
	public void LostEnemy(){
		_enemy = null;
		controlledPawn.enemy = null;

	}
	public virtual bool IsEnemy(Pawn target){
		if(isAgressive){
			if(target.isAi){
                //Debug.Log("ENEMY? ME"+controlledPawn +"YOu" +target);
                return aibase.IsEnemy(target.mainAi.aiGroup);
			}else{
                return true;
			}
		}else{
			
			return false;
		}
		
	}
	
	public virtual void WasHitBy(Pawn killer){
		if (_enemy != null) {
			return;
		}
		SetEnemy(killer);
	}
	
	public virtual void StartState(){
		foreach (AITransition trans in Transition) {
			trans.Start(this);
		}
		
		_enemy = controlledPawn.enemy;
		
	}
	public virtual void EndState(){
	
	}
    protected Pawn SelectTarget()
    {
        //select target by distance
        //but i have mind select by argo
		if (_enemy != null) {
			return _enemy;
		}
		Pawn target = null;
//		Debug.Log (_pawnArray);
		foreach (Pawn pawn in _pawnArray)
		{
            //Debug.Log("ENEMY? ME" + controlledPawn + "YOu" + pawn);
			if(!IsEnemy(pawn)){
				continue;
			}
			Vector3 myPos = transform.position; // моя позиция
			Vector3 targetPos = pawn.transform.position; // позиция цели
			Vector3 myFacingNormal = transform.forward; //направление взгляда нашей турели
			Vector3 toTarget = (targetPos - myPos);
			toTarget.Normalize();
			
			float angle = Mathf.Acos(Vector3.Dot(myFacingNormal, toTarget)) * 180 / 3.141596f;
			
			if (angle <= _angleRange / 2)
			{
				Vector3 direction = (pawn.transform.position - transform.position);
				float rangeDistance = direction.magnitude;
				direction.y += 0.3f;
				direction.Normalize();
				foreach (RaycastHit hit in Physics.RaycastAll(transform.position, direction, rangeDistance))
					if (hit.transform == pawn.transform)
				{
					target = pawn;
					break;
				}
			}

		}
		return target;
    }
    protected bool DirectVisibility(out float distance)
    {
        distance = 0f;
        RaycastHit hit = new RaycastHit();
		Pawn target = SelectTarget ();
		if (target == null) {
			return false;
		}

		if (Physics.Linecast (transform.position, target.myTransform.position, out hit))
			if (hit.collider == target.collider) {
				distance = hit.distance;
				SetEnemy(target);
				return true;
			}
            else
            {
                distance = hit.distance;
                return false;
            }
        else
            return false;

    }
	protected virtual void DecideTacktick(){
	
			if(controlledPawn.naturalWeapon!=null){
				if(controlledPawn.CurWeapon==null){
					isMelee= true;
					return;
				}
				if(controlledPawn.CurWeapon.weaponRange>_distanceToTarget*2){
					isMelee= true;
				}

			
			}
			isMelee = false;
			isMelee = false;
	
	}
	protected virtual void Attack(){
		if(controlledPawn.CurWeapon!=null&&!isMelee){
			controlledPawn.StartFire();
			return;
		}
		controlledPawn.RandomKick();
	}
	protected virtual void StopAttack(){
		if(controlledPawn.CurWeapon!=null&&!isMelee){
			controlledPawn.StopFire();
		}
		controlledPawn.StopKick();
	}
	protected virtual bool IsInWeaponRange(){
	   float weaponDistance =controlledPawn.OptimalDistance(isMelee);

       return AIAgentComponent.FlatDifference(_enemy.myTransform.position, controlledPawn.myTransform.position).sqrMagnitude - _enemy.GetSize() - controlledPawn.GetSize() < weaponDistance * weaponDistance;
	}
	public virtual bool SpecificFinish(){
		return false;
	}
    
}
