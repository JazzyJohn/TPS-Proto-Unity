﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SettingGUI : MonoBehaviour {

	public bool FullScreen_Z;

	public enum _TypeSettingPanel{MainMenu, GameMode};

	public _TypeSettingPanel TypeSettingPanel;
	
	[SerializeField]
	private bool _Visable;
	private UIPanel panel;
	private PauseMenu Pause;
	
	public bool Visable
	{
		get
		{
			return _Visable;
		}
		set
		{
			_Visable = value;
			panel.alpha = Convert.ToSingle(value);
		}
	}

	public UIPanel control;
	public UIPanel video;
	public UIPanel game;

	public Volumes volumes;
	public List<string> AllResolution;
	
	public GraphicSetting graphicSetting; 

	public _Control Control;

	private MainMenuGUI MainMenu;

	[System.Serializable]
	public class _Control
	{
		public List<SettingCommand> controls = new List<SettingCommand>();
		public List<GameObject> toggleList;

		public class SettingCommand{
			public UIToggle codename;
			public UILabel keyname;
			public string command;			
		}

		public UIScrollBar mouseSensitivity;
		public UILabel mouseLabel;
	}

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

	public void HideAllSettingsPanel() {
		video.alpha = 0f;
		control.alpha = 0f;
	}
	public void ShowControl() {
		if(  control.alpha ==0){
			CearControlls();				
		}
		
		HideAllSettingsPanel();
		control.alpha = 1f;
		
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
				foreach(_Control.SettingCommand setCommand in Control.controls){
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
				foreach (_Control.SettingCommand setCommand in Control.controls)
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

	public void ShowVideo()
	{
     
		HideAllSettingsPanel();
		video.alpha = 1f;
	}

	public void FullScreen() // На весь экран
	{
		switch(FullScreen_Z)
		{
		case true:
			FullScreen_Z = false;
			break;
		case false:
			FullScreen_Z = true;
			break;
		}

		string[] x_y = new string[2];

		x_y[0] = "1024";
		x_y[1] = "768";

		if (PlayerPrefs.GetString("ResolutionValue", "none") != "none")
			x_y = PlayerPrefs.GetString("ResolutionValue").Split('x');
		else if (graphicSetting.Resolution.text != null)
			x_y = graphicSetting.Resolution.text.Split('x');
		Screen.SetResolution(int.Parse(x_y[0]), int.Parse(x_y[1]), FullScreen_Z);

		if(MainMenu != null && TypeSettingPanel != _TypeSettingPanel.MainMenu)
			MainMenu.ReSize();
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
		InputManager.instance.SaveSensitivity(Control.mouseSensitivity.value * 2.0f);
		
	}
	public void CearControlls(){
		newMap = new Dictionary<string, KeyCode>();
		Dictionary<string, KeyCode> map = InputManager.instance.GetMap();
		foreach (_Control.SettingCommand setCommand in Control.controls)
		{
			setCommand.keyname.text = map[setCommand.command].ToString();				
		}
		Control.mouseSensitivity.value = InputManager.instance.GetSensitivity() / 2f;
		SetMouseLabel();
		
	}
	public void DefaultControl() {
		newMap = new Dictionary<string, KeyCode>();
		Dictionary<string, KeyCode> map = InputManager.instance.ForceReload();
		foreach (_Control.SettingCommand setCommand in Control.controls)
		{
			setCommand.keyname.text = map[setCommand.command].ToString();
		}
		Control.mouseSensitivity.value = InputManager.instance.GetSensitivity() / 2f;
		SetMouseLabel();
	}
	public void SetMouseLabel() {
		Control.mouseLabel.text = (Control.mouseSensitivity.value * 100f).ToString("0");
	}
	
	public void SetValueVolume(UILabel IntArg, UIScrollBar ScrollArg) //Установка звука (Текст)
	{
		string value = (ScrollArg.value * 100f).ToString("0");
		IntArg.text = value;
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
				graphicSetting.Texture.text = "Low";
				graphicSetting.Shadow.text = "Low";
				graphicSetting.Lighning.text = "Low";
				graphicSetting.TextureScroll.value = 0f;
				graphicSetting.ShadowScroll.value = 0f;
				graphicSetting.LighningScroll.value = 0f;
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
				graphicSetting.Texture.text = "Medium";
				graphicSetting.Shadow.text = "Medium";
				graphicSetting.Lighning.text = "Medium";
				graphicSetting.TextureScroll.value = 0.5f;
				graphicSetting.ShadowScroll.value = 0.5f;
				graphicSetting.LighningScroll.value = 0.5f;
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
				graphicSetting.Texture.text = "High";
				graphicSetting.Shadow.text = "High";
				graphicSetting.Lighning.text = "High";
				graphicSetting.TextureScroll.value = 1f;
				graphicSetting.ShadowScroll.value = 1f;
				graphicSetting.LighningScroll.value = 1f;
				break;
			default:
				ValueLabel.text = "High";
				break;
			}
			break;
		}
		else
			ValueLabel.text = AllResolution[arg1];
		
		if (graphicSetting.Lighning.text != graphicSetting.Texture.text 
		    || graphicSetting.Shadow.text != graphicSetting.Texture.text
		    || graphicSetting.Lighning.text != graphicSetting.Shadow.text)
		{
			graphicSetting.Graphic.text = "Optional";
			graphicSetting.GraphicScroll.value = 1f;
		}
		else if (graphicSetting.Texture.text == "Low" && graphicSetting.Shadow.text == "Low"
		         && graphicSetting.Lighning.text == "Low" && graphicSetting.GraphicScroll.value != 0f)
		{
			graphicSetting.Graphic.text = "Low";
			graphicSetting.GraphicScroll.value = 0f;
		}
		else if (graphicSetting.Texture.text == "Medium" && graphicSetting.Shadow.text == "Medium"
		         && graphicSetting.Lighning.text == "Medium" && graphicSetting.GraphicScroll.value != 1/3)
		{
			graphicSetting.Graphic.text = "Medium";
			graphicSetting.GraphicScroll.value = 1f/3;
		}
		else if (graphicSetting.Texture.text == "High" && graphicSetting.Shadow.text == "High"
		         && graphicSetting.Lighning.text == "High" && graphicSetting.GraphicScroll.value != 1/3*2)
		{
			graphicSetting.Graphic.text = "High";
			graphicSetting.GraphicScroll.value = 1f/3*2;
		}
	}

	public void SaveGraphicSetting()
	{
		PlayerPrefs.SetFloat("Resolution", graphicSetting.ResolutionScroll.value);
		PlayerPrefs.SetString("ResolutionValue", graphicSetting.Resolution.text);
		PlayerPrefs.SetFloat("TextureQuality", graphicSetting.TextureScroll.value);
		PlayerPrefs.SetFloat("ShadowQuality", graphicSetting.ShadowScroll.value);
		PlayerPrefs.SetFloat("LighningQuality", graphicSetting.LighningScroll.value);
		PlayerPrefs.SetFloat("OverallVolume", volumes.VolumeScroll.value);
		PlayerPrefs.SetFloat("SoundFX", volumes.SoundFxScroll.value);
		PlayerPrefs.SetFloat("Music", volumes.MusicScroll.value);
		PlayerPrefs.SetString("SaveSetting", "yes");
		ApplyGraphicSetting();
	}

	public void ApplyGraphicSetting()
	{
		string[] x_y;
		if (PlayerPrefs.GetString("ResolutionValue", "not") != "not")
			x_y = PlayerPrefs.GetString("ResolutionValue").Split('x');
		else
			x_y = graphicSetting.Resolution.text.Split('x');
		Screen.SetResolution(int.Parse(x_y[0]), int.Parse(x_y[1]), Screen.fullScreen);
		switch(graphicSetting.Texture.text)
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
        AudioListener.volume = volumes.SoundFxScroll.value * volumes.VolumeScroll.value;
        MusicHolder.SetVolume(volumes.MusicScroll.value * volumes.VolumeScroll.value);
	}

	public void DefaultGraphic()
	{
		graphicSetting.ResolutionScroll.value = 1f;
		graphicSetting.TextureScroll.value = 1f;
		graphicSetting.ShadowScroll.value = 1f;
		graphicSetting.LighningScroll.value = 1f;
		volumes.VolumeScroll.value = 1f;
		volumes.SoundFxScroll.value = 1f;
		volumes.MusicScroll.value = 1f;
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

	void Awake()
	{
		foreach (GameObject onContrl in Control.toggleList) {
			_Control.SettingCommand commandClass = new _Control.SettingCommand();
			commandClass.codename = onContrl.GetComponent<UIToggle>();
			commandClass.command = onContrl.GetComponent<ControlButton>().command;
			commandClass.keyname = onContrl.transform.GetChild(0).GetComponent<UILabel>();
			Control.controls.Add(commandClass);
		}
		panel = this.GetComponent<UIPanel>();
	}

	public void ShowSetting()
	{
		if(TypeSettingPanel == _TypeSettingPanel.MainMenu)
		{
		//Debug.Log (_RoomsNgui.RoomsFound.alpha);
			if (panel.alpha > 0f) {
				MainMenu.HideAllPanel();
				MainMenu._PanelsNgui.SliderPanel.alpha = 1f;
				
			}
			else
			{
				MainMenu.HideAllPanel();
				panel.alpha = 1f;
				if (control.alpha == 1)
				{
					CearControlls();
				}
			}
		}
		else
		{
			Pause.VisableSetting = !Pause.VisableSetting;
		}
	}

	// Use this for initialization
	void Start () 
	{

		FullScreen_Z = Screen.fullScreen;
		if(TypeSettingPanel == _TypeSettingPanel.MainMenu)
			MainMenu = FindObjectOfType<MainMenuGUI>();
		else
			Pause = FindObjectOfType<PauseMenu>();

		Resolution[] resolutions = Screen.resolutions;
		AllResolution.Clear();
		foreach (Resolution res in resolutions) 
		{
			AllResolution.Add(res.width + "x" + res.height);
		}
		graphicSetting.ResolutionScroll.numberOfSteps = AllResolution.Count;
		if(PlayerPrefs.GetString("SaveSetting", "no") == "yes")
		{
			//Загрузка настроек
			graphicSetting.ResolutionScroll.value = PlayerPrefs.GetFloat("Resolution");
			graphicSetting.TextureScroll.value = PlayerPrefs.GetFloat("TextureQuality");
			graphicSetting.ShadowScroll.value = PlayerPrefs.GetFloat("ShadowQuality");
			graphicSetting.LighningScroll.value = PlayerPrefs.GetFloat("LighningQuality");
			volumes.VolumeScroll.value = PlayerPrefs.GetFloat("OverallVolume");
			volumes.SoundFxScroll.value = PlayerPrefs.GetFloat("SoundFX");
			volumes.MusicScroll.value = PlayerPrefs.GetFloat("Music");
			StartCoroutine(SetDefoltGraphic(1));
		}
		else
		{
			DefaultGraphic();
			StartCoroutine(SetDefoltGraphic(0));
		}
        ShowControl();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
