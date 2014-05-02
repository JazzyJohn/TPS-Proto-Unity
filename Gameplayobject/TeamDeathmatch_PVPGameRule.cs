using UnityEngine;
using System.Collections;

public class TeamDeathmatch_PVPGameRule : PVPGameRule {

		public int killMax;
		
		public override bool IsGameEnded(){
			for(int i=0;i<teamKill.Length;i++){
				if(killMax<=teamKill[i]){
					return true;
				}
			}
			return base.IsGameEnded();
		}
		void Awake(){
			
			base.Awake();
			maxScore=killMax;
		}
		public override void ActuakKillCount(int team){
		Debug.Log ("KILL COUNT" + team);
			teamKill[team-1]++;
			teamScore[team-1]++;
		}
		

}