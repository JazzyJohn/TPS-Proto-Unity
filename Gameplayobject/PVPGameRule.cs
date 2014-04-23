using UnityEngine;
using System.Collections;

public class PVPGameRule : MonoBehaviour {

		private int[] teamKill;
		
		private int[] teamScore;
		
		private float timer;
		
		private int maxScore;
		
		public float gameTime;
		
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


		
		void Awake(){
			
			teamScore= new Int(PlayerManager.instance.MaxTeam);
		}

		void Update(){
			if(IsGameEnded()){
				PhotonNetwork.LoadLevel(NextMap());
			
			}	
			timer+= Time.deltaTime;			
		}
		virtual bool IsGameEnded(){
			if(timer>gameTime){
				return true;
			}
			return false;
		}
		virtual PlayerMainGui.GameStats GetStats(){
			PlayerMainGui.GameStats  stats = new PlayerMainGui.GameStats();
			stats.gameTime = gameTime-timer;
			stats.score = teamScore;
			stats.maxScore =maxScore;
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
		virtual void ActuakKillCount(int team){
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
		virtual void ActuakSpawnCount(int team){
			
		}
		
		
}