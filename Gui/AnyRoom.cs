using UnityEngine;
using System.Collections;

public class AnyRoom : MonoBehaviour {

	public enum TypeRoom{NewRoom, JoinRoom};
	public TypeRoom _TypeRoom;

	public UILabel Name;
	public UILabel SizeRoom;

	public Element_metod MainScriptGUI;

	public ServerHolder Server; 
	bool have = false;
	public bool shablon;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (!shablon)
		{
			have = false;
			foreach(RoomInfo room in Server.allRooms)
			{
				if(gameObject.name == room.name)
				{
					SizeRoom.text = room.playerCount + " / " + room.maxPlayers;
					have=true;
				}
			}
			if (!have)
			{
				MainScriptGUI.Rooms.Remove(gameObject.name);
				Destroy(this.gameObject);
			}
		}
	}

	public void SelectBut()
	{
		MainScriptGUI.ActivBut = this.gameObject;
	}

}
