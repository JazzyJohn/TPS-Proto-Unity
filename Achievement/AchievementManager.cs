using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnidecodeSharpFork;

//ALL ACHIEVMENT IN DIIFERENT THREAD
public class AchievementParam{
	public float current;
	public float needed;
	public string resetEvent;


}
public enum AchievementType{
	ACHIEVEMENT,
	DAYLIC,
	TASK

}
public struct AchievementReward
{
    public int cash;
    public int gold;
    public int skill;
}
public class Achievement{
	public Dictionary<string,AchievementParam> achivParams = new Dictionary<string, AchievementParam>();
	public String name;
    public String engName;
	public String description;
    public Achievement next;
    public AchievementReward reward = new AchievementReward();
    public int order;
  	public int achievementId;
	public Texture2D textureIcon;
	public AchievementType type;
	public bool isDone = false;
	public int amount  =0;
	public bool isMultiplie = false;
	public override string ToString(){
		return name + " " + description + " " + achievementId + " " + achivParams.ToString();
	}
	public virtual bool CheckDone(){
		bool lIsDone = true;
		foreach (KeyValuePair<string, AchievementParam> entry in achivParams) {
			lIsDone=lIsDone&&(entry.Value.current>=entry.Value.needed);
		}
		isDone = lIsDone;
		return isDone;
	}


    public string GetProgress()
    {
        float cnt=0.0f,max=0.0f;
        foreach (KeyValuePair<string, AchievementParam> entry in achivParams)
        {
            cnt += entry.Value.current;
            max += entry.Value.needed;

        }
        return cnt.ToString("0") +"/"+max.ToString("0");
    }
    public float GetFloatProgress()
    {
        float cnt = 0.0f;
        foreach (KeyValuePair<string, AchievementParam> entry in achivParams)
        {
            cnt += entry.Value.current;
            

        }
        return cnt;
    }
}
public class AchievementManager : MonoBehaviour, LocalPlayerListener, GameListener{
	//Its here because it's clearly parse params and this calss for parse www data
	
	struct IncomingMessage{
		public string param;
		public float delta;
        public string mapContext;
        public string gunContext;
        public int teamContext;
        public int weaponType;

	}
		
	ConcurrentQueue<IncomingMessage> incomeQueue = new ConcurrentQueue<IncomingMessage>();


	ConcurrentQueue<Achievement> outcomeQueue = new ConcurrentQueue<Achievement>();
	//initilization

