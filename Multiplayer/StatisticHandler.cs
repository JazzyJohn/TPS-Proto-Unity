using UnityEngine;
using System.Collections;

public class StatisticHandler : MonoBehaviour {

	public static string KILLED_BY ="killedBy";

	public static string STATISTIC_PHP="http://vk.rakgames.ru/kaspi/statistic.php";

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


	public static void SendPlayerKillbyPlayer(int Uid,string Name, int KillerUid,string KillerName)
	{
		var form = new WWWForm();
		form.AddField("action",KILLED_BY);
		form.AddField("uid",Uid);
		form.AddField("name",Name);
		form.AddField("killeruid",KillerUid);	
		form.AddField("killername",KillerName);
		StatisticHandler.instance.StartCoroutine(SendForm (form));
	}
	public static void SendPlayerKillbyNPC(int Uid,string Name){
		var form = new WWWForm ();
		form.AddField ("action", KILLED_BY);
		form.AddField ("uid", Uid);
		form.AddField ("name", Name);
		StatisticHandler.instance.StartCoroutine(SendForm (form));
}
	protected static IEnumerator SendForm(WWWForm form){
		Debug.Log (form + STATISTIC_PHP);
		WWW w =new WWW(STATISTIC_PHP, form);
		yield return w;

	}
}
