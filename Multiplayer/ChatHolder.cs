using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ChatHolder : MonoBehaviour {

	private PhotonView photonview;

	private int roomId;

	public string roomName;

	[Serializable]
	public  struct ChatMessage{
		public string message;
		public string uid;
		public string playerName;
		public int team;
		
	}

	private Queue<ChatMessage> messages = new Queue<ChatMessage>();

	public Player localPlayer;
	private Vector2 scrollPos = Vector2.zero;

	public float chatHeight =300.0f;
	public float chatWidth = 400.0f;
	// Use this for initialization
	void Start(){
		photonview = GetComponent<PhotonView> ();

	}
	public void SetPlayer (Player newPlayer) {
		localPlayer = newPlayer;
	}
	private string chatInput = "";
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
		GUI.SetNextControlName("ChatField");
		chatInput = GUILayout.TextField(chatInput);
		if(Input.GetButtonDown("Send")){
			//Debug.Log (GUI.GetNameOfFocusedControl());
		
			if(GUI.GetNameOfFocusedControl()==""){
				GUI.FocusControl("ChatField");
				
			}



		}
		if (GUILayout.Button("Send", GUILayout.Height(17))){
	
		
			if(GUI.GetNameOfFocusedControl()=="ChatField"){
				GUI.FocusControl("");
				if(chatInput!=""){
					AddMessage(chatInput);
					chatInput ="";
				}
				
			}	
			

		}

		GUILayout.EndArea();
	}

	void AddMessage(string Message){

		photonview.RPC("RPCAddToChat",PhotonTargets.All,Message, localPlayer.team, localPlayer.GetUid (), localPlayer.GetName());
	}
	[RPC]
	public void RPCAddToChat(string message,int team,string uid,string name){
		ChatMessage mess = new ChatMessage ();
		mess.message = message;
		mess.playerName = name;
		mess.uid = uid;
		mess.team = team;
		messages.Enqueue (mess);

	}

}
