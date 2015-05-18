using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
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
    public static string PARAM_DOUBLE_KILL = "DoubleKill";
	public static string PARAM_TRIPLE_KILL = "TripleKill";
	public static string PARAM_RAMPAGE_KILL = "Rampage";
	public static string PARAM_RAMPAGE_GOING_KILL = "RampageGoing";
    public static string PARAM_HATE = "hate";
    public static string PARAM_GREATE_WARRIOR = "GreateWarrior";
    public static string PARAM_DEATH_BRINGER = "DeathBringer";
    public static string PARAM_LEGEND = "Legend";
    public static string PARAM_WIN = "Win";
    public static string PARAM_SCORE = "Score";
    public static string PARAM_END_GAME = "EndGame";
	public static string PARAM_LOSE = "Lose";
	public static string PARAM_KILL_AI= "KillAI"; 
	public static string PARAM_KILL_FRIEND = "KillFriend";
	public static string PARAM_KILL_BY_FRIEND= "KilledByFriend";
    public static string PARAM_ROOM_FINISHED = "RoomFinished";
	public static string PARAM_JUGGER_TAKE= "JuggerTake";
    public static string PARAM_JUGGER_KILL= "JuggerKill";
	public static string PARAM_HEAD_SHOOT = "HeadShoot";
    public static string PARAM_LONG_SHOOT = "LongShoot";    
    public static string PARAM_HEAD_SHOOT_AI = "HeadShootAI";
	public static string PARAM_DEATH = "Death";
    public static string PARAM_DEATH_AI = "DeathAI";
    public static string PARAM_STIM_PACK = "StimPack";
	public static string PARAM_WALL_RUN = "WallRun";
	public static string PARAM_ASSIST= "Assist"; 
	public static string PARAM_ASSIST_AI= "AssistAI";
    public static string PARAM_DOUBLE_JUMP = "DoubeJump";
    public static string PARAM_RELOAD = "Reload";
    public static string PARAM_JUMP = "Jump";
	public static string PARAM_GRENADE_KILL = "GrenadeKill";
	public static string PARAM_DAMAGE = "Damage";
	public static string PARAM_DAMAGE_DELIVER= "DamageDeliver";
	public static string PARAM_AMMO_SPENT= "AmmoSpent";
	public static string PARAM_AMMO_HIT= "AmmoHit";
	
	public static string PARAM_TASK_RESET= "taskReset";
    public static string PARAM_MELEE_KILL="MeleeKill";
    public static string PARAM_KILL_MOUNT = "KillMount";
    public static string PARAM_TASK_COMPLITE = "TaskComplite";


    public static float LONG_SHOT_SQRT_MAG = 625.0f;
}
public delegate void UP(string action);
public delegate void KillUp(string action,KillInfo info);
public delegate void Apear(Player target);
public delegate void Sync();

public enum PlayerTitle{
    NONE,
    HATE,
    GREATE_WARRIOR,
    DEATH_BRINGER,
    LEGEND
}


public class ActionResolver:  LocalPlayerListener,GameListener
{
    public  List<UP> ups = new List<UP>();
    public  List<Sync> syncs  = new List<Sync>();
    public List<KillUp> killUps = new List<KillUp>();
    public  List<Apear> apears = new List<Apear>();
        //Event Section
	private Player myPlayer;
    private int playerStrike;
    private float lastKillTime;
    const float DOUBLE_KILL_TIME = 3.0f;
    const float TRIPLE_KILL_TIME = 3.0f;
    const float RAMPAGE_KILL_TIME = 4.0f;

    

    private int playerTitleKills;
    private PlayerTitle playerTitle;

    private int[] titleDictionary = new int[] { 20, 15, 10, 5 };

    private static ActionResolver s_Instance = null;


