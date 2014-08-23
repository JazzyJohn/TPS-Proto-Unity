using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuGUI : MonoBehaviour {

	int numMessage=0;
	int numMessageOld;
	private ServerHolder Server;

	[HideInInspector] 
	public GameObject ActivBut;
	
	public RoomsNgui _RoomsNgui;

	public PanelsNgui _PanelsNgui;

	public List<Chat> _chat;

	public Class1 ChatComponent;

	public Class2 SizeScreen;

	public PlayerComponent _PlayerComponent;

	public PlayerInfo _playerInfo; 

	public Timer _timer;

	public Dictionary<string, string> Rooms = new Dictionary<string, string>();

	public UIRect[] MainPanels;

    public GAMEMODE gameMode = GAMEMODE.PVP;

	void Awake(){
		HideAllPanel();
		DontDestroyOnLoad(transform.gameObject);
	}

	// Use this for initialization
	void Start () 
	{

		_PanelsNgui.SliderPanel.alpha = 1f;

        Screen.lockCursor = false;
		//Поправить размер формы
		ReSize();
        _playerInfo.Player = FindObjectOfType<GlobalPlayer>();
		//Получение с сервера комнат
		Server = _playerInfo.Player.GetComponent<ServerHolder>();
        

		if (Server.allRooms != null) {
            foreach (RoomData room in Server.allRooms)
            {
								GameObject NewRoom = Instantiate (_RoomsNgui.ShablonRoom) as GameObject;
								NewRoom.transform.parent = _RoomsNgui.AllRoom.transform;
								NewRoom.name = room.name;
								NewRoom.GetComponent<AnyRoom> ().Name.text = room.name;
								NewRoom.GetComponent<AnyRoom> ().shablon = false;
								NewRoom.GetComponent<AnyRoom> ().SizeRoom.text = room.playerCount + " / " + room.maxPlayers;
								NewRoom.transform.localScale = new Vector3 (1f, 1f, 1f);

								_RoomsNgui.Grid.Reposition ();
								_RoomsNgui.ScrollBar.barSize = 0;
						}
				}
	}
	public void HideAllPanel(){
		foreach(UIRect panel in MainPanels){
			panel.alpha=0.0f;

		}


	}

	void  LoadData() //Задержка в получении и записи значений игрока
	{

		GetPlayerInfo();
		SetPlayerInfoInGUI();
	}

	public void PlayBut() //Вход или создание новой комнаты
	{
		Debug.Log ("PLAY");
		if (ActivBut != null) {
						switch (ActivBut.GetComponent<AnyRoom> ()._TypeRoom) {
						case AnyRoom.TypeRoom.JoinRoom:
                                foreach (RoomData room in Server.allRooms)
                                {
										if (room.name == ActivBut.name && room.playerCount < Server.newRoomMaxPlayers) {
												//PhotonNetwork.JoinRoom (room.name);
												Server.JoinRoom(room);
												HideAllPanel();
												_RoomsNgui.Loading.alpha = 1f;
                                                _PanelsNgui.SliderPanel.alpha = 1f;

										}
								}
								break;
						}

		} else {
			if(Server.allRooms.Count>0){
                Server.JoinRoom(Server.allRooms[UnityEngine.Random.Range(0,Server.allRooms.Count)]);
				HideAllPanel();
				_RoomsNgui.Loading.alpha = 1f;
                _PanelsNgui.SliderPanel.alpha = 1f;
			}else{
			
				if (_RoomsNgui.CreateRoom.alpha >0f) {
					HideAllPanel();
					_PanelsNgui.SliderPanel.alpha= 1f;
				} else {
					HideAllPanel();
					_RoomsNgui.CreateRoom.alpha = 1f;
				}
				_RoomsNgui.NameNewRoom.value = Server.newRoomName;
			}


		}

	}
	public void CreateRoom(){
		HideAllPanel ();
		_RoomsNgui.CreateRoom.alpha = 1f;

		
		_RoomsNgui.NameNewRoom.value = Server.newRoomName;

	}

	public void ToogleMode(int mode) {
		if (_RoomsNgui.CreateRoom.alpha == 1.0f)
		{
			gameMode = (GAMEMODE)mode;
			Server.RoomNewName((GAMEMODE)mode);
			_RoomsNgui.NameNewRoom.value = Server.newRoomName;
		}
	}
	public void ToggleMap(string mapName) {
		if (_RoomsNgui.CreateRoom.alpha == 1.0f)
		{
			Server.map = mapName;
			
		}
	}

	public void Loading() //Изменения процентов при загрузке(вызывает прогресс бар)
	{
		if (_RoomsNgui.Loading.alpha != 1f && ActivBut != null)
		{
			HideAllPanel();
			_RoomsNgui.Loading.alpha = 1f;
            _PanelsNgui.SliderPanel.alpha = 1f;
		}
      
		_RoomsNgui.LoadingProcent.text = (_RoomsNgui.LoadingProgress.value*100).ToString("f0") + "%";
	}

	public void StartBut() //Создать комнату
	{

       
        if (Server.map == "")
        {
            return;
        }
		Server.newRoomName = _RoomsNgui.NameNewRoom.value;
		HideAllPanel ();
		_RoomsNgui.Loading.alpha = 1f;
        _PanelsNgui.SliderPanel.alpha = 1f;
		Server.CreateNewRoom(gameMode);
	}

	public void BackBut() //Вернуться к выбору комнат
	{
		HideAllPanel ();
		_RoomsNgui.RoomsFound.alpha = 1f;

	}

	public void RefreshRoom() // Обновления списка комнат
	{
        foreach (RoomData room in Server.allRooms)
		{
			if (!Rooms.ContainsValue(room.name))
			{
				Rooms.Add(room.name, room.name);
				
				AnyRoom NewRoom = (Instantiate(_RoomsNgui.ShablonRoom) as GameObject).GetComponent<AnyRoom>();
				NewRoom.transform.parent = _RoomsNgui.AllRoom.transform;
				NewRoom.name = room.name;
				NewRoom.shablon = false;
				NewRoom.Name.text = room.name;
				NewRoom.SizeRoom.text = room.playerCount + " / " + room.maxPlayers;
				NewRoom.transform.localScale = new Vector3(1f, 1f, 1f);
				
				_RoomsNgui.Grid.Reposition();
				_RoomsNgui.ScrollBar.barSize = 0;
			}
		}
	}
	public void ShowGameList(){

	}
	public void GetPlayerInfo() // Получение значений игрока
	{

		PlayerMainGui.LevelStats lvl = LevelingManager.instance.GetPlayerStats ();
		_playerInfo.playerName = _playerInfo.Player.PlayerName;
		_playerInfo.KP = _playerInfo.Player.cash;
		_playerInfo.GITP = _playerInfo.Player.gold;
		_playerInfo.playerLvl = lvl.playerLvl;
		_playerInfo.playerExp =  LevelingManager.instance.playerExp;
		_playerInfo.playerExpNeed =  LevelingManager.instance.playerNeededExp[_playerInfo.playerLvl];
		_playerInfo.playerProcent = lvl.playerProcent;
	}

	public void SetPlayerInfoInGUI() //Запись значений игрока в ГУИ
	{
		_PlayerComponent.Name.text = _playerInfo.playerName;
		_PlayerComponent.Lvl.text = "Lvl " + _playerInfo.playerLvl;
		_PlayerComponent.Exp.text = _playerInfo.playerExp + " / " + _playerInfo.playerExpNeed;
		_PlayerComponent.ExpBar.value = _playerInfo.playerProcent /100f;
		_PlayerComponent.KP.text = _playerInfo.KP.ToString();
		_PlayerComponent.GITP.text = _playerInfo.GITP.ToString();
	}

	public void ReSize() //Правка позиции компонентов
	{


		StartCoroutine (LateFrameResize ());
	}
	public IEnumerator LateFrameResize(){
        yield return new WaitForSeconds(1.0f);
		ChatComponent.ChatLabel.transform.localPosition = new Vector3(-180, -(ChatComponent.ChatPanel.GetViewSize().y/2)+20, 0f);
		_PanelsNgui.slaiderPanel.ReSize ();
        _PanelsNgui.mainpanel.Invalidate(true);
	}

	public void FullScreen() // На весь экран
	{
		Resolution[] resolutions = Screen.resolutions;
		switch(SizeScreen.FullScreen_Z)
		{
		case true:
			SizeScreen.FullScreen_Z = false;
			Screen.SetResolution(Screen.width, Screen.height, SizeScreen.FullScreen_Z);
			ReSize();

			break;
		case false:
			SizeScreen.FullScreen_Z = true;
			Screen.SetResolution(Screen.width, Screen.height, SizeScreen.FullScreen_Z);
			ReSize();

			break;
		}
	}

	public void EnterText()// Ввод текста в чат
	{
		Chat ChatMessage = new Chat();
		numMessage++;
		if (numMessage > ChatComponent.MessageLimit)
			_chat.Remove(_chat[0]);
		ChatMessage.ID = numMessage.ToString();
		ChatMessage.name = _playerInfo.playerName;
		ChatMessage.Message = ChatComponent.TextInput.value;

		_chat.Add(ChatMessage);

		ChatComponent.TextInput.value = null;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		_timer.timer+=Time.deltaTime;
		if (_timer.timer>=_timer.TimeRefresh)
		{
			if(numMessageOld != numMessage)
			{
				string text="";
				for(int i=0; i < _chat.Count; i++)
				{
					text += "[db9b27]"+_chat[i].name+": "+"[-]"+_chat[i].Message+"\n";
				}
				ChatComponent.ChatLabel.text = text;
				_timer.timer=0;
				numMessageOld = numMessage;
			}
			RefreshRoom();
		}

		//TODO:LoadMap
		if (Server.connectingToRoom) {
			_RoomsNgui.LoadingProgress.value = Server.LoadProcent()/100f;
		}
		if (LevelingManager.instance.isLoaded) {
			LoadData();		
		
		}



	}
    

	public void scroll() //СкролБар чата
	{
		ChatComponent.ChatPanel.transform.localPosition = new Vector3(0f, -(ChatComponent.ChatScrollBar.value*1600), 0f);
	}



	public void ShowRoomList(){

		//Debug.Log (_RoomsNgui.RoomsFound.alpha);
		if (_RoomsNgui.RoomsFound.alpha >0f) {
			HideAllPanel();
			_PanelsNgui.SliderPanel.alpha= 1f;
		} else {
			HideAllPanel();
			_RoomsNgui.RoomsFound.alpha = 1f;
		}


	}
	public void AddCoins(){
		 _playerInfo.Player.AskJsForMagazine("gitp_5");
	
	
	}

}

