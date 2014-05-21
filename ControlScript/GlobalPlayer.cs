using UnityEngine;
using System;



public class GlobalPlayer : MonoBehaviour {
	void Awake(){
			if(FindObjectsOfType<GlobalPlayer>().Length>1){
				Destroy(gameObject);
			}else{
				Application.ExternalCall ("SayMyName");
				if (Application.platform == RuntimePlatform.WindowsEditor) {
					SetUid("VKTEST");
				}
				DontDestroyOnLoad(transform.gameObject);
			}
		
	}
	public string PlayerName="VK NAME";

	public string UID;
	
	public int gold;
	
	public int cash;
	
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
	public String GetPlayerName(){
		return PlayerName;
	}	
	public String GetUID(){
		return UID;
	}
	public void ReloadProfile(){
		StartCoroutine(ReloadProfile(UID));
	}
	
	protected void parseProfile(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		gold = int.Parse (xmlDoc.SelectSingleNode ("gold").InnerText);
		cash = int.Parse (xmlDoc.SelectSingleNode ("cash").InnerText);
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
		parseProfile(w.text);
	}	
	protected IEnumerator  StartStats(string Uid){
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
}