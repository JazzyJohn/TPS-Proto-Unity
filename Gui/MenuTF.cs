using UnityEngine;
using System.Collections;
//[ExecuteInEditMode]

public class MenuTF : MonoBehaviour {

	private int _Pl_Ro; //0 - player, 1 -robot

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

	public Color[] _ButtonTeamFontColor;
	public Color[] _ButtonTeamFontColorPress;

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

//-----------------------------------------------------------------------------------------------------------
	void Start () 
	{
		_Pl_Ro = -1;
		int fontSize= Mathf.RoundToInt(Screen.width * _TextSize);
		_StartGS.fontSize = fontSize;
		_TeamGSbutton[0].fontSize = fontSize;
		_TeamGSbutton[1].fontSize = fontSize;
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



		//_OpenAnimate = true;
	}
	
//---------------------------------------------------------------------------------------------------------------
	void FixedUpdate () 
	{
		//Debug.Log(Choice._Robot + "lol" + Choice._Player);

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
		}
		*/
	}
	
//-----------------------------------------------------------------------------------------------------------------------------------
	public void DrawMenu ()
	{
		//Background
		GUI.Box(_BackGroungRect, "", _GS);
        
//PlayerMenu-----------------------------------------------------------------

			//Buttons----------------------------------------------------------------------------
			if(GUI.Button(new Rect(Screen.width/_ButtonPosX, Screen.height/_ButtonPosY, Screen.width/_ButtonSizeX, Screen.height/_ButtonSizeY), "", _GSbutton[0]))
			{

			    for(int i = 0; i < _GSbutton.Length; i ++)
				{
					_GSbutton[i].normal.background = _ButtonPlayerTex[i];
				}
				_GSbutton[0].normal.background = _ButtonPlayerTexPress[0];


				Choice._Player = 0;

				_Pl_Ro = 0;
	
			}


			//-----------------------------
			if(GUI.Button(new Rect(Screen.width/_ButtonPosX + Screen.width/_ButtonSizeX, Screen.height/_ButtonPosY, Screen.width/_ButtonSizeX, Screen.height/_ButtonSizeY), "", _GSbutton[1]))
			{

				for(int i = 0; i < _GSbutton.Length; i ++)
				{
					_GSbutton[i].normal.background = _ButtonPlayerTex[i];
				}
				_GSbutton[1].normal.background = _ButtonPlayerTexPress[1];


				Choice._Player = 1;
			
				_Pl_Ro = 0;

			}
		    

			//-------------------------------
			if(GUI.Button(new Rect(Screen.width/_ButtonPosX + Screen.width/_ButtonSizeX * 2, Screen.height/_ButtonPosY, Screen.width/_ButtonSizeX, Screen.height/_ButtonSizeY), "", _GSbutton[2]))
			{

				for(int i = 0; i < _GSbutton.Length; i ++)
				{
					_GSbutton[i].normal.background = _ButtonPlayerTex[i];
				}
				_GSbutton[2].normal.background = _ButtonPlayerTexPress[2];


				Choice._Player = 2;

				_Pl_Ro = 0;

			}

		    //--------------------------------
			if(GUI.Button(new Rect(Screen.width/_ButtonPosX + Screen.width/_ButtonSizeX * 3, Screen.height/_ButtonPosY, Screen.width/_ButtonSizeX, Screen.height/_ButtonSizeY), "", _GSbutton[3]))
			{

				for(int i = 0; i < _GSbutton.Length; i ++)
				{
					_GSbutton[i].normal.background = _ButtonPlayerTex[i];
				}
				_GSbutton[3].normal.background = _ButtonPlayerTexPress[3];



				Choice._Player = 3;

				_Pl_Ro = 0;

			}
		//--------------------------------------------------------------------------


//RobotMenu-----------------------------------------------------------------
			//Buttons----------------------------------------------------------------------------
			if(GUI.Button(new Rect(Screen.width/_robButtonPosX, Screen.height/_robButtonPosY, Screen.width/_robButtonSizeX, Screen.height/_robButtonSizeY), "", _robGSbutton[0]))
		    {		
				for(int i = 0; i < _robGSbutton.Length; i ++)
				{
					_robGSbutton[i].normal.background = _ButtonRobotTex[i];
				}
				_robGSbutton[0].normal.background = _ButtonRobotTexPress[0];


				Choice._Robot = 0;

				_Pl_Ro = 1;
				
			}

			if(GUI.Button(new Rect(Screen.width/_robButtonPosX + Screen.width/_robButtonSizeX, Screen.height/_robButtonPosY, Screen.width/_robButtonSizeX, Screen.height/_robButtonSizeY), "", _robGSbutton[1]))
			{	
				for(int i = 0; i < _robGSbutton.Length; i ++)
				{
					_robGSbutton[i].normal.background = _ButtonRobotTex[i];
				}
				_robGSbutton[1].normal.background = _ButtonRobotTexPress[1];


				Choice._Robot = 1;

				_Pl_Ro = 1;
				
			}

			if(GUI.Button(new Rect(Screen.width/_robButtonPosX + Screen.width/_robButtonSizeX * 2, Screen.height/_robButtonPosY, Screen.width/_robButtonSizeX, Screen.height/_robButtonSizeY), "", _robGSbutton[2]))
			{	
				for(int i = 0; i < _robGSbutton.Length; i ++)
				{
					_robGSbutton[i].normal.background = _ButtonRobotTex[i];
				}
				_robGSbutton[2].normal.background = _ButtonRobotTexPress[2];


				Choice._Robot = 2;

				_Pl_Ro = 1;
				
			}	
			
//--------------------------------------------------------------------------
	//GunsMenu
		GUI.Box(new Rect(Screen.width/16, Screen.height/4.2f, Screen.width/6, Screen.height/24), "", _GunsGS[0]);
	
		//Player
		if(_Pl_Ro == 0)
		{
		
			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 2), Screen.width/6, Screen.height/24), "", _GunsGS[1]))
			{	


			}

			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 3), Screen.width/6, Screen.height/24), "", _GunsGS[2]))
			{	
				
				
			}

			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 4), Screen.width/6, Screen.height/24), "", _GunsGS[3]))
			{	

			
			}

			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 5), Screen.width/6, Screen.height/24), "", _GunsGS[4]))
			{	
				
				
			}

		}
		//Player------------------
	
		//Robot
		if(_Pl_Ro == 1)
		{
			
			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 2), Screen.width/6, Screen.height/24), "", _GunsGS[5]))
			{	
				
				
			}
			
			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 3), Screen.width/6, Screen.height/24), "", _GunsGS[6]))
			{	
				
				
			}
			
			if(GUI.Button(new Rect(Screen.width/16, Screen.height/4.2f + (Screen.height/24 * 4), Screen.width/6, Screen.height/24), "", _GunsGS[7]))
			{	
				
				
			}

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
		if(GUI.Button(new Rect(Screen.width/_TeamButtonPosX, Screen.height/_TeamButtonPosY, Screen.width/_TeamButtonSizeX, Screen.height/_TeamButtonSizeY), "Команда 1 ("+teamCount[0]+")", _TeamGSbutton[0]))
		{
			
			for(int i = 0; i <  _TeamGSbutton.Length; i ++)
			{
				_TeamGSbutton[i].normal.background = _ButtonTeamTex[i];
				_TeamGSbutton[i].normal.textColor =_ButtonTeamFontColor[i];
			}
			_TeamGSbutton[0].normal.background = _ButtonTeamTexPress[0];
			_TeamGSbutton[0].normal.textColor =_ButtonTeamFontColorPress[0];
			
			
			Choice._Team = 1;
		}

		if(GUI.Button(new Rect(Screen.width/_TeamButtonPosX + Screen.width/_TeamButtonSizeX, Screen.height/_TeamButtonPosY, Screen.width/_TeamButtonSizeX, Screen.height/_TeamButtonSizeY), "Команда 2 ("+teamCount[1]+")", _TeamGSbutton[1]))
		{
			
			for(int i = 0; i < _TeamGSbutton.Length; i ++)
			{
				_TeamGSbutton[i].normal.background = _ButtonTeamTex[i];
				_TeamGSbutton[i].normal.textColor =_ButtonTeamFontColor[i];
			}
			_TeamGSbutton[1].normal.background = _ButtonTeamTexPress[1];
			_TeamGSbutton[1].normal.textColor =_ButtonTeamFontColorPress[1];
			
			
			Choice._Team = 2;
		}
	//Team-------------------------


	
		/*if(Choice._Player != -1 && Choice._Robot != -1 && Choice._Team != -1)
		{
			if(GUI.Button(new Rect(Screen.width/2 - (Screen.width/5)/2 , Screen.height/4 * 3.5f, Screen.width/5, Screen.height/15), _TimerSpawnView, _StartGS))
			{
				if(_TimerSpawn < 0)
				{

				}
			}
		}*/


	
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
		return GUI.Button (new Rect (Screen.width / 2 - (Screen.width / 5) / 2, Screen.height / 4 * 3.5f, Screen.width / 5, Screen.height / 15), _TimerSpawnView, _StartGS);
	}
	//StartButton-------------------
}


public static class Choice
{
	public static int _Player;
	public static int _Robot;
	public static int _Team;
}
