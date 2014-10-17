using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AITeamBotBase : AIBase
{
		public override bool IsEnemy(Pawn target)
		{
			if(target.team!=controlledPawn.team){
				return true;
			}else{
				return base.IsEnemy(target);
			}
		}
   
   public virtual bool IsPlayerEnemy(Pawn target)
   {
        if(target.team==controlledPawn.team){
				return false;
		}else{
			return base.IsPlayerEnemy(target);
		}
   }
  
   public virtual void EnemyFromSwarm(Pawn enemy){
		if(enemy.team!=controlledPawn.team){
			_currentState.EnemyFromSwarm(enemy);
		}
   }

}