using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
//using System.Threading;
public class Reward{
	int count = 0;
	public string descr;
	public void Increment(){
		count++;
	}	
	public int GetRewardCounter(){
		return count;
	}
	public void Reset(){
		count= 0;
	}

}
public static class ParamLibrary{
	public static string PARAM_KILL = "Kill";
	public static string PARAM_TRIPLE_KILL = "TripleKill";
	public static string PARAM_RAMPAGE_KILL = "Rampage";
	public static string PARAM_RAMPAGE_GOING_KILL = "RampageGoing";
	public static string PARAM_WIN = "Win";
	public static string PARAM_LOSE = "Lose";
	public static string PARAM_KILL_AI= "KillAI"; 
	public static string PARAM_KILL_FRIEND = "KillFriend";
	public static string PARAM_KILL_BY_FRIEND= "KilledByFriend";
    public static string PARAM_ROOM_FINISHED = "RoomFinished";
	public static string PARAM_JUGGER_TAKE= "JuggerTake";
    public static string PARAM_JUGGER_KILL= "JuggerKill";
	public static string PARAM_HEAD_SHOOT = "HeadShoot";
    public static string PARAM_HEAD_SHOOT_AI = "HeadShootAI";
	public static string PARAM_DEATH = "Death";
    public static string PARAM_DEATH_AI = "DeathAI";
    public static string PARAM_STIM_PACK = "StimPack";
	public static string PARAM_WALL_RUN = "WallRun";
	public static string PARAM_ASSIST= "Assist"; 
	public static string PARAM_ASSIST_AI= "AssistAI"; 
	
}

public class  RewardManager : MonoBehaviour, LocalPlayerListener,GameListener{ 
		
	
   

	class MoneyReward : Reward{
		public int cash = 0;
		public int gold =0;
		public MoneyReward(int cash,int gold){
			this.cash = cash;
			this.gold = gold;
		}
	}

    private Dictionary<string, MoneyReward> rewardMoneyDictionary = new Dictionary<string, MoneyReward>();

    public List<RewardGUI> GetAllReward()
    {
        int multiplier = 1;
        if (AfterGameBonuses.wasStamined)
        {
            multiplier = Mathf.RoundToInt(PremiumManager.STAMINA_MULTIPLIER);
        }
        List<RewardGUI> answer = new List<RewardGUI>();
        foreach (KeyValuePair<string, MoneyReward> entry in rewardMoneyDictionary)
        {
            if (entry.Value.GetRewardCounter() > 0)
            {
                if (entry.Value.cash > 0)
                {
                    RewardGUI reward = new RewardGUI();
                    reward.count = entry.Value.GetRewardCounter();

                    reward.amount = reward.count * entry.Value.cash * multiplier;
                    reward.text = TextGenerator.instance.GetSimpleText(entry.Key);
                    reward.isCash = true;
                    answer.Add(reward);
                }
                if (entry.Value.gold>0){
                     RewardGUI reward = new RewardGUI();
                    reward.count = entry.Value.GetRewardCounter();

                    reward.amount = reward.count * entry.Value.gold * multiplier;
                    reward.text = TextGenerator.instance.GetSimpleText(entry.Key);
                    reward.isCash = false;
                    answer.Add(reward);
                }
                entry.Value.Reset();
            }
        }
        return answer;
    }


