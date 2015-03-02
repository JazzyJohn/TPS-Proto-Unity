using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class LevelingManager : MonoBehaviour, LocalPlayerListener,GameListener{ 
	

		
	public int playerLvl = 0;
	
	public int playerExp = 0;
	
	public int[] playerNeededExp;

	public int[] classLvl;
	
	public int[] classExp;
	
	public int[] classNeededExp;
	
	public string UID;

	private Statistic Stat; // Статистика (+)

	public bool isLoaded = false;


	public PlayerMainGui.LevelStats GetPlayerStats(){
		PlayerMainGui.LevelStats stats  = new PlayerMainGui.LevelStats();
		stats.playerLvl = playerLvl;
		if(playerLvl==0){
			stats.playerProcent  =(int)(playerExp/(float)playerNeededExp[playerLvl]*100.0f);
        }
        else if (playerLvl >= playerNeededExp.Length)
        {
            stats.playerProcent = 100;
        }else{
			//Debug.Log ((playerExp-playerNeededExp[playerLvl-1])+ "/" +((float)playerNeededExp[playerLvl]-playerNeededExp[playerLvl-1])+(playerExp-playerNeededExp[playerLvl-1])/((float)playerNeededExp[playerLvl]-playerNeededExp[playerLvl-1]));
			stats.playerProcent =(int)((playerExp-playerNeededExp[playerLvl-1])/((float)playerNeededExp[playerLvl]-playerNeededExp[playerLvl-1])*100.0f);
		}

		stats.classLvl = new int[classLvl.Length];
		stats.classProcent = new int[classLvl.Length];
		for(int i = 0;i<classLvl.Length;i++){
			int oneClassLvl = classLvl[i];
			stats.classLvl[i] =  oneClassLvl;
			if(oneClassLvl==0){
				stats.classProcent[i]  =(int)(classExp[i]/(float)classNeededExp[oneClassLvl]*100.0f);
            }
            else if (oneClassLvl >= classNeededExp.Length)
            {
                stats.classProcent[i] = 100;
            }
            else {
				stats.classProcent[i] =(int)((classExp[i]-classNeededExp[oneClassLvl-1])/((float)classNeededExp[oneClassLvl]-classNeededExp[oneClassLvl-1])*100.0f);
			}
		}
		return stats;
	
	}
	class ExpReward : Reward{
        public int amount;
		public ExpReward(int amount){
            this.amount = amount;
		}
	}
    Dictionary<string, ExpReward> expDictionary = new Dictionary<string, ExpReward>();

    public List<RewardGUI>  GetAllReward()
    {
        int multiplier = 1;
        if (AfterGameBonuses.wasStamined)
        {
            multiplier = Mathf.RoundToInt(PremiumManager.STAMINA_MULTIPLIER);
        }
         List<RewardGUI> answer = new List<RewardGUI>();
         foreach (KeyValuePair<string, ExpReward> entry in expDictionary)
         {
             if (entry.Value.GetRewardCounter() > 0)
             {
                 RewardGUI reward = new RewardGUI();
                 reward.count = entry.Value.GetRewardCounter();
                 entry.Value.Reset();
                 reward.amount = reward.count * entry.Value.amount * multiplier;
                 reward.text = TextGenerator.instance.GetSimpleText(entry.Key);

                 answer.Add(reward);
             }
         }
         return answer;
    }

	public void Init(string uid){
		EventHolder.instance.Bind (this);
		DontDestroyOnLoad(transform.gameObject);
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", uid);
		UID = uid;
		StartCoroutine(LoadLvling (form));
	
	}
	void  OnApplicationQuit() {
		//QuitSyncLvl();

	}
	
	public void SetNetworkLvl(){
		NetworkController.Instance.SetNetworkLvl(playerLvl);
	}
	
	protected IEnumerator LoadLvling(WWWForm form){
		Debug.Log (form );
		WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.LOAD_LVL);
		
		
		yield return w;
		//Debug.Log (w.text);
		ParseList (w.text);
	
	}
	//parse XML string to normal Leveling Pattern
	protected void ParseList(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		
		//Parse Stuct Info First
		
		playerNeededExp= new int[int.Parse(xmlDoc.SelectSingleNode("leveling/player/levelcount").InnerText)];
		int i = 0;
		foreach (XmlNode node in xmlDoc.SelectNodes("leveling/player/level")) {
			playerNeededExp[i++]=int.Parse (node.InnerText);
		}
		int classAmount = int.Parse (xmlDoc.SelectSingleNode ("leveling/classes/classcount").InnerText);
		classExp = new int[classAmount];
		classLvl = new int[classAmount];
		classNeededExp = new int[int.Parse(xmlDoc.SelectSingleNode("leveling/classes/levelcount").InnerText)];
		i = 0;
		foreach (XmlNode node in xmlDoc.SelectNodes("leveling/classes/level")) {
			classNeededExp[i++]=int.Parse (node.InnerText);
		}
		foreach (XmlNode node in xmlDoc.SelectNodes("leveling/expdictionary/slots")) {
			//Debug.Log ("LEVELING" +node.SelectSingleNode("name").InnerText+" "+node.SelectSingleNode("value").InnerText);
			ExpReward reward = new ExpReward(int.Parse(node.SelectSingleNode("value").InnerText));
			reward.descr =node.SelectSingleNode("name").InnerText;
			expDictionary.Add(node.SelectSingleNode("name").InnerText,reward);
		}
		//Parse  Data
		playerLvl = int.Parse(xmlDoc.SelectSingleNode("leveling/player/currentlvl").InnerText);
		playerExp = int.Parse(xmlDoc.SelectSingleNode("leveling/player/currentexp").InnerText);
		 i =0;
		foreach (XmlNode node in xmlDoc.SelectNodes("leveling/classes/current")) {
					classLvl[i]= int.Parse(node.SelectSingleNode("lvl").InnerText);
					classExp[i++]= int.Parse(node.SelectSingleNode("exp").InnerText);
		}
        StartCoroutine(PassiveSkillManager.instance.InitSkillTree(XML));
		isLoaded = true;
	}
	//adding exp to current
	public bool UpExp(string cause,int selected=-1){
        if(GameRule.instance.IsPractice()){
			return false;
		}
		if (!expDictionary.ContainsKey(cause))
        {
            return false ;
        }
		
		int exp = expDictionary[cause].amount;
		exp = Mathf.RoundToInt( exp *PremiumManager. GetMultiplier());
        if (exp == 0)
        {
            return false;
        }
		expDictionary[cause].Increment();
		bool sendByLvl=false;
		//Check if we on max lvl
		if(playerLvl<playerNeededExp.Length){
			//add exp
			playerExp+=exp;
			//if lvl mark that it's time to sync with server
			if(playerExp>=playerNeededExp[playerLvl]){
				sendByLvl= true;
				playerLvl++;
			}
		}
		if(selected!=-1&&classLvl[selected]<classNeededExp.Length){
			classExp[selected]+=exp;
			if(classExp[selected]>=classNeededExp[classLvl[selected]]){
				sendByLvl= true;
				classLvl[selected]++;
			}
		}
		if(sendByLvl){
			SyncLvl();
		}
        if (PlayerMainGui.instance != null)
        {
            String reward = TextGenerator.instance.GetExpText(cause,exp);
            PlayerMainGui.instance.AddMessage(reward, PlayerMainGui.MessageType.LVL_REWARD);
        }
		return sendByLvl;
	}

  
	public void SyncLvl(){
		WWWForm form = new WWWForm ();
		
		form.AddField ("uid", UID);
		form.AddField ("playerExp", playerExp);
		form.AddField ("playerLvl", playerLvl);
		for(int i=0; i <classLvl.Length;i++){
			form.AddField ("classExp[]", classExp[i]);
			form.AddField ("classLvl[]", classLvl[i]);
		}
		
		StartCoroutine(SyncSend(form));
		SetNetworkLvl();
	}
	IEnumerator SyncSend(WWWForm form){
		WWW www =StatisticHandler.GetMeRightWWW(form,StatisticHandler.SAVE_LVL);
        yield return www;
        
        XmlDocument xmlDoc = new XmlDocument();
        Debug.Log(www.text);
        xmlDoc.LoadXml(www.text);
		XmlNode node =xmlDoc.SelectSingleNode("result/item_reward");
		if(node!=null){
			
			FindObjectOfType<AddShops>().NewItem(node.InnerText);
            ItemManager.instance.ReloadItem();
			
		}
        node = xmlDoc.SelectSingleNode("result/open_set");
        if (node != null)
        {

            GlobalPlayer.instance.open_set = int.Parse(node.InnerText);

        }

	}

	public void QuitSyncLvl(){
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		form.AddField ("playerExp", playerExp);
		form.AddField ("playerLvl", playerLvl);
		for(int i=0; i <classLvl.Length;i++){
			form.AddField ("classExp[]", classExp[i]);
			form.AddField ("classLvl[]", classLvl[i]);
		}
		StatisticHandler.SendTCP(StatisticHandler.SAVE_LVL,form);

	}
	
	//Event Section
	private Player myPlayer;
	private int playerStrike;
	
	public void EventAppear(Player target){
        //Debug.Log("PLAYER SPAWM"+   target);
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
					UpExp(ParamLibrary.PARAM_KILL,target.selected);
					break;
				case 3:
					UpExp(ParamLibrary.PARAM_TRIPLE_KILL,target.selected);
					break;
				case 4:
					UpExp(ParamLibrary.PARAM_RAMPAGE_KILL,target.selected);
					break;
				default:
					UpExp(ParamLibrary.PARAM_RAMPAGE_GOING_KILL,target.selected);
                    break;
					
			}
			
            if (killinfo.isHeadShoot)
            {
                UpExp(ParamLibrary.PARAM_HEAD_SHOOT, target.selected);
            }
		}
	}
    public void EventPawnKillAI(Player target, KillInfo killinfo)
    {
	

		if (target == myPlayer) {

			UpExp(ParamLibrary.PARAM_KILL_AI,target.selected);
            if (killinfo.isHeadShoot)
            {
                UpExp(ParamLibrary.PARAM_HEAD_SHOOT_AI, target.selected);
            }
		}
	
	}
	public void EventPawnKillAssistPlayer(Player target){
		if (target == myPlayer) {

			UpExp(ParamLibrary.PARAM_ASSIST, target.selected);	
		}
	}
    public void EventPawnKillAssistAI(Player target){
		if (target == myPlayer) {

			UpExp(ParamLibrary.PARAM_ASSIST_AI, target.selected);	
		}
	}
	public void EventTeamWin(int teamNumber){
		//if we not winner so no change in exp, or we a winner but no send were initiate we sync data 
        
		if ((myPlayer.team	!= teamNumber&&!UpExp(ParamLibrary.PARAM_LOSE))||(myPlayer.team == teamNumber&&!UpExp(ParamLibrary.PARAM_WIN))) {
			SyncLvl();
		}
		
	}
	public void EventKilledByFriend(Player target,Player friend){
		if (target == myPlayer) {
			UpExp(ParamLibrary.PARAM_KILL_BY_FRIEND,target.selected);	
		}
	}
    public void EventKilledAFriend(Player target, Player friend, KillInfo killinfo)
    {
		if (target == myPlayer) {
			UpExp(ParamLibrary.PARAM_KILL_FRIEND,target.selected);	
		}
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
	public void EventPawnReload(Player target){}
	public void EventStart(){}
	public void EventRestart(){
			SyncLvl();
	}
	public	void EventJuggerTake(Player target){
	
	}
    public void EventJuggerKill(Player target, KillInfo killinfo)
    {
	
	}
	public void EventRoomFinished(){}


    public void AddPremiumBoost(int boost)
    {
        playerExp += boost;
        //if lvl mark that it's time to sync with server
        if (playerExp >= playerNeededExp[playerLvl])
        {
            
            playerLvl++;
        }
        SyncLvl();
    }
	private static LevelingManager s_Instance = null;
	
	public static LevelingManager instance {
		get {
			if (s_Instance == null) {
				//Debug.Log ("FIND");
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (LevelingManager)) as LevelingManager;
			}

		
			// If it is still null, create a new instance
			if (s_Instance == null) {
			//	Debug.Log ("CREATE");
				GameObject obj = new GameObject("LevelingManager");
				s_Instance = obj.AddComponent(typeof (LevelingManager)) as LevelingManager;
				
			}
			
			return s_Instance;
		}
	}

}