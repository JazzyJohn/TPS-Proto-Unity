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

    public SettingsPanel _SettingPanel;

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
        foreach (GameObject onContrl in _SettingPanel.toggleList) {
            SettingsPanel.SettingCommand commandClass = new SettingsPanel.SettingCommand();
            commandClass.codename = onContrl.GetComponent<UIToggle>();
            commandClass.command = onContrl.GetComponent<ControlButton>().command;
            commandClass.keyname = onContrl.transform.GetChild(0).GetComponent<UILabel>();
            _SettingPanel.controls.Add(commandClass);
        }
	}

	IEnumerator SetDefoltGraphic(int i)
	{
		yield return new WaitForSeconds(0.01f);
		switch(i)
		{
		case 0:
			SaveGraphicSetting();
			break;
		case 1:
			ApplyGraphicSetting();
			break;
		}
	}

	// Use this for initialization
	void Start () 
	{

        

        Screen.lockCursor = false;
		//Поправить размер формы
		ReSize();
        _playerInfo.Player = FindObjectOfType<GlobalPlayer>();
		//Получение с сервера комнат
		Server = _playerInfo.Player.GetComponent<ServerHolder>();
        if (PhotonNetwork.inRoom)
        {
            Server.LeaveRoom();


        }

		if (Server.allRooms != null) {
						foreach (RoomInfo room in Server.allRooms) {
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
        ShowSetting();
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
								foreach (RoomInfo room in Server.allRooms) {
										if (room.name == ActivBut.name && room.playerCount < Server.newRoomMaxPlayers) {
												//PhotonNetwork.JoinRoom (room.name);
												Server.JoinRoom(room.name);
												HideAllPanel();
												_RoomsNgui.Loading.alpha = 1f;
                                                _PanelsNgui.SliderPanel.alpha = 1f;

										}
								}
								break;
						}

		} else {
			if(Server.allRooms.Length>0){
				Server.JoinRoom();
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
		foreach(RoomInfo room in Server.allRooms)
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
    public void OnGUI()
    {
	
		if(waitForInput){
			Event e = Event.current;
            Debug.Log("WAIT" + command + " " + e.keyCode);
            if (e!=null&&e.isKey)
            {
                Debug.Log("SET" + e.keyCode);
				newMap[command] = e.keyCode;
				waitForInput= false;
				foreach(SettingsPanel.SettingCommand setCommand in _SettingPanel.controls){
                    if (setCommand.command == command)
                    {
                        setCommand.keyname.text = e.keyCode.ToString();	
						break;
					}		
				}
			}
            if (e != null && e.isMouse)
            {
                switch (e.button)
                {
                    case 0:
                        newMap[command] = KeyCode.Mouse0;
                        break;
                    case 1:
                        newMap[command] = KeyCode.Mouse1;
                        break;
                    case 2:
                        newMap[command] = KeyCode.Mouse2;
                        break;
                }
                
                waitForInput = false;
                foreach (SettingsPanel.SettingCommand setCommand in _SettingPanel.controls)
                {
                    if (setCommand.command == command)
                    {
                        setCommand.keyname.text = newMap[command].ToString();
                        setCommand.codename.value = false;
                        break;
                    }
                }
            }
		
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

    public void ShowSetting()
    {

        //Debug.Log (_RoomsNgui.RoomsFound.alpha);
        if (_PanelsNgui.settings.alpha > 0f) {
            HideAllPanel();
            _PanelsNgui.SliderPanel.alpha = 1f;
            
        }
        else
        {
            HideAllPanel();
            _PanelsNgui.settings.alpha = 1f;
           
        }
       


    }
	
	
    public void HideAllSettingsPanel() {
        _SettingPanel.video.alpha = 0f;
        _SettingPanel.control.alpha = 0f;
    }
    public void ShowControl() {
			if(  _SettingPanel.control.alpha ==0){
				CearControlls();				
			}
			
            HideAllSettingsPanel();
            _SettingPanel.control.alpha = 1f;
     
    }
	
    public void ShowVideo()
    {

        HideAllSettingsPanel();
        _SettingPanel.video.alpha = 1f;

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
	//CONTROLL SECTION 
	private string command ="";
	
	private bool waitForInput= false;
	
	private Dictionary<string,KeyCode> newMap ;

    public void WaitForKey(string command)
    {
		waitForInput = true;
        this.command = command;
	}
	public void ApplyControlls(){
        foreach (KeyValuePair<string, KeyCode> oneCom in newMap)
        {
			InputManager.instance.SaveKey(oneCom.Key,oneCom.Value);
		}
        InputManager.instance.SaveSensitivity(_SettingPanel.mouseSensitivity.value * 2.0f);
	
	}
	public void CearControlls(){
        newMap = new Dictionary<string, KeyCode>();
        Dictionary<string, KeyCode> map = InputManager.instance.GetMap();
        foreach (SettingsPanel.SettingCommand setCommand in _SettingPanel.controls)
        {
            setCommand.keyname.text = map[setCommand.command].ToString();				
		}
        _SettingPanel.mouseSensitivity.value = InputManager.instance.GetSensitivity() / 2f;
        SetMouseLabel();
	
	}
    public void DefaultControl() {
        newMap = new Dictionary<string, KeyCode>();
        Dictionary<string, KeyCode> map = InputManager.instance.ForceReload();
        foreach (SettingsPanel.SettingCommand setCommand in _SettingPanel.controls)
        {
            setCommand.keyname.text = map[setCommand.command].ToString();
        }
        _SettingPanel.mouseSensitivity.value = InputManager.instance.GetSensitivity() / 2f;
        SetMouseLabel();
    }
    public void SetMouseLabel() {
        _SettingPanel.mouseLabel.text = (_SettingPanel.mouseSensitivity.value * 100f).ToString("0");
    }

	public void SetValueVolume(int IntArg, UIScrollBar ScrollArg) //Установка звука (Текст)
	{
		string value = (ScrollArg.value * 100f).ToString("0");
		switch(IntArg)
		{
		case 0:
			_SettingPanel.volumes.Volume.text = value;
			break;
		case 1:
			_SettingPanel.volumes.SoundFx.text = value;
			break;
		case 2:
			_SettingPanel.volumes.Music.text = value;
			break;
		}
	}
	public void SetGraphic(UILabel ValueLabel, UIScrollBar ScrollValue, string Setting) //Настройки графики (текст)
	{

		int arg1 = Mathf.RoundToInt(ScrollValue.value*(ScrollValue.numberOfSteps-1));

		if(Setting != "Resolution")
		switch(arg1)
		{
		case 0:
			switch(Setting)
			{
			case "Graphic":
				_SettingPanel.graphicSetting.Texture.text = "Low";
				_SettingPanel.graphicSetting.Shadow.text = "Low";
				_SettingPanel.graphicSetting.Lighning.text = "Low";
				_SettingPanel.graphicSetting.TextureScroll.value = 0f;
				_SettingPanel.graphicSetting.ShadowScroll.value = 0f;
				_SettingPanel.graphicSetting.LighningScroll.value = 0f;
					break;
			default:

				ValueLabel.text = "Low";
				break;
			}
			break;
		case 1:
			switch(Setting)
			{
			case "Graphic":
				_SettingPanel.graphicSetting.Texture.text = "Medium";
				_SettingPanel.graphicSetting.Shadow.text = "Medium";
				_SettingPanel.graphicSetting.Lighning.text = "Medium";
				_SettingPanel.graphicSetting.TextureScroll.value = 0.5f;
				_SettingPanel.graphicSetting.ShadowScroll.value = 0.5f;
				_SettingPanel.graphicSetting.LighningScroll.value = 0.5f;
				break;
			default:
				ValueLabel.text = "Medium";
				break;
			}
			break;
		case 2:
			switch(Setting)
			{
			case "Graphic":
				_SettingPanel.graphicSetting.Texture.text = "High";
				_SettingPanel.graphicSetting.Shadow.text = "High";
				_SettingPanel.graphicSetting.Lighning.text = "High";
				_SettingPanel.graphicSetting.TextureScroll.value = 1f;
				_SettingPanel.graphicSetting.ShadowScroll.value = 1f;
				_SettingPanel.graphicSetting.LighningScroll.value = 1f;
				break;
			default:
				ValueLabel.text = "High";
				break;
			}
			break;
		}
		else
			ValueLabel.text = _SettingPanel.AllResolution[arg1];

		if (_SettingPanel.graphicSetting.Lighning.text != _SettingPanel.graphicSetting.Texture.text 
		    || _SettingPanel.graphicSetting.Shadow.text != _SettingPanel.graphicSetting.Texture.text
		    || _SettingPanel.graphicSetting.Lighning.text != _SettingPanel.graphicSetting.Shadow.text)
		{
			_SettingPanel.graphicSetting.Graphic.text = "Optional";
			_SettingPanel.graphicSetting.GraphicScroll.value = 1f;
		}
		else if (_SettingPanel.graphicSetting.Texture.text == "Low" && _SettingPanel.graphicSetting.Shadow.text == "Low"
		         && _SettingPanel.graphicSetting.Lighning.text == "Low" && _SettingPanel.graphicSetting.GraphicScroll.value != 0f)
		{
			_SettingPanel.graphicSetting.Graphic.text = "Low";
			_SettingPanel.graphicSetting.GraphicScroll.value = 0f;
		}
		else if (_SettingPanel.graphicSetting.Texture.text == "Medium" && _SettingPanel.graphicSetting.Shadow.text == "Medium"
		         && _SettingPanel.graphicSetting.Lighning.text == "Medium" && _SettingPanel.graphicSetting.GraphicScroll.value != 1/3)
		{
			_SettingPanel.graphicSetting.Graphic.text = "Medium";
			_SettingPanel.graphicSetting.GraphicScroll.value = 1f/3;
		}
		else if (_SettingPanel.graphicSetting.Texture.text == "High" && _SettingPanel.graphicSetting.Shadow.text == "High"
		         && _SettingPanel.graphicSetting.Lighning.text == "High" && _SettingPanel.graphicSetting.GraphicScroll.value != 1/3*2)
		{
			_SettingPanel.graphicSetting.Graphic.text = "High";
			_SettingPanel.graphicSetting.GraphicScroll.value = 1f/3*2;
		}
	}

	public void SaveGraphicSetting()
	{
		PlayerPrefs.SetFloat("Resolution", _SettingPanel.graphicSetting.ResolutionScroll.value);
		PlayerPrefs.SetFloat("TextureQuality", _SettingPanel.graphicSetting.TextureScroll.value);
		PlayerPrefs.SetFloat("ShadowQuality", _SettingPanel.graphicSetting.ShadowScroll.value);
		PlayerPrefs.SetFloat("LighningQuality", _SettingPanel.graphicSetting.LighningScroll.value);
		PlayerPrefs.SetFloat("OverallVolume", _SettingPanel.volumes.VolumeScroll.value);
		PlayerPrefs.SetFloat("SoundFX", _SettingPanel.volumes.SoundFxScroll.value);
		PlayerPrefs.SetFloat("Music", _SettingPanel.volumes.MusicScroll.value);
		PlayerPrefs.SetString("SaveSetting", "yes");
		ApplyGraphicSetting();
	}

	public void ApplyGraphicSetting()
	{
		string[] x_y = _SettingPanel.graphicSetting.Resolution.text.Split('x');
		Screen.SetResolution(int.Parse(x_y[0]), int.Parse(x_y[1]), Screen.fullScreen);
		switch(_SettingPanel.graphicSetting.Texture.text)
		{
		case "Low":
			QualitySettings.SetQualityLevel((int)QualityLevel.Fast);
			break;
		case "Medium":
			QualitySettings.SetQualityLevel((int)QualityLevel.Good);
			break;
		case "High":
			QualitySettings.SetQualityLevel((int)QualityLevel.Fantastic);
			break;
		}
        AudioListener.volume = _SettingPanel.volumes.SoundFxScroll.value * _SettingPanel.volumes.VolumeScroll.value;
        MusicHolder.SetVolume(_SettingPanel.volumes.MusicScroll.value * _SettingPanel.volumes.VolumeScroll.value);
	}

	public void DefaultGraphic()
	{
		_SettingPanel.graphicSetting.ResolutionScroll.value = 0.125f;
		_SettingPanel.graphicSetting.TextureScroll.value = 1f;
		_SettingPanel.graphicSetting.ShadowScroll.value = 1f;
		_SettingPanel.graphicSetting.LighningScroll.value = 1f;
		_SettingPanel.volumes.VolumeScroll.value = 1f;
		_SettingPanel.volumes.SoundFxScroll.value = 1f;
		_SettingPanel.volumes.MusicScroll.value = 1f;
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
[System.Serializable]
public class SettingsPanel
{
	
	public class SettingCommand{
		public UIToggle codename;
		public UILabel keyname;
		public string command;			
	}
	public List<SettingCommand> controls = new List<SettingCommand>();
    public List<GameObject> toggleList;
	public UISlider mouseSensitivity;
    public UILabel mouseLabel;
    public UIPanel control;
	public UIPanel video;
    public UIPanel game;
	public Volumes volumes;
	public List<string> AllResolution;

	public GraphicSetting graphicSetting; 

	[System.Serializable]
	public class Volumes
	{
		public UILabel Volume;
		public UILabel SoundFx;
		public UILabel Music;
		public UIScrollBar VolumeScroll;
		public UIScrollBar SoundFxScroll;
		public UIScrollBar MusicScroll;
	}

	[System.Serializable]
	public class GraphicSetting
	{
		public UILabel Resolution;
		public UILabel Graphic;
		public UILabel Texture;
		public UILabel Shadow;
		public UILabel Lighning;
		public UIScrollBar ResolutionScroll;
		public UIScrollBar GraphicScroll;
		public UIScrollBar TextureScroll;
		public UIScrollBar ShadowScroll;
		public UIScrollBar LighningScroll;
	}
}