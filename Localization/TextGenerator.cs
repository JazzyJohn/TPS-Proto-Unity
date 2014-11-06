using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;


public class TextGenerator : MonoBehaviour{
	public TextAsset xml;
	
	void Awake(){
		XmlDocument xmlDoc = new XmlDocument();
       // Debug.Log(xml.text);
		xmlDoc.LoadXml(xml.text);
		ParseText(xmlDoc);

	
	
	}
	
	private  Dictionary<string, string>  moneyTexts = new Dictionary<string, string>();

    private Dictionary<string, string> simpleText = new Dictionary<string, string>();
	
	private  Dictionary<string, string>  lvlTexts = new Dictionary<string, string>();

	private  Dictionary<AnnonceType, string>  mainAnnonceText = new Dictionary<AnnonceType, string>();

	private  Dictionary<AnnonceType, string>  addAnnonceText = new Dictionary<AnnonceType, string>();



    public void ParseText(XmlDocument xmlDoc)
    {
		foreach (XmlNode node in xmlDoc.SelectNodes("texts/moneytexts/oneentry")) {
           // Debug.Log(node.SelectSingleNode("name").InnerText + node.SelectSingleNode("text").InnerText);
            moneyTexts.Add(node.SelectSingleNode("name").InnerText, node.SelectSingleNode("text").InnerText);
		}
        foreach (XmlNode node in xmlDoc.SelectNodes("texts/simpletexts/oneentry"))
        {
            // Debug.Log(node.SelectSingleNode("name").InnerText + node.SelectSingleNode("text").InnerText);
            simpleText.Add(node.SelectSingleNode("name").InnerText, node.SelectSingleNode("text").InnerText);
        }
        foreach (XmlNode node in xmlDoc.SelectNodes("texts/lvltexts/oneentry"))
        {
         //   Debug.Log(node.SelectSingleNode("name").InnerText + node.SelectSingleNode("text").InnerText);
            lvlTexts.Add(node.SelectSingleNode("name").InnerText, node.SelectSingleNode("text").InnerText);
		}
		foreach (XmlNode node in xmlDoc.SelectNodes("texts/annonce/oneentry"))
        {
           
			AnnonceType type =(AnnonceType)System.Enum.Parse(typeof(AnnonceType), node.SelectSingleNode("name").InnerText);
          //  Debug.Log(type + node.SelectSingleNode("text").InnerText);
            mainAnnonceText.Add(type, node.SelectSingleNode("text").InnerText);
			XmlNode addText = node.SelectSingleNode("textAdd");
			if(addText!=null){
				addAnnonceText.Add(type,addText.InnerText);
			}
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
	public string GetMainAnnonceText(AnnonceType type){
		
		if(mainAnnonceText.ContainsKey(type)){
			return mainAnnonceText[type];
		}else{
			return "";
		}
		
	}
	
	public string GetAddAnnonceText(AnnonceType type,string text){
		string result;
        if (addAnnonceText.ContainsKey(type))
        {
			result = addAnnonceText[type];
		}else{
			return text;
		}
		return String.Format(result,text);
	}
    public string GetSimpleText(string text)
    {

        if (simpleText.ContainsKey(text))
        {
            return simpleText[text];
        }
        else
        {
            return text;
        }
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