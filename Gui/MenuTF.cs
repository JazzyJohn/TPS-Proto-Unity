﻿using UnityEngine;
using System.Collections;
//[ExecuteInEditMode]

public class MenuTF : MonoBehaviour {

	public static int _Pl_Ro; //0 - player, 1 -robot

	private int _PlayerNum;
	private int _RobotNum;

	
//Background-----------------------
	//0.8f
	public float _BGWidthСoefficient = 0.98f;
	public float _BGHeightСoefficient = 0.98f;

	private int _BackgroundWidth;
	private int _BackgroundHeight;

	private Rect _BackGroungRect;

	//BGpos
	private float _BoxXsp;
	private float _BoxYsp;
	private float _BoxX;
	private float _BoxY;

	//AnimateControl
	//private bool _OpenAnimate;
	//private bool _CloseAnimate;
//---------------------------------


//Button---------------------------
	//Player
	public float _ButtonSizeX = 5.4f;
	public float _ButtonSizeY = 14f;
	public float _ButtonPosX = 8f;
	public float _ButtonPosY = 11f;

	public Texture2D[] _ButtonPlayerTex;
	public Texture2D[] _ButtonPlayerTexPress;

	public GUIStyle[] _GSbutton = new GUIStyle[4];


	//Robot
	public float _robButtonSizeX = 14f;
	public float _robButtonSizeY = 14f;
	public float _robButtonPosX = 2.5f;
	public float _robButtonPosY = 5.9f;

	public Texture2D[] _ButtonRobotTex;
	public Texture2D[] _ButtonRobotTexPress;
	
	public GUIStyle[] _robGSbutton = new GUIStyle[3];


	//Y
	//public int _ButtonIntervalY = 10;
	//X
	//public int _ButtonIntervalX = 10;


	//Team
	public float _TeamButtonSizeX = 4.7f;
	public float _TeamButtonSizeY = 20f;
	public float _TeamButtonPosX = 3.4f;
	public float _TeamButtonPosY = 44.4f;

	public Texture2D[] _ButtonTeamTex;
	public Texture2D[] _ButtonTeamTexPress;

	public GUIStyle[] _TeamGSbutton = new GUIStyle[2];
//---------------------------------


	public GUIStyle _GS = new GUIStyle();

	public GUIStyle[] _GunsGS = new GUIStyle[10];

	//StarpButton
	public GUIStyle _StartGS = new GUIStyle();
	public float _TextSize = 0.022f;


	//Preview image's
	private Texture2D _ImgCont;

	public Texture2D[] _PlayerPrevImg;
	public Texture2D[] _RobotPrevImg;

	public float _SizePrevImg = 1.4f;
	public float _PosPrevImgX = 3f;
	public float _PosPrevImgY = 3.9f;
	

	//TimerSpawn
	private float _TimerSpawn;
	private string _TimerSpawnView;

