using UnityEngine;
using System;
using System.Collections.Generic;

public class Generation : MonoBehaviour {
	public GameObject[] Parts;
	public List<Part> Rooms;
	public Part s;
	int RoomsCount;

	public void Next()
	{
		RoomsCount++;
		Part NewPart = Parts[UnityEngine.Random.Range(0, Parts.Length)].GetComponent<Part>();
		NewPart.Numb = RoomsCount;
		if(Rooms.Count != 0) NewPart.ConnectToPart(Rooms[Rooms.Count - 1]);
		Rooms.Add(NewPart);
	}
	void Ready()
	{
		for (int i = 0; i < Rooms.Count; i++) {
			Rooms[i].PartTransform.BroadcastMessage("Started");
		}
	}
	public void Next(int Count)
	{
		for (int i = 0; i < Count; i++) {
			Next();
		}
	}
}
