using UnityEngine;
using System.Collections;
using nstuff.juggerfall.extension.models;

public class PVPGameRule : GameRule {

		protected int[] teamKill;
		
		
		public AnnonceType type;
		

		
		protected void Awake(){
            base.Awake();
			
			teamScore= new int[PlayerManager.instance.MaxTeam];
			teamKill= new int[PlayerManager.instance.MaxTeam];
		}

		void Update(){
			
            Annonce();
            base.Update();
            #if UNITY_EDITOR
                        if (Input.GetKeyDown(KeyCode.L))
                        {
                            GameEnded();
                        }
            #endif
		}

      
        public void Annonce() {
            if (teamScore[0] > teamScore[1] && type!=AnnonceType.INTERGRALEAD)
            {
                type = AnnonceType.INTERGRALEAD;
                PlayerMainGui.instance.Annonce(type);
            }
            if (teamScore[0] < teamScore[1] && type != AnnonceType.RESLEAD)
            {
                type = AnnonceType.RESLEAD;
                PlayerMainGui.instance.Annonce(type);
            }
        }
		public virtual bool IsGameEnded(){
			if(timer>gameTime){
				return true;
			}
			return false;
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
			return PlayerManager.instance.playerCount()<=1;
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
		
		
	
		public void GameEnded(){
			//PhotonNetwork.automaticallySyncScene = true;
			
			isGameEnded=true;
			//Player player = GameObject.Find ("Player").GetComponent<Player> ();
			//player.GameEnd ();
			EventHolder.instance.FireEvent(typeof(GameListener),"EventTeamWin",Winner());
            GlobalPlayer.instance.MathcEnd();
            ItemManager.instance.RemoveOldAndExpired();
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
          
            if (!isGameEnded && pvpmodel.isGameEnded)
            {
                GameEnded();
                isGameEnded = true;
            }
            
        }
}