	public void Init(string uid){
		EventHolder.instance.Bind (this);
		DontDestroyOnLoad(transform.gameObject);
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", uid);
		UID = uid;
		StartCoroutine(LoadAchivment (form));
	
	}
	protected IEnumerator LoadAchivment(WWWForm form){
		//Debug.Log (form );
        WWW w = StatisticHandler.GetMeRightWWW(form, StatisticHandler.LOAD_ACHIVE);
		
		yield return w;
		//Debug.Log (w.text);
		IEnumerator numenator = ParseList (w.text);

		while(numenator.MoveNext()){
			yield return numenator.Current;
		}
	}
	//parse XML string to normal Achivment Pattern
	public  IEnumerator ParseList(string XML){
        incomeQueue.Clear();
		outcomeQueue.Clear();
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
//       Debug.Log(XML);
		ongoingAchivment= new List<Achievement>();
		finishedAchivment= new List<Achievement>();
		daylyFinished =bool.Parse(xmlDoc.SelectSingleNode("achivements/daylyfinish").InnerText);
		foreach (XmlNode node in xmlDoc.SelectNodes("achivements/achivement")) {
			Achievement achivment = new Achievement();
			achivment.name = node.SelectSingleNode("name").InnerText;
            achivment.engName = achivment.name.Unidecode();
			achivment.description = node.SelectSingleNode("description").InnerText;
            if (node.SelectSingleNode("nextdescription") != null)
            {
                	Achievement nextachivment = new Achievement();
                    nextachivment.description = node.SelectSingleNode("nextdescription").InnerText;
                    nextachivment.name = node.SelectSingleNode("name").InnerText;
                    achivment.next = nextachivment;
                    nextachivment.reward.cash = int.Parse(node.SelectSingleNode("nextcashreward").InnerText);
                    nextachivment.reward.gold = int.Parse(node.SelectSingleNode("nextgoldreward").InnerText);
                    nextachivment.reward.skill = int.Parse(node.SelectSingleNode("nextskillreward").InnerText);
            }
            achivment.reward.cash = int.Parse(node.SelectSingleNode("cashreward").InnerText);
            achivment.reward.gold = int.Parse(node.SelectSingleNode("goldreward").InnerText);
            achivment.reward.skill = int.Parse(node.SelectSingleNode("skillreward").InnerText);
			achivment.achievementId = int.Parse(node.SelectSingleNode("id").InnerText);
            achivment.order = int.Parse(node.SelectSingleNode("order").InnerText);
			WWW www = StatisticHandler.GetMeRightWWW( node.SelectSingleNode ("icon").InnerText);
		
			yield return www;
			achivment.textureIcon = new Texture2D(www.texture.width, www.texture.height);
			www.LoadImageIntoTexture(achivment.textureIcon);
			
			foreach (XmlNode paramNode in node.SelectNodes("param")) {
				AchievementParam param = new AchievementParam();
                if (node.SelectSingleNode("progress") != null)
                {
                    param.current = float.Parse(node.SelectSingleNode("progress").InnerText);
                }
				param.needed = float.Parse(paramNode.SelectSingleNode("value").InnerText);
				param.resetEvent = paramNode.SelectSingleNode("resetname").InnerText;
				achivment.achivParams.Add(paramNode.SelectSingleNode("name").InnerText,param);
			}
				achivment.amount  =int.Parse(node.SelectSingleNode("amount").InnerText);
//			Debug.Log("ACHIVMENT " +achivment);
			bool open = bool.Parse(node.SelectSingleNode("open").InnerText);
            achivment.isDone = open;
            achivment.isMultiplie = bool.Parse(node.SelectSingleNode("multiplie").InnerText);
			achivment.type = (AchievementType)Enum.Parse(typeof(AchievementType), node.SelectSingleNode("type").InnerText);
			//Debug.Log(open);
            ongoingAchivment.Add(achivment);
			if(open){
             
                if (achivment.isMultiplie)
                {
					bool ready = bool.Parse(node.SelectSingleNode("ready").InnerText);
                    Debug.Log(achivment.description + "  " + ready);
					if(!ready){
					    finishedAchivment.Add(achivment);
                    }
                    else
                    {
                        achivment.isDone = false;   
                    }
				}else{
					finishedAchivment.Add(achivment);
				}
				
				
			}

		}
		//loots of counting arrays, dictionary, logic don't want to drop fps by this)
		 myThread = new Thread(new ThreadStart(this.AchivmentLoop));
		 myThread.Start ();		
	}

	//Achivment Hondler
	private List<Achievement> ongoingAchivment;

	private List<Achievement> finishedAchivment;
	
	public bool daylyFinished = false;

	private Thread myThread;

    private bool work = true;

