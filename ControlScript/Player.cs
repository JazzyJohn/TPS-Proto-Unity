using UnityEngine;
using System;
using System.Collections.Generic;

public enum PawnType{PAWN,BOT};

//BIT MASK 
public enum GameClassEnum{ENGINEER,ASSAULT,SCOUT,MEDIC,ANY,ROBOTHEAVY,ROBOTMEDIUM,ROBOTLIGHT,ANYROBOT};

public class Player : MonoBehaviour {
	public List<string> friendsInfo = new List<string>();

	public float respawnTime = 10.0f;
	
	public float robotTime = 60.0f;
	
	public float robotKillReduce  =5.0f;

	public bool isStarted = false;

	public int selected;

	public int selectedBot;
	
	private Pawn currentPawn;
	
	private RobotPawn robotPawn;
	
	public bool inBot;
	
	private GhostObject ghostBot;
	
	private bool canSpawnBot=false;
	
	public int team =0;

	private RobotPawn prefabBot;
	
	private GameObject prefabGhostBot;

	public string PlayerName="VK NAME";

	public string UID;

	public Texture2D vkAvavtar;
	
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
	
	private Camera myCamera;

	private bool isDead=true;

	private bool canSpamBot = true;

	private PhotonView photonView;

	public UseObject useTarget;
	//Func name for delayed external call
	public string delayedExternalCallName;
	//param for delayed external call
	public string delayedExternalCallData;
	
	public const float SQUERED_RADIUS_OF_ACTION = 16.0f;
	
	public GlobalPlayer globalPlayer;



	public bool isPlayerFriend(string playerId)
	{
	 	foreach (string id in friendsInfo) 
		{
			if(id.Equals(playerId))
				return true;
		}

		return false;
	}
	
	void Start(){
		photonView = GetComponent<PhotonView> ();
		PlayerManager.instance.addPlayer(this);
		if (photonView.isMine) {
						myCamera = Camera.main;
						((PlayerMainGui)myCamera.GetComponent (typeof(PlayerMainGui))).SetLocalPlayer(this);
						robotTimer = robotTime;
		
						//this.name = "Player";		
						PlayerName = "Player" + PhotonNetwork.playerList.Length;
						//	photonView.RPC ("ASKTeam", PhotonTargets.MasterClient);
						globalPlayer =  FindObjectOfType<GlobalPlayer>();
						UID = globalPlayer.GetUID();
						PlayerName = globalPlayer.GetPlayerName();
						//vkAvavtar= globalPlayer.GetPlayerAvatar();
						friendsInfo = globalPlayer.friendsInfo;
						photonView.RPC("RPCSetNameUID",PhotonTargets.AllBuffered,UID,PlayerName);
						EventHolder.instance.FireEvent(typeof(LocalPlayerListener),"EventAppear",this);
						//StatisticHandler.StartStats(UID,PlayerName);
		} else {
			Destroy(GetComponent<MusicHolder>());
			Destroy(GetComponent<AudioSource>());	
		}
	}
	[RPC]
	public void ASKTeam(){
		Debug.Log ("ASKTeam" + this);
		photonView.RPC("RPCSetTeam",PhotonTargets.AllBuffered,PlayerManager.instance.NextTeam());
	}
	public void SetTeam(int intTeam){
		team = intTeam;	
	}
	[RPC]
	public void RPCSetTeam(int intTeam){
		//Debug.Log ("setTeam" + intTeam);
		team = intTeam;	
	}

