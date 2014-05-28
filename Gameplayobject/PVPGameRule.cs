using UnityEngine;
using System.Collections;

public class PVPGameRule : GameRule {

		protected int[] teamKill;
		
		protected int[] teamScore;
		
		protected float timer=0.0f;

		protected float restartTimer=0.0f;
		
		protected int maxScore;
		
		public float gameTime;

		public float restartTime;

		public static bool isGameEnded = false;

		public PhotonView photonView;
		
		public int curStage=0;
		 // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
		private static PVPGameRule s_Instance = null;
	 
		// This defines a static instance property that attempts to find the manager object in the scene and
		// returns it to the caller.
		public static PVPGameRule instance {
			get {
				if (s_Instance == null) {
					// This is where the magic happens.
					//  FindObjectOfType(...) returns the first AManager object in the scene.
					s_Instance =  FindObjectOfType(typeof (PVPGameRule)) as PVPGameRule;
				}
	 
				return s_Instance;
			}
		}	


		
		protected void Awake(){
			isGameEnded = false;
			PhotonNetwork.isMessageQueueRunning = true;
			photonView = GetComponent<PhotonView>();
			teamScore= new int[PlayerManager.instance.MaxTeam];
			teamKill= new int[PlayerManager.instance.MaxTeam];
		}

		void Update(){
			if (FinalStage ()&&curStage==0) {
				curStage=1;
			}
			if (isGameEnded) {
				restartTimer+= Time.deltaTime;				
				if(restartTimer>restartTime){
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
			timer+= Time.deltaTime;			
		}
		public virtual bool IsGameEnded(){
			if(timer>gameTime){
				return true;
			}
			return false;
		}
		public virtual PlayerMainGui.GameStats GetStats(){
			PlayerMainGui.GameStats  stats = new PlayerMainGui.GameStats();
			stats.gameTime = gameTime-timer;
			stats.score = teamScore;
			stats.maxScore =maxScore;
			return stats;	
		}

		string NextMap(){
			return Application.loadedLevelName;
		}
		
		public void Kill(int team){
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
		public void Spawn(int team){
			photonView.RPC("RPCSpawn",PhotonTargets.MasterClient,team);
		}
		[RPC]
		public  void RPCSpawn(int team){
			ActuakSpawnCount(team);
		}
		public virtual void ActuakSpawnCount(int team){
			

		}
		public float GetRestartTimer(){
			return restartTime- restartTimer;

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
		public int Winner(){
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
		public void StartGame(){
			IsLvlChanging = false;
			FindObjectOfType<AIDirector> ().StartDirector ();
		}
		
		[RPC]
		public void GameEnded(){
			PhotonNetwork.automaticallySyncScene = true;
			
			isGameEnded=true;
			Player player = GameObject.Find ("Player").GetComponent<Player> ();
			player.GameEnd ();
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