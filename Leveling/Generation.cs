using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class Generation : MonoBehaviour {
    public Part[] Parts;
	public Transform startTransform;
	public List<Part> Rooms;

	public int RoomsCount;
	public bool TestGenerator;

	[HideInInspector]
    public Transform PathEngine;
	[HideInInspector]
	public Part[] LeftTurnParts;
	[HideInInspector]
	public Part[] RightTurnParts;

	public int Cache;
	public int[] PartCacheBase;
	public Queue<int[]> PartCache = new Queue<int[]> ();
	public bool isCreation = false;
	public Queue<Thread> PartCreation = new Queue<Thread> ();
	
    public void MovePathTo(Transform transform)
    {
        PathEngine.position = transform.position;
        PathfindingEngine.Instance.GenerateStaticMap();
    }
	void CacheBaseLoad()
	{
		PartCacheBase = new int[Parts.Length];
		for (int i = 0; i < Parts.Length; i++) {
			PartCacheBase[i] = Parts[i].GetCache();
		}
	}
	void CacheLoad()
	{
		int[] NewCache = new int[PartCacheBase.Length];

		for (int i = 0; i < PartCacheBase.Length; i++) {
			NewCache[i] = PartCacheBase[i];
			if (Parts[i].Difficult == DIFFICULT.EASY) NewCache[i] += Cache;
			else  if (Parts[i].Difficult == DIFFICULT.HARD) NewCache[i] += -Cache;
		}
		Debug.Log ("CacheLoad");
		PartCache.Enqueue (NewCache);
	}

	int LoadPartIndexAtCache(int Cache)
	{
		int[] NewCache = PartCache.Peek();
//		if (Cache <= 100)Debug.Log (Cache);
//		Debug.Log (Cache);
		for (int i = 0, dCache = 0; i < Parts.Length; i++) {
			dCache += NewCache[i];
			if(Cache <= dCache) return i;
		}
		Debug.Log ("LoadPartIndexAtCache Error");
		return 0;
	}

	void AddRoomInCache (Part NewRoom)
	{
		if (NewRoom.Difficult == DIFFICULT.EASY) Cache -= 10;
		else if (NewRoom.Difficult == DIFFICULT.HARD) Cache += 10;
	}

	public void Next(int Count)
	{
		for (int i = 0; i < Count; i++) {
			Next();	
		}
	}

	public void Next()
	{
		Thread RoomCreation = new Thread(CacheLoad);
		PartCreation.Enqueue (RoomCreation);
	}

	int GetNextRoomIndex(int[] Cache)
	{
		int FullCache = 0;
		foreach (int Num in Cache)
			FullCache += Num;
		int Index = UnityEngine.Random.Range (0, FullCache) + 1;
		return LoadPartIndexAtCache (Index);
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
	}
    void Start()
	{
		CacheBaseLoad ();
		if (TestGenerator) TestRoomNext (1000);
		Next (10);
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
			PartCreation.Peek().Start();
			PartCreation.Dequeue();
			isCreation = true;
		}
		if (PartCache.Count != 0) {
			int Index = GetNextRoomIndex(PartCache.Peek());
			if(!TestGenerator) RoomCreate(Parts[Index]);
			else TestRoomUp(Rooms[Index]);
			PartCache.Dequeue();
		}
	}
}