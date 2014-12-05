using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class SocUser{
	public Texture2D avatar;
	
	public string name;
	
	public string uid;
}

public class Winner{
	public SocUser  user;
	
	public int score;
	
	public Winner(SocUser user,int score){
		this.user = user;
		this.score = score;
	}
}


public class Tournament{
	public bool isFinished;

	public bool isActive;
	
	public Date start;
	
	public Date end;
	
	public string name;

	public string desctiption;
	
	public int prizePlaces;
	
	public int[] goldReward;
	
	public int[] cashReward;
		
	public Winner[] winers;
}






public class TournamentManager : MonoBehaviour{

	public Dictionary<string,SocUser>  allUsers = new Dictionary<string,SocUser>();

	List<string> uids =  new List<string>();

	public Winner[] aiKillers;
	
	public Winner[] killers;
	
	public void ParseDate(XmlDocument xmlData){
		if(xmlData.SelectSingleNode ("player/tournament")==null){
			return;
		}
		 XmlNodeList killersXml = xmlDoc.SelectNodes("items/tournament/globalkillers");
	
		killers = new Winers[killersXml.Count];
		
		for(int j=0;j<killersXml.Count;j++){
			XmlNode node  =killersXml[j];
			killers[j] = new Winner(GetUser(node.SelectSingleNode("uid").InnerText),int.Parse(node.SelectSingleNode("score").InnerText));
			
		}
		killersXml = xmlDoc.SelectNodes("items/tournament/globalaikillers");
	
		aiKillers = new Winers[killersXml.Count];
		
		for(int j=0;j<killersXml.Count;j++){
			XmlNode node  =killersXml[j];
			aiKillers[j] = new Winner(GetUser(node.SelectSingleNode("uid").InnerText),int.Parse(node.SelectSingleNode("score").InnerText));
			
		}
		Application.ExternalCall ("GetUsers",String.Join(", ", uids.ToArray()););
	}
	
	public SocUser GetUser(string uid){
		if(!allUsers.Contains(uid)){
			uids.Add(uid);
			allUsers[uid] = new SocUser(uid);
		}
		
		return allUsers[uid];
	
	}
	
	public IEnumerator SetSocInfo(string data){
		uids.Clear();
		string[] dataSplit =  data.Split(",");
		foreach(string dateEntry in dataSplit){
			string[] oneEntry =  dateEntry.Split(";");
			allUsers[oneEntry[0]].name = oneEntry[1];
			
			WWW www = new WWW(oneEntry[2]);

			
		   yield return www;
		   allUsers[oneEntry[0]].avatar = new Texture2D(100, 100);
		   www.LoadImageIntoTexture( allUsers[oneEntry[0]].avatar);
		
		}
	
	}







	private static TournamentManager s_Instance = null;
	
	public static TournamentManager instance {
		get {
			if (s_Instance == null) {
				//Debug.Log ("FIND");
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (TournamentManager)) as TournamentManager;
			}

		
			// If it is still null, create a new instance
			if (s_Instance == null) {
			//	Debug.Log ("CREATE");
				GameObject obj = new GameObject("TextGenerator");
				s_Instance = obj.AddComponent(typeof (TournamentManager)) as TournamentManager;
				
			}
			
			return s_Instance;
		}
	}

}