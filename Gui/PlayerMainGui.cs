	using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerMainGui : MonoBehaviour {
	public Texture crosshair;

	public float crosshairHeight;

	public float crosshairWidth;

	public Player LocalPlayer;

	public Camera MainCamera;

	public float MarkSize;

	public Texture EnemyMark;
	public GUIStyle AliaMark;

	public float VersionSize;
	
	public Texture  VersionMark;

	public enum MessageType{
			STD_MESSAGE=0,
			DMG_TEXT=1,
			KILL_TEXT = 2
	}
	public Texture[] messageTexture;
	public float[] messageSize;
	public float messageDelay=1.0f;
	public class GUIMessage{
		public float destroyTime;
		public Vector3 worldPoint;
		public string text="";
		public MessageType type;
		public float getMessageSize(PlayerMainGui outer){
			if(outer.messageSize.Length<=(int)type){
				return outer.messageSize[0];
			}
			return outer.messageSize[(int)type];
		}
		public Texture getTexture(PlayerMainGui outer){
			if(outer.messageTexture.Length<=(int)type){
				return outer.messageTexture[0];
			}
			return outer.messageTexture[(int)type];
		}
		
	}
	
	 // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
	private static PlayerMainGui s_Instance = null;
	 
	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	public static PlayerMainGui instance {
		get {
			if (s_Instance == null) {
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (PlayerMainGui)) as PlayerMainGui;
			}
 
			return s_Instance;
		}
	}	

	private Queue<GUIMessage> guiMessages = new Queue<GUIMessage>();
	class LogMess{
		public string mess;
		public string trace;
	}
	private Queue<LogMess> logMessages = new Queue<LogMess>();
	public bool showDebug = false;
	public class PlayerStats{
		public float robotTime=0;
		public float health=0;
		public float ammoInGun=0;
		public float ammoInGunMax=0;
		public float ammoInBag=0;
		public float reloadTime=0;
		public int jetPackCharge= 0;
		public string gunName="";
			
	}
	public class GameStats{
		public float gameTime;
		public int[] score;
		public int maxScore;
	}
	enum GUIState{
		Normal,
		Playerlist,
		Dedicated,
			
	}
	private GUIState guiState;

	public GUISkin guiSkin;

	public GUISkin messageSkin;

	private MenuTF respawnMenu;

	private ChatHolder[] chats;

	public static bool IsMouseAV;
	// Use this for initialization
	void Start () {
		MainCamera = Camera.main;
		respawnMenu = GetComponent<MenuTF>();
	}

	public void SetLocalPlayer(Player newPlayer){
		LocalPlayer = newPlayer;
		chats = FindObjectsOfType<ChatHolder> ();
		foreach (ChatHolder holder in chats) {
			holder.SetPlayer(newPlayer);	
		}
	}

	void Update(){
		if (guiState == GUIState.Dedicated) {
			return;
		}
		guiState = GUIState.Normal;
		if (Input.GetButton ("ScoreBtn")) {
			guiState =GUIState.Playerlist;
		}
		if (Input.GetButtonDown ("Debug")) {
			showDebug= !showDebug;
		}
	}

	// Update is called once per frame
	void OnGUI () {
				IsMouseAV = Screen.lockCursor;
				if (guiState == GUIState.Dedicated) {
					DedicatedDraw();
				}
				if (LocalPlayer == null) {
						return;
				}
				float screenX = Screen.width, screenY = Screen.height;
				GUI.skin = guiSkin;
				switch (guiState) {
				case GUIState.Normal:
			
				//MAIN HUD
						if (LocalPlayer.IsDead ()) {
							Screen.lockCursor = false;
							RespawnGui();
								
					
						} else {
							
							MainHud();	
							if (GUI.GetNameOfFocusedControl () != "") {
									Screen.lockCursor = false;
							}else{
								Screen.lockCursor = true;
							}
						}

						break;

				case GUIState.Playerlist:
					PlayerList();
				
					break;


				}
				//Message Section
				GUI.skin = messageSkin;
				while (guiMessages.Count>0&&guiMessages.Peek().destroyTime<Time.time) {
						guiMessages.Dequeue ();
				}
		
				foreach (GUIMessage guiMessage in guiMessages) {
						Vector3 Position = MainCamera.WorldToScreenPoint (guiMessage.worldPoint);
						float size = guiMessage.getMessageSize (this);
						Rect messRect = new Rect (Position.x - size / 2, Screen.height - Position.y, size, size);
			
						Texture messTexture = guiMessage.getTexture (this);
						if (messTexture != null) {
								GUI.Label (messRect, messTexture);
						}
						if (guiMessage.text != "") {
								GUI.Label (messRect, guiMessage.text);
						}
				}
				while (logMessages.Count>50) {
						logMessages.Dequeue ();

				}
				if (showDebug) {
						GUI.BeginGroup (new Rect (0, Screen.height - 300, 400, 300));
						int j = 0;
						foreach (LogMess logMessage in logMessages) {
								j++;
								Rect messRect = new Rect (0, 40 * (logMessages.Count - j), 300, 40);
								GUI.Label (messRect, logMessage.mess);
								//messRect = new Rect (0,50*(j*2+1),300,50);
								//GUI.Label(messRect,logMessage.trace);
						}
						GUI.EndGroup ();
				} else {

					chats[0].DrawChatBox();

				}

		//LABEL SECTION
		GUI.skin = guiSkin;

		Rect versionrect = new Rect (screenX  - VersionSize,0, VersionSize, VersionSize);
		
		GUI.DrawTexture(versionrect, VersionMark);
		GUI.color = Color.black;
		Vector3 pivotPoint = new Vector2(screenX  - VersionSize/2, VersionSize/2);
		GUIUtility.RotateAroundPivot(45.0f, pivotPoint);
		versionrect = new Rect (screenX  - VersionSize,VersionSize/2.5f, VersionSize*10, VersionSize);
		GUI.Label(versionrect , "Version:" +PlayerManager.instance.version + " Date: "+ System.DateTime.Now.ToShortDateString());
		
		

	}

	void RespawnGui(){
		/*float screenX = Screen.width, screenY = Screen.height;
		Screen.lockCursor = false;
		
		
		Pawn[] prefabClass = PlayerManager.instance.avaiblePawn;
		float slotsizeX = screenX / prefabClass.Length;
		float slotsizeY = screenY / 10;
		int timer = (int)LocalPlayer.GetRespawnTimer ();
		for (int i=0; i<prefabClass.Length; i++) {
			Rect crosrect = new Rect (slotsizeX * i, crosshairHeight * 4, slotsizeX, slotsizeY);
			
			if (GUI.Button (crosrect, prefabClass [i].publicName + "(" + timer + ")")) {
				LocalPlayer.selected = i;
				LocalPlayer.isStarted = true;
			}
			
		}*/

		respawnMenu.DrawMenu ();
		if (Choice._Player != -1 && Choice._Robot != -1&&Choice._Team!= -1) {
			LocalPlayer.SetTeam (Choice._Team);
			LocalPlayer.selected = Choice._Player;
			LocalPlayer.selectedBot = Choice._Robot;		
			int timer = (int)LocalPlayer.GetRespawnTimer ();
			if(respawnMenu.SetTimer(timer)){
				LocalPlayer.isStarted  =true;

			}
		}
		if (PhotonNetwork.isMasterClient&&(Application.platform==RuntimePlatform.WindowsPlayer||Application.platform==RuntimePlatform.WindowsEditor)) {
			//Debug.Log("olololo");
			Rect crosrect = new Rect (0, 0, 100, 50);
			if(GUI.Button(crosrect,"Dedicated")){
				PhotonNetwork.Destroy(LocalPlayer.GetView());
				guiState = GUIState.Dedicated;
			}
		}
		
	}

	void MainHud(){
		float screenX = Screen.width, screenY = Screen.height;
		//Screen.lockCursor = true;
		Rect crosrect = new Rect ((screenX - crosshairWidth) / 2, (screenY - crosshairHeight) / 2, crosshairWidth, crosshairHeight);
		
		Pawn myPawn = LocalPlayer.GetCurrentPawn ();

		GUI.Label (crosrect, crosshair);
		if (myPawn!=null&&LocalPlayer.useTarget != null&&(myPawn.myTransform.position-LocalPlayer.useTarget.myTransform.position).sqrMagnitude<Player.SQUERED_RADIUS_OF_ACTION) {
			GUI.Label (crosrect, LocalPlayer.useTarget.guiIcon);
			crosrect = new Rect ((screenX - crosshairWidth) / 2, (screenY - crosshairHeight) / 2, crosshairWidth*5, crosshairHeight);
			if(LocalPlayer.useTarget is WeaponPicker){
				GUI.Label (crosrect, "Pick Up " + LocalPlayer.useTarget.tooltip);
			}else{
				GUI.Label (crosrect, "use" + LocalPlayer.useTarget.tooltip);
			}
		}
		crosrect = new Rect (screenX - crosshairWidth, 0, crosshairWidth, crosshairHeight);
		PlayerStats localstats = LocalPlayer.GetPlayerStats ();
		if (!LocalPlayer.inBot) {
			float timer = localstats.robotTime; 
			
			if (timer == 0) {
				GUI.Label (crosrect, "PRES F TO SPAMN ROBOT");
			} else {
				
				GUI.Label (crosrect, GetFormatedTime (timer));
			}
		} else {
			GUI.Label (crosrect, "PRES E  TO EXIT");
			
		}
	
		if (myPawn != null) {
			if (myPawn.curLookTarget != null) {
				Pawn targetPawn = myPawn.curLookTarget.GetComponent<Pawn> (); 
				
				if (targetPawn != null) {
					Rect labelrect = new Rect ((screenX - crosshairWidth) / 2, screenY / 2 - crosshairHeight * 2, crosshairWidth * 2, crosshairHeight * 2);
					
					if (targetPawn.player != null && targetPawn.player != LocalPlayer) {
						//Rect labelrect = new Rect ((screenX  - crosshairWidth)/2,screenY/2-crosshairHeight*2, crosshairWidth*2, crosshairHeight*2);
						GUI.Label (labelrect, targetPawn.player.GetName ());
					}
					if (!LocalPlayer.inBot && targetPawn == LocalPlayer.GetRobot ()) {
						GUI.Label (labelrect, "PRESS E TO ENTER");
					}
					
				}
				
			}
		}
		List<Pawn> seenablePawn = LocalPlayer.GetCurrentPawn ().getAllSeenPawn ();
		//Debug.Log (seenablePawn);
		float maxsize = LocalPlayer.GetCurrentPawn ().seenDistance;
		Pawn robot = LocalPlayer.GetRobot ();
		for (int i=0; i<seenablePawn.Count; i++) {
			if (robot == seenablePawn [i]) {
				continue;
			}
			
			Pawn target = seenablePawn [i];
			Vector3 Position = MainCamera.WorldToScreenPoint (target.myTransform.position + target.headOffset);
			
			float size = MarkSize * maxsize / Position.z;
			
			Rect mark = new Rect (Position.x - size / 2, Screen.height - Position.y, size, size);

			if (seenablePawn [i].team == LocalPlayer.team) {
				AliaMark.fontSize  = Mathf.RoundToInt(size/2.5f);
				GUI.Label (mark,target.health.ToString(), AliaMark);
			} else {
				GUI.Label (mark, EnemyMark);
			}
		}
		Rect statsRect = new Rect (screenX - crosshairWidth * 4, Screen.height - crosshairHeight * 5, crosshairWidth * 4, crosshairHeight);

		GUI.Label (statsRect, "JetPack Charges: " + localstats.jetPackCharge);
	
	    statsRect = new Rect (screenX - crosshairWidth * 4, Screen.height - crosshairHeight * 4, crosshairWidth * 4, crosshairHeight);
		if(localstats.reloadTime<=0){
			GUI.Label (statsRect, "Weapon: " + localstats.gunName);
		}else{
			GUI.Label (statsRect, "Reloading: " + localstats.gunName+"("+localstats.reloadTime.ToString("0.0")+")");
		}
		statsRect = new Rect (screenX - crosshairWidth * 4, Screen.height - crosshairHeight * 3, crosshairWidth * 4, crosshairHeight);
		GUI.Label (statsRect, "Health: " + localstats.health);
		statsRect = new Rect (screenX - crosshairWidth * 4, Screen.height - crosshairHeight * 2, crosshairWidth * 4, crosshairHeight);
		GUI.Label (statsRect, "Ammo: " + localstats.ammoInGun + "/" + localstats.ammoInGunMax);
		statsRect = new Rect (screenX - crosshairWidth * 4, Screen.height - crosshairHeight, crosshairWidth * 4, crosshairHeight);
		GUI.Label (statsRect, "Ammo in Bag: " + localstats.ammoInBag);

		
		//game stats section
		GameStats gamestats = PVPGameRule.instance.GetStats ();
		Rect rectforName = new Rect ((screenX - crosshairWidth * 10) / 2, 0, crosshairWidth * 10, crosshairHeight);
		GUI.Label (rectforName, LocalPlayer.GetName () + " Team:" + FormTeamName (LocalPlayer.team));
		rectforName = new Rect ((screenX - crosshairWidth * 10) / 2, crosshairHeight / 2, crosshairWidth * 10, crosshairHeight);
		GUI.Label (rectforName, "K/D/A " + LocalPlayer.Score.Kill + "/" + LocalPlayer.Score.Death + "/" + LocalPlayer.Score.Assist);
		rectforName = new Rect ((screenX - crosshairWidth * 10) / 2, crosshairHeight, crosshairWidth * 10, crosshairHeight);
		GUI.Label (rectforName, FormTeamName (1) + gamestats.score [0] + "|" + gamestats.maxScore + " |" + FormTeamName (2) + gamestats.score [1]);
		

	}

	void DedicatedDraw(){
		float screenX = Screen.width, screenY = Screen.height;
		GameStats gamestats = PVPGameRule.instance.GetStats ();
		Rect rectforName = new Rect ((screenX - crosshairWidth * 10) / 2, crosshairHeight, crosshairWidth * 10, crosshairHeight);
		GUI.Label (rectforName, FormTeamName (1) + gamestats.score [0] + "|" + gamestats.maxScore + " |" + FormTeamName (2) + gamestats.score [1]);

		Player[] players = PlayerManager.instance.FindAllPlayer ();
		for (int i =0; i<players.Length; i++) {
			float size = crosshairWidth / 2;
			Rect messRect = new Rect (size, size * (6 + i), screenX, size);
			GUI.Label (messRect, players [i].GetName () +"    Kill:"+players [i].Score.Kill +"    Death:"+players [i].Score.Death +"    Assist:"+players [i].Score.Assist);
			
			
		}

	}
	
	void PlayerList(){
		float screenX = Screen.width, screenY = Screen.height;
		Player[] players = PlayerManager.instance.FindAllPlayer ();
		for (int i =0; i<players.Length; i++) {
			float size = crosshairWidth / 2;
			Rect messRect = new Rect (size, size * (6 + i), screenX, size);
			GUI.Label (messRect, players [i].GetName ()+" "+players [i].IsMaster());
			
			
		}
		
	}
	
	public void AddMessage(string text,Vector3 worldPoint, MessageType type ){
		GUIMessage message = new GUIMessage();
		message.destroyTime = Time.time + messageDelay;
		message.text = text;
		message.worldPoint = worldPoint;
		message.type = type;
		guiMessages.Enqueue(message);
	}
	public static string GetFormatedTime(float input){
		int seconds;
		int minutes;
		
		minutes = Mathf.FloorToInt(input / 60);
		seconds = Mathf.FloorToInt(input - minutes * 60);
		return string.Format("{0:0}:{1:00}", minutes, seconds);
	}
	public static string FormTeamName(int team){
		switch (team) {
			case 1:
			return "A TEAM"	;
				break;
		case 2:
			return"POWER RANGERS";
				break;
		}
		return "";
	}
	void HandleLog(string logString, string stackTrace, LogType type) {
		if (type == LogType.Warning) {
			return;		
		}
		LogMess mess = new LogMess ();
		mess.mess = logString;
		mess.trace = stackTrace;
		logMessages.Enqueue( mess);

	}
	void OnEnable() {
		Application.RegisterLogCallback(HandleLog);
	}
}