	private void AchivmentLoop(){
		while (work) {
			//Debug.Log (incomeQueue.Count	);
			try{
                bool wasChange = false;
						while (incomeQueue.Count>0) {
								IncomingMessage mess = incomeQueue.Dequeue ();
                                //Debug.Log(mess.param);
								if(IsPractice){
									continue;
								}

                          
                                if (mess.param == ParamLibrary.PARAM_TASK_RESET)
                                {
									foreach (Achievement achiv in ongoingAchivment) {
										  if (achiv.type == AchievementType.DAYLIC){
												foreach(AchievementParam param in achiv.achivParams.Values ) {
													param.current =0;
												}
										  }
									}
								}else{

                                    string[] events  = new string[9]{
                                            mess.param,
                                            mess.param + "by"+ mess.gunContext,
                                            mess.param + "with"+ mess.weaponType,
                                            mess.param + "on" + mess.mapContext,
                                            mess.param + "team"+mess.teamContext.ToString(),
                                            mess.param + "by"+ mess.gunContext + "on" +  mess.mapContext,
                                            mess.param + "by"+ mess.gunContext + "team" +  mess.teamContext.ToString(),
                                            mess.param + "on"+ mess.gunContext + "team" +  mess.teamContext.ToString(),
                                            mess.param + "by"+ mess.gunContext + "on"+ mess.mapContext + "team" +  mess.teamContext.ToString(),
                                                };
                                    foreach (string eventStr in events)
                                    {

                                     //   Debug.Log(eventStr);
                                        foreach (Achievement achiv in ongoingAchivment)
                                        {
                                            if (achiv.isDone)
                                            {
                                                continue;
                                            }
                                            if (achiv.achivParams.ContainsKey(eventStr))
                                            {
                                                //Debug.Log(mess.param + "  " + mess.delta + "  " + achiv.achivParams[mess.param].current);
                                                wasChange = true;
                                                achiv.achivParams[eventStr].current += mess.delta;
                                            }
                                            foreach (AchievementParam param in achiv.achivParams.Values)
                                            {
                                                if (param.resetEvent == eventStr)
                                                {
                                                    param.current = 0;
                                                }
                                            }

                                        }
                                    }
								}
						}
						ongoingAchivment.ForEach (delegate(Achievement obj) {
                                if (obj.isDone)
                                {
                                    return ;
                                }
								if (obj.CheckDone ()) {
                                    Debug.Log(obj.description);
										outcomeQueue.Enqueue (obj);
										
								}
						});

						//	
                        if (wasChange)
                        {
                            needupdate = true;
                        }
						Thread.Sleep (1000);
					}catch( Exception e){
                        Debug.LogError(e);
					}
				}
	}
	protected void SyncAchievement(List<int> syncAchivment,Achievement[] tasks){
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		foreach(int id in syncAchivment){
			form.AddField ("ids[]", id);
		}

        foreach (Achievement task in tasks)
        {
            form.AddField("daylicProggress[" + task.order + "]", task.GetFloatProgress().ToString("0.0"));

        }
		


		StatisticHandler.instance.StartCoroutine(SendAchive(form));
		
	}
	IEnumerator SendAchive(WWWForm form){
        WWW w = StatisticHandler.GetMeRightWWW(form, StatisticHandler.SAVE_ACHIVE);
		yield return w;
        Debug.Log(w.text);
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(w.text);
		
		if(xmlDoc.SelectSingleNode("result/error").InnerText=="0"){
			daylyFinished =bool.Parse(xmlDoc.SelectSingleNode("result/daylyfinish").InnerText);
		}else{
		
		}
	}
	
	public IEnumerator SkipAchive(int id){
        GUIHelper.ShowConnectionStart();
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		form.AddField ("id", id);
        WWW w = StatisticHandler.GetMeRightWWW(form, StatisticHandler.SKIP_ACHIVE);
		yield return w;
        Debug.Log(w.text);
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(w.text);
		
		if(xmlDoc.SelectSingleNode("result/error").InnerText=="0"){
            work = false;
            yield return new WaitForSeconds(1.0f);
            Debug.Log("Sending");
			form = new WWWForm ();
			
			form.AddField ("uid", UID);
			w = StatisticHandler.GetMeRightWWW(form, StatisticHandler.LOAD_ACHIVE);
		
			yield return w;
		    Debug.Log (w.text);
			IEnumerator numenator = ParseList (w.text);

			while(numenator.MoveNext()){
				yield return numenator.Current;
			}
            Debug.Log("reload achivemtn");
            GlobalPlayer.instance.gold -= int.Parse(xmlDoc.SelectSingleNode("result/price").InnerText);
            MissionGUI gui = FindObjectOfType<MissionGUI>();
            if (gui != null)
            {
                gui.Draw();
            }
		}else{
		
		}
        GUIHelper.ConnectionStop();
	}

