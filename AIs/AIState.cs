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

	public virtual void WasHitBy(Pawn killer){

	}
	public void SetEnemy(Pawn enemy){
		_enemy = enemy;
		controlledPawn.enemy = enemy;

	}
	public void LostEnemy(){
		_enemy = null;
		controlledPawn.enemy = null;

	}

}