	public PhotonView GetView(){

		return photonView;
	}
	public void GameEnd(){
		if (currentPawn != null) {
			currentPawn.RequestKillMe ();
		}
		if(robotPawn!= null) {
			robotPawn.RequestKillMe ();
		}
	}
	public void Respawn(Pawn newPawn){
		if (!inBot&&photonView.isMine) {
			currentPawn.RequestKillMe();
			currentPawn  =PlayerManager.instance.SpawmPlayer(newPawn,currentPawn.myTransform.position,currentPawn.myTransform.rotation);
			canSpamBot=false;
			AfterSpawnSetting(currentPawn,PawnType.PAWN,team);
		}

	}
	void Update(){
		if (!photonView.isMine) {
			return;
		}
		isDead =currentPawn==null||currentPawn.isDead;
		
		if(isDead){
		
			SendDelayedExternal();
			respawnTimer-=Time.deltaTime;
//			Debug.Log ("Dead");
			if(respawnTimer<=0&&isStarted){
				respawnTimer=respawnTime;
				currentPawn =PlayerManager.instance.SpawmPlayer(PlayerManager.instance.pawnName[selected],team);
				currentPawn.ChangeDefaultWeapon(Choice._Player);
				ItemManager.instance.SaveItemForSlot();
				PVPGameRule.instance.Spawn(team);
				AfterSpawnSetting(currentPawn,PawnType.PAWN,team);
				prefabBot =PlayerManager.instance.avaibleBots[selectedBot];
				prefabGhostBot =PlayerManager.instance.ghostsBots[selectedBot];

			}
			canSpamBot=true;
		}else{
			Ray centerofScreen =Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit hitinfo;			
			if(robotPawn==null){
				robotTimer-=Time.deltaTime;
				
				if(robotTimer<=0&&canSpamBot){
					if(Input.GetButton("SpawnBot")){
						
						if(Physics.Raycast(centerofScreen, out hitinfo,50.0f)){
							if(ghostBot==null){
								GameObject ghostGameObj = Instantiate(prefabGhostBot,hitinfo.point,currentPawn.transform.rotation) as GameObject;
								ghostBot =ghostGameObj.GetComponent<GhostObject>();
							}
							ghostBot.myTransform.position = hitinfo.point;
							ghostBot.myTransform.rotation= Quaternion.LookRotation(-currentPawn.transform.forward);
							
							if(Physics.SphereCast(hitinfo.point+Vector3.up*ghostBot.size,ghostBot.size,Vector3.up,out hitinfo,100.0f)){
								//Debug.Log (hitinfo.collider);
								if(canSpawnBot){
									ghostBot.MakeBad();
									//Debug.Log (ghostBot.myRenderer.sharedMaterial.color);
								}
								canSpawnBot=false;
							}else{
								if(!canSpawnBot){
									
									ghostBot.MakeNormal();
									//Debug.Log (ghostBot.myRenderer.sharedMaterial.color);
								}
								canSpawnBot=true;
							}
						}
						
					}
					if(Input.GetButtonUp("SpawnBot")){
						if(ghostBot!=null&&canSpawnBot){
							Vector3 spamPoint =ghostBot.transform.position;
							spamPoint.y+= 30;
							robotPawn =(RobotPawn)PlayerManager.instance.SpawmPlayer(prefabBot,spamPoint,ghostBot.transform.rotation);
							robotPawn.ChangeDefaultWeapon(selectedBot);
							//Debug.Log("robot spawn"+robotPawn);
							AfterSpawnSetting(robotPawn,PawnType.BOT,team);

						
							canSpawnBot=false;							
						}
						//Debug.Log("destory chost");
						Destroy(ghostBot.gameObject);	
					}

				}
			}
				
				if(inBot){
					useTarget= null;
					if(Input.GetButtonDown("Use")){
						ExitBot();
					}
					
				}else {
					
						if(!inBot&&robotPawn!=null){
                            //Debug.Log(currentPawn.curLookTarget.gameObject +" "+ (currentPawn.myTransform.position - robotPawn.myTransform.position).sqrMagnitude);
							if(currentPawn.curLookTarget!=null&&currentPawn.curLookTarget.gameObject==robotPawn.gameObject&&(currentPawn.myTransform.position-robotPawn.myTransform.position).sqrMagnitude<SQUERED_RADIUS_OF_ACTION*2.0f){
								if(Input.GetButtonDown("Use")){
									EnterBot();
								}
							}
						}

					if(currentPawn.curLookTarget!=null){

						useTarget = currentPawn.curLookTarget.GetComponent<UseObject>();
                     
					if(useTarget!=null&&(currentPawn.myTransform.position-useTarget.myTransform.position).sqrMagnitude<SQUERED_RADIUS_OF_ACTION&&Input.GetButtonDown("Use")){
							useTarget.Use(currentPawn);

						}
					}else{
						useTarget= null;
					}
					//Debug.Log (currentPawn.curLookTarget);

				}
			if(Input.GetButton("Fire2")){
				currentPawn.ToggleAim(true);
				if(robotPawn!=null){
					robotPawn.ToggleAim(true);
				}
			}else{
				currentPawn.ToggleAim(false);
				if(robotPawn!=null){
					robotPawn.ToggleAim(false);
				}
			}
			if(Input.GetButtonDown("Weapon1")){
			
				if(inBot&&robotPawn!=null){
					robotPawn.ChangeWeapon (0);
				}else{
					currentPawn.ChangeWeapon (0);
				}
			}
			if(Input.GetButtonDown("Weapon2")){
				
				if(inBot&&robotPawn!=null){
					robotPawn.ChangeWeapon (1);
				}else{
					currentPawn.ChangeWeapon (1);
				}
			}
			if(Input.GetButtonDown("Weapon3")){
				
				if(inBot&&robotPawn!=null){
					robotPawn.ChangeWeapon (2);
				}else{
					currentPawn.ChangeWeapon (2);
				}
			}
			if(Input.GetButtonDown("Suicide")){
				currentPawn.RequestKillMe();
				if(robotPawn!=null){
					robotPawn.RequestKillMe();
				}
				Score.Death++;
				isStarted = false;
				isDead = true;
				StatisticHandler.SendPlayerKillbyNPC(UID, PlayerName);
			}
		}
	
	}
	
