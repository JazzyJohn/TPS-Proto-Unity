﻿using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.Text;

public class StatisticHandler : MonoBehaviour {

	public static string KILLED_BY ="killedBy";

    public static string KILL_NPC= "killNpc";

	public static string ADD_USER ="addUser";

	public static string LOAD_ACHIVE = "loadachive";
	
	public static string SAVE_ACHIVE = "saveachive";

    public static string SAVE_OPERATION = "saveoperation";
	
	public static string LOAD_LVL = "loadlvl";

	public static string SAVE_LVL = "savelvl";
	
	public static string RELOAD_STATS="returnAllStats";
	
	public static string LOAD_ITEMS = "loaditemsnew";
	
	public static string LOAD_STIMS = "loadstimsnew";
	
	public static string LOAD_SHOP = "loadshopnew";
	
	public static string BUY_ITEM = "buyItem";

    public static string BUY_LOTTERY_PLAY = "buyLotteryPlay";

    public static string SEND_LOTTERY_RESULT = "loteryResult";

    public static string BUY_ITEM_KIT = "buyKit";

    public static string RESET_SKILL = "resetSkills";

    public static string BUY_SKILL_ITEM = "buyPremiumSkill";

    public static string USE_ITEM = "useItem";
	
	public static string REPAIR_ITEM ="repairItem";

    public static string BUY_NEXT_SET = "buynextset";
	
	public static string DISENTEGRATE_ITEM = "disentegrateItem";

	public static string SAVE_ITEM = "saveitemnew";
	
	public static string MARK_ITEM = "markitem";	
	
	public static string UN_MARK_ITEM = "unmarkitem";	

	public static string NEWS_ALL = "allnews";
	
	public static string UPDATE_LORE = "updatelore";
	
	public static string LOAD_LORE = "loadlore";
	 
	public static string LOAD_MONEY_REWARD = "loadmoneyreward";
	
	public static string SYNC_MONEY_REWARD = "syncmoneyreward";
	
	public static string SPEND_SKILL_POINT = "spendskillpoint";
	
	public static string GLOBAL_ERROR_LOG = "globalerro-rlog";

    public static string DOUBLE_REWARD = "doublereward";
	
	public static string LOWER_STAMINA = "lowerstamina";
	
	public static string CHARGE_DATA ="chargedata";
	
	public static string REGISTRATION = "registration";
	
	public static string STATISTIC_DATA = "statisticdata";
	
	public static string SKIP_ACHIVE = "skipTask";
	
	public static string UPDATE_ACHIVE = "finishTask";

	public static string LOGIN = "login";
	
    private static string STATISTIC_PHP = "http://juggerfall.com/";

    private static string STATISTIC_PHP_HTTPS = "https://juggerfall.com/";

	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
	private static StatisticHandler s_Instance = null;
	
	private static  string UNITY_KEY = "redrageawesome";
	
	public static string SID  = "DebugEditor";
	
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
	public void Awake(){
	   if (Application.platform != RuntimePlatform.WindowsEditor&&Application.platform !=  RuntimePlatform.WindowsPlayer)
       {
		   Application.RegisterLogCallback(HandleLog);
	   }

	
	}
	void HandleLog(string logString, string stackTrace, LogType type) {
        if (type == LogType.Error || type == LogType.Exception)
        {
			WWWForm form = new WWWForm();

			form.AddField("uid",GlobalPlayer.instance.GetUID());
			form.AddField("time",Time.time.ToString());
			form.AddField("logString",logString);
			form.AddField("stackTrace",stackTrace);
			StartCoroutine(SendForm (form,GLOBAL_ERROR_LOG));
            Debug.Log("SENDform");
		}
    }
	public static void SendPlayerKillbyPlayer(string Uid,string Name, string KillerUid,string KillerName)
	{
        try
        {
		WWWForm form = new WWWForm();

		form.AddField("uid",Uid);
		form.AddField("name",Name);
		form.AddField("killeruid",KillerUid);	
		form.AddField("killername",KillerName);
		StatisticHandler.instance.StartCoroutine(SendForm (form,KILLED_BY));
        }
        catch (Exception e)
        {

            Debug.Log(e);
        }
       
	}
    public static void SendPlayerKillNPC(string Uid, string Name)
    {
        try
        {
            WWWForm form = new WWWForm();

            form.AddField("uid", Uid);
            form.AddField("name", Name);
            StatisticHandler.instance.StartCoroutine(SendForm(form, KILL_NPC));
        }
        catch (Exception e)
        {

            Debug.Log(e);
        }
       
    }
	public static void SendPlayerKillbyNPC(string Uid,string Name){
		WWWForm form = new WWWForm ();
          try
        {
		    form.AddField ("uid", Uid);
		    form.AddField ("name", Name);
		    StatisticHandler.instance.StartCoroutine(SendForm (form,KILLED_BY));
        }
          catch (Exception e)
          {

              Debug.Log(e);
          }
	}
	public static void StartStats(string Uid,string Name){
		WWWForm form = new WWWForm ();
		
		form.AddField ("uid", Uid);
		form.AddField ("name", Name);
		StatisticHandler.instance.StartCoroutine(SendForm (form,ADD_USER));
	}
	public static IEnumerator SendForm(WWWForm form,string url){
		//Debug.Log (form + url);
		WWW w = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {

						//Debug.Log ("STATS HTTP SEND");
						w = new WWW (GetSTATISTIC_PHP()+ url, form);
		}
		else{
			//Debug.Log ("STATS HTTPS SEND");
			 w = new WWW (GetSTATISTIC_PHP_HTTPS()+ url, form);
		}
			yield return w;
			
	



	}
	
