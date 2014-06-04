using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Element_metod : MonoBehaviour {

	int numMessage=0;
	int numMessageOld;
	private ServerHolder Server;

	[HideInInspector] 
	public GameObject ActivBut;
	
	public RoomsNgui _RoomsNgui;

	public List<Chat> _chat;

	public Class1 ChatComponent;

	public Class2 SizeScreen;

	public PlayerComponent _PlayerComponent;

	public PlayerInfo _playerInfo; 

	public Timer _timer;

	public Dictionary<string, string> Rooms = new Dictionary<string, string>();

	// Use this for initialization
	void Start () 
	{
		//Показать список комнат
		_RoomsNgui.CreateRoom.alpha = 0f;
		_RoomsNgui.RoomsFound.alpha = 1f;
		_RoomsNgui.Loading.alpha = 0f;

		//Поправить размер формы
		ReSize();

		//Загрузить значения персонажа
		StartCoroutine(StartWithPausing());

		//Получение с сервера комнат
		Server = _playerInfo.Player.GetComponent<ServerHolder>();
		foreach(RoomInfo room in Server.allRooms)
		{
			GameObject NewRoom = Instantiate(_RoomsNgui.ShablonRoom) as GameObject;
			NewRoom.transform.parent = _RoomsNgui.AllRoom.transform;
			NewRoom.name = room.name;
			NewRoom.GetComponent<AnyRoom>().Name.text = room.name;
			NewRoom.GetComponent<AnyRoom>().shablon = false;
			NewRoom.GetComponent<AnyRoom>().SizeRoom.text = room.playerCount + " / " + room.maxPlayers;
			NewRoom.transform.localScale = new Vector3(1f, 1f, 1f);

			_RoomsNgui.Grid.Reposition();
			_RoomsNgui.ScrollBar.barSize = 0;
		}
	}

	IEnumerator StartWithPausing() //Задержка в получении и записи значений игрока
	{
		//P.S. нужно поправить
		yield return new WaitForSeconds(2f);
		GetPlayerInfo();
		SetPlayerInfoInGUI();
	}

	public void PlayBut() //Вход или создание новой комнаты
	{
		if (ActivBut != null)
		{
			switch(ActivBut.GetComponent<AnyRoom>()._TypeRoom)
			{
			case AnyRoom.TypeRoom.JoinRoom:
				foreach(RoomInfo room in Server.allRooms)
				{
					if (room.name == ActivBut.name && room.playerCount < Server.newRoomMaxPlayers)
					{
						PhotonNetwork.JoinRoom (room.name);
						Server.connectingToRoom = true;

						_RoomsNgui.CreateRoom.alpha = 0f;
						_RoomsNgui.RoomsFound.alpha = 0f;
						_RoomsNgui.Loading.alpha = 1f;
					}
				}
				break;
			case AnyRoom.TypeRoom.NewRoom:
				_RoomsNgui.RoomsFound.alpha = 0f;
				_RoomsNgui.CreateRoom.alpha = 1f;
				_RoomsNgui.Loading.alpha = 0f;

				_RoomsNgui.NameNewRoom.value = Server.newRoomName;

				break;
			}
		}
	}

	public void Loading() //Изменения процентов при загрузке(вызывает прогресс бар)
	{
		if (_RoomsNgui.Loading.alpha != 1f && ActivBut != null)
		{
			_RoomsNgui.CreateRoom.alpha = 0f;
			_RoomsNgui.RoomsFound.alpha = 0f;
			_RoomsNgui.Loading.alpha = 1f;
		}
		_RoomsNgui.LoadingProcent.text = (_RoomsNgui.LoadingProgress.value*100).ToString("f0") + "%";
	}

	public void StartBut() //Создать комнату
	{
		Server.newRoomName = _RoomsNgui.NameNewRoom.value;
		_RoomsNgui.CreateRoom.alpha = 0f;
		_RoomsNgui.RoomsFound.alpha = 0f;
		_RoomsNgui.Loading.alpha = 1f;
		Server.CreateNewRoom();
	}

	public void BackBut() //Вернуться к выбору комнат
	{
		_RoomsNgui.CreateRoom.alpha = 0f;
		_RoomsNgui.RoomsFound.alpha = 1f;
		_RoomsNgui.Loading.alpha = 0f;
	}

	public void RefreshRoom() // Обновления списка комнат
	{
		foreach(RoomInfo room in Server.allRooms)
		{
			if (Rooms.ContainsValue(room.name))
			{

			}
			else
			{
				Rooms.Add(room.name, room.name);
				
				GameObject NewRoom = Instantiate(_RoomsNgui.ShablonRoom) as GameObject;
				NewRoom.transform.parent = _RoomsNgui.AllRoom.transform;
				NewRoom.name = room.name;
				NewRoom.GetComponent<AnyRoom>().shablon = false;
				NewRoom.GetComponent<AnyRoom>().Name.text = room.name;
				NewRoom.GetComponent<AnyRoom>().SizeRoom.text = room.playerCount + " / " + room.maxPlayers;
				NewRoom.transform.localScale = new Vector3(1f, 1f, 1f);
				
				_RoomsNgui.Grid.Reposition();
				_RoomsNgui.ScrollBar.barSize = 0;
			}
		}
	}

	public void GetPlayerInfo() // Получение значений игрока
	{
		_playerInfo.playerName = _playerInfo.Player.GetComponent<GlobalPlayer>().PlayerName;
		_playerInfo.playerLvl = _playerInfo.Player.GetComponent<LevelingManager>().playerLvl;
		_playerInfo.playerExp = _playerInfo.Player.GetComponent<LevelingManager>().playerExp;
		_playerInfo.playerExpNeed = _playerInfo.Player.GetComponent<LevelingManager>().playerNeededExp[_playerInfo.playerLvl];
	}

	public void SetPlayerInfoInGUI() //Запись значений игрока в ГУИ
	{
		_PlayerComponent.Name.text = _playerInfo.playerName;
		_PlayerComponent.Lvl.text = "Lvl " + _playerInfo.playerLvl;
		_PlayerComponent.Exp.text = _playerInfo.playerExp + " / " + _playerInfo.playerExpNeed;
		_PlayerComponent.ExpBar.value = (float) _playerInfo.playerExp/_playerInfo.playerExpNeed;
	}

	public void ReSize() //Правка позиции компонентов
	{
		ChatComponent.ChatLabel.transform.localPosition = new Vector3(-180, -(ChatComponent.ChatPanel.GetViewSize().y/2)+20, 0f);
	}

	public void FullScreen() // На весь экран
	{
		Resolution[] resolutions = Screen.resolutions;
		switch(SizeScreen.FullScreen_Z)
		{
		case true:
			SizeScreen.FullScreen_Z = false;
			Screen.SetResolution(800, 600, SizeScreen.FullScreen_Z);
			ReSize();

			break;
		case false:
			SizeScreen.FullScreen_Z = true;
			Screen.SetResolution(resolutions[resolutions.Length-1].width, resolutions[resolutions.Length-1].height, SizeScreen.FullScreen_Z);
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
		//if(Загрузка карты)
		//_RoomsNgui.LoadingProgress.value = 0.1f;
	}

	public void scroll() //СкролБар чата
	{
		ChatComponent.ChatPanel.transform.localPosition = new Vector3(0f, -(ChatComponent.ChatScrollBar.value*1600), 0f);
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
	public bool Premium_have;
	
	public GameObject Player;
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