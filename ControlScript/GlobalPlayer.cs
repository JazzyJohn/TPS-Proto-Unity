using UnityEngine;
using System;



public class GlobalPlayer : MonoBehaviour {
	void Awake(){
			Application.ExternalCall ("SayMyName");
			DontDestroyOnLoad(transform.gameObject);
	}
	public string PlayerName="VK NAME";

	public string UID;
	public void  SetName(String newname)
	{
		PlayerName = newname;
	
		Application.ExternalCall( "SayMyUid");
		
	}
	public void  SetUid(string uid)
	{

		UID = uid;
		
		StatisticHandler.StartStats(UID,PlayerName);
		FindObjectOfType(typeof(AchievementManager)).Init(UID);
		FindObjectOfType(typeof(LevelingManager)).Init(UID);
	}
	public String GetName(){
		return PlayerName;
	}	
	public String GetUid(){
		return UID;
	}
	
}