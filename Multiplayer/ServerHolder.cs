using UnityEngine;
using System.Collections;


	
public class ServerHolder : MonoBehaviour 
{
	public bool shouldLoad = true;
	int number = 0;
	
	Vector2 scroll;
	Vector2 mapScroll;

	public bool createRoom = false;
	public bool connectingToRoom = false;
	
	string playerName;
	public string newRoomName;
	public int newRoomMaxPlayers;
	private const float FLOAT_COEF =100.0f;

	public RoomInfo[] allRooms;
	public string version;
	public struct LoadProgress{
		public int allLoader;
		public int finishedLoader;
		public float curLoader;
		
	}
	public static LoadProgress progress= new LoadProgress();


	// Use this for initialization
	void Start()
	{

		if (PhotonNetwork.inRoom) {
			if(PhotonNetwork.isMasterClient){
				FindObjectOfType<PVPGameRule> ().StartGame ();

			}
			Camera.main.GetComponent<PlayerMainGui> ().enabled = true;

			PhotonNetwork.Instantiate ("Player", Vector3.zero, Quaternion.identity, 0);

		} else {
			PhotonNetwork.autoJoinLobby = true;
			PhotonNetwork.ConnectUsingSettings(version);
			
			allRooms = PhotonNetwork.GetRoomList();
			newRoomMaxPlayers = 10;
			newRoomName = "Test chamber " + Random.Range(100, 999);
		}
		
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
				PhotonNetwork.ConnectUsingSettings(version);
				nextUpdateTime += updateRate;
			}
		}
	}
	
	void OnReceivedRoomList()
	{
		allRooms = PhotonNetwork.GetRoomList();
		//print ("Обновлен список комнат. Сейчас их " + allRooms.Length + ".");
	}
	
	void OnReceivedRoomListUpdate()
	{
		allRooms = PhotonNetwork.GetRoomList();
		//print ("Обновлен список комнат. Сейчас их " + allRooms.Length + ".");
	}
	
	void OnGUI()
	{

		if(!shouldLoad){
			if (!PhotonNetwork.inRoom && PhotonNetwork.connected)
			{
				float screenX = Screen.width, screenY = Screen.height;
				RoomInfo[] availableRooms = allRooms;
				
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

			
		*/
			GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

			if(connectingToRoom)
			{
				
				GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 20, 200, 40), "Загрузка карты... " + LoadProcent().ToString("0.0") +"%");
			}

		}
	}
	

	
	void ShowConnectMenu()
	{
				GUILayout.Space (10);

		if (!PhotonNetwork.inRoom)
		{

			if (!createRoom)
			{
				scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(200), GUILayout.Height(150));

				
								if (allRooms.Length > 0) {
										foreach (RoomInfo room in allRooms) {
												GUILayout.BeginHorizontal ("box");
												GUILayout.Label (room.name +"  "+  room.playerCount + " / " +room.maxPlayers, GUILayout.Width (150));
												GUILayout.FlexibleSpace ();
					
												if( room.playerCount<newRoomMaxPlayers){
													if (GUILayout.Button ("Войти", GUILayout.Width (100))) {
																
																				
															PhotonNetwork.JoinRoom (room.name);
															connectingToRoom = true;
													}
												}
												GUILayout.EndHorizontal ();
										}
								}
				
								if (allRooms.Length == 0)
										GUILayout.Label ("Нет доступных комнат.");
				
								GUILayout.EndScrollView ();
								GUILayout.Space (5);
				
								GUILayout.FlexibleSpace ();
								GUILayout.BeginHorizontal ();
								GUILayout.FlexibleSpace ();
				
								if (GUILayout.Button ("Создать комнату", GUILayout.Width (150), GUILayout.Height (25)))
										createRoom = true;
				
								GUILayout.EndHorizontal ();
						} else {
								GUILayout.BeginHorizontal ();
								GUILayout.Label ("Создание новой комнаты.", GUILayout.Width (130));
								GUILayout.EndHorizontal ();
				
								GUILayout.BeginHorizontal ();
								GUILayout.Label ("Название комнаты: ", GUILayout.Width (130));
								newRoomName = GUILayout.TextField (newRoomName, 30, GUILayout.Height (25));
								GUILayout.EndHorizontal ();
				
								GUILayout.FlexibleSpace ();
				
								GUILayout.BeginHorizontal ();
				

						if (GUILayout.Button("Отмена", GUILayout.Width (150), GUILayout.Height (25)))
							createRoom = false;
						GUILayout.FlexibleSpace ();
						if (GUILayout.Button("Создать", GUILayout.Width (150), GUILayout.Height (25)))
						{
							ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
							customProps["MapName"] = "kaspi_map_c_2_test";
							string[] exposedProps = new string[customProps.Count];
							exposedProps[0] = "MapName";

							PhotonNetwork.CreateRoom(newRoomName, true, true, newRoomMaxPlayers, customProps, exposedProps);
						}

				
								GUILayout.EndHorizontal ();
				

			
						}
				}


		}
		
	public void JoinRoom(string room = ""){
			if(room==""){
				PhotonNetwork.JoinRandomRoom();
			}else{
									
				PhotonNetwork.JoinRoom (room);
			}
			connectingToRoom = true;
	}
		
		
	public void CreateNewRoom() //Создание комноты (+)
	{
		ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
		customProps["MapName"] = "kaspi_map_c_2_test";
		string[] exposedProps = new string[customProps.Count];
		exposedProps[0] = "MapName";
		
		PhotonNetwork.CreateRoom(newRoomName, true, true, newRoomMaxPlayers, customProps, exposedProps);
	}

	public void LoadNextMap(){
		connectingToRoom = true;
		StartCoroutine (LoadMap ((string)PhotonNetwork.room.customProperties ["MapName"]));
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
		FindObjectOfType<MainMenuGUI> ().CreateRoom ();
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
		PhotonNetwork.isMessageQueueRunning = false;
		progress.allLoader=0;
		progress.finishedLoader=0;
		progress.curLoader=0;
		connectingToRoom = true;
		if (shouldLoad) {
		
			StartCoroutine (LoadMap ((string)PhotonNetwork.room.customProperties ["MapName"]));
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
	IEnumerator LoadMap (string mapName)
	{
		AsyncOperation async;

		connectingToRoom = true;
		PhotonNetwork.DestroyPlayerObjects 	( PhotonNetwork.player);
		

		
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

			
		PhotonResourceWrapper.TryToReload();
		PhotonNetwork.isMessageQueueRunning = true;

		FinishLoad ();
		yield return new WaitForEndOfFrame();
		GameObject menu =Instantiate (loader.playerHud, Vector3.zero, Quaternion.identity) as GameObject;
		Camera.main.GetComponent<PlayerMainGui> ().enabled = true;
		menu.transform.parent = Camera.main.transform;
		PhotonNetwork.Instantiate ("Player",Vector3.zero,Quaternion.identity,0);
	
	}
	public void FinishLoad(){
		if(!shouldLoad){
			Camera.main.GetComponent<PlayerMainGui> ().enabled = true;
			PhotonNetwork.Instantiate ("Player",Vector3.zero,Quaternion.identity,0);
		}
		connectingToRoom = false;
		if (PhotonNetwork.isMasterClient) 
		{
			FindObjectOfType<PVPGameRule> ().StartGame ();
		}
		MainMenuGUI mainMenu = FindObjectOfType<MainMenuGUI> ();
		if (mainMenu != null) {
				Destroy (mainMenu.gameObject);
		}
		
	
	}

	public static Vector3 ReadVectorFromShort(PhotonStream stream){
		Vector3 newPosition = Vector3.zero;
	//Debug.Log (stream.ReceiveNext ());
	newPosition.x = ((short)stream.ReceiveNext())/FLOAT_COEF;
	//Debug.Log (newPosition.x);
		newPosition.y = ((short)stream.ReceiveNext())/FLOAT_COEF;
		newPosition.z = ((short)stream.ReceiveNext())/FLOAT_COEF;
		return newPosition;
	}
	public static void WriteVectorToShort(PhotonStream stream,Vector3 vect){
		
		stream.SendNext((short)(vect.x*FLOAT_COEF));
		stream.SendNext((short)(vect.y*FLOAT_COEF));
		stream.SendNext((short)(vect.z*FLOAT_COEF));
		
	}
	void OnMasterClientSwitched( PhotonPlayer newMaster )
	{
		//TODO: director fix
	
		if (PhotonNetwork.isMasterClient) {
			FindObjectOfType<PVPGameRule> ().StartGame ();	
		}
	}


}