	static string GetMd5Hash(MD5 md5Hash, string input){

		// Convert the input string to a byte array and compute the hash.
		byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

		// Create a new Stringbuilder to collect the bytes
		// and create a string.
		StringBuilder sBuilder = new StringBuilder();

		// Loop through each byte of the hashed data 
		// and format each one as a hexadecimal string.
		for (int i = 0; i < data.Length; i++)
		{
			sBuilder.Append(data[i].ToString("x2"));
		}

		// Return the hexadecimal string.
		return sBuilder.ToString();
	}
	
	public static WWW GetMeRightWWW(WWWForm form,string URL){
	    Hashtable headers = form.headers;
	    string rawData = System.Text.Encoding.UTF8.GetString(form.data);
	
		using (MD5 md5Hash = MD5.Create())
        {
            headers["X-Digest"] = GetMd5Hash(md5Hash, rawData + UNITY_KEY);
			
		}
		WWW www = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			//Debug.Log ("STATS HTTP SEND" + StatisticHandler.STATISTIC_PHP_HTTPS + URL);
			www = new WWW (StatisticHandler.GetSTATISTIC_PHP()+ URL,form.data,headers);
		}
		else{
			//Debug.Log ("STATS HTTPS SEND"+StatisticHandler.GetSTATISTIC_PHP_HTTPS()+  URL);
			www = new WWW (StatisticHandler.GetSTATISTIC_PHP_HTTPS()+  URL,form.data,headers);
		}
	
		return www;
	
	}
	public static WWW GetMeRightWWW(string URL){
		WWW www = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			//Debug.Log ("STATS HTTP SEND" + StatisticHandler.STATISTIC_PHP_HTTPS +  node.SelectSingleNode ("textureGUIName").InnerText);
			www = new WWW (StatisticHandler.GetSTATISTIC_PHP()+ URL);
		}
		else{
			//Debug.Log ("STATS HTTPS SEND"+StatisticHandler.STATISTIC_PHP_HTTPS +  node.SelectSingleNode ("textureGUIName").InnerText);
			www = new WWW (StatisticHandler.GetSTATISTIC_PHP_HTTPS()+  URL);
		}
		
		return www;
		
	}
    public static String GetSTATISTIC_PHP(){
        if(GlobalPlayer.instance!=null&&GlobalPlayer.instance.STATISTIC_PHP!=""){
            return GlobalPlayer.instance.STATISTIC_PHP;
        }
        return STATISTIC_PHP;
    }

    public static String GetSTATISTIC_PHP_HTTPS()
    {
        if (GlobalPlayer.instance != null && GlobalPlayer.instance.STATISTIC_PHP_HTTPS!= "")
        {
            return GlobalPlayer.instance.STATISTIC_PHP_HTTPS;
        }
        return STATISTIC_PHP_HTTPS;
    }
	public static String GetNormalURL(){
	
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			//Debug.Log ("STATS HTTP SEND" + StatisticHandler.STATISTIC_PHP_HTTPS +  node.SelectSingleNode ("textureGUIName").InnerText);
            return StatisticHandler.GetSTATISTIC_PHP();
		}
		else{
			//Debug.Log ("STATS HTTPS SEND"+StatisticHandler.STATISTIC_PHP_HTTPS +  node.SelectSingleNode ("textureGUIName").InnerText);
            return StatisticHandler.GetSTATISTIC_PHP_HTTPS();
		}
	}
	
	
	public static void  SendTCP(string URL,WWWForm form){
		WWW www = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			//Debug.Log ("STATS HTTP SEND" + StatisticHandler.GetSTATISTIC_PHP_HTTPS()+  node.SelectSingleNode ("textureGUIName").InnerText);
			www = new WWW (StatisticHandler.GetSTATISTIC_PHP()+ URL,form);
		}
		else{
			//Debug.Log ("STATS HTTPS SEND"+StatisticHandler.GetSTATISTIC_PHP_HTTPS()+  node.SelectSingleNode ("textureGUIName").InnerText);
			www = new WWW (StatisticHandler.GetSTATISTIC_PHP_HTTPS()+  URL,form);
		}
		int i = 0;
		while(true){

			if(www.isDone){
				return;
			}
			i++;
			//YES that nor best sync www that can exists but it's work!!!!
			if(i>1000000){
				return;
			}
		}

		

	/*	string url = "";
		if (String.Compare (Application.absoluteURL, 0, "https", 0, 5) != 0) {
			url = StatisticHandler.GetSTATISTIC_PHP()+ URL;
		}else{
			url = StatisticHandler.GetSTATISTIC_PHP_HTTPS()+ URL;
		}
		string result = System.Text.Encoding.UTF8.GetString (form.data);

		byte[] buf = new byte[1024];
		string header = "POST "+url+" HTTP/1.1\r\n" +
			"Host: localhost:2006\r\n" +
				"Connection: keep-alive\r\n" +
				"User-Agent: Mozilla/5.0\r\n" +
				result+"\r\n";
				
		
		TcpClient client = new TcpClient("vk.rakgames.ru", 80);            

		// send request
		client.Client.Send(System.Text.Encoding.ASCII.GetBytes(header));*/


	}
}
