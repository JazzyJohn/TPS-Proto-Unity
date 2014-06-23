using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;


public class InputManager{

	
	private   Dictionary<String,String> keyMap  = new Dictionary<String,String> ();
	private   float mouseSensitivity = 1.0f;
	private static string MOUSESENSITIVITY ="mouseSensitivity";
	
	public InputManager(){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(SettingsManager.instance.configTable.text);
		foreach (XmlNode node in xmlDoc.SelectNodes("input/keys")) {
			string name = node.SelectSingleNode ("name").InnerText;
			if( PlayerPrefs.HasKey(name)){
				SetKey(name,PlayerPrefs.GetString(name));
			}else{
				SetKey(name, node.SelectSingleNode ("default").InnerText);
			}
		
		}
		if( PlayerPrefs.HasKey(MOUSESENSITIVITY)){
				mouseSensitivity =  PlayerPrefs.GetFloat(MOUSESENSITIVITY);
		}else{
				mouseSensitivity =  1.0f;
		}
	
	}
	
	
	public  float GetAxisRaw(string name){
		return Input.GetAxisRaw(name);
	}
	public  float GetMouseAxis(string name){
		return Input.GetAxis(name)*mouseSensitivity;
	}
    public bool GetButton(string name)
    {
        return Input.GetKey(keyMap[name]);
		
	}
	public  bool GetButtonUp(string name){
		return Input.GetKeyUp(keyMap[name]);
		
	}


    public bool GetButtonDown(string name)
    {
		return Input.GetKeyDown(keyMap[name]);
		
	}
	private  void SetKey(string name,string key){
		keyMap[name] = key;
	}
	public void SaveKey(string name,string key){
		keyMap[name] = key;
		PlayerPrefs.SetString(name,key);
	}
	public void SaveSensitivity(float value){
		mouseSensitivity =  value;
		PlayerPrefs.SetFloat(MOUSESENSITIVITY,value);
	}
	 private static InputManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static InputManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = new InputManager();
            }


            return s_Instance;
        }
    }
	
}