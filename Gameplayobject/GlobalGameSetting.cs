using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;



public class GlobalGameSetting : MonoBehaviour{

    protected Hashtable huntTable = new Hashtable();
	
	
	protected Dictionary<string, List<float>> AISettings = new Dictionary<string, List<float>>();
	
	public const string MAX_ATTACKERS ="maxAttackers";
	
	public const string COOL_DOWN ="coolDown";
	
	void Awake(){
		TextAsset xml = Resources.Load("GameSetting") as TextAsset;
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(xml.text);
		ParseHuntTable(xmlDoc);
		//xml = Resources.Load("aiTable") as TextAsset;
		//xmlDoc = new XmlDocument();
		//xmlDoc.LoadXml(xml.text);
		ParseAiTable(xmlDoc);
	
	
	}
	private void ParseHuntTable(XmlDocument xmlDoc){
		
		
		//Parse Stuct Info First
		
	
	
		foreach (XmlNode node in xmlDoc.SelectNodes("hunttable/onecreature")) {
			huntTable.Add(node.SelectSingleNode("name").InnerText,int.Parse(node.SelectSingleNode("score").InnerText));
		}
	}
	private void ParseAiTable(XmlDocument xmlDoc){
		
		
		//Parse Stuct Info First
		
	
		
		foreach (XmlNode node in xmlDoc.SelectNodes("aitable/settings")) {
			List<float> list  = new List<float>();
			foreach(XmlNode valNode in node.SelectNodes("values")){
				list.Add(float.Parse(valNode.InnerText));
			}
			AISettings.Add(node.SelectSingleNode("name").InnerText,list);
		}
	}
    public Hashtable GetHuntScoreTable()
    {
		return huntTable;
	}

	public float GetAiSettings(string name,int type,float defValue){
		if(AISettings.ContainsKey(name)&&AISettings[name].Count>type){	
			return AISettings[name][type];		
		}
		return defValue;
	}
	public int GetAiSettings(string name,int type,int defValue){
		if(AISettings.ContainsKey(name)&&AISettings[name].Count>type){	
			return (int)AISettings[name][type];		
		}
		return defValue;
	}
	private static GlobalGameSetting s_Instance = null;
	
	public static GlobalGameSetting instance {
		get {
			if (s_Instance == null) {
				//Debug.Log ("FIND");
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (GlobalGameSetting)) as GlobalGameSetting;
			}

		
			// If it is still null, create a new instance
			if (s_Instance == null) {
			//	Debug.Log ("CREATE");
				GameObject obj = new GameObject("GlobalGameSetting");
				s_Instance = obj.AddComponent(typeof (GlobalGameSetting)) as GlobalGameSetting;
				
			}
			
			return s_Instance;
		}
	}



}
