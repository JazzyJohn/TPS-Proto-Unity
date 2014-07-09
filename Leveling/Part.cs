using UnityEngine;
using System;
using System.Collections.Generic;

public class Part : MonoBehaviour
{
	public Transform Enter;
	public Transform Exit;
	public PreSpawner Spawner;

    public Generation generator;
	[HideInInspector]
	public int Numb;
	[HideInInspector]
	public Transform PartTransform;

    protected void Awake()
    {
        PartTransform = transform;
    }

	public void ConnectToPart(Part OldPart,Generation generator)
	{
        PartTransform.rotation = Quaternion.FromToRotation(OldPart.Enter.forward, OldPart.Exit.forward) * OldPart.PartTransform.rotation;
		PartTransform.position = OldPart.Exit.position + PartTransform.position - Enter.position;
        StaticBatchingUtility.Combine(gameObject);
        this.generator = generator;
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
