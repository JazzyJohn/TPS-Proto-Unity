using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuGUI : MonoBehaviour {

	int numMessage=0;
	int numMessageOld;
	private ServerHolder Server;

	public MoveNGUICamer CamMove;
	
	public GameObject loadingScreen;

	[HideInInspector] 
	public GameObject ActivBut;
	
	public RoomsNgui _RoomsNgui;
	
	public LoginPanel _LoginPanel;

	public PanelsNgui _PanelsNgui;
	
	public MoneyAnnonce _MoneyAnnonce;

	public List<Chat> _chat;

	public Class1 ChatComponent;

	public Class2 SizeScreen;

	public PlayerComponent _PlayerComponent;

	public PlayerInfo _playerInfo; 

	public Timer _timer;

	public Dictionary<string, string> Rooms = new Dictionary<string, string>();

	public UIRect[] MainPanels;

    public GAMEMODE gameMode = GAMEMODE.PVP;

    public UIWidget loadingWidget;

    public UIWidget allWWidget;

    public void LoadingFinish()
    {
		if(loadingWidget!=null){
			Destroy(loadingWidget.gameObject);
			loadingWidget=null;
		}
        allWWidget.alpha = 1.0f;
		int count =ItemManager.instance.MarkedAmount();
        if (_PanelsNgui.markedPanel != null)
        {
            if (count > 0)
            {
                _PanelsNgui.markedPanel.alpha = 1.0f;
                _PanelsNgui.markedPanel.GetComponent<MarkItemGui>().Init(count);
            }
            else
            {
                _PanelsNgui.markedPanel.alpha = 0.0f;
            }
        }
    }


	void Awake(){
		HideAllPanel();
		DontDestroyOnLoad(transform.gameObject);
	}
    void Update()
    {

        if (PremiumManager.instance.IsPremium())
        {
          
            _PlayerComponent.premium.text = TextGenerator.instance.GetSimpleText("PremiumLeft",PremiumManager.instance.TimeLeft());

        }
        else
        {
            _PlayerComponent.premium.text = TextGenerator.instance.GetSimpleText("NoPremiumTitle");
        }

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
        
		AddMessageToChat("Система", "Добро пожаловать !");

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
				NewRoom.transform.localPosition = new Vector3(0f, 0f, 0f);

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
		StartCoroutine(PausePlay());
	}

	public IEnumerator PausePlay()
	{
		yield return new WaitForSeconds(0.05f);
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
			case AnyRoom.TypeRoom.NewRoom:
				CreateRoom();
				break;
			}
			
		} else {
			if(Server.allRooms.Count>0){
				Server.JoinRoom(Server.allRooms[UnityEngine.Random.Range(0,Server.allRooms.Count)]);
				HideAllPanel();
				_RoomsNgui.Loading.alpha = 1f;
				_PanelsNgui.SliderPanel.alpha = 1f;
			}else{

                Server.CreateNewRoom();
			}
			
			
		}
	}

	public void CreateRoom(){
		HideAllPanel ();
		_RoomsNgui.CreateRoom.alpha = 1f;

		
		_RoomsNgui.NameNewRoom.value = Server.newRoomName;

	}
	public void LoginPage(){
		allWWidget.alpha = 0.0f;
		if(loadingWidget!=null){
			loadingWidget.alpha=0.0f;
		}
        _LoginPanel.mainPanel.alpha = 1.0f;
		if( PlayerPrefs.HasKey("login")){
			_LoginPanel.emailField.value = PlayerPrefs.GetString("login");
		}else{
			_LoginPanel.login.alpha= 0.0f;
			_LoginPanel.registration.alpha = 1.0f;
		}
	}
    public void OpenEnterPage()
    {
        allWWidget.alpha = 0.0f;
        if (loadingWidget != null)
        {
            loadingWidget.alpha = 0.0f;
        }
        _LoginPanel.mainPanel.alpha = 1.0f;
        _LoginPanel.login.alpha = 1.0f;
        _LoginPanel.registration.alpha = 0.0f;
    }
    public void OpenRegPage()
    {
        allWWidget.alpha = 0.0f;
        if (loadingWidget != null)
        {
            loadingWidget.alpha = 0.0f;
        }
        _LoginPanel.mainPanel.alpha = 1.0f;
        _LoginPanel.login.alpha = 0.0f;
        _LoginPanel.registration.alpha = 1.0f;
    }
	public void Login(UILabel login,UILabel password){
        RegistrationAPI.instance.Login(login.text, password.text);
	}
	public void Registrate(UILabel login,UILabel password,UILabel repeatPassword,UILabel nick){
        if (password.text != repeatPassword.text)
        {
            _LoginPanel.regError.text = "Пароли не совпадают";
			return;
		}
        if (password.text =="")
        {
            _LoginPanel.regError.text = "Пароль не может быть пустым";
            return;
        }
        if (login.text == "")
        {
            _LoginPanel.regError.text = "Логин не может быть пустым";
            return;
        }
        if (nick.text == "")
        {
            _LoginPanel.regError.text = "Ник не может быть пустым";
            return;
        }
        RegistrationAPI.instance.Registration(login.text, password.text, nick.text);
	}
	public void SetRegistarationError(string text){
        _LoginPanel.regError.text = text;
	}
	public void SetLoginError(string text){
        _LoginPanel.logError.text = text;
	}
	public void FinishLogin(){
		allWWidget.alpha = 0.0f;
		_LoginPanel.mainPanel.alpha = 0.0f;
		if(loadingWidget!=null){
            loadingWidget.alpha = 1.0f;
		}else{
			allWWidget.alpha = 1.0f;
		}
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


        if (Server.map == "" || Server.newRoomName=="")
        {
            return;
        }
		CamMove.enabled = false;
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
				NewRoom.transform.localPosition = new Vector3(0f, 0f, 0f);
				
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
		/* Чат
		ChatComponent.ChatLabel.transform.localPosition = new Vector3(-180, -(ChatComponent.ChatPanel.GetViewSize().y/2)+20, 0f);
		_PanelsNgui.slaiderPanel.ReSize ();
      _PanelsNgui.mainpanel.Invalidate(true);
      */
	}

	public void FullScreen() // На весь экран
	{

        GlobalPlayer.FullScreen(!Screen.fullScreen);


       ReSize();
	}

	public void AddMessageToChat(string name, string message)//Добавление сообщения в чат
	{
        if (ChatComponent.ChatShablon == null)
        {
            return;
        }
		ChatComponent.NumMessage++;

		if (_chat.Count > ChatComponent.MessageLimit)
			_chat.Remove(_chat[0]);

		Chat ChatMessage = new Chat();
		ChatMessage.ID = (_chat.Count-1).ToString();
		ChatMessage.MessageObj = Instantiate(ChatComponent.ChatShablon) as Transform;
		ChatMessage.MessageObj.parent = ChatComponent.ChatZone;
		ChatMessage.MessageObj.localPosition = new Vector3(0f, 0f, 0f);
		ChatMessage.MessageObj.localEulerAngles = new Vector3(0f, 0f, 0f);
		ChatMessage.MessageObj.localScale = new Vector3(1f, 1f, 1f);
		ChatMessage.Message = ChatMessage.MessageObj.GetComponent<MessageNGUI_ForChat>();
		ChatMessage.Message.NameText.text = name;
		ChatMessage.Message.Text.text = message;
		ChatMessage.Message.Obj1.alpha = 1f;
		
		_chat.Add(ChatMessage);

		ChatComponent.ChatZone.GetComponent<UITable>().Reposition();
		ChatComponent.ChatZone.parent.GetComponent<UIScrollView>().ResetPosition();
	}

	public void EnterText()// Ввод текста в чат
	{
		AddMessageToChat(_playerInfo.playerName, ChatComponent.TextInput.value);

		ChatComponent.TextInput.value = null;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		_timer.timer+=Time.deltaTime;
		if (_timer.timer>=_timer.TimeRefresh)
		{
			RefreshRoom();
			_timer.timer = 0;
		}

	
		if (LevelingManager.instance.isLoaded) {
			LoadData();		
		
		}



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
    

    public void SetMessage(string text)
    {
        _PanelsNgui.annonce.text = text;
        _PanelsNgui.annonceTweener.tweenFactor = 0.0f;
        _PanelsNgui.annonceTweener.PlayForward();
    }
	public void SetMoneyMessage(string cash,string gold)
    {
        _MoneyAnnonce.gold.text = gold;
		_MoneyAnnonce.cash.text = cash;
        _MoneyAnnonce.annonceTweener.tweenFactor = 0.0f;
        _MoneyAnnonce.annonceTweener.PlayForward();
    }

    public void MoneyError()
    {
        AskMoneyShow();
    }
    public void AskMoneyShow()
    {
        CamMove.RideTo(1);
        _PanelsNgui.askAboutMoneyPanel.alpha = 1f;
    }
    public void AskMoneyHide()
    {
        CamMove.Reset();
        
        _PanelsNgui.askAboutMoneyPanel.alpha = 0.0f;
    }
    public void MoneyBuyShow()
    {
        CamMove.RideTo(2);
        _PanelsNgui.askAboutMoneyPanel.alpha = 0.0f;
        _PanelsNgui.moneyBuyPanel.alpha = 1f;
    }
    public void MoneyBuyHide()
    {
        CamMove.Reset();
        _PanelsNgui.moneyBuyPanel.alpha = 0.0f;
    }
    public void AskExternalBuy(string item)
    {

        GlobalPlayer.instance.AskJsForMagazine(item);
    }

    public void SetAvatar(Texture2D avatar)
    {
        _PlayerComponent.avatar.mainTexture = avatar;
    }
	
	public void ShowServerWait(){
		if(_PanelsNgui.serverResponse!=null){
			_PanelsNgui.serverResponse.alpha = 1.0f;
		}
	
	}
	public void HideServerWait(){
		if(_PanelsNgui.serverResponse!=null){
			_PanelsNgui.serverResponse.alpha = 0.0f;
		}
	}
}


//Группы переменных

[System.Serializable]
public class Class1
{
	public int NumMessage = 0;
	public int MessageLimit;
	public UIInput TextInput;
	public Transform ChatZone;
	public Transform ChatShablon;
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
	public Transform MessageObj;
	public MessageNGUI_ForChat Message;

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
    public UITexture avatar;
    public UILabel premium;
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
    public UILabel annonce;
    public UITweener annonceTweener;
	public UIPanel markedPanel;
    public UIPanel askAboutMoneyPanel;
    public UIPanel moneyBuyPanel;
	public UIWidget serverResponse;
}
[System.Serializable]
public class LoginPanel
{
	public UIPanel mainPanel;
	public UIWidget registration;
	public UIWidget login;
	public UIInput 	emailField;
	public UILabel regError;
	public UILabel logError;
	
	
}
[System.Serializable]
public class MoneyAnnonce
{
	public UILabel cash;
	public UILabel gold;
    public UITweener annonceTweener;

}


