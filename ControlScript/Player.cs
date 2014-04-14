using UnityEngine;
using System;

public enum PawnType{PAWN,BOT};


public class Player : MonoBehaviour {
	
	public float respawnTime = 10.0f;
	
	public float robotTime = 10.0f;

	public bool isStarted = false;

	public int selected;
	
	private Pawn currentPawn;
	
	private Pawn robotPawn;
	
	public bool inBot;
	
	private GameObject ghostBot;
	
	public int team =0;

	private Pawn prefabBot;
	
	private GameObject prefabGhostBot;

	public string PlayerName="VK NAME";

	public int UID;
	
	 // Declare your serializable data.
	[System.Serializable]
	public class PlayerScore
		{
		public int Kill =0;
		public int Death=0;
		public int Assist=0;
		public int RobotKill=0;
	}
 
	private float robotTimer;
	
	private float respawnTimer;
	
	public PlayerScore Score = new PlayerScore();
	
	private Camera camera;

	private bool isDead;

	private PhotonView photonView;

	void Start(){
		photonView = GetComponent<PhotonView> ();

		if (photonView.isMine) {
		camera = Camera.main;
			((PlayerMainGui)camera.GetComponent (typeof(PlayerMainGui))).LocalPlayer = this;
			//TODO: UNCOMMENT
			robotTimer = robotTime;
		
			this.name = "Player";		
			PlayerName = "Player" + PhotonNetwork.playerList.Length;
			Application.ExternalCall( "SayMyName");
			photonView.RPC("ASKTeam",PhotonTargets.MasterClient);

		}
	}
	[RPC]
	public void ASKTeam(){

		photonView.RPC("SetTeam",PhotonTargets.All,PlayerManager.instance.NextTeam());
	}
	[RPC]
	public void SetTeam(int intTeam){
		team = intTeam;
	}
	void Update(){
		
		isDead =currentPawn==null||currentPawn.isDead;
		
		if(isDead){
		
			Pawn[] prefabClass=	PlayerManager.instance.avaiblePawn;




			respawnTimer-=Time.deltaTime;
			if(respawnTimer<=0&&isStarted){
				respawnTimer=respawnTime;
				currentPawn =PlayerManager.instance.SpawmPlayer(prefabClass[selected],team);
				AfterSpawnSetting(currentPawn,PawnType.PAWN);
				prefabBot =PlayerManager.instance.avaibleBots[selected];
				prefabGhostBot =PlayerManager.instance.ghostsBots[selected];
			}
		}else{
			Ray centerofScreen =Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit hitinfo;			
			if(robotPawn==null){
				robotTimer-=Time.deltaTime;
				
				if(robotTimer<=0){
					if(Input.GetButtonUp("SpawnBot")){
						if(ghostBot==null){
							return;
						}
						Vector3 spamPoint =ghostBot.transform.position;
						spamPoint.y+= 10;
						robotPawn =PlayerManager.instance.SpawmPlayer(prefabBot,spamPoint,ghostBot.transform.rotation);
						Debug.Log("robot spawn"+robotPawn);
						AfterSpawnSetting(robotPawn,PawnType.BOT);

						Destroy(ghostBot);				
					}
					if(Input.GetButtonDown("SpawnBot")){
						
						if(Physics.Raycast(centerofScreen, out hitinfo,50.0f)){
							ghostBot =Instantiate(prefabGhostBot,hitinfo.point,currentPawn.transform.rotation) as GameObject;
							
						}
					}
					if(Input.GetButton("SpawnBot")){
						
						if(Physics.Raycast(centerofScreen, out hitinfo,50.0f)){
							if(ghostBot==null){
								ghostBot =Instantiate(prefabGhostBot,hitinfo.point,currentPawn.transform.rotation) as GameObject;
							}
							ghostBot.transform.position = hitinfo.point;
							ghostBot.transform.rotation = currentPawn.transform.rotation;
						}
					
					}
				}
			}
			if(Physics.Raycast(centerofScreen, out hitinfo,3.0f)){
				if(inBot){
					if(Input.GetButtonDown("Use")){
						ExitBot();
					}
					
				}else 
					if(!inBot&&robotPawn!=null){
						if(hitinfo.collider.gameObject==robotPawn.gameObject){
							if(Input.GetButtonDown("Use")){
								EnterBot();
							}
						}
					}

			}
			
		}
	
	}
	