	public void RobotDead(Player Killer){
		robotTimer=robotTime;
		if (inBot) {
				inBot = false;
				currentPawn.Activate ();
				currentPawn.rigidbody.MovePosition (robotPawn.playerExitPositon.position);
				currentPawn.myTransform.rotation = robotPawn.playerExitPositon.rotation;
				currentPawn.transform.parent = null;
		}
	
	}
	public void PawnDead(Player Killer,Pawn killerPawn ){
	

		int viewID = 0,pawnViewId =0;
		if (Killer != null) {
			viewID = Killer.photonView.viewID;
			
			PVPGameRule.instance.Kill (Killer.team);
			EventHolder.instance.FireEvent(typeof(LocalPlayerListener),"EventPawnDeadByPlayer",this);
		} else {
			EventHolder.instance.FireEvent(typeof(LocalPlayerListener),"EventPawnDeadByAI",this);
		}
		photonView.RPC("RPCPawnDead",photonView.owner,viewID,pawnViewId);
			

	}
	[RPC]
	public void RPCPawnDead(int viewId,int pawnViewId){
		
		Score.Death++;
		isStarted = false;
		isDead = true;
		if (viewId != 0) {
			Player killer = PhotonView.Find (viewId).GetComponent<Player> ();
			PlayerMainGui.instance.InitKillCam(killer);
			if(isPlayerFriend(killer.UID))
			{
				EventHolder.instance.FireEvent(typeof(LocalPlayerListener),"EventKilledByFriend",this,killer);
			}
		
			StatisticHandler.SendPlayerKillbyPlayer(UID, PlayerName, killer.UID, killer.PlayerName);
		} else {
			Pawn killer = PhotonView.Find (pawnViewId).GetComponent<Pawn> ();
			PlayerMainGui.instance.InitKillCam(killer);
			StatisticHandler.SendPlayerKillbyNPC(UID, PlayerName);
		}
		
		
		
	}
	public void PawnKill(Player Victim,Vector3 position){

		if (Victim != null) {
			photonView.RPC ("RPCPawnKill", photonView.owner, position,Victim.photonView.viewID);

		} else {
			photonView.RPC ("RPCAIKill", photonView.owner, position);
		}


	}
	[RPC]
	public void RPCPawnKill(Vector3 position,int viewId){

		//TODO: move text to config
		PlayerMainGui.instance.AddMessage("NAILED IT",position,PlayerMainGui.MessageType.KILL_TEXT);
		EventHolder.instance.FireEvent(typeof(LocalPlayerListener),"EventPawnKillPlayer",this);
		Player victim = PhotonView.Find (viewId).GetComponent<Player> ();

		if(isPlayerFriend(victim.UID))
		{
			EventHolder.instance.FireEvent(typeof(LocalPlayerListener),"EventKilledAFriend",this,victim);
		}

		if(!inBot){
			Score.Kill++;
			robotTimer-=robotKillReduce;
		}else{
			Score.Kill++;
			Score.RobotKill++;
		}



	}
	[RPC]
	public void RPCAIKill(Vector3 position){
		
		//TODO: move text to config
		PlayerMainGui.instance.AddMessage("NAILED IT",position,PlayerMainGui.MessageType.KILL_TEXT);
		EventHolder.instance.FireEvent(typeof(LocalPlayerListener),"EventPawnKillAI",this);

		
		
		
	}
	//Delayed external for that function that can destrupt user like VK wallpost
	public void SendDelayedExternal(){
		if(delayedExternalCallName!=""){
			Application.ExternalCall(delayedExternalCallName,delayedExternalCallData);
			delayedExternalCallName="";
		}
	}
		
