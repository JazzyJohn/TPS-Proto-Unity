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
public class  RewardManager : MonoBehaviour, LocalPlayerListener,GameListener{ 
		
	public const string PARAM_KILL = "Kill";
	public const string PARAM_WIN = "Win";
	public const string PARAM_LOSE = "Lose";
	public const string PARAM_KILL_AI= "KillAI"; 
	public const string PARAM_KILL_FRIEND = "KillFriend";
	public const string PARAM_KILL_BY_FRIEND= "KilledByFriend";
    public const string PARAM_ROOM_FINISHED = "RoomFinished";
	public const string PARAM_JUGGER_TAKE= "JuggerTake";
    public const string PARAM_JUGGER_KILL= "JuggerKill";
	
   

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
        List<RewardGUI> answer = new List<RewardGUI>();
        foreach (KeyValuePair<string, MoneyReward> entry in rewardMoneyDictionary)
        {
            if (entry.Value.GetRewardCounter() > 0)
            {
                if (entry.Value.cash > 0)
                {
                    RewardGUI reward = new RewardGUI();
                    reward.count = entry.Value.GetRewardCounter();

                    reward.amount = reward.count * entry.Value.cash;
                    reward.text = TextGenerator.instance.GetSimpleText(entry.Key);
                    reward.isCash = true;
                    answer.Add(reward);
                }
                if (entry.Value.gold>0){
                     RewardGUI reward = new RewardGUI();
                    reward.count = entry.Value.GetRewardCounter();

                    reward.amount = reward.count * entry.Value.cash;
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
        if(rewardMoneyDictionary.ContainsKey(reason)){
	    	upCash+= rewardMoneyDictionary[reason].cash;
			upGold+= rewardMoneyDictionary[reason].gold;
            if (rewardMoneyDictionary[reason].cash != 0 || rewardMoneyDictionary[reason].gold != 0)
            {
                rewardMoneyDictionary[reason].Increment();
            }
        }
        if (PlayerMainGui.instance != null)
        {
            String reward = TextGenerator.instance.GetMoneyText(reason,rewardMoneyDictionary[reason].cash, rewardMoneyDictionary[reason].gold);
            PlayerMainGui.instance.AddMessage(reward, PlayerMainGui.MessageType.MONEY_REWARD);
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
			UpMoney(PARAM_KILL);	
		}
	}
    public void EventPawnKillAI(Player target, KillInfo killinfo)
    {
	

		if (target == myPlayer) {

			UpMoney(PARAM_KILL_AI);	
		}
	
	}
	public void EventTeamWin(int teamNumber){
		//if we not winner so no change in exp, or we a winner but no send were initiate we sync data 
		if (myPlayer.team	!= teamNumber||(myPlayer.team == teamNumber)) {
            UpMoney(PARAM_WIN);
			SyncReward();
		}else{
		   UpMoney(PARAM_LOSE);
			SyncReward();
		}
		
	}
	public void EventKilledByFriend(Player target,Player friend){
		if (target == myPlayer) {
			UpMoney(PARAM_KILL_BY_FRIEND);	
		}
	}
    public void EventKilledAFriend(Player target, Player friend, KillInfo killinfo)
    {
		if (target == myPlayer) {
			UpMoney(PARAM_KILL_FRIEND);	
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
        UpMoney(PARAM_ROOM_FINISHED);	

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