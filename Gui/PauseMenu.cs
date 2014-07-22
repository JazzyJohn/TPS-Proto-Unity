using UnityEngine;
using System;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public GameObject camera;
	public SettingGUI Setting;
	public UIPanel Menu;

	public UIPanel PausePanel;

	public bool Pause;

	public UIPanel PlayGUI;

	public PlayerHudNgui MainHUD;

	private PlayerMainGui PlayerGUI;


	public bool _VisableSetting;

	public bool VisableSetting
	{
		get
		{
			return _VisableSetting;
		}
		set
		{
			Setting.Visable = value;
			_VisableSetting = value;
			Menu.alpha = Convert.ToSingle(!value);
		}
	}

	// Use this for initialization
	void Start () 
	{
		camera = GameObject.Find("Main Camera");
		PlayerGUI = FindObjectOfType<PlayerMainGui>();
	}

	public void GoToMainMenu()
	{
    
		Pause = false;
		camera.GetComponent<BlurEffect>().enabled = false;
		Screen.lockCursor = false;
        
        Application.LoadLevel(0);

         
	}
     public void ActivateMenu(){
         if (PausePanel.alpha == 0f)
		{
                Pause = true;
				camera.GetComponent<BlurEffect>().enabled = true;
				PlayerGUI.guiState = PlayerMainGui.GUIState.Pause;
				Screen.lockCursor = false;
				PlayGUI.alpha = 0f;
				VisableSetting = false;
				PausePanel.alpha = 1f;
			
		}
    }
     public bool IsActive()
     {
         if (InputManager.instance.GetButtonDown("Pause"))
         {
         
             return !Pause;
         }
         return Pause;
     }
	public void BackToGame()
	{
      
            Pause = false;
            camera.GetComponent<BlurEffect>().enabled = false;
            PlayerGUI.guiState = PlayerMainGui.GUIState.Normal;
            
            Setting.CearControlls();
            PausePanel.alpha = 0f;
            PlayGUI.alpha = 1f;
        
	}


}