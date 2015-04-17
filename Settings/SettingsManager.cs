using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;


public class SettingsManager: MonoBehaviour{

	 private static SettingsManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static SettingsManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof (SettingsManager)) as SettingsManager;
            }


            return s_Instance;
        }
    }
	
	public TextAsset configTable;
	void Awake(){
		LoadSetting();
	}
	public int GetSetting(string name){
        return PlayerPrefs.GetInt(name);
		
	}
	public void LoadSetting(){
       // PlayerPrefs.DeleteAll();
       
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(configTable.text);
		foreach (XmlNode node in xmlDoc.SelectNodes("settings/setting")){
			string name = node.SelectSingleNode ("name").InnerText;
			if( PlayerPrefs.HasKey(name)){
				SettingLogic(name,PlayerPrefs.GetInt(name));
			}else{
				SettingLogic(name, int.Parse(node.SelectSingleNode ("default").InnerText));
			}		
		}
	
	}
	public void SetSetting(string name, int value){

        PlayerPrefs.SetInt(name, value);
		SettingLogic(name,value);
	}
	
	

	public const string FULLSCREEN = "fullscreen";
	public const string MUSICVOLUME = "musicvolume";
	public const string SOUNDVOLUME = "soundvolume";
	public void SettingLogic(string name,int value){
		if(name ==FULLSCREEN){
            Resolution[] resolutions = Screen.resolutions;
			if(value==1){
				Screen.SetResolution(800, 600, false);	
				GlobalPlayer.ResizeCall();				
			}else{
				Screen.SetResolution(resolutions[resolutions.Length-1].width, resolutions[resolutions.Length-1].height, true);
				GlobalPlayer.ResizeCall();
			}
		}
		if(name==SOUNDVOLUME){
		
		   AudioListener.volume =((float) value)/100.0f;
		}
		if(name==MUSICVOLUME){
		
		  MusicHolder.SetVolume(((float) value)/100.0f);
		}
	}
    public const int LowFps = 30;

    public const int HighFps = 60;

    private float timer;

    public const float TIMECHECK = 5.0f;
    public void CheckSetting(int fps)
    {
       /* if (GameRule.instance == null || !GameRule.instance.start)
        {
            return;
        }
        if (fps < LowFps || fps > HighFps)
        {
            if (timer == 0)
            {
                timer = Time.time;
            }
        }
        else
        {
            timer = 0;
        }

        if (timer != 0 && timer + TIMECHECK < Time.time)
        {
            timer = 0;
            if (fps < LowFps)
            {
                QualitySettings.DecreaseLevel();
                PlayerPrefs.SetFloat("GraphicQuality", QualitySettings.GetQualityLevel()); 
            }

            if (fps > HighFps)
            {
                QualitySettings.IncreaseLevel();
                PlayerPrefs.SetFloat("GraphicQuality", QualitySettings.GetQualityLevel()); 
            }
        }*/


    }
	
	
	

}