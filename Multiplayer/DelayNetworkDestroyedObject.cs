using UnityEngine;
using System;


public class DelayNetworkDestroyedObject : MonoBehaviour {

	public float timeLimit=10.0f;
	protected float timer = 0.0f;
	
	void Update () {
		timer += Time.deltaTime;
		if (timer > timeLimit) {
			if(PhotonNetwork.isMasterClient){
				SendMessage("RequestKillMe", SendMessageOptions.DontRequireReceiver);
			}
		}
		
	}

}