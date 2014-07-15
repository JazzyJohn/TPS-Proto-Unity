using UnityEngine;
using System;
using System.Collections.Generic;

public class Generation : MonoBehaviour {
    public Part[] Parts;
    public Part[] LeftTurnParts;
    public Part[] RightTurnParts;
    public int lastTurn;
	public List<Part> Rooms;
	public Part s;
	int RoomsCount;
    public int RoomCache;
    public Transform startTransform;
    public Transform PathEngine;
	public void Next()
	{
		RoomsCount++;
        int type = UnityEngine.Random.Range(0, 3);
        Part NewPart = null;
        switch (type)
        {
            //ForwardRoom
            case 0:
                NewPart = Instantiate(Parts[UnityEngine.Random.Range(0, Parts.Length)]) as Part;
                break;
            //Right TurnRoom
            case 1:
                if (lastTurn >= 1 || RightTurnParts.Length == 0)
                {
                    NewPart = Instantiate( Parts[UnityEngine.Random.Range(0, Parts.Length)]) as Part;
                }
                else
                {
                    lastTurn++;
                    NewPart = Instantiate(RightTurnParts[UnityEngine.Random.Range(0, RightTurnParts.Length)]) as Part;
                }
                break;
            //LEft TurnRoom
            case 2:
                if (lastTurn <= -1 || LeftTurnParts.Length == 0)
                {
                    NewPart = Instantiate(Parts[UnityEngine.Random.Range(0, Parts.Length)]) as Part;
                }
                else
                {
                    lastTurn--;
                    NewPart = Instantiate(LeftTurnParts[UnityEngine.Random.Range(0, LeftTurnParts.Length)]) as Part;
                }
                break;
        }
		
     
		NewPart.Numb = RoomsCount;
        if (Rooms.Count != 0) NewPart.ConnectToPart(Rooms[Rooms.Count - 1],this);

        else
        {
            NewPart.PartTransform.position = startTransform.position;
            NewPart.PartTransform.rotation = startTransform.rotation;
        }

		Rooms.Add(NewPart);

        if (RoomCache<Rooms.Count)
        {
            Rooms[0].DestroyRoom();
            Rooms.RemoveAt(0);
        }
        NewPart.Started();
       
	}
	void Ready()
	{
		for (int i = 0; i < Rooms.Count; i++) {
            Rooms[i].Started();
		}
	}
	public void Next(int Count)
	{
        RoomCache = (int)Math.Round(Count * 1.5f);
		for (int i = 0; i < Count; i++) {
			Next();
		}
	}
    public void MovePathTo(Transform transform)
    {
        PathEngine.position = transform.position;
        PathfindingEngine.Instance.GenerateStaticMap();
    }
    
}
