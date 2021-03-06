using UnityEngine;
using System.Collections;
using nstuff.juggerfall.extension.models;
using System.Collections.Generic;

public class PVPGameRule : GameRule {

		protected int[] teamKill;
		
		
		

		
		protected void Awake(){
            base.Awake();
			
			teamScore= new int[PlayerManager.instance.MaxTeam];
			teamKill= new int[PlayerManager.instance.MaxTeam];
		}

		void Update(){
			
            Annonce();
            base.Update();
          
		}

      
		
		public override PlayerMainGui.GameStats GetStats(){
			PlayerMainGui.GameStats  stats = new PlayerMainGui.GameStats();
			stats.gameTime = gameTime-timer;
			stats.score = teamScore;
			stats.maxScore =maxScore;
			return stats;	
		}

		string NextMap(){
			return Application.loadedLevelName;
		}
		
		public override bool IsPractice(){
            
                  return false;
            
           // return PlayerManager.instance.playerCount()<=1;
		}
		/*public override void Kill(int team){
            ActuakKillCount(team);
		}
		
		public virtual void ActuakKillCount(int team){
			teamKill[team-1]++;
		}
		//For ticket system like in Battlefield
        public override void Spawn(int team)
        {
			ActuakSpawnCount(team);
		}
		
		public virtual void ActuakSpawnCount(int team){
			

		}
		*/
	
		public override int Winner(){
			int maxScore = 0;
			int winner = 0;
			for(int i=0;i<teamScore.Length;i++){
				if(maxScore<teamScore[i]){
					maxScore=teamScore[i];
					winner = i;
				}
			}
			return (winner+1);
		}
		
		
	
	

		public bool FinalStage (){
			for(int i=0;i<teamScore.Length;i++){
				if(maxScore<teamScore[i]+5){
					return true;
				}
			}
			return false;
		}
        public override void SetFromModel(GameRuleModel model)
        {
            PVPGameRuleModel pvpmodel = (PVPGameRuleModel)model;
            for (int i = 0; i < pvpmodel.teamKill.Count; i++)
            {
                teamKill[i] = (int)pvpmodel.teamKill[i];
                teamScore[i] = (int)pvpmodel.teamScore[i];
            }
            Debug.Log(timer);
            timer = pvpmodel.time;
          
            if (!isGameEnded && pvpmodel.isGameEnded)
            {
                GameEnded();
                isGameEnded = true;
            }
            
        }
}