    private string UID;
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
		foreach (XmlNode node in xmlDoc.SelectNodes("reward/money")) {
			//Debug.Log ("Reward" +node.SelectSingleNode("name").InnerText+" "+node.SelectSingleNode("value").InnerText);
			MoneyReward reward = new MoneyReward(int.Parse(node.SelectSingleNode("value").InnerText),int.Parse(node.SelectSingleNode("valueGold").InnerText));
			reward.descr =node.SelectSingleNode("name").InnerText;
            rewardMoneyDictionary.Add(node.SelectSingleNode("name").InnerText,reward);
		}
	
	}
	private int upCash = 0;
    private int upGold = 0;
	public void UpMoney(string reason){
		if(GameRule.instance.IsPractice()){
			return;
		}
	
        if(rewardMoneyDictionary.ContainsKey(reason)){
			MoneyReward reward = rewardMoneyDictionary[reason];
			
	    	upCash+=Mathf.RoundToInt( reward.cash *PremiumManager. GetMultiplier());
			upGold+=Mathf.RoundToInt( reward.gold *PremiumManager. GetMultiplier());
            if (reward.cash != 0 || reward.gold != 0)
            {
               reward.Increment();
            }
            if (PlayerMainGui.instance != null)
            {
                if (reward.cash > 0)
                {
                    String rewardStr = TextGenerator.instance.GetMoneyText(reason, Mathf.RoundToInt(reward.cash * PremiumManager.GetMultiplier()));
                    PlayerMainGui.instance.AddMessage(rewardStr, PlayerMainGui.MessageType.MONEY_REWARD);
                }
                if (reward.gold > 0)
                {
                    String rewardStr = TextGenerator.instance.GetMoneyText(reason,Mathf.RoundToInt( reward.gold * PremiumManager.GetMultiplier()));
                    PlayerMainGui.instance.AddMessage(rewardStr, PlayerMainGui.MessageType.GOLD_REWARD);
                }
            }
        }
     
		
	}
    private String MakeRewardString(int cash,int gold)
    {
        String reward  = "";
        if (cash > 0)
        {
            reward = "KP +" + cash + " ";
        }
        if (gold > 0)
        {
            reward = "GTIP +" + gold;
        }
        return reward;
    }
	
	private void SyncReward(){
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		form.AddField ("upCash", upCash);
		form.AddField ("upGold", upGold);
        upCash = 0;
		upGold=0;
		StartCoroutine(_SyncReward (form));
	}
	private IEnumerator _SyncReward(WWWForm form){
		WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.SYNC_MONEY_REWARD);
		
		yield return w;
        GlobalPlayer.instance.parseProfile(w.text);
		
	}
	//Event Section
	private Player myPlayer;
	public void EventAppear(Player target){
		if (target.isMine) {
			myPlayer = target;
			
		}
	}
    public void EventPawnKillPlayer(Player target, KillInfo killinfo)
    {
		if (target == myPlayer) {
			playerStrike++;
			switch(playerStrike){
				case 1:
				case 2:
					UpMoney(ParamLibrary.PARAM_KILL);	
					break;
				case 3:
					UpMoney(ParamLibrary.PARAM_TRIPLE_KILL);
					break;
				case 4:
					UpMoney(ParamLibrary.PARAM_RAMPAGE_KILL);
					break;
				default:
					UpMoney(ParamLibrary.PARAM_RAMPAGE_GOING_KILL);
					break;
					
			}
			
			
		}
	}
    public void EventPawnKillAI(Player target, KillInfo killinfo)
    {
	

		if (target == myPlayer) {

			UpMoney(ParamLibrary.PARAM_KILL_AI);	
		}
	
	}
	public void EventPawnKillAssistPlayer(Player target){
		if (target == myPlayer) {

			UpMoney(ParamLibrary.PARAM_ASSIST);	
		}
	}
    public void EventPawnKillAssistAI(Player target){
		if (target == myPlayer) {

			UpMoney(ParamLibrary.PARAM_ASSIST_AI);	
		}
	}
	public void EventTeamWin(int teamNumber){
		//if we not winner so no change in exp, or we a winner but no send were initiate we sync data 
		if (myPlayer.team	== teamNumber) {
            UpMoney(ParamLibrary.PARAM_WIN);
			SyncReward();
		}else{
		   UpMoney(ParamLibrary.PARAM_LOSE);
			SyncReward();
		}
		
	}
	public void EventKilledByFriend(Player target,Player friend){
		if (target == myPlayer) {
			UpMoney(ParamLibrary.PARAM_KILL_BY_FRIEND);	
		}
	}
    public void EventKilledAFriend(Player target, Player friend, KillInfo killinfo)
    {
		if (target == myPlayer) {
			UpMoney(ParamLibrary.PARAM_KILL_FRIEND);	
		}
	}
    public void EventPawnDeadByPlayer(Player target, KillInfo killinfo)
    {
		SyncReward();
	}
	public void EventPawnDeadByAI(Player target){
		SyncReward();
	}
	public	void EventJuggerTake(Player target){
	
	}
    public void EventJuggerKill(Player target, KillInfo killinfo)
    {
	
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
    public void EventRoomFinished()
    {
        UpMoney(ParamLibrary.PARAM_ROOM_FINISHED);	

    }

    public void AddPremiumBoost(int cashBoost)
    {
        upCash += cashBoost;
       
        SyncReward();
    }

    private static RewardManager s_Instance = null;

    public static RewardManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                //Debug.Log ("FIND");
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(RewardManager)) as RewardManager;
            }


            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                //	Debug.Log ("CREATE");
                GameObject obj = new GameObject("RewardManager");
                s_Instance = obj.AddComponent(typeof(RewardManager)) as RewardManager;

            }

            return s_Instance;
        }
    }
}