	//GUNprev
	float[] _GUNprevSize = new float[4];
	bool [] _ActivGUNprevSize = new bool[4];
	public GUIStyle _GUNprev;
	public static Texture2D[] _GUNprevTexPlayer;
	public static Texture2D[] _GUNprevTexRobo;

//-----------------------------------------------------------------------------------------------------------
	void Start () 
	{
		_Pl_Ro = -1;

		_StartGS.fontSize = Mathf.RoundToInt(Screen.height * _TextSize);

		_BackgroundWidth = Mathf.RoundToInt(Screen.width/_BGWidthСoefficient);
		_BackgroundHeight = Mathf.RoundToInt(Screen.height/_BGHeightСoefficient);

		//BG_set
		_BoxXsp = 0;
		_BoxYsp = 0;
		_BoxX = Screen.width/2 - _BackgroundWidth/2;
		_BoxY = Screen.height/2 - _BackgroundHeight/2;
		_BackGroungRect  = new Rect(_BoxXsp, _BoxYsp, _BackgroundWidth, _BackgroundHeight);

		//ButtonTexture
		for(int t = 0; t < _ButtonPlayerTex.Length; t++)
		{	
			_GSbutton[t].normal.background = _ButtonPlayerTex[t];
			_GSbutton[t].active.background = _ButtonPlayerTexPress[t];
			_GSbutton[t].onNormal.background = _ButtonPlayerTexPress[t];
		}
		for(int t = 0; t < _ButtonRobotTex.Length; t++)
		{	
			_robGSbutton[t].normal.background = _ButtonRobotTex[t];
			_robGSbutton[t].active.background = _ButtonRobotTexPress[t];
			_robGSbutton[t].onNormal.background = _ButtonRobotTexPress[t];
		}
		for(int t = 0; t < _ButtonTeamTex.Length; t++)
		{	
			_TeamGSbutton[t].normal.background = _ButtonTeamTex[t];
			_TeamGSbutton[t].active.background = _ButtonTeamTexPress[t];
			_TeamGSbutton[t].onNormal.background = _ButtonTeamTexPress[t];
		}

		Choice._Player = -1;
		Choice._Robot = -1;
		Choice._Team = -1;

		_TimerSpawn = 5;


		for(int i = 0; i < _GSbutton.Length; i++)
		_GSbutton[i].fontSize = Mathf.RoundToInt(Screen.height * _TextSize);

		for(int i = 0; i < _robGSbutton.Length; i++)
		_robGSbutton[i].fontSize = Mathf.RoundToInt(Screen.height * _TextSize/1.5f);

		for(int i = 0; i < _TeamGSbutton.Length; i++)
		_TeamGSbutton[i].fontSize = Mathf.RoundToInt(Screen.height * _TextSize);

		for(int i = 0; i < _GunsGS.Length; i++)
		_GunsGS[i].fontSize = Mathf.RoundToInt(Screen.height * _TextSize/1.5f);


		_GUNprevTexPlayer = new Texture2D[4];
		_GUNprevTexRobo = new Texture2D[3];

		_GUNprev.alignment = TextAnchor.MiddleCenter;
	}
	
//---------------------------------------------------------------------------------------------------------------
	void FixedUpdate () 
	{

		/*//MouseMenuLol
		if(_OpenAnimate == false && _CloseAnimate == false)
		{
			_BackGroungRect.x = - Mathf.Clamp((Input.mousePosition.x - Screen.width/2 * -5) * 0.05f, -10, 10);
			_BackGroungRect.y = + Mathf.Clamp((Input.mousePosition.y - Screen.height/2 * 5) * 0.05f, -10, 10);
		}*/

//----------------------------------------------------------------------------------------------------------------------------------

		//Preview player, robot, bla-bla-bla
		if(_Pl_Ro == -1)
		{
			_ImgCont = null;
		}

		if(_Pl_Ro == 0)
		{
			_ImgCont = _PlayerPrevImg[Choice._Player];
		}
		if(_Pl_Ro == 1)
		{
			_ImgCont = _RobotPrevImg[Choice._Robot];
		}
		//----------------------------------------------


		/*//TimerSpawn
		if(_TimerSpawn > 0 && Choice._Player != -1 && Choice._Robot != -1 && Choice._Team != -1)
		{
			_TimerSpawn -= Time.deltaTime; 
		}

		if(_TimerSpawn > 0)
		{
		    _TimerSpawnView = Mathf.RoundToInt(_TimerSpawn).ToString();
		}
		else
		{
			_TimerSpawnView = "В БОЙ";
		}*/
	}
	
//-----------------------------------------------------------------------------------------------------------------------------------
	public void DrawMenu ()
	{
		//Background
		GUI.Box(_BackGroungRect, "", _GS);
        
//PlayerMenu-----------------------------------------------------------------

			//Buttons----------------------------------------------------------------------------
			if(GUI.Button(new Rect(Screen.width/_ButtonPosX, Screen.height/_ButtonPosY, Screen.width/_ButtonSizeX, Screen.height/_ButtonSizeY), "ИНЖЕНЕР", _GSbutton[0]))
			{

			    for(int i = 0; i < _GSbutton.Length; i ++)
				{
					_GSbutton[i].normal.background = _ButtonPlayerTex[i];
				    _GSbutton[i].normal.textColor = Color.black;
				}
				_GSbutton[0].normal.background = _ButtonPlayerTexPress[0];
			    _GSbutton[0].normal.textColor = Color.white;

				Choice._Player =(int) GameClassEnum.ENGINEER;

				_Pl_Ro = 0;

			    ResetPrevWind();

			}


			//-----------------------------
			if(GUI.Button(new Rect(Screen.width/_ButtonPosX + Screen.width/_ButtonSizeX, Screen.height/_ButtonPosY, Screen.width/_ButtonSizeX, Screen.height/_ButtonSizeY), "МЕДИК", _GSbutton[1]))
			{

				for(int i = 0; i < _GSbutton.Length; i ++)
				{
					_GSbutton[i].normal.background = _ButtonPlayerTex[i];
				    _GSbutton[i].normal.textColor = Color.black;
				}
				_GSbutton[1].normal.background = _ButtonPlayerTexPress[1];
			    _GSbutton[1].normal.textColor = Color.white;

				Choice._Player = (int)GameClassEnum.MEDIC;
			
				_Pl_Ro =0;

			    ResetPrevWind();

			}
		    

			//-------------------------------
			if(GUI.Button(new Rect(Screen.width/_ButtonPosX + Screen.width/_ButtonSizeX * 2, Screen.height/_ButtonPosY, Screen.width/_ButtonSizeX, Screen.height/_ButtonSizeY), "ШТУРМОВИК", _GSbutton[2]))
			{

				for(int i = 0; i < _GSbutton.Length; i ++)
				{
					_GSbutton[i].normal.background = _ButtonPlayerTex[i];
				    _GSbutton[i].normal.textColor = Color.black;
				}
				_GSbutton[2].normal.background = _ButtonPlayerTexPress[2];
			    _GSbutton[2].normal.textColor = Color.white;

				Choice._Player = (int)GameClassEnum.ASSAULT;

				_Pl_Ro =0;

			    ResetPrevWind();

			}

		    //--------------------------------
			if(GUI.Button(new Rect(Screen.width/_ButtonPosX + Screen.width/_ButtonSizeX * 3, Screen.height/_ButtonPosY, Screen.width/_ButtonSizeX, Screen.height/_ButtonSizeY), "СНАЙПЕР", _GSbutton[3]))
			{

				for(int i = 0; i < _GSbutton.Length; i ++)
				{
					_GSbutton[i].normal.background = _ButtonPlayerTex[i];
				    _GSbutton[i].normal.textColor = Color.black;
				}
				_GSbutton[3].normal.background = _ButtonPlayerTexPress[3];
			    _GSbutton[3].normal.textColor = Color.white;


				Choice._Player = (int)GameClassEnum.SCOUT;

				_Pl_Ro = 0;

			    ResetPrevWind();

			}
		//--------------------------------------------------------------------------


//RobotMenu-----------------------------------------------------------------
			//Buttons----------------------------------------------------------------------------
			if(GUI.Button(new Rect(Screen.width/_robButtonPosX, Screen.height/_robButtonPosY, Screen.width/_robButtonSizeX, Screen.height/_robButtonSizeY), "ТЯЖЁЛЫЙ РОБОТ", _robGSbutton[0]))
		    {		
				for(int i = 0; i < _robGSbutton.Length; i ++)
				{
					_robGSbutton[i].normal.background = _ButtonRobotTex[i];
				    _robGSbutton[i].normal.textColor = Color.black;
				}
				_robGSbutton[0].normal.background = _ButtonRobotTexPress[0];
			    _robGSbutton[0].normal.textColor = Color.white;
			 
				Choice._Robot = 0;

				_Pl_Ro = 1;
				
			    ResetPrevWind();

			}

			if(GUI.Button(new Rect(Screen.width/_robButtonPosX + Screen.width/_robButtonSizeX, Screen.height/_robButtonPosY, Screen.width/_robButtonSizeX, Screen.height/_robButtonSizeY), "СРЕДНИЙ РОБОТ", _robGSbutton[1]))
			{	
				for(int i = 0; i < _robGSbutton.Length; i ++)
				{
					_robGSbutton[i].normal.background = _ButtonRobotTex[i];
				    _robGSbutton[i].normal.textColor = Color.black;
				}
				_robGSbutton[1].normal.background = _ButtonRobotTexPress[1];
			    _robGSbutton[1].normal.textColor = Color.white;

				Choice._Robot = 1;

				_Pl_Ro = 1;

			    ResetPrevWind();
				
			}

			if(GUI.Button(new Rect(Screen.width/_robButtonPosX + Screen.width/_robButtonSizeX * 2, Screen.height/_robButtonPosY, Screen.width/_robButtonSizeX, Screen.height/_robButtonSizeY), "ЛЁГКИЙ РОБОТ", _robGSbutton[2]))
			{	
				for(int i = 0; i < _robGSbutton.Length; i ++)
				{
					_robGSbutton[i].normal.background = _ButtonRobotTex[i];
				    _robGSbutton[i].normal.textColor = Color.black;
				}
				_robGSbutton[2].normal.background = _ButtonRobotTexPress[2];
			    _robGSbutton[2].normal.textColor = Color.white;

				Choice._Robot = 2;

				_Pl_Ro = 1;

			    ResetPrevWind();
				
			}	
			
//--------------------------------------------------------------------------
	//GunsMenu
		GUI.Box(new Rect(Screen.width/16, Screen.height/4.2f, Screen.width/6, Screen.height/24), "СНАРЯЖЕНИЕ", _GunsGS[0]);
	
		//Player
		if(_Pl_Ro == 0)
		{
			float _lol = _GUNprevSize[0] + _GUNprevSize[1] + _GUNprevSize[2];

			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 2), Screen.width/6, Screen.height/24), "ЛИЧНОЕ", _GunsGS[1]))
			{	
				WeaponPlayer._IntActive = 1;

				for(int i = 0; i < _ActivGUNprevSize.Length; i++)
				{
					_ActivGUNprevSize[i] = false;
				}
				_ActivGUNprevSize[0] = true;
			}

			if(_ActivGUNprevSize[0] == true && _GUNprevSize[0] < Screen.width/12)
			{
				_GUNprevSize[0] = Mathf.Lerp(_GUNprevSize[0], Screen.width/12, Time.deltaTime * 4);
				GUI.Box(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 3), Screen.width/6, _GUNprevSize[0]), _GUNprevTexPlayer[0],  _GUNprev);
			}

			if(_ActivGUNprevSize[0]  == false)
			_GUNprevSize[0] = Mathf.Lerp(_GUNprevSize[0], 0, Time.deltaTime * 4);

			//----------------------------------------------------------------------------------------------------------------------------------------



			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 3) + _GUNprevSize[0], Screen.width/6, Screen.height/24), "ОСНОВНОЕ", _GunsGS[2]))
			{	
				WeaponPlayer._IntActive = 2;

				for(int i = 0; i < _ActivGUNprevSize.Length; i++)
				{
					_ActivGUNprevSize[i] = false;
				}
				_ActivGUNprevSize[1] = true;
			}

			if(_ActivGUNprevSize[1] == true && _GUNprevSize[1] < Screen.width/12 && _GUNprevSize[0] < 5)
			{
				_GUNprevSize[1] = Mathf.Lerp(_GUNprevSize[1], Screen.width/12, Time.deltaTime * 4);
				GUI.Box(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 4), Screen.width/6, _GUNprevSize[1]), _GUNprevTexPlayer[1], _GUNprev);
			}
			
			if(_ActivGUNprevSize[1]  == false)
			_GUNprevSize[1] = Mathf.Lerp(_GUNprevSize[1], 0, Time.deltaTime * 4);
			//---------------------------------------------------------------------------------------------------------------------------------------

			

			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 4) + _GUNprevSize[0] + _GUNprevSize[1], Screen.width/6, Screen.height/24), "ДОПОЛНИТЕЛЬНОЕ", _GunsGS[3]))
			{	
				WeaponPlayer._IntActive = 3;

				for(int i = 0; i < _ActivGUNprevSize.Length; i++)
				{
					_ActivGUNprevSize[i] = false;
				}
				_ActivGUNprevSize[2] = true;
			}
			if(_ActivGUNprevSize[2] == true && _GUNprevSize[2] < Screen.width/12 && _GUNprevSize[0] + _GUNprevSize[1] < 5)
			{
				_GUNprevSize[2] = Mathf.Lerp(_GUNprevSize[2], Screen.width/12, Time.deltaTime * 4);
				GUI.Box(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 5), Screen.width/6, _GUNprevSize[2]), _GUNprevTexPlayer[2],  _GUNprev);
			}
			
			if(_ActivGUNprevSize[2]  == false)
				_GUNprevSize[2] = Mathf.Lerp(_GUNprevSize[2], 0, Time.deltaTime * 4);
			//---------------------------------------------------------------------------------------------------------------------------------------



			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 5) + _lol, Screen.width/6, Screen.height/24), "ГРАНАТЫ", _GunsGS[4]))
			{	
				WeaponPlayer._IntActive = 4;

				for(int i = 0; i < _ActivGUNprevSize.Length; i++)
				{
					_ActivGUNprevSize[i] = false;
				}
				_ActivGUNprevSize[3] = true;
			}
			if(_ActivGUNprevSize[3] == true && _GUNprevSize[3] < Screen.width/12 && _lol < 5)
			{
				_GUNprevSize[3] = Mathf.Lerp(_GUNprevSize[3], Screen.width/12, Time.deltaTime * 4);
				GUI.Box(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 6), Screen.width/6, _GUNprevSize[3]), _GUNprevTexPlayer[3], _GUNprev);
			}
			
			if(_ActivGUNprevSize[3]  == false)
				_GUNprevSize[3] = Mathf.Lerp(_GUNprevSize[3], 0, Time.deltaTime * 4);


		}
		//Player------------------
	
		//Robot
		if(_Pl_Ro == 1)
		{
			
			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 2), Screen.width/6, Screen.height/24), "БЛИЖНИЙ БОЙ", _GunsGS[5]))
			{	
				WeaponPlayer._IntActiveRobo = 1;

				for(int i = 0; i < _ActivGUNprevSize.Length; i++)
				{
					_ActivGUNprevSize[i] = false;
				}
				_ActivGUNprevSize[0] = true;
			}
			
			if(_ActivGUNprevSize[0] == true && _GUNprevSize[0] < Screen.width/12)
			{
				_GUNprevSize[0] = Mathf.Lerp(_GUNprevSize[0], Screen.width/12, Time.deltaTime * 4);
				GUI.Box(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 3), Screen.width/6, _GUNprevSize[0]), _GUNprevTexRobo[0],  _GUNprev);
			}
			
			if(_ActivGUNprevSize[0]  == false)
				_GUNprevSize[0] = Mathf.Lerp(_GUNprevSize[0], 0, Time.deltaTime * 4);
			//-----------------------------------------------------------------------------------------------------------------------------------------

			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 3) + _GUNprevSize[0], Screen.width/6, Screen.height/24), "ОСНОВНОЕ", _GunsGS[6]))
			{	
				WeaponPlayer._IntActiveRobo = 2;

				for(int i = 0; i < _ActivGUNprevSize.Length; i++)
				{
					_ActivGUNprevSize[i] = false;
				}
				_ActivGUNprevSize[1] = true;
			}
			
			if(_ActivGUNprevSize[1] == true && _GUNprevSize[1] < Screen.width/12 && _GUNprevSize[0] < 5)
			{
				_GUNprevSize[1] = Mathf.Lerp(_GUNprevSize[1], Screen.width/12, Time.deltaTime * 4);
				GUI.Box(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 4), Screen.width/6, _GUNprevSize[1]), _GUNprevTexRobo[1], _GUNprev);
			}
			
			if(_ActivGUNprevSize[1]  == false)
				_GUNprevSize[1] = Mathf.Lerp(_GUNprevSize[1], 0, Time.deltaTime * 4);
			//-----------------------------------------------------------------------------------------------------------------------------------------



			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 4) + _GUNprevSize[0] + _GUNprevSize[1], Screen.width/6, Screen.height/24), "ТЯЖЁЛОЕ", _GunsGS[7]))
			{	
				WeaponPlayer._IntActiveRobo = 3;

				for(int i = 0; i < _ActivGUNprevSize.Length; i++)
				{
					_ActivGUNprevSize[i] = false;
				}
				_ActivGUNprevSize[2] = true;
			}
			if(_ActivGUNprevSize[2] == true && _GUNprevSize[2] < Screen.width/12 && _GUNprevSize[0] + _GUNprevSize[1] < 5)
			{
				_GUNprevSize[2] = Mathf.Lerp(_GUNprevSize[2], Screen.width/12, Time.deltaTime * 4);
				GUI.Box(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 5), Screen.width/6, _GUNprevSize[2]), _GUNprevTexRobo[2],  _GUNprev);
			}
			
			if(_ActivGUNprevSize[2]  == false)
				_GUNprevSize[2] = Mathf.Lerp(_GUNprevSize[2], 0, Time.deltaTime * 4);
			//-----------------------------------------------------------------------------------------------------------------------------------------

		}
		//Robot--------------------
