using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIItem{
	public int index;
	public Texture2D texture;
	public string name;
}

public class WeaponPlayer : MonoBehaviour {


	public static int _IntActive;

	//Robo
	public int _MeleeCrobo;
	public Texture2D[] _MeleePrevRobo;
	public string _MeleeRoboName;
	
	public int _MainCrobo;
	public Texture2D[] _MainPrevRobo;
	public string  _MainRoboName;
	
	public int _HeavyCrobo;
	public Texture2D[] _HeavyPrevRobo;
	public string _HeavyRoboName;
	
	public static int _IntActiveRobo=2;


	//Button
	public float _ButtSizeX = 6f;
	public float _ButtSizeY = 6f;

	public GUIStyle _PersonalGS;

	public GUIStyle[] _MeleeGSrobo;
	public GUIStyle[] _MainGSrobo;
	public GUIStyle[] _HeavyGSrobo;

	void Start () 
	{
		/*_PersonalGS = new GUIStyle[_PersonalC];
		_MainGS = new GUIStyle[_MainC];
		_ExtraGS = new GUIStyle[_ExtraC];
		_GrenadeGS = new GUIStyle[_GrenadeC];
		
		_MeleeGSrobo = new GUIStyle[_MeleeCrobo];
		_MainGSrobo = new GUIStyle[_MainCrobo];
		_HeavyGSrobo = new GUIStyle[_HeavyCrobo];*/
	}

	void Update () 
	{
	
	}

	public void DrawMenu ()
	{

		GUI.depth = -1;

//Player--------------------------------------------------------------------------------------------------------------------------------------------------

		if (MenuTF._Pl_Ro == 0) {
						List<GUIItem> listOfItems = ItemManager.instance.GetItemForSlot ((GameClassEnum)Choice._Player, (_IntActive-1));
						switch (_IntActive) {

						//Main
						case 1:
								for (int i = 0; i < listOfItems.Count; i++) {
										_PersonalGS.normal.background = listOfItems[i].texture;
										if (GUI.Button (new Rect (Screen.width / 4, Screen.height / 3.1f + (Screen.height / _ButtSizeY) * i, Screen.width / _ButtSizeX, Screen.height / _ButtSizeY), listOfItems [i].name, _PersonalGS )) {	
												Choice._Personal = listOfItems[i].index;
												MenuTF._GUNprevTexPlayer [0] = listOfItems[i].texture;
										}
								}
								break;
						//Personal
						case 2:
								for (int i = 0; i < listOfItems.Count; i++) {
										_PersonalGS.normal.background = listOfItems[i].texture;
										if (GUI.Button (new Rect (Screen.width / 4, Screen.height / 3.1f + (Screen.height / _ButtSizeY) * i, Screen.width / _ButtSizeX, Screen.height / _ButtSizeY),listOfItems [i].name, _PersonalGS )) {	
												Choice._Main = listOfItems[i].index;
												MenuTF._GUNprevTexPlayer [1] = listOfItems[i].texture;
										}
								}

								break;
						//Extra
						case 3:

								for (int i = 0; i < listOfItems.Count; i++) {
										_PersonalGS.normal.background = listOfItems[i].texture;
										if (GUI.Button (new Rect (Screen.width / 4, Screen.height / 3.1f + (Screen.height / _ButtSizeY) * i, Screen.width / _ButtSizeX, Screen.height / _ButtSizeY),listOfItems [i].name, _PersonalGS )) {	
												Choice._Extra = listOfItems[i].index;
												MenuTF._GUNprevTexPlayer [2] = listOfItems[i].texture;
										}
								}
								break;
						//Grenade
						case 4:
								for (int i = 0; i < listOfItems.Count; i++) {
										_PersonalGS.normal.background = listOfItems[i].texture;
										if (GUI.Button (new Rect (Screen.width / 4, Screen.height / 3.1f + (Screen.height / _ButtSizeY) * i, Screen.width / _ButtSizeX, Screen.height / _ButtSizeY),listOfItems [i].name, _PersonalGS )) {	
												Choice._Grenade = listOfItems[i].index;
												MenuTF._GUNprevTexPlayer [3] = listOfItems[i].texture;
										}
								}
								break;

						}

				}
		
	

	
	


//Robo--------------------------------------------------------------------------------------------------------------------------------------------------

		if(MenuTF._Pl_Ro == 1)
		{
			/*//Melee
		if(_IntActiveRobo == 1)
			for(int i = 0; i < _MeleeCrobo; i++)
		{
				if(GUI.Button(new Rect(Screen.width/4, Screen.height/3.1f + (Screen.height/_ButtSizeY)*i, Screen.width/_ButtSizeX, Screen.height/_ButtSizeY), "", _MeleeGSrobo[i]))
			{	
				Choice._MeleeRobo = i;
				MenuTF._GUNprevTexRobo[0] = _MeleePrevRobo[i];
			}
		}*/
		
		//Main
		/*if(_IntActiveRobo == 2)
			for(int i = 0; i < _MainC; i++)
		{
				if(GUI.Button(new Rect(Screen.width/4, Screen.height/3.1f + (Screen.height/_ButtSizeY)*i, Screen.width/_ButtSizeX, Screen.height/_ButtSizeY), "", _MainGSrobo[i]))
			{	
				Choice._MainRobo = i;
				MenuTF._GUNprevTexRobo[1] = _MainPrevRobo[i];
			}
		}
		
		//Heavy
		if(_IntActiveRobo == 3)
			for(int i = 0; i < _ExtraC; i++)
		{
				if(GUI.Button(new Rect(Screen.width/4, Screen.height/3.1f + (Screen.height/_ButtSizeY)*i, Screen.width/_ButtSizeX, Screen.height/_ButtSizeY), "", _HeavyGSrobo[i]))
			{	
				Choice._HeavyRobo = i;
				MenuTF._GUNprevTexRobo[2] = _HeavyPrevRobo[i];
			}
		}*/
		//------------------------------------------------------------------------------------------------------------------------------------------
		}

	}

}
