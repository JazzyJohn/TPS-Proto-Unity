using UnityEngine;
using System.Collections;

public class PVPGameRule : GameRule {

		protected int[] teamKill;
		
		protected int[] teamScore;
		
		

		
		protected void Awake(){
            base.Awake();
			
			teamScore= new int[PlayerManager.instance.MaxTeam];
			teamKill= new int[PlayerManager.instance.MaxTeam];
		}

		void Update(){
			if (FinalStage ()&&curStage==0) {
				curStage=1;
			}
			if (isGameEnded) {

				restartTimer+= Time.deltaTime;				
				if(restartTimer>restartTime&&!lvlChanging){
					lvlChanging= true;
					FindObjectOfType<ServerHolder>().LoadNextMap();
				}
			}else	if (!PhotonNetwork.isMasterClient) {
				return;
			}else if(IsGameEnded()){
				IsLvlChanging= true;
				isGameEnded=true;
				photonView.RPC("GameEnded",PhotonTargets.All);
					//PhotonNetwork.automaticallySyncScene = true;
					//PhotonNetwork.LoadLevel(NextMap());
			
			}
            Annonce();
			timer+= Time.deltaTime;			
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
		
		public override void Kill(int team){
			photonView.RPC("RPCKill",PhotonTargets.MasterClient,team);
		}
		[RPC]
		public  void RPCKill(int team){
			//0 for neutral creature so lower teamIndex by 1
			ActuakKillCount(team);
		}
		public virtual void ActuakKillCount(int team){
			teamKill[team-1]++;
		}
		//For ticket system like in Battlefield
        public override void Spawn(int team)
        {
			photonView.RPC("RPCSpawn",PhotonTargets.MasterClient,team);
		}
		[RPC]
		public  void RPCSpawn(int team){
			ActuakSpawnCount(team);
		}
		public virtual void ActuakSpawnCount(int team){
			

		}
		
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
				if (stream.isWriting) {
					for(int i=0;i<teamScore.Length;i++){
						stream.SendNext(teamScore[i]);
					}
				} else {
					for(int i=0;i<teamScore.Length;i++){
					teamScore[i] =(int) stream.ReceiveNext();
					}


				}

		}
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
		
		
		[RPC]
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
}