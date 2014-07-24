using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;




public static class ThreadRandomGen
{
	private static Random _global = new Random();
	[ThreadStatic]
	private static Random _local;

	public static int Next(int max)
	{
		Random inst = _local;
		if (inst == null)
		{
			int seed;
			lock (_global) seed = _global.Next();
			_local = inst = new Random(seed);
		}
		return inst.Next(int max);
	}
}
public enum BIOMS{CITY,DESERT,SWAMP,FOREST};
//This is some sort of Dictionary for generator game rule and Ai director fill it with info 
//about course of generation. It's needed cause Generator is multithread and we couldn't use direct access
// to director or gamerule
public struct GeneratorCondition{
	public BIOMS currentBiom;
	
}
public class Generation : MonoBehaviour {
    public Part[] Parts;
	public Transform startTransform;
	public List<Part> Rooms;

    public bool onStart=false;
	int RoomsCount;
    public int RoomCache;

	public bool TestGenerator;

	[HideInInspector]
    public Transform PathEngine;
	[HideInInspector]
	public Part[] LeftTurnParts;
	[HideInInspector]
	public Part[] RightTurnParts;

	public volatile GeneratorCondition condition= GeneratorCondition();
	

	public volatile int Cache;
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
		int[] NewCache = new int[PartCacheBase.Length];
		FullCache = 0;
		for (int i = 0; i < PartCacheBase.Length; i++) {
			NewCache[i] = PartCacheBase[i];
			if (Parts[i].Difficult == DIFFICULT.EASY) NewCache[i] += Cache;
			else  if (Parts[i].Difficult == DIFFICULT.HARD) NewCache[i] += -Cache;
			FullCache+=	NewCache[i];
			
		}
		//Debug.Log ("CacheLoad");
		
		PartCache.Enqueue (	GetNextRoomIndex(NewCache,FullCache));
	}

	int LoadPartIndexAtCache(int[] weights, int needWeight)
	{

		
//		if (Cache <= 100)Debug.Log (Cache);
//		Debug.Log (Cache);
		for (int i = 0, dCache = 0; i < weights.Length; i++) {
			dCache += weights[i];
			if(needWeight <= dCache) return i;

		}
		Debug.Log ("LoadPartIndexAtCache Error");
		return 0;
	}
	
	int GetNextRoomIndex(int[] weights,int fullWeights)
	{
		

		int Index =ThreadRandomGen.Next( fullWeights-1) + 1;
		return LoadPartIndexAtCache (weights,Index);

	}
	
	//END Multi Thread Section
	void AddRoomInCache (Part NewRoom)
	{
		if (NewRoom.Difficult == DIFFICULT.EASY) Cache -= 10;
		else if (NewRoom.Difficult == DIFFICULT.HARD) Cache += 10;
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
		RoomsCount++;

		AddRoomInCache (NewPart);

		isCreation = false;

        if (RoomCache < Rooms.Count)
        {
            RemovFirstPart();
        }
    }
    void Start()
	{
        if (onStart)
        {
			CacheBaseLoad ();
			if (TestGenerator) TestRoomNext (1000);
			Next (10);
        }
      

	
	}
  
	void TestRoomNext(int Count)
	{
		for (int i = 0; i < Parts.Length; i++) {
			RoomCreate(Parts[i]);
		}
		for (int i = 0; i < Count; i++) {
			Next();
		}
	}
	void TestRoomUp(Part TestRoom)
	{
		Vector3 NewPosition = TestRoom.PartTransform.position;
		NewPosition.y += 0.01f;
		TestRoom.PartTransform.position = NewPosition;
		AddRoomInCache (TestRoom);
		isCreation = false;
	}
	void Update()
	{
		if (!isCreation && PartCreation.Count != 0 && PartCache.Count == 0) {
		
		}
		if (PartCache.Count != 0) {
			int Index = PartCache.Peek();
			if(!TestGenerator) RoomCreate(Parts[Index]);
			else TestRoomUp(Rooms[Index]);
			
		}
	}
	void StartNextThread(){
		PartCreation.Dequeue().Start();
		
		isCreation = true;
	}
}