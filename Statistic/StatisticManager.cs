using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class StatisticManager : MonoBehaviour{ 
	
	private Dictionary<string, int> toSendData = new Dictionary<string, int>();
	
    private float timerDelay =0.0f;
	
	private static float SEND_TIME = 60;

    public string UID ;

	public void Init(string uid){
        ActionResolver.instance.AddUp(UpDataSimple);
		DontDestroyOnLoad(transform.gameObject);
        UID = uid;
	
	}
	void Update(){
		timerDelay += Time.deltaTime;
		if(timerDelay>SEND_TIME){

			timerDelay= 0;
			SendData();
		}
	}
	
	void SendData(){
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		form.AddField ("data[time]", SEND_TIME.ToString("0"));
		foreach (KeyValuePair<string, int> pair in toSendData)
		{
//            Debug.Log("charge[" + pair.Key + "]"+ pair.Value.chargeSpend);
			if(pair.Value==0){
				continue;
			}
			form.AddField ("data["+pair.Key+"]", pair.Value);
			
		}
        toSendData.Clear();
	 	StatisticHandler.instance.StartCoroutine(StatisticHandler.SendForm (form,StatisticHandler.STATISTIC_DATA));
	
	}
	

	
	
	public void UpDataSimple(string key){
		UpData(key,1);
	}	
	public void UpData(string key, int value){
		if(!toSendData.ContainsKey(key)){
			toSendData[key]=0;
		}
		toSendData[key]+=value;
	}
   

	public void AddDamageDeliver(int damage){
		UpData(ParamLibrary.PARAM_DAMAGE_DELIVER,damage);
	}
	public void AddDamage(int damage){
		UpData(ParamLibrary.PARAM_DAMAGE,damage);
	}
	public void AmmoSpent(int spent){
		UpData(ParamLibrary.PARAM_AMMO_SPENT,spent);
	}
	public void AmmoHit(int hit){
		UpData(ParamLibrary.PARAM_AMMO_HIT,hit);
	}
    public void AddTask()
    {
        UpDataSimple(ParamLibrary.PARAM_TASK_COMPLITE);
    }
	private static StatisticManager s_Instance = null;
	
	public static StatisticManager instance {
		get {
			if (s_Instance == null) {
				//Debug.Log ("FIND");
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (StatisticManager)) as StatisticManager;
			}

		
			// If it is still null, create a new instance
			if (s_Instance == null) {
			//	Debug.Log ("CREATE");
				GameObject obj = new GameObject("StatisticManager");
				s_Instance = obj.AddComponent(typeof (StatisticManager)) as StatisticManager;
				
			}
			
			return s_Instance;
		}
	}
	
}