using UnityEngine;
using System.Collections;
using nstuff.juggerfall.extension.models;

public class JuggerFight_PVPGameRule : GameRule {

        public Base[] bases;

        

        public override void StartGame()
        {
           
            base.StartGame();
            if(bases.Length==0){
                Base[] tempBases = FindObjectsOfType<Base>();
                bases = new Base[2];
                foreach (Base baseObj in tempBases)
                {
                    bases[baseObj.team - 1] = baseObj;
                }
            }
            foreach (Base baseObj in bases)
            {
                baseObj.StartBase();
            }
        }


		
		protected void Awake(){
            base.Awake();
			
			teamScore= new int[2];

		}

		void Update(){
			
            Annonce();
            base.Update();
				
		}

        public AnnonceType type;
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
            for (int i = 0; i < bases.Length; i++)
            {
                if (maxScore < bases[i].health)
                {
                    maxScore = (int)bases[i].health;
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
            PVPJuggerFightGameRuleModel pvpmodel = (PVPJuggerFightGameRuleModel)model;
        
            if (!isGameEnded && pvpmodel.isGameEnded)
            {
                GameEnded();
                isGameEnded = true;
            }
            for (int i = 0; i < pvpmodel.baseHealth.Count; i++)
            {
                bases[i].health = (int)pvpmodel.baseHealth[i];
                teamScore[i] = (int)pvpmodel.teamScore[i];
            }
        }
}