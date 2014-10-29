using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class PlayerMainGui : MonoBehaviour {

	public HUDText P1Hud;


	public Player LocalPlayer;

	public Camera MainCamera;
	
	public Transform MainCameraTransform;

	public float MarkSize;

	public GUIStyle EnemyMark;
	public GUIStyle AliaMark;

	public float VersionSize;
	
	public Texture  VersionMark;

	public enum MessageType{
			STD_MESSAGE=0,
			DMG_TEXT=1,
			KILL_TEXT = 2,
			ACHIVEMENT = 3,
			ANALIZE_POINT = 4,
			OPEN_LORE = 5,
            MONEY_REWARD=6,
            LVL_REWARD=7
	}
	public Texture[] messageTexture;
	public GUIStyle[] messageStyle;
	public float[] messageSize;
	public float messageDelay=1.0f;
	public class GUIMessage{
		public float destroyTime;
        public HUDText.Entry entry;
		public Vector3 worldPoint;
		public Texture icon= null;
		public string text="";
		public MessageType type;
		public float getMessageSize(PlayerMainGui outer){
			if(outer.messageSize.Length<=(int)type){
				return outer.messageSize[0];
			}
			return outer.messageSize[(int)type];
		}
		public GUIStyle getStyle(PlayerMainGui outer){
			if(outer.messageStyle.Length<=(int)type){
				return outer.messageStyle[0];
			}
			return outer.messageStyle[(int)type];
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
        public float maxHealth = 200;
		public BaseWeapon gun;
		public float ammoInBag=0;
		public float reloadTime=0;
		public float jetPackCharge= 0;
        public float pumpCoef = 0;
		public SkillBehaviour skill;
		
	       
	}
	public class LevelStats{
		public int[]  classLvl;	
		public int[]  classProcent;
		public int  playerLvl;
		public int  playerProcent;
	}
	public class GameStats{ 
		public float gameTime;
		public int[] score;
		public int maxScore;
	}
	public enum GUIState{
		Normal,
		Playerlist,
		Dedicated,
		GameResult,
		KillCam,
		Respawn,
        None,
        Pause
			
	}
    public GUIState guiState = GUIState.None;

	public GUISkin guiSkin;

	public GUISkin messageSkin;

    private SelectPlayerGUI respawnMenu;

    private PauseMenu pausegui;

	private PlayerHudNgui hud;

	private ChatHolder[] chats;

	public static bool IsMouseAV;


	

	Statistic stat;
	// Use this for initialization
	void Start () {
		MainCamera = Camera.main;
		MainCameraTransform= MainCamera.transform;
		
	if(GameObject.Find("P1"))
			P1Hud = GameObject.Find("Player_HUD_text").GetComponent<HUDText>();
        

	

	}
    void OnEnabled()
    {
        Debug.Log("OLOLO");
    }
	public void SetLocalPlayer(Player newPlayer){
		LocalPlayer = newPlayer;
		chats = FindObjectsOfType<ChatHolder> ();
		foreach (ChatHolder holder in chats) {
			holder.SetPlayer(newPlayer);	
		}

		stat =  Transform.FindObjectOfType<Statistic>(); //Статистика (+)
        stat.SetLocalPalyer(newPlayer);

		hud = Transform.FindObjectOfType<PlayerHudNgui> ();
		hud.SetLocalPlayer(LocalPlayer);
		respawnMenu = Transform.FindObjectOfType<SelectPlayerGUI>();
		pausegui = Transform.FindObjectOfType<PauseMenu>();
       
        respawnMenu.SetLocalPlayer(LocalPlayer);
		ChageState(GUIState.Respawn);
        
	}

    public void ReSize() {
        hud.ReSize();
        stat.ReSize();
    }


	void Update(){
        if (LocalPlayer == null)
        {
            if (NetworkController.smartFox!=null&&PlayerView.allPlayer.ContainsKey(NetworkController.smartFox.MySelf.Id))
            {
                SetLocalPlayer( PlayerView.allPlayer[NetworkController.smartFox.MySelf.Id].observed);
            }
            return;
        }
		if (guiState == GUIState.Dedicated) {
			return;
		}
        if (pausegui.IsActive())
        
        {

            ChageState(GUIState.Pause);
            return;
        }
        if (InputManager.instance.GetButton("ScoreBtn"))
        {
			
						ChageState (GUIState.Playerlist);
					
						
		} else {
            if (LocalPlayer != null && !LocalPlayer.IsDead())
            {

                if (PVPGameRule.instance.isGameEnded)
                {

                    ChageState(GUIState.GameResult);
                   
                }
                else
                {
                    ChageState(GUIState.Normal);
                }



            }
            else
            {
              
                if (guiState != GUIState.KillCam)
                {
                    ChageState(GUIState.Respawn);
                }
            }
		}
		
		 Messagess();
	
	

	}
   
	
	public void ChageState(GUIState nextState ){
       
        switch (nextState)
        {
            case GUIState.Normal:
                hud._DeadGUI.DeActivate();
                 pausegui.BackToGame();
                stat.DeActivate();
                respawnMenu.DeActivate();
                hud.Activate();
                break;
            case GUIState.Respawn:
                hud._DeadGUI.DeActivate();
                pausegui.BackToGame();
                stat.DeActivate();
                hud.DeActivate();
                respawnMenu.Activate();
                break;
            case GUIState.Playerlist:
                hud._DeadGUI.DeActivate();
                pausegui.BackToGame();
                hud.DeActivate();
                respawnMenu.DeActivate();
                stat.Activate();
                break;
            case GUIState.KillCam:
                hud._DeadGUI.Activate();
                pausegui.BackToGame();
                stat.DeActivate();
                respawnMenu.DeActivate();
                hud.DeActivate();
                break;
            case GUIState.GameResult:
                hud._DeadGUI.DeActivate();
                pausegui.BackToGame();
                stat.DeActivate();
                respawnMenu.DeActivate();
                hud.DeActivate();
                break;
            case GUIState.Pause:
                hud._DeadGUI.DeActivate();
                stat.DeActivate();
                respawnMenu.DeActivate();
                hud.DeActivate();
                pausegui.ActivateMenu();
                break;
            
            

        }
		
		guiState =nextState;
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
				case GUIState.Respawn:
						Screen.lockCursor = false;
						RespawnGui();
					break;
				case GUIState.Normal:
			
						//MAIN HUD
						MainHud();	
							if (GUI.GetNameOfFocusedControl () != "") {
									Screen.lockCursor = false;
							}else{
								Screen.lockCursor = true;
							}

						break;

				case GUIState.Playerlist:
					PlayerList();
				
					break;
				case GUIState.KillCam:
                    
					Screen.lockCursor = false;
					PlayerList();
					
					break;
				case GUIState.GameResult:
					GameResult();
					
					break;

				
				}
				
			
			 
			LabelSection();
		

	}
	void Messagess(){
		float screenX = Screen.width, screenY = Screen.height;
		//Message Section
		//GUI.skin = messageSkin;
		while (guiMessages.Count>0&&guiMessages.Peek().destroyTime<Time.time) {

            P1Hud.Delete(guiMessages.Dequeue().entry);
		}
		//		Debug.Log (guiMessages.Count);
		foreach (GUIMessage guiMessage in guiMessages) {
			
				float size=0;
				GUIStyle messStyle = guiMessage.getStyle (this);
				switch(guiMessage.type){
				case MessageType.ACHIVEMENT:
				size = guiMessage.getMessageSize (this);
				Rect achvmessRect = new Rect (screenX - size ,screenY - size/1.5f, size, size);

					GUI.Label (achvmessRect,"ACHIVMENT UNLOCK: \n " +guiMessage.text,messStyle);
				
				break;
				
				default:
               
				break;
				}
		}


        foreach (ShowOnGuiComponent component in ShowOnGuiComponent.allShowOnGui)
        {


         
            float size = MarkSize;

            
            string publicName = component.GetTitle();
            if (component.hudentry == null)
            {
                //Debug.Log("createFor" + component);
                if (component.prefabName == "")
                {
                    component.hudentry = P1Hud.Add(publicName, Color.red, component.myTransform.position, true);
                }
                else
                {
                    component.hudentry = P1Hud.Add(component.prefabName, component.spriteName, publicName, component.myTransform.position, true);
                }
                component.hudentry.withArrow = component.withArrow;
            
            }
            component.hudentry.isShow =component.IsShow(MainCameraTransform,LocalPlayer.team); 
            component.hudentry.startpos = component.myTransform.position;
           // 
            component.hudentry.label.text = publicName;
			
			component.ChangeTeamColor(component.team == LocalPlayer.team||component.alwaysFriendly);
           

        }
        P1Hud.ReDraw();
		while (logMessages.Count>50) {
				logMessages.Dequeue ();

		}
		/*if (showDebug) {
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

		}*/
	}
	void LabelSection(){
		float screenX = Screen.width, screenY = Screen.height;
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
    public void Annonce(AnnonceType type) {
        hud.Annonce(type, AnnonceAddType.NONE, "");
    
    }
    public void Annonce(AnnonceType type, AnnonceAddType sprite)
    {
        hud.Annonce(type, sprite, "");

    }
    public void Annonce(AnnonceType type, AnnonceAddType sprite,string text)
    {
        hud.Annonce(type, sprite, text);

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

		/*
		if (Choice._Player != -1 && Choice._Robot != -1&&Choice._Team!= -1) {
			LocalPlayer.SetTeam (Choice._Team);
			
			int timer = (int)LocalPlayer.GetRespawnTimer ();
			if(respawnMenu.SetTimer(timer)){
				LocalPlayer.selectedBot = Choice._Robot;		
				LocalPlayer.selected = Choice._Player;
				LocalPlayer.isStarted  = true;


			}
		}
		if (PhotonNetwork.isMasterClient&&(Application.platform==RuntimePlatform.WindowsPlayer||Application.platform==RuntimePlatform.WindowsEditor)) {
			//Debug.Log("olololo");
			Rect crosrect = new Rect (0, 0, 100, 50);
			if(GUI.Button(crosrect,"Dedicated")){
				PhotonNetwork.Destroy(LocalPlayer.GetView());
				guiState = GUIState.Dedicated;
			}
		}*/
		
	}
	void GameResult(){
		float screenX = Screen.width, screenY = Screen.height;
		float TimerLabel = screenX / 4;
		Rect crosrect = new Rect ((screenX - TimerLabel) / 2, (screenY - TimerLabel) / 2, TimerLabel, TimerLabel);
        GUI.Label(crosrect, "WINNER: " + FormTeamName(  GameRule.instance.Winner()) + "");
		crosrect = new Rect ((screenX - TimerLabel) / 2, (screenY - TimerLabel) / 2 +TimerLabel, TimerLabel, TimerLabel);
        float restart = GameRule.instance.GetRestartTimer();
        if (restart < 0)
        {
            GUI.Label(crosrect, "Prepare for loading");

        }
        else
        {
            GUI.Label(crosrect, "NEXT ROUND IN  " + GameRule.instance.GetRestartTimer().ToString("0.0") + " sec.");
        }
        
	}
	void MainHud(){
		float screenX = Screen.width, screenY = Screen.height;
		//Screen.lockCursor = true;
		
		hud.Activate();
		/*Pawn myPawn = LocalPlayer.GetCurrentPawn ();

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
		}	*/
		/*Pawn robot = LocalPlayer.GetRobot ();
		List<Pawn> seenablePawn = LocalPlayer.GetCurrentPawn ().getAllSeenPawn ();
		if (LocalPlayer.inBot) {
			//Debug.Log("ROBOT");
			seenablePawn = robot.getAllSeenPawn ();
		}

		//Debug.Log (seenablePawn[0]);
		float maxsize = LocalPlayer.GetCurrentPawn ().seenDistance;
		if (LocalPlayer.inBot) {
			maxsize= robot.seenDistance;
		}
		for (int i=0; i<seenablePawn.Count; i++) {
			
			Pawn target = seenablePawn [i];
			if (!target.isActive||target.myTransform==null) {
				//Debug.Log ("it'sROBOIT");
				continue;
			}

		
			Vector3 Position = MainCamera.WorldToScreenPoint (target.myTransform.position + target.headOffset);
			if(Position.z<0){
				continue;
			}
			float size = MarkSize * maxsize / Position.z;
			
			Rect mark = new Rect (Position.x - size / 2, Screen.height - Position.y, size, size);
			string publicName = "";
			if(robot == target){
				publicName = "MY JUGGER (Press E)";
			}else{
				if(target.player!=null){
					publicName	=target.player.GetName();
				}else{
					publicName	=target.publicName;
				}
			}
			if (seenablePawn [i].team == LocalPlayer.team) {
			
				GUI.Label (mark, publicName+"\n"+target.health.ToString("0"), AliaMark);
			} else {
				GUI.Label (mark, publicName , EnemyMark);
			}
		}/*
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
		*/
			
	}

	void DedicatedDraw(){
		/*float screenX = Screen.width, screenY = Screen.height;
        GameStats gamestats = GameRule.instance.GetStats();
		Rect rectforName = new Rect ((screenX - crosshairWidth * 10) / 2, crosshairHeight, crosshairWidth * 10, crosshairHeight);
		GUI.Label (rectforName, FormTeamName (1) + gamestats.score [0] + "|" + gamestats.maxScore + " |" + FormTeamName (2) + gamestats.score [1]);

		List<Player> players = PlayerManager.instance.FindAllPlayer ();
		for (int i =0; i<players.Count; i++) {
			float size = crosshairWidth / 2;
			Rect messRect = new Rect (size, size * (6 + i), screenX, size);
			GUI.Label (messRect, players [i].GetName () +"    Kill:"+players [i].Score.Kill +"    Death:"+players [i].Score.Death +"    Assist:"+players [i].Score.Assist);
			
			
		}*/

	}
	
	void PlayerList(){
			
	
		
	}



	
	public void AddMessage(string text,Vector3 worldPoint, MessageType type ){
		GUIMessage message = new GUIMessage();
		message.destroyTime = Time.time + messageDelay;
		message.text = text;
		message.worldPoint = worldPoint;
		message.type = type;
		float damage = 0f;
		
        message.entry = P1Hud.Add(message.text, Color.red,  message.worldPoint,false);
		guiMessages.Enqueue(message);
	}
	public void AddMessage(string text, MessageType type ){
		try{
		switch(type){
			case MessageType.MONEY_REWARD:
				hud.AddMoneyMessage(text);
			break;
			case MessageType.LVL_REWARD:
				hud.AddLvlMessage(text);
			break;
			default:
				GUIMessage message = new GUIMessage();
				message.destroyTime = Time.time + messageDelay;
				message.text = text;
				message.worldPoint = Vector3.zero;
				message.type = type;
				guiMessages.Enqueue(message);
			break;
		}
		}catch(Exception e){
			Debug.Log(e);
		}
	}
	public void AddMessage(string text, Texture icon, MessageType type ){
		GUIMessage message = new GUIMessage();
		message.destroyTime = Time.time + messageDelay;
		message.text = text;
		message.icon = icon;
		message.worldPoint = Vector3.zero;
		message.type = type;
		guiMessages.Enqueue(message);
	}
	public void RemoveMessage(HUDText.Entry entry){
        if (P1Hud != null)
        {
            P1Hud.Delete(entry);
        }
		
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
                return "Команда А";
				break;
		case 2:
			return"Команда B";
				break;
            case 0:
                return "Планета Каспи";
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
	public void InitKillCam(Player killer){
		if (killer != null) {
				KillCamera s_Instance =FindObjectOfType<KillCamera>();
				s_Instance.enabled = true;
				if (s_Instance.Init (killer)) {
						
						ChageState (GUIState.KillCam);
				} else {
						ChageState (GUIState.Respawn);
				}
		} else {
			ChageState (GUIState.Respawn);
		}
	}
	public void InitKillCam(Pawn killer){
		if (killer != null) {
				KillCamera s_Instance =FindObjectOfType<KillCamera>();
				s_Instance.enabled = true;
				if (s_Instance.Init (killer)) {
						Debug.Log("killcam");
						ChageState (GUIState.KillCam);
				} else {
						ChageState (GUIState.Respawn);
				}
		} else {
			ChageState (GUIState.Respawn);
		}
	}
	public void StopKillCam(){
		ChageState(GUIState.Respawn);
	}
}
