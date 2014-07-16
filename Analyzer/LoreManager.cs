using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class LoreBlock{
	public int blockId;
	
	public string openName;
	
	public int needToOpen;
	
	public int alreadyAnalyzed;
	
	public int pointModifier;
	
	public string text = "";
}

public class LoreEntry{
	public List<int> openBlockId;
	
	public int entryID;
	
	public string guiIconWeb;
	
	public Texture guiIcon;
	
	public string name;
	
	public LoreBlock[] allBlock;
}
	[Serializable]
	public class AnalyzeEntry{
		public string name;
		
		public int point = 0;
		public AnalyzeEntry(){
		
		}
		
		public AnalyzeEntry(string name,int point){
			this.name = name;
			this.point = point;
		}
	}

public class LoreManager : MonoBehaviour{
	private struct WebSendData{
		public int blockId;

        public int addPoint;
	
		public WebSendData(int blockId, int addPoint){
			this.blockId = blockId;
			this.addPoint = addPoint;
		}
	}


	ConcurrentQueue<AnalyzeEntry> incomeQueue = new ConcurrentQueue<AnalyzeEntry>();

	ConcurrentQueue<AnalyzeEntry> outcomeQueue = new ConcurrentQueue<AnalyzeEntry>();
	
	ConcurrentQueue<int> openedQueue = new ConcurrentQueue<int>();
	
	ConcurrentQueue<WebSendData> sendQueue = new ConcurrentQueue<WebSendData>();
	
	private Thread myThread;

	public LoreEntry[] allEntry;
		
	private static LoreManager s_Instance = null;

    private string UID;
	
	public static LoreManager instance {
		get {
			if (s_Instance == null) {
				//Debug.Log ("FIND");
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (LoreManager)) as LoreManager;
			}

		
			// If it is still null, create a new instance
			if (s_Instance == null) {
			//	Debug.Log ("CREATE");
				GameObject obj = new GameObject("LoreManager");
				s_Instance = obj.AddComponent(typeof (LoreManager)) as LoreManager;
				
			}
			
			return s_Instance;
		}
	}
	//Add point from Analizer
	public void AddAnalyzePoint(string name,int point){
		incomeQueue.Enqueue(new AnalyzeEntry(name,point));
		
	}
	
	public void LoreLoop(){
		while (true) {
			while (incomeQueue.Count>0) {
				AnalyzeEntry income = incomeQueue.Dequeue ();
				AnalyzeEntry outcome = new AnalyzeEntry();
				outcome.name  = income.name;
				for(int i = 0; i<	allEntry.Length;i++){
					
					LoreEntry entry = allEntry[i];
                    if (entry.openBlockId.Count == entry.allBlock.Length)
                    {
						continue;
					}
                    for (int j = 0; j < entry.allBlock.Length; j++)
                    {
						if(entry.allBlock[j].needToOpen<=entry.allBlock[j].alreadyAnalyzed){
							continue;
						}
						if(entry.allBlock[j].openName == income.name){
							int addPoint= income.point*entry.allBlock[j].pointModifier;
							entry.allBlock[j].alreadyAnalyzed +=addPoint;
							outcome.point += addPoint;
							sendQueue.Enqueue(new WebSendData(entry.allBlock[j].blockId,addPoint));
							
							if(entry.allBlock[j].needToOpen<=entry.allBlock[j].alreadyAnalyzed){
								entry.openBlockId.Add(j);
								openedQueue.Enqueue(i);
							}
						}
					
					}
				}
				outcomeQueue.Enqueue(outcome);
			
			}
		
			Thread.Sleep (1000);
		}
	}
	
	public void Update(){
		while (outcomeQueue.Count>0) {
			AnalyzeEntry outcome = outcomeQueue.Dequeue ();
			PlayerMainGui.instance.AddMessage(outcome.point.ToString(),PlayerMainGui.MessageType.ANALIZE_POINT);
	
		}
		while (openedQueue.Count>0) {
			int index = openedQueue.Dequeue ();
			PlayerMainGui.instance.AddMessage(allEntry[index].name,allEntry[index].guiIcon,PlayerMainGui.MessageType.OPEN_LORE);
	
		}
		if(sendQueue.Count>0){
			StartCoroutine(UpdateWeb());
		}
	}
	
	public IEnumerator UpdateWeb(){
		var form = new WWWForm ();
			
		form.AddField ("uid", UID);
		while (sendQueue.Count>0) {
			WebSendData data = sendQueue.Dequeue ();
			form.AddField ("index[]", data.blockId);
            form.AddField("amount[]", data.addPoint);
		}
		WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.UPDATE_LORE);
		yield return w;
	}
	
	
	public void Init(string uid){
		DontDestroyOnLoad(transform.gameObject);
		
		 
		UID = uid;
		StartCoroutine(LoadLore ());
	}

    public IEnumerator LoadLore()
    {
		WWWForm form = new WWWForm ();
			
		form.AddField ("uid", UID);
		WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.LOAD_LORE);
       yield return w;
		ParseList (w.text);

		
		myThread = new Thread(new ThreadStart(this.LoreLoop));
		myThread.Start ();		
    }
	protected void ParseList(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);

		
		XmlNodeList loreentrys = xmlDoc.SelectNodes("loreData/loreentry");
		allEntry = new LoreEntry[loreentrys.Count];
		for(int j=0;j<loreentrys.Count;j++){
            XmlNode node = loreentrys[j];
			LoreEntry entry   =new LoreEntry();
			allEntry[j] = entry;
			entry.entryID = int.Parse (node.SelectSingleNode ("id").InnerText);
			entry.guiIconWeb = node.SelectSingleNode ("guiIconWeb").InnerText;
			entry.name = node.SelectSingleNode ("name").InnerText;
			
			XmlNodeList loreblocks = xmlDoc.SelectNodes("loreblock");
			entry.allBlock  = new LoreBlock[loreblocks.Count];
			for(int i=0;i<loreblocks.Count;i++){
				LoreBlock block = new LoreBlock();
                XmlNode nodeBlock = loreblocks[i];
				entry.allBlock[i] =block;
                block.blockId = int.Parse(nodeBlock.SelectSingleNode("blockId").InnerText);
                block.openName = nodeBlock.SelectSingleNode("openName").InnerText;
                block.needToOpen = int.Parse(nodeBlock.SelectSingleNode("needToOpen").InnerText);
                block.pointModifier = int.Parse(nodeBlock.SelectSingleNode("pointModifier").InnerText);
                block.alreadyAnalyzed = int.Parse(nodeBlock.SelectSingleNode("alreadyAnalyzed").InnerText);
				if(block.alreadyAnalyzed >=	block.needToOpen){
					entry.openBlockId.Add(i);
				}
			}
		}
	
	}
}