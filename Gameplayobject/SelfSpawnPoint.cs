using UnityEngine;
using System.Collections;

public class SelfSpawnPoint : ObjectSpawnPoint {

	public GameObject[] prefabs;
	
	public void SpawObject(){
		if(prefabs.Length>0){
			SetIsAvalable(false);
			//spawnedObject =PhotonNetwork.InstantiateSceneObject(prefabs[(int)(UnityEngine.Random.value*prefabs.Length)].name,transform.position,transform.rotation,0,null) as GameObject;
            spawnedObject = NetworkController.Instance.SimplePrefabSpawn(prefabs[(int)(UnityEngine.Random.value * prefabs.Length)].name, transform.position, transform.rotation);
		}
			
	}
    public void OnDestroy()
    {
        if (spawnedObject != null)
        {
            spawnedObject.GetComponent<DestroyableNetworkObject>().StartCoroutineRequestKillMe();
        }
    }
}
