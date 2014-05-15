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
	public bool isDone = false;
	public override string ToString(){
		return name + " " + description + " " + achievementId + " " + achivParams.ToStringFull ();
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

	struct IncomingMessage{
		public string param;
		public float delta;

	}
		
	ConcurrentQueue<IncomingMessage> incomeQueue = new ConcurrentQueue<IncomingMessage>();


	ConcurrentQueue<Achievement> outcomeQueue = new ConcurrentQueue<Achievement>();
	//initilization

	void Awake(){
		EventHolder.instance.Bind (this);

	}
	protected IEnumerator LoadAchivment(WWWForm form){
				Debug.Log (form );
				WWW w = null;
				if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
					
					Debug.Log ("STATS HTTP SEND");
					w = new WWW (StatisticHandler.STATISTIC_PHP + StatisticHandler.LOAD_ACHIVE, form);
				}
				else{
					Debug.Log ("STATS HTTPS SEND");
					w = new WWW (StatisticHandler.STATISTIC_PHP_HTTPS + StatisticHandler.LOAD_ACHIVE, form);
				}
				yield return w;
				ParseList (w.text);
	}
	//parse XML string to normal Achivment Pattern
	protected void ParseList(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		ongoingAchivment= new List<Achievement>();
		onFinishedAchivment= new List<Achievement>();
		foreach (XmlNode node in xmlDoc.SelectNodes("achivements/achivement")) {
			Achievement achivment = new Achievement();
			achivment.name = node.SelectSingleNode("name").InnerText;
			achivment.description = node.SelectSingleNode("description").InnerText;

			achivment.achievementId = int.Parse(node.SelectSingleNode("id").InnerText);
			foreach (XmlNode paramNode in node.SelectNodes("param")) {
				AchievementParam param = new AchievementParam();
				param.current =0.0f;
				param.needed = float.Parse(paramNode.SelectSingleNode("value").InnerText);
				achivment.achivParams.Add(paramNode.SelectSingleNode("name").InnerText,param);
			}
//			Debug.Log("ACHIVMENT " +achivment);
		
			ongoingAchivment.Add(achivment);

		}
		//loots of counting arrays, dictionary, logic don't want to drop fps by this)
		 myThread = new Thread(new ThreadStart(this.AchivmentLoop));
		 myThread.Start ();		
	}

	//Achivment Hondler
	private List<Achievement> ongoingAchivment;

	private List<Achievement> onFinishedAchivment;

	private Thread myThread;

	private void AchivmentLoop(){
		while (true) {
			Debug.Log (incomeQueue.Count	);
						while (incomeQueue.Count>0) {
								IncomingMessage mess = incomeQueue.Dequeue ();
								foreach (Achievement achiv in ongoingAchivment) {
										if (achiv.achivParams.ContainsKey (mess.param)) {
						//Debug.Log (achiv.ToString());
												achiv.achivParams [mess.param].current += mess.delta;
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

						Thread.Sleep (1000);
				}
	}
	
	void OnDestroy(){
		myThread.Abort ();
		
	}
	void Update(){
		while (outcomeQueue.Count>0) {
			Achievement finished = outcomeQueue.Dequeue();
			myPlayer.AchivmenUnlock(finished);
			onFinishedAchivment.Add(finished);
		}
		//Debug.Log (onFinishedAchivment.Count);
	}
	public List<Achievement> GetAchivment (){
	
		return onFinishedAchivment;
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
	public Vector3 wallRunningStartPosition;

	public void EventAppear(Player target){
		if (target.GetView ().isMine) {
			myPlayer = target;
			var form = new WWWForm ();
			
			form.AddField ("uid", myPlayer.UID);
		
			StartCoroutine(LoadAchivment (form));

		}
	}

	public void EventPawnDeadByPlayer(Player target){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_DEATH;
		
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
	public void EventPawnKillPlayer(Player target){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_KILL;
			incomeQueue.Enqueue(mess);
		}
	}
	public void EventPawnKillAI(Player target){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=1.0f;
			mess.param =PARAM_KILL_AI;
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
		}
	}
	public void EventStartSprintRun(Player target, Vector3 position){

	}
	public void EventEndSprintRun(Player target,Vector3 position){

	}
	public void EventEndWallRun(Player target, Vector3 position){
		if (target == myPlayer) {
			IncomingMessage mess = new IncomingMessage();
			mess.delta=(wallRunningStartPosition -position).magnitude;
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
}