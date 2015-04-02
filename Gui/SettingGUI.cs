using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SettingGUI : MonoBehaviour {

	public bool FullScreen_Z;



	public enum _TypeSettingPanel{MainMenu, GameMode};

	public _TypeSettingPanel TypeSettingPanel;
	
	[SerializeField]
	private bool _Visable;
    public UIPanel panel;
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

    public MainMenuGUI MainMenu;

	[System.Serializable]
	public class _Control
	{
		public List<SettingCommand> controls = new List<SettingCommand>();
		public ControlButton[] toggleList;

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
        public GameObject soundIcon;
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
        GA.API.Design.NewEvent("GUI:Settings:ShowControl"); 
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
        GA.API.Design.NewEvent("GUI:Settings:Video"); 
		HideAllSettingsPanel();
		video.alpha = 1f;
        CheckVideo();
	}

	public void FullScreen() // На весь экран
	{
        FullScreen_Z = !FullScreen_Z;
		GlobalPlayer.FullScreen(FullScreen_Z);

        
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
            Debug.Log(setCommand.command);
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
    float oldVolumeScroll=1.0f;
    float oldSoundFxScroll = 1.0f;
    float oldMusicScroll = 1.0f;
    public void ToggleSound()
    {
        if (volumes.VolumeScroll.value!=0)
        {
            oldVolumeScroll = volumes.VolumeScroll.value;
            volumes.VolumeScroll.value = 0.0f;
            oldSoundFxScroll = volumes.SoundFxScroll.value;
            volumes.SoundFxScroll.value = 0.0f;
            oldMusicScroll = volumes.MusicScroll.value;
            volumes.MusicScroll.value = 0.0f;

        }
        else
        {
            volumes.VolumeScroll.value = oldVolumeScroll;
            volumes.SoundFxScroll.value = oldSoundFxScroll;
            volumes.MusicScroll.value = oldMusicScroll;
        }
        AudioListener.volume = volumes.SoundFxScroll.value * volumes.VolumeScroll.value;
        MusicHolder.SetVolume(volumes.MusicScroll.value * volumes.VolumeScroll.value);
        PlayerPrefs.SetFloat("OverallVolume", volumes.VolumeScroll.value);
        PlayerPrefs.SetFloat("SoundFX", volumes.SoundFxScroll.value);
        PlayerPrefs.SetFloat("Music", volumes.MusicScroll.value);
        SaveGraphicSetting();
        SetIcon();
    }
	
	public void SetValueVolume(UILabel IntArg, UIScrollBar ScrollArg) //Установка звука (Текст)
	{
		string value = (ScrollArg.value * 100f).ToString("0");
		IntArg.text = value;
        
        if (panel.alpha == 1.0f)
        {
            if (ScrollArg == volumes.SoundFxScroll)
            {
                AudioListener.volume = volumes.SoundFxScroll.value * volumes.VolumeScroll.value;
            }
            else if (ScrollArg == volumes.MusicScroll)
            {
                MusicHolder.SetVolume(volumes.MusicScroll.value * volumes.VolumeScroll.value);
            }
            else
            {
                AudioListener.volume = volumes.SoundFxScroll.value * volumes.VolumeScroll.value;
                MusicHolder.SetVolume(volumes.MusicScroll.value * volumes.VolumeScroll.value);
            }
        }
        SetIcon();
	}

    private void SetIcon()
    {
        if (volumes.VolumeScroll.value > 0)
        {
            volumes.soundIcon.SetActive(true);
        }
        else
        {
            volumes.soundIcon.SetActive(false);
        }
    }
    


	public void SetGraphic(UILabel ValueLabel, UIScrollBar ScrollValue, string Setting) //Настройки графики (текст)
	{

        int arg1 = Mathf.RoundToInt(ScrollValue.value * (ScrollValue.numberOfSteps-1));
      
      
        if (Setting != "Resolution")
        {
            
                switch (Setting)
                {
                    case "Graphic":
                      if (ScrollValue.value != 1)
                        {
                            graphicSetting.Texture.text = QualitySettings.names[arg1];
                            graphicSetting.Shadow.text = QualitySettings.names[arg1];
                            graphicSetting.Lighning.text = QualitySettings.names[arg1];
                            graphicSetting.TextureScroll.value = ((float)arg1) / (graphicSetting.TextureScroll.numberOfSteps-1);
                            graphicSetting.ShadowScroll.value = ((float)arg1) / (graphicSetting.ShadowScroll.numberOfSteps-1);
                            graphicSetting.LighningScroll.value = ((float)arg1) / (graphicSetting.LighningScroll.numberOfSteps-1);
                        }
                        break;
                        
                    default:

                       
                        ValueLabel.text = QualitySettings.names[arg1];
                        break;

                }
                    if (graphicSetting.Lighning.text == graphicSetting.Texture.text
                    && graphicSetting.Shadow.text == graphicSetting.Texture.text)
                {
                    int index = 0;
                    for (int i = 0; i < QualitySettings.names.Length; i++)
                    {
                        if (QualitySettings.names[i] == graphicSetting.Texture.text)
                        {
                            index = i;
                        }

                    }
                    graphicSetting.Graphic.text = QualitySettings.names[index];
                  //  Debug.Log(((float)index) / (graphicSetting.GraphicScroll.numberOfSteps) + " " + QualitySettings.names[index]);
                    graphicSetting.GraphicScroll.value =( (float)index) / (graphicSetting.GraphicScroll.numberOfSteps-1);

                } 
       
                else {
                    graphicSetting.Graphic.text = "Optional";
                    graphicSetting.GraphicScroll.value = 1f;
                }
            
        }else{
           // Debug.Log( AllResolution[arg1]);
            ValueLabel.text = AllResolution[arg1];
        }

        
        
	}

	public void SaveGraphicSetting()
	{
		PlayerPrefs.SetFloat("Resolution", graphicSetting.ResolutionScroll.value);
		PlayerPrefs.SetString("ResolutionValue", graphicSetting.Resolution.text);
        int arg1 = Mathf.RoundToInt(graphicSetting.GraphicScroll.value * (graphicSetting.GraphicScroll.numberOfSteps - 1));
        PlayerPrefs.SetFloat("GraphicQuality", arg1);
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

        int arg1 = Mathf.RoundToInt(graphicSetting.GraphicScroll.value * (graphicSetting.GraphicScroll.numberOfSteps - 1));
        QualitySettings.SetQualityLevel(arg1);
	
        AudioListener.volume = volumes.SoundFxScroll.value * volumes.VolumeScroll.value;
        MusicHolder.SetVolume(volumes.MusicScroll.value * volumes.VolumeScroll.value);
	}

	public void DefaultGraphic()
	{
        Debug.Log(QualitySettings.GetQualityLevel());
        graphicSetting.GraphicScroll.value = ((float)QualitySettings.GetQualityLevel()) / (graphicSetting.ResolutionScroll.numberOfSteps );
        graphicSetting.ResolutionScroll.value = ((float)QualitySettings.GetQualityLevel()) / (graphicSetting.ResolutionScroll.numberOfSteps - 1);
        graphicSetting.TextureScroll.value = ((float)QualitySettings.GetQualityLevel()) / (graphicSetting.TextureScroll.numberOfSteps - 1);
        graphicSetting.ShadowScroll.value = ((float)QualitySettings.GetQualityLevel()) / (graphicSetting.ShadowScroll.numberOfSteps - 1);
        graphicSetting.LighningScroll.value = ((float)QualitySettings.GetQualityLevel()) / (graphicSetting.LighningScroll.numberOfSteps - 1);
        volumes.SoundFxScroll.value = 0.5f;
        volumes.VolumeScroll.value = 0.5f;
        volumes.MusicScroll.value = 0.5f;
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
        Control.toggleList = GetComponentsInChildren<ControlButton>();
		foreach (ControlButton onContrl in Control.toggleList) {
			_Control.SettingCommand commandClass = new _Control.SettingCommand();
			commandClass.codename = onContrl.GetComponent<UIToggle>();
			commandClass.command = onContrl.GetComponent<ControlButton>().command;
			commandClass.keyname = onContrl.transform.GetChild(0).GetComponent<UILabel>();
			Control.controls.Add(commandClass);
		}
		
	}

	public void ShowSetting()
	{
		
				MainMenu.HideAllPanel();
				panel.alpha = 1f;
				if (control.alpha == 1)
				{
					CearControlls();
				}
	}
    public void HideSetting()
    {

       
        panel.alpha = 0f;
      
    }

	// Use this for initialization
	void Start () 
	{
		NGUI_setting.setting = this;

    ///    PlayerPrefs.DeleteAll();

		FullScreen_Z = Screen.fullScreen;
		

		Resolution[] resolutions = Screen.resolutions;
		AllResolution.Clear();
		foreach (Resolution res in resolutions) 
		{
            //Debug.Log(res.width + "x" + res.height);
			AllResolution.Add(res.width + "x" + res.height);
		}
        
		graphicSetting.ResolutionScroll.numberOfSteps = AllResolution.Count;
        graphicSetting.GraphicScroll.numberOfSteps = QualitySettings.names.Length+1;
        graphicSetting.TextureScroll.numberOfSteps = QualitySettings.names.Length;
        graphicSetting.ShadowScroll.numberOfSteps = QualitySettings.names.Length;
        graphicSetting.LighningScroll.numberOfSteps = QualitySettings.names.Length;
       
        CheckVideo();
        ShowControl();
	}
    void CheckVideo()
    {
        RestoreGrahicSettingInMenu();
        if (PlayerPrefs.GetString("SaveSetting", "no") == "yes")
        {
            //Загрузка настроек
            graphicSetting.ResolutionScroll.value = PlayerPrefs.GetFloat("Resolution");
            volumes.VolumeScroll.value = PlayerPrefs.GetFloat("OverallVolume");
            volumes.SoundFxScroll.value = PlayerPrefs.GetFloat("SoundFX");
            volumes.MusicScroll.value = PlayerPrefs.GetFloat("Music");
           // Debug.Log(volumes.MusicScroll.value + "  " + volumes.VolumeScroll.value);
            AudioListener.volume = volumes.SoundFxScroll.value * volumes.VolumeScroll.value;
            MusicHolder.SetVolume(volumes.MusicScroll.value * volumes.VolumeScroll.value);
            SetIcon();
            StartCoroutine(SetDefoltGraphic(1));
        }
        else
        {
            volumes.VolumeScroll.value = 0.5f;
            volumes.SoundFxScroll.value = 0.5f;
            volumes.MusicScroll.value = 0.5f;
        }

    }
	public void RestoreGrahicSettingInMenu(){
		if(PlayerPrefs.GetString("SaveSetting", "no") == "yes")
		{
			//Загрузка настроек
            graphicSetting.GraphicScroll.value = PlayerPrefs.GetFloat("GraphicQuality") / (graphicSetting.GraphicScroll.numberOfSteps - 1);
			graphicSetting.TextureScroll.value = PlayerPrefs.GetFloat("TextureQuality");
			graphicSetting.ShadowScroll.value = PlayerPrefs.GetFloat("ShadowQuality");
			graphicSetting.LighningScroll.value = PlayerPrefs.GetFloat("LighningQuality");

            volumes.VolumeScroll.value = PlayerPrefs.GetFloat("OverallVolume");
            volumes.SoundFxScroll.value = PlayerPrefs.GetFloat("SoundFX");
            volumes.MusicScroll.value = PlayerPrefs.GetFloat("Music");
        }
		else
		{
			DefaultGraphic();
			StartCoroutine(SetDefoltGraphic(0));
		}
	}
	
	
	// Update is called once per frame
	void Update () {
	
	}
}

public static class NGUI_setting
{
	public static SettingGUI setting;

	public static void setQuality(int arg1)
	{
		
		setting.graphicSetting.Texture.text = QualitySettings.names[arg1];
		setting.graphicSetting.Shadow.text = QualitySettings.names[arg1];
		setting.graphicSetting.Lighning.text = QualitySettings.names[arg1];
		setting.graphicSetting.TextureScroll.value = ((float)arg1) / (setting.graphicSetting.TextureScroll.numberOfSteps-1);
		setting.graphicSetting.ShadowScroll.value = ((float)arg1) / (setting.graphicSetting.ShadowScroll.numberOfSteps-1);
		setting.graphicSetting.LighningScroll.value = ((float)arg1) / (setting.graphicSetting.LighningScroll.numberOfSteps-1);
		setting.graphicSetting.Graphic.text = QualitySettings.names[arg1];
		setting.graphicSetting.GraphicScroll.value =( (float)arg1) / (setting.graphicSetting.GraphicScroll.numberOfSteps-1);

		setting.SaveGraphicSetting();
	}
}
