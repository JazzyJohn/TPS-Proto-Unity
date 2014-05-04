using UnityEngine;
using System.Collections;
//[ExecuteInEditMode]

public class MenuTF : MonoBehaviour {

	private int _Pl_Ro; //0 - player, 1 -robot

	private int _PlayerNum;
	private int _RobotNum;

	//public float _TextSize;

	
//Background-----------------------
	//0.8f
	public float _BGWidthСoefficient;
	public float _BGHeightСoefficient;

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
	
	public bool[] _playerActiveButton = new bool[3];

	//Robot
	public float _robButtonSizeX = 14f;
	public float _robButtonSizeY = 14f;
	public float _robButtonPosX = 2.5f;
	public float _robButtonPosY = 5.9f;

	public Texture2D[] _ButtonRobotTex;
	public Texture2D[] _ButtonRobotTexPress;
	
	public GUIStyle[] _robGSbutton = new GUIStyle[3];

	public bool[] _robotActiveButton;

	//Y
	public int _ButtonIntervalY = 10;
	//X
	public int _ButtonIntervalX = 10;
//---------------------------------


	public GUIStyle _GS = new GUIStyle();

	public GUIStyle[] _GunsGS = new GUIStyle[10];

	public GUIStyle _TS = new GUIStyle ();

	//Preview image's
	private Texture2D _ImgCont;

	public Texture2D[] _PlayerPrevImg;
	public Texture2D[] _RobotPrevImg;

	public float _SizePrevImg = 1.4f;
	public float _PosPrevImgX = 3.4f;
	public float _PosPrevImgY = 3.9f;

//-----------------------------------------------------------------------------------------------------------
	void Start () 
	{
		_Pl_Ro = -1;
		_playerActiveButton = new bool[4];
		_robotActiveButton = new bool[3];

		//_GS.fontSize = Mathf.RoundToInt(Screen.width * _TextSize);

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

		Choice._Player = -1;
		Choice._Robot = -1;
		Choice._Team = -1;
		//_OpenAnimate = true;

	}
	
//---------------------------------------------------------------------------------------------------------------
	void Update () 
	{
		//Debug.Log(Choice._Robot + "lol" + Choice._Player);

		/*//OpenAnimate
		if(_OpenAnimate == true)
		{
			_BackGroungRect.x += 20f;

			if(_BackGroungRect.x >= _BoxX)
			{
				_OpenAnimate = false;
				_BackGroungRect.x = _BoxX;
			}
		}
		//CloseAnimate
		if(_CloseAnimate == true)
		{
			_BackGroungRect.x -= 20f;
			
			if(_BackGroungRect.x <= _BoxXsp)
			{
				_CloseAnimate = false;
				_BackGroungRect.x = _BoxXsp;
				_OpenAnimate = true;
			}
		}

		//MouseMenuLol
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

				for(int i = 0; i < _playerActiveButton.Length; i ++)
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


		//SetImage
		if(_ImgCont != null)
		{
			GUI.Label(new Rect(Screen.width/_PosPrevImgX, Screen.height/_PosPrevImgY, Screen.width/_SizePrevImg, Screen.height/_SizePrevImg), _ImgCont);
		}
#region selectTeam
		for (int i = 1; i<3; i++) {
			if (GUI.Button (new Rect(
						i * (Screen.width * 0.2f), 
						0, 
						Screen.width * 0.2f, 
						Screen.width * 0.05f
				), PlayerMainGui.FormTeamName(i),_TS)) {
				Choice._Team = i;
			}
		}
#endregion

	}//GUI

}


public static class Choice
{
	public static int _Player=-1;
	public static int _Robot=-1;
	public static int _Team=-1;	
}
