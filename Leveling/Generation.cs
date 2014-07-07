using UnityEngine;
using System;
using System.Collections.Generic;

public class Generation : MonoBehaviour {
	public GameObject[] Parts;
	List<Part> Rooms;
//	public List<Vector3> OccupiedParts;
//	public Vector3 CurrentTrigEmptyPart;

	// Use this for initialization
	void Start () 
	{
		Next(5);
	}

	void CloseRoom(Part DeadRoom)
	{
		DeadRoom.Dead();
		Rooms.Remove(DeadRoom);
	}

	void Next()
	{
		Part NewPart = new Part(Parts[UnityEngine.Random.Range(0, Parts.Length)]);
		if(Rooms.Count != 0) NewPart.ConnectToPart(Rooms[Rooms.Count - 1]);
		Rooms.Add(NewPart);
	}
	void Next(int Count)
	{
		for (int i = 0; i < Count; i++) {
			Next();
		}
	}
//	public void OccupiedPart(Vector3 Point)
//	{
//		OccupiedParts.Add(Point);
//	}
//	public void OccupiedPart(Transform Part)
//	{
//		for (int i = 0; i < Part.childCount; i++) {
//			Vector3 Norm = PartTransformToNormal(Part.GetChild(i).transform.position);
//			if((Part.GetChild(i).name.Contains("TrigPart") && !isOccupied(Norm)) || (Part.GetChild(i).name.Contains("TrigEmpty") && CurrentTrigEmptyPart != Norm)) OccupiedParts.Add(Norm);
//			if(Part.GetChild(i).name == "TrigEmptyExit") CurrentTrigEmptyPart = Norm;
//		}
//	}
//	public bool isOccupied(Vector3 Point)
//	{
//		return OccupiedParts.Contains(Point);
//	}
//	public Transform InstantiateFreePart(Vector3 Point, Vector3 Direction)
//	{
//		for (int i = 2; i < Parts.Length; i++) {
//			Transform TestTransform = ((GameObject)Instantiate(Parts[Random.Range(i, Parts.Length)], Vector3.zero, Quaternion.identity)).transform;
//			TestTransform.rotation = Quaternion.FromToRotation(-TestTransform.FindChild("Enter").transform.forward, Direction);
//			TestTransform.position = Point + TestTransform.position - TestTransform.FindChild("Enter").position;
//			if (!isOccupied(TestTransform)) return TestTransform;
//			Destroy(TestTransform.gameObject);
//		}
//		return null;
//	}
//	Vector3 PartTransformToNormal(Vector3 Point)
//	{
//		return new Vector3(Point.x - (Point.x % 20f), Point.y - (Point.y % 20f), Point.z - (Point.z % 20f));
//	}
//	public bool isOccupied(Transform Part)
//	{
//		for (int i = 0; i < Part.childCount; i++) {
//			Vector3 Norm = PartTransformToNormal(Part.GetChild(i).transform.position);
//			if(Norm == CurrentTrigEmptyPart) continue;
//			string Name = Part.GetChild(i).name;
//			if((Name == "TrigEmptyExit" || Name.Contains("TrigPart")) && isOccupied(Norm)) return true;
//		}
//		return false;
//	}

//	 Update is called once per frame
	void Update () {

	}
}

public class Part : MonoBehaviour 
{
	public GameObject PartObject;
	public Transform PartTransform;
	public Transform Enter;
	public Transform Exit;

	public void Dead()
	{
		Destroy(PartTransform.gameObject);
	}
	public Part (GameObject GO)
	{
		PartObject = GO;
		GameObject NewPart = (GameObject)Instantiate(GO, Vector3.zero, Quaternion.identity);
		PartTransform = NewPart.transform;
		Enter = PartTransform.FindChild("Enter");
		Exit = PartTransform.FindChild("Exit");
	}
	public void ConnectToPart(Part OldPart)
	{
		PartTransform.rotation = Quaternion.FromToRotation(-Enter.forward, OldPart.Exit.forward);
		PartTransform.position = OldPart.Exit.position + PartTransform.position - Enter.position;
	}
}