	public void AchivmenUnlock(Achievement achv){
		PlayerMainGui.instance.AddMessage(achv.name+"\n"+achv.description,Vector3.zero,PlayerMainGui.MessageType.ACHIVEMENT);
		delayedExternalCallName ="AchivmenUnlock";
		delayedExternalCallData = achv.name + " " + achv.description;
		
	}
	
	public void DamagePawn(BaseDamage damage){
		if (!photonView.isMine) {
			return;
		}
		if (damage.sendMessage) {
			if(damage.isContinius){
				PlayerMainGui.instance.AddMessage ((damage.Damage/Time.deltaTime).ToString ("0.0"), damage.hitPosition, PlayerMainGui.MessageType.DMG_TEXT);
			}else{
				PlayerMainGui.instance.AddMessage (damage.Damage.ToString (), damage.hitPosition, PlayerMainGui.MessageType.DMG_TEXT);
			}
		}
	}
	public void PawnAssist(){
		Score.Assist++;
	}
	public void EnterBot(){
		inBot=true;
		currentPawn.DeActivate();
		currentPawn.transform.parent = robotPawn.transform;
		robotPawn.Activate ();

	

	}
	public void ExitBot(){
		//robotTimer=robotTime;
		inBot= false;
		currentPawn.myTransform.parent = null;
		currentPawn.myTransform.position = robotPawn.playerExitPositon.position;
		currentPawn.myTransform.rotation = robotPawn.playerExitPositon.rotation;
		currentPawn.Activate ();
		robotPawn.DeActivate();
		
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
		if (curPawn != null) {
			stats.health = curPawn.health;
			if(curPawn.CurWeapon!=null){
			
				stats.gun  = curPawn.CurWeapon;
				stats.ammoInBag = curPawn.GetAmmoInBag ();
				stats.reloadTime = curPawn.CurWeapon.ReloadTimer();
                stats.pumpCoef = curPawn.CurWeapon.PumpCoef();
				
			}
			stats.jetPackCharge  = curPawn.GetJetPackCharges();
		}

		
		return stats;

	}
	public string IsMaster(){
		if (photonView.owner.isMasterClient) {
			return "Host";		
		}
		return "";
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
			stream.SendNext(inBot);
			stream.SendNext(Score.Kill);
			stream.SendNext(Score.Death);
			stream.SendNext(Score.Assist);
			stream.SendNext(Score.RobotKill);
		}
		else
		{
			// Network player, receive data
			inBot= (bool) stream.ReceiveNext();
			Score.Kill = (int)  stream.ReceiveNext();
			Score.Death = (int)  stream.ReceiveNext();
			Score.Assist = (int)  stream.ReceiveNext();
			Score.RobotKill = (int)  stream.ReceiveNext();
		}
	}
	
	
	public String GetName(){
		return PlayerName;
	}	
	public String GetUid(){
		return UID;
	}
	public Pawn GetCurrentPawn(){
		return currentPawn;
	}
	public Pawn GetRobot(){
		return robotPawn;
	}

	public Pawn GetActivePawn(){
		if(robotPawn!=null &&!currentPawn.isActive){
			return robotPawn;
		}else{
			return currentPawn;
		}
		return null;
	}
	public void AfterSpawnSetting(Pawn pawn,PawnType type,int rTeam){
	
		if (photonView.isMine) {
			//Debug.Log ("SEND");
			photonView.RPC("RPCAfterSpawnSetting",PhotonTargets.AllBuffered,pawn.GetComponent<PhotonView>().viewID,(int)type,rTeam);
		}
	}


	[RPC]
	public void RPCAfterSpawnSetting(int viewid,int type,int iteam){
	
		PawnType pType = (PawnType)type;
		//Debug.Log (viewid);
		PhotonView view = PhotonView.Find (viewid);
		if (view == null) {
			return;
		}
		Pawn pawn =PhotonView.Find (viewid).GetComponent<Pawn>();

		team = iteam;	
		pawn.player = this;
		pawn.team = this.team;
		switch (pType) {
			case PawnType.PAWN:
			currentPawn = pawn;
			break;
			case PawnType.BOT:
			robotPawn = (RobotPawn)pawn;
			break;
		}
	}
	[RPC]
	public void RPCSetNameUID(string rUID,String rPlayerName){
		UID=rUID;
		PlayerName=rPlayerName;
	}

}