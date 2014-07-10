using UnityEngine;
using System;
using System.Collections.Generic;

public enum PARTDIRECTION
{
    FORWARD,
    RIGHT,
    LEFT
}

public class Part : MonoBehaviour
{
	public PARTDIRECTION type;
	public Transform Enter;
	public Transform[] Exit;
	public PreSpawner Spawner;

	[HideInInspector]
	public bool Entered = false;
	[HideInInspector]
    public Generation generator;
	[HideInInspector]
	public int Numb;
	[HideInInspector]
	public Transform PartTransform;
	[HideInInspector]
	public List<Part> ConnectedParts;

    protected void Awake()
    {
        PartTransform = transform;
    }

	public void ConnectToPart(Part OldPart)
	{
		int FreeExit = OldPart.Exit.Length - OldPart.ConnectedParts.Count - 1;
		PartTransform.rotation = Quaternion.FromToRotation(OldPart.Enter.forward, OldPart.Exit[FreeExit].forward) * OldPart.PartTransform.rotation;
		PartTransform.position = OldPart.Exit[FreeExit].position + PartTransform.position - Enter.position;
        StaticBatchingUtility.Combine(gameObject);
		OldPart.ConnectedParts.Add(this);
	}
    public virtual void Started()
    {
		Spawner.Spawn();
    }
    public virtual void PlayerEnter()
    {
        generator.Next();
        
        ((RunnerGameRule)GameRule.instance).NextRoom();
        ((AIDirector_Runner)AIDirector.instance).NextBlock();
    }
    public virtual void DestroyRoom(){
        Spawner.Destroy();
        Destroy(gameObject);
    }
	public virtual void DestroyConnectedRoom(){
		foreach (Part part in ConnectedParts)
		{
			part.DestroyConnectedRoom();
			part.DestroyRoom();
		}
	}
}
[Serializable]
public class PreSpawner
{
	public GameObject[] Prefabs;
	public Transform[] SpawnPoints;
    public List<TransformPrefab> SpawnedPrefabs = new List<TransformPrefab>();
	
	public void Spawn(){
		for (int i = 0; i < SpawnPoints.Length; i++) {
			GameObject RandomPrefab = Prefabs[UnityEngine.Random.Range(0, Prefabs.Length)];
            Transform RandomPrefabSpawned = PhotonNetwork.InstantiateSceneObject(RandomPrefab.name, SpawnPoints[i].position, SpawnPoints[i].rotation, 0, null).transform;
			SpawnedPrefabs.Add(new TransformPrefab(RandomPrefab, RandomPrefabSpawned));
		}
	}
    public void Destroy()
    {
        foreach (TransformPrefab pref in SpawnedPrefabs)
        {
            pref.GameObj.GetComponent<DestroyableNetworkObject>().StartCoroutineRequestKillMe();
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
