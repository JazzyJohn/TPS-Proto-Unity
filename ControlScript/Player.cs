using UnityEngine;
using System;

public enum PawnType{PAWN,BOT};


public class Player : MonoBehaviour {
	
	public float respawnTime = 10.0f;
	
	public float robotTime = 2.0f;

	public bool isStarted = false;

	public int selected;
	
	private Pawn currentPawn;
	
	private Pawn robotPawn;
	
	public bool inBot;
	
	private GhostObject ghostBot;
	
	private bool canSpawnBot=false;
	
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
			photonView.RPC("ASKTeam",PhotonTargets.MasterClient);
			Application.ExternalCall( "SayMyName");
		
			//StatisticHandler.StartStats(UID,PlayerName);
		}
	}
	[RPC]
	public void ASKTeam(){

		photonView.RPC("SetTeam",PhotonTargets.AllBuffered,PlayerManager.instance.NextTeam());
	}
	[RPC]
	public void SetTeam(int intTeam){
		Debug.Log ("setTeam" + intTeam);
		team = intTeam;	
	}
	void Update(){
		if (!photonView.isMine) {
			return;
		}
		isDead =currentPawn==null||currentPawn.isDead;
		
		if(isDead){
		
			Pawn[] prefabClass=	PlayerManager.instance.avaiblePawn;




			respawnTimer-=Time.deltaTime;
			if(respawnTimer<=0&&isStarted){
				respawnTimer=respawnTime;
				currentPawn =PlayerManager.instance.SpawmPlayer(prefabClass[selected],team);
				PVPGameRule.instance.Kill(team);
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
						if(ghostBot!=null&&canSpawnBot){
							Vector3 spamPoint =ghostBot.transform.position;
							spamPoint.y+= 10;
							robotPawn =PlayerManager.instance.SpawmPlayer(prefabBot,spamPoint,ghostBot.transform.rotation);
							Debug.Log("robot spawn"+robotPawn);
							AfterSpawnSetting(robotPawn,PawnType.BOT);

							Destroy(ghostBot);	
							canSpawnBot=false;							
						}
						
					}
					if(Input.GetButton("SpawnBot")){
						
						if(Physics.Raycast(centerofScreen, out hitinfo,50.0f)){
							if(ghostBot==null){
									GameObject ghostGameObj = Instantiate(prefabGhostBot,hitinfo.point,currentPawn.transform.rotation) as GameObject;
									ghostBot =ghostGameObj.geComponent<GhostObject>();
							}
							ghostBot.myTransform.position = hitinfo.point;
							ghostBot.myTransform.rotation = currentPawn.transform.rotation;
						
							if(Physics.Raycast(hitinfo.point,Vector3.up,100.0f)){
								//ghostBot =Instantiate(prefabGhostBot,hitinfo.point,currentPawn.transform.rotation) as GameObject;
								if(canSpawnBot){
									ghostBot.renderer.material.color = ghostBot.badColor;
								}
								canSpawnBot=false;
							}else{
								if(!canSpawnBot){
									ghostBot.renderer.material.color = ghostBot.normalColor;
								}
								canSpawnBot=true;
							}
						}
					
					}
				}
			}

				if(inBot){
					if(Input.GetButtonDown("Use")){
						ExitBot();
					}
					
				}else {
					
						if(!inBot&&robotPawn!=null){
							if(currentPawn.curLookTarget.gameObject==robotPawn.gameObject){
								if(Input.GetButtonDown("Use")){
									EnterBot();
								}
							}
						}

				}
			if(Input.GetButtonDown("Fire2")){
				currentPawn.ToggleAim();
				if(robotPawn!=null){
					robotPawn.ToggleAim();
				}
			}
			if(Input.GetButtonUp("Fire2")){
				currentPawn.ToggleAim();
				if(robotPawn!=null){
					robotPawn.ToggleAim();
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
			PVPGameRule.instance.Kill(Killer.team);
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
	public void PawnKill(Player Victim,Vector3 position){
		photonView.RPC("RPCPawnKill",PhotonTargets.All,position);

	}
	[RPC]
	public void RPCPawnKill(Vector3 position){

		if (!photonView.isMine) {
			return;
		}
		//TODO: move text to config
		PlayerMainGui.instance.AddMessage("NAILED IT",position,PlayerMainGui.MessageType.KILL_TEXT);

		if(!inBot){
			Score.Kill++;
		}else{
			Score.RobotKill++;
		}



	}
	
	public void DamagePawn(float damage, Vector3 position){
		PlayerMainGui.instance.AddMessage(damage.toString(),position,PlayerMainGui.MessageType.DMG_TEXT);
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
		robotPawn.GetComponent<ThirdPersonCamera>().Reset ();

	}
	public void ExitBot(){
		//robotTimer=robotTime;
		inBot= false;
		currentPawn.transform.parent = null;
		currentPawn.Activate ();
		robotPawn.GetComponent<ThirdPersonController>().enabled = false;
		robotPawn.GetComponent<ThirdPersonCamera>().enabled = false;

		robotPawn.StopMachine ();
	}
	public bool IsDead(){
		return isDead;

	}
	public PlayerMainGui.PlayerStats GetPlayerStats(){
		PlayerMainGui.PlayerStats stats = new PlayerMainGui.PlayerStats ();
		stats.robotTime = GetRobotTimer();
		Pawn curPawn = null;
		if (inBot) {
			curPawn = robotPawn;
		} else {

			curPawn= currentPawn;

		}
		stats.health = curPawn.health;
		stats.ammoInGun = curPawn.CurWeapon.curAmmo;
		stats.ammoInGunMax = curPawn.CurWeapon.clipSize;
		stats.ammoInBag = curPawn.GetAmmoInBag ();
		
		return stats;

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
			stream.SendNext(UID);

		}
		else
		{
			// Network player, receive data
			PlayerName= (String) stream.ReceiveNext();
			UID= (int) stream.ReceiveNext();
		}
	}
	public void  SetName(String newname)
	{
		PlayerName = newname;
	
		StatisticHandler.StartStats(UID,PlayerName);
		
	}
	public void  SetUid(int uid)
	{

		UID = uid;
		
	}
	public void  SendData()
	{
		StatisticHandler.StartStats(UID,PlayerName);
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
		Debug.Log (viewid);
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