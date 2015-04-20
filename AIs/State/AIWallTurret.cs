using UnityEngine;
using System.Collections;

public class AIWallTurret : AIState
{
	private float _lostTimer;

	public float lostTime=5.0f;

    public override void Tick()
    {
        if (DirectVisibility(out _distanceToTarget))
        {
            //code to animation attack
            if (_enemy.isDead)
            {

                LostEnemy();
                controlledPawn.StopFire();
            }
            else
            {
                if (controlledPawn.isAimimngAtEnemy())
                {
                    controlledPawn.StartFire();
                }
                else
                {
                    controlledPawn.StopFire();

                }
            }
           // Debug.Log("Shot");
        }
        else
        {
            if (_enemy != null&& _enemy.isDead)
            {
                LostEnemy();
                controlledPawn.StopFire();
            }
            else
            {
                if (_enemy != null)
                {
                    controlledPawn.StopFire();
                    //Debug.Log(Time.deltaTime+" "+_lostTimer+" "+lostTime);
                    _lostTimer += AIBase.TickPause;
                    if (_lostTimer > lostTime)
                    {
                        _lostTimer = 0.0f;
                        Debug.Log("enemyLOST");
                        LostEnemy();

                    }

                }
                else
                {

                    controlledPawn.StopFire();
                }
            }
        }
    }

    public override bool IsEnemy(Pawn target)
    {
       
        if (target.isDead)
        {
            return false;
        }
        else
        {

            return !(target.team == controlledPawn.team);
        }
            
     

    }
}
