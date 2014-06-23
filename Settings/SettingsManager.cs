using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public SettingsManager: MonoBehaviour{

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
		return PlayerPrefs.GetInt(name)
		
	}
	public void LoadSetting(){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(configTable);
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
		
		PlayerPrefs.SetInt(name,value)
		SettingLogic(name,value);
	}
	
	

	public static const string FULLSCREEN = "fullscreen";
	public static const string MUSICVOLUME = "musicvolume";
	public static const string SOUNDVOLUME = "soundvolume";
	public void SettingLogic(string name,int value){
		if(name ==FULLSCREEN){
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
}