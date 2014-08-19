using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ChatHolder : MonoBehaviour {


	public bool isMain;
	
	public bool isGameChat;
	
	public string roomName;
	
	public Room myRoom

	[Serializable]
	public  struct ChatMessage{
		public string message;
		
		public string playerName;
		
		
	}

	private Queue<ChatMessage> messages = new Queue<ChatMessage>(),
	tempMessages= new Queue<ChatMessage>();

	public Player localPlayer;
	private Vector2 scrollPos = Vector2.zero;

	public float chatHeight =300.0f;
	public float chatWidth = 400.0f;
	private string chatInput = "";
	private bool needChat = false;
	private bool closeChat = true;
	// Use this for initialization
	void Start(){
		if( NetworkController.smartFox!=null){
			NetworkController.smartFox.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
			NetworkController.smartFox.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
		}
		//scrollPos.y = 1000000;
	}
	public void Init(){
		if( NetworkController.smartFox!=null){
			NetworkController.smartFox.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
			NetworkController.smartFox.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
		}
	}
	public void SetPlayer (Player newPlayer) {
		localPlayer = newPlayer;
	}


	void Update(){

        if (InputManager.instance.GetButtonDown("Send"))
        {
			//Debug.Log (GUI.GetNameOfFocusedControl());
			needChat=true;	
			chatInput="";
			
		}
		while (tempMessages.Count>0) {
			messages.Enqueue(tempMessages.Dequeue ());
			scrollPos.y = Mathf.Infinity;
			
		}


	}
	public void OnJoinRoom(BaseEvent evt) {

        
      
      Sfs2X.Entities.Room room = (Sfs2X.Entities.Room)evt.Params["room"];
	  if(room.Name ==roomName)[
		myRoom = room;
	  }
	
	
	}
	
	public void OnPublicMessage(BaseEvent evt) {


		string message = (string)evt.Params["message"];
		User sender = (User)evt.Params["sender"];
		if(Params.Contains["room"]){
			Sfs2X.Entities.Room room = (Sfs2X.Entities.Room)evt.Params["room"];
			if(room==myRoom){
				RPCAddToChat(message,sender.GetVariable("playerName"));
			}
		}else{
			if(isMain){
				
				RPCAddToChat(message,sender.GetVariable("playerName"));
			}
		}
		Sfs2X.Entities.Room room = (Sfs2X.Entities.Room)evt.Params["room"];
		if(room.Name ==roomName)[
			myRoom = room;
		}
	
	
	}
	// Update is called once per frame
	public void DrawChatBox () {
		while (messages.Count>50) {
			messages.Dequeue ();
			
		}
		GUILayout.BeginArea(new Rect(0, Screen.height -  chatHeight, chatWidth, chatHeight));
		
		//Show scroll list of chat messages
		scrollPos = GUILayout.BeginScrollView(scrollPos);

		GUI.color = Color.black;
		foreach (ChatMessage chatMessage in messages) {
		
			GUILayout.Label(chatMessage.playerName + ":" + chatMessage.message);
		}

		GUILayout.EndScrollView();



		if (needChat) {
			GUI.SetNextControlName("ChatField"+roomId);
			chatInput = GUILayout.TextField(chatInput);
			GUI.FocusControl("ChatField"+roomId);
		}

		Event e = Event.current;
		if (e.keyCode == KeyCode.Return) {
	
		
			if(GUI.GetNameOfFocusedControl()=="ChatField"+roomId){
				needChat= false;
				GUI.FocusControl("");
				if(chatInput!=""){
					AddMessage(chatInput);
					chatInput ="";
				}
				
			}	
			

		}

		GUILayout.EndArea();
		GUI.color = Color.white;
	}

	void AddMessage(string Message){
		if(isMain){
			NetworkController.smartFox.send( new PublicMessageRequest(Message) );
		}else{
			NetworkController.smartFox.send( new PublicMessageRequest(Message) ,myRoom);
		}
	}

	public void RPCAddToChat(string message,string name){
		ChatMessage mess = new ChatMessage ();
		mess.message = message;
		mess.playerName = name;
		

		tempMessages.Enqueue (mess);


	}
	

}
