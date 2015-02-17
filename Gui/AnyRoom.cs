using UnityEngine;
using System.Collections;

public class AnyRoom : MonoBehaviour {

	public string RoomData room ; 

	public UILabel Name;

	public UILabel GameMode;
	
	public UILabel SizeRoom;

	public MainMenuGUI MainScriptGUI;

	public ServerHolder Server; 
	bool have = false;
	public bool shablon;

	// Use this for initialization
	void Start () {
		Server = FindObjectOfType<ServerHolder> ();
	}
   
	// Update is called once per frame
	
	void Update(){
		if(!Server.allRoom.Contains(room)){
			MainScriptGUI.Rooms.Remove(room.name);
			Destroy(this.gameObject);
		}
		
	}
	
	void UpdateRoom (RoomData room ) 
	{
		this.room =room;	
		Name.text = room.name;
		SizeRoom.text = room.playerCount + " / " + room.maxPlayers;
		GameMode.test =  TextGenerator.instance.GetSimpleText(room.mode);
			
	}

	public void SelectBut()
	{
		MainScriptGUI.JoinRoom(room);
	}


}
