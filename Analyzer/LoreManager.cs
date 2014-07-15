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



	ConcurrentQueue<AnalyzeEntry> incomeQueue = new ConcurrentQueue<AnalyzeEntry>();

	ConcurrentQueue<AnalyzeEntry> outcomeQueue = new ConcurrentQueue<AnalyzeEntry>();
	
	ConcurrentQueue<int> openedQueue = new ConcurrentQueue<int>();
	
	private Thread myThread;

	public LoreEntry[] allEntry;
		
	private static LoreManager s_Instance = null;
	
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
				for(int i = 0; i<	allEntry.Lenght;i++){
					
					LoreEntry entry = allEntry[i];
					if(entry.openBlockId.Count==entry.allBlock.Lenght){
						continue;
					}
					for(int j =0;j<entry.allBlock.Lenght;j++){
						if(entry.allBlock[j].needToOpen<=entry.allBlock[j].alreadyAnalyzed){
							continue;
						}
						if(entry.allBlock[j].openName == income.name){
							entry.allBlock[j].alreadyAnalyzed +=income.point*entry.allBlock[j].pointModifier;
							outcome.point = income.point*entry.allBlock[j].pointModifier;
							if(entry.allBlock[j].needToOpen<=entry.allBlock[j].alreadyAnalyzed){
								entry.openBlockId.Add(entry.allBlock[j].blockId);
								openedQueue.Enqueue(i);
							}
						}
					
					}
				}
				outcomeQueuel.Enqueue(outcome);
			
			}
		
			Thread.Sleep (1000);
		}
	}
	
	public void Update(){
		while (outcomeQueuel.Count>0) {
			AnalyzeEntry outcome = outcomeQueuel.Dequeue ();
			PlayerMainGui.instance.AddMessage(outcome.point.ToString(),PlayerMainGui.MessageType.ANALIZE_POINT);
	
		}
		while (openedQueue.Count>0) {
			int index = openedQueue.Dequeue ();
			PlayerMainGui.instance.AddMessage(allEntry[index].name,allEntry[index].guiIcon,PlayerMainGui.MessageType.OPEN_LORE);
	
		}
	}
	public void Init(){
		DontDestroyOnLoad(transform.gameObject);
		 myThread = new Thread(new ThreadStart(this.LoreLoop));
		 myThread.Start ();		
		 	WWWForm form = new WWWForm ();
			
		form.AddField ("uid", uid);
		UID = uid;
		StartCoroutine(LoadLore (form));
	}
}