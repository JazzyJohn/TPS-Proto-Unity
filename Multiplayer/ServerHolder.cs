using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using Sfs2X.Logging;


public enum GAMEMODE { PVP, PVE,RUNNER };

public class RoomData
{
    public string name;
    public int id;
    public int playerCount;
    public int maxPlayers;
}

public class ServerHolder : MonoBehaviour 
{
	public bool shouldLoad = true;
	int number = 0;
	
	Vector2 scroll;
	Vector2 mapScroll;

	public bool createRoom = false;
	public bool connectingToRoom = false;

    public string map = "kaspi_map_c_2_test";
	string playerName;
	public string newRoomName;
	public int newRoomMaxPlayers;
    public int newPVERoomMaxPlayers;
    public int newRunnerRoomMaxPlayers;
	private const float FLOAT_COEF =100.0f;

    public GAMEMODE DefaultGameMode;

    public List<RoomData> allRooms = new List<RoomData>();
    
    private  Sfs2X.Entities.Room _gameRoom;
    
    public Sfs2X.Entities.Room gameRoom{
        get { 
            if(_gameRoom==null){
                _gameRoom = NetworkController.smartFox.LastJoinedRoom;
            }
            return _gameRoom;
        }   
        set{
            _gameRoom = value;
        }
    }

	public string version;
	public struct LoadProgress{
		public int allLoader;
		public int finishedLoader;
		public float curLoader;
		
	}

    public string ExtName = "fps";  // The server extension we work with
    public string ExtClass = "dk.fullcontrol.fps.FpsExtension"; // The class name of the extension
 

	public static LoadProgress progress= new LoadProgress();


