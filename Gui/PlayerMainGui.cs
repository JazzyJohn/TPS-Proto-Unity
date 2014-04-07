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
			Screen.showCursor = true;
			LocalPlayer.selected = 0;

			Pawn[] prefabClass = PlayerManager.instance.avaiblePawn;
			float slotsizeX =screenX/prefabClass.Length;
			float slotsizeY = screenY/2;
			for (int i=0; i<prefabClass.Length;i++) {
				Rect crosrect = new Rect (slotsizeX*i,( screenY  - slotsizeY)/2, slotsizeX, slotsizeY);
				
				if(GUI.Button(crosrect, prefabClass[i].publicName)){
					LocalPlayer.selected = i;
					LocalPlayer.isStarted=true;
				}

			}

		} else{
			Screen.showCursor = false;	
			Rect crosrect = new Rect ((screenX  - crosshairWidth)/2,( screenY  - crosshairHeight)/2, crosshairWidth, crosshairHeight);
			
			GUI.Label(crosrect, crosshair);
			if(!LocalPlayer.inBot){
				float timer =LocalPlayer.GetRobotTimer();
				crosrect = new Rect (screenX-crosshairWidth,0, crosshairWidth, crosshairHeight);
				if(timer==0){
					GUI.Label(crosrect,"PRES F TO SPAMN ROBOT");
				}else{

					GUI.Label(crosrect,GetFormatedTime(timer));
				}
				Pawn myPawn = LocalPlayer.GetCurrentPawn();
				if(myPawn!=null){
					if(myPawn.curLookTarget!=null){
						Pawn targetPawn = myPawn.curLookTarget.GetComponent<Pawn>(); 

						if(targetPawn!=null){
						
							if(targetPawn.player!=null){
								Rect labelrect = new Rect ((screenX  - crosshairWidth)/2,screenY/2-crosshairHeight*2, crosshairWidth, crosshairHeight);
								GUI.Label(labelrect,targetPawn.player.GetName());
							}
						}
					}
				}
			}

		}
		Rect rectforName = new Rect ((screenX-crosshairWidth)/2,0, crosshairWidth, crosshairHeight);
		GUI.Label(rectforName,LocalPlayer.GetName());

	}
	public static string GetFormatedTime(float input){
		int seconds;
		int minutes;
		
		minutes = Mathf.FloorToInt(input / 60);
		seconds = Mathf.FloorToInt(input - minutes * 60);
		return string.Format("{0:0}:{1:00}", minutes, seconds);
	}
}