    public IEnumerator UpdateTask()
    {
        GUIHelper.ShowConnectionStart();
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
	
        WWW w = StatisticHandler.GetMeRightWWW(form, StatisticHandler.UPDATE_ACHIVE);
		yield return w;
        Debug.Log(w.text);
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(w.text);
		
		if(xmlDoc.SelectSingleNode("result/error").InnerText=="0"){
            work = false;
           yield return new WaitForSeconds(1.0f);
			form = new WWWForm ();
			
			form.AddField ("uid", UID);
           
			w = StatisticHandler.GetMeRightWWW(form, StatisticHandler.LOAD_ACHIVE);
		
			yield return w;
			Debug.Log (w.text);
			IEnumerator numenator = ParseList (w.text);

			while(numenator.MoveNext()){
				yield return numenator.Current;
			}
            Debug.Log("reload achivemtn");
            GlobalPlayer.instance.gold -= int.Parse(xmlDoc.SelectSingleNode("result/price").InnerText);
            MissionGUI gui = FindObjectOfType<MissionGUI>();
            if (gui != null)
            {
                gui.Draw();
            }
		}else{
		
		}
        GUIHelper.ConnectionStop();
	}
	void OnDestroy(){
		if (myThread != null) {
			myThread.Abort ();
		}
		
	}
    private bool IsPractice;

    private bool needupdate = false;

    private int time =-10;

    private const int TIME_DELAY = 10;
	void Update(){
        if (GameRule.instance != null)
        {
            IsPractice = GameRule.instance.IsPractice();
        }
        else
        {
            IsPractice = false;
        }
		List<int> syncAchivment = new List<int>();
		while (outcomeQueue.Count>0) {
			Achievement finished = outcomeQueue.Dequeue();
			myPlayer.AchivmenUnlock(finished);
			finishedAchivment.Add(finished);
            if (finished.isMultiplie)
            {
                GA.API.Design.NewEvent("Achievement:Daylic:" + finished.engName);
            }
            else
            {
                GA.API.Design.NewEvent("Achievement:Open:" + finished.engName);
            }
          
			syncAchivment.Add(finished.achievementId);
		}

        if (needupdate && time+TIME_DELAY<Time.time)
        {
            time = Mathf.RoundToInt(Time.time);
            needupdate = false;
            SyncAchievement(new List<int>(), GetTask());	
        }
        
		if (syncAchivment.Count > 0) {
            time = Mathf.RoundToInt(Time.time);
            needupdate = false;
			SyncAchievement (syncAchivment,GetTask());		
		}
		//Debug.Log (onFinishedAchivment.Count);
	}
	public List<Achievement> GetAchivment (){
	
		return finishedAchivment;
	}

    public List<Achievement> GetDaylics()
    {
        List<Achievement> list = new List<Achievement>();
        for (int i = 0; i < ongoingAchivment.Count; i++)
        {
          //  Debug.Log(ongoingAchivment[i].name +"  " + ongoingAchivment[i].isMultiplie);
            if (ongoingAchivment[i].type == AchievementType.DAYLIC)
            {
                list.Add(ongoingAchivment[i]);
            }
        }
        return list;
    }
	public Achievement[] GetTask()
    {
        Achievement[] list = new Achievement[3];
        for (int i = 0; i < ongoingAchivment.Count; i++)
        {
          //  Debug.Log(ongoingAchivment[i].name +"  " + ongoingAchivment[i].isMultiplie);
            if (ongoingAchivment[i].type == AchievementType.TASK)
            {
                list[ongoingAchivment[i].order-1] =ongoingAchivment[i];
            }
        }
        return list;
    }