    private AnnonceType lastType = AnnonceType.KILL;
    public static ActionResolver instance
    {
        get
        {
            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                s_Instance = new ActionResolver();

            }

            return s_Instance;
        }
    }
    public void AddUp(UP up)
    {
        ups.Add(up);
    }
    public void AddUp(KillUp up)
    {
        killUps.Add(up);
    }
    public void AddSync(Sync up)
    {
        syncs.Add(up);
    }
    public void AddApear(Apear up)
    {
        apears.Add(up);
    }
    public ActionResolver()
    {
        EventHolder.instance.Bind(this);
    }
	public void EventAppear(Player target){
		if (target.isMine) {
			myPlayer = target;
            foreach (Apear apear in apears)
            {
                apear(target);
            }
		}
	}
    
    public void AllAction(string action)
    {
        foreach (UP up in ups)
        {
            up(action);
        }
    }
    public void AllKillUp(string action, KillInfo info)
    {
        foreach (KillUp up in killUps)
        {
            up(action,info);
        }
    }
    public void AllSync()
    {
        foreach (Sync up in syncs)
        {
            up();
        }
    }
    public AnnonceType GetLastKill()
    {
        return lastType;
    }
    public void EventPawnKillPlayer(Player target, KillInfo killinfo)
    {
		if (target == myPlayer) {
			playerStrike++;
            playerTitleKills++;
            AllAction(ParamLibrary.PARAM_KILL);
            AllKillUp(ParamLibrary.PARAM_KILL, killinfo);
            if (killinfo.isHeadShoot)
            {
                AllAction(ParamLibrary.PARAM_HEAD_SHOOT);
            }
            if (killinfo.isOnMount)
            {
                AllAction(ParamLibrary.PARAM_KILL_MOUNT);
            }
            if (killinfo.isMelee)
            {
                AllAction(ParamLibrary.PARAM_MELEE_KILL);
            }
            BaseWeapon weapon = ItemManager.instance.GetWeaponprefabByID(killinfo.weaponId);

            if (weapon != null && weapon.slotType==SLOTTYPE.GRENADE)
            {
                AllAction(ParamLibrary.PARAM_GRENADE_KILL);
            }
            lastType = AnnonceType.KILL;
            playerTitle = PlayerTitle.NONE;
            for (int i = 0; i < titleDictionary.Length; i++)
            {
                if (titleDictionary[i] < playerTitleKills)
                {
                    playerTitle = (PlayerTitle)(titleDictionary.Length-i);
                }
            }
            switch (playerTitle)
            {
                case PlayerTitle.HATE:
                    lastType = AnnonceType.HATE;
                    AllAction(ParamLibrary.PARAM_HATE);
                    AllKillUp(ParamLibrary.PARAM_HATE, killinfo);
                    break;
                case PlayerTitle.GREATE_WARRIOR:
                    lastType = AnnonceType.GREATE_WARRIOR;
                    AllAction(ParamLibrary.PARAM_GREATE_WARRIOR);
                    AllKillUp(ParamLibrary.PARAM_GREATE_WARRIOR, killinfo);
                    break;
                case PlayerTitle.DEATH_BRINGER:
                    lastType = AnnonceType.DEATH_BRINGER;
                    AllAction(ParamLibrary.PARAM_DEATH_BRINGER);
                    AllKillUp(ParamLibrary.PARAM_DEATH_BRINGER, killinfo);
                    break;
                case PlayerTitle.LEGEND:
                    lastType = AnnonceType.LEGEND;
                    AllAction(ParamLibrary.PARAM_LEGEND);
                    AllKillUp(ParamLibrary.PARAM_LEGEND, killinfo);
                    break;
            }
			switch(playerStrike){			
                case 1:
                    break;
				case 2:
                    if(lastKillTime+DOUBLE_KILL_TIME>Time.time){
                        AllAction(ParamLibrary.PARAM_DOUBLE_KILL);
                        lastType = AnnonceType.DOUBLEKILL;
                        AllKillUp(ParamLibrary.PARAM_DOUBLE_KILL, killinfo);
                    }
	                else
                        playerStrike =1;
                    
                    
					break;
				case 3:
                    if (lastKillTime + TRIPLE_KILL_TIME > Time.time){
                        AllAction(ParamLibrary.PARAM_TRIPLE_KILL);
                        lastType = AnnonceType.TRIPLIKILL;
                        AllKillUp(ParamLibrary.PARAM_TRIPLE_KILL, killinfo);
                    }
                    else
                        playerStrike = 1;
                  
					break;
				case 4:
                    if (lastKillTime + RAMPAGE_KILL_TIME > Time.time){
                        AllAction(ParamLibrary.PARAM_RAMPAGE_KILL);
                        AllKillUp(ParamLibrary.PARAM_RAMPAGE_KILL, killinfo);
                        lastType = AnnonceType.RAMPAGE;
                    }
                    else
                        playerStrike = 1;
                
					break;
				default:
                    if (lastKillTime + RAMPAGE_KILL_TIME > Time.time){
                        AllAction(ParamLibrary.PARAM_RAMPAGE_KILL);
                        AllKillUp(ParamLibrary.PARAM_RAMPAGE_KILL, killinfo);
                        lastType = AnnonceType.RAMPAGE;
                    }
                    else
                        playerStrike = 1;
					break;
					
			}
           

			
			lastKillTime=Time.time;
		}
	}
    public void EventPawnKillAI(Player target, KillInfo killinfo)
    {
	      EventPawnKillPlayer(target, killinfo);		
	
	}
	public void EventPawnKillAssistPlayer(Player target){
		if (target == myPlayer) {

            AllAction(ParamLibrary.PARAM_ASSIST);	
		}
	}
    public void EventPawnKillAssistAI(Player target){
		if (target == myPlayer) {

            AllAction(ParamLibrary.PARAM_ASSIST_AI);	
		}
	}
	public void EventTeamWin(int teamNumber){
		//if we not winner so no change in exp, or we a winner but no send were initiate we sync data 
		if (myPlayer.team	== teamNumber) {
            AllAction(ParamLibrary.PARAM_WIN);
            AllSync();
		}else{
            AllAction(ParamLibrary.PARAM_LOSE);
            AllSync();
		}
		
	}
	public void EventKilledByFriend(Player target,Player friend){
		if (target == myPlayer) {
            AllAction(ParamLibrary.PARAM_KILL_BY_FRIEND);	
		}
	}
    public void EventKilledAFriend(Player target, Player friend, KillInfo killinfo)
    {
		if (target == myPlayer) {
            AllAction(ParamLibrary.PARAM_KILL_FRIEND);	
		}
	}
    public void EventPawnDeadByPlayer(Player target, KillInfo killinfo)
    {
        playerTitleKills = 0;
        playerStrike = 0;
        playerTitle = PlayerTitle.NONE;
        AllSync();
	}
	public void EventPawnDeadByAI(Player target){
        playerTitleKills=0;
        playerStrike = 0;
        playerTitle = PlayerTitle.NONE;
        AllSync();
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
        AllAction(ParamLibrary.PARAM_ROOM_FINISHED);	

    }

  
}
public class  RewardManager : MonoBehaviour{ 
		
	
   

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
          //  multiplier = Mathf.RoundToInt(PremiumManager.STAMINA_MULTIPLIER);
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
    ActionResolver resolver;