//GunsMenu----------------------------------------------------------------------

		//SetImage
		if(_ImgCont != null)
		{
			GUI.Label(new Rect(Screen.width/_PosPrevImgX, Screen.height/_PosPrevImgY, Screen.width/_SizePrevImg, Screen.height/_SizePrevImg), _ImgCont);
		}




	//Team
		int[] teamCount = new int[2];
		Player[] players = PlayerManager.instance.FindAllPlayer ();
		foreach(Player player in players) {
			if(player.team!=0){
				teamCount[player.team-1]++;
			}
		}
		bool teamAblock=false,teamBblock=false;
		if(teamCount[0]>teamCount[1]+1){
			teamAblock=true;
		}
		if(teamCount[1]>teamCount[0]+1){
			teamBblock=true;
		}
		if(GUI.Button(new Rect(Screen.width/_TeamButtonPosX, Screen.height/_TeamButtonPosY, Screen.width/_TeamButtonSizeX, Screen.height/_TeamButtonSizeY), PlayerMainGui.FormTeamName(1)+" ("+teamCount[0]+")", _TeamGSbutton[0])&&!teamAblock)
		{
			
			for(int i = 0; i <  _TeamGSbutton.Length; i ++)
			{
				_TeamGSbutton[i].normal.background = _ButtonTeamTex[i];
				_TeamGSbutton[i].normal.textColor = Color.black;
			}
			_TeamGSbutton[0].normal.background = _ButtonTeamTexPress[0];
			_TeamGSbutton[0].normal.textColor = Color.white;
			
			
			Choice._Team = 1;
		}

		if(GUI.Button(new Rect(Screen.width/_TeamButtonPosX + Screen.width/_TeamButtonSizeX, Screen.height/_TeamButtonPosY, Screen.width/_TeamButtonSizeX, Screen.height/_TeamButtonSizeY), PlayerMainGui.FormTeamName(2)+" ("+teamCount[1]+")", _TeamGSbutton[1])&&!teamBblock)
		{
			
			for(int i = 0; i < _TeamGSbutton.Length; i ++)
			{
				_TeamGSbutton[i].normal.background = _ButtonTeamTex[i];
				_TeamGSbutton[i].normal.textColor = Color.black;
			}
			_TeamGSbutton[1].normal.background = _ButtonTeamTexPress[1];
			_TeamGSbutton[1].normal.textColor = Color.white;
			
			Choice._Team = 2;
		}
	//Team-------------------------
