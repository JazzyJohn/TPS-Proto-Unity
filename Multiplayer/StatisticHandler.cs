using UnityEngine;
using System;
using System.Collections;

public class StatisticHandler : MonoBehaviour {

	public static string KILLED_BY ="killedBy";

	public static string ADD_USER ="addUser";

	public static string STATISTIC_PHP="http://vk.rakgames.ru/kaspi/";

	public static string STATISTIC_PHP_HTTPS="https://vk.rakgames.ru/kaspi/";

	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
	private static StatisticHandler s_Instance = null;
	
	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	public static StatisticHandler instance {
		get {

			// If it is still null, create a new instance
			if (s_Instance == null) {
				GameObject obj = new GameObject("StatisticHandler");
				s_Instance = obj.AddComponent(typeof (StatisticHandler)) as StatisticHandler;
				//Debug.Log ("Could not locate an AManager object.  AManager was Generated Automaticly.");
			}
			
			return s_Instance;
		}
	}


	public static void SendPlayerKillbyPlayer(string Uid,string Name, string KillerUid,string KillerName)
	{
		var form = new WWWForm();

		form.AddField("uid",Uid);
		form.AddField("name",Name);
		form.AddField("killeruid",KillerUid);	
		form.AddField("killername",KillerName);
		StatisticHandler.instance.StartCoroutine(SendForm (form,KILLED_BY));
	}
	public static void SendPlayerKillbyNPC(string Uid,string Name){
		var form = new WWWForm ();
	
		form.AddField ("uid", Uid);
		form.AddField ("name", Name);
		StatisticHandler.instance.StartCoroutine(SendForm (form,KILLED_BY));
}
	public static void StartStats(string Uid,string Name){
		var form = new WWWForm ();
		
		form.AddField ("uid", Uid);
		form.AddField ("name", Name);
		StatisticHandler.instance.StartCoroutine(SendForm (form,ADD_USER));
	}
	protected static IEnumerator SendForm(WWWForm form,string url){
		Debug.Log (form + url);
		WWW w = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {

						Debug.Log ("STATS HTTP SEND");
						w = new WWW (STATISTIC_PHP + url, form);
		}
		else{
			Debug.Log ("STATS HTTPS SEND");
			 w = new WWW (STATISTIC_PHP_HTTPS + url, form);
		}
			yield return w;
			Debug.Log (w.text);
	



	}
}
