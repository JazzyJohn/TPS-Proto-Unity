using UnityEngine;
using System.Collections;

public class ObjectDroper : MonoBehaviour {

	public GameObject[] objectList;
	void OnDestroy() {
		if (PhotonNetwork.isMasterClient&&!GameRule.IsLvlChanging) {
			if(objectList.Length>0){
				PhotonNetwork.Instantiate(objectList[(int)(UnityEngine.Random.value*objectList.Length)].name,transform.position,transform.rotation,0);
			}
			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
