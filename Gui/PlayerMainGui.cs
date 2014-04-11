using UnityEngine;
using System.Collections;



public class PlayerMainGui : MonoBehaviour {
	public Texture crosshair;

	public float crosshairHeight;

	public float crosshairWidth;

	public Player LocalPlayer;

	// Use this for initialization
	void Start () {
		
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
			
		}
		Rect rectforName = new Rect ((screenX-crosshairWidth)/2,0, crosshairWidth*2, crosshairHeight);
		GUI.Label(rectforName,LocalPlayer.GetName());
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
}
