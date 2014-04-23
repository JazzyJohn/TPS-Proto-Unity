using UnityEngine;
using System.Collections;

public class TeamDeathmatch_PVPGameRule : MonoBehaviour {

		public int killMax;
		
		virtual bool IsGameEnded(){
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
		override void ActuakKillCount(int team){
			teamKill[team-1]++;
			teamScore[team-1]++;
		}
		

}