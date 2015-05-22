using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuGUI : MonoBehaviour {
    public static int EVENT_KEY = 2;

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

    public Dictionary<string, AnyRoom> Rooms = new Dictionary<string, AnyRoom>();

	public UIRect[] MainPanels;
	
	public UIRect[] HideInGamePanels;
	
	public UIRect[] ShowInGamePanels;

    public GAMEMODE gameMode = GAMEMODE.PVP;

    public UIWidget loadingWidget;

    public UIWidget allWWidget;

	public bool inGame =false;

    public AskWindow askWindow;

    public EventNotifyWindow eventWindow;

    public HUDHolder HUDholder;

    public InventoryGUI shop;
    public void LoadingFinish()
    {
		
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
		_PanelsNgui.SliderPanel.alpha= 0.0f;
        foreach (UIRect panel in HideInGamePanels)
        {
            panel.gameObject.SetActive(true);
        }
        foreach (UIRect panel in ShowInGamePanels)
        {
            panel.gameObject.SetActive(false);
        }

    
        if (GUIHelper.messages.Count > 0)
        {
            //Debug.Log("PULLSIZE" + GUIHelper.messages.Dequeue());
            SimpleMessage(GUIHelper.messages.Dequeue());
        }
        if (!eventWindow.IsActive())
        {
            ShopEvent evnt = GUIHelper.shopEvent;
            if (evnt != null)
            {
                ShowEvent(evnt);

            }
        }
       
    }
    public void TryShowEvent()
    {
        if (allWWidget.alpha != 1.0f)
        {
            return;
        }
        if (eventWindow.IsActive())
        {
            return;
        }
        ShopEvent evnt = GUIHelper.shopEvent;
        if (evnt != null)
        {
            ShowEvent(evnt);

        }
    }

    public void NotifyClose(){
        ShopEvent evnt = GUIHelper.shopEvent;
        if (evnt != null)
        {
            ShowEvent(evnt);

        }
    }
    public static MainMenuGUI instance;
	void Awake(){
	
		HideAllPanel();
		DontDestroyOnLoad(transform.gameObject);
        if (instance == null)
        {
            instance = this;
        }
	}
    void Update()
    {
        if (_PlayerComponent.premium != null)
        {
            if (PremiumManager.instance.IsPremium())
            {

                //_PlayerComponent.premium.text = TextGenerator.instance.GetSimpleText("PremiumLeft", PremiumManager.instance.TimeLeft());

            }
            else
            {
                _PlayerComponent.premium.text = TextGenerator.instance.GetSimpleText("NoPremiumTitle");
            }
        }
        
        RefreshRoom();
     
        if (LevelingManager.instance.isLoaded)
        {
            LoadData();

        }
        
		
    }
	public void ShowNews(){
        if (loadingWidget != null)
        {
            Destroy(loadingWidget.gameObject);
            loadingWidget = null;
        }
		_PanelsNgui.SliderPanel.alpha= 1.0f;
	}

	// Use this for initialization
	void Start () 
	{

		

        Screen.lockCursor = false;
		//Поправить размер формы
	
        _playerInfo.Player = FindObjectOfType<GlobalPlayer>();
		//Получение с сервера комнат
		Server = _playerInfo.Player.GetComponent<ServerHolder>();
        ReSize();
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

				_RoomsNgui.ScrollBar.barSize = 0;
			}
			_RoomsNgui.Grid.Reposition ();
		}
        //_PanelsNgui.SliderPanel.alpha = 1f;
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
        if (inGame)
        {
            return;
        }
		StartCoroutine(PausePlay());
	}
	public void  JoinRoom(RoomData room){
		if(inGame){
			return;
		}
        if (room.playerCount < room.maxPlayers)
        {
				//PhotonNetwork.JoinRoom (room.name);
				Server.JoinRoom(room);
				HideAllPanel();
				_RoomsNgui.Loading.alpha = 1f;
				//_PanelsNgui.SliderPanel.alpha = 1f;
				
		}
		
	
	}
	public IEnumerator PausePlay()
	{
		
		yield return new WaitForSeconds(0.05f);
		Debug.Log ("PLAY");
        GA.API.Design.NewEvent("GUI:MainMenu:Play", 1); 
   
		if (ActivBut != null) {
			/*switch (ActivBut.GetComponent<AnyRoom> ()._TypeRoom) {
			case AnyRoom.TypeRoom.JoinRoom:
				foreach (RoomData room in Server.allRooms)
				{
					if (room.name == ActivBut.name && room.playerCount < Server.newRoomMaxPlayers) {
						//PhotonNetwork.JoinRoom (room.name);
						Server.JoinRoom(room);
						HideAllPanel();
						_RoomsNgui.Loading.alpha = 1f;
						//_PanelsNgui.SliderPanel.alpha = 1f;
						
					}
				}
				break;
			case AnyRoom.TypeRoom.NewRoom:
				CreateRoom();
				break;
			}*/
			
		} else {
            List<RoomData> allRooms = Server.GiveAlowedRooms();
            if (allRooms.Count > 0)
            {
				Server.JoinRoom(allRooms[UnityEngine.Random.Range(0,allRooms.Count)]);
				HideAllPanel();
				_RoomsNgui.Loading.alpha = 1f;
				//_PanelsNgui.SliderPanel.alpha = 1f;
			}else{

                Server.CreateNewRoom();
			}
			
			
		}
	}

   

	public void CreateRoom(){
		if(inGame){
			return;
		}
	
		HideAllPanel ();
		_RoomsNgui.CreateRoom.alpha = 1f;
        _RoomsNgui.mainpanel.alpha = 1f;
		
		//_RoomsNgui.NameNewRoom.value = Server.newRoomName;
		
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
          
			Server.SetMap(mapName);
            _RoomsNgui.NameNewRoom.value = Server.newRoomName;
		}
	}
    public void NextMap()
    {
        ServerHolder.Instance.NextMap();
    }
    public void PrevMap()
    {
        ServerHolder.Instance.PrevMap();
    }

	public void LoadingScreen() //Изменения процентов при загрузке(вызывает прогресс бар)
	{
		//_PanelsNgui.SliderPanel.alpha= 1.0f;
		_RoomsNgui.Loading.alpha = 1f;
      
        shop.LoadStart();
	}
	
	public void StartBut() //Создать комнату
	{


        if (Server.map == "" || Server.newRoomName=="")
        {
            return;
        }
		
		
		Server.newRoomName = _RoomsNgui.NameNewRoom.value;
		HideAllPanel ();
		_RoomsNgui.Loading.alpha = 1f;
      //  _PanelsNgui.SliderPanel.alpha = 1f;
        Server.CreateNewRoom(Server.map);
        CamMove.enabled = false;
	}

	public void BackBut() //Вернуться к выбору комнат
	{
		HideAllPanel ();
		_RoomsNgui.RoomsFound.alpha = 1f;

	}

	UIScrollView gamesRoom;

	public void RefreshRoom() // Обновления списка комнат
	{
        int i = 0;

		if(!gamesRoom)
			gamesRoom = _RoomsNgui.Grid.GetComponentInParent<UIScrollView>();

        foreach (RoomData room in Server.allRooms)
		{
            i++;
			if (!Rooms.ContainsKey(room.name))
			{
				
				AnyRoom NewRoom = (Instantiate(_RoomsNgui.ShablonRoom) as GameObject).GetComponent<AnyRoom>();
				Rooms.Add(room.name,NewRoom );
				
				NewRoom.transform.parent = _RoomsNgui.AllRoom.transform;
				
				NewRoom.transform.localScale = new Vector3(1f, 1f, 1f);
				NewRoom.transform.localPosition = new Vector3(0f, 0f, 0f);

				//_RoomsNgui.ScrollBar.barSize = 0;
			}
            Rooms[room.name].UpdateRoom(room, i);
		}
		_RoomsNgui.Grid.Reposition();
		gamesRoom.UpdatePosition();
		gamesRoom.UpdateScrollbars(true);

	}



	public void ShowGameList(){
        HideAllPanel();
        _RoomsNgui.mainpanel.alpha = 1f;
        _RoomsNgui.RoomsFound.alpha = 1f;
	}
	public void GetPlayerInfo() // Получение значений игрока
	{

		PlayerMainGui.LevelStats lvl = LevelingManager.instance.GetPlayerStats ();
		_playerInfo.playerName = _playerInfo.Player.PlayerName;
		_playerInfo.KP = _playerInfo.Player.cash;
		_playerInfo.GITP = _playerInfo.Player.gold;
        _playerInfo.SKILLPOINT =PassiveSkillManager.instance.skillPointLeft;
		_playerInfo.playerLvl = lvl.playerLvl;
		_playerInfo.playerExp =  LevelingManager.instance.playerExp;
        if (LevelingManager.instance.playerNeededExp.Length > _playerInfo.playerLvl)
        {
            _playerInfo.playerExpNeed = LevelingManager.instance.playerNeededExp[_playerInfo.playerLvl];
        }
        else
        {
            _playerInfo.playerExpNeed = _playerInfo.playerExp;
        }
		_playerInfo.playerProcent = lvl.playerProcent;
	}

	public void SetPlayerInfoInGUI() //Запись значений игрока в ГУИ
	{
		_PlayerComponent.Name.text = _playerInfo.playerName;
		_PlayerComponent.Lvl.text =  _playerInfo.playerLvl.ToString();
		_PlayerComponent.Exp.text = _playerInfo.playerExp + " / " + _playerInfo.playerExpNeed;
		_PlayerComponent.ExpBar.value = _playerInfo.playerProcent /100f;
		_PlayerComponent.KP.text = _playerInfo.KP.ToString();
		_PlayerComponent.GITP.text = _playerInfo.GITP.ToString();
        _PlayerComponent.SKILLPOINT.text = _playerInfo.SKILLPOINT.ToString(); 
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
	
	
    
	public void ShowRoomList(){

		//Debug.Log (_RoomsNgui.RoomsFound.alpha);
		if (_RoomsNgui.RoomsFound.alpha >0f) {
			HideAllPanel();
		//	_PanelsNgui.SliderPanel.alpha= 1f;
		} else {
			HideAllPanel();
			_RoomsNgui.RoomsFound.alpha = 1f;
		}


	}
	public void AddCoins(){
		 _playerInfo.Player.AskJsForMagazine("gitp_5");
	}
    public void SimpleMessage(string text)
    {
        _PanelsNgui.simpleMessagePanel.alpha =1.0f;
        _PanelsNgui.simpleMessage.text = text;
    }
    public void CloseMessage()
    {
        _PanelsNgui.simpleMessagePanel.alpha = 0.0f;
        if (GUIHelper.messages.Count > 0)
        {
            SetMessage(GUIHelper.messages.Dequeue());
        }
    }
    public void SetMessage(string text)
    {
        if (_PanelsNgui.annonce == null)
        {
            return;
        }
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
        askWindow.action = MoneyBuyShow;
     
        askWindow.Show(TextGenerator.instance.GetSimpleText("NoMoney"));
    }
  
    public void MoneyBuyShow()
    {
        GA.API.Design.NewEvent("GUI:MainMenu:MoneyBuyShow", 1); 
        CamMove.RideTo(2);
      //  _PanelsNgui.askAboutMoneyPanel.alpha = 0.0f;
        _PanelsNgui.moneyBuyPanel.alpha = 1f;
    }
    public void MoneyBuyHide()
    {
        CamMove.Reset();
        _PanelsNgui.moneyBuyPanel.alpha = 0.0f;
    }
    public void AskExternalBuy(string item)
    {
        GA.API.Design.NewEvent("GUI:MainMenu:AskExternalBuy:" + item, 1); 
        GlobalPlayer.instance.AskJsForMagazine(item);
        MoneyBuyHide();
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
    public void TakeScreenShoot()
    {
        
        ScreenShootManager.instance.TakeScreenshot(true);
    }
    public void TakeScreenShootWall()
    {

        ScreenShootManager.instance.TakeScreenshotToWall(TextGenerator.instance.GetSimpleText("i'm in red rage"));
    }
	
	//IN GAME SECTION 
	
	public UIPanel PlayGUI;

    public PlayerMainGui PlayerGUI;

    private bool Pause;
	
	
	
	public void FinishLvlLoad() //Изменения процентов при загрузке(вызывает прогресс бар)
	{
		//_PanelsNgui.SliderPanel.alpha= 0.0f;
		_RoomsNgui.Loading.alpha = 0.0f;
        gameObject.SetActive(false);
        allWWidget.alpha = 1.0f;
		inGame= true;
		foreach(UIRect panel in HideInGamePanels){
			panel.gameObject.SetActive(false);
		}
        foreach (UIRect panel in ShowInGamePanels)
        {
			panel.gameObject.SetActive(true);
		}
		PlayGUI = PlayerGUI.PlayGUI;
        PlayerGUI.pausegui = this;
        Pause = false;
	}

	 public void ActivateMenu(){
         if (!gameObject.activeSelf)
         {
                GA.API.Design.NewEvent("GUI:Pause:Show"); 
                Pause = true;
               
				PlayerGUI.guiState = PlayerMainGui.GUIState.Pause;
				Screen.lockCursor = false;
				PlayGUI.gameObject.SetActive(false);

                gameObject.SetActive(true);
			
		}
    }
     public bool IsActive()
     {
         if (InputManager.instance.GetButtonDown("Pause")||Input.GetKeyDown(KeyCode.Escape))
         {
         
             return !Pause;
         }
         return Pause;
     }
	public void BackToGame()
	{
          
            Pause = false;
          
            PlayerGUI.guiState = PlayerMainGui.GUIState.Normal;

		
            gameObject.SetActive(false);
            PlayGUI.gameObject.SetActive(true);

        
	}
	public void GoToMainMenu()
	{
		if (ServerHolder.Instance.connectingToRoom)
        {
            return;
        }
        GA.API.Design.NewEvent("GUI:Pause:MainMenu");
		Pause = true;
             
		Screen.lockCursor = false;
        NetworkController.Instance.LeaveRoomReuqest();
        ItemManager.instance.LeaveRoom();
        if (HUDholder != null)
        {
            Destroy(HUDholder.gameObject);
        }
        Application.LoadLevel("Empty");
        gameObject.SetActive(true);
		inGame= true;
		foreach(UIRect panel in HideInGamePanels){
			panel. gameObject.SetActive(true);
		}
		foreach(UIRect panel in ShowInGamePanels){
			panel. gameObject.SetActive(false);
		}
       
	}
    void OnLevelWasLoaded(int level)
    {
        if(level==0||level==1){
            FindObjectOfType<MusicHolder>().SetStage( MUSIC_STAGE.MAIN_MENU_LOOP);
            gameObject.SetActive(true);
            inGame = false;
            foreach (UIRect panel in HideInGamePanels)
            {
                panel.gameObject.SetActive(true);
            }
            foreach (UIRect panel in ShowInGamePanels)
            {
                panel.gameObject.SetActive(false);
            }
            TryShowEvent();
            
        }
    }
    public void ShowEvent(ShopEvent evt)
    {

        InventorySlot item;
                   
        switch (evt.type)
        {
            case ShopEventType.CAN_TAKE_HOLLIDAY:
                item = ItemManager.instance.GetItem(evt.item);
                eventWindow.action = delegate()
                {
                    shop.SetItemForChoiseSet(item);
                };
                eventWindow.Show(evt.text, item, TextGenerator.instance.GetSimpleText("take_item"));
                break;
            case ShopEventType.CAN_TAKE:
                 item = ItemManager.instance.GetItem(evt.item);
                if (!item.IsEvented())
                {
                    return;
                }
                eventWindow.action = delegate()
                {
                    shop.SetItemForChoiseSet(item);
                };
                eventWindow.Show(evt.text, item, TextGenerator.instance.GetSimpleText("take_item"));
                eventWindow.ShowEnd(evt.end);
                break;
            case ShopEventType.DISCOUNT:
                 item = ItemManager.instance.GetItem(evt.item);
                if (item.isAvailable())
                {
                    return;
                }
                int amount = item.prices[EVENT_KEY].GetPrice();
                eventWindow.action = delegate()
                {
                  
                    if (item.prices[0].type == BuyPrice.KP_PRICE)
                    {
                        GA.API.Business.NewEvent("Shop:BUYItem:" + item.engName, "GASH", amount);
                    }
                    else
                    {
                        GA.API.Business.NewEvent("Shop:BUYItem:" + item.engName, "GOLD", amount);
                    }
                    StartCoroutine(ItemManager.instance.BuyItem(item.prices[EVENT_KEY].id, shop));
                };
               // string text = item.prices[EVENT_KEY].GetText();
                eventWindow.Show(evt.text, item, TextGenerator.instance.GetMoneyText("simple_buy_item", amount));
                eventWindow.ShowEnd(evt.end);
                break;
            case ShopEventType.KITDISCOUNT:
                InvItemGroupSlot slot = ItemManager.instance.GetKit(evt.item);
                int slotPrice = slot.GetPrice();
                eventWindow.action = delegate()
                {

                    IndicatorManager.instance.Remove(IndicatorManager.KITS, evt.end);
                    StartCoroutine(ItemManager.instance.BuyItemKit(slot.id ));
                };
               // string text = item.prices[EVENT_KEY].GetText();
                eventWindow.Show(evt.text, slot, TextGenerator.instance.GetMoneyText("simple_buy_item", slotPrice));
                break;

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
    public UILabel SKILLPOINT;
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
    public int SKILLPOINT;
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
    public UIPanel mainpanel;
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
    public UIWidget SliderPanel;
	public SlaiderPanel slaiderPanel;
    public UIPanel mainpanel;
    public UIPanel settings;
    public UILabel annonce;
    public UITweener annonceTweener;
	public UIPanel markedPanel;
    public UIPanel askAboutMoneyPanel;
    public UIRect moneyBuyPanel;
    public UIRect serverResponse;
    public UIRect simpleMessagePanel;
    public UILabel simpleMessage;
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