    private string UID;
	public void Init(string uid){
       ActionResolver.instance.AddUp(UpMoney);
       ActionResolver.instance.AddSync(SyncReward);
       ActionResolver.instance.AddApear(EventApear);
       
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
    private ObscuredInt upCash = 0;
    private ObscuredInt upGold = 0;
	public void UpMoney(string reason){
		if(GameRule.instance.IsPractice()){
			return;
		}
	
        if(rewardMoneyDictionary.ContainsKey(reason)){
			MoneyReward reward = rewardMoneyDictionary[reason];
            int cahsReward =Mathf.RoundToInt(reward.cash * PremiumManager.GetMultiplierMoney(reason, Player.localPlayer.team));
            int goldReward = Mathf.RoundToInt(reward.gold * PremiumManager.GetMultiplierMoney(reason, Player.localPlayer.team));
            upCash += cahsReward;
            upGold += goldReward;
            if (reward.cash != 0 || reward.gold != 0)
            {
               reward.Increment();
            }
            if (PlayerMainGui.instance != null)
            {
                if (reward.cash > 0)
                {
                    String rewardStr = TextGenerator.instance.GetMoneyText(reason, Mathf.RoundToInt(cahsReward));
                    PlayerMainGui.instance.AddMessage(rewardStr, PlayerMainGui.MessageType.MONEY_REWARD);
                }
                if (reward.gold > 0)
                {
                    String rewardStr = TextGenerator.instance.GetMoneyText(reason, Mathf.RoundToInt(goldReward));
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
    public void EventApear(Player player)
    {

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