	private static AchievementManager s_Instance = null;
	
	public static AchievementManager instance {
		get {
			if (s_Instance == null) {
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (AchievementManager)) as AchievementManager;
			}
			
			// If it is still null, create a new instance
			if (s_Instance == null) {
				GameObject obj = new GameObject("AchievementManager");
				s_Instance = obj.AddComponent(typeof (AchievementManager)) as AchievementManager;
				
			}
			
			return s_Instance;
		}
	}

	//event handle and logic to generate message for achivment

	public Player myPlayer;
	public string UID;
	public Vector3 wallRunningStartPosition;
    private int killCnt;

	public void EventAppear(Player target){
		if (target.isMine) {
			myPlayer = target;
			

		}
	}
   IncomingMessage CreateMessageFromContext(KillInfo killinfo){
       IncomingMessage mess = new IncomingMessage();
       mess.delta = 1.0f;
       BaseWeapon weapon = ItemManager.instance.GetWeaponprefabByID(killinfo.weaponId);

       if (weapon != null)
       mess.weaponType = (int)weapon.slotType;
       mess.gunContext = killinfo.weaponId.ToString();
       mess.mapContext = ServerHolder.currentMap;
       mess.teamContext = myPlayer.team;
       
       return mess;
   }
   IncomingMessage CreateMessageFromContext()
   {
       IncomingMessage mess = new IncomingMessage();
       mess.delta = 1.0f;
      
       mess.mapContext = ServerHolder.currentMap;
       mess.teamContext = myPlayer.team;

       return mess;
   }
	public void EventPawnDeadByPlayer(Player target,KillInfo killinfo){
		if (target == myPlayer) {
			IncomingMessage mess = CreateMessageFromContext(killinfo);
			mess.param =ParamLibrary.PARAM_DEATH;
        
			incomeQueue.Enqueue(mess);
            killCnt = 0;


		}
	}
	public void EventPawnDeadByAI(Player target){

		if (target == myPlayer) {
            IncomingMessage mess = CreateMessageFromContext();
			mess.param =ParamLibrary.PARAM_DEATH_AI;
	
			incomeQueue.Enqueue(mess);
            mess = CreateMessageFromContext();
            mess.param = ParamLibrary.PARAM_DEATH;
       
            incomeQueue.Enqueue(mess);
            killCnt = 0;
		}
	}
    public void EventPawnKillPlayer(Player target, KillInfo killinfo)
    {
		if (target == myPlayer) {
            IncomingMessage mess = CreateMessageFromContext(killinfo);
			mess.param =ParamLibrary.PARAM_KILL;
         
            incomeQueue.Enqueue(mess);
            if (killinfo.isHeadShoot)
            {
                mess = CreateMessageFromContext(killinfo);
                mess.param = ParamLibrary.PARAM_HEAD_SHOOT;
                
                incomeQueue.Enqueue(mess);
            }
            killCnt++;
            switch (killCnt)
            {
                case 2:
                    mess = CreateMessageFromContext(killinfo);
                    mess.param = ParamLibrary.PARAM_DOUBLE_KILL;
                   
			        incomeQueue.Enqueue(mess);
                    break;
                case 3:
                     mess = CreateMessageFromContext(killinfo);
                    mess.param = ParamLibrary.PARAM_TRIPLE_KILL;
                    
			        incomeQueue.Enqueue(mess);
                    break;
                case 4:
                     mess = CreateMessageFromContext(killinfo);
                    mess.param = ParamLibrary.PARAM_RAMPAGE_KILL;
                    incomeQueue.Enqueue(mess);
                    break;
            }
		}
	}

	//hawk
	public void EventKilledByFriend(Player target,Player friend){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =ParamLibrary.PARAM_KILL_BY_FRIEND;
			incomeQueue.Enqueue(mess);
		}
	}
    public void EventKilledAFriend(Player target, Player friend, KillInfo killinfo)
    {
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
            mess.gunContext = killinfo.weaponId.ToString();
            mess.mapContext = ServerHolder.currentMap;
			mess.param =ParamLibrary.PARAM_KILL_FRIEND;
            mess.teamContext = myPlayer.team;
			incomeQueue.Enqueue(mess);
		}
	}

    public void EventPawnKillAI(Player target, KillInfo killinfo)
    {
		if (target == myPlayer) {
            IncomingMessage mess = CreateMessageFromContext(killinfo);
            mess.param = ParamLibrary.PARAM_KILL;

            incomeQueue.Enqueue(mess);
            if (killinfo.isHeadShoot)
            {
                mess = CreateMessageFromContext(killinfo);
                mess.param = ParamLibrary.PARAM_HEAD_SHOOT;

                incomeQueue.Enqueue(mess);
            }
            killCnt++;
            switch (killCnt)
            {
                case 2:
                    mess = CreateMessageFromContext(killinfo);
                    mess.param = ParamLibrary.PARAM_DOUBLE_KILL;

                    incomeQueue.Enqueue(mess);
                    break;
                case 3:
                    mess = CreateMessageFromContext(killinfo);
                    mess.param = ParamLibrary.PARAM_TRIPLE_KILL;

                    incomeQueue.Enqueue(mess);
                    break;
                case 4:
                    mess = CreateMessageFromContext(killinfo);
                    mess.param = ParamLibrary.PARAM_RAMPAGE_KILL;
                    incomeQueue.Enqueue(mess);
                    break;
            }
        }
	}
	public void EventPawnGround(Player target){

	}
	public void EventPawnDoubleJump(Player target){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =ParamLibrary.PARAM_DOUBLE_JUMP;
			incomeQueue.Enqueue(mess);
		}
	}
	public void EventStartWallRun(Player target,Vector3 position){
		if (target == myPlayer) {
			wallRunningStartPosition=position;
			//Debug.Log (position);
		}
	}
	public void EventStartSprintRun(Player target, Vector3 position){

	}
	public void EventEndSprintRun(Player target,Vector3 position){

	}
	public	void EventJuggerTake(Player target){
		
		}
    public void EventJuggerKill(Player target, KillInfo killinfo)
    {
	
	}
	
	public void EventPawnKillAssistPlayer(Player target){
	
	}
    public void EventPawnKillAssistAI(Player target){
	
	}
	public void EventEndWallRun(Player target, Vector3 position){
		if (target == myPlayer) {
			if(wallRunningStartPosition.sqrMagnitude==0){
				return;
			}
			IncomingMessage mess = new IncomingMessage();
			mess.delta=(wallRunningStartPosition -position).magnitude;
			//Debug.Log (mess.delta);
			wallRunningStartPosition = Vector3.zero;
			mess.param =ParamLibrary.PARAM_WALL_RUN;
			incomeQueue.Enqueue(mess);
			
		}
	}
	public void EventPawnReload(Player target){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =ParamLibrary.PARAM_RELOAD;
			incomeQueue.Enqueue(mess);
		}
	}
	public void EventRoomFinished(){
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =ParamLibrary.PARAM_ROOM_FINISHED;
			incomeQueue.Enqueue(mess);
	
	
	
	}
	public void UnEvnetAchive(string key, float value){
		IncomingMessage mess = new IncomingMessage();
		mess.delta=value;
		mess.param =key;
		incomeQueue.Enqueue(mess);
	}

    public void EventStart() { }
    public void EventTeamWin(int teamNumber) {
        if (teamNumber == myPlayer.team)
        {

            IncomingMessage mess = CreateMessageFromContext();
            mess.param = ParamLibrary.PARAM_WIN;
        
			incomeQueue.Enqueue(mess);
            killCnt = 0;
        }
    }
    public void EventRestart() { }
    
}