using UnityEngine;
using System.Collections;

using System;
using SmartFoxClientAPI;
using SmartFoxClientAPI.Data


public class NetworkController : MonoBehaviour {
	public static int MAX_VIEW_IDS;
	
	private static SmartFoxClient smartFoxClient;
	
	public static SmartFoxClient GetClient() {
		return SmartFox.Connection;
	}
	
	#region Events
	
	public static bool started = false;
	
	private void SubscribeEvents() {
		SFSEvent.onJoinRoom += OnJoinRoom;
		SFSEvent.onUserEnterRoom += OnUserEnterRoom;
		SFSEvent.onUserLeaveRoom += OnUserLeaveRoom;
		SFSEvent.onObjectReceived += OnObjectReceived;
		SFSEvent.onPublicMessage += OnPublicMessage;
    }
	
	private void UnsubscribeEvents() {
		SFSEvent.onJoinRoom -= OnJoinRoom;
		SFSEvent.onUserEnterRoom -= OnUserEnterRoom;
		SFSEvent.onUserLeaveRoom -= OnUserLeaveRoom;
		SFSEvent.onObjectReceived -= OnObjectReceived;
		SFSEvent.onPublicMessage -= OnPublicMessage;
	}
	
	void FixedUpdate() {
		if (started) {
			smartFoxClient.ProcessEventQueue();
		}
	}
	
	#endregion Events
		// We start working from here
	void Awake() {
		Application.runInBackground = true; // Let the application be running whyle the window is not active.
		smartFoxClient = GetClient();
		if (smartFoxClient==null) {
			Application.LoadLevel("login");
			return;
		}	
		SubscribeEvents();
		started = true;
	}
	
}