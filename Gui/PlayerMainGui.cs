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
			if(!LocalPlayer.inBot){
				float timer =LocalPlayer.GetRobotTimer();

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
			
		}

		Rect rectforName = new Rect ((screenX-crosshairWidth)/2,0, crosshairWidth*4, crosshairHeight);
		GUI.Label(rectforName,LocalPlayer.GetName()+ "Team:" +FormTeamName(LocalPlayer.team));
		rectforName = new Rect ((screenX-crosshairWidth)/2,crosshairHeight/2, crosshairWidth*2, crosshairHeight);
		GUI.Label(rectforName,"K/D/A "  +LocalPlayer.Score.Kill+"/"+LocalPlayer.Score.Death+"/"+LocalPlayer.Score.Assist);

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
