using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AITeamBotBase : AIBase
{
		public override bool IsEnemy(Pawn target)
		{

           // Debug.Log("ENEMY? ME" + controlledPawn + " : "+controlledPawn.team+" YOu" + target+" : "+target.team );
			if(target.team!=controlledPawn.team){
				return true;
			}else{
				return base.IsEnemy(target);
			}
		}

        public override bool IsPlayerEnemy(Pawn target)
   {
        if(target.team==controlledPawn.team){
				return false;
		}else{
			return base.IsPlayerEnemy(target);
		}
   }

        public override void EnemyFromSwarm(Pawn enemy)
        {
		if(enemy.team!=controlledPawn.team){
			_currentState.EnemyFromSwarm(enemy);
		}
   }

}