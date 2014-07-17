using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
//using System.Threading;

public class  RewardManager : MonoBehaviour, LocalPlayerListener,GameListener{ 
		
	public const string PARAM_KILL = "Kill";
	public const string PARAM_WIN = "Win";
	public const string PARAM_KILL_AI= "KillAI"; 
	public const string PARAM_KILL_FRIEND = "KillFriend";
	public const string PARAM_KILL_BY_FRIEND= "KilledByFriend"; 
	
	public Distionary<string,int> rewardMoneyDictionary = new Dictionary<string, int>();
	
	public void Init(string uid){
		EventHolder.instance.Bind (this);
		DontDestroyOnLoad(transform.gameObject);
		WWWForm form = new WWWForm ();
			UID = uid;
		form.AddField ("uid", uid);
		
		StartCoroutine(LoadReward (form));
	
	}
	
	protected IEnumerator LoadReward(WWWForm form){
		
		
		WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.LOAD_MONEY_REWARD);
		
		yield return w;
		//Debug.Log (w.text);
		ParseList (w.text);
	
	}
	//parse Xml into Reward Dictionary
	protected void ParseList(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		foreach (XmlNode node in xmlDoc.SelectNodes("money")) {
			//Debug.Log ("LEVELING" +node.SelectSingleNode("name").InnerText+" "+node.SelectSingleNode("value").InnerText);
			expDictionary.Add(node.SelectSingleNode("name").InnerText,int.Parse(node.SelectSingleNode("value").InnerText));
		}
	
	}
	private int upCash = 0;
	public void UpMoney(string reason){
		upCash+= rewardMoneyDictionary[reason];
		
	}
	
	private void SyncReward(){
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		form.AddField ("upCash", upCash);
		
		StartCoroutine(_SyncReward (form));
	}
	private IEnumerator _SyncReward(WWWForm form){
		WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.SYNC_MONEY_REWARD);
		
		yield return w;
		GlobalPlayer.instance.parseProfile(w.text)
		
	}
	//Event Section
	private Player myPlayer;
	public void EventAppear(Player target){
		if (target.GetView ().isMine) {
			myPlayer = target;
			
		}
	}
	public void EventPawnKillPlayer(Player target){
		if (target == myPlayer) {
			UpMoney(PARAM_KILL);	
		}
	}
	public void EventPawnKillAI(Player target){
	

		if (target == myPlayer) {

			UpMoney(PARAM_KILL_AI);	
		}
	
	}
	public void EventTeamWin(int teamNumber){
		//if we not winner so no change in exp, or we a winner but no send were initiate we sync data 
		if (myPlayer.team	!= teamNumber||(myPlayer.team == teamNumber)) {
			UpMoney(PARAM_WIN)
			SyncReward();
		}
		
	}
	public void EventKilledByFriend(Player target,Player friend){
		if (target == myPlayer) {
			UpMoney(PARAM_KILL_BY_FRIEND);	
		}
	}
	public void EventKilledAFriend(Player target,Player friend){
		if (target == myPlayer) {
			UpMoney(PARAM_KILL_FRIEND);	
		}
	}
	public void EventPawnDeadByPlayer(Player target){
		SyncReward();
	}
	public void EventPawnDeadByAI(Player target){
		SyncReward();
	}
	public void EventPawnGround(Player target){	}
	public void EventPawnDoubleJump(Player target){}
	public void EventStartWallRun(Player target,Vector3 position){}
	public void EventStartSprintRun(Player target, Vector3 position){}
	public void EventEndSprintRun(Player target,Vector3 position){}
	public void EventEndWallRun(Player target, Vector3 position){}
	public void EventPawnReload(Player target){}
	public void EventStart(){}
	public void EventRestart(){}
	
}