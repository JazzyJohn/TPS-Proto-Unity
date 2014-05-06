using UnityEngine;
using System.Collections;

public class SelfSpawnPoint : ObjectSpawnPoint {

	public GameObjects[] prefabs;
	
	public void SpawObject(){
		if(prefabs.Length>0){
			isAvalable = false;
			spawnedObject =PhotonNetwork.Instantiate(prefabs[(int)(UnityEngine.Random.value*prefabs.Length)].name,transform.position,transform.rotation,0) as GameObject;
		}
			
	}
}