//Группы переменных

[System.Serializable]
public class Class1
{
	public int MessageLimit;
	public UIInput TextInput;
	public UILabel ChatLabel;
	public UIPanel ChatPanel;
	public UIScrollBar ChatScrollBar;
}

[System.Serializable]
public class Class2
{
	public bool FullScreen_Z;
}

[System.Serializable]
public class Chat
{
	public string ID;
	public string name;
	public string Message;
}

[System.Serializable]
public class Timer
{
	[HideInInspector]
	public float timer;
	public float TimeRefresh;
}

[System.Serializable]
public class PlayerComponent
{
	public UILabel Lvl;
	public UILabel Name;
	public UILabel Exp;
	public UIProgressBar ExpBar;
	public UILabel KP;
	public UILabel GITP;
	public UILabel PremDate;
	public UIPanel Prem_have;
	public UIPanel Prem_DoNot_have;
}

[System.Serializable]
public class PlayerInfo
{
	public string playerName;
	public int playerLvl;
	public int playerExp;
	public int playerExpNeed;
	public int KP;
	public int GITP;
	public float playerProcent;
	public bool Premium_have;
	

	public GlobalPlayer Player;
}

[System.Serializable]
public class RoomsNgui
{
	public GameObject ShablonRoom;
	public GameObject AllRoom;

	public UIWidget RoomsFound;
	public UIWidget CreateRoom;
	public UIWidget Loading;
	public UIProgressBar LoadingProgress;
	public UILabel LoadingProcent;

	public UIInput NameNewRoom;


	public UIGrid Grid;
	public UIScrollBar ScrollBar;
}
[System.Serializable]
public class PanelsNgui
{
	public UIPanel SliderPanel;
	public SlaiderPanel slaiderPanel;
    public UIPanel mainpanel;
    public UIPanel settings;
}