/*

	//StartButton
		if(Choice._Player != -1 && Choice._Robot != -1 && Choice._Team != -1)
		{
			if(GUI.Button(new Rect(Screen.width/2 - (Screen.width/5)/2 , Screen.height/4 * 3.5f, Screen.width/5, Screen.height/15), "В БОЙ", _StartGS))
			{
				if(_TimerSpawn < 0)
				{

				}
			}
		}
	//StartButton-------------------

	*/
	}//GUI


	//StartButton
	public bool SetTimer(int timer){
		if(timer > 0)
		{
			_TimerSpawnView = Mathf.RoundToInt(timer).ToString();
		}
		else
		{
			_TimerSpawnView = "В БОЙ";
		}
		return GUI.Button (new Rect (Screen.width / 2 - (Screen.width / 5) / 2, Screen.height / 4 * 3.5f, Screen.width / 5, Screen.height / 15), _TimerSpawnView, _StartGS)&&timer<=0;
	}
	//StartButton-------------------
	void ResetPrevWind()
	{
		for(int i = 0; i < _ActivGUNprevSize.Length; i++)
		{
			_ActivGUNprevSize[i]  = false;

		}
		_ActivGUNprevSize[1]  = true;
		WeaponPlayer._IntActive = 2;
		WeaponPlayer._IntActiveRobo = 1;
	//	ItemManager.instance.defSettings[Choice._Player];
		for(int i = 0; i<4;i++){
			int index = Choice.ForGuiSlot(i);
			if(index!=-1){
				_GUNprevTexPlayer[i] =ItemManager.instance.weaponIndexTable [index].textureGUI;
			}
		}
	
		
	}

}