	// Use this for initialization
    public void Connect()
	{


        NetworkController.smartFox.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
        NetworkController.smartFox.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
        NetworkController.smartFox.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnCreateRoomError);
        NetworkController.smartFox.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        NetworkController.smartFox.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserLeaveRoom);
        NetworkController.smartFox.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
        NetworkController.smartFox.AddEventListener(SFSEvent.ROOM_REMOVE, OnRoomDeleted);

        SetupRoomList();

	}

    //SMART FOX LOGIC
    void OnPublicMessage(BaseEvent evt) {
		string message = (string)evt.Params["message"];
		User sender = (User)evt.Params["sender"];
	
	}

	void OnJoinRoom(BaseEvent evt) {

        
      
      Sfs2X.Entities.Room room = (Sfs2X.Entities.Room)evt.Params["room"];
      NetworkController.Instance.pause = true;
      gameRoom = room;
	
      if (room == null)
      {
          return; 
      }
    	// If we joined a game room, then we either created it (and auto joined) or manually selected a game to join
      if (shouldLoad)
      {

          StartCoroutine(LoadMap(room.GetVariable("map").GetStringValue()));
          
      }
      else
      {
          FinishLoad();
      }

	}

	public void OnCreateRoomError(BaseEvent evt) {
		string error = (string)evt.Params["error"];
		Debug.Log("Room creation error; the following error occurred: " + error);
	}

	public void OnUserEnterRoom(BaseEvent evt) {
		User user = (User)evt.Params["user"];
		
	}

	private void OnUserLeaveRoom(BaseEvent evt) {
		User user = (User)evt.Params["user"];
		
	}

	/*
    * Handle a new room in the room list
    */
	public void OnRoomAdded(BaseEvent evt) { //Room room) {
        Sfs2X.Entities.Room room = (Sfs2X.Entities.Room)evt.Params["room"];
       
		// Update view (only if room is game)
		if ( room.IsGame ) {
			SetupRoomList();
		}
	}


    private void SetupRoomList()
    {
        List<Sfs2X.Entities.Room> roomList = NetworkController.smartFox.RoomManager.GetRoomListFromGroup("games");
        allRooms = new List<RoomData>();
        foreach (Sfs2X.Entities.Room room in roomList)
        {
            // Show only game rooms
            if (!room.IsGame || room.IsHidden || room.IsPasswordProtected)
            {
                continue;
            }
            RoomData roomData = new RoomData();
            roomData.id = room.Id;
            roomData.name = room.Name;
            roomData.playerCount = room.UserCount;
            roomData.maxPlayers = room.MaxUsers;
            allRooms.Add(roomData);
            ///Debug.Log("Room id: " + room.Id + " has name: " + room.Name +"map" +room.GetVariable("map").GetStringValue());

        }

    }

	/*
	* Handle a room that was removed
	*/
	public void OnRoomDeleted(BaseEvent evt) { //Room room) {
		SetupRoomList();
	}

    //SMART FOX LOGIC;
    public void RoomNewName(GAMEMODE mode) {
        switch(mode){
            case GAMEMODE.PVE:
                newRoomName = "Test PVE chamber " + UnityEngine.Random.Range(100, 999);
                break;
            case GAMEMODE.PVP:
                newRoomName = "Test PVP chamber " + UnityEngine.Random.Range(100, 999);
                break;
            case GAMEMODE.RUNNER:
                newRoomName = "Runner chamber " + UnityEngine.Random.Range(100, 999);
                break;
       
        }
    }
	void Update()
	{
		/*float updateRate = 3;
		float nextUpdateTime = 0;
		
		if (!PhotonNetwork.connected)

		{
			if (Time.time - updateRate > nextUpdateTime)
				nextUpdateTime = Time.time - Time.deltaTime;
			
			while (nextUpdateTime < Time.time)
			{
				PhotonNetwork.ConnectUsingSettings(version);
				nextUpdateTime += updateRate;
			}
		}*/
        SetupRoomList();
	}
	
	void OnReceivedRoomList()
	{
		//allRooms = PhotonNetwork.GetRoomList();
		//print ("Обновлен список комнат. Сейчас их " + allRooms.Length + ".");
	}
	
	void OnReceivedRoomListUpdate()
	{
		//allRooms = PhotonNetwork.GetRoomList();
		//print ("Обновлен список комнат. Сейчас их " + allRooms.Length + ".");
	}
	
	void OnGUI()
	{
        if (!shouldLoad && gameRoom == null)
        {
            ShowConnectMenu();
        }
        /*if(!shouldLoad){
            if (!PhotonNetwork.inRoom && PhotonNetwork.connected)
            {
                float screenX = Screen.width, screenY = Screen.height;
            //	RoomInfo[] availableRooms = allRooms;
				
                float slotsizeX = screenX / 5;
                float slotsizeY = screenY / (availableRooms.Length + 1);

                GUILayout.BeginArea(new Rect (Screen.width  - 400, Screen.height - 230, 400, 230), "Соединение", GUI.skin.GetStyle("window"));
                ShowConnectMenu ();
                GUILayout.EndArea();
            }

        /*	else if (PhotonNetwork.connected)
            {
        <<<<<<< HEAD
                RoomOptions options = new RoomOptions ();
                options.maxPlayers = 10;
                PhotonNetwork.CreateRoom("My Room",options,null);
            }
            void OnCreatedRoom()
            {
                FindObjectOfType<PVPGameRule> ().StartGame ();
        =======
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
				
                if (GUILayout.Button("Покинуть комнату", GUILayout.Width (130), GUILayout.Height (25)))
                {
                    PhotonNetwork.LeaveRoom();
                    createRoom = false;
                }
				
                GUILayout.EndHorizontal();
        >>>>>>> pr/6
            }

		
		
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

            if(connectingToRoom)
            {
				
                GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 20, 200, 40), "Загрузка карты... " + LoadProcent().ToString("0.0") +"%");
            }

        }	*/
    }



    void ShowConnectMenu()
    {


        if (!createRoom)
        {

            GUILayout.Space(5);

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Создать комнату", GUILayout.Width(150), GUILayout.Height(25)))
            {
                CreateNewRoom(DefaultGameMode);
            }
                
            GUILayout.EndHorizontal();




        }
    }
		
	public void JoinRoom(RoomData room){

        NetworkController.smartFox.Send(new JoinRoomRequest(room.id));
	}

    public void LeaveRoom() {
        Debug.Log("LeaveTRoom");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.ConnectUsingSettings(version);
    }
	public void CreateNewRoom(GAMEMODE mode) //Создание комноты (+)
	{
        if (map == "")
        {
            return;
        }
		/*ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps["MapName"] = map;
		string[] exposedProps = new string[customProps.Count];
		exposedProps[0] = "MapName";
       
        */
        bool isVisible = true;
        int roomCnt = newRoomMaxPlayers;
        RoomSettings settings = new RoomSettings(newRoomName);
        RoomVariable gameRule = null, maxScore = null, maxTime=null;
        settings.MaxVariables = 10;
        switch (mode)
        {
            case GAMEMODE.PVE:
                roomCnt = newPVERoomMaxPlayers;
                 gameRule =new SFSRoomVariable("ruleClass", "nstuff.juggerfall.extension.gamerule.PVEGameRule");
                 settings.Variables.Add(new SFSRoomVariable("teamCount", 2));
                 maxTime =new SFSRoomVariable("maxTime", 0);
                maxScore =new SFSRoomVariable("maxScore",0);
                break;
            case GAMEMODE.RUNNER:
                roomCnt = newRunnerRoomMaxPlayers;
                isVisible = false;
                gameRule = new SFSRoomVariable("ruleClass", "nstuff.juggerfall.extension.gamerule.RunnerGameRule");
                 maxTime =new SFSRoomVariable("maxTime", 0);
                maxScore =new SFSRoomVariable("maxScore",0);
                break;
            case GAMEMODE.PVP:
                gameRule = new SFSRoomVariable("ruleClass", "nstuff.juggerfall.extension.gamerule.PVPGameRule");
                settings.Variables.Add(new SFSRoomVariable("teamCount", 2));
                maxTime =new SFSRoomVariable("maxTime", 0);
                maxScore =new SFSRoomVariable("maxScore", 25);
                break;
        }

        settings.Variables.Add(maxScore);
        settings.Variables.Add(maxTime);
        settings.GroupId = "games";
        settings.IsGame = true;
        RoomVariable mapVar = new SFSRoomVariable("map", map);
        settings.Variables.Add(mapVar);
        RoomVariable visVar = new SFSRoomVariable("visible", isVisible);
        settings.Variables.Add(visVar);
        settings.Variables.Add(gameRule);
        settings.MaxUsers =(short)roomCnt;
        settings.MaxSpectators = 0;
        settings.Extension = new RoomExtension(ExtName, ExtClass);
        NetworkController.smartFox.Send(new CreateRoomRequest(settings, true, NetworkController.smartFox.LastJoinedRoom));

      
	}

	public void LoadNextMap(){
		connectingToRoom = true;
        PhotonNetwork.isMessageQueueRunning = false;
        Debug.Log("LOAD"+this);
        if (PhotonNetwork.offlineMode)
        {
        
            StartCoroutine(LoadMap(map));
        }
        else
        {
            StartCoroutine(LoadMap((string)PhotonNetwork.room.customProperties["MapName"]));
        }
	}
    public void LoadNextMap(string map)
    {

        StartCoroutine(LoadMap(map,true));
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
	void OnFailedToConnectToPhoton(DisconnectCause cause){
		PhotonNetwork.offlineMode = true;
	
	}
	
	void OnPhotonJoinRoomFailed()
	{
		connectingToRoom = false;
		FindObjectOfType<MainMenuGUI> ().CreateRoom ();
		print ("Не удалось подключиться к комнате.");
	}
	
	void OnPhotonCreateRoomFailed()
	{
		connectingToRoom = false;
		FindObjectOfType<MainMenuGUI> ().ShowRoomList ();
		print ("Не удалось создать комнату.");
	}
	
	void OnPhotonJoinRandomJoinFailed()
	{
		connectingToRoom = false;
		FindObjectOfType<MainMenuGUI> ().ShowRoomList ();
		print ("Не удалось подключиться к случайной комнате.");
	}
	
	void OnJoinedRoom()
	{
		print("Удалось подключиться к комнате " + PhotonNetwork.room.name);
		PhotonNetwork.isMessageQueueRunning = false;
		progress.allLoader=0;
		progress.finishedLoader=0;
		progress.curLoader=0;
		connectingToRoom = true;
		if (shouldLoad) {
            if (PhotonNetwork.offlineMode)
            {
                StartCoroutine(LoadMap(map));
            }
            else
            {
                StartCoroutine(LoadMap((string)PhotonNetwork.room.customProperties["MapName"]));
            }
		} else {
			FinishLoad ();
		}

		/*Camera.main.GetComponent<PlayerMainGui> ().enabled = true;
		PhotonNetwork.Instantiate ("Player",Vector3.zero,Quaternion.identity,0);
		if (PhotonNetwork.isMasterClient) {
			FindObjectOfType<PVPGameRule> ().StartGame ();
		}*/



	}

	public float LoadProcent() {
		if(progress.allLoader==0){
			return 0f;
		}
		return ((float)progress.finishedLoader)/progress.allLoader*100f +progress.curLoader/progress.allLoader;
	}
	IEnumerator LoadMap (string mapName,bool next =false)
	{
		AsyncOperation async;

		connectingToRoom = true;
        
		
		yield return new WaitForSeconds(1);
	


	

		Application.backgroundLoadingPriority = ThreadPriority.High;
		Debug.Log("Загружаем карту " + mapName );
		async = Application.LoadLevelAsync(mapName);
		yield return async;
		Debug.Log ("Загрузка завершена.");
		
		MapDownloader loader = FindObjectOfType<MapDownloader>();
		if (loader != null) {
				PrefabManager[] managers = FindObjectsOfType<PrefabManager> ();
				//MapLoader + MApUnity load = 2;
				
				progress.allLoader = 2+managers.Length;
				progress.finishedLoader=1;
				progress.curLoader=0;
				IEnumerator innerCoroutineEnumerator = loader.DownloadAndCache ();
				while (innerCoroutineEnumerator.MoveNext())
						yield return innerCoroutineEnumerator.Current;

				progress.finishedLoader++;
				progress.curLoader=0;
				foreach (PrefabManager manger in managers) {
						IEnumerator innerPrefabEnum = manger.DownloadAndCache ();
						while (innerPrefabEnum.MoveNext())
								yield return innerPrefabEnum.Current;
						progress.finishedLoader++;
						progress.curLoader=0;
				}
		}
		IEnumerator itemCoroutineEnumerator =ItemManager.instance.ReoadItemsSync();
		while(itemCoroutineEnumerator.MoveNext())
			yield return itemCoroutineEnumerator.Current;



      
		FinishLoad ();
		yield return new WaitForEndOfFrame();
		GameObject menu =Instantiate (loader.playerHud, Vector3.zero, Quaternion.identity) as GameObject;
		Camera.main.GetComponent<PlayerMainGui> ().enabled = true;
		menu.transform.parent = Camera.main.transform;
        menu.transform.localPosition = Vector3.zero;
        menu.transform.localRotation = Quaternion.identity;
        Debug.Log("SPawn");
        NetworkController.Instance.pause = false;
        if (next)
        {
            Player[] allPlayer = FindObjectsOfType<Player>();
            foreach (Player player in allPlayer)
            {
                player.Restart();
            }
        }
      //  NetworkController.Instance.SpawnPlayer( Vector3.zero, Quaternion.identity);
	
	}
	public void FinishLoad(){
		if(!shouldLoad){
            MapDownloader loader = FindObjectOfType<MapDownloader>();
            GameObject menu = Instantiate(loader.playerHud, Vector3.zero, Quaternion.identity) as GameObject;

            menu.transform.parent = Camera.main.transform;
            menu.transform.localPosition = Vector3.zero;
            menu.transform.localRotation = Quaternion.identity;
            Camera.main.GetComponent<PlayerMainGui>().enabled = true;

            NetworkController.Instance.pause = false;
		}
		connectingToRoom = false;
		
		MainMenuGUI mainMenu = FindObjectOfType<MainMenuGUI> ();
		if (mainMenu != null) {
				Destroy (mainMenu.gameObject);
		}
		
	
	}

	public static Vector3 ReadVectorFromShort(PhotonStream stream){
		Vector3 newPosition = Vector3.zero;
	//Debug.Log (stream.ReceiveNext ());
	newPosition.x = ((int)stream.ReceiveNext())/FLOAT_COEF;
	//Debug.Log (newPosition.x);
    newPosition.y = ((int)stream.ReceiveNext()) / FLOAT_COEF;
    newPosition.z = ((int)stream.ReceiveNext()) / FLOAT_COEF;
		return newPosition;
	}
	public static void WriteVectorToShort(PhotonStream stream,Vector3 vect){

        stream.SendNext((int)(vect.x * FLOAT_COEF));
        stream.SendNext((int)(vect.y * FLOAT_COEF));
        stream.SendNext((int)(vect.z * FLOAT_COEF));
		
	}
	








}
