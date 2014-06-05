using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class GlobalPlayer : MonoBehaviour {

	void Awake(){
			if(FindObjectsOfType<GlobalPlayer>().Length>1){
				Destroy(gameObject);
			}else{
				Application.ExternalCall ("SayMyName");
				if (Application.platform == RuntimePlatform.WindowsEditor) {
					SetUid(UID);
				}
				DontDestroyOnLoad(transform.gameObject);
			}
		
	}
	public List<string> friendsInfo = new List<string>();

	public string PlayerName="VK NAME";

	public string UID;
	
	public int gold;
	
	public int cash;
	
	

	void Update(){
		if(Input.GetButtonDown("FullScreen")){
			if(Screen.fullScreen){
				Screen.SetResolution(960, 600, false);
			}else{
				Screen.SetResolution( Screen.resolutions[ Screen.resolutions.Length-1].width,  Screen.resolutions[ Screen.resolutions.Length-1].height, true);
			}
		}
	}
	public String GetPlayerName(){
		return PlayerName;
	}	
	public String GetUID(){
		return UID;
	}
	public void addFriendInfo(string vkId)
	{
		friendsInfo.Add (vkId);
	}
	
	public void addFriendInfoList(string vkIds)
	{
		//Debug.Log (vkIds);
		string[] ids = vkIds.Split (',');
		foreach (string id in ids)
			friendsInfo.Add (id);
	}

	
	protected void parseProfile(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		gold = int.Parse (xmlDoc.SelectSingleNode ("player/gold").InnerText);
		cash = int.Parse (xmlDoc.SelectSingleNode ("player/cash").InnerText);
	}
	
	
	protected IEnumerator  StartStats(string Uid,string Name){
		WWWForm form = new WWWForm ();
		
		form.AddField ("uid", Uid);
		form.AddField ("name", Name);
		WWW w = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			Debug.Log ("STATS HTTP SEND" + StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.ADD_USER);
			w = new WWW (StatisticHandler.STATISTIC_PHP + StatisticHandler.ADD_USER, form);
		}
		else{
			Debug.Log ("STATS HTTPS SEND"+StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.ADD_USER);
			w = new WWW (StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.ADD_USER, form);
		}

		yield return w;
		//Debug.Log (w.text);
		parseProfile(w.text);
	}	
	protected IEnumerator  ReloadStats(string Uid){
		WWWForm form = new WWWForm ();
	
		form.AddField ("uid", Uid);
		WWW w = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			Debug.Log ("STATS HTTP SEND" + StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.RELOAD_STATS);
			w = new WWW (StatisticHandler.STATISTIC_PHP + StatisticHandler.RELOAD_STATS, form);
		}
		else{
			Debug.Log ("STATS HTTPS SEND"+StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.RELOAD_STATS);
			w = new WWW (StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.RELOAD_STATS, form);
		}

		yield return w;
		parseProfile(w.text);
	}	
	
	//EXTERNAL SECTION 
	
	public void AskJsForMagazine(string item){
		Application.ExternalCall ("ItemBuy",item);
	
	}
	public void ReloadProfile(){
		StartCoroutine(ReloadStats(UID));
	}
	
	public void  SetName(String newname)
	{
		PlayerName = newname;
	
		Application.ExternalCall( "SayMyUid");
		
	}
	public void  SetUid(string uid)
	{

		UID = uid;
		
		StartCoroutine(StartStats(UID,PlayerName));
		LevelingManager.instance.Init(UID);
		AchievementManager.instance.Init(UID);
		ItemManager.instance.Init(UID);
	}
}