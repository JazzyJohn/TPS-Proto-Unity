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
				//Debug.Log(Time.deltaTime+" "+_lostTimer+" "+lostTime);
				_lostTimer+=AIBase.TickPause;
				if(_lostTimer>lostTime){
					_lostTimer = 0.0f;
					Debug.Log("enemyLOST");
					LostEnemy();

				}

			}else{
				controlledPawn.StopFire();
			}
        }
    }
	
	
}
