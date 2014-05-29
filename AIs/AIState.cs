using UnityEngine;
using System.Collections;

public class AIState : MonoBehaviour {
	public  Pawn controlledPawn;

	protected Pawn _enemy;



	protected Pawn[] _pawnArray;
	
	protected float
				_distanceToTarget,
				_angleRange;

	
	public Pawn[] PawnList
	{
		set
		{
			_pawnArray = value;
			//Debug.Log(_pawnArray.Length);
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
	void Start () {
	
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
		return true;
	}
	
	public virtual void WasHitBy(Pawn killer){
		if (_enemy != null) {
			return;
		}
		SetEnemy(killer);
	}
	
	public virtual void StartState(){
	
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

}
