using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class LevelingManager : MonoBehaviour, LocalPlayerListener,GameListener{ 
	
	public const string PARAM_KILL = "Kill";
	public const string PARAM_WIN = "Win";
	public const string PARAM_LOSE = "Lose";
	public const string PARAM_KILL_AI= "KillAI"; 
	public const string PARAM_KILL_FRIEND = "KillFriend";
	public const string PARAM_KILL_BY_FRIEND= "KilledByFriend";
    public const string PARAM_HEAD_SHOOT = "HeadShoot";
    public const string PARAM_HEAD_SHOOT_AI = "HeadShootAI";
		
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
			}else{
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
	protected IEnumerator LoadLvling(WWWForm form){
		Debug.Log (form );
		WWW w = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			Debug.Log ("STATS HTTP SEND" + StatisticHandler.GetSTATISTIC_PHP()+ StatisticHandler.LOAD_LVL);
			w = new WWW (StatisticHandler.GetSTATISTIC_PHP()+ StatisticHandler.LOAD_LVL, form);
		}
		else{
			//Debug.Log ("STATS HTTPS SEND"+StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_LVL);
            w = new WWW(StatisticHandler.GetSTATISTIC_PHP_HTTPS() + StatisticHandler.LOAD_LVL, form);
		}

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
        if (!expDictionary.ContainsKey(cause))
        {
            return false ;
        }
		int exp = expDictionary[cause].amount;
		expDictionary[cause].Increment();
		bool sendByLvl=false;
		//Check if we on max lvl
		if(playerLvl<=playerNeededExp.Length){
			//add exp
			playerExp+=exp;
			//if lvl mark that it's time to sync with server
			if(playerExp>=playerNeededExp[playerLvl]){
				sendByLvl= true;
				playerLvl++;
			}
		}
		if(selected!=-1&&classLvl[selected]<=classNeededExp.Length){
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
		StatisticHandler.instance.StartCoroutine(StatisticHandler.SendForm (form,StatisticHandler.SAVE_LVL));
		
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
	public void EventAppear(Player target){
		if (target.isMine) {
			myPlayer = target;
			
		}
	}
    public void EventPawnKillPlayer(Player target, KillInfo killinfo)
    {
		if (target == myPlayer) {
			UpExp(PARAM_KILL,target.selected);
            if (killinfo.isHeadShoot)
            {
                UpExp(PARAM_HEAD_SHOOT, target.selected);
            }
		}
	}
    public void EventPawnKillAI(Player target, KillInfo killinfo)
    {
	

		if (target == myPlayer) {

			UpExp(PARAM_KILL_AI,target.selected);
            if (killinfo.isHeadShoot)
            {
                UpExp(PARAM_HEAD_SHOOT_AI, target.selected);
            }
		}
	
	}
	public void EventTeamWin(int teamNumber){
		//if we not winner so no change in exp, or we a winner but no send were initiate we sync data 
		if ((myPlayer.team	!= teamNumber&&!UpExp(PARAM_LOSE))||(myPlayer.team == teamNumber&&!UpExp(PARAM_WIN))) {
			SyncLvl();
		}
		
	}
	public void EventKilledByFriend(Player target,Player friend){
		if (target == myPlayer) {
			UpExp(PARAM_KILL_BY_FRIEND,target.selected);	
		}
	}
    public void EventKilledAFriend(Player target, Player friend, KillInfo killinfo)
    {
		if (target == myPlayer) {
			UpExp(PARAM_KILL_FRIEND,target.selected);	
		}
	}
    public void EventPawnDeadByPlayer(Player target, KillInfo killinfo) { }
	public void EventPawnDeadByAI(Player target){}
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