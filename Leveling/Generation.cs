using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;




public static class ThreadRandomGen
{
	private static System.Random _global = new System.Random();
	[ThreadStatic]
	private static System.Random _local;

	public static int Next(int max)
	{
		System.Random inst = _local;
		if (inst == null)
		{
			int seed;
			lock (_global) seed = _global.Next();
			_local = inst = new System.Random(seed);
		}
		return inst.Next(max);
	}
}
public enum DIFFICULT
{
	HARD,
	MEDIUM,
	EASY
}
public enum BIOMS{
	CITY,
	DESERT,
	SWAMP,
	FOREST
};
public enum ROOMTYPE{
	AI,
	ACROBATIC,
	STEALTH,
	RIDDLE
	
}
public enum ROOMSUBTYPE{
	NORMAL,
	AIBOSS
}
//This is some sort of Dictionary for generator game rule and Ai director fill it with info 
//about course of generation. It's needed cause Generator is multithread and we couldn't use direct access
// to director or gamerule
public class GeneratorCondition{
	public volatile BIOMS currentBiom;
    public volatile Dictionary<ROOMTYPE, int> byTypeWeight;
}
public class Generation : MonoBehaviour {
    public Part[] Parts;
	public Transform startTransform;
	public List<Part> Rooms;

	public Transform[] TestCubic;
    public bool onStart=false;
    public int RoomsCount;
    public int RoomCache;

	public bool TestGenerator;

	[HideInInspector]
    public Transform PathEngine;
	[HideInInspector]
	public Part[] LeftTurnParts;
	[HideInInspector]
	public Part[] RightTurnParts;

	public GeneratorCondition condition=  new GeneratorCondition();

    public string TExt;
	

	private volatile int DificultyCache=0;
	private volatile int normalRoomCount=0;
	public int[] PartCacheBase;
	public volatile ConcurrentQueue<int> PartCache = new ConcurrentQueue<int> ();
	public bool isCreation = false;
	public volatile ConcurrentQueue<Thread> PartCreation = new ConcurrentQueue<Thread> ();
	
    public void MovePathTo(Transform transform)
    {
        PathEngine.position = transform.position;
        PathfindingEngine.Instance.GenerateStaticMap();
    }
	public void CacheBaseLoad()
	{
		PartCacheBase = new int[Parts.Length];
		for (int i = 0; i < Parts.Length; i++) {
			PartCacheBase[i] = Parts[i].GetCache();
		}
	}
	//Multi Thread Section
	//THIS function  involved in multi thread Weight count for room
	// so any operation must do with caution;
	
	void CacheLoad()
	{
        //Debug.Log("CacheLoad");
		int[] NewCache = new int[PartCacheBase.Length];
		int FullCache = 0;
		for (int i = 0; i < PartCacheBase.Length; i++) {
			NewCache[i] = PartCacheBase[i];
			if(Parts[i].biom != condition.currentBiom){
				NewCache[i] = 0;
				continue;
			}
			if(!condition.byTypeWeight.ContainsKey(Parts[i].roomType)){
				NewCache[i] = 0;
				continue;
			}
			NewCache[i]+=condition.byTypeWeight[Parts[i].roomType];
			if (Parts[i].Difficult == DIFFICULT.EASY) NewCache[i] +=DificultyCache;
			else  if (Parts[i].Difficult == DIFFICULT.HARD) NewCache[i] += -DificultyCache;
			
			if(Parts[i].subType !=ROOMSUBTYPE.NORMAL){
				NewCache[i] *= (int)(Parts[i].subTypeMultiplier * normalRoomCount);
			}
			
			
			FullCache+=	NewCache[i];
		}
       // Debug.Log("FullCache" + FullCache);
		
		PartCache.Enqueue (	GetNextRoomIndex(NewCache,FullCache));
	}

	int LoadPartIndexAtCache(int[] weights, int needWeight)
	{

		
//		if (Cache <= 100)Debug.Log (Cache);
      //  Debug.Log(needWeight);
		for (int i = 0, dCache = 0; i < weights.Length; i++) {
			dCache += weights[i];
			if(needWeight <= dCache) return i;

		}
		//Debug.Log ("LoadPartIndexAtCache Error");
		return 0;
	}
	
	int GetNextRoomIndex(int[] weights,int fullWeights)
	{

      //  Debug.Log("Random TRy" );
		int Index =ThreadRandomGen.Next( fullWeights-1) + 1;
      //  Debug.Log("needed Index" + Index);
		return LoadPartIndexAtCache (weights,Index);

	}
	
	//END Multi Thread Section
	void AddRoomInCache (Part NewRoom)
	{
        RoomsCount++;
		if (NewRoom.Difficult == DIFFICULT.EASY)DificultyCache -= 10;
		else if (NewRoom.Difficult == DIFFICULT.HARD)DificultyCache += 10;
		
		if(NewRoom.subType==ROOMSUBTYPE.NORMAL){
			normalRoomCount++;
		}else{
			normalRoomCount =0;
		}
	}

	public void Next(int Count)
	{  
        RoomCache = (int)(Count*1.5);
		for (int i = 0; i < Count; i++) {
			Next();	
		}
	}

	public void Next()
	{
		Thread RoomCreation = new Thread(CacheLoad);
		PartCreation.Enqueue (RoomCreation);
	}

	

	void RemoveFirstRoom()
	{
		Rooms[0].DestroyRoom();
		Rooms.RemoveAt(0);
	}
	void RoomCreate(Part NewPart)
	{
		Part NewRoom = (Part)Instantiate(NewPart);

		if (Rooms.Count != 0) NewRoom.ConnectToPart(Rooms[Rooms.Count - 1], this);
		else NewRoom.PartTransform.position = startTransform.position;
		
		Rooms.Add(NewRoom);
		NewRoom.Started();
		NewRoom.Numb = RoomsCount;
		

		AddRoomInCache (NewPart);

		isCreation = false;

        if (RoomCache < Rooms.Count)
        {
            RemoveFirstRoom();
        }
    }
    void Start()
	{
        if (onStart)
        {
			CacheBaseLoad ();
            if (TestGenerator)
            {
                TestRoomNext(1000);
            }
            else
            {
                Next(10);
            }
        }
      

	
	}
  
	void TestRoomNext(int Count)
	{
		for (int i = 0; i < Count; i++) {
			Next();
		}
	}
	void TestRoomUp(Transform TestRoom)
	{
		//TestRoom.PartTransform.position.y += 0.01f;
		TestRoom.localScale= TestRoom.localScale +Vector3.up*0.01f;
	
		isCreation = false;
	}
	void Update()
	{
		if (!isCreation && PartCreation.Count != 0 && PartCache.Count == 0) {
           // Debug.Log("startThread");
			StartNextThread();
		}
		if (PartCache.Count != 0) {
            int Index = PartCache.Dequeue();
           // Debug.Log("INDEX" + Index);
            TExt += " " + Index;
            if (!TestGenerator)
            {
                RoomCreate(Parts[Index]);

            }
            else
            {
                AddRoomInCache(Parts[Index]);
            }
			if(TestCubic.Length>0){
				TestRoomUp(TestCubic[Index]);
			}
			
		}
        if (RoomsCount == 1000 && TestGenerator)
        {
           // Debug.Log(TExt);
           // Debug.Log(DificultyCache);
        }
	}
	void StartNextThread(){
		
        ((RunnerGameRule)GameRule.instance).ChangeCondition(condition, RoomsCount);
        PartCreation.Dequeue().Start();
		isCreation = true;
	}
}