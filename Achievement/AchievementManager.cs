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
public class Achievement{
	public Dictionary<string,AchievementParam> achivParams = new Dictionary<string, AchievementParam>();
	public String name;
    public String engName;
	public String description;
  	public int achievementId;
	public Texture2D textureIcon;
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
}
public class AchievementManager : MonoBehaviour, LocalPlayerListener, GameListener{
	//Its here because it's clearly parse params and this calss for parse www data
	public static string PARAM_DEATH = "Death";
    public static string PARAM_DEATH_AI = "DeathAI";
    public static string PARAM_KILL = "Kill";
    public static string PARAM_KILL_AI = "KillAi";
    public static string PARAM_DOUBLE_JUMP = "DoubeJump";
    public static string PARAM_JUMP = "Jump";
    public static string PARAM_WALL_RUN = "WallRun";
    public static string PARAM_RELOAD = "Reload";
    public static string PARAM_KILL_FRIEND = "KillFriend";
    public static string PARAM_KILL_BY_FRIEND = "KilledByFriend";
    public static string PARAM_ROOM_FINISHED = "RoomFinished";
    public static string PARAM_HEAD_SHOOT = "HeadShoot";
    public static string PARAM_HEAD_SHOOT_AI = "HeadShootAI";
    public static string PARAM_WIN = "Win";
    public static string PARAM_STIM_PACK = "StimPack";
	struct IncomingMessage{
		public string param;
		public float delta;

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
	protected IEnumerator ParseList(string XML){
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
           
			achivment.achievementId = int.Parse(node.SelectSingleNode("id").InnerText);
			WWW www = StatisticHandler.GetMeRightWWW( node.SelectSingleNode ("icon").InnerText);
		
			yield return www;
			achivment.textureIcon = new Texture2D(www.texture.width, www.texture.height);
			www.LoadImageIntoTexture(achivment.textureIcon);
			
			foreach (XmlNode paramNode in node.SelectNodes("param")) {
				AchievementParam param = new AchievementParam();
				param.current =0.0f;
				param.needed = float.Parse(paramNode.SelectSingleNode("value").InnerText);
				param.resetEvent = paramNode.SelectSingleNode("resetname").InnerText;
				achivment.achivParams.Add(paramNode.SelectSingleNode("name").InnerText,param);
			}
				achivment.amount  =int.Parse(node.SelectSingleNode("amount").InnerText);
//			Debug.Log("ACHIVMENT " +achivment);
			bool open = bool.Parse(node.SelectSingleNode("open").InnerText);
            achivment.isDone = open;
            achivment.isMultiplie = bool.Parse(node.SelectSingleNode("multiplie").InnerText);
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

	private void AchivmentLoop(){
		while (true) {
			//Debug.Log (incomeQueue.Count	);
			try{
						while (incomeQueue.Count>0) {
								IncomingMessage mess = incomeQueue.Dequeue ();
                                //Debug.Log(mess.param);
								if(IsPractice){
									continue;
								}
								foreach (Achievement achiv in ongoingAchivment) {
                                        if (achiv.isDone)
                                        {
                                            continue;
                                        }
										if (achiv.achivParams.ContainsKey (mess.param)) {
                                            //Debug.Log(mess.param + "  " + mess.delta + "  " + achiv.achivParams[mess.param].current);
												achiv.achivParams [mess.param].current += mess.delta;
										}
										foreach(AchievementParam param in achiv.achivParams.Values ) {
											if(param.resetEvent==mess.param){
												param.current =0;
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
					
						Thread.Sleep (1000);
					}catch( Exception e){
                        Debug.LogError(e);
					}
				}
	}
	protected void SyncAchievement(List<int> syncAchivment){
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		foreach(int id in syncAchivment){
			form.AddField ("ids[]", id);
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
	
	void OnDestroy(){
		if (myThread != null) {
			myThread.Abort ();
		}
		
	}
    private bool IsPractice;
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
		if (syncAchivment.Count > 0) {
			SyncAchievement (syncAchivment);		
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
            if (ongoingAchivment[i].isMultiplie)
            {
                list.Add(ongoingAchivment[i]);
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

	public void EventAppear(Player target){
		if (target.isMine) {
			myPlayer = target;
			

		}
	}

	public void EventPawnDeadByPlayer(Player target,KillInfo killinfo){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_DEATH;
		
			incomeQueue.Enqueue(mess);
			mess = new IncomingMessage();
			mess.delta=1.0f;
            mess.param = PARAM_DEATH + "by" + killinfo.weaponId.ToString();
		
			incomeQueue.Enqueue(mess);


		}
	}
	public void EventPawnDeadByAI(Player target){

		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_DEATH_AI;
		
			incomeQueue.Enqueue(mess);
            mess = new IncomingMessage();
            mess.delta = 1.0f;
            mess.param = PARAM_DEATH;

            incomeQueue.Enqueue(mess);
		}
	}
    public void EventPawnKillPlayer(Player target, KillInfo killinfo)
    {
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_KILL;
			incomeQueue.Enqueue(mess);
			mess = new IncomingMessage();
			mess.delta=1.0f;
            mess.param = PARAM_KILL + "by" + killinfo.weaponId.ToString();
		
			incomeQueue.Enqueue(mess);
            if (killinfo.isHeadShoot)
            {
                mess = new IncomingMessage();
                mess.delta = 1.0f;
                mess.param = PARAM_HEAD_SHOOT;
                incomeQueue.Enqueue(mess);
            }
		}
	}

	//hawk
	public void EventKilledByFriend(Player target,Player friend){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_KILL_BY_FRIEND;
			incomeQueue.Enqueue(mess);
		}
	}
    public void EventKilledAFriend(Player target, Player friend, KillInfo killinfo)
    {
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_KILL_FRIEND;
			incomeQueue.Enqueue(mess);
		}
	}

    public void EventPawnKillAI(Player target, KillInfo killinfo)
    {
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_KILL_AI;
			incomeQueue.Enqueue(mess);
			mess = new IncomingMessage();
			mess.delta=1.0f;
            mess.param = PARAM_KILL_AI + "by" + killinfo.weaponId.ToString();
		
			incomeQueue.Enqueue(mess);
            if (killinfo.isHeadShoot)
            {
                mess = new IncomingMessage();
                mess.delta = 1.0f;
                mess.param = PARAM_HEAD_SHOOT_AI;
                incomeQueue.Enqueue(mess);
            }
		}
	}
	public void EventPawnGround(Player target){

	}
	public void EventPawnDoubleJump(Player target){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_DOUBLE_JUMP;
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
	public void EventEndWallRun(Player target, Vector3 position){
		if (target == myPlayer) {
			if(wallRunningStartPosition.sqrMagnitude==0){
				return;
			}
			IncomingMessage mess = new IncomingMessage();
			mess.delta=(wallRunningStartPosition -position).magnitude;
			//Debug.Log (mess.delta);
			wallRunningStartPosition = Vector3.zero;
			mess.param =PARAM_WALL_RUN;
			incomeQueue.Enqueue(mess);
			
		}
	}
	public void EventPawnReload(Player target){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_RELOAD;
			incomeQueue.Enqueue(mess);
		}
	}
	public void EventRoomFinished(){
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_ROOM_FINISHED;
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

            IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
            mess.param = PARAM_WIN;
			incomeQueue.Enqueue(mess);
        }
    }
    public void EventRestart() { }
    
}