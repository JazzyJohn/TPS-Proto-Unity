using UnityEngine;
using System.Collections;

public class AnyRoom : MonoBehaviour {

	public  RoomData room ;

    public UILabel Number;

	public UILabel Name;

    public UILabel  MapName;

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
		if(room!=null&&!Server.allRooms.Contains(room)){
			MainScriptGUI.Rooms.Remove(room.name);
			Destroy(this.gameObject);
		}
		
	}
	
	public void UpdateRoom (RoomData room,int i ) 
	{
		this.room =room;	
		Name.text = room.name;
		SizeRoom.text = room.playerCount + " / " + room.maxPlayers;
		GameMode.text =  TextGenerator.instance.GetSimpleText(room.mode);
        MapName.text = room.map;
        Number.text = i.ToString();
	}

	public void SelectBut()
	{
		MainScriptGUI.JoinRoom(room);
	}


}
