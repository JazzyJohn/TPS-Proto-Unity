using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


//ALL ACHIEVMENT IN DIIFERENT THREAD
public class AchievementParam{
	public float current;
	public float needed;
	public string resetEvent;


}
public class Achievement{
	public Dictionary<string,AchievementParam> achivParams = new Dictionary<string, AchievementParam>();
	public String name;
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
		
}
public class AchievementManager : MonoBehaviour, LocalPlayerListener{
	//Its here because it's clearly parse params and this calss for parse www data
	public const string PARAM_DEATH = "Death";
	public const string PARAM_DEATH_AI = "DeathAI";
	public const string PARAM_KILL = "Kill";
	public const string PARAM_KILL_AI= "KillAI";
	public const string PARAM_DOUBLE_JUMP= "DoubeJump";
	public const string PARAM_WALL_RUN= "WallRun";
	public const string PARAM_RELOAD="Reload";
	public const string PARAM_KILL_FRIEND = "KillFriend";
	public const string PARAM_KILL_BY_FRIEND= "KilledByFriend"; 
	public const string PARAM_ROOM_FINISHED = "RoomFinished";
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
		WWW w = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			Debug.Log ("STATS HTTP SEND" + StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_ACHIVE);
			w = new WWW (StatisticHandler.STATISTIC_PHP + StatisticHandler.LOAD_ACHIVE, form);
		}
		else{
			Debug.Log ("STATS HTTPS SEND"+StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_ACHIVE);
			w = new WWW (StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_ACHIVE, form);
		}
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
//        Debug.Log(XML);
		ongoingAchivment= new List<Achievement>();
		finishedAchivment= new List<Achievement>();
		daylyFinished =bool.Parse(xmlDoc.SelectSingleNode("achivements/daylyfinish").InnerText);
		foreach (XmlNode node in xmlDoc.SelectNodes("achivements/achivement")) {
			Achievement achivment = new Achievement();
			achivment.name = node.SelectSingleNode("name").InnerText;
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
			achivment.isMultiplie = bool.Parse(node.SelectSingleNode("multiplie").InnerText);
			//Debug.Log(open);
			if(!open){
				ongoingAchivment.Add(achivment);
			}else{

                if (achivment.isMultiplie)
                {
					bool ready = bool.Parse(node.SelectSingleNode("ready").InnerText);
					if(ready){
						ongoingAchivment.Add(achivment);
					}else{
						finishedAchivment.Add(achivment);
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
								foreach (Achievement achiv in ongoingAchivment) {
                               
										if (achiv.achivParams.ContainsKey (mess.param)) {
					
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
								if (obj.CheckDone ()) {
										outcomeQueue.Enqueue (obj);
										
								}
						});
					
						ongoingAchivment.RemoveAll (delegate(Achievement achv) {
								
								return achv.isDone;
						});
						//	
					
						Thread.Sleep (1000);
					}catch( Exception e){
					
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
	void Update(){
		List<int> syncAchivment = new List<int>();
		while (outcomeQueue.Count>0) {
			Achievement finished = outcomeQueue.Dequeue();
			myPlayer.AchivmenUnlock(finished);
			finishedAchivment.Add(finished);
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

	public void EventPawnDeadByPlayer(Player target,string weapon_id){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_DEATH;
		
			incomeQueue.Enqueue(mess);
			mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_DEATH+"by"+weapon_id;
		
			incomeQueue.Enqueue(mess);


		}
	}
	public void EventPawnDeadByAI(Player target){

		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_DEATH_AI;
		
			incomeQueue.Enqueue(mess);
		}
	}
	public void EventPawnKillPlayer(Player target,string weapon_id){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_KILL;
			incomeQueue.Enqueue(mess);
			mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_KILL+"by"+weapon_id;
		
			incomeQueue.Enqueue(mess);
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
	public void EventKilledAFriend(Player target,Player friend,string weapon_id){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_KILL_FRIEND;
			incomeQueue.Enqueue(mess);
		}
	}

	public void EventPawnKillAI(Player target,string weapon_id){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_KILL_AI;
			incomeQueue.Enqueue(mess);
			mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_KILL_AI+"by"+weapon_id;
		
			incomeQueue.Enqueue(mess);
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
	public void EventJuggerKill(Player target,string weapon_id){
	
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
}