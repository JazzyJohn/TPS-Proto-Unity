using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class StatisticManager : MonoBehaviour, LocalPlayerListener,GameListener{ 
	
	private Dictionary<string, int> toSendData = new Dictionary<string, int>();
	
    private float timerDelay =0.0f;
	
	private static float SEND_TIME = 60;

    public string UID ;

	public void Init(string uid){
		EventHolder.instance.Bind (this);
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
	
	private Player myPlayer;
	private int playerStrike;
	
	public void EventAppear(Player target){
        //Debug.Log("PLAYER SPAWM"+   target);
		if (target.isMine) {
			myPlayer = target;
			
		}
	}
	public void UpData(string key){
		UpData(key,1);
	}	
	public void UpData(string key, int value){
		if(!toSendData.ContainsKey(key)){
			toSendData[key]=0;
		}
		toSendData[key]+=value;
	}
    public void EventPawnKillPlayer(Player target, KillInfo killinfo)
    {
		if (target == myPlayer) {
			UpData(ParamLibrary.PARAM_KILL);
			playerStrike++;
			switch(playerStrike){
				
				case 3:
					UpData(ParamLibrary.PARAM_TRIPLE_KILL);
					break;
				case 4:
					UpData(ParamLibrary.PARAM_RAMPAGE_KILL);
					break;
				
					
			}
			
            if (killinfo.isHeadShoot)
            {
                UpData(ParamLibrary.PARAM_HEAD_SHOOT);
            }
            BaseWeapon weapon = ItemManager.instance.GetWeaponprefabByID(killinfo.weaponId);
            if (weapon!=null&&weapon.slotType == SLOTTYPE.GRENADE)
            {
                UpData(ParamLibrary.PARAM_GRENADE_KILL);
			}
		}
	}
    public void EventPawnKillAI(Player target, KillInfo killinfo)
    {
        UpData(ParamLibrary.PARAM_KILL_AI);

		
	
	}
	public void EventPawnKillAssistPlayer(Player target){
		if (target == myPlayer) {
            UpData(ParamLibrary.PARAM_ASSIST);
		}
	}
    public void EventPawnKillAssistAI(Player target){
		
	}
	public void EventTeamWin(int teamNumber){
		//if we not winner so no change in exp, or we a winner but no send were initiate we sync data 
        
		if (myPlayer.team	== teamNumber) {
			UpData(ParamLibrary.PARAM_WIN);
		}else{
			UpData(ParamLibrary.PARAM_LOSE);
		}
		
	}
	public void EventKilledByFriend(Player target,Player friend){
	
	}
    public void EventKilledAFriend(Player target, Player friend, KillInfo killinfo)
    {
		
	}
    public void EventPawnDeadByPlayer(Player target, KillInfo killinfo) { 
		playerStrike=0;
	}
	public void EventPawnDeadByAI(Player target){
		playerStrike=0;
	}
	public void EventPawnGround(Player target){	}
	public void EventPawnDoubleJump(Player target){}
	public void EventStartWallRun(Player target,Vector3 position){}
	public void EventStartSprintRun(Player target, Vector3 position){}
	public void EventEndSprintRun(Player target,Vector3 position){}
	public void EventEndWallRun(Player target, Vector3 position){}
	public void EventPawnReload(Player target){
		if (target == myPlayer) {
			UpData(ParamLibrary.PARAM_RELOAD);
		}
	}
	public void EventStart(){}
	public void EventRestart(){
			
	}
	public	void EventJuggerTake(Player target){
	
	}
    public void EventJuggerKill(Player target, KillInfo killinfo)
    {
	
	}
	public void EventRoomFinished(){}

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