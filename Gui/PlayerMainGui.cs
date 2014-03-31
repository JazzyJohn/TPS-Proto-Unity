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

			Rect crosrect = new Rect ((screenX  - crosshairWidth)/2,( screenY  - crosshairHeight)/2, crosshairWidth, crosshairHeight);
			
			GUI.Label(crosrect, crosshair);
			crosrect = new Rect (0,0, crosshairWidth, crosshairHeight);
			Debug.Log(GetFormatedTime(LocalPlayer.GetRobotTimer()));
			GUI.Label(crosrect,GetFormatedTime(LocalPlayer.GetRobotTimer()));
		}
	}
	public static string GetFormatedTime(float input){
		int seconds;
		int minutes;
		
		minutes = Mathf.FloorToInt(input / 60);
		seconds = Mathf.FloorToInt(input - minutes * 60);
		return string.Format("{0:0}:{1:00}", minutes, seconds);
	}
}
