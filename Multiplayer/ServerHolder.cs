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
using Sfs2X.Entities.Data;
using nstuff.juggerfall.extension.models;


public enum GAMEMODE { PVP, PVE,RUNNER, PVPJUGGERFIGHT,PVPHUNT,PVE_HOLD};

public class RoomData
{
    public string name;
    public int id;
    public int playerCount;
    public int maxPlayers;
}
[Serializable]
public class MapData
{
    public string name;
    public string version;
	public string weaponAsset;
	public string characterAsset;
    public GameObject playerHud;
	public GAMEMODE gameMode= GAMEMODE.PVP;
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

    public MapData[] allMaps;

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
        NetworkController.smartFox.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomError);
        NetworkController.smartFox.AddEventListener(SFSEvent.ROOM_REMOVE, OnRoomDeleted);
       
        SetupRoomList();

	}
      
    //SMART FOX LOGIC
    void OnPublicMessage(BaseEvent evt) {
		string message = (string)evt.Params["message"];
		User sender = (User)evt.Params["sender"];
	
	}

	void OnJoinRoom(BaseEvent evt) {


        blockQuickStart = false;
      Sfs2X.Entities.Room room = (Sfs2X.Entities.Room)evt.Params["room"];
	  if(!room.IsGame){
			return;
	  }
      NetworkController.Instance.pause = true;
	  
      gameRoom = room;
	
      if (room == null)
      {
          return; 
      }
      CancelInvoke("RetryRoomCreate");	
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
		MainMenuGUI mainMenu = FindObjectOfType<MainMenuGUI> ();
		if(mainMenu!=null){
            mainMenu.CreateRoom();
		}
        blockQuickStart = false;
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
    public void OnRoomError(BaseEvent evt)
    { //Room room) {
        Debug.Log(evt.Params["errorMessage"]);
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
            case GAMEMODE.PVPJUGGERFIGHT:
                 newRoomName = "Jugger Fight chamber " + UnityEngine.Random.Range(100, 999);
                break;
			case GAMEMODE.PVPHUNT:
                 newRoomName = "Hunt chamber " + UnityEngine.Random.Range(100, 999);
                break;
			case GAMEMODE.PVE_HOLD:
				newRoomName = "Hold chamber " + UnityEngine.Random.Range(100, 999);
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
            if (GUILayout.Button("Войти в комнату", GUILayout.Width(150), GUILayout.Height(25)))
            {
                JoinRoom(allRooms[0]);
            }

                
            GUILayout.EndHorizontal();




        }
    }
		
	public void JoinRoom(RoomData room){

        NetworkController.smartFox.Send(new JoinRoomRequest(room.id));
	}

    GAMEMODE lastMode;
    bool blockQuickStart = false;
    public void CreateNewRoom() //Создание комноты (+)
    {
        if (blockQuickStart)
        {
            return;
        }
        map = allMaps[UnityEngine.Random.Range(0,allMaps.Length)].name;
        blockQuickStart = true;
        newRoomName = map +UnityEngine.Random.Range(1000,10000).ToString();
        CreateNewRoom(allMaps[UnityEngine.Random.Range(0,allMaps.Length)].gameMode);
       
    }


	public void CreateNewRoom(GAMEMODE mode) //Создание комноты (+)
	{
        if (map == "")
        {
            Debug.Log("emptyMap");
			MainMenuGUI mainMenu = FindObjectOfType<MainMenuGUI> ();
			if (mainMenu != null) {
				mainMenu.SetMessage("Не выбрана карта");
				mainMenu. CreateRoom();
			}
            return;
        }
		/*ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps["MapName"] = map;
		string[] exposedProps = new string[customProps.Count];
		exposedProps[0] = "MapName";
       
        */
		lastMode= mode;
        bool isVisible = true;
        int roomCnt = newRoomMaxPlayers;
        RoomSettings settings = new RoomSettings(newRoomName);
        RoomVariable gameRule = null;
        settings.MaxVariables = 10;
		GameSettingModel setting = new GameSettingModel();
		setting.isWithPractice = false;
        switch (mode)
        {
            case GAMEMODE.PVE:
                roomCnt = newPVERoomMaxPlayers;
                gameRule =new SFSRoomVariable("ruleClass", "nstuff.juggerfall.extension.gamerule.PVEGameRule");
                setting.teamCount = 2;
                setting.maxTime =0;
                setting.maxScore= 0;
                break;
            case GAMEMODE.RUNNER:
                roomCnt = newRunnerRoomMaxPlayers;
                isVisible = false;
                gameRule = new SFSRoomVariable("ruleClass", "nstuff.juggerfall.extension.gamerule.RunnerGameRule");
                setting.maxTime =0;
                setting.maxScore= 0;
                break;
            case GAMEMODE.PVP:
                gameRule = new SFSRoomVariable("ruleClass", "nstuff.juggerfall.extension.gamerule.PVPGameRule");
                setting.teamCount = 2;
                setting.maxTime =0;
                setting.maxScore= 10;
                break;
            case GAMEMODE.PVPJUGGERFIGHT:
                gameRule = new SFSRoomVariable("ruleClass", "nstuff.juggerfall.extension.gamerule.PVPJuggerFightGameRule");
                setting.teamCount = 2;
                setting.maxTime =0;
                setting.maxScore= 25;
                break;
			case GAMEMODE.PVPHUNT:
				gameRule = new SFSRoomVariable("ruleClass", "nstuff.juggerfall.extension.gamerule.HuntGameRule");
                setting.teamCount = 2;
                setting.maxTime =0;
                setting.maxScore= 500;
            
               // Debug.Log(GlobalGameSetting.instance.GetHuntScoreTable());
				//data.Put("huntTable",new SFSDataWrapper(5,GlobalGameSetting.instance.GetHuntScoreTable()));				
             
				
               
			break;
			case GAMEMODE.PVE_HOLD:
				gameRule = new SFSRoomVariable("ruleClass", "nstuff.juggerfall.extension.gamerule.PVEHoldGameRule");
                setting.teamCount = 2;
                setting.maxTime =0;
                setting.maxScore= 500;
            
               // Debug.Log(GlobalGameSetting.instance.GetHuntScoreTable());
				//data.Put("huntTable",new SFSDataWrapper(5,GlobalGameSetting.instance.GetHuntScoreTable()));				
             
				
               
			break;
        }
        SFSObject data = new SFSObject();
		data.PutClass("gameSetting",setting);
        settings.Variables.Add(new SFSRoomVariable("gameVar", data));
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
        Debug.Log(map);
        NetworkController.smartFox.Send(new CreateRoomRequest(settings, true, NetworkController.smartFox.LastJoinedRoom));
		Invoke("RetryRoomCreate", 10);
      
	}

	
    public void LoadNextMap(string map)
    {

        StartCoroutine(LoadMap(map,true));
    }
	
	public float LoadProcent() {
		if(progress.allLoader==0){
			return 0f;
		}
		return ((float)progress.finishedLoader)/progress.allLoader*100f +progress.curLoader/progress.allLoader;
	}
	public void RetryRoomCreate(){
		CreateNewRoom(lastMode);
		
	}
	
	
	IEnumerator LoadMap (string mapName,bool next =false)
	{
		//AsyncOperation async;
        progress.allLoader = 4;
        progress.finishedLoader = 1;
        progress.curLoader = 0;
		connectingToRoom = true;
        AfterGameBonuses.Clear();
		
		MainMenuGUI mainMenu = FindObjectOfType<MainMenuGUI> ();
		GameObject loadingScreen = null; 
		if (mainMenu != null) {
				Destroy (mainMenu.gameObject);
				ItemManager.instance.ClearShop();
				loadingScreen = Instantiate(mainMenu.loadingScreen, Vector3.zero, Quaternion.identity) as GameObject;
             

		}
        if (Camera.main != null)
        {
            PlayerMainGui oldmap = Camera.main.GetComponent<PlayerMainGui>();
            if (oldmap != null)
            {
                oldmap.enabled = false;
            }
        }
		yield return new WaitForSeconds(1);





        Application.backgroundLoadingPriority = ThreadPriority.High;
	/*	Debug.Log("Загружаем карту " + mapName );
		async = Application.LoadLevelAsync(mapName);
		yield return async;
		Debug.Log ("Загрузка завершена.");*/
        MapData data = null;
        foreach(MapData iterdata in allMaps){
            if (iterdata.name == mapName)
            {
                data = iterdata;
                break; 
            }
        }
       
            MapLoader loader = FindObjectOfType<MapLoader>();
            Debug.Log(loader);
            if (loader != null)
            {

                //MapLoader + MApUnity load = 2;

                
                Debug.Log("Загружаем карту " + mapName);
                IEnumerator innerCoroutineEnumerator;
                if (data != null)
                {
                    bool skipWeapon = false, skipPawn = false;
                    IEnumerator tempEnumerator;
                    if (loader.IsInCache(data.weaponAsset))
                    {
                        tempEnumerator = loader.DownloadAndCache(data.weaponAsset, AssetBundleType.WEAPON);
                        while (tempEnumerator.MoveNext())
                        {
                            yield return tempEnumerator.Current;
                        }
                        progress.finishedLoader++;
                        skipWeapon = true;
                    }
                    if (loader.IsInCache(data.characterAsset))
                    {
                         tempEnumerator = loader.DownloadAndCache(data.characterAsset, AssetBundleType.PAWN);
                        while (tempEnumerator.MoveNext())
                        {
                            yield return tempEnumerator.Current;
                        }
                        progress.finishedLoader++;
                        skipPawn = true;
                    }
                    
                    tempEnumerator = loader.Load(mapName, data.version);
                    while (tempEnumerator.MoveNext())
                    {
                        yield return tempEnumerator.Current;
                    }
                    progress.finishedLoader++;
                    if (!skipPawn)
                    {
                         tempEnumerator = loader.DownloadAndCache(data.characterAsset, AssetBundleType.PAWN);
                         while (tempEnumerator.MoveNext())
                        {
                            yield return tempEnumerator.Current;
                        }
                         progress.finishedLoader++;

                    }
                    if (!skipWeapon)
                    {
                         tempEnumerator = loader.DownloadAndCache(data.weaponAsset, AssetBundleType.WEAPON);
                         while (tempEnumerator.MoveNext())
                        {
                            yield return tempEnumerator.Current;
                        }
                         progress.finishedLoader++;

                    }
                }
                else
                {
                    innerCoroutineEnumerator = loader.Load(mapName);

                    while (innerCoroutineEnumerator.MoveNext())
                    {
                        yield return innerCoroutineEnumerator.Current;
                    }


                    PrefabManager[] managers = FindObjectsOfType<PrefabManager>();
                    progress.allLoader = 2 + managers.Length;
                    Debug.Log("Загрузка завершена.");

                    progress.finishedLoader++;
                    progress.curLoader = 0;
                    foreach (PrefabManager manger in managers)
                    {
                        IEnumerator innerPrefabEnum = manger.DownloadAndCache();
                        while (innerPrefabEnum.MoveNext())
                            yield return innerPrefabEnum.Current;
                        progress.finishedLoader++;
                        progress.curLoader = 0;
                    }
                }

            }
        ItemManager.instance.ConnectToPrefab();

     
		if(loadingScreen!=null){
			Destroy(loadingScreen);
		}
      
		FinishLoad ();
		
		yield return new WaitForEndOfFrame();
        GameObject menu;
        if (data != null)
        {
            menu = Instantiate(data.playerHud, Vector3.zero, Quaternion.identity) as GameObject;
        }
        else
        {
            MapDownloader downloadata = FindObjectOfType<MapDownloader>();
            menu = Instantiate(downloadata.playerHud, Vector3.zero, Quaternion.identity) as GameObject;
        }
      
		Camera.main.GetComponent<PlayerMainGui> ().enabled = true;
		//menu.transform.parent = Camera.main.transform;
        menu.transform.localPosition = Vector3.zero;
        menu.transform.localRotation = Quaternion.identity;
        Debug.Log("Compileted Load Map");
        
        if (next)
        {
            Player[] allPlayer = FindObjectsOfType<Player>();
            foreach (Player player in allPlayer)
            {
                player.Restart();
            }
            if (NetworkController.IsMaster())
            {
                NetworkController.Instance.MasterViewUpdate();
                NetworkController.Instance.SendMapData();
            }
        }
		ChatHolder[] chats = FindObjectsOfType<ChatHolder> ();
		foreach(ChatHolder chat in chats){
			if(chat.isGameChat){
				chat.myRoom = gameRoom;
				break;
			}
		}
        NetworkController.Instance.pause = false;
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
		
		
        AITargetManager.Reload();
        TimeManager.Instance.Init();
	}

	








}
