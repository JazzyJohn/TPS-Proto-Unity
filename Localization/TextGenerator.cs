using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;


public class TextGenerator : MonoBehaviour{
	public TextAsset xml;
	
	void Awake(){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(xml.text);
		ParseText(xmlDoc);

	
	
	}
	
	private  Dictionary<string, string>  moneyTexts = new Dictionary<string, string>();
	
	private  Dictionary<string, string>  lvlTexts = new Dictionary<string, string>();
	
	
	
	
	
	public void ParseText(){
		foreach (XmlNode node in xmlDoc.SelectNodes("moneytexts/oneentry")) {
			moneyTexts.Add(node.SelectSingleNode("cause").InnerText,node.SelectSingleNode("text").InnerText);
		}
		foreach (XmlNode node in xmlDoc.SelectNodes("lvltexts/oneentry")) {
			lvlTexts.Add(node.SelectSingleNode("cause").InnerText,node.SelectSingleNode("text").InnerText);
		}
	}

	public string GetExpText(string cause, int amount){
		string result;
		if(lvlTexts.ContainsKey(cause)){
			result = lvlTexts[cause];
		}else{
			return amount+"";
		}
		 return String.Format(result,amount);
	}
	public string GetMoneyText(string cause, int cash,int gold){
		string result;
		if(moneyTexts.ContainsKey(cause)){
			result = moneyTexts[cause];
		}else{
			return cash+" "+gold;
		}
		return String.Format(result,cash,gold);
	}

	private static TextGenerator s_Instance = null;
	
	public static TextGenerator instance {
		get {
			if (s_Instance == null) {
				//Debug.Log ("FIND");
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (TextGenerator)) as TextGenerator;
			}

		
			// If it is still null, create a new instance
			if (s_Instance == null) {
			//	Debug.Log ("CREATE");
				GameObject obj = new GameObject("TextGenerator");
				s_Instance = obj.AddComponent(typeof (TextGenerator)) as TextGenerator;
				
			}
			
			return s_Instance;
		}
	}

}