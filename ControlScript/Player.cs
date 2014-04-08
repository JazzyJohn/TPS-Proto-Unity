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
	
	private int team =1;

	private Pawn prefabBot;
	
	private GameObject prefabGhostBot;

	public string PlayerName="VK NAME";

	public int UID;
	
	 // Declare your serializable data.
	[System.Serializable]
	public class PlayerScore
		{
		public int Kill;
		public int Death;
		public int Assist;
		public int RobotKill;
	}
 
	private float robotTimer;
	
	private float respawnTimer;
	
	private PlayerScore Score;
	
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
		}
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
	
	public void RobotDead(){
		robotTimer=robotTime;
		inBot= false;
		currentPawn.transform.parent = null;
		currentPawn.DeActivate ();
	}
	public void PawnDead(Player Killer){
	
		if (!photonView.isMine) {
			int viewID = 0;
			if(Killer!=null){
				viewID = Killer.photonView.viewID;
			}
			photonView.RPC("RPCPawnDead",PhotonTargets.Others,viewID);
			
		}
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
		photonView.RPC("RPCPawnKill",PhotonTargets.Others);

	}
	[RPC]
	public void RPCPawnKill(){
		if (!photonView.isMine) {
			return;
		}
		if(inBot){
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

		((ThirdPersonController)robotPawn.GetComponent(typeof(ThirdPersonController))).enabled = true;
		((ThirdPersonCamera)robotPawn.GetComponent(typeof(ThirdPersonCamera))).enabled = true;
		((MouseLook)robotPawn.GetComponent(typeof(MouseLook))).enabled = true;
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
	public void  SetName(String newname,int uid)
	{
		PlayerName = newname;
		UID = uid;
	}
	public String GetName(){
		return PlayerName;
	}	
	public Pawn GetCurrentPawn(){
		return currentPawn;
	}

	public void AfterSpawnSetting(Pawn pawn,PawnType type){
		pawn.player = this;
		if (photonView.isMine) {
			photonView.RPC("RPCAfterSpawnSetting",PhotonTargets.OthersBuffered,pawn.GetComponent<PhotonView>().viewID,(int)type);
		}
	}


	[RPC]
	public void RPCAfterSpawnSetting(int viewid,int type){
	
		PawnType pType = (PawnType)type;
		Pawn pawn =PhotonView.Find (viewid).GetComponent<Pawn>();

		pawn.player = this;
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