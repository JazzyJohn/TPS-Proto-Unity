using UnityEngine;
using System;



public class GlobalPlayer : MonoBehaviour {
	void Awake(){
			Application.ExternalCall ("SayMyName");
			if (Application.platform == RuntimePlatform.WindowsEditor) {
			SetUid("VKTEST");
			}
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
		FindObjectOfType<AchievementManager>().Init(UID);
		FindObjectOfType<LevelingManager>().Init(UID);
	}
	public String GetPlayerName(){
		return PlayerName;
	}	
	public String GetUID(){
		return UID;
	}
	
}