public static class Choice
{
	public static int _Player;
	public static int _Robot;
	public static int _Team;

	//PlayerWeapons
	public static int[] _Personal = new int[4]{-1,-1,-1,-1};
	public static int[] _Main = new int[4]{-1,-1,-1,-1};
	public static int[] _Extra = new int[4]{-1,-1,-1,-1};
	public static int[] _Grenad = new int[4]{-1,-1,-1,-1};
	public static int[] _Taunt = new int[4]{-1,-1,-1,-1};
	
	public static int ForGuiSlot(int slot){
		switch(slot){
			case 0:
				return _Personal[_Player];
				break;
			case 1:
				return _Main[_Player];
				break;
			case 2:
				return _Extra[_Player];
				break;
			case 3:
				return _Grenad[_Player];
				break;
			case 4:
				return _Taunt[_Player];
				break;	
				
		}
		return -1;
	}
	public static void SetChoice(int slot, int gameClass, int index){
		switch(slot){
			case 0:
			 	_Personal[gameClass]=index;
				break;
			case 1:
				 _Main[gameClass]=index;
				break;
			case 2:
				 _Extra[gameClass]=index;
				break;
			case 3:
				 _Grenad[gameClass]=index;
				break;
			case 4:
				 _Taunt[gameClass]=index;
				break;
		}
	}
	
	//RoboWeapons
	public static int[] _MeleeRobo = new int[3]{-1,-1,-1};
	public static int[] _MainRobo = new int[3]{-1,-1,-1};
	public static int[] _HeavyRobo = new int[3]{-1,-1,-1};
	public static int[] _TauntRobo = new int[3]{-1,-1,-1};
	public static void SetChoiceRobot(int slot, int gameClass, int index){
		switch(slot){
			case 0:
				 _MeleeRobo[gameClass]=index;
				break;
			case 1:
				 _MainRobo[gameClass]=index;
				break;
			case 2:
				 _HeavyRobo[gameClass]=index;
				break;
			case 3:
				 _TauntRobo[gameClass]=index;
				break;
		
		}
	}
	public static int ForGuiSlotRobot(int slot){
		switch(slot){
			case 0:
				return _MeleeRobo[_Robot];
				break;
			case 1:
				return _MainRobo[_Robot];
				break;
			case 2:
				return _HeavyRobo[_Robot];
				break;
			case 3:
				return _TauntRobo[_Robot];
				break;
		}
		return -1;
	}
}
