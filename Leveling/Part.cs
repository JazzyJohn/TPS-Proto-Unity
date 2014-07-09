using UnityEngine;
using System;
using System.Collections.Generic;

public class Part : MonoBehaviour
{
	public GameObject PartObject;
	private Transform Enter;
	public Transform Exit;
	public PreSpawner Spawner;

	[HideInInspector]
	public int Numb;
	[HideInInspector]
	public Transform PartTransform;

	public void ConnectToPart(Part OldPart)
	{
		PartTransform.rotation = Quaternion.FromToRotation(-Enter.forward, OldPart.Exit.forward);
		PartTransform.position = OldPart.Exit.position + PartTransform.position - Enter.position;
	}
}
[Serializable]
public class PreSpawner
{
	public GameObject[] Prefabs;
	public Transform[] SpawnPoints;
	List<TransformPrefab> SpawnedPrefabs;
	
	public void Spawn(){
		for (int i = 0; i < SpawnPoints.Length; i++) {
			GameObject RandomPrefab = Prefabs[UnityEngine.Random.Range(0, Prefabs.Length)];
			Transform RandomPrefabSpawned = PhotonNetwork.InstantiateSceneObject(RandomPrefab.name, Vector3.zero, Quaternion.identity, 0, null).transform;
			SpawnedPrefabs.Add(new TransformPrefab(RandomPrefab, RandomPrefabSpawned));
		}
	}
}
public class TransformPrefab
{
	public GameObject Prefab;
	public Transform GameObj;
	public TransformPrefab(GameObject Prefab, Transform GameObj)
	{
		this.Prefab = Prefab;
		this.GameObj = GameObj;
	}
}
