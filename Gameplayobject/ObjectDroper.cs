using UnityEngine;
using System.Collections;

public class ObjectDroper : MonoBehaviour {

	public GameObject[] objectList;
	void OnDestroy() {
		if (NetworkController.Instance.IsMaster()&&!GameRule.IsLvlChanging) {
			if(objectList.Length>0){
                NetworkController.Instance.SimplePrefabSpawn(objectList[(int)(UnityEngine.Random.value * objectList.Length)].name, transform.position, transform.rotation);
			}
			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
