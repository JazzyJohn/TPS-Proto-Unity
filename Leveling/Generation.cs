using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class Generation : MonoBehaviour {
    public Part[] Parts;
	public Transform startTransform;
	public List<Part> Rooms;
    public bool onStart=false;
	int RoomsCount;
    
	[HideInInspector]
    public Transform PathEngine;
	[HideInInspector]
	public Part[] LeftTurnParts;
	[HideInInspector]
	public Part[] RightTurnParts;

    System.Random rand = new System.Random();

	public int Cache;
	public int[] PartCacheBase;
	public List<int[]> PartCache = new List<int[]>();
	public List<int> IndexPartToSpawn = new List<int>(); 
	
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
	void CacheLoad()
	{
		Debug.Log ("CacheLoad");
		int[] NewCache = new int[PartCacheBase.Length];
		int HARD = Cache, EASY = Cache;

		if (Cache < 0) HARD = Math.Abs (Cache);
		else EASY = Math.Abs (Cache);

		for (int i = 0; i < PartCacheBase.Length; i++) {
			if (Parts[i].Difficult == DIFFICULT.EASY) NewCache[i] = PartCacheBase[i] + EASY;
			else NewCache[i] = PartCacheBase[i] + HARD;
		}
		PartCache.Add (NewCache);
	}

	int LoadPartIndexAtCache(int Cache)
	{
		Debug.Log ("LoadPartIndexAtCache");
        int[]  PartCacheTemp = PartCache[0];
        PartCache.RemoveAt(0);
		for (int i = 0, dCache = 0; i < Parts.Length; i++) {
            dCache += PartCacheTemp[i];
			if(Cache <= dCache)return i;
		}
		Debug.Log ("LoadPartIndexAtCache Error");
		return 0;
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
		RoomCreation.Start ();
	}

	void NextAtCache()
	{
		if (PartCache.Count == 0) {
			Debug.Log ("NextAtCache Error");
			return;
		}
		Debug.Log ("NextAtCache");
		int FullCache = 0;
		foreach (int Num in PartCache[0])
			FullCache += Num;

   
        IndexPartToSpawn.Add(LoadPartIndexAtCache(rand.Next(FullCache + 1)));
		//PartCache.RemoveAt (0);
	}

	void RemovFirstPart()
	{
		Rooms[0].DestroyRoom();
		Rooms.RemoveAt(0);
	}
	void RoomCreate()
	{
		if (IndexPartToSpawn.Count == 0) {
			Debug.Log ("RoomCreate Error");
			return;
		}
		Part NewPart = (Part)Instantiate(Parts[IndexPartToSpawn[0]]);
		IndexPartToSpawn.RemoveAt (0);
		Debug.Log ("RoomCreate");
		NewPart.Numb = RoomsCount;

        if (Rooms.Count != 0) NewPart.ConnectToPart(Rooms[Rooms.Count - 1], this);
        else
        {
            NewPart.PartTransform.position = startTransform.position;
            NewPart.PartTransform.rotation = startTransform.rotation;

        }
		
		Rooms.Add(NewPart);
		
		NewPart.Started();
		RoomsCount++;

		if (NewPart.Difficult == DIFFICULT.EASY) Cache--;
		else if (NewPart.Difficult == DIFFICULT.HARD) Cache++;
	}
    void Start()
	{
        if (onStart)
        {
            CacheBaseLoad();
            Next(10);
        }
      
	}

	void Update()
	{
		if (PartCache.Count != 0) {
			for (int i = 0; i < PartCache.Count; i++) {
				NextAtCache();
			}
		}
		if (IndexPartToSpawn.Count != 0) {
			for (int i = 0; i < IndexPartToSpawn.Count; i++) {
				RoomCreate();
			}
		}
	}
}