	public void RobotDead(Player Killer){
		robotTimer=robotTime;
		inBot= false;
		currentPawn.transform.parent = null;
		currentPawn.Activate ();
	}
	public void PawnDead(Player Killer){
	

		int viewID = 0;
		if(Killer!=null){
			viewID = Killer.photonView.viewID;
		}
		photonView.RPC("RPCPawnDead",PhotonTargets.All,viewID);
			

	}
	[RPC]
	public void RPCPawnDead(int viewId){
		if (!photonView.isMine) {
			return;
		}
		Score.Death++;
		if (viewId != 0) {
			Player killer = PhotonView.Find (viewId).GetComponent<Player> ();
			StatisticHandler.SendPlayerKillbyPlayer(UID, PlayerName, killer.UID, killer.PlayerName);
		} else {
			StatisticHandler.SendPlayerKillbyNPC(UID, PlayerName);
		}
		
		
		
	}
	public void PawnKill(Player Victim){
		photonView.RPC("RPCPawnKill",PhotonTargets.All);

	}
	[RPC]
	public void RPCPawnKill(){

		if (!photonView.isMine) {
			return;
		}

		if(!inBot){
			Score.Kill++;
		}else{
			Score.RobotKill++;
		}



	}
	public void PawnAssist(){
		Score.Assist++;
	}
	public void EnterBot(){
		inBot=true;
		currentPawn.DeActivate();
		currentPawn.transform.parent = robotPawn.transform;

		robotPawn.GetComponent<ThirdPersonController>().enabled = true;
		robotPawn.GetComponent<ThirdPersonCamera>().enabled = true;
		robotPawn.GetComponent<MouseLook>().enabled = true;
	}
	public void ExitBot(){
		//robotTimer=robotTime;
		inBot= false;
		currentPawn.transform.parent = null;
		currentPawn.Activate ();
		robotPawn.GetComponent<ThirdPersonController>().enabled = false;
		robotPawn.GetComponent<ThirdPersonCamera>().enabled = false;
		robotPawn.GetComponent<MouseLook>().enabled = false;
		robotPawn.StopMachine ();
	}
	public bool IsDead(){
		return isDead;

	}
	public float GetRobotTimer(){
		if (robotTimer < 0) {
			return 0;
		}
		return robotTimer;
	}
	public float GetRespawnTimer(){
		if (respawnTimer < 0) {
			return 0;
		}
		return respawnTimer;
	}
	//NetworkSection
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(PlayerName);

		}
		else
		{
			// Network player, receive data
			PlayerName= (String) stream.ReceiveNext();
		
		}
	}
	public void  SetName(String newname)
	{
		PlayerName = newname;

	}
	public void  SetUid(int uid)
	{

		UID = uid;
	}

	
	public String GetName(){
		return PlayerName;
	}	
	public Pawn GetCurrentPawn(){
		return currentPawn;
	}
	public Pawn GetRobot(){
		return robotPawn;
	}
	public void AfterSpawnSetting(Pawn pawn,PawnType type){
	
		if (photonView.isMine) {
			photonView.RPC("RPCAfterSpawnSetting",PhotonTargets.AllBuffered,pawn.GetComponent<PhotonView>().viewID,(int)type);
		}
	}


	[RPC]
	public void RPCAfterSpawnSetting(int viewid,int type){
	
		PawnType pType = (PawnType)type;
		Pawn pawn =PhotonView.Find (viewid).GetComponent<Pawn>();

		pawn.player = this;
		pawn.team = this.team;
		switch (pType) {
			case PawnType.PAWN:
			currentPawn = pawn;
			break;
			case PawnType.BOT:
			robotPawn = pawn;
			break;
		}
	}
	

}