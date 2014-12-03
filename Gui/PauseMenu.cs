using UnityEngine;
using System;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public GameObject camera;
    public BlurEffect effect;
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

    void Awake()
    {
       camera = GameObject.Find("Main Camera");
       effect =camera.GetComponent<BlurEffect>();
       PlayerGUI = FindObjectOfType<PlayerMainGui>();
    }
	
	public void GoToMainMenu()
	{
        GA.API.Design.NewEvent("GUI:Pause:MainMenu" ); 
		Pause = false;
        if (effect != null)
        {
            effect.enabled = false;
        }
		Screen.lockCursor = false;
        NetworkController.Instance.LeaveRoomReuqest();
     
        Application.LoadLevel(0);

         
	}
     public void ActivateMenu(){
         if (PausePanel.alpha == 0f)
         {
                GA.API.Design.NewEvent("GUI:Pause:Show"); 
                Pause = true;
                if (effect != null)
                {
                    effect.enabled = true;
                }
				PlayerGUI.guiState = PlayerMainGui.GUIState.Pause;
				Screen.lockCursor = false;
				PlayGUI.alpha = 0f;
				VisableSetting = false;
				PausePanel.alpha = 1f;
			
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
            if (effect != null)
            {
                effect.enabled = false;
            }
            PlayerGUI.guiState = PlayerMainGui.GUIState.Normal;
            if (Setting != null)
            {
                Setting.CearControlls();
            }
            PausePanel.alpha = 0f;
            PlayGUI.alpha = 1f;

        
	}


}