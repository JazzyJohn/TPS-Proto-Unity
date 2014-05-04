using UnityEngine;
using System.Collections;

public class AITurret : AIState
{
	private float _lostTimer;

	public float lostTime=5.0f;

    public override void Tick()
    {
        if (DirectVisibility(out _distanceToTarget))
        {
            //code to animation attack
			if(controlledPawn.isAimimngAtEnemy()){
				controlledPawn.StartFire();
			}else{
				controlledPawn.StopFire();

			}
           // Debug.Log("Shot");
        }
        else
        {
			if(_enemy!= null){
				controlledPawn.StopFire();
				_lostTimer+=Time.deltaTime;
				if(_lostTimer>lostTime){
					_lostTimer = 0.0f;
					LostEnemy();

				}

			}else{
				controlledPawn.StopFire();
			}
        }
    }
	public override void WasHitBy(Pawn killer){
		if (_enemy != null) {
			return;
		}
		SetEnemy(killer);
	}
    private Pawn SelectTarget()
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
    private bool DirectVisibility(out float distance)
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
