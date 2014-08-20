using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class GlobalGameSetting : MonoBehaviour{
	
	protected Hashtable huntTable =new Hashtable();
	
	void Awake(){
		TextAsset xml = Resources.Load("huntTable") as TextAsset;
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		ParseHuntTable(xmlDoc);

	
	
	}
	private void ParseHuntTable(mlDocument xmlDoc){
		
		
		//Parse Stuct Info First
		
	
		int i = 0;
		foreach (XmlNode node in xmlDoc.SelectNodes("hunttable/onecreature")) {
			huntTable.add(node.electSingleNode("name").InnerText,int.Parse(node.electSingleNode("score").InnerText));
		}
	}
	public Hashtable GetHuntScoreTable(){
		return huntTable;
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
