using UnityEngine;
using System.Collections;

public class ServerHolder : MonoBehaviour 
{
	int number = 0;
	
	Vector2 scroll;
	Vector2 mapScroll;
	
	bool createRoom = false;
	bool connectingToRoom = false;
	
	string playerName;
	string newRoomName;
	int newRoomMaxPlayers;
	
	RoomInfo[] allRooms;
	
	// Use this for initialization
	void Start()
	{
		PhotonNetwork.autoJoinLobby = true;
		PhotonNetwork.ConnectUsingSettings("0.1");
		
		allRooms = PhotonNetwork.GetRoomList();
		newRoomMaxPlayers = 64;
		newRoomName = "Test chamber " + Random.Range(100, 999);
		
	}
	
	void Update()
	{
		float updateRate = 3;
		float nextUpdateTime = 0;
		
		if (!PhotonNetwork.connected)
		{
			if (Time.time - updateRate > nextUpdateTime)
				nextUpdateTime = Time.time - Time.deltaTime;
			
			while (nextUpdateTime < Time.time)
			{
				PhotonNetwork.ConnectUsingSettings("0.1");
				nextUpdateTime += updateRate;
			}
		}
	}
	
	void OnReceivedRoomList()
	{
		allRooms = PhotonNetwork.GetRoomList();
		print ("Обновлен список комнат. Сейчас их " + allRooms.Length + ".");
	}
	
	void OnReceivedRoomListUpdate()
	{
		allRooms = PhotonNetwork.GetRoomList();
		print ("Обновлен список комнат. Сейчас их " + allRooms.Length + ".");
	}
	
	void OnGUI()
	{
		float screenX = Screen.width, screenY = Screen.height;
		RoomInfo[] availableRooms = allRooms;
		
		float slotsizeX = screenX / 5;
		float slotsizeY = screenY / (availableRooms.Length + 1);
		
		if (!PhotonNetwork.inRoom && PhotonNetwork.connected)
		{
			GUILayout.BeginArea(new Rect (Screen.width / 2 - 250, Screen.height/2 - 150, 500, 330), "Соединение", GUI.skin.GetStyle("window"));
			ShowConnectMenu ();
			GUILayout.EndArea();
		}
		
		/*
		else if (PhotonNetwork.connected)
		{
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			
			if (GUILayout.Button("Покинуть комнату", GUILayout.Width (130), GUILayout.Height (25)))
			{
				PhotonNetwork.LeaveRoom();
				createRoom = false;
			}
			
			GUILayout.EndHorizontal();
		}
		*/
		
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		
	}
	
	void ShowConnectMenu()
	{
		GUILayout.Space(10);
		
		if (!PhotonNetwork.inRoom)
		{
			if (!createRoom)
			{
				scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(480), GUILayout.Height(225));
				
				if (allRooms.Length > 0)
				{
					foreach (RoomInfo room in allRooms)
					{
						GUILayout.BeginHorizontal("box");
						GUILayout.Label (room.name, GUILayout.Width(150));
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("Войти", GUILayout.Width (100)))
						{
							PhotonNetwork.JoinRoom(room.name);
							connectingToRoom = true;
						}
						GUILayout.EndHorizontal();
					}
				}
				
				if (allRooms.Length == 0)
					GUILayout.Label("Нет доступных комнат.");
				
				GUILayout.EndScrollView();
				GUILayout.Space(5);
				
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				
				if (GUILayout.Button("Создать комнату", GUILayout.Width(150), GUILayout.Height(25)))
					createRoom = true;
				
				GUILayout.EndHorizontal();
			}
			
			else
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Создание новой комнаты.", GUILayout.Width(130));
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Название комнаты: ", GUILayout.Width(130));
				newRoomName = GUILayout.TextField(newRoomName, 30, GUILayout.Height(25));
				GUILayout.EndHorizontal();
				
				GUILayout.FlexibleSpace();
				
				GUILayout.BeginHorizontal();
				
				if (GUILayout.Button("Отмена", GUILayout.Width (150), GUILayout.Height (25)))
					createRoom = false;
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button("Создать", GUILayout.Width (150), GUILayout.Height (25)))
				{
					PhotonNetwork.CreateRoom(newRoomName, true, true, newRoomMaxPlayers);
				}
				
				GUILayout.EndHorizontal();
				
			}
		}
	}
	
	void OnJoinedLobby()
	{
		print ("Мы в лобби.");
		
	}
	
	void OnConnectedToPhoton()
	{
		print ("Мы подключились к Photon.");
		
		if (PhotonNetwork.room != null)
			PhotonNetwork.LeaveRoom();
		
		connectingToRoom = false;
	}
	
	
	void OnPhotonJoinRoomFailed()
	{
		connectingToRoom = false;
		print ("Не удалось подключиться к комнате.");
	}
	
	void OnPhotonCreateRoomFailed()
	{
		connectingToRoom = false;
		print ("Не удалось создать комнату.");
	}
	
	void OnPhotonJoinRandomJoinFailed()
	{
		connectingToRoom = false;
		print ("Не удалось подключиться к случайной комнате.");
	}
	
	void OnJoinedRoom()
	{
		print("Удалось подключиться к комнате " + PhotonNetwork.room.name);
		connectingToRoom = true;
		
		Camera.main.GetComponent<PlayerMainGui> ().enabled = true;
		PhotonNetwork.Instantiate ("Player",Vector3.zero,Quaternion.identity,0);
	}
}
