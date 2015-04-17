using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum AITriggerType { SeeEnemy, LostEnemy, SpecificFinish, TimedChange, DeadEnemy };
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
            case AITriggerType.DeadEnemy:
                trigger = new EnemyDeadTrigger();
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
            if (owner._enemy != null && !owner._enemy.isDead)
            {
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
    public class EnemyDeadTrigger : AITrigger
    {
        public bool isTriggered(AIState owner, Params[] parametrs)
        {
            if (owner._enemy == null || owner._enemy.isDead)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    }	
	public class SpecificFinish:AITrigger{
		public bool isTriggered(AIState owner, Params[] parametrs){
            if (owner.IsSpecificFinish())
            {
				return true;
			} else {
				return false;
			}		
			
		}
		
	}	
	public class TimedTrigger:AITrigger{
		
		public TimedTrigger(){
			
		}
		public bool isTriggered(AIState owner, Params[] parametrs){
          
            float delay = parametrs[0].value;
            if (delay + owner.timeStart < Time.time)
            {
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
				
	public bool targetVisible= false;

    public bool isMelee = false;

    public AIBase aibase;

    public bool IsDebug = false;

    public float timeStart;
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
	protected void Awake () {
        aibase = GetComponent<AIBase>();
        enabled = false;
	}
	
	// Update is called once per frame
	public virtual void Tick () {
	
	}

	
	public virtual void SetEnemy(Pawn enemy){
		if(_enemy != enemy){
			aibase.SetEnemy(enemy);
            AITargetManager.AddAttacker(_enemy, controlledPawn);
		}
		_enemy = enemy;
		controlledPawn.enemy = enemy;
		targetVisible = true;
	}
	
	public virtual void EnemyFromSwarm(Pawn enemy){
		if(_enemy==null||_enemy.isDead){
			_enemy = enemy;
			controlledPawn.enemy = enemy;
		}
	}
	public  virtual void AllyKill(){
	
	}
    public virtual void LostEnemy()
    {
        AITargetManager.RemoveAttacker(_enemy, controlledPawn);
		_enemy = null;
		controlledPawn.enemy = null;
		
	}
	public virtual bool IsEnemy(Pawn target){
		if(isAgressive){
			if(target.isAi){
                //Debug.Log("ENEMY? ME"+controlledPawn +"YOu" +target);
                return aibase.IsEnemy(target);
			}else{
                return aibase.IsPlayerEnemy(target);
			}
		}else{
			
			return false;
		}
		
	}
	
	public virtual void WasHitBy(Pawn killer){
		if (_enemy != null) {
			if(!targetVisible){
				SetEnemy(killer);
			}
			return;
		}
		SetEnemy(killer);
	}
	
	public virtual void StartState(){
		foreach (AITransition trans in Transition) {
			trans.Start(this);
		}
		
		_enemy = controlledPawn.enemy;
        timeStart = Time.time;
	}
	public virtual void EndState(){
	
	}
    protected virtual Pawn SelectTarget()
    {
        //select target by distance
        //but i have mind select by argo
		if (_enemy != null) {
			return _enemy;
		}
		Pawn target = null;
       
		
		foreach (Pawn pawn in _pawnArray)
		{
           
			if(!IsEnemy(pawn)){
				continue;
			}
			Vector3 myPos = controlledPawn.myTransform.position; // моя позиция
			Vector3 targetPos = pawn.myTransform.position; // позиция цели
			Vector3 myFacingNormal = controlledPawn.myTransform.forward; //направление взгляда нашей турели
			Vector3 toTarget = (targetPos - myPos);
			toTarget.Normalize();
			
			float angle = Mathf.Acos(Vector3.Dot(myFacingNormal, toTarget)) * 180 / 3.141596f;
			
			if (angle <= _angleRange / 2)
			{
				
                target = pawn;
                     
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
			targetVisible= false;
			return false;
		}

       //  Debug.Log("SELECTED TARGET" + target);
        if (Physics.Linecast(controlledPawn.myTransform.position, target.myTransform.position, out hit,controlledPawn.seenlist)){

          //  Debug.Log("SELECTED TARGET" + hit.collider);
            if (hit.collider == target.collider)
            {
                distance = hit.distance;
                SetEnemy(target);
				targetVisible =true;
                return true;
            }
            else
            {
                distance = hit.distance;
				targetVisible =false;
                return false;
            }
        }
        else
        {
            distance = (target.myTransform.position - controlledPawn.myTransform.position).magnitude;
            SetEnemy(target);
			targetVisible = true;
            return true;
        }

    }
	
	protected virtual void Attack(){
		if(controlledPawn.CurWeapon!=null&&!isMelee){
          //  Debug.Log("shoot");
			controlledPawn.StartFire();
			return;
		}
        controlledPawn.MeleeHit();
	}
	protected virtual void StopAttack(){
		if(controlledPawn.CurWeapon!=null&&!isMelee){
            //Debug.Log("stop shoot");
			controlledPawn.StopFire();
		}
		
	}
	protected virtual bool IsInWeaponRange(){
	   float weaponDistance =controlledPawn.OptimalDistance(isMelee);
       return AIAgentComponent.FlatDifference(_enemy.myTransform.position, controlledPawn.myTransform.position).sqrMagnitude - _enemy.GetSize()*_enemy.GetSize() - controlledPawn.GetSize()*controlledPawn.GetSize() < weaponDistance * weaponDistance;
	}
	public virtual bool IsSpecificFinish(){
		return false;
	}
	public virtual void KickFinish(){
	
	
	}
}
