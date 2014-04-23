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
	public Texture AliaMark;
	
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
		public float getMessageSize(){
			if(messageSize.Leght<=(int)type){
				return messageSize[0];
			}
			return messageSize[(int)type];
		}
		public Texture getTexture(){
			if(messageTexture.Leght<=(int)type){
				return messageTexture[0];
			}
			return messageTexture[(int)type];
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
	public class PlayerStats{
		public float robotTime;
		public float health;
		public float ammoInGun;
		public float ammoInGunMax;
		public float ammoInBag;
			
	}
	public class GameStats{
		public float gameTime;
		public int[] score;
		public int maxScore;
	}

	// Use this for initialization
	void Start () {
		MainCamera = Camera.main;
	}
	
	// Update is called once per frame
	void OnGUI () {
		if (LocalPlayer == null) {
			return;
		}
		float screenX = Screen.width, screenY = Screen.height;
		
		//MAIN HUD
		if (LocalPlayer.IsDead()) {
			Screen.lockCursor = false;


			Pawn[] prefabClass = PlayerManager.instance.avaiblePawn;
			float slotsizeX =screenX/prefabClass.Length;
			float slotsizeY = screenY/10;
			int timer =(int) LocalPlayer.GetRespawnTimer();
			for (int i=0; i<prefabClass.Length;i++) {
				Rect crosrect = new Rect (slotsizeX*i,  slotsizeY, slotsizeX, slotsizeY);
				
				if(GUI.Button(crosrect, prefabClass[i].publicName+"("+timer+")")){
					LocalPlayer.selected = i;
					LocalPlayer.isStarted=true;
				}

			}

		} else{
			Screen.lockCursor = true;
			Rect crosrect = new Rect ((screenX  - crosshairWidth)/2,( screenY  - crosshairHeight)/2, crosshairWidth, crosshairHeight);
			
			GUI.Label(crosrect, crosshair);
			crosrect = new Rect (screenX-crosshairWidth,0, crosshairWidth, crosshairHeight);
			PlayerStats localstats  =LocalPlayer.GetPlayerStats();
			if(!LocalPlayer.inBot){
				float timer =localstats.robotTime; 

				if(timer==0){
					GUI.Label(crosrect,"PRES F TO SPAMN ROBOT");
				}else{

					GUI.Label(crosrect,GetFormatedTime(timer));
				}
			}else{
				GUI.Label(crosrect,"PRES E  TO EXIT");
				
			}

				Pawn myPawn = LocalPlayer.GetCurrentPawn();
				if(myPawn!=null){
					if(myPawn.curLookTarget!=null){
						Pawn targetPawn = myPawn.curLookTarget.GetComponent<Pawn>(); 

						if(targetPawn!=null){
							Rect labelrect = new Rect ((screenX  - crosshairWidth)/2,screenY/2-crosshairHeight*2, crosshairWidth*2, crosshairHeight*2);

							if(targetPawn.player!=null&&targetPawn.player!=LocalPlayer){
								//Rect labelrect = new Rect ((screenX  - crosshairWidth)/2,screenY/2-crosshairHeight*2, crosshairWidth*2, crosshairHeight*2);
								GUI.Label(labelrect,targetPawn.player.GetName());
							}
							if(!LocalPlayer.inBot&&targetPawn==LocalPlayer.GetRobot()){
							    GUI.Label(labelrect,"PRESS E TO ENTER");
							}

						}

					}
				}
			List<Pawn> seenablePawn = LocalPlayer.GetCurrentPawn().getAllSeenPawn ();
			//Debug.Log (seenablePawn);
			float maxsize =LocalPlayer.GetCurrentPawn().seenDistance;
			Pawn robot = LocalPlayer.GetRobot ();
			for (int i=0; i<seenablePawn.Count; i++) {
				if(robot==seenablePawn[i]){
					continue;
				}
				
				Pawn  target =seenablePawn[i];
				Vector3 Position = MainCamera.WorldToScreenPoint(target.myTransform.position+target.headOffset);
				
				float size =MarkSize*maxsize/Position.z;
				
				Rect mark = new Rect (Position.x-size/2,Screen.height-Position.y,size,size);
				if(seenablePawn[i].team ==LocalPlayer.team){
					
					GUI.Label(mark,AliaMark);
				}else{
					GUI.Label(mark,EnemyMark);
				}
			}	

			Rect statsRect = new Rect (screenX-crosshairWidth*4,Screen.height -crosshairHeight*3, crosshairWidth*4, crosshairHeight);
			GUI.Label(statsRect,"Health: "+localstats.health);
			statsRect = new Rect (screenX-crosshairWidth*4,Screen.height -crosshairHeight*2, crosshairWidth*4, crosshairHeight);
			GUI.Label(statsRect,"Ammo: "+localstats.ammoInGun+"/"+localstats.ammoInGunMax);
			statsRect = new Rect (screenX-crosshairWidth*4,Screen.height -crosshairHeight, crosshairWidth*4, crosshairHeight);
			GUI.Label(statsRect,"Ammo in Bag: "+localstats.ammoInBag);

		}
		//game stats section
		gamestats = PVPGameRule.instance.GetStats();
		Rect rectforName = new Rect ((screenX-crosshairWidth)/2,0, crosshairWidth*4, crosshairHeight);
		GUI.Label(rectforName,LocalPlayer.GetName()+ "Team:" +FormTeamName(LocalPlayer.team));
		rectforName = new Rect ((screenX-crosshairWidth)/2,crosshairHeight/2, crosshairWidth*2, crosshairHeight);
		GUI.Label(rectforName,"K/D/A "  +LocalPlayer.Score.Kill+"/"+LocalPlayer.Score.Death+"/"+LocalPlayer.Score.Assist);
		rectforName = new Rect ((screenX-crosshairWidth)/2,crosshairHeight, crosshairWidth*2, crosshairHeight);
		GUI.Label(rectforName,FormTeamName(1)  +gamestats.score[0] +"|"+gamestats.maxScore+" |" FormTeamName(2) +gamestats.score[1]);
		
		//Message Section
		while(guiMessages.Count>0&&guiMessages.Peek().destroyTime<Time.time){
			guiMessages.Dequeue();
		}
		
		foreach (GuiMessages guiMessage in guiMessages)
		{
			Vector3 Position = MainCamera.WorldToScreenPoint(guiMessage.worldPoint);
			float size =guiMessage.getMessageSize();
			Rect messRect = new Rect (Position.x-size/2,Screen.height-Position.y,size,size);
			
			Texture messTesxture =guiMessage.getTexture();
			if(messTesxture!=null){
				GUI.Label(messRect,guiMessage.messTesxture);
			}
			if(guiMessage.text!=""){
				GUI.Label(messRect,guiMessage.text);
			}
		}
	}
	
	public void AddMessage(string text,Vector3 worldPoint, MessageType type ){
		GUIMessage message = new GUIMessage();
		message.destroyTime = Time.time + messageDelay;
		message.text = text;
		message.worldPoint = worldPoint;
		message.type = type;
		guiMessages.Enqueue(messages);
	}
	public static string GetFormatedTime(float input){
		int seconds;
		int minutes;
		
		minutes = Mathf.FloorToInt(input / 60);
		seconds = Mathf.FloorToInt(input - minutes * 60);
		return string.Format("{0:0}:{1:00}", minutes, seconds);
	}
	public string FormTeamName(int team){
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
}
